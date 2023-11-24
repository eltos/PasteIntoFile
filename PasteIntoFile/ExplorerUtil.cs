using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Shell32;

namespace PasteIntoFile {
    public static class ExplorerUtil {


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
        /// <param name="explorer">File Explorer or Desktiop shell window</param>
        /// <returns></returns>
        private static string GetExplorerPath(SHDocVw.InternetExplorer explorer) {
            // check location URL
            if (!string.IsNullOrEmpty(explorer?.LocationURL)) {
                var uri = new Uri(explorer.LocationURL);
                return uri.LocalPath;
            }
            // Fallback to folder items path, e.g. for Desktop
            var items = (explorer?.Document as IShellFolderViewDual)?.Folder?.Items();
            if (items != null) {
                foreach (FolderItem item in items) {
                    var path = Path.GetDirectoryName(item?.Path);
                    if (path != null) {
                        return path;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Get path of file selected in active or only windows explorer window
        /// </summary>
        /// <returns>file path string or null</returns>
        public static Shell32.FolderItems GetActiveExplorerSelectedFiles() {
            return GetSelectedFiles(GetActiveExplorer());
        }

        private static Shell32.FolderItems GetSelectedFiles(SHDocVw.InternetExplorer explorer) {
            return (explorer?.Document as Shell32.IShellFolderViewDual2)?.SelectedItems();
        }

        /// <summary>
        /// Get a reference to the Desktop
        /// </summary>
        /// <returns></returns>
        private static SHDocVw.InternetExplorer GetDesktop() {
            const int SWC_DESKTOP = 0x00000008;
            const int SWFO_NEEDDISPATCH = 0x00000001;
            object oNull = null;
            var win = new SHDocVw.ShellWindows();
            return win.FindWindowSW(ref oNull, ref oNull, SWC_DESKTOP, out _, SWFO_NEEDDISPATCH) as SHDocVw.InternetExplorer;
        }

        /// <summary>
        /// Get a reference to the currently focussed windows explorer or desktop
        /// </summary>
        /// <returns></returns>
        private static SHDocVw.InternetExplorer GetActiveExplorer() {

            IntPtr handle = GetForegroundWindow();

            // check if it's one of the open file explorer windows
            var shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.InternetExplorer window in shellWindows) {
                if (window.HWND == (int)handle) {
                    return window;
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
        private static void SelectFileInWindow(SHDocVw.InternetExplorer window, string path, bool edit = true) {
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
                var shellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.InternetExplorer window in shellWindows) {
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
            desktopFolder.ParseDisplayName(IntPtr.Zero, null, path, out var pchEaten, out var ppidl, 0);
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

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ILCreateFromPathW(string pszPath);

        [DllImport("shell32.dll")]
        private static extern int SHGetDesktopFolder(out IShellFolder ppshf);

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

        [ComImport, Guid("000214E6-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellFolder {
            void ParseDisplayName(IntPtr hwnd, IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);
            // NOTE: we declared only what we needed...
        }


    }
}
