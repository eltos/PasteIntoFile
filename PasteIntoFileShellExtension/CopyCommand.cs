using System.Runtime.InteropServices;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Clsids.CopyCommand)]
[ProgId("PasteIntoFile.ShellExtension.CopyCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class CopyCommand : BaseExplorerCommand {

    public override string Clsid => Clsids.CopyCommand;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Titles.Copy);
        return HResult.S_OK;
    }

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = CheckSingleFileSelected(psiItemArray);
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return InvokePasteIntoFile(psiItemArray, PasteMode.Copy);
    }
}
