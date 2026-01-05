using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PasteIntoFile {
public abstract class Webp {

    public static byte[] Encode(Image image, bool lossless = true, float quality = 100) {
        // Convert to bitmap
        var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using (var graphic = Graphics.FromImage(bitmap)) {
            graphic.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        }

        // Encode bitmap data
        BitmapData data = null;
        try {
            data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var size = lossless
                ? EncodeLosslessBGRA(data.Scan0, data.Width, data.Height, data.Stride, out var ptr)
                : EncodeBGRA(data.Scan0, data.Width, data.Height, data.Stride, quality, out ptr);
            var bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            return bytes;

        } finally {
            if (data != null) bitmap.UnlockBits(data);
        }
    }

    [DllImport("libwebp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGR")]
    private static extern int EncodeBGRA([In] IntPtr bgr, int width, int height, int stride, float qualityFactor, out IntPtr output);

    [DllImport("libwebp.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGRA")]
    private static extern int EncodeLosslessBGRA([In] IntPtr bgra, int width, int height, int stride, out IntPtr output);


}
}
