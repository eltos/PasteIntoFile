using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using WebP.Natives.Enums;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPConfig {
    public int lossless;
    public float quality;
    public int method;
    public WebPImageHint image_hint;
    public int target_size;
    public float target_PSNR;
    public int segments;
    public int sns_strength;
    public int filter_strength;
    public int filter_sharpness;
    public int filter_type;
    public int auto_filter;
    public int alpha_compression;
    public int alpha_filtering;
    public int alpha_quality;
    public int pass;
    public int show_compressed;
    public int preprocessing;
    public int partitions;
    public int partition_limit;
    public int emulate_jpeg_size;
    public int thread_level;
    public int low_memory;
    public int near_lossless;
    public int exact;
    public int delta_palletization;
    public int use_sharp_yuv;
    private readonly int pad1;
    private readonly int pad2;
}
