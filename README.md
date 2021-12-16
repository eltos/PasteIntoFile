# Paste Into File

[![Test Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml)
[![Release Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-release.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/releases)

A Windows desktop application to paste clipboard contents (text and images) into files via the context menu

----------------

_This is a fork of [sorge13248/PasteIntoFile](https://github.com/sorge13248/PasteIntoFile), itself being a fork of [EslaMx7/PasteIntoFile](https://github.com/EslaMx7/PasteIntoFile). Several other forks from out in the wild where cherry-picked and merged here and many improvements implemented._  
_See the [contributors page](https://github.com/eltos/PasteIntoFile/graphs/contributors) for details on collaborators._

----------------

## Features

+ Shortcut in context menu (right click in any folder)
+ Supports text and images (with preview).
+ Resizeable GUI to adjust filename, filetype and path
+ Optional autosave mode (only shows dialog when holding SHIFT)
+ Optional clearing of clipboard after save
+ First launch wizard to register app with Windows Explorer context menu
+ Support for Windows 10 dark theme
+ Support for HiDPI monitors
+ Modern folder selector

![Paste Into File](screenshot.png)  
![Paste Into File kontext menu](screenshot-1.png)


## Installation

+ Make sure you have _.NET Framework 4.5+_ installed in your system. (_Included in Windows 10_)
+ Download the compiled binary from the [Release page](https://github.com/eltos/PasteIntoFile/releases)
+ Run the executable and follow the first-launch wizard

_Tested on Windows 10_

## Command Line Use

Run the following commands in a terminal (Command Prompt or PowerShell).
- To add the *Paste Into File* entry in the File Explorer context menu:
   ```powershell
   PasteIntoFile /reg
   ``` 
- To remove the *Paste Into File* entry from the File Explorer context menu:
   ```powershell
   PasteIntoFile /unreg
   ``` 
- To configure the default filename template format (see [format specifiers](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)):
   ```powershell
   PasteIntoFile /filename "yyyy-MM-dd HH-mm-ss"
   ```
- To explicitly set the target path (and optionally also filename) once:
  ```powershell
  PasteIntoFile \path\to\directory [filename.ext]
  ``` 


