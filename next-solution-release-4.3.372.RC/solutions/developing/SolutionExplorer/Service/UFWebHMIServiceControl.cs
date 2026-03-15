using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UFInterfaces.Service;
using UFProjectManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using Utilities;

namespace UFProjectManager.Service
{
    internal class UFWebHMIServiceControl : IServiceControl
    {
        enum CommandType
        {
            Open,
            Install,
            Uninstall,
            Start,
            Stop
        }

        #region Declarations
        readonly string applicationName;
        readonly string projectUri;
        readonly Uri documentsUri;
        readonly Uri imageUri;
        readonly static string processFileName = System.IO.Path.ChangeExtension(Properties.Settings.Default.CheckedAssemblyName_WebHMI, ".exe");
        readonly static string processConfigFileName = "appsettings.json";
        readonly static string projectArgument = "--WebNExTHMISettings:Project=";

        readonly static ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);

        string serviceName;
        string arguments;
        string newProjectUri;
        #endregion

        #region Constructors
        public UFWebHMIServiceControl(string serviceName)
        {
            this.serviceName = serviceName;
        }

        public UFWebHMIServiceControl(string applicationName, string projectUri, Uri imageUri, Uri documentsUri)
        {
            this.applicationName = applicationName;
            this.projectUri = projectUri;
            this.imageUri = imageUri;
            this.documentsUri = documentsUri;
        }
        #endregion

        #region Properties
        string DisplayName
        {
            get
            {
                return String.Format("{0} ({1})", UFInterfaces.Properties.Resources.WebHMI_ServiceDisplayName, applicationName);
            }
        }

        #endregion

        #region IServiceControl
        public string FriendlyName
        {
            get
            {
                return UFInterfaces.Properties.Resources.WebHMI_ServiceDisplayName;
            }
        }

        public string Name
        {
            get
            {
                if (serviceName == null)
                {
                    var processName = System.IO.Path.GetFileNameWithoutExtension(processFileName);
                    serviceName = ServiceInstaller.ServiceInstaller.GetValidServiceName(String.Format("{0} ({1})", processName, applicationName));
                }

                return serviceName;
            }
        }

        public bool UseCredentialProvider
        {
            get
            {
                return false;
            }
        }

        public void Install()
        {
            newProjectUri = GetProjectFilePath();
            if (!String.IsNullOrEmpty(newProjectUri))
            {
                PrepareArguments(CommandType.Install);
                Execute();
            }
        }

        public void Install(string username, string password)
        {
            newProjectUri = GetProjectFilePath();
            if (!String.IsNullOrEmpty(newProjectUri))
            {
                PrepareArguments(CommandType.Install, username, password);
                Execute();
            }
        }

        public void OpenServiceControl()
        {
            PrepareArguments(CommandType.Open);
            Execute();
        }

        public void Start()
        {
            PrepareArguments(CommandType.Start);
            Execute();
        }

        public void Stop()
        {
            PrepareArguments(CommandType.Stop);
            Execute();
        }

        public void Uninstall()
        {
            PrepareArguments(CommandType.Uninstall);
            Execute();
        }
        #endregion

        #region Methods
        void PrepareArguments(CommandType type)
        {
            PrepareArguments(type, null, null);
        }

        void PrepareArguments(CommandType type, string username, string password)
        {
            arguments = string.Format(@"""/N{0}"" ""/Y{1}"" ""/F{2}""", Name, DisplayName, GetWebServiceFullPath());
            if (type == CommandType.Install)
            {
                if (!String.IsNullOrEmpty(newProjectUri))
                {
                    arguments = string.Format(@"{0} ""/P01{1}{2}""", arguments, projectArgument, newProjectUri);

                    var imgSourceFolder = imageUri.GetPathString();
                    if (!imageUri.IsAbsoluteUri)
                        imgSourceFolder = String.Format("{0}{1}", System.IO.Path.GetDirectoryName(newProjectUri), imgSourceFolder);
                    if (System.IO.Directory.Exists(imgSourceFolder))
                    {
                        var dir = new System.IO.DirectoryInfo(imgSourceFolder);
                        if (dir.Exists && (dir.GetDirectories().Length > 0 || dir.GetFiles().Length > 0))
                        {
                            var destFolder = System.IO.Path.Combine(GetWebServiceImagePath(), System.IO.Path.GetFileNameWithoutExtension(newProjectUri));
                            arguments = string.Format(@"{0} ""/C{1};{2}""", arguments, imgSourceFolder, destFolder);
                        }
                    }

                    var docSourceFolder = documentsUri.GetPathString();
                    if (!documentsUri.IsAbsoluteUri)
                        docSourceFolder = String.Format("{0}{1}", System.IO.Path.GetDirectoryName(newProjectUri), docSourceFolder);
                    if (System.IO.Directory.Exists(docSourceFolder))
                    {
                        var dir = new System.IO.DirectoryInfo(docSourceFolder);
                        if (dir.Exists && (dir.GetDirectories().Length > 0 || dir.GetFiles().Length > 0))
                        {
                            var destFolder = System.IO.Path.Combine(GetWebServiceDocumentsPath(), System.IO.Path.GetFileNameWithoutExtension(newProjectUri));
                            arguments = string.Format(@"{0} ""/O{1};{2}""", arguments, docSourceFolder, destFolder);
                        }
                    }
                }
                arguments = string.Format("/I {0}", arguments);
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                    arguments = string.Format("{0} /A{1} /W{2}", arguments, username, password);
            }
            else if (type == CommandType.Uninstall)
            {
                arguments = string.Format("/U {0}", arguments);

                var currentProjectUri = newProjectUri;
                if (String.IsNullOrEmpty(currentProjectUri))
                {
                    var pathName = TryGetPathNameByServiceName(Name);
                    if (!String.IsNullOrEmpty(pathName))
                    {
                        var index1 = pathName.IndexOf(projectArgument);
                        if (index1 != -1)
                        {
                            index1 = index1 + projectArgument.Length;
                            var index2 = Math.Max(0, pathName.IndexOf("\"", index1));
                            if (index2 > 0)
                                currentProjectUri = pathName.Substring(index1, index2 - index1);
                            else
                                currentProjectUri = pathName.Substring(index1);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(currentProjectUri))
                { 
                    var imgDestFolder = System.IO.Path.Combine(GetWebServiceImagePath(), System.IO.Path.GetFileNameWithoutExtension(currentProjectUri));
                    var imgDir = new System.IO.DirectoryInfo(imgDestFolder);
                    var docDestFolder = System.IO.Path.Combine(GetWebServiceDocumentsPath(), System.IO.Path.GetFileNameWithoutExtension(currentProjectUri));
                    var docDir = new System.IO.DirectoryInfo(docDestFolder);
                    if (imgDir.Exists || docDir.Exists)
                    {
                        var foldersPar = "";
                        if (imgDir.Exists)
                            foldersPar = imgDestFolder;
                        if (docDir.Exists)
                            foldersPar = String.IsNullOrEmpty(foldersPar) ? docDestFolder : String.Format("{0};{1}", foldersPar, docDestFolder);
                        if (!String.IsNullOrEmpty(foldersPar))
                            arguments = string.Format(@"{0} ""/R{1}""", arguments, foldersPar);
                    }
                }
            }
            else if (type == CommandType.Start)
                arguments = string.Format("/S {0}", arguments);
            else if (type == CommandType.Stop)
                arguments = string.Format("/T {0}", arguments);

            var configFilePath = GetWebServiceConfigPath();
            if (System.IO.File.Exists(configFilePath))
            {
                var configObject = JObject.Parse(System.IO.File.ReadAllText(configFilePath));
                if (configObject["WebNExTHMISettings"] != null && 
                    configObject["WebNExTHMISettings"]["HttpsListeningPort"] != null &&
                    (string)configObject["WebNExTHMISettings"]["HttpsListeningPort"] != "0")
                    arguments = string.Format("/H {0}", arguments);
            }
        }

        void Execute()
        {
            string path = Properties.Settings.Default.ServiceManagerExecutable;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }

        string GetProjectFilePath()
        {
            if (XpoHelpers.XpoHelper.IsDataSource(projectUri))
            {
                var filter = String.Format("({0})|*{1}",
                    UFProjectManagerComponent.projectManagerComponent.TypeLabel,
                    UFProjectManagerComponent.projectManagerComponent.FileType);
                var svgPath = UFProjectManagerComponent.projectManagerComponent.UIInterface.ShowSelectFileDialog(filter);
                if (svgPath != null && svgPath.Length > 0)
                    return svgPath[0];
                else
                    return null;
            }
            else
                return projectUri;
        }

        string GetWebServiceFullPath()
        {
            string rootPath = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                rootPath = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            rootPath = String.Format("{0}\\{1}\\{2}", rootPath ?? ".", Properties.Settings.Default.ServerWebHMIRootFolder, processFileName);

            return rootPath;
        }

        string GetWebServiceConfigPath()
        {
            string rootPath = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                rootPath = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            rootPath = String.Format("{0}\\{1}\\{2}", rootPath ?? ".", Properties.Settings.Default.ServerWebHMIRootFolder, processConfigFileName);

            return rootPath;
        }

        string GetWebServiceImagePath()
        {
            string rootPath = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                rootPath = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            rootPath = String.Format("{0}\\{1}\\{2}\\", rootPath ?? ".", Properties.Settings.Default.ServerWebHMIRootFolder, Properties.Settings.Default.ServerWebHMIImagesFolder);

            return rootPath;
        }

        string GetWebServiceDocumentsPath()
        {
            string rootPath = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
                rootPath = System.IO.Path.GetDirectoryName(entryAssembly.Location);
            rootPath = String.Format("{0}\\{1}\\{2}\\", rootPath ?? ".", Properties.Settings.Default.ServerWebHMIRootFolder, Properties.Settings.Default.ServerWebHMIDocumentsFolder);

            return rootPath;
        }
        
        String TryGetPathNameByServiceName(string serviceName)
        {
            string filePath = null;

            try
            {
                string qry = "SELECT PATHNAME FROM WIN32_SERVICE WHERE NAME = '" + serviceName + "'";
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(qry);
                foreach (System.Management.ManagementObject mngntObj in searcher.Get())
                {
                    filePath = (string)mngntObj["PATHNAME"];
                }
            }
            catch
            { }

            return filePath;
        }
        #endregion
    }
}
