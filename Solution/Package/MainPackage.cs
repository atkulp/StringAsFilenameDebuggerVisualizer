using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace ArianKulp.DebuggerExtensions
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    //[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(MainPackage.PackageGuidString)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class MainPackage : Package
    {
        /// <summary>
        /// StringDebuggerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "578EB6FB-F915-480A-B4A8-D850C955BFBC";

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPackage"/> class.
        /// </summary>
        public MainPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            //mostly gotten from https://github.com/visualstudioextensibility/VSX-Samples/tree/master/VSIXDebuggerVisualizer
            var PAYLOAD_FILE_NAMES = new List<string>() { "ArianKulp.StringExtensions.dll" };

            string sourceFolderFullName;
            string destinationFolderFullName;
            IVsShell shell;
            object documentsFolderFullNameObject = null;
            string documentsFolderFullName;

            try
            {
                base.Initialize();

                // The Visualizer dll is in the same folder than the package because its project is added as reference to this project,
                // so it is included inside the .vsix file. We only need to deploy it to the correct destination folder.
                sourceFolderFullName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                // Get the destination folder for visualizers
                shell = base.GetService(typeof(SVsShell)) as IVsShell;
                shell.GetProperty((int)__VSSPROPID2.VSSPROPID_VisualStudioDir, out documentsFolderFullNameObject);
                documentsFolderFullName = documentsFolderFullNameObject.ToString();
                destinationFolderFullName = Path.Combine(documentsFolderFullName, "Visualizers");

                foreach (var payload in PAYLOAD_FILE_NAMES)
                {
                    var sourceFileFullName = Path.Combine(sourceFolderFullName, payload);
                    var destinationFileFullName = Path.Combine(destinationFolderFullName, payload);

                    CopyFileIfNewerVersion(sourceFileFullName, destinationFileFullName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private void CopyFileIfNewerVersion(string sourceFileFullName, string destinationFileFullName)
        {
            FileVersionInfo destinationFileVersionInfo;
            FileVersionInfo sourceFileVersionInfo;
            bool copy = false;

            if (File.Exists(destinationFileFullName))
            {
                sourceFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(sourceFileFullName);
                destinationFileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(destinationFileFullName);
                if (sourceFileVersionInfo.FileMajorPart > destinationFileVersionInfo.FileMajorPart)
                {
                    copy = true;
                }
                else if (sourceFileVersionInfo.FileMajorPart == destinationFileVersionInfo.FileMajorPart
                   && sourceFileVersionInfo.FileMinorPart > destinationFileVersionInfo.FileMinorPart)
                {
                    copy = true;
                }
            }
            else
            {
                // First time
                copy = true;
            }

            if (copy)
            {
                File.Copy(sourceFileFullName, destinationFileFullName, true);
            }
        }

        #endregion
    }
}
