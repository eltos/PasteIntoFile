using System.Runtime.InteropServices;
using PasteIntoFile.Core;

namespace PasteIntoFileShellExtension;

[ComVisible(true)]
[Guid(Clsids.PasteCommand)]
[ProgId("PasteIntoFile.ShellExtension.Command")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class PasteCommand : BaseExplorerCommand {

    public override string Clsid => Clsids.PasteCommand;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        // Reload if the settings file has changed to reflect current autoSave mode
        // while avoiding expensive file I/O on slow storage.
        SettingsUtil.ReloadIfChanged(Settings.Default);

        var title = Titles.Paste;
        if (!Settings.Default.autoSave) {
            title += "…";  // opens a dialog
        }
        ppszName = Marshal.StringToCoTaskMemUni(title);
        return HResult.S_OK;
    }

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        pCmdState = EXPCMDSTATE.ECS_ENABLED;
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        return InvokePasteIntoFile(psiItemArray, PasteMode.Paste);
    }
}
