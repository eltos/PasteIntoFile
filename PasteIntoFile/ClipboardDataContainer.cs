using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PasteIntoFile {

    public enum Type {
        IMAGE, HTML, CSV, SYLK, DIF, RTF, URL, FILES,
        TEXT // text is last since it catches all unknown extensions
    }
    
    public static class TypeMethods {
        /// <summary>
        /// Returns true if the format is text like, i.e. text from clipboard can be saved as this format
        /// </summary>
        public static bool IsLikeText(this Type f) {
            return new[] { Type.TEXT, Type.HTML, Type.CSV, Type.SYLK, Type.DIF, Type.RTF}.Contains(f);
        }
        
        public static string[] Extensions(this Type f) {
            switch (f) {
                case Type.IMAGE:
                    return new[] { "png", "bpm", "emf", "gif", "ico", "jpg", "tif", "wmf" };
                case Type.HTML:
                    return new[] { "html", "htm" };
                case Type.CSV:
                    return new[] { "csv", "tsv", "tab" };
                case Type.SYLK:
                    return new[] { "slk" };
                case Type.DIF:
                    return new[] { "dif" };
                case Type.RTF:
                    return new[] { "rtf" };
                case Type.URL:
                    return new[] { "url" };
                case Type.TEXT:
                    return new[] { "txt", "bat", "java", "js", "json", "cpp", "cs", "css", "csv", "md", "html", "php", "ps1", "py" };
                case Type.FILES:
                    return Array.Empty<string>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(f), f, null);
            }
        }

        public static string DefaultExtension(this Type f) {
            var ext = f.Extensions();
            return ext.Length > 0 ? ext.First() : "";
        }

        public static Type FromExtension(string ext) {
            foreach (Type f in Enum.GetValues(typeof(Type))) {
                if (f.Extensions().Contains(ext))
                    return f;
            }
            return Type.TEXT;
        }

    }
    
    
    /// <summary>
    /// Class to hold all supported clipboard data
    /// and provide concise methods to access it
    /// </summary>
    public class ClipboardDataContainer {
        
        public DateTime Timestamp;
        public readonly Dictionary<Type, object> Data = new Dictionary<Type, object>();
        
        public string Text => Data.ContainsKey(Type.TEXT) ? Data[Type.TEXT] as string : null;
        public string Html => Data.ContainsKey(Type.HTML) ? Data[Type.HTML] as string : null;
        public string Csv => Data.ContainsKey(Type.CSV) ? Data[Type.CSV] as string : null;
        public string Sylk => Data.ContainsKey(Type.SYLK) ? Data[Type.SYLK] as string : null;
        public string Dif => Data.ContainsKey(Type.DIF) ? Data[Type.DIF] as string : null;
        public string Rtf => Data.ContainsKey(Type.RTF) ? Data[Type.RTF] as string : null;
        public Image Image => Data.ContainsKey(Type.IMAGE) ? Data[Type.IMAGE] as Image : null;
        public StringCollection Files => Data.ContainsKey(Type.FILES) ? Data[Type.FILES] as StringCollection : null;
        public string TextUrl => Text != null && Uri.IsWellFormedUriString(Text.Trim(), UriKind.RelativeOrAbsolute) ? Text.Trim() : null;

        public bool Has(Type type) {
            return this[type] != null;
        }
        
        public object this[Type t] => Data.ContainsKey(t) ? Data[t] : t == Type.URL ? TextUrl : null;


        public bool HasDataThatCanBeSaveAs(Type type) {
            return Has(type) ||
                   Has(Type.TEXT) && type.IsLikeText(); // allow to save text as html, csv, etc.
        }

        /// <summary>
        /// Determines the primary type of data in this container according to a custom priorization order
        /// </summary>
        public Type? PrimaryType() {
            if (Has(Type.IMAGE))
                return Type.IMAGE;
            if (Has(Type.TEXT))
                return Type.TEXT;
            foreach (Type t in Enum.GetValues(typeof(Type))) {
                if (Has(t))
                    return t;
            }
            return null;
        }

        public static ClipboardDataContainer fromCliboardData() {
            var container = new ClipboardDataContainer();
            container.Timestamp = DateTime.Now;
            
            // https://docs.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats
            if (Clipboard.ContainsImage())
                container.Data.Add(Type.IMAGE, Clipboard.GetImage());
            if (Clipboard.ContainsData(DataFormats.Html))
                container.Data.Add(Type.HTML, readClipboardHtml());
            if (Clipboard.ContainsText())
                container.Data.Add(Type.TEXT, Clipboard.GetText());
            if (Clipboard.ContainsFileDropList())
                container.Data.Add(Type.FILES, Clipboard.GetFileDropList());
            if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
                container.Data.Add(Type.CSV, readClipboardString(DataFormats.CommaSeparatedValue));
            if (Clipboard.ContainsData(DataFormats.SymbolicLink))
                container.Data.Add(Type.SYLK, readClipboardString(DataFormats.SymbolicLink));
            if (Clipboard.ContainsData(DataFormats.Rtf))
                container.Data.Add(Type.RTF, readClipboardString(DataFormats.Rtf));
            if (Clipboard.ContainsData(DataFormats.Dif))
                container.Data.Add(Type.DIF, readClipboardString(DataFormats.Dif));

            return container;
        }
        
        private static string readClipboardHtml() {
            var content = Clipboard.GetText(TextDataFormat.Html);
            Match match = Regex.Match(content, @"StartHTML:(?<startHTML>\d*).*EndHTML:(?<endHTML>\d*)", RegexOptions.Singleline);
            if (match.Success) {
                var startHTML = Math.Max(int.Parse(match.Groups["startHTML"].Value), 0);
                var endHTML = Math.Min(int.Parse(match.Groups["endHTML"].Value), content.Length);
                return content.Substring(startHTML, endHTML-startHTML);
            }
            return null;
        }

        private static string readClipboardString(string format) {
            var data = Clipboard.GetData(format);
            if (data is string str)
                return str;
            if (data is MemoryStream stream)
                return new StreamReader(stream).ReadToEnd().TrimEnd('\0');
            return null;
        }

    }
}