using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PasteIntoFile {
    public class MasterForm : Form {

        public bool DarkMode = false;
        public Color TextColor = Color.Black;


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

        [DllImport("dwmapi.dll", PreserveSig = true)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref bool attrValue, int attrSize);

        public static int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    }
}
