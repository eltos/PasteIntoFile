using System;
using System.Runtime.InteropServices;
using WebP.Helpers;
using WebP.Natives;
using WebP.Natives.Enums;
using WebP.Natives.Structs;

namespace WebP;

public readonly struct WebPInfo {
    [method: Obsolete("WebPInfo.GetFrom is obsolete. Use WebPObject instead of this.")]
    public static WebPInfo GetFrom(byte[] webP) {
        var handle = GCHandle.Alloc(webP, GCHandleType.Pinned);

        try {
            var features = new WebPBitstreamFeatures();
            var status = Native.WebPGetFeatures(handle.AddrOfPinnedObject(), webP.Length, ref features);
            if (status is not Vp8StatusCode.Ok)
                throw new ExternalException(status.ToString());
            return new WebPInfo(features);
        } catch (Exception ex) {
            throw ThrowHelper.Create(ex);
        } finally {
            if (handle.IsAllocated)
                handle.Free();
        }
    }

    internal WebPInfo(WebPBitstreamFeatures features) {
        Width = features.Width;
        Height = features.Height;
        HasAlpha = features.Has_alpha is not 0;
        IsAnimated = features.Has_animation is not 0;
    }

    public int Width { get; }
    public int Height { get; }
    public bool HasAlpha { get; }
    public bool IsAnimated { get; }
}
