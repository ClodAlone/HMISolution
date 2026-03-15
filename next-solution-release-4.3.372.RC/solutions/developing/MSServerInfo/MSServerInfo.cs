using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace MSServerInfo
{
    #region Helper Class
    public class MSServerInfo
    {
        #region Public Methods

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
        public static string GetServeName()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            return a.GetName().Name;
        }

        public static string GetTagsRootName()
        {
            return Properties.Settings.Default.TagsRootName;
        }

        public static String GetDriversFolder()
        {
            return String.Format("{0}Drivers", GetAssemblyPath());
        }

        public static String GetDriversUIName(String path)
        {
            return path.Replace(".dll", ".UI.dll");
        }

        public static String GetDriverListFile()
        {
            return String.Format("{0}Drivers\\Drivers.xml", GetAssemblyPath());
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

        public static string GetSchedulersListMethodGuid()
        {
            return Properties.Settings.Default.GetSchedulersListGuid;
        }

        public static string GetSetSchedulerSettingsMethodGuid()
        {
            return Properties.Settings.Default.SetSchedulerSettingsGuid;
        }

        public static string GetSchedulersListMethodName()
        {
            return Properties.Settings.Default.GetSchedulersListMethodName;
        }

        public static string GetSetSchedulerSettingsMethodName()
        {
            return Properties.Settings.Default.SetSchedulerSettingsMethodName;
        }

        public static string GetSchedulerUtilsFolder()
        {
            return Properties.Settings.Default.SchedulerUtilsFolder;
        }

        public static string GetReloadVariableName()
        {
            return Properties.Settings.Default.ReloadVariableName;
        }

        public static string GetReloadVariableGuid()
        {
            return Properties.Settings.Default.ReloadVariableGuid;
        }

        public static string GetExecuteOffName()
        {
            return Properties.Settings.Default.ExecuteOffName;
        }

        public static string GetEnableStateName()
        {
            return Properties.Settings.Default.EnabledStateName;
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
    #endregion

    #region Guids Declarations
    public static partial class Guids
    {
        #region Roots
        /// <summary>
        /// The Guid for the root Schedulers component.
        /// </summary>
        public static Guid RootScheduleGuid = new Guid("{ 0xbd048de6, 0x7fe9, 0x4a1a, { 0xad, 0xa1, 0x26, 0xf0, 0x8e, 0xac, 0x93, 0x1 } }");

        /// <summary>
        /// The Guid for the root Services component.
        /// </summary>
        public static Guid ServiceFolderGuid = new Guid("{ 0x5b722106, 0x9e8c, 0x4445, { 0x8e, 0x73, 0x7d, 0x83, 0x5f, 0x50, 0x11, 0xef } }");
        #endregion
    }
    #endregion
}
