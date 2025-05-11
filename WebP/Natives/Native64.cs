using System;
using System.Runtime.InteropServices;
using System.Security;
using WebP.Natives.Enums;
using WebP.Natives.Structs;

namespace WebP.Natives;

[SuppressUnmanagedCodeSecurity]
public static class Native64 {
    private const string DllPath = "libwebp.x64.dll";

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigInitInternal")]
    public static extern int WebPConfigInitInternal_x64(ref WebPConfig config, WebPPreset preset, float quality,
                                                        int webpDecoderAbiVersion);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetFeaturesInternal")]
    public static extern Vp8StatusCode WebPGetFeaturesInternal_x64([In] IntPtr rawWebP, UIntPtr dataSize,
                                                                   ref WebPBitstreamFeatures features,
                                                                   int webpDecoderAbiVersion);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigLosslessPreset")]
    public static extern int WebPConfigLosslessPreset_x64(ref WebPConfig config, int level);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPValidateConfig")]
    public static extern int WebPValidateConfig_x64(ref WebPConfig config);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureInitInternal")]
    public static extern int WebPPictureInitInternal_x64(ref WebPPicture pic, int webpDecoderAbiVersion);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGR")]
    public static extern int WebPPictureImportBGR_x64(ref WebPPicture pic, IntPtr bgr, int stride);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRA")]
    public static extern int WebPPictureImportBGRA_x64(ref WebPPicture pic, IntPtr bgra, int stride);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRX")]
    public static extern int WebPPictureImportBGRX_x64(ref WebPPicture pic, IntPtr bgr, int stride);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncode")]
    public static extern int WebPEncode_x64(ref WebPConfig config, ref WebPPicture picture);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureFree")]
    public static extern void WebPPictureFree_x64(ref WebPPicture pic);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetInfo")]
    public static extern int WebPGetInfo_x64([In] IntPtr data, UIntPtr dataSize, out int width, out int height);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRInto")]
    public static extern int WebPDecodeBGRInto_x64([In] IntPtr data, UIntPtr dataSize, IntPtr outputBuffer,
                                                   int outputBufferSize, int outputStride);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRAInto")]
    public static extern int WebPDecodeBGRAInto_x64([In] IntPtr data, UIntPtr dataSize, IntPtr outputBuffer,
                                                    int outputBufferSize, int outputStride);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPInitDecoderConfigInternal")]
    public static extern int WebPInitDecoderConfigInternal_x64(ref WebPDecoderConfig webPDecoderConfig,
                                                               int webpDecoderAbiVersion);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecode")]
    public static extern Vp8StatusCode WebPDecode_x64(IntPtr data, UIntPtr dataSize, ref WebPDecoderConfig config);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFreeDecBuffer")]
    public static extern void WebPFreeDecBuffer_x64(ref WebPDecBuffer buffer);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGR")]
    public static extern int WebPEncodeBGR_x64([In] IntPtr bgr, int width, int height, int stride, float qualityFactor,
                                               out IntPtr output);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGRA")]
    public static extern int WebPEncodeBGRA_x64([In] IntPtr bgra, int width, int height, int stride,
                                                float qualityFactor, out IntPtr output);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGR")]
    public static extern int WebPEncodeLosslessBGR_x64([In] IntPtr bgr, int width, int height, int stride,
                                                       out IntPtr output);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGRA")]
    public static extern int WebPEncodeLosslessBGRA_x64([In] IntPtr bgra, int width, int height, int stride,
                                                        out IntPtr output);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFree")]
    public static extern void WebPFree_x64(IntPtr p);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
    public static extern int WebPGetDecoderVersion_x64();

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureDistortion")]
    public static extern int WebPPictureDistortion_x64(ref WebPPicture srcPicture, ref WebPPicture refPicture,
                                                       int metricType, IntPtr pResult);
}
