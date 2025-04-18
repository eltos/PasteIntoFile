using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace WebP.Natives.Structs;

[StructLayout(LayoutKind.Sequential),
 SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public struct WebPYuvaBuffer {
    public IntPtr y;
    public IntPtr u;
    public IntPtr v;
    public IntPtr a;
    public int y_stride;
    public int u_stride;
    public int v_stride;
    public int a_stride;
    public UIntPtr y_size;
    public UIntPtr u_size;
    public UIntPtr v_size;
    public UIntPtr a_size;
}
