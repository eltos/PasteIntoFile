using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace PasteIntoFile
{
    public static class ExplorerUtil
    {


        /// <summary>
        /// Get path of active or only windows explorer window
        /// </summary>
        /// <returns>path string or null</returns>
        public static string GetActiveExplorerPath()
        {
            return GetExplorerPath(GetActiveExplorer());
        }

        private static string GetExplorerPath(SHDocVw.InternetExplorer explorer)
        {
            return (explorer?.Document as Shell32.IShellFolderViewDual2)?.Folder.Items().Item().Path;
        }

        private static SHDocVw.InternetExplorer GetActiveExplorer()
        {
            // modified from https://stackoverflow.com/a/5708578/13324744
            IntPtr handle = GetForegroundWindow();
            var shellWindows = new SHDocVw.ShellWindows();
            foreach (SHDocVw.InternetExplorer window in shellWindows)
            {
                if (window.HWND == (int) handle || shellWindows.Count == 1)
                {
                    return window;
                }
            }
            return null;
        } 
        
        
        
        /// <summary>
        /// Request file name edit by user in active explorer path
        /// </summary>
        /// <param name="filePath">Path of file to select/edit</param>
        /// <param name="edit">can be set to false to select only (without entering edit mode)</param>
        public static void RequestFilenameEdit(string filePath, bool edit = true)
        {
            filePath = Path.GetFullPath(filePath);
            var dirPath = Path.GetDirectoryName(filePath);
            IntPtr hwnd = ILCreateFromPathW(filePath);

            // code below thankfully taken from https://stackoverflow.com/a/8682999/13324744
            IntPtr folder = PathToAbsolutePIDL(hwnd, dirPath);
            IntPtr file = PathToAbsolutePIDL(hwnd, filePath);
            try
            {
                SHOpenFolderAndSelectItems(folder, 1, new[] { file }, edit ? 1 : 0);
            }
            finally
            {
                ILFree(folder);
                ILFree(file);
            }
        }

        private static IntPtr GetShellFolderChildrenRelativePIDL(IntPtr hwnd, IShellFolder parentFolder, string displayName)
        {
            IBindCtx bindCtx;
            CreateBindCtx(0, out bindCtx);
            uint pchEaten;
            uint pdwAttributes = 0;
            IntPtr ppidl;
            parentFolder.ParseDisplayName(hwnd, bindCtx, displayName, out pchEaten, out ppidl, ref pdwAttributes);
            return ppidl;
        }

        private static IntPtr PathToAbsolutePIDL(IntPtr hwnd, string path)
        {
            IShellFolder desktopFolder;
            SHGetDesktopFolder(out desktopFolder);
            return GetShellFolderChildrenRelativePIDL(hwnd, desktopFolder, path);
        }
        
        
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        
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
        internal interface IShellFolder
        {
            void ParseDisplayName(IntPtr hwnd, IBindCtx pbc, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);
            // NOTE: we declared only what we needed...
        }


    }
}