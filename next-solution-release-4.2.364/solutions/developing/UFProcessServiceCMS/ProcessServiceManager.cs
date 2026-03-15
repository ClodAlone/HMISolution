using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UFProcessServiceCMS
{
    enum CommandType
    {
        Open,
        Install,
        Uninstall,
        Start,
        Stop
    }

    public class ProcessServiceManager<T> where T : IProcessServiceCMS
    {
        #region Declarations
        readonly ProcessServiceCMS<T> processService;
        readonly string applicationName;
        readonly string[] dependencies;

        string arguments;
        #endregion

        #region Constructors
        public ProcessServiceManager(ProcessServiceCMS<T> processService)
            : this(processService, null, null)
        { }

        public ProcessServiceManager(ProcessServiceCMS<T> processService, string applicationName) 
            : this(processService, applicationName, null)
        { }

        public ProcessServiceManager(ProcessServiceCMS<T> processService, string applicationName, string[] dependencies)
        {
            this.processService = processService;
            this.applicationName = applicationName;
            this.dependencies = dependencies;
        }
        #endregion

        #region Public Properties
        string serviceName;
        public string ServiceName
        {
            get
            {
                if (serviceName == null)
                {
                    if (!String.IsNullOrEmpty(applicationName))
                        serviceName = String.Format("{0} ({1})", processService.ServerName, applicationName);
                    else
                        serviceName = processService.ServerName;

                    serviceName = ServiceInstaller.ServiceInstaller.GetValidServiceName(serviceName);
                }

                return serviceName;
            }
        }

        string displayName;
        public string DisplayName
        {
            get
            {
                if (displayName == null)
                {
                    if (!String.IsNullOrEmpty(applicationName))
                        displayName = String.Format("{0} ({1})", processService.ServiceDisplayName, applicationName);
                    else
                        displayName = processService.ServiceDisplayName;
                }

                return displayName;
            }
        }

        public string UserName { get; set; }

        public string Password { get; set; }
        #endregion

        #region Public Methods
        public void Open(params string[] args)
        {
            PrepareArguments(CommandType.Open, args);
            Execute();
        }
        public void Install(params string[] args)
        {
            PrepareArguments(CommandType.Install, args);
            Execute();
        }

        public void Uninstall()
        {
            PrepareArguments(CommandType.Uninstall);
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

        public ServiceInstaller.ServiceState GetServiceStatus()
        {
            ServiceInstaller.ServiceState status = ServiceInstaller.ServiceState.Unknown;
            try
            {
                status = ServiceInstaller.ServiceInstaller.GetServiceStatus(ServiceName);
            }
            catch
            { }

            return status;
        }
        #endregion

        #region Methods
        void PrepareArguments(CommandType type, params string[] args)
        {
            string serverPath = processService.GetServerPath();
            string serviceArguments = String.Empty;
            if (args.Length > 0)
                serviceArguments = String.Format(processService.ServiceArguments, args);
            arguments = string.Format(Properties.Settings.Default.ServiceManagerProcessArgs/*"/K" "/N{0}" "/Y{1}" "/F{2}" "/D{3}"*/,
                ServiceName, DisplayName, serverPath, dependencies != null && dependencies.Length > 0 ? String.Join("|", dependencies) : String.Empty);

            if (type == CommandType.Install)
            {
                arguments = string.Format("/I {0}", arguments);
                if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                    arguments = string.Format("{0} /A{1} /W{2}", arguments, UserName, Password);
            }
            else if (type == CommandType.Open && !String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                arguments = string.Format("{0} /A{1} /W{2}", arguments, UserName, Password);
            else if (type == CommandType.Uninstall)
                arguments = string.Format("/U {0}", arguments);
            else if (type == CommandType.Start)
                arguments = string.Format("/S {0}", arguments);
            else if (type == CommandType.Stop)
                arguments = string.Format("/T {0}", arguments);
            var currentStyle = Utilities.ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin");
            if (!string.IsNullOrEmpty(currentStyle))
                arguments = string.Format("{0} /J{1}", arguments, currentStyle);

            string[] serviceArgs = serviceArguments.Split(new String[] { "\"-" }, StringSplitOptions.RemoveEmptyEntries);
            if (serviceArgs.Length > 0)
            {
                for (int i = 0; i < serviceArgs.Length; i++)
                {
                    string s = serviceArgs[i].Replace('\"', ' ').Trim();
                    if (s.Length > 0)
                        arguments += string.Format(Properties.Settings.Default.ServiceInstallProcessParams/*" "/P{0}-{1}"*/, i.ToString("00"), s);
                }
            }
        }

        void Execute()
        {
            string path = Properties.Settings.Default.ServiceInstallProcess/*"InstallServerService.exe"*/;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }
        #endregion
    }
}
