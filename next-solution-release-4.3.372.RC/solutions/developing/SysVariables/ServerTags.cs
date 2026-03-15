using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFUAEditor.ComponentService;
using Utilities;

namespace SysVariables
{
    public static class ServerTags
    {
        #region Declarations
        static readonly Dictionary<String, String> serverTags;
        #endregion

        #region Constructors
        static ServerTags()
        {
            serverTags = new Dictionary<String, String>();

            // Alarms
            serverTags.Add("AlarmsSoundState", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsSoundStateName));
            serverTags.Add("AlarmsSoundBuzzing", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingName));
            serverTags.Add("AlarmsNumEnabled", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumEnabledName));
            serverTags.Add("AlarmsNumActiveOn", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumActiveOnName));
            serverTags.Add("AlarmsNumActiveOff", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumActiveOffName));
            serverTags.Add("AlarmsNumActiveOnOff", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumActiveOnOffName));
            serverTags.Add("AlarmsNumShelved", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumShelvedName));
            serverTags.Add("AlarmsNumNotAck", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName, UFUAServerInfo.BrowserNames.AlarmsNumNotAckName));

            //Messages
            serverTags.Add("MessagesNumEnabled", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.MessagesName, UFUAServerInfo.BrowserNames.MessagesNumEnabledName));
            serverTags.Add("MessagesNumActiveOn", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.MessagesName, UFUAServerInfo.BrowserNames.MessagesNumActiveOnName));
            serverTags.Add("MessagesNumShelved", String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.MessagesName, UFUAServerInfo.BrowserNames.MessagesNumShelvedName));

            //Redundancy
            serverTags.Add("RedundancyActiveServerState", UFUAServerInfo.BrowserNames.RedundancyActiveServerStateName);
            serverTags.Add("RedundancyActiveServerHostName", UFUAServerInfo.BrowserNames.RedundancyActiveServerHostNameName);
            serverTags.Add("RedundancyArrayAliveServerHostName", UFUAServerInfo.BrowserNames.RedundancyArrayAliveServerHostNameName);

            //Web sessions count
            serverTags.Add("ActiveSessionsWebHMI", UFUAServerInfo.BrowserNames.ActiveSessionsWebHMIName);
            serverTags.Add("ActiveSessionsWebClient", UFUAServerInfo.BrowserNames.ActiveSessionsWebClientName);
            serverTags.Add("ActiveSessionsWebApps", UFUAServerInfo.BrowserNames.ActiveSessionsWebAppsName);
        }
        #endregion

        #region Methods
        public static bool Contains(String name)
        {
            return serverTags.ContainsKey(name);
        }

        public static IReadOnlyList<String> GetNames()
        {
            return serverTags.Keys.ToList();
        }

        public static OPCUAEntityReference GetEntityReference(IDocument parent, String name)
        {
            if (!serverTags.ContainsKey(name) || parent == null)
                return null;

            var ufuaEditorManager = parent.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditorManager != null)
            {
                var opcString = ufuaEditorManager.GetNodeIdEntityReference(parent, serverTags[name], UFUAServerInfo.Guids.SystemTagsGuid.ToString());
                var ret = opcString.FromXml<OPCUAEntityReference>();
                ret.ReadablePath = null;
                ret.IsSystemVariable = true;
                ret.HumanReadable = string.Format("{0} ({1})", name, SysNames.dataSynkName/*ufuaEditorManager.GetDefApplicationName(parent)*/);
                return ret;
            }

            return null;
        }
        #endregion
    }
}
