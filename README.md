StringAsFilenameDebuggerVisualizer
==================================

Arian T. Kulp
-------------

Debugger visualizers for Visual Studio 2017 to open/view files by taking a string
variable value as the filename.

Install using prebuilt VSIX or install manually by copying the built DLL
(ArianKulp.DebuggerExtensions.dll) to \$UserProfileStudio 2017.

Note, this actually adds multiple visualizers so you can locate the file in Windows Explorer, open the file in Visual
Studio (without adding it to the project/solution), or in Notepad.

### Release History

-   v1.1

    -   Update to proper installation of visualizer via VSIX

    -   Added “Locate file in Explorer” command

-   v1.0

    -   Initial release

### Known bugs:

None at the moment

### Future plans

-   Add settings for choosing text editor other than Notepad

-   Possibly add option to save current string value into temp file and then
    open it

-   Add option to Peek at file contents in current window