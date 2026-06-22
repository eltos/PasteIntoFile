using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using SHDocVw;
using Shell32;

namespace PasteIntoFile {
    public static class ExplorerUtil {

        private static readonly Guid SID_STopLevelBrowser = new Guid("4C96BE40-915C-11CF-99D3-00AA004AE837");

        /// <summary>
        /// Get path of active or only windows explorer window
        /// </summary>
        /// <returns>path string or null</returns>
        public static string GetActiveExplorerPath() {
            return GetExplorerPath(GetActiveExplorer());
        }

        /// <summary>
        /// Heuristic method to get the path of a shell window
        /// </summary>
        /// <param name="explorer">File Explorer or Desktop shell window</param>
        /// <returns></returns>
        private static string GetExplorerPath(InternetExplorer explorer) {
            if (explorer == null) {
                return null;
            }

            // check special case of Desktop
            if (explorer == GetDesktop()) {
                return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            // Windows 11 tabbed Explorer:
            // ShellWindows may expose multiple tabs with the same top-level HWND.
            // Resolve the passed Explorer frame to the currently focused/visible tab first.
            // On older Windows versions this simply resolves to the only ShellView of that Explorer window.
            explorer = GetFocusedExplorerTab(explorer) ?? explorer;

            // Prefer the current ShellView PIDL. This also works for empty folders where Folder.Items()
            // would not provide an item from which we could infer the parent directory.
            var shellViewPath = GetPathFromShellView(explorer);
            if (!string.IsNullOrEmpty(shellViewPath)) {
                return shellViewPath;
            }

            // Try folder item path
            // this is subjected to fail for empty folders and some virtual folders (e.g. Desktop)
            var items = (explorer?.Document as IShellFolderViewDual)?.Folder?.Items();
            if (items != null) {
                foreach (FolderItem item in items) {
                    var path = Path.GetDirectoryName(item?.Path);
                    if (path != null) {
                        return path;
                    }
                }
            }

            // Use LocationURL attribute
            if (!string.IsNullOrEmpty(explorer?.LocationURL)) {
                var path = new Uri(explorer.LocationURL).LocalPath;
                // Unfortunately LocationURL contains a percent-encoded UNC path, even though that is not valid anymore according to
                // https://learn.microsoft.com/en-us/troubleshoot/windows-client/networking/url-encoding-unc-paths-not-url-decoded
                // The UNC encoding uses the ancient Windows-1251 encoding, so a simple UrlDecode fails (e.g. %F6 -> ? instead of ö)
                // Therefore we have to manually replace %XX with the corresponding character in Windows-1251
                // Note that this is subjected to fail in the unlikely case that the folder name actually contains e.g. "%F6" since windows
                // fails to properly encode that as "%25F6" in the LocationURL
                var decoded_path = "";
                for (var i = 0; i < path.Length;) {
                    decoded_path += Uri.HexUnescape(path, ref i);
                }
                return decoded_path;
            }

            return null;
        }

        /// <summary>
        /// Heuristic method to get the currently focused or visible tab of a windows explorer window,
        /// since ShellWindows may expose multiple tabs with the same top-level HWND.
        /// </summary>
        /// <param name="explorer"></param>
        /// <returns></returns>
        private static InternetExplorer GetFocusedExplorerTab(InternetExplorer explorer) {
            if (explorer == null) {
                return null;
            }

            var focusedHwnd = GetFocusedWindow(new IntPtr(explorer.HWND));
            try {
                InternetExplorer hitTestVisibleTab = null;
                InternetExplorer fallbackVisibleTab = null;
                var shellWindows = new ShellWindows();
                foreach (InternetExplorer window in shellWindows) {
                    if (window == null || window.HWND != explorer.HWND) {
                        continue;
                    }

                    var viewHwnd = GetShellViewWindow(window);
                    if (viewHwnd == IntPtr.Zero) {
                        continue;
                    }

                    // Best case: keyboard focus is inside the ShellView of this tab.
                    if (focusedHwnd != IntPtr.Zero &&
                        (focusedHwnd == viewHwnd || IsChild(viewHwnd, focusedHwnd))) {
                        return window;
                    }

                    // Important for address bar, navigation pane and search box focus:
                    // The focused HWND is outside the ShellView, so we cannot identify the active tab by focus.
                    // Inactive tab ShellViews may still report IsWindowVisible == true, therefore verify that
                    // the ShellView is actually hit-test visible at its screen position.
                    if (hitTestVisibleTab == null && IsWindowHitTestVisible(viewHwnd)) {
                        hitTestVisibleTab = window;
                    }

                    // Last-resort fallback for older Explorer implementations.
                    if (fallbackVisibleTab == null && IsWindowVisible(viewHwnd)) {
                        fallbackVisibleTab = window;
                    }
                }

                return hitTestVisibleTab ?? fallbackVisibleTab;

            } catch {
                // Fall back to the originally supplied Explorer object.
                return explorer;
            }

        }

        private static IntPtr GetFocusedWindow(IntPtr hwnd) {
            uint processId;
            var threadId = GetWindowThreadProcessId(hwnd, out processId);
            if (threadId == 0) {
                return IntPtr.Zero;
            }

            var info = new GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(typeof(GUITHREADINFO));

            return GetGUIThreadInfo(threadId, ref info) ? info.hwndFocus : IntPtr.Zero;
        }

        private static IntPtr GetShellViewWindow(InternetExplorer explorer) {
            var shellView = GetShellView(explorer);
            if (shellView == null) {
                return IntPtr.Zero;
            }

            try {
                IntPtr hwnd;
                return shellView.GetWindow(out hwnd) == 0 ? hwnd : IntPtr.Zero;
            } catch {
                return IntPtr.Zero;
            } finally {
                Marshal.ReleaseComObject(shellView);
            }
        }

        private static string GetPathFromShellView(InternetExplorer explorer) {
            var shellView = GetShellView(explorer);
            if (shellView == null) {
                return null;
            }

            try {
                var folderView = shellView as IFolderView;
                if (folderView == null) {
                    return null;
                }

                var iid = typeof(IPersistFolder2).GUID;
                IntPtr folderPtr;
                if (folderView.GetFolder(ref iid, out folderPtr) != 0 || folderPtr == IntPtr.Zero) {
                    return null;
                }

                IPersistFolder2 persistFolder = null;
                try {
                    persistFolder = (IPersistFolder2)Marshal.GetObjectForIUnknown(folderPtr);

                    IntPtr pidl;
                    if (persistFolder.GetCurFolder(out pidl) != 0 || pidl == IntPtr.Zero) {
                        return null;
                    }

                    try {
                        var path = new StringBuilder(260);
                        return SHGetPathFromIDListW(pidl, path) ? path.ToString() : null;
                    } finally {
                        ILFree(pidl);
                    }
                } finally {
                    if (persistFolder != null) {
                        Marshal.ReleaseComObject(persistFolder);
                    }

                    Marshal.Release(folderPtr);
                }
            } catch {
                return null;
            } finally {
                Marshal.ReleaseComObject(shellView);
            }
        }

        private static IShellView GetShellView(InternetExplorer explorer) {
            if (explorer == null) {
                return null;
            }

            object browserObject = null;

            try {
                var serviceProvider = explorer as IComServiceProvider;
                if (serviceProvider == null) {
                    return null;
                }

                var serviceId = SID_STopLevelBrowser;
                var browserId = typeof(IShellBrowser).GUID;

                if (serviceProvider.QueryService(ref serviceId, ref browserId, out browserObject) != 0 || browserObject == null) {
                    return null;
                }

                var shellBrowser = (IShellBrowser)browserObject;

                IShellView shellView;
                return shellBrowser.QueryActiveShellView(out shellView) == 0 ? shellView : null;
            } catch {
                return null;
            } finally {
                if (browserObject != null) {
                    Marshal.ReleaseComObject(browserObject);
                }
            }
        }

        private static bool IsWindowHitTestVisible(IntPtr hwnd) {
            if (hwnd == IntPtr.Zero || !IsWindowVisible(hwnd)) {
                return false;
            }

            RECT rect;
            if (!GetWindowRect(hwnd, out rect)) {
                return false;
            }

            if (rect.right <= rect.left || rect.bottom <= rect.top) {
                return false;
            }

            // Use a few sample points instead of only the exact center. The center can occasionally
            // be covered by an overlay, an empty area, a scrollbar, or another child control.
            var points = new[] {
                new POINT((rect.left + rect.right) / 2, (rect.top + rect.bottom) / 2),
                new POINT(rect.left + Math.Max(1, (rect.right - rect.left) / 4), (rect.top + rect.bottom) / 2),
                new POINT(rect.right - Math.Max(1, (rect.right - rect.left) / 4), (rect.top + rect.bottom) / 2),
                new POINT((rect.left + rect.right) / 2, rect.top + Math.Max(1, (rect.bottom - rect.top) / 4)),
                new POINT((rect.left + rect.right) / 2, rect.bottom - Math.Max(1, (rect.bottom - rect.top) / 4))
            };

            foreach (var point in points) {
                var hitHwnd = WindowFromPoint(point);

                if (hitHwnd == hwnd || IsChild(hwnd, hitHwnd)) {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Get path of file selected in active or only windows explorer window
        /// </summary>
        /// <returns>file path string or null</returns>
        public static FolderItems GetActiveExplorerSelectedFiles() {
            return GetSelectedFiles(GetActiveExplorer());
        }

        private static FolderItems GetSelectedFiles(InternetExplorer explorer) {
            return (explorer?.Document as IShellFolderViewDual2)?.SelectedItems();
        }

        /// <summary>
        /// Get a reference to the Desktop
        /// </summary>
        /// <returns></returns>
        private static InternetExplorer GetDesktop() {
            const int SWC_DESKTOP = 0x00000008;
            const int SWFO_NEEDDISPATCH = 0x00000001;
            object oNull = null;
            var win = new ShellWindows();
            return win.FindWindowSW(ref oNull, ref oNull, SWC_DESKTOP, out _, SWFO_NEEDDISPATCH) as InternetExplorer;
        }

        /// <summary>
        /// Get a reference to the currently focussed windows explorer or desktop
        /// </summary>
        /// <returns></returns>
        private static InternetExplorer GetActiveExplorer() {

            IntPtr handle = GetForegroundWindow();

            // check if it's one of the open file explorer windows
            var shellWindows = new ShellWindows();
            foreach (InternetExplorer window in shellWindows) {
                if (window.HWND == (int)handle) {
                    return GetFocusedExplorerTab(window) ?? window;
                }
            }

            // check if it's the desktop
            var desktop = GetDesktop();
            if (desktop != null) {
                if (desktop.HWND == (int)handle) {
                    return desktop;
                }

                // When the user changes the wallpaper, the handle no longer match, therefore
                // use heuristic from https://stackoverflow.com/a/17712961/13324744
                StringBuilder className = new StringBuilder(256);
                if (GetClassName(handle.ToInt32(), className, className.Capacity) > 0) {
                    if (className.ToString() == "WorkerW") {
                        return desktop;
                    }
                }
            }

            // default to the only open window
            if (shellWindows.Count == 1) {
                return shellWindows.Item(0);
            }

            return null;
        }

        public static event EventHandler FilenameEditComplete;

        /// <summary>
        /// Searches the file with given path in the given shell window and selects it if found
        /// </summary>
        /// <param name="window">The shell window</param>
        /// <param name="path">The path of the file to select</param>
        /// <param name="edit">Select in edit mode if true, otherwise just select</param>
        private static void SelectFileInWindow(InternetExplorer window, string path, bool edit = true) {
            if (!(window?.Document is IShellFolderViewDual view)) return;

            var selected = false;
            void SelectFile() {
                foreach (FolderItem folderItem in view.Folder.Items()) {
                    if (!selected && folderItem.Path == path) {
                        selected = true;
                        SetForegroundWindow((IntPtr)window.HWND);
                        // https://docs.microsoft.com/en-us/windows/win32/shell/shellfolderview-selectitem
                        view.SelectItem(folderItem, 16 /* focus it, */ + 8 /* ensure it's visible, */
                                                                       + 4 /* deselect all other and */
                                                                       + (edit ? 3 : 1) /* select or edit */);
                        FilenameEditComplete?.Invoke(null, EventArgs.Empty);
                        return;
                    }
                }
            }

            // refresh folder to make sure the new item is available
            window.DocumentComplete += (object disp, ref object url) => SelectFile();
            window.Refresh();
            // try it anyways after 1s in case event is not called (such as for desktop)
            Task.Delay(new TimeSpan(0, 0, 1)).ContinueWith(o => SelectFile());
        }

        /// <summary>
        /// Request file name edit by user in active explorer path.
        /// This method will return immediately, and the FilenameEditComplete event handler
        /// will be called asynchronously on success.
        /// </summary>
        /// <param name="filePath">Path of file to select/edit</param>
        /// <param name="mayChangeFocus">Whether focus may be changed to a different explorer window if required</param>
        /// <param name="mayOpenNew">Whether a new explorer window may be opened if required</param>
        /// <param name="edit">can be set to false to select only (without entering edit mode)</param>
        /// <returns>True if the request was scheduled, false otherwise</returns>
        public static bool AsyncRequestFilenameEdit(string filePath, bool mayChangeFocus = true, bool mayOpenNew = true, bool edit = true) {
            filePath = Path.GetFullPath(filePath);
            var dirPath = Path.GetDirectoryName(filePath);

            // check focussed shell window (or Desktop) first
            var focussedWindow = GetActiveExplorer();
            if (GetExplorerPath(focussedWindow) == dirPath) {
                SelectFileInWindow(focussedWindow, filePath, edit);
                return true;
            }

            // then check other open shell windows
            // (but not Desktop, since we cannot bring it to foreground)
            if (mayChangeFocus) {
                var shellWindows = new ShellWindows();
                foreach (InternetExplorer window in shellWindows) {
                    if (GetExplorerPath(window) == dirPath) {
                        SelectFileInWindow(window, filePath, edit);
                        return true;
                    }
                }

                // or open a new shell window
                if (mayOpenNew) {
                    IntPtr file;
                    SHParseDisplayName(filePath, IntPtr.Zero, out file, 0, out _);
                    try {
                        SHOpenFolderAndSelectItems(file, 0, null, edit ? 1 : 0);
                        Task.Run(() => FilenameEditComplete?.Invoke(null, EventArgs.Empty)); // call asynchronously
                        return true;
                    } finally {
                        ILFree(file);
                    }
                }

            }
            return false;
        }

        /// <summary>
        /// Move a path to the recycling bind
        /// </summary>
        /// <param name="path">Path to recycle</param>
        public static void MoveToRecycleBin(string path) {
            Shell shell = new Shell();
            Folder recyclingBin = shell.NameSpace(ShellSpecialFolderConstants.ssfBITBUCKET);
            recyclingBin.MoveHere(path);
        }


        private static IntPtr PathToPidl(string path, bool special = false) {
            if (special) {
                // Will use special paths, e.g. "::{374DE290-123F-4565-9164-39C4925E467B}" for Downloads
                IntPtr fileSpecial;
                SHParseDisplayName(path, IntPtr.Zero, out fileSpecial, 0, out _);
                return fileSpecial;
            }

            // Will use full paths, e.g. "C:\Users\User\Downloads" for Downloads
            // https://docs.microsoft.com/en-us/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellfolder-parsedisplayname
            SHGetDesktopFolder(out IShellFolder desktopFolder);
            desktopFolder.ParseDisplayName(IntPtr.Zero, null, path, out _, out var ppidl, 0);
            return ppidl;
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetClassName(int hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("shell32.dll", SetLastError = true)]
        public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext,
            [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

        [DllImport("shell32.dll")]
        private static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, IntPtr[] apidl, int dwFlags);

        [DllImport("shell32.dll")]
        private static extern void ILFree(IntPtr pidl);

        [DllImport("shell32.dll")]
        private static extern int SHGetDesktopFolder(out IShellFolder ppshf);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SHGetPathFromIDListW(IntPtr pidl, StringBuilder pszPath);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT {
            public int x;
            public int y;

            public POINT(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GUITHREADINFO {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public RECT rcCaret;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellFolder {
            void ParseDisplayName(IntPtr hwnd, IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);
            // NOTE: we declared only what we needed...
        }

        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IComServiceProvider {
            [PreserveSig]
            int QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
        }

        [ComImport, Guid("000214E2-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellBrowser {
            [PreserveSig]
            int GetWindow(out IntPtr phwnd);

            [PreserveSig]
            int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);

            [PreserveSig]
            int InsertMenusSB(IntPtr hmenuShared, IntPtr lpMenuWidths);

            [PreserveSig]
            int SetMenuSB(IntPtr hmenuShared, IntPtr holemenuRes, IntPtr hwndActiveObject);

            [PreserveSig]
            int RemoveMenusSB(IntPtr hmenuShared);

            [PreserveSig]
            int SetStatusTextSB(IntPtr pszStatusText);

            [PreserveSig]
            int EnableModelessSB([MarshalAs(UnmanagedType.Bool)] bool fEnable);

            [PreserveSig]
            int TranslateAcceleratorSB(IntPtr pmsg, ushort wID);

            [PreserveSig]
            int BrowseObject(IntPtr pidl, uint wFlags);

            [PreserveSig]
            int GetViewStateStream(uint grfMode, out IStream ppStrm);

            [PreserveSig]
            int GetControlWindow(uint id, out IntPtr phwnd);

            [PreserveSig]
            int SendControlMsg(uint id, uint uMsg, IntPtr wParam, IntPtr lParam, out IntPtr pret);

            [PreserveSig]
            int QueryActiveShellView(out IShellView ppshv);

            [PreserveSig]
            int OnViewWindowActive(IShellView pshv);

            [PreserveSig]
            int SetToolbarItems(IntPtr lpButtons, uint nButtons, uint uFlags);
        }

        [ComImport, Guid("000214E3-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellView {
            [PreserveSig]
            int GetWindow(out IntPtr phwnd);

            [PreserveSig]
            int ContextSensitiveHelp([MarshalAs(UnmanagedType.Bool)] bool fEnterMode);
        }

        [ComImport, Guid("CDE725B0-CCC9-4519-917E-325D72FAB4CE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFolderView {
            [PreserveSig]
            int GetCurrentViewMode(out uint pViewMode);

            [PreserveSig]
            int SetCurrentViewMode(uint ViewMode);

            [PreserveSig]
            int GetFolder(ref Guid riid, out IntPtr ppv);
        }

        [ComImport, Guid("1AC3D9F0-175C-11D1-95BE-00609797EA4F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFolder2 {
            [PreserveSig]
            int GetClassID(out Guid pClassID);

            [PreserveSig]
            int Initialize(IntPtr pidl);

            [PreserveSig]
            int GetCurFolder(out IntPtr ppidl);
        }
    }
}
