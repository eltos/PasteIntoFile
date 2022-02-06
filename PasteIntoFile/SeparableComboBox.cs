using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PasteIntoFile {
    public class SeparableComboBox : ComboBox {
        
        public static readonly object SEPARATOR = new object();
        
        public SeparableComboBox() {
            DrawMode = DrawMode.OwnerDrawVariable;
            DrawItem += DoDrawItem;
            SelectedIndexChanged += DoSelectedIndexChanged;
            MeasureItem += DoMeasureItem;
            
        }

        public void AddWithSeparator(IEnumerable<object> items) {
            if (Items.Count > 0) 
                Items.Add(SEPARATOR);
            Items.AddRange(items.ToArray());
            
            UpdateDropDownHeight();
        }

        public object[] ItemArray() {
            var array = new object[Items.Count];
            Items.CopyTo(array, 0);
            return array;
        }

        private void DoMeasureItem(object sender, MeasureItemEventArgs e) {
            if (Items[e.Index] == SEPARATOR)
                e.ItemHeight = 5;
        }

        private void UpdateDropDownHeight() {
            var height = 0;
            foreach (var i in ItemArray()) {
                height += i == SEPARATOR ? 5 : ItemHeight;
            }
            DropDownHeight = 2 + Math.Min(500, Math.Max(ItemHeight, height));
        }

        private void DoSelectedIndexChanged(object sender, EventArgs e) {
            if (SelectedIndex > 0 && Items[SelectedIndex] == SEPARATOR)
                SelectedIndex++;
        }

        private void DoDrawItem(object sender, DrawItemEventArgs e) {
            var item = Items[e.Index];
            if (item == SEPARATOR)
                e.Graphics.DrawLine(Pens.DarkGray,
                    new Point(e.Bounds.Left + 2, (e.Bounds.Top + e.Bounds.Bottom)/2),
                    new Point(e.Bounds.Right - 3, (e.Bounds.Top + e.Bounds.Bottom)/2));
            else {
                e.DrawBackground();
                TextRenderer.DrawText(e.Graphics, item.ToString(), e.Font, e.Bounds, e.ForeColor, TextFormatFlags.Left);
                e.DrawFocusRectangle();
            }
        }
    }
}