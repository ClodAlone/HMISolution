using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.Service;

namespace UFProjectManager.Service
{
    internal class DiscoveredService : IServiceControl
    {
        enum CommandType
        {
            Uninstall,
            Start,
            Stop
        }

        #region Declarations
        readonly string serviceName;
        readonly string displayName;

        string arguments;
        #endregion

        #region Constructors
        public DiscoveredService(string serviceName, string displayName)
        {
            this.serviceName = serviceName;
            this.displayName = displayName;
        }
        #endregion

        #region IServiceControl
        public string FriendlyName
        {
            get
            {
                return displayName;
            }
        }

        public string Name
        {
            get
            {
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
        { }

        public void Install(string username, string password)
        { }

        public void OpenServiceControl()
        { }

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
            arguments = string.Format(@"""/N{0}"" ""/Y{1}""", serviceName, displayName);
            if (type == CommandType.Uninstall)
                arguments = string.Format("/U {0}", arguments);
            else if (type == CommandType.Start)
                arguments = string.Format("/S {0}", arguments);
            else if (type == CommandType.Stop)
                arguments = string.Format("/T {0}", arguments);
        }

        void Execute()
        {
            string path = Properties.Settings.Default.ServiceManagerExecutable;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }
        #endregion
    }
}
