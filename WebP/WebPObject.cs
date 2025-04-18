using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WebP.Natives;
using WebP.Natives.Enums;
using WebP.Natives.Structs;

namespace WebP;

public sealed class WebPObject : IDisposable {
    #region Ctors

    public WebPObject(Image image) {
        _initWithImage = true;
        ImageCache = image;
    }

    public WebPObject(byte[] bytes) {
        BytesCache = bytes;
    }

    #endregion

    #region Fields

    private readonly object _cacheLockHandle = new();

    private byte[] _bytesCache;

    private readonly bool _initWithImage;

    internal const int WebpMaxDimension = 16383;

    #endregion

    #region Properties

    private (IntPtr Pointer, int Size) DynamicArray { get; set; } = (IntPtr.Zero, 0);

    private byte[] BytesCache {
        get {
            lock (_cacheLockHandle) {
                if (_bytesCache is not null)
                    return _bytesCache;
                if (DynamicArray.Pointer == IntPtr.Zero)
                    return null;
                _bytesCache = new byte[DynamicArray.Size];
                Marshal.Copy(DynamicArray.Pointer, _bytesCache, 0, DynamicArray.Size);
                return _bytesCache;
            }
        }
        set => _bytesCache = value;
    }

    private Image ImageCache { get; set; }

    private WebPInfo? InfoCache { get; set; }

    #endregion

    #region Private methods

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void VerifyImage(Image image) {
        switch (image) {
            case null:
                throw new ArgumentNullException(nameof(image));
            case { Width: 0 } or { Height: 0 }:
                throw new DataException("Image contains no data.");
            case { Width: > WebpMaxDimension } or { Height: > WebpMaxDimension }:
                throw new IOException($"Image too big. {WebpMaxDimension}x{WebpMaxDimension} is maximal size.");
        }
    }

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitmap ConvertTo32Argb(Image image) {
        var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
        using var graphic = Graphics.FromImage(bitmap);
        graphic.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
        return bitmap;
    }

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BitmapData GetBitmapData(Bitmap bitmap, ImageLockMode mode) {
        return bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), mode, bitmap.PixelFormat);
    }

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static WebPInfo GetInfoFrom(IntPtr pointer, int size) {
        var features = new WebPBitstreamFeatures();
        var status = Native.WebPGetFeatures(pointer, size, ref features);
        if (status is not Vp8StatusCode.Ok)
            throw new ExternalException(status.ToString());
        return new WebPInfo(features);
    }

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void StoreEncodedResult(Image image, bool lossy, float quality) {
        VerifyImage(image);
        using var bitmap = ConvertTo32Argb(image);
        BitmapData data = null;
        try {
            data = GetBitmapData(bitmap, ImageLockMode.ReadOnly);
            var size = lossy
                ? Native.WebPEncodeBgra(data.Scan0, data.Width, data.Height, data.Stride, quality, out var ptr)
                : Native.WebPEncodeLosslessBgra(data.Scan0, data.Width, data.Height, data.Stride, out ptr);
            if (size is 0)
                throw new IOException("Cannot encode image by unknown cause.");
            DynamicArray = (ptr, size);
        } finally {
            if (data is not null) bitmap.UnlockBits(data);
        }
    }

    private delegate int DecodeInto(IntPtr ptr, int size, IntPtr output, int outputSize, int outputStride);

    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Bitmap Decode(IntPtr pointer, int size) {
        Bitmap bmp = null;
        BitmapData data = null;
        int length;

        try {
            var info = GetInfoFrom(pointer, size);
            bmp = new Bitmap(info.Width, info.Height, info.HasAlpha
                                 ? PixelFormat.Format32bppArgb
                                 : PixelFormat.Format24bppRgb);
            data = GetBitmapData(bmp, ImageLockMode.WriteOnly);
            length = ((DecodeInto)(info.HasAlpha
                    ? Native.WebPDecodeBgraInto
                    : Native.WebPDecodeBgrInto))
               .Invoke(pointer, size, data.Scan0, data.Stride * info.Height, data.Stride);
        } finally {
            if (data is not null) bmp.UnlockBits(data);
        }

        if (length is 0)
            throw new IOException("Cannot decode image by unknown cause.");

        return bmp;
    }

    #endregion

    #region Public methods

    public Image GetImage() {
        if (DynamicArray.Pointer != IntPtr.Zero)
            return ImageCache ??= Decode(DynamicArray.Pointer, DynamicArray.Size);

        var cache = BytesCache;
        var handle = GCHandle.Alloc(cache, GCHandleType.Pinned);
        try {
            return ImageCache ??= Decode(handle.AddrOfPinnedObject(), cache.Length);
        } finally {
            if (handle.IsAllocated) handle.Free();
        }
    }

    public byte[] GetWebPLossy(float quality = 70, bool forceLossy = false) {
        if (BytesCache is null)
            StoreEncodedResult(ImageCache, true, quality);
        else if (forceLossy)
            StoreEncodedResult(GetImage(), true, quality);
        return BytesCache;
    }

    public byte[] GetWebPLossless() {
        if (BytesCache is null)
            StoreEncodedResult(ImageCache, false, 0);
        return BytesCache;
    }

    public WebPInfo GetInfo() {
        if (InfoCache.HasValue)
            return InfoCache.Value;
        if (DynamicArray.Pointer != IntPtr.Zero)
            return InfoCache ??= GetInfoFrom(DynamicArray.Pointer, DynamicArray.Size);

        var cache = BytesCache;
        var handle = GCHandle.Alloc(cache, GCHandleType.Pinned);
        try {
            return InfoCache ??= GetInfoFrom(handle.AddrOfPinnedObject(), cache.Length);
        } finally {
            if (handle.IsAllocated) handle.Free();
        }
    }

    #endregion

    #region Dtors

    ~WebPObject() {
        Dispose();
    }

    public void Dispose() {
        if (!_initWithImage)
            ImageCache?.Dispose();
        if (DynamicArray.Pointer != IntPtr.Zero)
            Native.WebPFree(DynamicArray.Pointer);
        GC.SuppressFinalize(this);
    }

    #endregion
}
