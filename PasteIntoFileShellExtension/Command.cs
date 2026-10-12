using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PasteIntoFileShellExtension;

// Shared title constants — match the localized strings from the main app's Resources.
// Since the shell extension runs in a surrogate process, we use hardcoded English titles
// that match the default (English) resource strings.
static class Titles {
    public const string Paste = "PasteIntoFile";
    public const string Copy = "Copy file contents";
    public const string Replace = "Paste into (Replace)";
    public const string Append = "Paste into (Append)";
}

// Shared COM HRESULT constants
static class HResult {
    public const int S_OK = 0;
    public const int S_FALSE = 1;
    public const int E_NOTIMPL = unchecked((int)0x80004001);
    public const int E_FAIL = unchecked((int)0x80004005);
}

// Shared COM class IDs — must match AppxManifest.xml registrations
static class Clsids {
    public const string Command = "A6F24F95-0E3D-4A37-8E58-1F05D66E76A5";
    public const string CopyCommand = "41C9024D-4DCC-4CFE-B19B-C267B2525F63";
    public const string ReplaceCommand = "90220591-C089-4812-AF4B-510694581EF4";
    public const string AppendCommand = "668F5257-E4B7-4DE7-84AE-B1AA710F38F5";
}

// Shared helper methods for common shell extension patterns
static class ExplorerCommandHelpers {
    // Check if exactly one file is selected and it exists on disk.
    // Returns ECS_ENABLED if so, otherwise ECS_HIDDEN.
    public static EXPCMDSTATE CheckSingleFileSelected(IShellItemArray psiItemArray) {
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
    public static string? GetSingleItemPath(IShellItemArray psiItemArray) {
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

    // Get the path to the PasteIntoFile executable relative to the assembly location.
    public static string? GetPasteIntoFileExecutablePath() {
        var extensionDirectory = Path.GetDirectoryName(typeof(Command).Assembly.Location);
        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            extensionDirectory = AppContext.BaseDirectory;
        }
        if (string.IsNullOrWhiteSpace(extensionDirectory)) {
            return null;
        }
        return Path.Combine(extensionDirectory, "PasteIntoFile.exe");
    }

    // Shared GetIcon implementation — returns the executable icon or E_NOTIMPL.
    public static int GetIcon(out IntPtr ppszIcon) {
        var exePath = GetPasteIntoFileExecutablePath();
        if (!string.IsNullOrWhiteSpace(exePath) && File.Exists(exePath)) {
            ppszIcon = Marshal.StringToCoTaskMemUni(exePath + ",0");
            return HResult.S_OK;
        }
        ppszIcon = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    // File extensions that do NOT support appending (binary / image formats)
    public static readonly string[] AppendNotSupportedExtensions = new[] {
        "bmp", "gif", "ico", "jpeg", "jpg", "png", "tif", "tiff", "webp", // images
        "svg", // vector
        "url"  // shortcuts
    };

    // Check if appending is supported for the given file path.
    public static bool IsAppendSupported(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return true;
        }
        var ext = Path.GetExtension(path)?.TrimStart('.').ToLowerInvariant();
        return string.IsNullOrEmpty(ext) || Array.IndexOf(AppendNotSupportedExtensions, ext) < 0;
    }

    // Check if exactly one file is selected and append is supported for it.
    // Returns ECS_ENABLED if so, otherwise ECS_HIDDEN.
    public static EXPCMDSTATE CheckSingleFileSelectedWithAppendSupport(IShellItemArray psiItemArray) {
        if (psiItemArray != null && psiItemArray.GetCount(out var count) == HResult.S_OK && count == 1) {
            psiItemArray.GetItemAt(0, out IShellItem? shellItem);
            if (shellItem != null) {
                try {
                    if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pathPtr) == HResult.S_OK && pathPtr != IntPtr.Zero) {
                        var path = Marshal.PtrToStringUni(pathPtr);
                        Marshal.FreeCoTaskMem(pathPtr);
                        if (!string.IsNullOrEmpty(path) && File.Exists(path) && IsAppendSupported(path)) {
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

    // Invoke PasteIntoFile.exe with paste command for a specific file.
    public static int InvokePasteIntoFile(IShellItemArray psiItemArray, bool overwrite, bool append) {
        try {
            var exePath = GetPasteIntoFileExecutablePath();
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) {
                return HResult.E_FAIL;
            }

            var filePath = GetSingleItemPath(psiItemArray);
            if (string.IsNullOrEmpty(filePath)) {
                return HResult.E_FAIL;
            }

            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            var startInfo = new ProcessStartInfo {
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("paste");
            startInfo.ArgumentList.Add("--directory=\"" + dir + "\"");
            startInfo.ArgumentList.Add("--filename=\"" + fileName + "\"");
            startInfo.ArgumentList.Add("--autosave=true");
            if (overwrite) {
                startInfo.ArgumentList.Add("--overwrite=true");
            }
            if (append) {
                startInfo.ArgumentList.Add("--append");
            }

            Process.Start(startInfo);
            return HResult.S_OK;
        } catch {
            return HResult.E_FAIL;
        }
    }
}

[ComVisible(true)]
[Guid(Clsids.Command)]
[ProgId("PasteIntoFile.ShellExtension.PasteCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class Command : IExplorerCommand {


    public int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Paste);
        return HResult.S_OK;
    }

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        return ExplorerCommandHelpers.GetIcon(out ppszIcon);
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(Clsids.Command);
        return HResult.S_OK;
    }

    public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = EXPCMDSTATE.ECS_ENABLED;
        return HResult.S_OK;
    }

    public int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        try {
            var exePath = ExplorerCommandHelpers.GetPasteIntoFileExecutablePath();

            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) {
                return HResult.E_FAIL;
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
            return HResult.S_OK;
        } catch {
            return HResult.E_FAIL;
        }
    }

    public int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HResult.S_OK;
    }

    public int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        ppEnum = null!;
        return HResult.E_NOTIMPL;
    }

    private static string? TryGetTargetFolderPath(IShellItemArray? shellItemArray) {
        if (shellItemArray == null) {
            return null;
        }

        try {
            if (shellItemArray.GetCount(out var count) != HResult.S_OK || count == 0) {
                return null;
            }

            for (uint i = 0; i < count; i++) {
                IShellItem? shellItem = null;

                try {
                    if (shellItemArray.GetItemAt(i, out shellItem) != HResult.S_OK || shellItem == null) {
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
            if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out pathPtr) != HResult.S_OK || pathPtr == IntPtr.Zero) {
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
}

// ─── Copy file contents command ───────────────────────────────────────────

[ComVisible(true)]
[Guid(Clsids.CopyCommand)]
[ProgId("PasteIntoFile.ShellExtension.CopyCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class CopyCommand : IExplorerCommand {

    public int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Copy);
        return HResult.S_OK;
    }

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        return ExplorerCommandHelpers.GetIcon(out ppszIcon);
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(Clsids.CopyCommand);
        return HResult.S_OK;
    }

    public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = ExplorerCommandHelpers.CheckSingleFileSelected(psiItemArray);
        return HResult.S_OK;
    }

    public int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        try {
            var exePath = ExplorerCommandHelpers.GetPasteIntoFileExecutablePath();
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath)) {
                return HResult.E_FAIL;
            }

            var filePath = ExplorerCommandHelpers.GetSingleItemPath(psiItemArray);
            if (string.IsNullOrEmpty(filePath)) {
                return HResult.E_FAIL;
            }

            var startInfo = new ProcessStartInfo {
                FileName = exePath,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("copy");
            startInfo.ArgumentList.Add(filePath);
            Process.Start(startInfo);
            return HResult.S_OK;
        } catch {
            return HResult.E_FAIL;
        }
    }

    public int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HResult.S_OK;
    }

    public int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        ppEnum = null!;
        return HResult.E_NOTIMPL;
    }
}

// ─── Replace file contents command ────────────────────────────────────────

[ComVisible(true)]
[Guid(Clsids.ReplaceCommand)]
[ProgId("PasteIntoFile.ShellExtension.ReplaceCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class ReplaceCommand : IExplorerCommand {

    public int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Replace);
        return HResult.S_OK;
    }

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        return ExplorerCommandHelpers.GetIcon(out ppszIcon);
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(Clsids.ReplaceCommand);
        return HResult.S_OK;
    }

    public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = ExplorerCommandHelpers.CheckSingleFileSelected(psiItemArray);
        return HResult.S_OK;
    }

    public int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return ExplorerCommandHelpers.InvokePasteIntoFile(psiItemArray, overwrite: true, append: false);
    }

    public int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HResult.S_OK;
    }

    public int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        ppEnum = null!;
        return HResult.E_NOTIMPL;
    }
}

// ─── Append to file command ───────────────────────────────────────────────

[ComVisible(true)]
[Guid(Clsids.AppendCommand)]
[ProgId("PasteIntoFile.ShellExtension.AppendCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class AppendCommand : IExplorerCommand {

    public int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Append);
        return HResult.S_OK;
    }

    public int GetIcon(IShellItemArray psiItemArray, out IntPtr ppszIcon) {
        return ExplorerCommandHelpers.GetIcon(out ppszIcon);
    }

    public int GetToolTip(IShellItemArray psiItemArray, out IntPtr ppszInfotip) {
        ppszInfotip = IntPtr.Zero;
        return HResult.E_NOTIMPL;
    }

    public int GetCanonicalName(out Guid pguidCommandName) {
        pguidCommandName = new Guid(Clsids.AppendCommand);
        return HResult.S_OK;
    }

    public int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = ExplorerCommandHelpers.CheckSingleFileSelectedWithAppendSupport(psiItemArray);
        return HResult.S_OK;
    }

    public int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return ExplorerCommandHelpers.InvokePasteIntoFile(psiItemArray, overwrite: false, append: true);
    }

    public int GetFlags(out EXPCMDFLAGS pFlags) {
        pFlags = EXPCMDFLAGS.ECF_DEFAULT;
        return HResult.S_OK;
    }

    public int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
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
