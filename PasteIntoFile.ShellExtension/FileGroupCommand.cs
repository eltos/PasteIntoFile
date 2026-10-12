using System.Runtime.InteropServices;
using PasteIntoFile.Core.Properties;

namespace PasteIntoFileShellExtension;

// Shared COM class ID for the file group command
static class FileGroupClsid {
    public const string Value = "109EFE3F-786B-4DFA-B639-4B673C05B932";
}

/// <summary>
/// Parent group command for file context menus.
/// Displays "Paste Into File" with a right-arrow submenu containing Copy, Replace, and Append.
/// </summary>
[ComVisible(true)]
[Guid(FileGroupClsid.Value)]
[ProgId("PasteIntoFile.ShellExtension.FileGroupCommand")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IExplorerCommand))]
public sealed class FileGroupCommand : BaseExplorerCommand {

    public override string Clsid => FileGroupClsid.Value;

    public override int GetTitle(IShellItemArray psiItemArray, out IntPtr ppszName) {
        ppszName = Marshal.StringToCoTaskMemUni(Resources.str_contextentry);
        return HResult.S_OK;
    }

    public override int GetState(IShellItemArray psiItemArray, bool fOkToBeSlow, out EXPCMDSTATE pCmdState) {
        // Show the group if exactly one item is selected
        if (psiItemArray != null && psiItemArray.GetCount(out var count) == HResult.S_OK && count == 1) {
            pCmdState = EXPCMDSTATE.ECS_ENABLED;
        } else {
            pCmdState = EXPCMDSTATE.ECS_HIDDEN;
        }
        return HResult.S_OK;
    }

    public override int Invoke(IShellItemArray psiItemArray, IntPtr pbc) {
        // The group itself is not invokable; only its subcommands are.
        return HResult.E_NOTIMPL;
    }

    public override int GetFlags(out EXPCMDFLAGS pFlags) {
        // Mark this command as having subcommands so Explorer shows the right-arrow indicator
        pFlags = EXPCMDFLAGS.ECF_HASSUBCOMMANDS;
        return HResult.S_OK;
    }

    public override int EnumSubCommands(out IEnumExplorerCommand ppEnum) {
        // Return the subcommands in the desired order: Copy, Replace, Append
        var subCommands = new List<IExplorerCommand> {
            new ReplaceCommand(),
            new AppendCommand(),
            new CopyCommand()
        };

        ppEnum = new ExplorerCommandEnumerator(subCommands);
        return HResult.S_OK;
    }
}
