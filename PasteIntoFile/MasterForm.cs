using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile {
    public class MasterForm : Form {

        public bool DarkMode = false;
        public Color TextColor = Color.Black;
        public const Int32 ALWAYS_ON_TOP = 1000;


        public IEnumerable<Control> GetAllChild(Control control, System.Type type = null) {
            var controls = control.Controls.Cast<Control>();
            var enumerable = controls as Control[] ?? controls.ToArray();
            return enumerable.SelectMany(ctrl => GetAllChild(ctrl, type))
                .Concat(enumerable)
                .Where(c => type == null || type == c.GetType());
        }

        public void BringToFrontForced() {
            WindowState = FormWindowState.Minimized;
            Show();
            BringToFront();
            WindowState = FormWindowState.Normal;
        }

        public void MakeDarkMode() {
            DarkMode = true;
            TextColor = Color.White;
            foreach (Control element in GetAllChild(this)) {
                element.ForeColor = TextColor;
                element.BackColor = element is Button ? Color.FromArgb(60, 60, 60) : Color.FromArgb(40, 40, 40);
            }
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref DarkMode, Marshal.SizeOf(DarkMode));
        }

        /// <summary>
        /// Adds an "Always on top" checkbox to the window bar context menu
        /// </summary>
        public void AllowAlwaysOnTop() {
            IntPtr MenuHandle = GetSystemMenu(Handle, false);
            InsertMenu(MenuHandle, 0, MF_BYPOSITION, ALWAYS_ON_TOP, Resources.str_always_on_top);
        }

        protected override void WndProc(ref Message msg) {
            if (msg.Msg == WM_SYSCOMMAND) {
                switch (msg.WParam.ToInt32()) {
                    case ALWAYS_ON_TOP:
                        // Toggle always on top state
                        SetAlwaysOnTop(!TopMost);
                        return;
                }
            }
            base.WndProc(ref msg);
        }

        /// <summary>
        /// Updates the form's always on top state
        /// and the corresponding checkbox in the window bar context menu
        /// </summary>
        /// <param name="topMost">Always on top or not</param>
        public void SetAlwaysOnTop(bool topMost) {
            // Set the form to always be on top
            TopMost = topMost;

            // Update the window bar context menu
            var info = new MENUITEMINFO {
                cbSize = (uint)Marshal.SizeOf(typeof(MENUITEMINFO)),
                fMask = MIIM_STATE, // mask what to be changed
                fState = TopMost ? MF_CHECKED : MF_UNCHECKED,
            };
            IntPtr MenuHandle = GetSystemMenu(Handle, false);
            SetMenuItemInfo(MenuHandle, 0, true, ref info);
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref bool attrValue, int attrSize);

        public static int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, uint wPosition, uint wFlags, uint wIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        static extern bool SetMenuItemInfo(IntPtr hMenu, uint uItem, bool fByPosition, [In] ref MENUITEMINFO lpmii);

        public const uint WM_SYSCOMMAND = 0x112;
        public const uint MF_BYPOSITION = 0x400;
        public const uint MF_CHECKED = 0x8;
        public const uint MF_UNCHECKED = 0x0;
        public const uint MIIM_STATE = 0x1;

        /// <summary>
        /// See https://learn.microsoft.com/de-de/windows/win32/api/winuser/ns-winuser-menuiteminfoa
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MENUITEMINFO {
            public uint cbSize;
            public uint fMask;
            public uint fType;
            public uint fState;
            public uint wID;
            public IntPtr hSubMenu;
            public IntPtr hbmpChecked;
            public IntPtr hbmpUnchecked;
            public IntPtr dwItemData;
            public string dwTypeData;
            public uint cch;
            public IntPtr hbmpItem;
        }

    }
}
