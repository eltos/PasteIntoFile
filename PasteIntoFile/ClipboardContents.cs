using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Add the content to the data object
        /// </summary>
        /// <param name="data">The data object to place contents to</param>
        public abstract void AddTo(IDataObject data);

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
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Bitmap, Image);
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
        public override string[] Extensions => new[] { "txt", "md", "log", "bat", "ps1", "java", "js", "cpp", "cs", "py", "css", "html", "php", "json", "csv" };
        public override string Description => string.Format(Resources.str_preview_text, Text.Length, Text.Split('\n').Length);
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Text, Text);
        }
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
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Html, Text);
        }
    }


    public class CsvContent : TextLikeContent {
        public CsvContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "csv", "tsv", "tab" };
        public override string Description => Resources.str_preview_csv;
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.CommaSeparatedValue, Text);
        }
    }


    public class SylkContent : TextLikeContent {
        public SylkContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "slk" };
        public override string Description => Resources.str_preview_sylk;
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.SymbolicLink, Text);
        }
    }


    public class DifContent : TextLikeContent {
        public DifContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "dif" };
        public override string Description => Resources.str_preview_dif;
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Dif, Text);
        }
    }


    public class RtfContent : TextLikeContent {
        public RtfContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "rtf" };
        public override string Description => Resources.str_preview_rtf;
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Rtf, Text);
        }
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
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Text, Text);
        }
    }


    public class FilesContent : BaseContent {
        public FilesContent(StringCollection files) {
            Data = files;
        }
        public StringCollection Files => Data as StringCollection;
        public List<string> FileList {
            get {
                var list = Files.Cast<string>().ToList();
                list.Sort();
                return list;
            }
        }

        public override string[] Extensions => new[] { "zip", "m3u", "txt" };
        public override string Description => string.Format(Resources.str_preview_files, Files.Count);
        public override void SaveAs(string path, string extension) {
            switch (extension) {
                case "zip":
                    // TODO: since zipping can take a while depending on file size, this should show a progress to the user
                    var archive = ZipFile.Open(path, ZipArchiveMode.Create);
                    foreach (var file in Files) {
                        if ((File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory) {
                            AddToZipArchive(archive, Path.GetFileName(file), new DirectoryInfo(file));
                        } else {
                            archive.CreateEntryFromFile(file, Path.GetFileName(file));
                        }
                    }
                    break;

                case "m3u":
                case "txt":
                    File.WriteAllLines(path, FileList, new UTF8Encoding(false));
                    break;
            }
        }

        private void AddToZipArchive(ZipArchive archive, string node, DirectoryInfo dir) {
            foreach (var file in dir.GetFiles()) {
                archive.CreateEntryFromFile(file.FullName, Path.Combine(node, file.Name));
            }
            foreach (var directory in dir.GetDirectories()) {
                AddToZipArchive(archive, Path.Combine(node, directory.Name), directory);
            }
        }

        public override void AddTo(IDataObject data) {
            string[] strArray = new string[Files.Count];
            Files.CopyTo(strArray, 0);
            data.SetData(DataFormats.FileDrop, true, strArray);
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

            // Various image formats
            if (Clipboard.ContainsFileDropList()
                && Clipboard.GetFileDropList() is StringCollection files
                && files.Count == 1) {
                try { // Image as file (try&catch instead of maintaining an extension list)
                    container.Contents.Add(new ImageContent(Image.FromFile(files[0])));
                } catch (Exception e) { }
            }
            if (Clipboard.ContainsData(DataFormats.EnhancedMetafile)
                && ReadClipboardMetafile() is Image emf)
                container.Contents.Add(new ImageContent(emf));
            if (Clipboard.ContainsImage()
                && Clipboard.GetImage() is Image img)
                container.Contents.Add(new ImageContent(img));

            if (Clipboard.ContainsData(DataFormats.Html)
                && ReadClipboardHtml() is string html)
                container.Contents.Add(new HtmlContent(html));
            if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue)
                && ReadClipboardString(DataFormats.CommaSeparatedValue) is string csv)
                container.Contents.Add(new CsvContent(csv));
            if (Clipboard.ContainsData(DataFormats.SymbolicLink)
                && ReadClipboardString(DataFormats.SymbolicLink) is string lnk)
                container.Contents.Add(new SylkContent(lnk));
            if (Clipboard.ContainsData(DataFormats.Rtf)
                && ReadClipboardString(DataFormats.Rtf) is string rtf)
                container.Contents.Add(new RtfContent(rtf));
            if (Clipboard.ContainsData(DataFormats.Dif)
                && ReadClipboardString(DataFormats.Dif) is string dif)
                container.Contents.Add(new DifContent(dif));

            if (Clipboard.ContainsFileDropList() && !Clipboard.ContainsText())
                container.Contents.Add(new FilesContent(Clipboard.GetFileDropList()));

            if (Clipboard.ContainsText() && Uri.IsWellFormedUriString(Clipboard.GetText().Trim(), UriKind.RelativeOrAbsolute))
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
                return content.Substring(startHtml, endHtml - startHtml);
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

        private static Metafile ReadClipboardMetafile() {
            Metafile emf = null;
            if (OpenClipboard(IntPtr.Zero)) {
                if (IsClipboardFormatAvailable(CF_ENHMETAFILE)) {
                    var ptr = GetClipboardData(CF_ENHMETAFILE);
                    if (!ptr.Equals(IntPtr.Zero))
                        emf = new Metafile(ptr, true);
                }
                CloseClipboard();
            }

            return emf;
        }

        public static ClipboardContents FromFile(string path) {
            var container = new ClipboardContents {
                Timestamp = DateTime.Now
            };

            // if it's an image (try&catch instead of maintaining a list of supported extensions)
            try {
                container.Contents.Add(new ImageContent(Image.FromFile(path)));
            } catch (Exception e) { }

            // if it's text like (check for absence of zero byte)
            if (!LooksLikeBinaryFile(path)) {
                container.Contents.Add(new TextContent(File.ReadAllText(path)));

                string firstLine = File.ReadLines(path).First();
                if (firstLine.StartsWith("<!DOCTYPE html>")) {
                    container.Contents.Add(new HtmlContent(File.ReadAllText(path)));
                }

            }

            return container.Contents.Count > 0 ? container : null;
        }

        /// <summary>
        /// Heuristically determines if a file is binary by checking for NULL values
        /// </summary>
        /// <param name="filepath">Path to file</param>
        /// <returns>true if most likely binary</returns>
        private static bool LooksLikeBinaryFile(string filepath) {
            var stream = File.OpenRead(filepath);
            int b;
            do {
                b = stream.ReadByte();
            }
            while (b > 0);
            return b == 0;
        }

        public void CopyToClipboard(string fileDropPath = null) {
            IDataObject data = new DataObject();
            foreach (var content in Contents) {
                content.AddTo(data);
            }
            if (fileDropPath != null) data.SetData(DataFormats.FileDrop, new[] { fileDropPath });
            Clipboard.SetDataObject(data, true);
        }



        private const uint CF_METAFILEPICT = 3;
        private const uint CF_ENHMETAFILE = 14;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetClipboardData(uint format);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool IsClipboardFormatAvailable(uint format);

    }
}
