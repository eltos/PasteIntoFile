using System.Collections;
using System.Runtime.InteropServices;

namespace PasteIntoFileShellExtension;

/// <summary>
/// COM-visible enumerator for IExplorerCommand subcommands.
/// Wraps a list of IExplorerCommand instances and exposes them via IEnumExplorerCommand.
/// </summary>
[ComVisible(true)]
[Guid("B2E4F8A1-3D5C-4B9E-A7F1-9C8D6E5A4B3C")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IEnumExplorerCommand))]
public class ExplorerCommandEnumerator : IEnumExplorerCommand {
    private readonly IEnumerator<IExplorerCommand> _enumerator;
    private IExplorerCommand? _current;

    public ExplorerCommandEnumerator(IList<IExplorerCommand> commands) {
        _enumerator = commands.GetEnumerator();
    }

    public int Next(uint celt, out IExplorerCommand rgelt, out uint pceltFetched) {
        rgelt = null!;
        pceltFetched = 0;

        if (celt == 0) {
            return HResult.S_OK;
        }

        if (_enumerator.MoveNext()) {
            _current = (IExplorerCommand)_enumerator.Current;
            rgelt = _current;
            pceltFetched = 1;
            return HResult.S_OK;
        }

        return HResult.S_FALSE;
    }

    public int Skip(uint celt) {
        for (uint i = 0; i < celt; i++) {
            if (!_enumerator.MoveNext()) {
                return HResult.S_FALSE;
            }
        }
        return HResult.S_OK;
    }

    public int Reset() {
        _enumerator.Reset();
        _current = null;
        return HResult.S_OK;
    }

    public int Clone(out IEnumExplorerCommand ppEnum) {
        // Clone is not supported; return E_NOTIMPL
        ppEnum = null!;
        return HResult.E_NOTIMPL;
    }
}
