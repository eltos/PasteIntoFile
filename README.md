# Paste Into File

[![Test Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-testbuild.yml)
[![Release Build](https://github.com/eltos/PasteIntoFile/actions/workflows/dotnet-release.yml/badge.svg)](https://github.com/eltos/PasteIntoFile/releases)

A Windows desktop application to paste clipboard contents (text, images and URLs) into files via the context menu

----------------

_This is a fork of [sorge13248/PasteIntoFile](https://github.com/sorge13248/PasteIntoFile), itself being a fork of [EslaMx7/PasteIntoFile](https://github.com/EslaMx7/PasteIntoFile)._
_See the [contributors page](https://github.com/eltos/PasteIntoFile/graphs/contributors) for details on collaborators._  

_This fork contains many new core functionalities such as clipboard monitoring, batch mode and rename inside the file explorer. In addition, the GUI was completely redesigned to make the layout resizeable and allow for comfortable text and image preview._
_The full changelog can be found on the [release page](https://github.com/eltos/PasteIntoFile/releases)._

----------------

## Features

+ Explorer context menu entry
+ [Autosave mode](https://github.com/eltos/PasteIntoFile/discussions/2): rename inside file explorer without dialog
+ [Batch mode](https://github.com/eltos/PasteIntoFile/discussions/4): monitor clipboard and save on change
+ First launch wizard

![Paste Into File](screenshot.png)  
![Paste Into File kontext menu](screenshot-1.png)


## Installation

+ Make sure you have _.NET Framework 4.8+_ installed (_Included in Windows 10_)
+ Download the latest PasteIntoFile.zip from the [Release page](https://github.com/eltos/PasteIntoFile/releases) and unzip to a location of your choice
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


