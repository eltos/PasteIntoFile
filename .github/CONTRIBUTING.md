# Contributing

All contributions are welcome. Thank you for contributing!

If you are new, check out the **[contribution page](https://github.com/eltos/PasteIntoFile/contribute)** for good first issues to address.


## Developer setup

Check the `TargetFrameworkVersion` in [PasteIntoFile.csproj](https://github.com/eltos/PasteIntoFile/blob/main/PasteIntoFile/PasteIntoFile.csproj)
and then ownload and install the [.NET Framework developer pack](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks#supported-versions-framework)
for this version.


This project uses [pre-commit](https://pre-commit.com) with [dotnet-format](https://github.com/dotnet/format) to ensure consistent file formatting and clean diffs.
Please install these tools before committing:
```bash
dotnet tool install --global dotnet-format --version 5.1.250801
pip install pre-commit
pre-commit install
dotnet restore
```

Other useful tools:
- [InsideClipboard](https://www.nirsoft.net/utils/inside_clipboard.html) for analyzing clipboard structure
- [JetBrains Rider](https://www.jetbrains.com/rider/) .NET IDE free for non-commercial use

## CI/CD
This project uses GitHub actions for test builds, synchronisation with crowdin and releases.

## Licensing
Contributions follow the "inbound=outbound" licensing as defined by the [GitHub Terms of Service](https://help.github.com/articles/github-terms-of-service/#6-contributions-under-repository-license).
