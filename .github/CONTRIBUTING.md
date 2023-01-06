# Contributing

All contributions are welcome. Thank you for contributing!

If you are new, check out the **[contribution page](https://github.com/eltos/PasteIntoFile/contribute)** for good first issues to address.


## Developer setup

This project uses [pre-commit](https://pre-commit.com) with [dotnet-format](https://github.com/dotnet/format) to ensure consistent file formatting and clean diffs.
Please install these tools before committing:
```bash
dotnet tool install --global dotnet-format --version 5.1.250801
pip install pre-commit
pre-commit install
```

## CI/CD
This project uses GitHub actions for test builds, synchronisation with crowdin and releases.
