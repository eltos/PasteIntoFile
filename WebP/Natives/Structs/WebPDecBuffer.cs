using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using WebP.Natives.Enums;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPDecBuffer {
    public WebpCspMode colorSpace;
    public int width;
    public int height;
    public int isExternalMemory;
    public RgbaYuvaBuffer u;
    private readonly uint pad1;
    private readonly uint pad2;
    private readonly uint pad3;
    private readonly uint pad4;
    public IntPtr private_memory;
}
