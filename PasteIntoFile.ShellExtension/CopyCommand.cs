using System.Runtime.InteropServices;
using PasteIntoFile.Core.Properties;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Clsids.CopyCommand)]
[ProgId("PasteIntoFile.ShellExtension.CopyCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class CopyCommand : BaseExplorerCommand {

    public override string Clsid => Clsids.CopyCommand;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Resources.str_contextentry_copyfromfile);
        return HResult.S_OK;
    }

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        // Show if exactly one item is selected
        if (psiItemArray != null && psiItemArray.GetCount(out var count) == HResult.S_OK && count == 1) {
            pCmdState = EXPCMDSTATE.ECS_ENABLED;
        } else {
            pCmdState = EXPCMDSTATE.ECS_HIDDEN;
        }
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return InvokePasteIntoFile(psiItemArray, PasteMode.Copy);
    }
}
