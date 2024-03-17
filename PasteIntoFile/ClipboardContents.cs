using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LINQtoCSV;
using PasteIntoFile.Properties;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PasteIntoFile {

    public class AppendNotSupportedException : ArgumentException { }

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
        /// <param name="append">If true, append content</param>
        public abstract void SaveAs(string path, string extension, bool append = false);

        /// <summary>
        /// Add the content to the data object to be placed in the clipboard
        /// </summary>
        /// <param name="data">The data object to place contents to</param>
        public abstract void AddTo(IDataObject data);

    }


    /// <summary>
    /// Holds image contents
    /// </summary>
    public abstract class ImageLikeContent : BaseContent {
        /// <summary>
        /// Convert the image to the format used for saving it, so it can be used for a preview
        /// </summary>
        /// <param name="extension">File extension determining the format</param>
        /// <returns>Image in target format or null if no suitable format is found</returns>
        public abstract Image ImagePreview(string extension);
    }


    public class ImageContent : ImageLikeContent {
        public ImageContent(Image image) {
            Data = image;
        }
        public Image Image => Data as Image;
        public override string[] Extensions => new[] { "png", "bmp", "gif", "jpg", "pdf", "tif" };
        public override string Description => string.Format(Resources.str_preview_image, Image.Width, Image.Height);

        public override void SaveAs(string path, string extension, bool append = false) {
            if (append)
                throw new AppendNotSupportedException();
            Image image = ImagePreview(extension);
            if (image == null)
                throw new FormatException(string.Format(Resources.str_error_cliboard_format_missmatch, extension));

            switch (extension) {
                case "pdf":
                    // convert image to ximage
                    var stream = new MemoryStream();
                    image.Save(stream, image.RawFormat);
                    stream.Position = 0;
                    XImage img = XImage.FromStream(stream);
                    // create pdf document
                    PdfDocument document = new PdfDocument();
                    document.Info.Creator = Resources.app_title;
                    PdfPage page = document.AddPage();
                    page.Width = XUnit.FromPoint(img.PointWidth);
                    page.Height = XUnit.FromPoint(img.PointHeight);
                    // insert image and save
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    gfx.DrawImage(img, 0, 0);
                    document.Save(path);
                    return;

                default:
                    image.Save(path);
                    return;
            }
        }

        /// <summary>
        /// Convert the image to the format used for saving it
        /// </summary>
        /// <param name="extension">File extension determining the format</param>
        /// <returns>Image in target format or null if no suitable format is found</returns>
        public override Image ImagePreview(string extension) {
            // Special formats with intermediate conversion types
            switch (extension.ToLower()) {
                case "pdf": extension = "png"; break;
            }
            // Find suitable codec and convert image
            foreach (var encoder in ImageCodecInfo.GetImageEncoders()) {
                if (encoder.FilenameExtension.ToLower().Contains(extension.ToLower())) {
                    var stream = new MemoryStream();
                    Image.Save(stream, encoder, null);
                    return Image.FromStream(stream);
                }
            }
            // TODO: Support conversion to EMF, WMF and ICO
            // Previously we had these in the list, but apparently these were silently saved as PNG in lack of a proper codec.

            // No suitable coded available
            return null;
        }

        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Bitmap, Image);
        }
    }

    /// <summary>
    /// Like ImageContent, but only for formats which support alpha channel
    /// </summary>
    public class TransparentImageContent : ImageContent {
        public TransparentImageContent(Image image) : base(image) { }
        public override string[] Extensions => new[] { "png", "gif", "pdf", "tif" }; // Note: gif has only alpha 100% or 0%
    }

    /// <summary>
    /// Like ImageContent, but only for formats which support animated frames
    /// </summary>
    public class AnimatedImageContent : ImageContent {
        public AnimatedImageContent(Image image) : base(image) { }
        public override string[] Extensions => new[] { "gif" };
    }

    /// <summary>
    /// A ImageLikeContent which supports vector graphics.
    /// Currently, this is tailored to Metafiles (EMF). Later, SVG, PDF, etc. might be added
    /// </summary>
    public class VectorImageContent : ImageLikeContent {
        public VectorImageContent(Metafile metafile) {
            Data = metafile;
        }
        public Metafile Metafile => Data as Metafile;
        public override string[] Extensions => new[] { "emf" };
        public override string Description => Resources.str_preview_image_vector;

        public override void SaveAs(string path, string extension, bool append = false) {
            if (append)
                throw new AppendNotSupportedException();
            switch (extension) {
                case "emf":
                    IntPtr h = Metafile.GetHenhmetafile();
                    uint size = GetEnhMetaFileBits(h, 0, null);
                    byte[] data = new byte[size];
                    GetEnhMetaFileBits(h, size, data);
                    using (FileStream w = File.Create(path)) {
                        w.Write(data, 0, checked((int)size));
                    }
                    break;

                default:
                    // fallback to save as raster image
                    new ImageContent(Metafile).SaveAs(path, extension);
                    break;
            }
        }

        /// <summary>
        /// Convert the image to the format used for saving it
        /// </summary>
        /// <param name="extension">File extension determining the format</param>
        /// <returns>Image in target format or null if no suitable format is found</returns>
        public override Image ImagePreview(string extension) {
            switch (extension.ToLower()) {
                case "emf":
                    return Metafile;

                default: // fallback to save as raster image
                    return new ImageContent(Metafile).ImagePreview(extension);
            }
        }
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.EnhancedMetafile, Metafile);
        }

        [DllImport("gdi32")]
        private static extern uint GetEnhMetaFileBits(IntPtr hemf, uint cbBuffer, byte[] lpbBuffer);
    }


    /// <summary>
    /// Class to hold SVG data
    /// </summary>
    public class SvgContent : TextLikeContent {
        public SvgContent(string xml) : base(xml) { }

        public string Xml {
            get {
                var xml = Text;
                if (!xml.StartsWith("<?xml"))
                    xml = "<?xml version=\"1.0\" encoding=\"" + Encoding.BodyName + "\"?>\n" + xml;
                return xml;
            }
        }

        public override string[] Extensions => new[] { "svg" };
        public override string Description => Resources.str_preview_svg;

        public override void SaveAs(string path, string extension, bool append = false) {
            if (append)
                throw new AppendNotSupportedException();
            switch (extension) {
                case "svg":
                default:
                    Save(path, Xml);
                    break;
            }
        }

        public override void AddTo(IDataObject data) {
            data.SetData("image/svg+xml", Stream);
        }
        public override string TextPreview(string extension) {
            return Xml;
        }
    }



    public abstract class TextLikeContent : BaseContent {
        public TextLikeContent(string text) {
            Data = text;
        }
        public string Text => Data as string;
        public Stream Stream => new MemoryStream(Encoding.GetBytes(Text));
        public static readonly Encoding Encoding = new UTF8Encoding(false); // omit unnecessary BOM bytes
        public override void SaveAs(string path, string extension, bool append = false) {
            Save(path, Text, append);
        }

        protected static void Save(string path, string text, bool append = false) {
            using (StreamWriter streamWriter = new StreamWriter(path, append))
                streamWriter.Write(EnsureNewline(text), Encoding);
        }

        public static string EnsureNewline(string text) {
            return text.TrimEnd('\n') + '\n';
        }

        /// <summary>
        /// Return a string used for preview
        /// </summary>
        /// <returns></returns>
        public abstract string TextPreview(string extension);
    }


    public class TextContent : TextLikeContent {
        public TextContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "txt", "md", "log", "bat", "ps1", "java", "js", "cpp", "cs", "py", "css", "html", "php", "json", "csv" };
        public override string Description => string.Format(Resources.str_preview_text, Text.Length, Text.Split('\n').Length);
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Text, Text);
            data.SetData(DataFormats.UnicodeText, Text);
        }
        public override string TextPreview(string extension) {
            return Text;
        }
    }


    public class HtmlContent : TextLikeContent {
        public HtmlContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "html", "htm", "xhtml" };
        public override string Description => Resources.str_preview_html;
        public override void SaveAs(string path, string extension, bool append = false) {
            var html = Text;
            if (!append && !html.StartsWith("<!DOCTYPE html>"))
                html = "<!DOCTYPE html>\n" + html;
            Save(path, html, append);
        }
        public override void AddTo(IDataObject data) {
            // prepare header
            // See https://learn.microsoft.com/windows/win32/dataxchg/html-clipboard-format
            var START = "<start_byte_of_html>";
            var STOP = "<end_byte_of_html>";
            var header = "Version:0.9\r\n" +
                         "StartHTML:" + START + "\r\n" +
                         "EndHTML:" + STOP + "\r\n" +
                         "StartFragment:" + START + "\r\n" +
                         "EndFragment:" + STOP + "\r\n";
            var bytecount = Encoding.UTF8.GetByteCount(Text);
            header = header.Replace(START, (header.Length).ToString().PadLeft(START.Length, '0'));
            header = header.Replace(STOP, (header.Length + bytecount).ToString().PadLeft(STOP.Length, '0'));

            data.SetData(DataFormats.Html, header + Text);
        }
        public override string TextPreview(string extension) {
            return Text;
        }
    }


    public class CsvContent : TextLikeContent {
        public CsvContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "csv", "tsv", "tab", "md" };
        public override string Description => Resources.str_preview_csv;
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.CommaSeparatedValue, Text);
        }

        /// <summary>
        /// Heuristically determine the (most likely) delimiter
        /// </summary>
        /// <returns></returns>
        public char Delimiter {
            get {
                var frequency = ";,|\t".ToDictionary(d => d, d => Text.Count(c => c == d));// Heuristic: most frequent delimiter
                var mostFrequent = frequency.Where(it => it.Value == frequency.Max(i => i.Value));
                return mostFrequent.First().Key; // Heuristic: first most frequent delimiter
            }
        }

        public class Row : List<DataRowItem>, IDataRow { }

        /// <summary>
        /// Parse the CVS data
        /// </summary>
        /// <returns>List of rows</returns>
        public IEnumerable<Row> Parse() {
            var context = new CsvContext();
            var readConfig = new CsvFileDescription();
            readConfig.FirstLineHasColumnNames = false;
            readConfig.SeparatorChar = Delimiter;
            var readStream = new StreamReader(new MemoryStream(readConfig.TextEncoding.GetBytes(Text)));
            return context.Read<Row>(readStream, readConfig);
        }

        /// <summary>
        /// Return a markdown table representation of the CSV data
        /// </summary>
        /// <returns>Markdown compatible string</returns>
        private string AsMarkdown() {
            var ncol = 0;

            var markdown = "";
            foreach (var row in Parse()) {
                ncol = Math.Max(ncol, row.Count);
                foreach (var item in row) {
                    markdown += "|" + (item.Value ?? "").PadRight(10);
                }
                markdown += "|\n";
            }

            var header = string.Concat(Enumerable.Repeat("|          ", ncol)) + "|\n";
            header += string.Concat(Enumerable.Repeat("|----------", ncol)) + "|\n";

            return header + markdown;
        }

        public override string TextPreview(string extension) {
            switch (extension) {
                case "md":
                    return AsMarkdown();
                default:
                    return Text;
            }
        }

        public override void SaveAs(string path, string extension, bool append = false) {
            Save(path, TextPreview(extension), append);
        }
    }


    public class GenericTextContent : TextLikeContent {
        private readonly string _format;
        public GenericTextContent(string format, string extension, string text) : base(text) {
            _format = format;
            Extensions = new[] { extension };
        }
        public override string[] Extensions { get; }
        public override string Description => Resources.str_preview;
        public override void AddTo(IDataObject data) {
            data.SetData(_format, Text);
        }
        public override string TextPreview(string extension) {
            return Text;
        }
    }


    public class UrlContent : TextLikeContent {
        public UrlContent(string text) : base(text) { }
        public override string[] Extensions => new[] { "url" };
        public override string Description => Resources.str_preview_url;
        public override void SaveAs(string path, string extension, bool append = false) {
            if (append)
                throw new AppendNotSupportedException();
            Save(path, "[InternetShortcut]\nURL=" + Text);
        }
        public override void AddTo(IDataObject data) {
            data.SetData(DataFormats.Text, Text);
        }
        public override string TextPreview(string extension) {
            return Text;
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
        public string FileListString => string.Join("\n", FileList);

        public override string[] Extensions => new[] { "zip", "m3u", "files", "txt" };
        public override string Description => string.Format(Resources.str_preview_files, Files.Count);
        public override void SaveAs(string path, string extension, bool append = false) {
            switch (extension) {
                case "zip":
                    // TODO: since zipping can take a while depending on file size, this should show a progress to the user
                    using (var archive = ZipFile.Open(path, append ? ZipArchiveMode.Update : ZipArchiveMode.Create)) {
                        foreach (var file in Files) {
                            if ((File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory) {
                                AddToZipArchive(archive, Path.GetFileName(file), new DirectoryInfo(file));
                            } else {
                                archive.CreateEntryFromFile(file, Path.GetFileName(file));
                            }
                        }
                    }

                    break;

                default:
                    new TextContent(FileListString).SaveAs(path, extension, append);
                    break;
            }
        }

        /// <summary>
        /// Provide a textual preview for extensions using text-like formats
        /// </summary>
        /// <param name="extension">File extension determining the format</param>
        /// <returns>Preview as text string</returns>
        ///
        public string TextPreview(string extension) {
            switch (extension) {
                case "zip":
                    return null;
                default:
                    return FileListString;
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
                if (type.IsInstanceOfType(content))
                    return content;
            }
            return null;
        }

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
            //
            // Note: if multiple clipboard contents support to same extension, the first in Contents is used


            // Images
            // ======

            // Collect images from in various formats (in order of priority)
            IList<Image> images = new List<Image>();

            // Native clipboard bitmap image
            if (Clipboard.ContainsImage() && Clipboard.GetImage() is Image bmp)
                images.Add(bmp);
            // Mime and file extension formats
            foreach (var ext in new[] { "png", "gif", "tif", "tiff", "jpg", "jpeg", "jfif", "bmp" }) {
                foreach (var format in new[] { ext.ToUpper(), "image/" + ext.ToLower() }) {
                    if (Clipboard.ContainsData(format) && Clipboard.GetData(format) is MemoryStream stream && Image.FromStream(stream) is Image img)
                        images.Add(img);
                }
            }
            // Native clipboard enhanced metafile
            if (Clipboard.ContainsData(DataFormats.EnhancedMetafile) && ReadClipboardMetafile() is Metafile emf)
                images.Add(emf);
            // Generic image from encoded data uri
            if (Clipboard.ContainsText() && ImageFromDataUri(Clipboard.GetText()) is Image image)
                images.Add(image);
            // Generic image from file
            if (Clipboard.ContainsFileDropList() && Clipboard.GetFileDropList() is StringCollection files && files.Count == 1) {
                try {
                    images.Add(Image.FromFile(files[0]));
                } catch { /* format not supported */ }
            }

            // Since images can have features (transparency, animations) which are not supported by all file format,
            // we handel images with such features separately:
            // 0. Vector image (if any)
            foreach (var img in images) {
                if (img is Metafile mf) {
                    container.Contents.Add(new VectorImageContent(mf));
                }
            }
            // 1. Animated image (if any)
            foreach (var img in images) {
                try {
                    if (img.GetFrameCount(FrameDimension.Time) > 1) {
                        container.Contents.Add(new AnimatedImageContent(img));
                        break;
                    }
                } catch { /* format does not support frames */ }
            }
            // 2. Transparent image (if any)
            foreach (var img in images) {
                if (((ImageFlags)img.Flags).HasFlag(ImageFlags.HasAlpha)) {
                    container.Contents.Add(new TransparentImageContent(img));
                    break;
                }
            }
            // 3. Image with no special features (if any)
            foreach (var img in images) {
                container.Contents.Add(new ImageContent(img));
                break;
            }


            // Other formats
            // =============

            if (ReadClipboardHtml() is string html)
                container.Contents.Add(new HtmlContent(html));

            if (ReadClipboardString(DataFormats.CommaSeparatedValue, "text/csv", "text/tab-separated-values") is string csv)
                container.Contents.Add(new CsvContent(csv));

            if (ReadClipboardString(DataFormats.SymbolicLink) is string lnk)
                container.Contents.Add(new GenericTextContent(DataFormats.SymbolicLink, "slk", lnk));

            if (ReadClipboardString(DataFormats.Rtf, "text/rtf") is string rtf)
                container.Contents.Add(new GenericTextContent(DataFormats.Rtf, "rtf", rtf));

            if (ReadClipboardString(DataFormats.Dif) is string dif)
                container.Contents.Add(new GenericTextContent(DataFormats.Dif, "dif", dif));

            if (ReadClipboardString("image/svg+xml") is string svg)
                container.Contents.Add(new SvgContent(svg));

            if (Clipboard.ContainsText() && Uri.IsWellFormedUriString(Clipboard.GetText().Trim(), UriKind.Absolute))
                container.Contents.Add(new UrlContent(Clipboard.GetText().Trim()));

            // make sure text content comes last, so it does not overwrite extensions used by previous special formats...
            if (ReadClipboardString(DataFormats.UnicodeText, DataFormats.Text, "text/plain") is string text)
                container.Contents.Add(new TextContent(text));

            // ...except for file list, which has even lower priority so as not to overwrite *.txt
            if (Clipboard.ContainsFileDropList())
                container.Contents.Add(new FilesContent(Clipboard.GetFileDropList()));


            return container;
        }

        private static string ReadClipboardHtml() {
            if (Clipboard.ContainsData(DataFormats.Html)) {
                var content = Clipboard.GetText(TextDataFormat.Html);
                var match = Regex.Match(content, @"StartHTML:(?<startHTML>\d*).*?EndHTML:(?<endHTML>\d*)", RegexOptions.Singleline);
                if (match.Success) {
                    var startHtml = Math.Max(int.Parse(match.Groups["startHTML"].Value), 0);
                    var endHtml = Math.Min(int.Parse(match.Groups["endHTML"].Value), content.Length);
                    return content.Substring(startHtml, endHtml - startHtml);
                }
            }
            return ReadClipboardString("text/html");
        }

        private static string ReadClipboardString(params string[] formats) {
            foreach (var format in formats) {
                if (!Clipboard.ContainsData(format))
                    continue;
                var data = Clipboard.GetData(format);
                switch (data) {
                    case string str:
                        return str;
                    case MemoryStream stream:
                        return new StreamReader(stream).ReadToEnd().TrimEnd('\0');
                }
            }
            return null;
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
            var ext = Path.GetExtension(path).ToLower().Trim('.');
            var container = new ClipboardContents {
                Timestamp = DateTime.Now
            };

            // if it's an image (try&catch instead of maintaining a list of supported extensions)
            try {
                var img = Image.FromFile(path);
                img = RotateFlipImageFromExif(img);
                if (img is Metafile mf) {
                    container.Contents.Add(new VectorImageContent(mf));
                } else {
                    container.Contents.Add(new ImageContent(img));
                }
            } catch { /* it's not */ }


            // if it's text like (check for absence of zero byte)
            if (!LooksLikeBinaryFile(path)) {
                var contents = File.ReadAllText(path);
                container.Contents.Add(new TextContent(contents));

                // check for doctype
                var firstLines = "";
                using (var reader = new StringReader(contents)) {
                    for (var i = 0; i < 2; i++)
                        firstLines += reader.ReadLine() + "\n";
                }
                var doctype = new Regex(@"<!DOCTYPE\s+(\S+).*>").Match(firstLines).Groups[1].Value.ToLower();

                // text like contents
                if (ext == "html" || ext == "htm" || doctype == "html")
                    container.Contents.Add(new HtmlContent(contents));
                if (ext == "svg" || doctype == "svg")
                    container.Contents.Add(new SvgContent(contents));
                if (ext == "csv")
                    container.Contents.Add(new CsvContent(contents));
                if (ext == "dif")
                    container.Contents.Add(new GenericTextContent(DataFormats.Dif, ext, contents));
                if (ext == "rtf")
                    container.Contents.Add(new GenericTextContent(DataFormats.Rtf, ext, contents));
                if (ext == "syk")
                    container.Contents.Add(new GenericTextContent(DataFormats.SymbolicLink, ext, contents));

            } else {
                container.Contents.Add(new TextContent(path));

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

        /// <summary>
        /// Convert a data uri to an image
        /// </summary>
        /// <param name="uri">The data URI, typically starting with data:image/</param>
        /// <returns>The image or null if the uri is not an image or conversion failed</returns>
        private static Image ImageFromDataUri(string uri) {
            try {
                var match = Regex.Match(uri, @"^data:image/\w+(?<base64>;base64)?,(?<data>.+)$");
                if (match.Success) {
                    if (match.Groups["base64"].Success) {
                        // Base64 encoded
                        var bytes = Convert.FromBase64String(match.Groups["data"].Value);
                        return Image.FromStream(new MemoryStream(bytes));
                    } else {
                        // URL encoded
                        var bytes = Encoding.Default.GetBytes(match.Groups["data"].Value);
                        bytes = WebUtility.UrlDecodeToBytes(bytes, 0, bytes.Length);
                        return Image.FromStream(new MemoryStream(bytes));
                    }
                }
            } catch { /* data uri malformed or not supported */ }
            return null;
        }

        /// <summary>
        /// Rotates and flips the given image according to the respective EXIF flag, and remove the EXIF flag
        /// </summary>
        /// <param name="image">The source image with possible EXIF tag</param>
        /// <returns>The rotated and flipped image without the EXIF tag</returns>
        private static Image RotateFlipImageFromExif(Image img) {
            // see https://exiftool.org/TagNames/EXIF.html
            if (img.PropertyIdList.Contains(0x0112)) {
                var orientation = (int)img.GetPropertyItem(0x0112).Value[0];
                if (orientation >= 1 && orientation <= 8) {
                    RotateFlipType rotateFlip = new Dictionary<int, RotateFlipType> {
                        [1] = RotateFlipType.RotateNoneFlipNone,
                        [2] = RotateFlipType.RotateNoneFlipX,
                        [3] = RotateFlipType.Rotate180FlipNone,
                        [4] = RotateFlipType.Rotate180FlipX,
                        [5] = RotateFlipType.Rotate90FlipX,
                        [6] = RotateFlipType.Rotate90FlipNone,
                        [7] = RotateFlipType.Rotate270FlipX,
                        [8] = RotateFlipType.Rotate270FlipNone,
                    }[orientation];
                    img.RotateFlip(rotateFlip);
                    img.RemovePropertyItem(0x0112);
                }
            }

            return img;
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
