using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UFUAServerBase;
using DevExpress.Xpo;
using Opc.Ua.Server;
using Opc.Ua;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using ADPluginInterfaces;
using ADServerInfo;
using OPCUAViewModel;
using ViewModelLib;
using ADPluginBase;
using UFInterfaces;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using log4net;
using Utilities.Logger;
using ADModel;

namespace ADServer
{
    public class ADUAServer : UAServer, IEntityReference
    {
        #region Declarations
        Dictionary<String, List<NotificationBase>> mapServerNotifications;
        Dictionary<String, OPCUAEntityReference> mapServerEntityReference;
        Dictionary<String, Dictionary<String, NotificationBase>> mapAreasAlarms;
        Dictionary<String, OPCUAEntityReference> dictTagPhoneNumberEntityReference;
        
        #endregion

        private Dictionary<String, IPlugin> _Plugins = new Dictionary<String, IPlugin>();
        public Dictionary<String, IPlugin> Plugins
        {
            get { return _Plugins; }
        }

        private Dictionary<String, PluginThread> _PluginThreads = new Dictionary<String, PluginThread>();
        public Dictionary<String, PluginThread> PluginThreads
        {
            get { return _PluginThreads; }
        }

        private Dictionary<NodeId, object> _UserList = new Dictionary<NodeId, object>();
        public Dictionary<NodeId, object> UserList
        {
            get { return _UserList; }
        }

        UnitOfWork messageuow = null;
        IDataLayer messagedl = null;

        
        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        private String GetConnectionString(String activeconnection, String xmlExt)
        {
            if (String.IsNullOrEmpty(activeconnection) || String.IsNullOrEmpty(xmlExt))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = ds.Replace('/', '\\');
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                    ds = String.Format("{0}\\{1}.{2}", ds.Substring(0, index), ADServerInfo.ADServerInfo.GetServeName(), xmlExt);

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                int index = ds.LastIndexOf('\\');
                if (index != -1)
                    ds = String.Format("{0}\\{1}_{2}.mdb", ds.Substring(0, index), ADServerInfo.ADServerInfo.GetServeName(), xmlExt);

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(CatalogSourceHeader);
                if (!String.IsNullOrEmpty(ds))
                    ds = String.Format("{0}_{1}", ADServerInfo.ADServerInfo.GetServeName(), xmlExt);

                helper.UpdatePartByName(CatalogSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else
                return helper.GetConnectionString();
        }

        void MessageCreateDataLayer()
        {
            if (messagedl == null)
            {
                try
                {
                string conn = GetConnectionString(ActiveConnectionString, "msgper");
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dict.GetDataStoreSchema(typeof(MessagePersistence).Assembly);
                messagedl = XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema); 
                }
                catch(Exception ex)
                {
                    OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.OpenPersistenceError, ex.ToString()), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                }
            }
            if (messageuow == null && messagedl != null)
                messageuow = new UnitOfWork(messagedl);
            
        }
        object lockMessageList = new object();
        List<ADPluginBase.MessagePersistence> MessageList = new List<MessagePersistence>();
        void GetPersistentMessages()
        {
            MessageCreateDataLayer();
            try
            {
                lock (lockMessageList)
                {
                    MessageList = (from p in new XPQuery<ADPluginBase.MessagePersistence>(messageuow, true).AsParallel()
                                   orderby p.TimeStamp descending
                                   select p).ToList();
                }
            }
            catch(Exception ex)
            { 
                OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ReadPersistenceError, ex.ToString()), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                            null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
            }
        }
        public void SavePersistentMessage(Message m)
        {
            try
            {
                lock (lockMessageList)
                {
                    MessageList.Add(new MessagePersistence(messageuow)
                    {
                        Email = m.Email,
                        Recipient = m.Recipient,
                        MobilePhoneNumber = m.MobilePhoneNumber,
                        ChatID = m.ChatID,
                        FCMTokenPath = m.FCMTokenPath,
                        NodeId = m.NodeId,
                        PhoneNumber = m.PhoneNumber,
                        PluginID = m.PluginID,
                        PluginName = m.PluginName,
                        ADGroupMessage = m.ADGroupMessage,
                        Textmessage = m.Textmessage,
                        Alarmmessage = m.Alarmmessage,
                        TimeStamp = m.TimeStamp,
                        TagName = m.TagName,
                        Attachments = m.Attachments,
                        CustomMessage = m.CustomMessage,
                        GroupId = m.GroupId,
                        Name = m.Name,
                        Reason = m.Reason,
                        ServerAlarmStringId = m.ServerAlarmStringId,
                        CultureName = m.CultureName
                    });
                
                    messageuow.CommitChanges();

                }
            }
            catch (Exception e)
            {
                OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.SavePersistenceError, e.ToString()), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                Utils.Trace(e, "Save Persistence Error");
            }
        }
        public void DeletePersistentMessage(Message m)
        {
            try
            {
                lock (lockMessageList)
                {
                    int idx = MessageList.FindIndex(o => { return o.NodeId == m.NodeId;  });
                    if (idx != -1)
                        MessageList[idx].Delete();
                    messageuow.CommitChangesAndDropIdentityMap();
                    GetPersistentMessages();
                }
            }
            catch(Exception e)
            {
                OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.DeletePersistenceError, e.ToString()), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                Utils.Trace(e, "Delete Persistence Error");
            }
        }

        public bool BelongToThisServer(string endpointUrl)
        {
            foreach (var item in BaseAddresses)
            {
                if (endpointUrl == item.Url.ToString())
                    return true;
            }
            
            return false;
        }

        public void UpdatePersistentMessage(Message m)
        {
            try
            {
                lock (lockMessageList)
                {
                    var messageToUpdate = (from d in MessageList where d.NodeId == m.NodeId select d).ToList();
                    if (messageToUpdate.Count > 0)
                    {
                        foreach (var t in messageToUpdate)
                        {
                            MessageList[MessageList.IndexOf(t)].Email = m.Email;
                            MessageList[MessageList.IndexOf(t)].MobilePhoneNumber = m.MobilePhoneNumber;
                            MessageList[MessageList.IndexOf(t)].NodeId = m.NodeId;
                            MessageList[MessageList.IndexOf(t)].Recipient = m.Recipient;
                            MessageList[MessageList.IndexOf(t)].PhoneNumber = m.PhoneNumber;
                            MessageList[MessageList.IndexOf(t)].ChatID = m.ChatID;
                            MessageList[MessageList.IndexOf(t)].FCMTokenPath = m.FCMTokenPath;
                            MessageList[MessageList.IndexOf(t)].PluginID = m.PluginID;
                            MessageList[MessageList.IndexOf(t)].PluginName = m.PluginName;
                            MessageList[MessageList.IndexOf(t)].ADGroupMessage = m.ADGroupMessage;
                            MessageList[MessageList.IndexOf(t)].Textmessage = m.Textmessage;
                            MessageList[MessageList.IndexOf(t)].Alarmmessage = m.Alarmmessage;
                            MessageList[MessageList.IndexOf(t)].TimeStamp = m.TimeStamp;
                            MessageList[MessageList.IndexOf(t)].TagName = m.TagName;
                            MessageList[MessageList.IndexOf(t)].Reason = m.Reason;
                            MessageList[MessageList.IndexOf(t)].Name = m.Name;
                            MessageList[MessageList.IndexOf(t)].CustomMessage = m.CustomMessage;
                            MessageList[MessageList.IndexOf(t)].Attachments = m.Attachments;
                            MessageList[MessageList.IndexOf(t)].GroupId = m.GroupId;
                            MessageList[MessageList.IndexOf(t)].ServerAlarmStringId = m.ServerAlarmStringId;
                            MessageList[MessageList.IndexOf(t)].CultureName = m.CultureName;
                        }
                        messageuow.CommitChanges();
                    }
                }
            }
            catch (Exception e)
            {
                OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.UpdatePersistenceError, e.ToString()), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                Utils.Trace(e, "Delete Persistence Error");
            }
        }
        List<Message> listaUpdate = new List<Message>();
        List<Message> listaDelete = new List<Message>();
        public void addMessage(Message m, bool delete = false)
        {
            lock (lockMessageList)
            {
                if (delete)
                    listaDelete.Add(m);
                else
                    listaUpdate.Add(m);
            }

            ThreadPool.QueueUserWorkItem((o) => {
                lock (lockMessageList)
                {
                    if (listaDelete.Count > 0)
                    {
                        foreach (var d in listaDelete)
                        {
                            DeletePersistentMessage(d);
                        }
                        listaDelete.Clear();
                    }
                    else if (listaUpdate.Count > 0)
                    {
                        foreach (var d in listaUpdate)
                        {
                            UpdatePersistentMessage(d);
                        }
                        listaUpdate.Clear();
                    }
                    
                }
            });
        }

        UsersAndRolesMapping GetUsersAndRolesMapping()
        {
            var usersAndRolesMapping = new UsersAndRolesMapping();
            var idl = CreateUserModelDataLayer();
            if (idl == null)
                return usersAndRolesMapping;
            
            using (var useruow = new UnitOfWork(idl))
            {
                var roles = (from p in new XPQuery<UFUserModel.UFRole>(useruow, true).AsParallel() select p).ToList();
                foreach (var role in roles)
                {
                    usersAndRolesMapping.NameRoles[role.Name] = role;
                    foreach (var user in role.UFUsers)
                    {
                        usersAndRolesMapping.NameUsers[user.Name] = user;
                        usersAndRolesMapping.NodeIdUsers.Add(user.NodeId, user);
                    }
                    usersAndRolesMapping.NodeIdRoles.Add(role.NodeId, role);
                        
                }
            }

            return usersAndRolesMapping;
        }

        object lockListToAck = new object();
        List<string> ListAlarmToAcknowledge;

        public void AcknowledgeAlarm(string alrId)
        {
            lock (lockListToAck)
            {
                if (ListAlarmToAcknowledge == null)
                    ListAlarmToAcknowledge = new List<string>();
                if (!ListAlarmToAcknowledge.Contains(alrId))
                    ListAlarmToAcknowledge.Add(alrId);
            }
        }

        private ADServer _OServer;
        public ADServer OServer
        {
            get { return _OServer; }
            set { _OServer = value; }
        }

        private bool _UseLocalDateTime;
        public bool UseLocalDateTime
        {
            get { return _UseLocalDateTime; }
            set { _UseLocalDateTime = value; }
        }

        private bool _NotifyEveryStateChange;
        public bool NotifyEveryStateChange
        {
            get { return _NotifyEveryStateChange; }
            set { _NotifyEveryStateChange = value; }
        }

        private bool _CustomMessage;
        public bool CustomMessage
        {
            get { return _CustomMessage; }
            set { _CustomMessage = value; }
        }

        private bool _AddDateTime;
        public bool AddDateTime
        {
            get { return _AddDateTime; }
            set { _AddDateTime = value; }
        }
        private bool _AddAlarmState;
        public bool AddAlarmState
        {
            get { return _AddAlarmState; }
            set { _AddAlarmState = value; }
        }
        private bool _AddNotificationText;
        public bool AddNotificationText
        {
            get { return _AddNotificationText; }
            set { _AddNotificationText = value; }
        }
        private bool _AddServerText;
        public bool AddServerText
        {
            get { return _AddServerText; }
            set { _AddServerText = value; }
        }
        private bool _AddNotificationName;
        public bool AddNotificationName
        {
            get { return _AddNotificationName; }
            set { _AddNotificationName = value; }
        }
        /// <summary>
        /// Loads the non-configurable properties for the application.
        /// </summary>
        /// <remarks>
        /// These properties are exposed by the server but cannot be changed by administrators.
        /// </remarks>

        protected override ServerProperties LoadServerProperties()
        {
            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.AlarmDispatcher,
                                            Properties.Resources.LoadingServerProperties,
                                            Properties.Resources.LoadedServerProperties))
            {
                ServerProperties properties = base.LoadServerProperties(); 

                using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                {
                    id = XpoHelpers.XpoHelper.GetProtectionCode(ufw);

                    var configurations = (from tag in new XPQuery<ADModel.ADGeneralSettings>(ufw).AsParallel() select tag).ToList();
                    if (configurations.Count == 0)
                        configurations.Add(new ADModel.ADGeneralSettings(ufw));

                    if (configurations.Count > 0)
                    {
                        var applicationName = String.IsNullOrEmpty(configurations[0].ApplicationName) ? Configuration.ApplicationName : configurations[0].ApplicationName;
                        configurations[0].EnsureDefaultSettings(applicationName);

                        ParentApplicationName = configurations[0].ParentApplicationName ?? String.Empty;
                        Configuration.ApplicationName = applicationName;

                        properties.ManufacturerName = configurations[0].ManufacturerName ?? String.Empty; //  "Progea srl";
                        properties.ProductName = configurations[0].ProductName ?? String.Empty; // "UF UA Server";
                        properties.ProductUri = configurations[0].ProductUri ?? String.Empty; // "http://progea.com/UFSolution/UAServer/v1.0";
                        properties.SoftwareVersion = configurations[0].SoftwareVersion ?? String.Empty; // Utils.GetAssemblySoftwareVersion();
                        properties.BuildNumber = configurations[0].BuildNumber ?? String.Empty; // Utils.GetAssemblyBuildNumber();
                        properties.BuildDate = configurations[0].BuildDate; // Utils.GetAssemblyTimestamp();

                        UseLocalDateTime = configurations[0].UseLocalDateTime;
                        CustomMessage = configurations[0].CustomMessage.Value;
                        AddDateTime = configurations[0].AddDateTime.Value;
                        AddAlarmState = configurations[0].AddAlarmState.Value;
                        AddNotificationText = configurations[0].AddNotificationText.Value;
                        AddServerText = configurations[0].AddServerText.Value;
                        AddNotificationName = configurations[0].AddNotificationName.Value;
                        if(CustomMessage && !AddDateTime &&
                            !AddAlarmState && !AddNotificationText &&
                            !AddServerText && !AddNotificationName)
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                    Properties.Resources.CustomMessageUndefined,
                                    System.Diagnostics.EventLogEntryType.Warning,
                                    LoggerDestination.AlarmDispatcher);

                        Parallel.ForEach(configurations[0].ADPlugins, plug =>
                        {
                            using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.AlarmDispatcher,
                                                            String.Format(Properties.Resources.LoadingPlugin, plug.Name, ADServerInfo.ADServerInfo.GetServerFolder(), plug.AssemblyName),
                                                            String.Format(Properties.Resources.LoadedPlugin, plug.Name, ADServerInfo.ADServerInfo.GetServerFolder(), plug.AssemblyName)))
                            {
                                var list = FindAndLoadDLL.LoadDLLs<IPlugin>(ADServerInfo.ADServerInfo.GetServerFolder(), plug.AssemblyName);
                                
                                Parallel.ForEach(list, plugin =>
                                {
                                    using (new StopWatcherLogger(Properties.Resources.LoggerSource, LoggerDestination.AlarmDispatcher,
                                                                    String.Format(Properties.Resources.InitializingPlugin, plug.Name),
                                                                    String.Format(Properties.Resources.InitializedPlugin, plug.Name)))
                                    {
                                        var ret = plugin.Init(ActiveConnectionString);
                                        if(!ret)
                                        {
                                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            string.Format(Properties.Resources.ADPluginNotInitiliazed,plug.Name), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.AlarmDispatcher);
                                        }
                                        else
                                        {
                                            lock (_Plugins)
                                            {
                                                _Plugins.Add(plug.Name, plugin);
                                                PluginThread p = new PluginThread() { pluginRef = plugin, ErrorThreshold = configurations[0].ErrorThreshold, 
                                                    ErrorDelay = configurations[0].ErrorDelay.Value };
                                                p.Startup(this);
                                                _PluginThreads.Add(plug.Name, p);

                                            }
                                        }
                                    }
                                });
                            }
                        });

                        /**/
                        int [] Delays = new int[10];
                        Delays[0] = configurations[0].PriorityDelay0;
                        Delays[1] = configurations[0].PriorityDelay1;
                        Delays[2] = configurations[0].PriorityDelay2;
                        Delays[3] = configurations[0].PriorityDelay3;
                        Delays[4] = configurations[0].PriorityDelay4;
                        Delays[5] = configurations[0].PriorityDelay5;
                        Delays[6] = configurations[0].PriorityDelay6;
                        Delays[7] = configurations[0].PriorityDelay7;
                        Delays[8] = configurations[0].PriorityDelay8;
                        Delays[9] = configurations[0].PriorityDelay9;
                        
                        var notifications = (from tag in new XPQuery<ADModel.ADNotification>(ufw).AsParallel() select tag).ToList();
                        if (notifications.Count > 0)
                        {
                            //load users info
                            var usersAndRolesMapping = GetUsersAndRolesMapping();
                            if (usersAndRolesMapping.IsEmpty)
                            {
                                Logger.WriteToEventLog(Properties.Resources.LoggerSource, 
                                    Properties.Resources.ADUsersNotLoaded,
                                    System.Diagnostics.EventLogEntryType.Warning,
                                    LoggerDestination.AlarmDispatcher);
                            }
                            else
                            {
                                foreach (var notify in notifications)
                                {
                                    if (notify.NotificationItem == null || notify.NotificationItem.Length == 0)
                                        continue;

                                    if (notify.RecipientID == null &&  string.IsNullOrEmpty(notify.MultiRecipientID) &&
                                        string.IsNullOrEmpty(notify.Recipient))
                                        continue;

                                    if (!PluginThreads.Keys.Contains(notify.PluginName))
                                    {

                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                            string.Format(Properties.Resources.ADPluginNotLoaded, notify.PluginName, notify.Name),
                                            System.Diagnostics.EventLogEntryType.Warning,
                                            LoggerDestination.AlarmDispatcher);
                                        continue;//log error
                                    }

                                    string lostRecipients = string.Empty;
                                    StringBuilder sb = new StringBuilder();
                                    StringBuilder sbID = new StringBuilder();
                                    StringBuilder sbName = new StringBuilder();

                                    if (!NodeId.IsNull(notify.RecipientID))
                                        notify.MultiRecipientID = notify.RecipientID.ToString();
                                    string multirecipient = notify.MultiRecipientID.TrimEnd(ADModel.ADNotification.delimiter);

                                    string[] recArray = null;
                                    if (!string.IsNullOrEmpty(multirecipient.Trim()))
                                        recArray = multirecipient.Split(ADModel.ADNotification.delimiter);
                                    string recnames = notify.Recipient.TrimEnd(ADModel.ADNotification.delimiter);
                                    var multiRecName = recnames.Split(ADModel.ADNotification.delimiter);
                                    bool bNodeId = (recArray != null && recArray.Length > 0);
                                    if (multiRecName.Length > 0 || bNodeId)
                                    {
                                        NotificationBase message = new NotificationBase(notify);
                                        message.ServerInstance = this;
                                        message.OPCItem = notify.NotificationItem.FromXml<OPCUAEntityReference>();

                                        sbName.Clear();
                                        sbID.Clear();
                                        
                                        if (bNodeId)
                                            FillRecipientsFromNodeId(ref message, recArray, multiRecName, usersAndRolesMapping, ref sbID);
                                        else
                                            FillRecipientsFromNames(ref message, multiRecName, usersAndRolesMapping, ref sbName);

                                        if (sbName.Length != 0 || sbID.Length != 0)
                                        {
                                            sb.Append(sbID.Length != 0 ? sbID.ToString() : sbName.ToString());
                                            sb.Append(", ");
                                        }

                                        int nDelay = (notify.Priority >= 0 && notify.Priority <= ADNotification.MAXPriority) ? notify.Priority : 
                                            ADNotification.MAXPriority;
                                        message.PriorityDelay = Delays[nDelay];
                                        message.CreateDataLayer(ActiveConnectionString);
                                        message.LoadNotificationStatus();
                                        currentNotifications.Add(message);
                                        currentEntityReference.Add(message.OPCItem);
                                    }
                                    lostRecipients = sb.ToString();
                                    if (!string.IsNullOrEmpty(lostRecipients))
                                    {
                                        //log error
                                        Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                        string.Format(Properties.Resources.ADRecipientNotFound, lostRecipients.Substring(0, lostRecipients.Length - 2), notify.Name),
                                        System.Diagnostics.EventLogEntryType.Warning,
                                        LoggerDestination.AlarmDispatcher);
                                    }
                                }
                            }
                        }

                        /* load persisted messages, if any */
                        GetPersistentMessages();
                        foreach (var plugin in PluginThreads)
                        { 
                            var sublist = (from mper in MessageList where mper.PluginName == plugin.Key 
                                           orderby mper.TimeStamp select mper).ToList();
                            foreach(var m in sublist)
                            {
                                plugin.Value.PostMessage(new Message() {  Email = m.Email, MobilePhoneNumber = m.MobilePhoneNumber, 
                                    NodeId = m.NodeId, Recipient = m.Recipient, PhoneNumber = m.PhoneNumber, ChatID = m.ChatID, FCMTokenPath = m.FCMTokenPath, PluginID = m.PluginID, PluginName = m.PluginName, ADGroupMessage = m.ADGroupMessage,
                                    Textmessage = m.Textmessage, Alarmmessage = m.Alarmmessage, TimeStamp = m.TimeStamp,
                                    TagName = m.TagName, Attachments = m.Attachments, Reason = m.Reason, Name = m.Name,
                                    GroupId = m.GroupId, CustomMessage = m.CustomMessage, CultureName = m.CultureName,
                                    ServerAlarmStringId = m.ServerAlarmStringId});
                            }
                            plugin.Value.PostMessageGroupComplete();
                        }

                        NotifyEveryStateChange = configurations[0].NotifyEveryStateChange;
                    }
                }

                // TBD - All applications have software certificates that need to added to the properties.

                return properties;
            }
        }
        
        void FillRecipientsFromNames(ref NotificationBase message, string[] names, UsersAndRolesMapping urMap, ref StringBuilder sb)
        {
            foreach (var r in names)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    if (r.StartsWith("\\"))
                    {
                        //user
                        string uName = r.Substring(1);
                        if (urMap.NameUsers.ContainsKey(uName))
                        {
                            if (names.Length == 1)
                                message.SingleRecipient = urMap.NameUsers[uName];
                            else
                                message.MultiRecipients.Add(urMap.NameUsers[uName]);
                        }
                        else
                        {
                            sb.Append(uName);
                            sb.Append(", ");
                        }
                    }
                    else
                    {
                        //role
                        if(urMap.NameRoles.ContainsKey(r))
                        {
                            if (names.Length == 1)
                                message.ListOfRecpient = urMap.NameRoles[r];
                            else
                                message.MultiRecipients.AddRange(urMap.NameRoles[r].UFUsers);
                        }
                        else
                        {
                            sb.Append(r);
                            sb.Append(", ");
                        }
                    }
                }
            }
        }
        
        void FillRecipientsFromNodeId(ref NotificationBase message, string[] recId, string[] names, UsersAndRolesMapping urMap, ref StringBuilder sb)
        {
            int i = 0;
            foreach (var r in recId)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    if (urMap.NodeIdRoles.ContainsKey(r))
                    {
                        //add all the role users
                        if (recId.Length == 1)
                            message.ListOfRecpient = urMap.NodeIdRoles[r];
                        else
                            message.MultiRecipients.AddRange(urMap.NodeIdRoles[r].UFUsers);
                    }
                    else if (urMap.NodeIdUsers.ContainsKey(r))
                    {
                        //add the user
                        if (recId.Length == 1)
                            message.SingleRecipient = urMap.NodeIdUsers[r];
                        else
                            message.MultiRecipients.Add(urMap.NodeIdUsers[r]);
                    }
                    else if (names != null && names.Length > i)
                    {
                        //could be a name
                        bool bUser = urMap.NameUsers.ContainsKey(names[i]);
                        bool bRole = urMap.NameRoles.ContainsKey(names[i]);
                        if (bUser && !bRole)
                        {
                            if (recId.Length == 1)
                                message.SingleRecipient = urMap.NameUsers[names[i]];
                            else
                                message.MultiRecipients.Add(urMap.NameUsers[names[i]]);
                        }
                        else if(bRole && !bUser)
                        {
                            if (recId.Length == 1)
                                message.ListOfRecpient = urMap.NameRoles[names[i]];
                            else
                                message.MultiRecipients.AddRange(urMap.NameRoles[names[i]].UFUsers);
                        }
                        else
                        { 
                            //otherwise, error.
                            sb.Append(names[i]);
                            sb.Append(", ");
                        }
                    }
                    else
                    {
                        sb.Append(recId);
                        sb.Append(", ");
                    }
                }
                i++;
            }
        }




        private const string notificationsessionname = "AlarmDispatcher";
        private List<NotificationBase> currentNotifications = new List<NotificationBase>();
        private List<OPCUAEntityReference> currentEntityReference = new List<OPCUAEntityReference>();
        
        protected override MasterNodeManager CreateMasterNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        {
            using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
            {
                var configurations = (from tag in new XPQuery<ADModel.ADGeneralSettings>(ufw).AsParallel() select tag).ToList();

                if (configurations.Count > 0)
                    UFUAConfiguration = configurations[0];
                else
                    UFUAConfiguration = new ADModel.ADGeneralSettings(ufw);
            }

            List<INodeManager> nodeManagers = new List<INodeManager>();

            // create the custom node managers.
            uaNodeManager = new ADUANodeManager(this, server, configuration);
            nodeManagers.Add(uaNodeManager);

            // create master node manager.
            return new MasterNodeManager(server, configuration, null, nodeManagers.ToArray());
        }

#if !DEBUG
        bool bLog = false;
        protected override bool EndVirtualDisk()
        {
            var mode = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxkOOLPaEzFVSCMAw5TuHTDA=="/* DIS */);
            if (!mode && !bLog)
            {
                bLog = true;
                OServer.OnSystemEvent(ObjectIds.Server, Properties.Resources.NoSvrLicence, EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                        null, (int)System.Diagnostics.EventLogEntryType.Warning, (int)LoggerDestination.License);
                
                OnBalloonEvent(Properties.Resources.NoSvrLicence, System.Windows.Forms.ToolTipIcon.Warning);
            }
            else if(mode)
                bLog = false;

            return mode;
        }
#endif
        private const string NodeIdAndServerFormat = "{0}-{1}";
        readonly Dictionary<string, List<NotificationBase>> mapNodeIdToNotifications = new Dictionary<string, List<NotificationBase>>();
        public void AddToMapNodeIdToNotifications(NotificationBase not)
        {
            try
            {
                lock (mapNodeIdToNotifications)
                {
                    string key = string.Format(NodeIdAndServerFormat, not.OPCItem.EndpointUrl, not.OPCItem.ResolvedNodeId.ToString());
                    if (!mapNodeIdToNotifications.ContainsKey(key))
                        mapNodeIdToNotifications[key] = new List<NotificationBase>();
                    mapNodeIdToNotifications[key].Add(not);
                }
            }
            catch (Exception ex)
            { }
        }
        public void UpdateNotifications(OPCUAEntityReference tag, DataValue value)
        {
            lock (mapNodeIdToNotifications)
            {
                string key = string.Format(NodeIdAndServerFormat, tag.EndpointUrl, tag.ResolvedNodeId.ToString());
                if (mapNodeIdToNotifications.ContainsKey(key))
                {
                    var tagNotificationList = mapNodeIdToNotifications[key];
                    foreach (var notification in tagNotificationList)
                    {
                        notification.UpdateNotificationStatus(value);
                    }
                }
            }
        }

        public void UpdateData(Message m)
        {
            var uid = new NodeId(m.UserId);
            var usersAndRolesMapping = GetUsersAndRolesMapping();
            if (!usersAndRolesMapping.IsEmpty && usersAndRolesMapping.NodeIdUsers.ContainsKey(uid))
            {
                var user = usersAndRolesMapping.NodeIdUsers[uid];
                if (!string.IsNullOrEmpty(user.TagPhoneNumber))
                {
                    //maybe tag
                    var realtimePhoneNumber = GetRealPhoneNumber(user.Name);
                    //if realtimePhoneNumber is null, ServerInstance.GetRealPhoneNumber already logged an error, use user.PhoneNumber if not empty...
                    if (!string.IsNullOrEmpty(realtimePhoneNumber))
                        m.PhoneNumber = realtimePhoneNumber;
                }
            }
        }

        protected override void OnServerStarted(IServerInternal server)
        {
            base.OnServerStarted(server);

            if(OServer != null)
                OServer.ServerCMSHelperSync.StartServerStatusInBackground();

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            ViewModelBase.bDoNotCheckDispatcherObjects = true;

            List<UFUserModel.UFUser> completeUserList = new List<UFUserModel.UFUser>();
            foreach (var o in currentNotifications)
            {
                if(o.NotificationType== ADModel.NotificationTypes.Local)
                {
                    o.PrepareExecution(notificationsessionname);
                }
                else
                {
                    if(string.IsNullOrEmpty(o.AlarmName))
                    {
                        //log error message
                        OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.NotificationErrorAlarmName, o.name), EventSeverity.Medium, ObjectTypeIds.SystemStatusChangeEventType, null,
                       null, (int)System.Diagnostics.EventLogEntryType.Warning, (int)LoggerDestination.License);
                        continue;
                    }
                    var key = o.OPCItem.EndpointUrl;

                    if (mapServerNotifications == null)
                        mapServerNotifications = new Dictionary<string, List<NotificationBase>>();
                    if (!mapServerNotifications.ContainsKey(key))
                        mapServerNotifications[key] = new List<NotificationBase>();

                    mapServerNotifications[key].Add(o);

                    if (mapServerEntityReference == null)
                        mapServerEntityReference = new Dictionary<string, OPCUAEntityReference>();
                    if(!mapServerEntityReference.ContainsKey(key))
                    {
                        o.PrepareExecution(notificationsessionname);
                        mapServerEntityReference.Add(key, o.OPCItem);
                    }
                    
                    o.AlarmNodeId = new NodeId(o.AlarmGuid, uaNodeManager.NamespaceIndex);
                }
                //prepare connection with TagPhoneNumber, if proper
                var ul = o.GetUserWithTags();
                if (ul != null && ul.Count > 0)
                {
                    var singleList = (from u in ul where !completeUserList.Contains(u) select u).ToList();
                    if(singleList.Count > 0)
                        completeUserList.AddRange(singleList);
                }
                    
            }
            if(completeUserList.Count > 0)
            {
                if (dictTagPhoneNumberEntityReference == null)
                    dictTagPhoneNumberEntityReference = new Dictionary<string, OPCUAEntityReference>();

                foreach (var user in completeUserList)
                {
                    lock(lockThreadObject)
                    {
                        if (!string.IsNullOrEmpty(user.TagPhoneNumber) && !dictTagPhoneNumberEntityReference.ContainsKey(user.Name))
                        {
                            try
                            {
                                var item = user.TagPhoneNumber.FromXml<OPCUAEntityReference>();
                                if (item != null && item.IsValid)
                                {
                                    PropertyObserver<OPCUAEntityReference> observerPhone = new PropertyObserver<OPCUAEntityReference>(item);
                                    observerPhone.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                                    {
                                        observerPhone.UnregisterHandler(p => p.MonitoredItemViewModel);
                                        lock (lockThreadObject)
                                            dictTagPhoneNumberEntityReference[user.Name] = item;
                                        OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.PhoneTagAdded, user.Name),
                                            EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null, null, (int)System.Diagnostics.EventLogEntryType.Information,
                                            (int)LoggerDestination.AlarmDispatcher);
                                    });
                                    item.Resolve(notificationsessionname);
                                    item.SetInUse(this, true);
                                }
                            }
                            catch (Exception e)
                            {
                                OServer.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ErrorPreparingServiceTags, user.Name, e.Message), EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                                null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                            }
                        }
                    }
                }
            }
            

            if (mapServerEntityReference != null)
            {
                lock (lockThreadObject)
                {
                    if (WorkerThread == null)
                        WorkerThread = new Thread(WorkingThread);

                    if (StopWorkerThread == null)
                        StopWorkerThread = new ManualResetEvent(false);
                    else
                        StopWorkerThread.Reset();

                    if (!WorkerThread.IsAlive)
                        WorkerThread.Start(0);
                }
            }
        }

        public string GetRealPhoneNumber(string recipient)
        {
            lock(lockThreadObject)
            {
                if (dictTagPhoneNumberEntityReference.ContainsKey(recipient))
                {
                    var tagPhoneNumber = dictTagPhoneNumberEntityReference[recipient];
                    if (tagPhoneNumber != null && tagPhoneNumber.MonitoredItemViewModel != null && tagPhoneNumber.MonitoredItemViewModel.DataValue != null &&
                        StatusCode.IsGood(tagPhoneNumber.MonitoredItemViewModel.DataValue.StatusCode))
                    {
                        return Convert.ToString(tagPhoneNumber.MonitoredItemViewModel.DataValue.Value);
                    }
                    else
                    {
                        /*
                         * invalid tag, log error
                         */
                        var error = string.Format(Properties.Resources.TelephonNumberTagInvalid, recipient);
                        if (tagPhoneNumber == null)
                        {
                            //tag not yet connected or not existing
                            error = string.Format(Properties.Resources.TelephonNumberTagNotExist, recipient);
                        }
                        else if (tagPhoneNumber.MonitoredItemViewModel == null)
                        {
                            //tag not connected
                            error = string.Format(Properties.Resources.TelephonNumberTagNotConnected, recipient);
                        }
                        else if (!StatusCode.IsGood(tagPhoneNumber.MonitoredItemViewModel.DataValue.StatusCode))
                        {
                            //tag quality not good
                            error = string.Format(Properties.Resources.TelephonNumberTagNotGood, recipient);
                        }

                        OServer.OnSystemEvent(ObjectIds.Server, error, EventSeverity.High, ObjectTypeIds.SystemStatusChangeEventType, null,
                            null, (int)System.Diagnostics.EventLogEntryType.Error, (int)LoggerDestination.AlarmDispatcher);
                    }
                }
            }
            return null;
        }


        public bool IsServerInActiveState()
        {
            if (OServer != null && OServer.ServerCMSHelperSync != null)
                return OServer.ServerCMSHelperSync.IsServerActive;
            return true;
        }



        private const ushort ALR_STATE_ACTIVE = 1;
        private const ushort ALR_STATE_HIG_HIGH_ACTIVE = 2;
        private const ushort ALR_STATE_HIGH_ACTIVE = 4;
        private const ushort ALR_STATE_LOW_ACTIVE = 8;
        private const ushort ALR_STATE_LOW_LOW_ACTIVE = 16;
        private const ushort ALR_STATE_HIGH_HIGH = 32;
        private const ushort ALR_STATE_HIGH = 64;
        private const ushort ALR_STATE_LOW = 128;
        private const ushort ALR_STATE_LOW_LOW = 256;
        private const ushort ALR_STATE_ACK = 512;
        private const ushort ALR_STATE_CONFIRMED = 1024;

        private const uint MSG_SENT_ACTIVE_ON = 1;
        private const uint MSG_SENT_ACTIVE_OFF = (MSG_SENT_ACTIVE_ON << 1);
        private const uint MSG_SENT_HIGH_HIGH_ACTIVE_ON = (MSG_SENT_ACTIVE_ON << 2);
        private const uint MSG_SENT_HIGH_HIGH_ACTIVE_OFF = (MSG_SENT_ACTIVE_ON << 3);
        private const uint MSG_SENT_HIGH_ACTIVE_ON = (MSG_SENT_ACTIVE_ON << 4);
        private const uint MSG_SENT_HIGH_ACTIVE_OFF = (MSG_SENT_ACTIVE_ON << 5);
        private const uint MSG_SENT_LOW_ACTIVE_ON = (MSG_SENT_ACTIVE_ON << 6);
        private const uint MSG_SENT_LOW_ACTIVE_OFF = (MSG_SENT_ACTIVE_ON << 7);
        private const uint MSG_SENT_LOW_LOW_ACTIVE_ON = (MSG_SENT_ACTIVE_ON << 8);
        private const uint MSG_SENT_LOW_LOW_ACTIVE_OFF = (MSG_SENT_ACTIVE_ON << 9);
        private const uint MSG_SENT_HIGH_HIGH_ON = (MSG_SENT_ACTIVE_ON << 10);
        private const uint MSG_SENT_HIGH_HIGH_OFF = (MSG_SENT_ACTIVE_ON << 11);
        private const uint MSG_SENT_HIGH_ON = (MSG_SENT_ACTIVE_ON << 12);
        private const uint MSG_SENT_HIGH_OFF = (MSG_SENT_ACTIVE_ON << 13);
        private const uint MSG_SENT_LOW_ON = (MSG_SENT_ACTIVE_ON << 14);
        private const uint MSG_SENT_LOW_OFF = (MSG_SENT_ACTIVE_ON << 15);
        private const uint MSG_SENT_LOW_LOW_ON = (MSG_SENT_ACTIVE_ON << 16);
        private const uint MSG_SENT_LOW_LOW_OFF = (MSG_SENT_ACTIVE_ON << 17);
        private const uint MSG_SENT_ACK = (MSG_SENT_ACTIVE_ON << 18);
        private const uint MSG_SENT_CONFIRMED = (MSG_SENT_ACTIVE_ON << 19);
        

        bool GetMessageSentFlag(NotificationBase n, uint mask)
        {
            if (NotifyEveryStateChange)
                return false;

            return (n.flMessageSent & mask) == mask;
        }

        Thread WorkerThread;
        ManualResetEvent StopWorkerThread;
        readonly object lockThreadObject = new object();
        protected virtual void WorkingThread(object data)
        {
            int sleepCycle = 1000;
            List<string> alrToAck = null;
            while (true)
            {
                if (ListAlarmToAcknowledge != null && ListAlarmToAcknowledge.Count > 0)
                {
                    if (alrToAck == null)
                        alrToAck = new List<string>();
                    lock (lockListToAck)
                    {
                        alrToAck.AddRange(ListAlarmToAcknowledge);
                        ListAlarmToAcknowledge.Clear();
                    }
                }

                if (mapServerEntityReference != null)//this map contains the connection with server
                foreach(var s in mapServerEntityReference)
                {
                    if (mapServerNotifications.ContainsKey(s.Key) && s.Value.MonitoredItemViewModel != null && s.Value.MonitoredItemViewModel.ConditionStateList != null && s.Value.MonitoredItemViewModel.IsValid)
                    {
                        var nlist = mapServerNotifications[s.Key];//this is a list of NotificationBase for the server, each is an Area
                        if (nlist.Count == 0)
                            continue;

                        var _list = new List<ConditionStateViewModel>(s.Value.MonitoredItemViewModel.ConditionStateList.Count);
                        _list.AddRange(s.Value.MonitoredItemViewModel.ConditionStateList.ToList());
                        if (alrToAck != null && alrToAck.Count > 0)
                        {
                            var tList = (from c in _list where alrToAck.Contains(c.NodeIdString) select c).ToList();
                            if (tList.Count > 0)
                            {
                                foreach (var c in tList)
                                {
                                    c.CallAcknowledge(true);
                                    alrToAck.Remove(c.NodeIdString);
                                }
                            }
                        }

                        foreach (var n in nlist)
                        {
                                if (string.IsNullOrEmpty(n.AlarmName))
                                    continue;
                                //for each Area in this server, filter the alarm in ConditionStateList, belonging to the Area
                            int ln = n.AlarmName.Length;
                                
                                var list = (from al in _list
                                            where (al.Source.Length >= ln && al.IsActiveBranch() &&
                                            al.Source.StartsWith(n.AlarmName)/*Substring(0, ln) == n.AlarmName*/) ||
                                            al.Source == n.AlarmName
                                            select al).ToList();
                                
                                if (mapAreasAlarms == null)
                                    mapAreasAlarms = new Dictionary<string, Dictionary<string, NotificationBase>>();
                                if (!mapAreasAlarms.ContainsKey(n.nodeId.ToString()))
                                    mapAreasAlarms.Add(n.nodeId.ToString(), new Dictionary<string, NotificationBase>());
                                var thisAreaAlarmMap = mapAreasAlarms[n.nodeId.ToString()];
                                bool confirmed = false;
                                if (list.Count > 0)
                                {
                                    bool active = false;
                                    bool HighHighActive = false;
                                    bool HighActive = false;
                                    bool LowActive = false;
                                    bool LowLowActive = false;
                                    bool Low = false;
                                    bool LowLow = false;
                                    bool High = false;
                                    bool HighHigh = false;
                                    bool acknowledged = false;
                                    string message = string.Empty;

                                    foreach (var csv in list)
                                    {
                                        //message = $"{csv.Time} - {csv.EnabledState} - {csv.Message}";
                                        message = $"{csv.Message}";
                                        ushort state = 0;
                                        
                                        active = csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.Active);
                                        HighHighActive = csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.HighHighActive);
                                        HighActive = csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.HighActive);
                                        LowActive = csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.LowActive);
                                        LowLowActive = csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.LowLowActive);
                                        HighHigh = csv.EnabledState.Contains(Opc.Ua.BrowseNames.HighHigh);
                                        High = csv.EnabledState.Contains(Opc.Ua.BrowseNames.High);
                                        Low = csv.EnabledState.Contains(Opc.Ua.BrowseNames.Low);
                                        LowLow = csv.EnabledState.Contains(Opc.Ua.BrowseNames.LowLow);

                                        acknowledged = (csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.Unconfirmed));
                                        confirmed = (csv.EnabledState.Contains(Opc.Ua.ConditionStateNames.Confirmed));

                                        state = (active ? (ushort)(state | ALR_STATE_ACTIVE) : (ushort)(state & ~ALR_STATE_ACTIVE));
                                        state = (HighHighActive ? (ushort)(state | ALR_STATE_HIG_HIGH_ACTIVE) : (ushort)(state & ~ALR_STATE_HIG_HIGH_ACTIVE));
                                        state = (HighActive ? (ushort)(state | ALR_STATE_HIGH_ACTIVE) : (ushort)(state & ~ALR_STATE_HIGH_ACTIVE));
                                        state = (LowActive ? (ushort)(state | ALR_STATE_LOW_ACTIVE) : (ushort)(state & ~ALR_STATE_LOW_ACTIVE));
                                        state = (LowLowActive ? (ushort)(state | ALR_STATE_LOW_LOW_ACTIVE) : (ushort)(state & ~ALR_STATE_LOW_LOW_ACTIVE));
                                        state = (HighHigh ? (ushort)(state | ALR_STATE_HIGH_HIGH) : (ushort)(state & ~ALR_STATE_HIGH_HIGH));
                                        state = (High ? (ushort)(state | ALR_STATE_HIGH) : (ushort)(state & ~ALR_STATE_HIGH));
                                        state = (Low ? (ushort)(state | ALR_STATE_LOW) : (ushort)(state & ~ALR_STATE_LOW));
                                        state = (LowLow ? (ushort)(state | ALR_STATE_LOW_LOW) : (ushort)(state & ~ALR_STATE_LOW_LOW));


                                        state = (acknowledged ? (ushort)(state | ALR_STATE_ACK) : (ushort)(state & ~ALR_STATE_ACK));
                                        state = (confirmed ? (ushort)(state | ALR_STATE_CONFIRMED) : (ushort)(state & ~ALR_STATE_CONFIRMED));
                                        
                                        if(!thisAreaAlarmMap.ContainsKey(csv.NodeIdString))
                                        {
                                            var newNotification = new NotificationBase();

                                            newNotification.CopyValues(n);
                                            newNotification.persistenceName = String.Format("{0}.{1}", newNotification.PluginName, csv.NodeIdString);
                                            newNotification.ServerAlarmStringId = csv.NodeIdString;

                                            

                                            newNotification.CreateDataLayer(ActiveConnectionString);
                                            if(!newNotification.LoadNotificationStatus())
                                            {
                                                ushort initState = 0;
                                                initState = (!active ? (ushort)(initState | ALR_STATE_ACTIVE) : (ushort)(initState & ~ALR_STATE_ACTIVE));
                                                initState = (!HighHighActive ? (ushort)(initState | ALR_STATE_HIG_HIGH_ACTIVE) : (ushort)(initState & ~ALR_STATE_HIG_HIGH_ACTIVE));
                                                initState = (!HighActive ? (ushort)(initState | ALR_STATE_HIGH_ACTIVE) : (ushort)(initState & ~ALR_STATE_HIGH_ACTIVE));
                                                initState = (!LowActive ? (ushort)(initState | ALR_STATE_LOW_ACTIVE) : (ushort)(initState & ~ALR_STATE_LOW_ACTIVE));
                                                initState = (!LowLowActive ? (ushort)(initState | ALR_STATE_LOW_LOW_ACTIVE) : (ushort)(initState & ~ALR_STATE_LOW_LOW_ACTIVE));
                                                initState = (!HighHigh ? (ushort)(initState | ALR_STATE_HIGH_HIGH) : (ushort)(initState & ~ALR_STATE_HIGH_HIGH));
                                                initState = (!High ? (ushort)(initState | ALR_STATE_HIGH) : (ushort)(initState & ~ALR_STATE_HIGH));
                                                initState = (!Low ? (ushort)(initState | ALR_STATE_LOW) : (ushort)(initState & ~ALR_STATE_LOW));
                                                initState = (!LowLow ? (ushort)(initState | ALR_STATE_LOW_LOW) : (ushort)(initState & ~ALR_STATE_LOW_LOW));
                                                initState = (!acknowledged ? (ushort)(initState | ALR_STATE_ACK) : (ushort)(initState & ~ALR_STATE_ACK));
                                                initState = (!confirmed ? (ushort)(initState | ALR_STATE_CONFIRMED) : (ushort)(initState & ~ALR_STATE_CONFIRMED));
                                                newNotification.CurrentWState = initState;
                                            }
                                            
                                            newNotification.ServerMessage = csv.Message;
                                            thisAreaAlarmMap.Add(csv.NodeIdString, newNotification);
                                        }

                                        System.Diagnostics.Debug.WriteLine("La Clausola è:" + thisAreaAlarmMap.ContainsKey(csv.NodeIdString));
                                        if (thisAreaAlarmMap.ContainsKey(csv.NodeIdString))
                                        {
                                            var currentNotification = thisAreaAlarmMap[csv.NodeIdString];
                                            System.Diagnostics.Debug.WriteLine("currentNotification è " + currentNotification.name + " currentNotification.CurrentWState è " + currentNotification.CurrentWState + ", mentre state è " + state);
                                            bool bChangedTime = currentNotification.firstTime && ((csv.Time.HasValue && !currentNotification.ServerTime.HasValue) ||
                                                (csv.Time.HasValue && currentNotification.ServerTime.HasValue && csv.Time.Value != currentNotification.ServerTime.Value));
                                            currentNotification.ServerTime = csv.Time;
                                            currentNotification.firstTime = false;
                                            if (currentNotification.CurrentWState != state)
                                            {
                                                //something happened!
                                                bool oldActive = ((currentNotification.CurrentWState & ALR_STATE_ACTIVE) == ALR_STATE_ACTIVE);
                                                bool oldHighHighActive = ((currentNotification.CurrentWState & ALR_STATE_HIG_HIGH_ACTIVE) == ALR_STATE_HIG_HIGH_ACTIVE);
                                                bool oldHighActive = ((currentNotification.CurrentWState & ALR_STATE_HIGH_ACTIVE) == ALR_STATE_HIGH_ACTIVE);
                                                bool oldLowActive = ((currentNotification.CurrentWState & ALR_STATE_LOW_ACTIVE) == ALR_STATE_LOW_ACTIVE);
                                                bool oldLowLowActive = ((currentNotification.CurrentWState & ALR_STATE_LOW_LOW_ACTIVE) == ALR_STATE_LOW_LOW_ACTIVE);
                                                bool oldHighHigh = ((currentNotification.CurrentWState & ALR_STATE_HIGH_HIGH) == ALR_STATE_HIGH_HIGH);
                                                bool oldHigh = ((currentNotification.CurrentWState & ALR_STATE_HIGH) == ALR_STATE_HIGH);
                                                bool oldLow = ((currentNotification.CurrentWState & ALR_STATE_LOW) == ALR_STATE_LOW);
                                                bool oldLowLow = ((currentNotification.CurrentWState & ALR_STATE_LOW_LOW) == ALR_STATE_LOW_LOW);
                                                bool oldAcknowledged = ((currentNotification.CurrentWState & ALR_STATE_ACK) == ALR_STATE_ACK);
                                                bool oldConfirmed = ((currentNotification.CurrentWState & ALR_STATE_CONFIRMED) == ALR_STATE_CONFIRMED);

                                                if ((oldActive && !active && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_ACTIVE_OFF)) //event OFF
                                                    || 
                                                    (!oldActive && active && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_ACTIVE_ON)) //Event ON
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (active)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_ACTIVE_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_ACTIVE_OFF);
                                                }
                                                else if ((oldHighHighActive && !HighHighActive && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_HIGH_ACTIVE_OFF)) 
                                                    || 
                                                    (!oldHighHighActive && HighHighActive && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_HIGH_ACTIVE_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (HighHighActive)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_HIGH_ACTIVE_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_HIGH_ACTIVE_OFF);
                                                }
                                                else if ((oldHighActive && !HighActive && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_ACTIVE_OFF)) 
                                                    || 
                                                    (!oldHighActive && HighActive && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_ACTIVE_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (HighActive)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_ACTIVE_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_ACTIVE_OFF);
                                                }
                                                else if ((oldLowActive && !LowActive && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_ACTIVE_OFF)) 
                                                    || 
                                                    (!oldLowActive && LowActive && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_ACTIVE_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (LowActive)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_ACTIVE_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_ACTIVE_OFF);
                                                }
                                                else if ((oldLowLowActive && !LowLowActive && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_LOW_ACTIVE_OFF)) 
                                                    || 
                                                    (!oldLowLowActive && LowLowActive && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_LOW_ACTIVE_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (LowLowActive)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_LOW_ACTIVE_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_LOW_ACTIVE_OFF);
                                                }
                                                else if ((oldHighHigh && !HighHigh && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_HIGH_OFF)) 
                                                    || 
                                                    (!oldHighHigh && HighHigh && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_HIGH_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (HighHigh)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_HIGH_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_HIGH_OFF);
                                                }
                                                else if ((oldHigh && !High && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_OFF)) 
                                                    || 
                                                    (!oldHigh && High && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_HIGH_ON))
                                                    )
                                                {
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (High)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_HIGH_OFF);
                                                }
                                                else if ((oldLow && !Low && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_OFF)) 
                                                    || 
                                                    (!oldLow && Low && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (Low)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_OFF);
                                                }
                                                else if ((oldLowLow && !LowLow && n.EnableOFF && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_LOW_OFF)) 
                                                    || 
                                                    (!oldLowLow && LowLow && n.EnableON && !GetMessageSentFlag(currentNotification, MSG_SENT_LOW_LOW_ON))
                                                    )
                                                {
                                                    //transition active/inactive
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    if (LowLow)
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_LOW_ON);
                                                    else
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_LOW_LOW_OFF);
                                                }
                                                else if (!oldAcknowledged && acknowledged && n.EnableACK && !GetMessageSentFlag(currentNotification, MSG_SENT_ACK))
                                                {
                                                    //ack event
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                    currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_ACK);
                                                }
                                                else if (!oldConfirmed && confirmed && n.EnableCONFIRMED && !GetMessageSentFlag(currentNotification, MSG_SENT_CONFIRMED))
                                                {
                                                    //transition active/inactive
                                                    if ((confirmed && n.EnableCONFIRMED) || (!confirmed && n.EnableCONFIRMED))
                                                    {
                                                        currentNotification.ServerEnabledState = csv.EnabledState;
                                                        currentNotification.serverSendNotification(message);
                                                        currentNotification.flMessageSent = (uint)(currentNotification.flMessageSent | MSG_SENT_CONFIRMED);
                                                    }
                                                }
                                                currentNotification.CurrentWState = state;
                                                currentNotification.SaveNotificationStatus();
                                            }
                                            else if (bChangedTime)
                                            {
                                                if (
                                                        (confirmed && n.EnableCONFIRMED) || (!confirmed && n.EnableCONFIRMED) || (acknowledged && n.EnableACK) ||
                                                        ((LowLow || Low || High || HighHigh || LowLowActive || LowActive || HighActive || HighHighActive || active) && n.EnableON) ||
                                                        ((!LowLow || !Low || !High || !HighHigh || !LowLowActive || !LowActive || !HighActive || !HighHighActive || !active) && n.EnableOFF)
                                                        )
                                                {
                                                    currentNotification.ServerEnabledState = csv.EnabledState;
                                                    currentNotification.serverSendNotification(message);
                                                }
                                                currentNotification.CurrentWState = state;
                                                currentNotification.SaveNotificationStatus();
                                            }
                                        }
                                        
                                    }
                                    
                                }

                                var nodeIdStringList = (from c in list select c.NodeIdString).ToList();
                                var listAlarmToReset = (from t in thisAreaAlarmMap.Keys where !nodeIdStringList.Contains(t) select t).ToList();
                                if (listAlarmToReset.Count > 0)
                                {
                                    //this alarm have been reset!!
                                    foreach (var nodeid in listAlarmToReset)
                                    {
                                        var notificationReset = thisAreaAlarmMap[nodeid];
                                        //send notification reset
                                        //String message = String.Format("{0} {1} - {2} - {3}", 
                                        //    DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), Opc.Ua.ConditionStateNames.Confirmed, notificationReset.ServerMessage );
                                        if (notificationReset.EnableCONFIRMED)
                                        {
                                            String message = notificationReset.ServerMessage;
                                            notificationReset.ServerTime = DateTime.UtcNow;
                                            notificationReset.ServerEnabledState = Opc.Ua.ConditionStateNames.Confirmed;
                                            notificationReset.serverSendNotification(message);
                                        }
                                        notificationReset.CurrentWState = ALR_STATE_CONFIRMED;
                                        notificationReset.flMessageSent = 0;
                                        notificationReset.SaveNotificationStatus();
                                        //remove from map
                                        thisAreaAlarmMap[nodeid].Dispose();
                                        thisAreaAlarmMap.Remove(nodeid);
                                    }

                                }
                            }//end scroll of area
                    }
                }
                if (StopWorkerThread.WaitOne(sleepCycle, false))
                    break;
            }
        }
        protected override void OnServerStopping()
        {
            System.Diagnostics.Trace.TraceInformation("ADUAServer.OnServerStopping");
            foreach (var plugin in PluginThreads)
            {
                plugin.Value.StopThread();
                plugin.Value.Dispose();
            }

            Thread thread = WorkerThread;
            lock (lockThreadObject)
            {
                if (dictTagPhoneNumberEntityReference != null)
                {
                    foreach (var tag in dictTagPhoneNumberEntityReference.Keys)
                    {
                        if (dictTagPhoneNumberEntityReference[tag] != null)
                        {
                            dictTagPhoneNumberEntityReference[tag].SetInUse(this, false);
                            dictTagPhoneNumberEntityReference[tag] = null;
                        }

                    }
                    dictTagPhoneNumberEntityReference.Clear();
                }

                if (StopWorkerThread != null)
                    StopWorkerThread.Set();
            }
            if (thread != null)
                thread.Join();
            if(mapAreasAlarms != null)
            {
                foreach (var areaAlarms in mapAreasAlarms.Values)
                {
                    foreach (var al in areaAlarms.Values)
                    {
                        al.Dispose();
                    }
                    areaAlarms.Clear();
                }
                mapAreasAlarms.Clear();
            }
            

            base.OnServerStopping();
        }

        #region IEntityReference Members

        public System.Windows.Media.ImageSource CollapsedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Media.ImageSource ExpandedImageSource
        {
            get { throw new NotImplementedException(); }
        }

        public System.Windows.Controls.ContextMenu contextMenu
        {
            get { throw new NotImplementedException(); }
        }

        public object Tooltip
        {
            get { throw new NotImplementedException(); }
        }

        public object ContainedObject
        {
            get { throw new NotImplementedException(); }
        }

        public object EntityParent
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeDefinitionString
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (messageuow != null)
            {
                messageuow.Disconnect();
                messageuow.Dispose();
                messageuow = null;
            }

            if (messagedl != null)
            {
                messagedl.Dispose();
                messagedl = null;
            }

            foreach (var plugin in PluginThreads)
            {
                plugin.Value.StopThread();
                plugin.Value.Dispose();
            }

            foreach(var notifica in currentNotifications)
            {
                notifica.Dispose();
            }
            currentNotifications.Clear();
            currentEntityReference.Clear();

            if (ListAlarmToAcknowledge != null)
                ListAlarmToAcknowledge.Clear();

            if (mapServerNotifications != null)
                mapServerNotifications.Clear();
            if(mapServerEntityReference != null)
                mapServerEntityReference.Clear();
            if(mapAreasAlarms != null)
                mapAreasAlarms.Clear();
            
            lock (lockThreadObject)
            {
                if (dictTagPhoneNumberEntityReference != null)
                {
                    foreach (var tag in dictTagPhoneNumberEntityReference.Keys)
                    {
                        if (dictTagPhoneNumberEntityReference[tag] != null)
                        {
                            dictTagPhoneNumberEntityReference[tag].SetInUse(this, false);
                            dictTagPhoneNumberEntityReference[tag] = null;
                        }

                    }
                    dictTagPhoneNumberEntityReference.Clear();
                }

                if (StopWorkerThread != null)
                    StopWorkerThread.Dispose();
            }
        }
}
}
