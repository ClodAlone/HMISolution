using System;
using System.IO;
using IWshRuntimeLibrary;

namespace WPFUtilities
{
    public class WindowsApplicationShortcut
    {
        #region Declarations
        readonly string linkName;
        #endregion

        #region Constructors
        public WindowsApplicationShortcut(string linkName, string executablePath)
        {
            this.linkName = linkName;
            TargetPath = executablePath;
            WorkingDirectory = System.IO.Path.GetDirectoryName(TargetPath);
            SetFullName();
        }
        #endregion

        #region Properties
        Environment.SpecialFolder directoryPath = Environment.SpecialFolder.Desktop;
        public Environment.SpecialFolder DirectoryPath
        {
            get
            {
                return directoryPath;
            }
            set
            {
                if (directoryPath == value)
                    return;
                directoryPath = value;
                SetFullName();
            }
        }

        public String FullName { get; private set; }

        public String Description { get; set; }

        public String TargetPath { get; set; }

        public String WorkingDirectory { get; set; }

        public String Arguments { get; set; }
        #endregion

        #region Public Methods
        public void CreateShortcut()
        {
            var shell = new WshShell();
            var shortcut = (IWshShortcut)shell.CreateShortcut(FullName);
            shortcut.Description = Description;
            shortcut.TargetPath = TargetPath;
            shortcut.WorkingDirectory = WorkingDirectory;
            shortcut.Arguments = Arguments;
            shortcut.Save();
        }
        #endregion

        #region Private Methods
        void SetFullName()
        {
            FullName = Path.Combine(Environment.GetFolderPath(DirectoryPath), String.Format("{0}.lnk", linkName));
        }
        #endregion
    }
}
