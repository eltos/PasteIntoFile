using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PasteIntoFile.Properties;

namespace PasteIntoFile {

    /// <summary>
    /// This is the base class to hold clipboard contents, metadata, and perform actions with it
    /// </summary>
    public abstract class BaseContent {
        
        /// <summary>
        /// List of known file extensions for this content type
        /// The first extension is considered the default
        /// </summary>
        public abstract string[] Extensions { get; }
        
        /// <summary>
        /// A friendly description of the contents
        /// </summary>
        public abstract string Description { get; }
        
        /// <summary>
        /// The actual data content
        /// </summary>
        protected object Data;

        /// <summary>
        /// The default extension for this content type
        /// </summary>
        public string DefaultExtension {
            get {
                var ext = Extensions;
                return ext.Length > 0 ? ext.First() : "";
            }
        }

        /// <summary>
        /// Saves the content to a file
        /// </summary>
        /// <param name="path">Full path where to save (incl. filename and extension)</param>
        /// <param name="extension">format to use for saving</param>
        public abstract void SaveAs(string path, string extension);

    }

    
    public class ImageContent : BaseContent {
        public ImageContent(Image image) {
            Data = image;
        }
        public Image Image => Data as Image;
        public override string[] Extensions => new[] { "png", "bpm", "emf", "gif", "ico", "jpg", "tif", "wmf" };
        public override string Description => string.Format(Resources.str_preview_image, Image.Width, Image.Height);
        public override void SaveAs(string path, string extension) {
            ImageFormat imageFormat;
            switch (extension) {
                case "bpm": imageFormat = ImageFormat.Bmp; break;
                case "emf": imageFormat = ImageFormat.Emf; break;
                case "gif": imageFormat = ImageFormat.Gif; break;
                case "ico": imageFormat = ImageFormat.Icon; break;
                case "jpg": imageFormat = ImageFormat.Jpeg; break;
                case "tif": imageFormat = ImageFormat.Tiff; break;
                case "wmf": imageFormat = ImageFormat.Wmf; break;
                default: imageFormat = ImageFormat.Png; break;
            }
            Image.Save(path, imageFormat);
        }
    }
    

    public abstract class TextLikeContent : BaseContent {
        public TextLikeContent(string text) {
            Data = text;
        }
        public string Text => Data as string;
        protected readonly Encoding Encoding = new UTF8Encoding(false); // omit unnecessary BOM bytes
        public override void SaveAs(string path, string extension) {
            File.WriteAllText(path, Text, Encoding);
        }
    }

    
    public class TextContent : TextLikeContent {
        public TextContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "txt", "md", "log", "bat", "ps1", "java", "js", "cpp", "cs", "py", "css", "html", "php", "json", "csv"};
        public override string Description => string.Format(Resources.str_preview_text, Text.Length, Text.Split('\n').Length);
    }

    
    public class HtmlContent : TextLikeContent {
        public HtmlContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "html", "htm", "xhtml" };
        public override string Description => Resources.str_preview_html;
        public override void SaveAs(string path, string extension) {
            var html = Text;
            if (!html.StartsWith("<!DOCTYPE html>"))
                html = "<!DOCTYPE html>\n" + html;
            File.WriteAllText(path, html, Encoding);
        }
    }

    
    public class CsvContent : TextLikeContent {
        public CsvContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "csv", "tsv", "tab" };
        public override string Description => Resources.str_preview_csv;
    }
    

    public class SylkContent : TextLikeContent {
        public SylkContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "slk" };
        public override string Description => Resources.str_preview_sylk;
    }
    

    public class DifContent : TextLikeContent {
        public DifContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "dif" };
        public override string Description => Resources.str_preview_dif;
    }
    

    public class RtfContent : TextLikeContent {
        public RtfContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "rtf" };
        public override string Description => Resources.str_preview_rtf;
    }
    

    public class UrlContent : TextLikeContent {
        public UrlContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "url" };
        public override string Description => Resources.str_preview_url;
        public override void SaveAs(string path, string extension) {
            File.WriteAllLines(path, new[] {
                @"[InternetShortcut]",
                @"URL=" + Text
            }, Encoding);
        }
    }
    
    
    
    
    
    /// <summary>
    /// Class to hold all supported clipboard content
    /// and provide concise methods to access it
    /// </summary>
    public class ClipboardContents {
        
        public DateTime Timestamp;
        public readonly IList<BaseContent> Contents = new List<BaseContent>();
        
        /// <summary>
        /// Return contents matching the given extension, fall back to text content if available
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public BaseContent ForExtension(string ext) {
            foreach (var content in Contents) {
                if (content.Extensions.Contains(ext))
                    return content;
            }
            // if ext is not compatible with text, return null ...
            foreach (var reserved in new BaseContent[] 
                         {new ImageContent(null), new UrlContent(null)}) {
                if (reserved.Extensions.Contains(ext))
                    return null;
            }
            // ... otherwise default to text
            return Contents.OfType<TextContent>().FirstOrDefault();
        }
        
        /// <summary>
        /// Return contents of specific type
        /// </summary>
        /// <param name="type">Any type of BaseContent or its subclasses</param>
        /// <returns></returns>
        public BaseContent ForContentType(Type type) {
            foreach (var content in Contents) {
                if (content.GetType() == type)
                    return content;
            }
            return null;
        }

        /// <summary>
        /// Determines the primary type of data in this container according to a custom prioritisation order
        /// </summary>
        public BaseContent PrimaryContent => ForContentType(typeof(ImageContent)) ??
                                             ForContentType(typeof(TextContent)) ??
                                             ForContentType(typeof(BaseContent));

        /// <summary>
        /// Static constructor to create an instance from the current clipboard data
        /// </summary>
        /// <returns></returns>
        public static ClipboardContents FromClipboard() {
            var container = new ClipboardContents {
                Timestamp = DateTime.Now
            };

            // Read all supported clipboard data
            // https://docs.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats
            
            if (Clipboard.ContainsImage())
                container.Contents.Add(new ImageContent(Clipboard.GetImage()));
            if (Clipboard.ContainsData(DataFormats.Html))
                container.Contents.Add(new HtmlContent(ReadClipboardHtml()));
            if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
                container.Contents.Add(new CsvContent(ReadClipboardString(DataFormats.CommaSeparatedValue)));
            if (Clipboard.ContainsData(DataFormats.SymbolicLink))
                container.Contents.Add(new SylkContent(ReadClipboardString(DataFormats.SymbolicLink)));
            if (Clipboard.ContainsData(DataFormats.Rtf))
                container.Contents.Add(new RtfContent(ReadClipboardString(DataFormats.Rtf)));
            if (Clipboard.ContainsData(DataFormats.Dif))
                container.Contents.Add(new DifContent(ReadClipboardString(DataFormats.Dif)));
            
            if (Clipboard.ContainsFileDropList() && !Clipboard.ContainsText())
                // save list of file paths instead
                container.Contents.Add(new TextContent(string.Join("\n", Clipboard.GetFileDropList().Cast<string>().ToList())));
            
            if (Clipboard.ContainsText() &&  Uri.IsWellFormedUriString(Clipboard.GetText().Trim(), UriKind.RelativeOrAbsolute))
                container.Contents.Add(new UrlContent(Clipboard.GetText().Trim()));

            // make sure text content comes last, as it may includes extensions from previous formats
            if (Clipboard.ContainsText())
                container.Contents.Add(new TextContent(Clipboard.GetText()));
            

            return container;
        }
        
        private static string ReadClipboardHtml() {
            var content = Clipboard.GetText(TextDataFormat.Html);
            var match = Regex.Match(content, @"StartHTML:(?<startHTML>\d*).*EndHTML:(?<endHTML>\d*)", RegexOptions.Singleline);
            if (match.Success) {
                var startHtml = Math.Max(int.Parse(match.Groups["startHTML"].Value), 0);
                var endHtml = Math.Min(int.Parse(match.Groups["endHTML"].Value), content.Length);
                return content.Substring(startHtml, endHtml-startHtml);
            }
            return null;
        }

        private static string ReadClipboardString(string format) {
            var data = Clipboard.GetData(format);
            switch (data) {
                case string str:
                    return str;
                case MemoryStream stream:
                    return new StreamReader(stream).ReadToEnd().TrimEnd('\0');
                default:
                    return null;
            }
        }

    }
}