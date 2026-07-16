using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Command.ClassId)]
[ProgId("PasteIntoFile.ShellExtension.PasteCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class Command : IExplorerCommand {
    public const string ClassId = "A6F24F95-0E3D-4A37-8E58-1F05D66E76A5";

    private const int S_OK = 0;
    private const int E_NOTIMPL = unchecked((int)0x80004001);
    private const int E_FAIL = unchecked((int)0x80004005);


    public int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        var title = "PasteIntoFile";
        ppszName = Marshal.StringToCoTaskMemUni(title);
        return S_OK;

    }

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        var exePath = GetPasteIntoFileExecutablePath();

        if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath)) {
            ppszIcon = Marshal.StringToCoTaskMemUni(exePath + ",0");
            return S_OK;
        }

        ppszIcon = IntPtr.Zero;
        return E_NOTIMPL;
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(ClassId);
        return S_OK;
    }

    public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = EXPCMDSTATE.ECS_ENABLED;
        return S_OK;
    }

    public int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        try {
            var exePath = GetPasteIntoFileExecutablePath();

            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) {
                return E_FAIL;
            }

            var targetFolder = TryGetTargetFolderPath(psiItemArray);

            var startInfo = new ProcessStartInfo {
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add("paste");

            if (!string.IsNullOrWhiteSpace(targetFolder)) {
                startInfo.ArgumentList.Add(targetFolder);
            }

            Process.Start(startInfo);
            return S_OK;
        } catch {
            return E_FAIL;
        }
    }

    public int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return S_OK;
    }

    public int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        ppEnum = null!;
        return E_NOTIMPL;
    }

    private static string? TryGetTargetFolderPath(IShellItemArray? shellItemArray) {
        if (shellItemArray == null) {
            return null;
        }

        try {
            if (shellItemArray.GetCount(out var count) != S_OK || count == 0) {
                return null;
            }

            for (uint i = 0; i < count; i++) {
                IShellItem? shellItem = null;

                try {
                    if (shellItemArray.GetItemAt(i, out shellItem) != S_OK || shellItem == null) {
                        continue;
                    }

                    var path = TryGetFileSystemPath(shellItem);

                    if (string.IsNullOrWhiteSpace(path)) {
                        continue;
                    }

                    if (Directory.Exists(path)) {
                        return path;
                    }

                    if (File.Exists(path)) {
                        return Path.GetDirectoryName(path);
                    }
                } finally {
                    if (shellItem != null) {
                        Marshal.ReleaseComObject(shellItem);
                    }
                }
            }
        } catch {
            return null;
        }

        return null;
    }

    private static string? TryGetFileSystemPath(IShellItem shellItem) {
        IntPtr pathPtr = IntPtr.Zero;

        try {
            if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out pathPtr) != S_OK || pathPtr == IntPtr.Zero) {
                return null;
            }

            return Marshal.PtrToStringUni(pathPtr);
        } catch {
            return null;
        } finally {
            if (pathPtr != IntPtr.Zero) {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }
    }

    private static string? GetPasteIntoFileExecutablePath() {
        var extensionDirectory = Path.GetDirectoryName(typeof(Command).Assembly.Location);

        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            extensionDirectory = AppContext.BaseDirectory;
        }

        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            return null;
        }

        return Path.Combine(extensionDirectory, "PasteIntoFile.exe");
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
