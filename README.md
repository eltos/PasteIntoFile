# Paste Into File

[![Test Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml)
[![Release Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-release.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/releases)

A Windows desktop application to paste clipboard contents into files and copy file contents to the clipboard via the context menu

----------------

_This is a fork of [sorge13248/PasteIntoFile](https://github.com/sorge13248/PasteIntoFile), itself being a fork of [EslaMx7/PasteIntoFile](https://github.com/EslaMx7/PasteIntoFile)._
_See the [contributors page](https://github.com/eltos/PasteIntoFile/graphs/contributors) for details on collaborators._  

_This fork contains many new core functionalities such as clipboard monitoring, batch mode and rename inside the file explorer. In addition, the GUI was completely redesigned to make the layout resizeable and allow for comfortable text and image preview._
_The full changelog can be found on the [release page](https://github.com/eltos/PasteIntoFile/releases)._

----------------

## Features

+ Explorer context menu entry: "Paste into file" or "Copy file contents"
+ [Autosave mode](https://github.com/eltos/PasteIntoFile/discussions/2): rename inside file explorer without dialog
+ [Batch mode](https://github.com/eltos/PasteIntoFile/discussions/4): monitor clipboard and save on change
+ First launch wizard

![Paste Into File](screenshot.png)  
![Paste Into File kontext menu](screenshot-1.png)


## Installation

+ Make sure you have _.NET Framework 4.8+_ installed (_Included in Windows 10_)
+ **[Download the latest version from the release page](https://github.com/eltos/PasteIntoFile/releases)**
  + You can use the installer (.msi file)  
    _Note: since it's unsigned, windows will "protect" you from installing an unknown app. Click "More info" and "Run anyway" to proceed._
  + Or you can download the portable PasteIntoFile.zip, unzip it's contents to a location of your choice and launch the executable to bring up the first-launch wizard

_Tested on Windows 10_

## Command Line Use

Use `help`, `help paste`, `help config` etc. to show available command line options, e.g.:
```powershell
> .\PasteIntoFile.exe help
PasteIntoFile 3.11.0.0
Copyright © PasteIntoFile GitHub contributors

  paste      (Default Verb) Paste clipboard contents into file
  copy       Copy file contents to clipboard
  config     Change configuration (without saving clipboard)
  wizard     Open the first-launch wizard
  help       Display more information on a specific command.
  version    Display version information.

> .\PasteIntoFile.exe help paste
PasteIntoFile 3.11.0.0
Copyright © PasteIntoFile GitHub contributors

  -d, --directory      Path of directory to save file into
  -f, --filename       Filename template (may contain format variables such as {0:yyyyMMdd HHmmSS})
  --text-extension     File extension for text contents
  --image-extension    File extension for image contents
  -c, --clear          Clear clipboard after save (true/false)
  -a, --autosave       Autosave file without prompt (true/false)
  --help               Display this help screen.
  --version            Display version information.
```

**Examples:**
- Add the *Paste Into File* entry in the File Explorer context menu:
   ```powershell
   PasteIntoFile config --register
   ``` 
- Configure the default filename template format (see [format specifiers](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)):
   ```powershell
   PasteIntoFile config -f "{0:yyyy-MM-dd HH-mm-ss}"
   ```
- Save clipboard contents in autosave mode to specific location:
  ```powershell
  PasteIntoFile -d the/directory -f the_filename --autosave=true
  ``` 
- Copy file contents to clipboard:
  ```powershell
  PasteIntoFile copy path_to/the_file
  ``` 


