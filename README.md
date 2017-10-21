# StringAsFilenameDebuggerVisualizer
## Arian T. Kulp
Debugger visualizer for Visual Studio 2017 to open a file by taking a string variable value as the filename.

Use prepackaged release or install manually by copying the built DLL (ArianKulp.DebuggerExtensions.dll) to $UserProfile\Documents\Visual Studio 2017\Visualizers.  Though the project contains a VSIX builder and a VSPackage, they are not functional as managed extensions **can't** be installed via VSIX.

Note, this actually adds two new visualizers so you can open the file in Visual Studio (without adding it to the project/solution), or in Notepad.

### Known bugs:
None at the moment

### Future plans
- Add settings for choosing text editor other than Notepad
- Possibly add option to save current string value into temp file and then open it