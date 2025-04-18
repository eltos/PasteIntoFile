using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPPicture : IDisposable {
    public int use_argb;
    public uint colorspace;
    public int width;
    public int height;
    public IntPtr y;
    public IntPtr u;
    public IntPtr v;
    public IntPtr a;
    public int y_stride;
    public int uv_stride;
    public int a_stride;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
    private readonly uint[] pad1;

    public IntPtr argb;
    public int argb_stride;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
    private readonly uint[] pad2;

    public IntPtr writer;
    public IntPtr custom_ptr;
    public int extra_info_type;
    public IntPtr extra_info;
    public IntPtr stats;
    public uint error_code;
    public IntPtr progress_hook;
    public IntPtr user_data;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13, ArraySubType = UnmanagedType.U4)]
    private readonly uint[] pad3;

    private readonly IntPtr memory;
    private readonly IntPtr memory_argb;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
    private readonly uint[] pad4;

    public void Dispose() {
    }
}
