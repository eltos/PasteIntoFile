using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPDecoderConfig {
    public WebPBitstreamFeatures input;
    public WebPDecBuffer output;
    public WebPDecoderOptions options;
}
