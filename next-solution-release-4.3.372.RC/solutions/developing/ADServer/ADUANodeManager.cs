using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerBase;
using Opc.Ua;
using Opc.Ua.Server;
using DevExpress.Xpo;
using ADPluginBase;
using System.Threading;
using ADPluginInterfaces;
using UFUAHistorian;
using DriverBaseInterfaces;
using Utilities;
using Utilities.Logger;

namespace ADServer
{
    public class ADUANodeManager : UANodeManager
    {
        public ADUANodeManager(UAServer uaserver, Opc.Ua.Server.IServerInternal s, Opc.Ua.ApplicationConfiguration c)
            : base(uaserver, s, c)
        {
            
        }
        
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                var thisserver = this.server as ADUAServer;
                foreach (var tag in mapNodeIdToNodeState)
                {
                    BaseInstanceState b = tag.Value as BaseInstanceState;
                    if (b != null && b.Parent != null)
                    {
                        string key = b.Parent.DisplayName.ToString();
                        if (thisserver.Plugins.ContainsKey(key))
                            tag.Value.Handle = thisserver.PluginThreads[key];
                    }

                }

                foreach (var p in thisserver.Plugins)
                {
                    p.Value.SystemEvent += ADUANodeManager_SystemEvent;
                }

                thisserver.OServer.SystemEvent += ADUANodeManager_SystemEvent;
            }
        }

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            base.DeleteAddressSpace();

            lock (Lock)
            {
                var thisserver = this.server as ADUAServer;
                foreach (var p in thisserver.Plugins)
                {
                    p.Value.SystemEvent -= ADUANodeManager_SystemEvent;
                }

                thisserver.OServer.SystemEvent -= ADUANodeManager_SystemEvent;
            }
        }

        protected void ADUANodeManager_SystemEvent(object sender, SystemEventArgs e)
        {
            e.logdestination = (int)LoggerDestination.AlarmDispatcher;
            UANodeManager_SystemEvent(sender, e);
        }

        public override ServiceResult OnMethodCall(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {
            ADUAServer adser = (ADUAServer)server.ServerInstance;
            if (adser != null && !adser.IsServerInActiveState())
                return StatusCodes.BadConditionDisabled;

            base.OnMethodCall(context, method, inputArguments, outputArguments);
            var sourceName = String.Format("node {0} user {1}", method, GetUserName(context));
            var eventName = String.Format("Method Calling Input parameters {0}", inputArguments);

            RaiseSystemEvents(null, sourceName, eventName, EventSeverity.High, DateTime.UtcNow);

            if (inputArguments.Count < 3)
                return StatusCodes.BadArgumentsMissing;
            try
            {
                var mail = (string)inputArguments[0];
                var phone = (string)inputArguments[1];
                var mobile = (string)inputArguments[2];
                var message = (string)inputArguments[3];
                var chatID = (string)inputArguments[4];

                var msg = new Message()
                {
                    Email = mail,
                    PhoneNumber = phone,
                    MobilePhoneNumber = mobile,
                    Textmessage = message,
                    TimeStamp = DateTime.UtcNow,
                    PluginID = method.NodeId,
                    ChatID = chatID,
                    NodeId = new NodeId(Guid.NewGuid())
                };

                var plugin = mapNodeIdToNodeState[msg.PluginID].Handle as PluginThread;

                if (plugin != null)
                    plugin.PostMessage(msg);
                else
                {
                    SystemEventArgs e = new SystemEventArgs();
                    e.sourceNode = ObjectIds.Server;

                    e.sourceName = Properties.Resources.LoggerSource;
                    e.EventName = string.Format(Properties.Resources.ADMethodFailed, message);
                    e.severity = EventSeverity.Medium;
                    e.time = DateTime.UtcNow;
                    e.eventtype = ObjectTypeIds.SystemStatusChangeEventType;
                    e.logtype = (int)System.Diagnostics.EventLogEntryType.Warning;
                    e.logdestination = (int)LoggerDestination.AlarmDispatcher;

                    ADUANodeManager_SystemEvent(this, e);

                }
            }
            catch (Exception e)
            {
                return StatusCodes.BadArgumentsMissing;
            }
            
            return ServiceResult.Good;
        }

    }
}
