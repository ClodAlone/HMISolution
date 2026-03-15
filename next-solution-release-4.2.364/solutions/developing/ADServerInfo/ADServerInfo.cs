using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace ADServerInfo
{
    public class ADServerInfo
    {
        #region Public Methods

        public static String GetPluginListFile()
        {
            return String.Format("{0}\\ADPlugins.xml", GetAssemblyPath());
        }

        public static String GetProcessName()
        {
            return Properties.Settings.Default.DefaultServerExe;
        }

        public static String GetSysTrayProcessName()
        {
            return Properties.Settings.Default.SysTrayApp;
        }

        public static String GetServerFolder()
        {
            return GetAssemblyPath();
        }

        public static String GetServerName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(Properties.Settings.Default.DefaultServerExe);
        }

        public static String GetServerPath()
        {
            return String.Format("{0}{1}", GetServerFolder(), Properties.Settings.Default.DefaultServerExe);
        }

        public static string GetServerConfigFile()
        {
            return String.Format("{0}{1}", GetServerFolder(),
                                                Properties.Settings.Default.DefaultServerConfig);
        }

        public static String GetPluginUIName(String path)
        {
            return path.Replace(".dll", ".UI.dll");
        }

        public static string GetServeName()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            return a.GetName().Name;
        }

        public static IList<string> GetCurrentApplicationBaseAddresses()
        {
            List<string> list = new List<string>();
            ServerConfiguration conf = GetCurrentServerConfiguration();
            if (conf.BaseAddresses.Count > 0)
            {
                list.AddRange(conf.BaseAddresses);
            }
            return list;
        }

        static ServerConfiguration configuration = null;
        public static ServerConfiguration GetCurrentServerConfiguration()
        {
            if (configuration == null)
            {
                var app = Utilities.ApplicationConfigurationHelper.LoadConfiguration(GetServerConfigFile());
                if (app != null)
                    configuration = app.ServerConfiguration;
                else
                    configuration = new ServerConfiguration();
            }

            return configuration;
        }

        #endregion

        #region Private Methods

        static string GetAssemblyPath()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            string s = a.Location.ToLower();
            string name = a.GetName().Name.ToLower() + ".dll";
            int idx = s.IndexOf(name);
            if (idx != -1)
                return a.Location.Substring(0, idx);
            return string.Empty;
        }       

        #endregion
    }
}
