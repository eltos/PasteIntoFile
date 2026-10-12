using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using PasteIntoFile.Core;

namespace PasteIntoFileShellExtension;


// Shared COM class IDs — must match AppxManifest.xml registrations
static class Clsids {
    public const string PasteCommand = "A6F24F95-0E3D-4A37-8E58-1F05D66E76A5";
    public const string CopyCommand = "41C9024D-4DCC-4CFE-B19B-C267B2525F63";
    public const string ReplaceCommand = "90220591-C089-4812-AF4B-510694581EF4";
    public const string AppendCommand = "668F5257-E4B7-4DE7-84AE-B1AA710F38F5";
}


// ─── Abstract base for all explorer commands ──────────────────────────────
// Implements the common IExplorerCommand boilerplate (GetIcon, GetToolTip,
// GetCanonicalName, GetFlags, EnumSubCommands) so each derived command only
// provides its title, state check, and invoke logic.

public enum PasteMode { Paste, Copy, Replace, Append }

public abstract class BaseExplorerCommand : IExplorerCommand {
    static BaseExplorerCommand() {
        // Configure settings provider to use the shared settings file path
        SettingsUtil.Apply(Settings.Default);
    }

    public abstract string Clsid { get; }

    // ─── Shared helper methods ───

    // Get the path to the PasteIntoFile executable relative to the assembly location.
    protected static string? GetPasteIntoFileExecutablePath() {
        var extensionDirectory = Path.GetDirectoryName(typeof(BaseExplorerCommand).Assembly.Location);
        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            extensionDirectory = AppContext.BaseDirectory;
        }
        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            return null;
        }
        return Path.Combine(extensionDirectory, "PasteIntoFile.exe");
    }

    // Check if exactly one file is selected and it exists on disk.
    // Returns ECS_ENABLED if so, otherwise ECS_HIDDEN.
    protected static EXPCMDSTATE CheckSingleFileSelected(IShellItemArray psiItemArray) {
        if (psiItemArray != null && psiItemArray.GetCount(out var count) == HResult.S_OK && count == 1) {
            psiItemArray.GetItemAt(0, out IShellItem? shellItem);
            if (shellItem != null) {
                try {
                    if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pathPtr) == HResult.S_OK && pathPtr != IntPtr.Zero) {
                        var path = Marshal.PtrToStringUni(pathPtr);
                        Marshal.FreeCoTaskMem(pathPtr);
                        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                            return EXPCMDSTATE.ECS_ENABLED;
                        }
                    }
                } finally {
                    Marshal.ReleaseComObject(shellItem);
                }
            }
        }
        return EXPCMDSTATE.ECS_HIDDEN;
    }

    // Extract the file system path of the first item in the array.
    // Returns null if the array is empty or the path cannot be retrieved.
    protected static string? GetSingleItemPath(IShellItemArray? psiItemArray) {
        if (psiItemArray?.GetCount(out var count) != HResult.S_OK || count == 0) {
            return null;
        }
        psiItemArray.GetItemAt(0, out IShellItem? shellItem);
        if (shellItem == null) {
            return null;
        }
        try {
            if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pathPtr) != HResult.S_OK || pathPtr == IntPtr.Zero) {
                return null;
            }
            var path = Marshal.PtrToStringUni(pathPtr);
            Marshal.FreeCoTaskMem(pathPtr);
            return path;
        } finally {
            Marshal.ReleaseComObject(shellItem);
        }
    }

    // Invoke PasteIntoFile with the specified mode.
    // Handles all four command types: paste to directory, copy file contents, replace, and append.
    protected static int InvokePasteIntoFile(IShellItemArray psiItemArray, PasteMode mode) {
        try {
            var exePath = GetPasteIntoFileExecutablePath();
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) {
                return HResult.E_FAIL;
            }

            var startInfo = new ProcessStartInfo {
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var path = GetSingleItemPath(psiItemArray);

            switch (mode) {
                case PasteMode.Paste:
                    startInfo.ArgumentList.Add("paste");
                    if (!string.IsNullOrWhiteSpace(path)) {
                        startInfo.ArgumentList.Add(path);
                    }
                    break;

                case PasteMode.Copy:
                    if (string.IsNullOrEmpty(path)) {
                        return HResult.E_FAIL;
                    }
                    startInfo.ArgumentList.Add("copy");
                    startInfo.ArgumentList.Add(path);
                    break;

                case PasteMode.Replace:
                case PasteMode.Append:
                    var directory = Path.GetDirectoryName(path);
                    var file = Path.GetFileName(path);
                    if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(file)) {
                        return HResult.E_FAIL;
                    }
                    startInfo.ArgumentList.Add("paste");
                    startInfo.ArgumentList.Add(directory);
                    startInfo.ArgumentList.Add("--filename");
                    startInfo.ArgumentList.Add(file);
                    startInfo.ArgumentList.Add("--autosave=true");
                    startInfo.ArgumentList.Add(mode == PasteMode.Replace ? "--overwrite=true" : "--append");
                    break;
            }

            Process.Start(startInfo);
            return HResult.S_OK;
        } catch {
            return HResult.E_FAIL;
        }
    }

    // ─── IExplorerCommand implementation ───

    public abstract int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName);

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        var exePath = GetPasteIntoFileExecutablePath();
        if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath)) {
            ppszIcon = Marshal.StringToCoTaskMemUni(exePath + ",0");
            return HResult.S_OK;
        }
        ppszIcon = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(Clsid);
        return HResult.S_OK;
    }

    public abstract int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState);

    public abstract int Invoke(IShellItemArray psiItemArray, IntPtr pbc);

    public virtual int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HResult.S_OK;
    }

    public virtual int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        ppEnum = null!;
        return HResult.E_NOTIMPL;
    }
}

[ComVisible(true)]
[Guid("A08CE4D0-FA25-44AB-B57C-C7B1C323E0B9")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IExplorerCommand {
    [PreserveSig]
    int GetTitle(
        [MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray,
        out IntPtr ppszName
    );

    [PreserveSig]
    int GetIcon(
        [MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray,
        out IntPtr ppszIcon
    );

    [PreserveSig]
    int GetToolTip(
        [MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray,
        out IntPtr ppszInfotip
    );

    [PreserveSig]
    int GetCanonicalName(out Guid pguidCommandName);

    [PreserveSig]
    int GetState(
        [MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray,
        [MarshalAs(UnmanagedType.Bool)] bool fOkToBeSlow,
        out EXPCMDSTATE pCmdState
    );

    [PreserveSig]
    int Invoke(
        [MarshalAs(UnmanagedType.Interface)] IShellItemArray psiItemArray,
        IntPtr pbc
    );

    [PreserveSig]
    int GetFlags(out EXPCMDFLAGS pFlags);

    [PreserveSig]
    int EnumSubCommands(
        [MarshalAs(UnmanagedType.Interface)] out IEnumExplorerCommand ppEnum
    );
}

[ComImport]
[Guid("B63EA76D-1F85-456F-A19C-48159EFA858B")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItemArray {
    [PreserveSig]
    int BindToHandler(
        IntPtr pbc,
        ref Guid bhid,
        ref Guid riid,
        out IntPtr ppvOut
    );

    [PreserveSig]
    int GetPropertyStore(
        GETPROPERTYSTOREFLAGS flags,
        ref Guid riid,
        out IntPtr ppv
    );

    [PreserveSig]
    int GetPropertyDescriptionList(
        ref PROPERTYKEY keyType,
        ref Guid riid,
        out IntPtr ppv
    );

    [PreserveSig]
    int GetAttributes(
        SIATTRIBFLAGS attribFlags,
        uint sfgaoMask,
        out uint psfgaoAttribs
    );

    [PreserveSig]
    int GetCount(out uint pdwNumItems);

    [PreserveSig]
    int GetItemAt(
        uint dwIndex,
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi
    );

    [PreserveSig]
    int EnumItems(out IntPtr ppenumShellItems);
}

[ComImport]
[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IShellItem {
    [PreserveSig]
    int BindToHandler(
        IntPtr pbc,
        ref Guid bhid,
        ref Guid riid,
        out IntPtr ppv
    );

    [PreserveSig]
    int GetParent(
        [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi
    );

    [PreserveSig]
    int GetDisplayName(
        SIGDN sigdnName,
        out IntPtr ppszName
    );

    [PreserveSig]
    int GetAttributes(
        uint sfgaoMask,
        out uint psfgaoAttribs
    );

    [PreserveSig]
    int Compare(
        [MarshalAs(UnmanagedType.Interface)] IShellItem psi,
        uint hint,
        out int piOrder
    );
}

[ComImport]
[Guid("A88826F8-186F-4987-AADE-EA0CEF8FBFE8")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IEnumExplorerCommand {
    [PreserveSig]
    int Next(uint celt, out IExplorerCommand rgelt, out uint pceltFetched);

    [PreserveSig]
    int Skip(uint celt);

    [PreserveSig]
    int Reset();

    [PreserveSig]
    int Clone(out IEnumExplorerCommand ppEnum);
}


static class HResult {
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOTIMPL = unchecked((int)0x80004001);
    public const int E_FAIL = unchecked((int)0x80004005);
}

public enum EXPCMDSTATE {
    ECS_ENABLED = 0,
    ECS_DISABLED = 1,
    ECS_HIDDEN = 2,
    ECS_CHECKBOX = 4,
    ECS_CHECKED = 8,
    ECS_RADIOCHECK = 16
}

[Flags]
public enum EXPCMDFLAGS {
    ECF_DEFAULT = 0,
    ECF_HASSUBCOMMANDS = 1,
    ECF_HASSPLITBUTTON = 2,
    ECF_HIDELABEL = 4,
    ECF_ISSEPARATOR = 8,
    ECF_HASLUASHIELD = 16,
    ECF_SEPARATORBEFORE = 32,
    ECF_SEPARATORAFTER = 64,
    ECF_ISDROPDOWN = 128,
    ECF_TOGGLEABLE = 256,
    ECF_AUTOMENUICONS = 512
}

public enum SIGDN : uint {
    SIGDN_NORMALDISPLAY = 0x00000000,
    SIGDN_PARENTRELATIVEPARSING = 0x80018001,
    SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
    SIGDN_PARENTRELATIVEEDITING = 0x80031001,
    SIGDN_DESKTOPABSOLUTEEDITING = 0x8004C000,
    SIGDN_FILESYSPATH = 0x80058000,
    SIGDN_URL = 0x80068000,
    SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007C001,
    SIGDN_PARENTRELATIVE = 0x80080001,
    SIGDN_PARENTRELATIVEFORUI = 0x80094001
}

public enum GETPROPERTYSTOREFLAGS {
    GPS_DEFAULT = 0
}

public enum SIATTRIBFLAGS {
    SIATTRIBFLAGS_AND = 1,
    SIATTRIBFLAGS_OR = 2,
    SIATTRIBFLAGS_APPCOMPAT = 3
}

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PROPERTYKEY {
    public Guid fmtid;
    public uint pid;
}
