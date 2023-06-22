using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

namespace PasteIntoFileShellExtension {

    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFilesAndFolders)]
    public class ContextEntryExtension : SharpContextMenu {

        public static bool IsFolder(string path) {
            var a = File.GetAttributes(path);
            return ((a & FileAttributes.Directory) == FileAttributes.Directory);
        }
        public static bool IsFile(string path) {
            return !IsFolder(path);
        }

        public IEnumerable<string> SelectedFolderPaths => SelectedItemPaths.Where(IsFolder).ToList();
        public IEnumerable<string> SelectedFilePaths => SelectedItemPaths.Where(IsFile).ToList();
        public int SelectedFolders => SelectedFolderPaths.Count();
        public int SelectedFiles => SelectedFilePaths.Count();
        public int SelectedItems => SelectedItemPaths.Count();


        protected override bool CanShowMenu() {
            return true;
        }

        protected override ContextMenuStrip CreateMenu() {

            //  Create the menu
            var menu = new ContextMenuStrip();

            menu.Items.Add(new ToolStripMenuItem(
                text: Resources.str_contextentry_copyfilenames,
                image: Resources.app_icon16,
                onClick: OnCopyFilenames
            ));

            return menu;
        }

        private void OnCopyFilenames(object sender, EventArgs e) {
            var files = SelectedItemPaths.ToList();
            files.Sort();
            var paths = string.Join("\n", files);

            // Copy to clipboard
            IDataObject data = new DataObject();
            data.SetData(DataFormats.Text, paths);
            data.SetData(DataFormats.UnicodeText, paths);
            string[] strArray = new string[files.Count];
            files.CopyTo(strArray, 0);
            data.SetData(DataFormats.FileDrop, true, strArray);
            Clipboard.SetDataObject(data, true);

        }

    }
}
