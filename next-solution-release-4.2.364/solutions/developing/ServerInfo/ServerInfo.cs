using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Configuration;

namespace UFUAServerInfo
{
    #region Helper Class
    public class UFUAServerInfo
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

        public static string GetServerCoreExtension()
        {
            return Properties.Settings.Default.CoreExt;
        }

        public static string GetDefaultConnectionString()
        {
            return Properties.Settings.Default.XpoDefaultConnectionString;
        }

        public static string GetAuditTraceSuffix()
        {
            return Properties.Settings.Default.AuditSuffix;
        }

        public static Char GetDynSettingsSeparator()
        {
            return Properties.Settings.Default.DynSettingsSeparator;
        }

        public static string GetTagRootName()
        {
            return Properties.Settings.Default.TagsRootName;
        }

        public static string GetDefaultAliasRootName()
        {
            return Properties.Settings.Default.DefaultAliasRootName;
        }

        public static string GetAlarmRootName()
        {
            return Properties.Settings.Default.AlarmsRootName;
        }

        public static string GetDriverRootName()
        {
            return Properties.Settings.Default.DriversRootName;
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

        public static string GetCFR21UserNameSetting()
        {
            return Properties.Settings.Default.CFR21UserName;
        }

        public static string GetCFR21UserName()
        {
            var userName = Properties.Settings.Default.CFR21UserName;
            if (userName.IndexOf(".\\") != -1)
                return userName.Substring(userName.IndexOf(".\\") + 2);
            else if (userName.IndexOf("\\") != -1)
                return userName.Split('\\')[1];
            else if (userName.IndexOf('@') != -1)
                return userName.Split('@')[0];
            else
                return userName;
        }

        public static string GetCFR21DomainName()
        {
            var domainName = Properties.Settings.Default.CFR21UserName;
            if (domainName.IndexOf(".\\") != -1)
                return null;
            else if (domainName.IndexOf("\\") != -1)
                return domainName.Split('\\')[0];
            else if (domainName.IndexOf('@') != -1)
                return domainName.Split('@')[1];
            else
                return null;
        }
        #endregion

        #region Private Methods

        static string assemblyPath;
        static string GetAssemblyPath()
        {
            if (assemblyPath == null)
            {
                Assembly a = Assembly.GetExecutingAssembly();
                string s = a.Location.ToLower();
                string name = a.GetName().Name.ToLower() + ".dll";
                int idx = s.IndexOf(name);
                if (idx != -1)
                    assemblyPath = a.Location.Substring(0, idx);
                else
                    assemblyPath = string.Empty;
            }

            return assemblyPath;
        }

        #endregion
    }
    #endregion

    #region Guids Declarations
    public static partial class Guids
    {
        #region Roots
        /// <summary>
        /// The Guid for the root Tags component.
        /// </summary>
        public static Guid RootTagsGuid = new Guid("{ 0xe9722fed, 0x7881, 0x4d4a, { 0xab, 0x87, 0xb0, 0x7d, 0x65, 0x14, 0x62, 0xcd } }");

        /// <summary>
        /// The Guid for the root Drivers component.
        /// </summary>
        public static Guid RootDriversGuid = new Guid("{ 0x18d09e90, 0x21f0, 0x4537, { 0x95, 0x81, 0xba, 0xfd, 0x5f, 0xb6, 0xb2, 0x19 } }");

        /// <summary>
        /// The Guid for the root Alarms component.
        /// </summary>
        public static Guid RootAlarmsGuid = new Guid("{ 0xd5832892, 0x59a8, 0x4a0d, { 0x85, 0x2c, 0xc8, 0xad, 0xcf, 0xfd, 0xbf, 0xfe } }");

        /// <summary>
        /// The Guid for the root Diagnostic component.
        /// </summary>
        public static Guid RootDiagnosticGuid = new Guid("{ 0xb8ab8282, 0x1d23, 0x4467, { 0x95, 0x47, 0x91, 0xd4, 0x74, 0xf8, 0xe5, 0xd1 } }");

        /// <summary>
        /// The Guid for publishing the system tag
        /// </summary>
        public static Guid SystemTagsGuid = new Guid("{ 0x60d54358, 0x8bf6, 0x485e, { 0xaa, 0xa4, 0xc5, 0xc4, 0xb1, 0x5, 0xb7, 0xb7 } }");
        #endregion
    }
    #endregion

    #region BrowseName Declarations
    public static partial class BrowserNames
    {
        #region Auditing
        /// <summary>
        /// The BrowseName for the IsAuditTraceEnabled component.
        /// </summary>
        public const string IsAuditTraceEnabled = "IsAuditTraceEnabled";

        /// <summary>
        /// The BrowseName for the LastUserNameOnAudit component.
        /// </summary>
        public const string LastUserNameOnAudit = "LastUserNameOnAudit";

        /// <summary>
        /// The BrowseName for the LastCommentOnAudit component.
        /// </summary>
        public const string LastCommentOnAudit = "LastCommentOnAudit";

        /// <summary>
        /// The BrowseName for the WriteAuditValue component.
        /// </summary>
        public const string WriteAuditValue = "WriteAuditValue";

        /// <summary>
        /// The BrowseName for the ResetStatistics component.
        /// </summary>
        public const string ResetStatistics = "ResetStatistics";

        /// <summary>
        /// The BrowseName for the AddEventLog component.
        /// </summary>
        public const string AddEventLog = "AddEventLog";
        #endregion

        #region System Tags

        #region BaseObjectState
        /// <summary>
        /// The BrowserNane for the SystemTags component.
        /// </summary>
        public const string SystemTagsName = "SystemTags";

        /// <summary>
        /// The BrowserNane for the SystemTagName component.
        /// </summary>
        public const string SystemTagName = "System";

        /// <summary>
        /// The BrowserNane for the Alarms component.
        /// </summary>
        public const string AlarmsName = "Alarms";

        /// <summary>
        /// The BrowserNane for the Messages component.
        /// </summary>
        public const string MessagesName = "Messages";

        /// <summary>
        /// The BrowserNane for the Historian component.
        /// </summary>
        public const string HistorianName = "Historian";

        /// <summary>
        /// The BrowserNane for the Event component.
        /// </summary>
        public const string EventLoggerName = "EventLogger";

        /// <summary>
        /// The BrowserNane for the Redundancy component.
        /// </summary>
        public const string RedundancyName = "Redundancy";
        #endregion

        #region BaseVariableState
        /// <summary>
        /// The BrowseName for the LicenseSerialNumber component.
        /// </summary>
        public const string LicenseSerialNumberName = "LicenseSerialNumber";
        /// <summary>
        /// The BrowseName for the RecordingInError component.
        /// </summary>
        public const string RecordingInErrorName = "RecordingInError";

        /// <summary>
        /// The BrowseName for the DynamicTagCount component.
        /// </summary>
        public const string DynamicTagCountName = "DynamicTags";

        /// <summary>
        /// The BrowseName for the AlarmsSoundState component.
        /// </summary>
        public const string AlarmsSoundStateName = "SoundState";

        /// <summary>
        /// The BrowseName for the AlarmsSoundBuzzing component.
        /// </summary>
        public const string AlarmsSoundBuzzingName = "SoundBuzzing";

        /// <summary>
        /// The BrowseName for the AlarmsNumEnabledState component.
        /// </summary>
        public const string AlarmsNumEnabledName = "NumEnabled";

        /// <summary>
        /// The BrowseName for the AlarmsNumActiveOnState component.
        /// </summary>
        public const string AlarmsNumActiveOnName = "NumActiveOn";

        /// <summary>
        /// The BrowseName for the AlarmsNumActiveOffState component.
        /// </summary>
        public const string AlarmsNumActiveOffName = "NumActiveOff";

        /// <summary>
        /// The BrowseName for the AlarmsNumActiveOnOffState component.
        /// </summary>
        public const string AlarmsNumActiveOnOffName = "NumActiveOnOff";

        /// <summary>
        /// The BrowseName for the AlarmsNumShelvedState component.
        /// </summary>
        public const string AlarmsNumShelvedName = "NumShelved";

        /// <summary>
        /// The BrowseName for the AlarmsNumNotAckState component.
        /// </summary>
        public const string AlarmsNumNotAckName = "NumNotAck";

        /// <summary>
        /// The BrowseName for the MessagesNumEnabledState component.
        /// </summary>
        public const string MessagesNumEnabledName = "NumEnabled";

        /// <summary>
        /// The BrowseName for the MessagesNumActiveOnState component.
        /// </summary>
        public const string MessagesNumActiveOnName = "NumActiveOn";

        /// <summary>
        /// The BrowseName for the MessagesNumShelvedState component.
        /// </summary>
        public const string MessagesNumShelvedName = "NumShelved";

        /// <summary>
        /// The BrowseName for the RuntimeAlarmSettingsUpdate component.
        /// </summary>
        public const string RuntimeAlarmSettingsUpdateMethodName = "RuntimeAlarmSettingsUpdate";

        /// <summary>
        /// The BrowseName for the RecordEntriesPending component.
        /// </summary>
        public const string RecordEntriesPendingName = "RecordEntriesPending";

        /// <summary>
        /// The BrowseName for the RecordEntriesRunning component.
        /// </summary>
        public const string RecordEntriesRunningName = "RecordEntriesRunning";

        /// <summary>
        /// The BrowseName for the FailsEntriesPending component.
        /// </summary>
        public const string FailsEntriesPendingName = "FailsEntriesPending";

        /// <summary>
        /// The BrowseName for the FailsEntriesRunning component.
        /// </summary>
        public const string FailsEntriesRunningName = "FailsEntriesRunning";

        /// <summary>
        /// The BrowseName for the DeleteEntriesPending component.
        /// </summary>
        public const string DeleteEntriesPendingName = "DeleteEntriesPending";

        /// <summary>
        /// The BrowseName for the DeleteEntriesRunning component.
        /// </summary>
        public const string DeleteEntriesRunningName = "DeleteEntriesRunning";

        /// <summary>
        /// The BrowseName for the FlushEntriesPending component.
        /// </summary>
        public const string FlushEntriesPendingName = "FlushEntriesPending";

        /// <summary>
        /// The BrowseName for the FlushEntriesRunning component.
        /// </summary>
        public const string FlushEntriesRunningName = "FlushEntriesRunning";

        /// <summary>
        /// The BrowseName for the DischargingHistoricalEntries component.
        /// </summary>
        public const string DischargingEntriesName = "DischargingEntries";

        /// <summary>
        /// The BrowseName for the RedundancyActiveServerState component.
        /// </summary>
        public const string RedundancyActiveServerStateName = "RedundancyActiveServerState";

        /// <summary>
        /// The BrowseName for the RedundancyActiveServerHostName component.
        /// </summary>
        public const string RedundancyActiveServerHostNameName = "RedundancyActiveServerHostName";

        /// <summary>
        /// The BrowseName for the RedundancyArrayAliveServerHostName component.
        /// </summary>
        public const string RedundancyArrayAliveServerHostNameName = "RedundancyArrayAliveServerHostName";

        /// <summary>
        /// The BrowseName for the RedundancySwitchActiveServerMethod component.
        /// </summary>
        public const string RedundancySwitchActiveServerMethodName = "RedundancySwitchActiveServer";
        #endregion

        #region Descriptions
        /// <summary>
        /// The Description for the LicenseSerialNumberDesc component.
        /// </summary>
        public const string LicenseSerialNumberDesc = "License serial number";
        /// <summary>
        /// The Description for the SystemTags component.
        /// </summary>
        public const string SystemTagsDesc = "Collection of variables for handling the server";

        /// <summary>
        /// The Description for the RecordingInError component.
        /// </summary>
        public const string RecordingInErrorDesc = "Cumulative historical, events and data logger error state";

        /// <summary>
        /// The Description for the AlarmsSoundState component.
        /// </summary>
        public const string DynamicTagCountDesc = "Number of dynamic Tags connected to I/O devices";

        /// <summary>
        /// The Description for the AlarmsSoundState component.
        /// </summary>
        public const string AlarmsSoundStateDesc = "Allow to enable or disable the beeper sound in the server";

        /// <summary>
        /// The Description for the AlarmsSoundBuzzing component.
        /// </summary>
        public const string AlarmsSoundBuzzingDesc = "Beeper are playing in the server";

        /// <summary>
        /// The Description for the AlarmsNumEnabledState component.
        /// </summary>
        public const string AlarmsNumEnabledDesc = "Number of enabled alarms in the server";

        /// <summary>
        /// The Description for the AlarmsNumActiveOnState component.
        /// </summary>
        public const string AlarmsNumActiveOnDesc = "Number of active alarms in the server with 'On' state";

        /// <summary>
        /// The Description for the AlarmsNumActiveOffState component.
        /// </summary>
        public const string AlarmsNumActiveOffDesc = "Number of active alarms in the server with 'Off' state";

        /// <summary>
        /// The Description for the AlarmsNumActiveOnOffState component.
        /// </summary>
        public const string AlarmsNumActiveOnOffDesc = "Number of active alarms in the server with 'On' or 'Off' state";

        /// <summary>
        /// The Description for the AlarmsNumShelvedState component.
        /// </summary>
        public const string AlarmsNumShelvedDesc = "Number of shelved alarms in the server";

        /// <summary>
        /// The Description for the AlarmsNumNotAckState component.
        /// </summary>
        public const string AlarmsNumNotAckDesc = "Number of alarms in the server with 'Not Acknowledge' state";

        /// <summary>
        /// The Description for the AlarmsNumEnabledState component.
        /// </summary>
        public const string MessagesNumEnabledDesc = "Number of enabled messages in the server";

        /// <summary>
        /// The Description for the AlarmsNumActiveOnState component.
        /// </summary>
        public const string MessagesNumActiveOnDesc = "Number of active messages in the server with 'On' state";

        /// <summary>
        /// The Description for the MessagesNumShelvedState component.
        /// </summary>
        public const string MessagesNumShelvedDesc = "Number of shelved messages in the server";

        /// <summary>
        /// The Description for the RuntimeAlarmSettingsUpdate component.
        /// </summary>
        public const string RuntimeAlarmSettingsUpdateMethodDesc = "Force the update of the alarms settings, from the runtime settings file.";

        /// <summary>
        /// The Description for the RecordEntriesPending component.
        /// </summary>
        public const string RecordEntriesPendingDesc = "Number of entries are waiting in the cache for inserting in the database";

        /// <summary>
        /// The Description for the RecordEntriesRunning component.
        /// </summary>
        public const string RecordEntriesRunningDesc = "Number of entries that worked thread is processing for inserting in the database";

        /// <summary>
        /// The Description for the FailsEntriesPending component.
        /// </summary>
        public const string FailsEntriesPendingDesc = "Number of fails entries are waiting in the cache for retrying";

        /// <summary>
        /// The Description for the FailsEntriesRunning component.
        /// </summary>
        public const string FailsEntriesRunningDesc = "Number of fails entries that worked thread is processing for retrying";

        /// <summary>
        /// The Description for the DeleteEntriesPending component.
        /// </summary>
        public const string DeleteEntriesPendingDesc = "Number of delete entries are waiting in the cache for deleting from database";

        /// <summary>
        /// The Description for the DeleteEntriesRunning component.
        /// </summary>
        public const string DeleteEntriesRunningDesc = "Number of delte entries that worked thread is processing for deleting from database";

        /// <summary>
        /// The Description for the FlushEntriesPending component.
        /// </summary>
        public const string FlushEntriesPendingDesc = "Number of entries are waiting in the cache for flushing safely to xml file";

        /// <summary>
        /// The Description for the FlushEntriesRunning component.
        /// </summary>
        public const string FlushEntriesRunningDesc = "Number of entries that worked thread is processing for flushing safely to xml file";

        /// <summary>
        /// The Description for the DischargingEntries component.
        /// </summary>
        public const string DischargingEntriesDesc = "The cache is full and new entries will be lost";

        /// <summary>
        /// The Description for the RedundancyActiveServerState component.
        /// </summary>
        public const string RedundancyActiveServerStateDesc = "Indicates wheter or not is the active redundancy server";

        /// <summary>
        /// The Description for the RedundancyActiveServerHostName component.
        /// </summary>
        public const string RedundancyActiveServerHostNameDesc = "Host name of the active server";

        /// <summary>
        /// The Description for the RedundancyArrayAliveServerHostName component.
        /// </summary>
        public const string RedundancyArrayAliveServerHostNameDesc = "Array of the alive host name servers";

        /// <summary>
        /// The Description for the RedundancySwitchActiveServerMethod component.
        /// </summary>
        public const string RedundancySwitchActiveServerMethodDesc = "Force the switch from active server to next server";
        #endregion

        #endregion
    }
    #endregion
}
