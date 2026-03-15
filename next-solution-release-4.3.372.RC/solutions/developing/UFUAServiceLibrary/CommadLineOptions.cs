using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace UFUAServiceLibrary
{
    public enum Operations : int
    { 
        OpDialog,
        OpInstall,
        OpUninstall,
        OpStart,
        OpStop
    }

    public class CommadLineOptions
    {
        #region Declarations
        const string parInstall = "/I";
        const string parUninstall = "/U";
        const string parDelayedAutoStart = "/K";
        const string parStart = "/S";
        const string parStop = "/T";
        const string parDisplayName = "/Y";
        const string parFilePath = "/F";
        const string parParameters = "/P";
        const string parDependencies = "/D";
        const string parServiceName = "/N";
        const string parUserName = "/A";
        const string parPassword = "/W";
        const string parTitle = "/Z";
        const string parSkin = "/J";
        const string parAddHttpsCertificate = "/H";
        const string parCopyFoldersImages = "/C";
        const string parCopyFoldersDocuments = "/O";
        const string parDeleteFolders = "/R";
        #endregion

        #region Constructors
        public CommadLineOptions() { }
        
        public CommadLineOptions(string[] args)
        {
            Parse(args);
        }
        #endregion

        #region Private Methods
        void Parse(string [] args)
        {
            if (args.Length < 1)
                return;
            Dictionary<string, string> argTable = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                string a = args[i];
                if (a.Length > 2)
                {
                    if (a.Contains(parParameters))
                    {
                        var parValue = a.Substring(4);
                        if (parValue.Contains(Properties.Settings.Default.SearchProjectPathParameter))
                        {
                            var filePath = parValue.Substring(Properties.Settings.Default.SearchProjectPathParameter.Length);
                            if (System.IO.File.Exists(filePath))
                            {
                                ProjectPath = System.IO.Path.GetDirectoryName(filePath);
                            }
                            else
                            {
                                filePath = XpoHelpers.XpoHelper.GetDataSourceFilePath(filePath);
                                if (filePath != null && System.IO.File.Exists(filePath))
                                {
                                    var rootPath = System.IO.Path.GetDirectoryName(filePath);
                                    var splits = rootPath.Split('\\', '/');
                                    if (splits.Length > 0)
                                    {
                                        rootPath = splits[0];
                                        for (int n = 1; n < splits.Length - 2; ++n)
                                            rootPath = String.Format("{0}{1}{2}", rootPath, System.IO.Path.DirectorySeparatorChar, splits[n]);
                                        ProjectPath = rootPath;
                                    }
                                }
                            }
                        }

                        Parameters += string.Format("\"{0}\" ", parValue);
                        //argTable[a.Substring(0, 4).ToUpper()] = a.Substring(4);
                    }
                    else
                        argTable[a.Substring(0, 2).ToUpper()] = a.Substring(2);
                }
                else
                {
                    argTable[a.ToUpper()] = string.Empty;
                }
            }
            if (argTable.ContainsKey(parInstall))
            {
                Operation = Operations.OpInstall;
            }
            else if (argTable.ContainsKey(parUninstall))
            {
                Operation = Operations.OpUninstall;
            }
            else if (argTable.ContainsKey(parStart))
            {
                Operation = Operations.OpStart;
            }
            else if (argTable.ContainsKey(parStop))
            {
                Operation = Operations.OpStop;
            }
            else
            {
                Operation = Operations.OpDialog;
            }

            if (argTable.ContainsKey(parServiceName))
                ServiceName = argTable[parServiceName];
            if (argTable.ContainsKey(parDisplayName))
                DisplayName = argTable[parDisplayName];
            if (argTable.ContainsKey(parFilePath))
                FilePath = argTable[parFilePath];
            //if (argTable.ContainsKey(parParameters))
            //    Parameters = argTable[parParameters];
            if (argTable.ContainsKey(parDependencies))
                Dependencies = argTable[parDependencies];
            if (argTable.ContainsKey(parDelayedAutoStart))
                DelayedAutoStart = true;
            if (argTable.ContainsKey(parUserName))
                UserName = argTable[parUserName];
            if (argTable.ContainsKey(parPassword))
                Password = argTable[parPassword];
            if (argTable.ContainsKey(parTitle) && !String.IsNullOrEmpty(argTable[parTitle]))
                Title = argTable[parTitle];
            if (argTable.ContainsKey(parSkin))
                CurrentSkin = argTable[parSkin];
            if (argTable.ContainsKey(parAddHttpsCertificate))
                AddHttpsCertificate = true;
            if (argTable.ContainsKey(parCopyFoldersImages))
            {
                var folders = argTable[parCopyFoldersImages].Split(';');
                CopySourceFolderImages = folders[0];
                if (folders.Length > 1)
                    CopyDestFolderImages = folders[1];
            }
            if (argTable.ContainsKey(parCopyFoldersDocuments))
            {
                var folders = argTable[parCopyFoldersDocuments].Split(';');
                CopySourceFolderDocuments = folders[0];
                if (folders.Length > 1)
                    CopyDestFolderDocuments = folders[1];
            }
            if (argTable.ContainsKey(parDeleteFolders))
                DeleteFolders = argTable[parDeleteFolders];
        }
        #endregion

        #region Public Methods
        public bool CheckOptions(bool showError = false)
        {
            bool bValid = !String.IsNullOrEmpty(ServiceName) && ServiceName.Length > 0;
            if (Operation == Operations.OpInstall)
                bValid &= !String.IsNullOrEmpty(FilePath) && FilePath.Length > 0;

            if (bValid)
                return true;

            if (showError)
                MessageBox.Show(UFUAServiceLibrary.Properties.Resources.InvalidOptions, UFUAServiceLibrary.Properties.Resources.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);

            return false;
        }
        #endregion

        #region Public Properties
        private Operations _Operation = Operations.OpDialog;
        public Operations Operation
        {
            get { return _Operation; }
            set
            {
                _Operation = value;
            }
        }

        private string _ServiceName;
        public string ServiceName
        {
            get { return _ServiceName; }
            set
            {
                _ServiceName = value;
            }
        }

        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set
            {
                _DisplayName = value;
            }
        }

        private string _FilePath;
        public string FilePath
        {
            get { return _FilePath; }
            set
            {
                _FilePath = value;
            }
        }

        private string _ProjectPath;
        public string ProjectPath
        {
            get { return _ProjectPath; }
            set
            {
                _ProjectPath = value;
            }
        }

        private string _Parameters;
        public string Parameters
        {
            get { return _Parameters; }
            set
            {
                _Parameters = value;
            }
        }

        private string _Dependencies;
        public string Dependencies
        {
            get { return _Dependencies; }
            set
            {
                _Dependencies = value;
            }
        }

        private bool _DelayedAutoStart;
        public bool DelayedAutoStart
        {
            get { return _DelayedAutoStart; }
            set
            {
                _DelayedAutoStart = value;
            }
        }


        private string _UserName = String.Empty;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
            }
        }

        private string _Password = String.Empty;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
            }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
            }
        }

        public string CurrentSkin
        {
            get;
            set;
        } = "Blend";

        public bool AddHttpsCertificate { get; private set; }
        public string CopySourceFolderImages { get; private set; }
        public string CopyDestFolderImages { get; private set; }
        public string CopyDestFolderDocuments { get; private set; }
        public string CopySourceFolderDocuments { get; private set; }
        public string DeleteFolders { get; private set; }
        #endregion
    }
}
