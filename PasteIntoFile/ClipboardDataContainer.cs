using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PasteIntoFile {
    
    /// <summary>
    /// Class to hold all supported clipboard data
    /// and provide concise methods to access it
    /// </summary>
    public class ClipboardDataContainer {
        
        public DateTime timestamp;
        public string text;
        public string html;
        public Image image;
        public StringCollection files;
        public string textUrl => text != null && Uri.IsWellFormedUriString(text.Trim(), UriKind.RelativeOrAbsolute) ? text.Trim() : null;

        public bool hasImage => image != null;
        public bool hasHtml => html != null;
        public bool hasText => text != null;
        public bool hasTextUrl => textUrl != null;
        public bool hasFiles => files != null;
        
        public static ClipboardDataContainer fromCliboardData() {
            var container = new ClipboardDataContainer();
            container.timestamp = DateTime.Now;
            if (Clipboard.ContainsImage())
                container.image = Clipboard.GetImage();
            if (Clipboard.ContainsData(DataFormats.Html))
                container.html = readHtmlClipboard();
            if (Clipboard.ContainsText())
                container.text = Clipboard.GetText();
            if (Clipboard.ContainsFileDropList())
                container.files = Clipboard.GetFileDropList();
            
            return container;
        }
        
        private static string readHtmlClipboard() {
            var content = Clipboard.GetText(TextDataFormat.Html);
            Match match = Regex.Match(content, @"StartHTML:(?<startHTML>\d*).*EndHTML:(?<endHTML>\d*)", RegexOptions.Singleline);
            if (match.Success) {
                var startHTML = int.Parse(match.Groups["startHTML"].Value);
                var endHTML = int.Parse(match.Groups["endHTML"].Value);
                return content.Substring(startHTML, endHTML-startHTML);
            }
            return null;
        }

    }
}