using System.Runtime.InteropServices;
using PasteIntoFile.Core.Properties;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Clsids.AppendCommand)]
[ProgId("PasteIntoFile.ShellExtension.AppendCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class AppendCommand : BaseExplorerCommand {

    public override string Clsid => Clsids.AppendCommand;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Resources.str_contextentry_append);
        return HResult.S_OK;
    }

    // File extensions that do NOT support appending (binary / image formats)
    private static readonly string[] AppendNotSupportedExtensions = new[] {
        "bmp", "gif", "ico", "jpeg", "jpg", "png", "tif", "tiff", "webp", // images
        "svg", // vector
        "url"  // shortcuts
    };

    // Check if appending is supported for the given file path.
    private static bool IsAppendSupported(string? path) {
        if (string.IsNullOrEmpty(path)) {
            return true;
        }
        var ext = Path.GetExtension(path)?.TrimStart('.').ToLowerInvariant();
        return string.IsNullOrEmpty(ext) || Array.IndexOf(AppendNotSupportedExtensions, ext) < 0;
    }

    // Check if exactly one file is selected and append is supported for it.
    // Returns ECS_ENABLED if so, otherwise ECS_HIDDEN.
    private static EXPCMDSTATE CheckSingleFileSelectedWithAppendSupport(IShellItemArray psiItemArray) {
        if (psiItemArray != null && psiItemArray.GetCount(out var count) == HResult.S_OK && count == 1) {
            psiItemArray.GetItemAt(0, out IShellItem? shellItem);
            if (shellItem != null) {
                try {
                    if (shellItem.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var pathPtr) == HResult.S_OK && pathPtr != IntPtr.Zero) {
                        var path = Marshal.PtrToStringUni(pathPtr);
                        Marshal.FreeCoTaskMem(pathPtr);
                        if (!string.IsNullOrEmpty(path) && IsAppendSupported(path)) {
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

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = CheckSingleFileSelectedWithAppendSupport(psiItemArray);
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return InvokePasteIntoFile(psiItemArray, PasteMode.Append);
    }
}
