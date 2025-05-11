using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WebP.Helpers;

internal static class ThrowHelper {
    [Obsolete]
    public static Exception Create(
        Exception inner,
        [CallerMemberName] string caller = "Unknown") {
        return new Exception($"{inner.Message}\nIn {caller}", inner);
    }

    public static Exception UnknownPlatform() {
        return new PlatformNotSupportedException("Unknown platform detected. Platform must be x86 or x64");
    }

    [Obsolete]
    public static Exception ContainsNoData([CallerMemberName] string caller = "Unknown") {
        return Create(new DataException("Bitmap contains no data"), caller);
    }

    [Obsolete]
    public static Exception SizeTooBig([CallerMemberName] string caller = "Unknown") {
        return
            Create(new DataException($"Dimension of bitmap is too large. Max is {WebPObject.WebpMaxDimension}x{WebPObject.WebpMaxDimension} pixels"),
                   caller);
    }

    [Obsolete]
    public static Exception CannotEncodeByUnknown([CallerMemberName] string caller = "Unknown") {
        return Create(new Exception("Cannot encode by unknown cause"), caller);
    }

    [Obsolete]
    public static Exception NullReferenced(string var, [CallerMemberName] string caller = "Unknown") {
        return Create(new NullReferenceException($"{var} is null"), caller);
    }

    [Obsolete]
    public static Exception QualityOutOfRange([CallerMemberName] string caller = "Unknown") {
        return Create(new NullReferenceException("Quality must be between"), caller);
    }
}
