using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

[assembly: DebuggerVisualizer(typeof(ArianKulp.DebuggerExtensions.StringOpenFileDebuggerExtension),
    typeof(VisualizerObjectSource),
    Target = typeof(System.String),
    Description = "[as filename] Open file in Visual Studio")]

[assembly: DebuggerVisualizer(typeof(ArianKulp.DebuggerExtensions.StringOpenFileInNotepadDebuggerExtension),
    typeof(VisualizerObjectSource),
    Target = typeof(System.String),
    Description = "[as filename] Open file in Notepad")]

[assembly: DebuggerVisualizer(typeof(ArianKulp.DebuggerExtensions.StringLocateFileDebuggerExtension),
    typeof(VisualizerObjectSource),
    Target = typeof(System.String),
    Description = "[as filename] Locate file in Explorer")]

namespace ArianKulp.DebuggerExtensions
{
    public abstract class BaseDialogDebuggerVisualizer : DialogDebuggerVisualizer
    {
        protected void ShowError(string msg = "Sorry, something went wrong")
        {
            ShowDialog(msg, OLEMSGICON.OLEMSGICON_CRITICAL);
        }

        protected void ShowDialog(string msg, OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO)
        {
            if (ThreadHelper.CheckAccess())
            {
                // This shouldn't ever get here...
                IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell));
                Guid clsid = Guid.Empty;
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    "String visualizer extension",
                     string.Format(CultureInfo.CurrentCulture, msg, this.GetType().FullName),
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    icon,
                    0,
                    out int result));
            }
        }
    }

    public class StringLocateFileDebuggerExtension : BaseDialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService));
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            var fn = objectProvider.GetObject()?.ToString();

            if (fn != null && ServiceProvider.GlobalProvider.GetService(typeof(DTE)) is DTE2 dte2)
            {
                if (!File.Exists(fn) && !Directory.Exists(fn))
                {
                    ShowError("File/folder does not exist");
                    return;
                }

                var attr = File.GetAttributes(fn);
                var isFolder = (attr & FileAttributes.Directory) != 0;

                // combine the arguments together
                // it doesn't matter if there is a space after ','
                string argument = isFolder ? $"\"{fn}\"" : $"/select, \"{fn}\"";

                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
            else
            {
                ShowError();
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(StringLocateFileDebuggerExtension));
            visualizerHost.ShowVisualizer();
        }
    }

    public class StringOpenFileDebuggerExtension : BaseDialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService));
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            var fn = objectProvider.GetObject()?.ToString();

            if (fn != null && ServiceProvider.GlobalProvider.GetService(typeof(DTE)) is DTE2 dte2)
            {
                if (!File.Exists(fn))
                {
                    ShowError("File does not exist");
                    return;
                }

                dte2.ItemOperations.OpenFile(fn);
            }
            else
            {
                ShowError();
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(StringOpenFileDebuggerExtension));
            visualizerHost.ShowVisualizer();
        }
    }

    public class StringOpenFileInNotepadDebuggerExtension : BaseDialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var fn = objectProvider.GetObject()?.ToString();

            if (fn != null)
            {
                if (!File.Exists(fn))
                {
                    ShowError("File does not exist");
                    return;
                }

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "notepad.exe";
                proc.StartInfo.Arguments = fn;
                proc.Start();
            }
            else
            {
                ShowError();
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(StringOpenFileInNotepadDebuggerExtension));
            visualizerHost.ShowVisualizer();
        }
    }
}