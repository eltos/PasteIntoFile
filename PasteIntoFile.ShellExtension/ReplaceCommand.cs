using System.Runtime.InteropServices;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Clsids.ReplaceCommand)]
[ProgId("PasteIntoFile.ShellExtension.ReplaceCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class ReplaceCommand : BaseExplorerCommand {

    public override string Clsid => Clsids.ReplaceCommand;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Replace);
        return HResult.S_OK;
    }

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = CheckSingleFileSelected(psiItemArray);
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return InvokePasteIntoFile(psiItemArray, PasteMode.Replace);
    }
}
