using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

[assembly: DebuggerVisualizer(typeof(ArianKulp.DebuggerExtensions.StringOpenFileDebuggerExtension),
    typeof(VisualizerObjectSource),
    Target = typeof(System.String),
    Description = "View file")]

[assembly: DebuggerVisualizer(typeof(ArianKulp.DebuggerExtensions.StringOpenFileInNotepadDebuggerExtension),
    typeof(VisualizerObjectSource),
    Target = typeof(System.String),
    Description = "View file in Notepad")]

namespace ArianKulp.DebuggerExtensions
{
    public class StringOpenFileDebuggerExtension : DialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException( nameof(windowService));
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

            var fn = objectProvider.GetObject()?.ToString();
            var dte2 = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;
            
            if (dte2 != null && fn != null)
            {
                dte2.ItemOperations.OpenFile(fn);
            }
            else
            {
                if (ThreadHelper.CheckAccess())
                {
                    // This shouldn't ever get here...
                    IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell));
                    Guid clsid = Guid.Empty;
                    int result;
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                        0,
                        ref clsid,
                        "String visualizer extension",
                         string.Format(CultureInfo.CurrentCulture, "Sorry, something went wrong.", this.GetType().FullName),
                        string.Empty,
                        0,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                        OLEMSGICON.OLEMSGICON_INFO,
                        0,
                        out result));
                }
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(StringOpenFileDebuggerExtension));
            visualizerHost.ShowVisualizer();
        }
    }

    public class StringOpenFileInNotepadDebuggerExtension : DialogDebuggerVisualizer
    {
        override protected void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            var fn = objectProvider.GetObject()?.ToString();

            if (fn != null)
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "notepad.exe";
                proc.StartInfo.Arguments = fn;
                proc.Start();
            }
            else
            {
                if (ThreadHelper.CheckAccess())
                {
                    // This shouldn't ever get here...
                    IVsUIShell uiShell = (IVsUIShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell));
                    Guid clsid = Guid.Empty;
                    int result;
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                        0,
                        ref clsid,
                        "String visualizer extension",
                         string.Format(CultureInfo.CurrentCulture, "Sorry, something went wrong.", this.GetType().FullName),
                        string.Empty,
                        0,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                        OLEMSGICON.OLEMSGICON_INFO,
                        0,
                        out result));
                }
            }
        }

        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(StringOpenFileInNotepadDebuggerExtension));
            visualizerHost.ShowVisualizer();
        }
    }
}