using System;
using System.Security;
using WebP.Helpers;
using WebP.Natives.Enums;
using WebP.Natives.Structs;

namespace WebP.Natives;

using static Native86;
using static Native64;

[SuppressUnmanagedCodeSecurity]
public static class Native {
    private const int WebpDecoderAbiVersion = 0x0208;

    public static int WebPConfigInit(ref WebPConfig config, WebPPreset preset, float quality) {
        return IntPtr.Size switch {
            4 => WebPConfigInitInternal_x86(ref config, preset, quality, WebpDecoderAbiVersion),
            8 => WebPConfigInitInternal_x64(ref config, preset, quality, WebpDecoderAbiVersion),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static Vp8StatusCode WebPGetFeatures(IntPtr rawWebP, int dataSize, ref WebPBitstreamFeatures features) {
        return IntPtr.Size switch {
            4 => WebPGetFeaturesInternal_x86(rawWebP, (UIntPtr)dataSize, ref features, WebpDecoderAbiVersion),
            8 => WebPGetFeaturesInternal_x64(rawWebP, (UIntPtr)dataSize, ref features, WebpDecoderAbiVersion),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPConfigLosslessPreset(ref WebPConfig config, int level) {
        return IntPtr.Size switch {
            4 => WebPConfigLosslessPreset_x86(ref config, level),
            8 => WebPConfigLosslessPreset_x64(ref config, level),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPValidateConfig(ref WebPConfig config) {
        return IntPtr.Size switch {
            4 => WebPValidateConfig_x86(ref config),
            8 => WebPValidateConfig_x64(ref config),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPPictureInitInternal(ref WebPPicture pic) {
        return IntPtr.Size switch {
            4 => WebPPictureInitInternal_x86(ref pic, WebpDecoderAbiVersion),
            8 => WebPPictureInitInternal_x64(ref pic, WebpDecoderAbiVersion),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPPictureImportBgr(ref WebPPicture pic, IntPtr bgr, int stride) {
        return IntPtr.Size switch {
            4 => WebPPictureImportBGR_x86(ref pic, bgr, stride),
            8 => WebPPictureImportBGR_x64(ref pic, bgr, stride),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPPictureImportBgra(ref WebPPicture pic, IntPtr bgra, int stride) {
        return IntPtr.Size switch {
            4 => WebPPictureImportBGRA_x86(ref pic, bgra, stride),
            8 => WebPPictureImportBGRA_x64(ref pic, bgra, stride),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPPictureImportBgrx(ref WebPPicture pic, IntPtr bgr, int stride) {
        return IntPtr.Size switch {
            4 => WebPPictureImportBGRX_x86(ref pic, bgr, stride),
            8 => WebPPictureImportBGRX_x64(ref pic, bgr, stride),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPEncode(ref WebPConfig config, ref WebPPicture picture) {
        return IntPtr.Size switch {
            4 => WebPEncode_x86(ref config, ref picture),
            8 => WebPEncode_x64(ref config, ref picture),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static void WebPPictureFree(ref WebPPicture picture) {
        switch (IntPtr.Size) {
            case 4:
                WebPPictureFree_x86(ref picture);
                break;
            case 8:
                WebPPictureFree_x64(ref picture);
                break;
            default: throw ThrowHelper.UnknownPlatform();
        }
    }

    public static int WebPGetInfo(IntPtr data, int dataSize, out int width, out int height) {
        return IntPtr.Size switch {
            4 => WebPGetInfo_x86(data, (UIntPtr)dataSize, out width, out height),
            8 => WebPGetInfo_x64(data, (UIntPtr)dataSize, out width, out height),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPDecodeBgrInto(IntPtr data, int dataSize, IntPtr outputBuffer, int outputBufferSize,
                                        int outputStride) {
        return IntPtr.Size switch {
            4 => WebPDecodeBGRInto_x86(data, (UIntPtr)dataSize, outputBuffer, outputBufferSize, outputStride),
            8 => WebPDecodeBGRInto_x64(data, (UIntPtr)dataSize, outputBuffer, outputBufferSize, outputStride),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPDecodeBgraInto(IntPtr data, int dataSize, IntPtr outputBuffer, int outputBufferSize,
                                         int outputStride) {
        return IntPtr.Size switch {
            4 => WebPDecodeBGRAInto_x86(data, (UIntPtr)dataSize, outputBuffer, outputBufferSize, outputStride),
            8 => WebPDecodeBGRAInto_x64(data, (UIntPtr)dataSize, outputBuffer, outputBufferSize, outputStride),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPInitDecoderConfig(ref WebPDecoderConfig webPDecoderConfig) {
        return IntPtr.Size switch {
            4 => WebPInitDecoderConfigInternal_x86(ref webPDecoderConfig, WebpDecoderAbiVersion),
            8 => WebPInitDecoderConfigInternal_x64(ref webPDecoderConfig, WebpDecoderAbiVersion),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static Vp8StatusCode WebPDecode(IntPtr data, int dataSize, ref WebPDecoderConfig webPDecoderConfig) {
        return IntPtr.Size switch {
            4 => WebPDecode_x86(data, (UIntPtr)dataSize, ref webPDecoderConfig),
            8 => WebPDecode_x64(data, (UIntPtr)dataSize, ref webPDecoderConfig),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static void WebPFreeDecBuffer(ref WebPDecBuffer buffer) {
        switch (IntPtr.Size) {
            case 4:
                WebPFreeDecBuffer_x86(ref buffer);
                break;
            case 8:
                WebPFreeDecBuffer_x64(ref buffer);
                break;
            default: throw ThrowHelper.UnknownPlatform();
        }
    }

    public static int WebPEncodeBgr(IntPtr bgr, int width, int height, int stride, float qualityFactor,
                                    out IntPtr output) {
        return IntPtr.Size switch {
            4 => WebPEncodeBGR_x86(bgr, width, height, stride, qualityFactor, out output),
            8 => WebPEncodeBGR_x64(bgr, width, height, stride, qualityFactor, out output),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPEncodeBgra(IntPtr bgra, int width, int height, int stride, float qualityFactor,
                                     out IntPtr output) {
        return IntPtr.Size switch {
            4 => WebPEncodeBGRA_x86(bgra, width, height, stride, qualityFactor, out output),
            8 => WebPEncodeBGRA_x64(bgra, width, height, stride, qualityFactor, out output),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPEncodeLosslessBgr(IntPtr bgr, int width, int height, int stride, out IntPtr output) {
        return IntPtr.Size switch {
            4 => WebPEncodeLosslessBGR_x86(bgr, width, height, stride, out output),
            8 => WebPEncodeLosslessBGR_x64(bgr, width, height, stride, out output),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPEncodeLosslessBgra(IntPtr bgra, int width, int height, int stride, out IntPtr output) {
        return IntPtr.Size switch {
            4 => WebPEncodeLosslessBGRA_x86(bgra, width, height, stride, out output),
            8 => WebPEncodeLosslessBGRA_x64(bgra, width, height, stride, out output),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static void WebPFree(IntPtr p) {
        switch (IntPtr.Size) {
            case 4:
                WebPFree_x86(p);
                break;
            case 8:
                WebPFree_x64(p);
                break;
            default: throw ThrowHelper.UnknownPlatform();
        }
    }

    public static int WebPGetDecoderVersion() {
        return IntPtr.Size switch {
            4 => WebPGetDecoderVersion_x86(),
            8 => WebPGetDecoderVersion_x64(),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }

    public static int WebPPictureDistortion(ref WebPPicture srcPicture, ref WebPPicture refPicture, int metricType,
                                            IntPtr pResult) {
        return IntPtr.Size switch {
            4 => WebPPictureDistortion_x86(ref srcPicture, ref refPicture, metricType, pResult),
            8 => WebPPictureDistortion_x64(ref srcPicture, ref refPicture, metricType, pResult),
            _ => throw ThrowHelper.UnknownPlatform()
        };
    }
}
