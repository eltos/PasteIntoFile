using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PasteIntoFile
{
    public class MasterForm : Form
    {
        
        
        public IEnumerable<Control> GetAllChild(Control control, System.Type type = null)
        {
            var controls = control.Controls.Cast<Control>();
            var enumerable = controls as Control[] ?? controls.ToArray();
            return enumerable.SelectMany(ctrl => GetAllChild(ctrl, type))
                .Concat(enumerable)
                .Where(c => type == null || type == c.GetType());
        }
    }
}