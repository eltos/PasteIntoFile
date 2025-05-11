using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPAuxStats {
    public int coded_size;
    public float PSNR_Y;
    public float PSNR_U;
    public float PSNR_V;
    public float PSNR_ALL;
    public float PSNRAlpha;
    public int block_count_intra4;
    public int block_count_intra16;
    public int block_count_skipped;
    public int header_bytes;
    public int mode_partition_0;
    public int residual_bytes_DC_segments0;
    public int residual_bytes_AC_segments0;
    public int residual_bytes_uv_segments0;
    public int residual_bytes_DC_segments1;
    public int residual_bytes_AC_segments1;
    public int residual_bytes_uv_segments1;
    public int residual_bytes_DC_segments2;
    public int residual_bytes_AC_segments2;
    public int residual_bytes_uv_segments2;
    public int residual_bytes_DC_segments3;
    public int residual_bytes_AC_segments3;
    public int residual_bytes_uv_segments3;
    public int segment_size_segments0;
    public int segment_size_segments1;
    public int segment_size_segments2;
    public int segment_size_segments3;
    public int segment_quant_segments0;
    public int segment_quant_segments1;
    public int segment_quant_segments2;
    public int segment_quant_segments3;
    public int segment_level_segments0;
    public int segment_level_segments1;
    public int segment_level_segments2;
    public int segment_level_segments3;
    public int alpha_data_size;
    public int layer_data_size;
    public int lossless_features;
    public int histogram_bits;
    public int transform_bits;
    public int cache_bits;
    public int palette_size;
    public int lossless_size;
    public int lossless_hdr_size;
    public int lossless_data_size;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
    private readonly uint[] pad;
}
