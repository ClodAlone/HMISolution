using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFUAServerBase;
using Opc.Ua;
using Opc.Ua.Server;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.Threading;
using MSModel;
using OPCUAViewModel;
using System.IO;
using System.Text.RegularExpressions;
using Utilities;
using Utilities.Logger;
using System.Xml;
using DriverBaseInterfaces;
using System.Threading.Tasks;
using MSSchedulerSettings.Document;

namespace MSServer
{
    

    public class MSUANodeManager : UANodeManager
    {

        #region internal data
        //bool EnableLog = false;
        private object lockObj = new object();
        private List<ScheduledEvent> currentSchedules = new List<ScheduledEvent>();
        private const string schedulingsessionname = "SchedulerServer";

        private const string GetSchedulersListMethodDescription = "Returns the list of Runtime editable Schedulers";
        private const string SetSchedulerSettingsMethodDescription = "Send settings for a scheduler to the server";
        private const string ReloadVariableDescription = "Force reload of the scheduler configuration";

        #endregion

        #region Ctor
        public MSUANodeManager(UAServer uaserver, Opc.Ua.Server.IServerInternal s, Opc.Ua.ApplicationConfiguration c)
            : base(uaserver, s, c)
        {
            
        }
        #endregion

        public void MSUANodeManager_SystemEvent(object sender, SystemEventArgs e)
        {
            e.logdestination = (int)LoggerDestination.Scheduler;
            UANodeManager_SystemEvent(sender, e);
        }

        #region Methods
        public void UpdateScheduleAction(object sender, ChangedSchedArgs e)
        {
            e.result = false;
            SchedulingDate sd;
            NodeId n = new NodeId(e.nodeid, NamespaceIndex);
            lock(lockObj){ 
            var present = (from c in currentSchedules where c.NodeId == n select c).ToList();
            if (present != null && present.Count > 0)
            {
                //update
                present[0].Enable = e.schedEvent.Enable;
                present[0].Type = e.schedEvent.Type;
                present[0].Date = e.schedEvent.Date;
                present[0].Time = e.schedEvent.Time;
                present[0].DateOff = e.schedEvent.DateOff;
                present[0].TimeOff = e.schedEvent.TimeOff;
                present[0].ValueOn = e.schedEvent.ValueOn;
                present[0].ValueOff = e.schedEvent.ValueOff;
                present[0].ExceptionValueOn = e.schedEvent.ExceptionValueOn;
                present[0].ExceptionValueOff = e.schedEvent.ExceptionValueOff;
                    if (present[0].Type == ScheduleType.calendar)
                {
                    present[0].Calendar.Clear();
                    foreach (var ci in e.schedEvent.Calendar)
                    {
                        sd = new SchedulingDate();
                        sd.DayOfWeek = ci.DayOfWeek;
                        sd.EndDate = ci.EndDate;
                        sd.Month = ci.Month;
                        sd.StartDate = ci.StartDate;
                        sd.Type = ci.Type;
                        sd.WeekOfMonth = ci.WeekOfMonth;
                        sd.DayOfMonth = ci.DayOfMonth;
                        if (sd.Type == CalendarItemType.SingleDate)
                            sd.EndDate = sd.StartDate.AddMinutes(1);

                        present[0].Calendar.Add(sd);
                    }
                }
                else if (present[0].Type == ScheduleType.weeklyPlan)
                {
                    present[0].WeeklyCalendar.Clear();
                    foreach (var ci in e.schedEvent.WeeklyCalendar)
                    {
                        sd = new SchedulingDate();
                        sd.DayOfWeek = MSModel.Utils.GetCalendarDayType(ci.StartDate.DayOfWeek);
                        sd.EndDate = ci.EndDate;
                        sd.StartDate = ci.StartDate;
                        present[0].WeeklyCalendar.Add(sd);
                    }
                }
                present[0].ExceptionsCalendar.Clear();
                foreach (var ci in e.schedEvent.ExceptionsCalendar)
                {
                    sd = new SchedulingDate();
                    sd.DayOfWeek = ci.DayOfWeek;
                    sd.EndDate = ci.EndDate;
                    sd.Month = ci.Month;
                    sd.StartDate = ci.StartDate;
                    sd.Type = ci.Type;
                    sd.WeekOfMonth = ci.WeekOfMonth;
                    sd.DayOfMonth = ci.DayOfMonth;
                    if (sd.Type == CalendarItemType.SingleDate)
                        sd.EndDate = sd.StartDate.AddMinutes(1);

                    present[0].ExceptionsCalendar.Add(sd);
                }

                var s = this.server as MSUAServer;
                if (present[0].Enable && s.ScheduleEngine != null)
                {
                    s.ScheduleEngine.PostEvent(present[0]);
                    e.result = true;
                }
            }
        }
        }
        public object GetSchedulerValue(NodeId node)
        { 
            NodeState baseObject = null;

            if (mapNodeIdToNodeState.ContainsKey(node))
                baseObject = mapNodeIdToNodeState[node];
            if (baseObject != null)
            {
                if (baseObject is BaseVariableState)
                {
                    var variable = baseObject as BaseVariableState;
                    return variable.Value;
                }
            }
            return null;
        }
        public void UpdateSchedulerVariable(NodeId node, DataValue value)
        {
            NodeState baseObject = null;

            if (mapNodeIdToNodeState.ContainsKey(node))
                baseObject = mapNodeIdToNodeState[node];
            if (baseObject != null)
            {
                if (baseObject is BaseVariableState)
                {
                    var variable = baseObject as BaseVariableState;
                    variable.StatusCode = value.StatusCode;
                    variable.Timestamp = value.SourceTimestamp;
                    if (value.Value != null)
                        variable.Value = value.Value;

                    if (Server.IsRunning)
                    {
                        SystemEventState be = new SystemEventState(null);

                        be.Initialize(
                            SystemContext,
                            null,
                            EventSeverity.High,
                            new LocalizedText(variable.BrowseName.Name, string.Empty, variable.BrowseName.Name));

                        be.SetChildValue(SystemContext, BrowseNames.SourceNode, variable.NodeId, false);
                        be.SetChildValue(SystemContext, BrowseNames.SourceName, variable.NodeId, false);
                        be.Time.Value = DateTime.Now;


                        Server.ReportEvent(/*SystemContext, */be);

                        if (variable.AreEventsMonitored)
                            variable.ReportEvent(SystemContext, be);
                    }
                }
                baseObject.ClearChangeMasks(SystemContext, true);
            }
        }

        internal void UpdateSchedulerExecuteOffCommandState(NodeId node, DataValue value)
        {
            NodeState baseObject = null;

            if (mapNodeIdToNodeState.ContainsKey(node))
                baseObject = mapNodeIdToNodeState[node];
            if (baseObject != null)
            {
                var children = new List<BaseInstanceState>();
                baseObject.GetChildren(SystemContext, children);
                var enexecuteOff = (from c in children
                                   where c is BaseVariableState && c.BrowseName == MSServerInfo.MSServerInfo.GetExecuteOffName()
                                   select c as BaseVariableState).FirstOrDefault();
                if (enexecuteOff != null)
                {
                    enexecuteOff.StatusCode = value.StatusCode;
                    enexecuteOff.Timestamp = value.SourceTimestamp;
                    if (enexecuteOff.Value != null)
                        enexecuteOff.Value = value.Value;
                }
                baseObject.ClearChangeMasks(SystemContext, true);
            }
        }

        internal void UpdateSchedulerEnableState(NodeId node, DataValue value)
        {
            NodeState baseObject = null;

            if (mapNodeIdToNodeState.ContainsKey(node))
                baseObject = mapNodeIdToNodeState[node];
            if (baseObject != null)
            {
                var children = new List<BaseInstanceState>();
                baseObject.GetChildren(SystemContext, children);
                var enableState = (from c in children
                                   where c is BaseVariableState && c.BrowseName == MSServerInfo.MSServerInfo.GetEnableStateName()
                                   select c as BaseVariableState).FirstOrDefault();
                if (enableState != null)
                {
                    enableState.StatusCode = value.StatusCode;
                    enableState.Timestamp = value.SourceTimestamp;
                    if (enableState.Value != null)
                        enableState.Value = value.Value;
                }
                baseObject.ClearChangeMasks(SystemContext, true);
            }
        }
        #endregion


        #region Overrides
        private static readonly String DataSourceHeader = "data source";
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            try
            {
                var s = this.server as MSUAServer;
                s.OServer.SchedulerChanging += UpdateScheduleAction;
                s.OServer.SystemEvent += MSUANodeManager_SystemEvent;

                using (var ufw = new UnitOfWork(UFUAServer.DefaultDataLayer))
                {

                    //var gsett = (from tag in new XPQuery<MSGeneralSettings>(ufw).AsParallel()
                    //             select tag).ToList();
                    //if (gsett.Count > 0)
                    //{
                    //    EnableLog = gsett[0].EnableLog;
                    //}
                    //if (EnableLog && (server.UFUAConfiguration.EventDefaultConnection == null || server.UFUAConfiguration.EventDefaultConnection.Length == 0))
                    //{
                    //    s.UFUAConfiguration.EventDefaultConnection = MSUAServer.GetConnectionString(server.ActiveConnectionString, Properties.Settings.Default.DefaultLogFileExt);
                    //}

                    //Create the folders and the variables
                    var folders = (from folder in new XPQuery<MSFolder>(ufw).AsParallel()
                                   where folder.Name != AliasRoot && folder.MSFolderAss == null
                                   select folder).ToList();
                    var rootASTags = (from tag in new XPQuery<MSScheduledAction>(ufw).AsParallel()
                                      where tag.MSFolderAss == null
                                      select tag).ToList();

                    var rootTags = (from tag in new XPQuery<MSScheduledAction>(ufw).AsParallel()
                                    select tag).ToList();



                    CreateUtilsTagAndMethod();

                    if (folders.Count > 0 || rootASTags.Count > 0)
                    {
                        if (baseFolderTags == null)
                            baseFolderTags = CreateRootFolder(MSServerInfo.MSServerInfo.GetTagsRootName(), MSServerInfo.Guids.RootScheduleGuid);

                        foreach (var tag in rootASTags)
                        {
                            CreateTag(tag, baseFolderTags.NodeId, baseFolderTags);
                        }

                        AddPredefinedNode(SystemContext, baseFolderTags);
                        mapNodeIdToNodeState.Add(baseFolderTags.NodeId, baseFolderTags);

                        foreach (var folder in folders)
                        {
                            CreateFolder(folder, baseFolderTags.NodeId, baseFolderTags);
                        }
                        AddReverseReferences(externalReferences);
                    }

                    string currConn = s.OServer.OrigConn;

                    if (rootTags.Count > 0)
                    {
                        lock(lockObj)
                        { 
                        ScheduledEvent se;
                        SchedulingDate sd;
                        foreach (var sa in rootTags)
                        {
                            Logger.WriteToEventLog(Properties.Resources.LoggerSource,
                                Properties.Resources.CreateScheduler,
                                System.Diagnostics.EventLogEntryType.Information,
                                LoggerDestination.Scheduler, sa.Name);
                            var info = new SystemEvent()
                            {
                                evtype = ObjectTypeIds.SystemStatusChangeEventType
                            };
                            RaiseSystemEvents(null, Properties.Resources.LoggerSource, string.Format(Properties.Resources.CreateScheduler, sa.Name),
                                EventSeverity.Low, DateTime.UtcNow, info);
                            se = new ScheduledEvent()
                            {
                                Name = sa.Name,
                                Enable = sa.Enable.Value,
                                Type = sa.Type,
                                Date = sa.Date,
                                Time = sa.Time,
                                DateOff = sa.DateOff,
                                TimeOff = sa.TimeOff,
                                NodeId = new NodeId(sa.NodeId, NamespaceIndex),
                                nodeManager = this,
                                ValueOn = sa.ValueOn,
                                ValueOff = sa.ValueOff,
                                ExceptionValueOn = sa.ExceptionValueOn,
                                ExceptionValueOff = sa.ExceptionValueOff,
                                ExecOnAtStartup = (sa.ExecOnAtStartup.HasValue ? sa.ExecOnAtStartup.Value : MSScheduledAction.defaultExecOnAtStartup),
                                ExecOffAtStartup = (sa.ExecOffAtStartup.HasValue ? sa.ExecOffAtStartup.Value : MSScheduledAction.defaultExecOffAtStartup),
                                HasCommandOn = !string.IsNullOrEmpty(sa.CommandsOn),
                                HasCommandOff = !string.IsNullOrEmpty(sa.CommandsOff),
                                HasCommandOnEx = !string.IsNullOrEmpty(sa.ExceptionCommandsOn),
                                HasCommandOffEx = !string.IsNullOrEmpty(sa.ExceptionCommandsOff),
                            };
                            if (sa.ScheduleItem != null && sa.ScheduleItem.Length > 0)
                            {
                                try
                                {
                                    se.OPCItem = sa.ScheduleItem.FromXml<OPCUAEntityReference>();
                                }
                                catch (Exception e)
                                {
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Scheduler);
                                    RaiseSystemEvents(null, Properties.Resources.LoggerSource, e.ToString(),
                                        EventSeverity.Medium, DateTime.UtcNow, info);
                                    Opc.Ua.Utils.Trace(e, String.Format(Properties.Resources.ErrorScheduleItem, sa.Name));
                                }
                            }

                            if (sa.EnableVariable != null && sa.EnableVariable.Length > 0)
                            {
                                try
                                {
                                    se.OPCEnableVar = sa.EnableVariable.FromXml<OPCUAEntityReference>();
                                }
                                catch (Exception e)
                                {
                                    Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Warning, LoggerDestination.Scheduler);
                                    RaiseSystemEvents(null, Properties.Resources.LoggerSource, e.ToString(),
                                        EventSeverity.Medium, DateTime.UtcNow, info);
                                    Opc.Ua.Utils.Trace(e, String.Format(Properties.Resources.ErrorEnableVariable, sa.Name));
                                }
                            }

                            if (se.Type == ScheduleType.calendar)
                            {
                                foreach (var ci in sa.Calendar)
                                {
                                    sd = new SchedulingDate();
                                    sd.DayOfWeek = ci.DayOfWeek;
                                    sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                                    sd.Month = ci.Month;
                                    sd.StartDate = ci.StartDate;
                                    sd.Type = ci.Type;
                                    sd.WeekOfMonth = ci.WeekOfMonth;
                                    sd.DayOfMonth = ci.DayOfMonth;
#if DEBUG
                                    System.Diagnostics.Trace.TraceInformation(string.Format("start:{0} end:{1} type:{2} Elapsed:{3}",
                                        sd.StartDate, sd.EndDate,
                                        sd.Type == CalendarItemType.SingleDate ? "single" : (sd.Type == CalendarItemType.DateRange ? "range" : "WeekNDay"), sd.Elapsed));
#endif
                                    se.Calendar.Add(sd);
                                }
                            }
                            else if (se.Type == ScheduleType.weeklyPlan)
                            {
                                foreach (var ci in sa.WeeklyCalendar)
                                {
                                    sd = new SchedulingDate();
                                    sd.DayOfWeek = MSModel.Utils.GetCalendarDayType(ci.StartDate.DayOfWeek);
                                    sd.EndDate = ci.EndDate;
                                    sd.StartDate = ci.StartDate;
                                    se.WeeklyCalendar.Add(sd);
                                }
                            }

                            foreach (var ci in sa.ExceptionsCalendar)
                            {
                                sd = new SchedulingDate();
                                sd.DayOfWeek = ci.DayOfWeek;
                                sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                                sd.Month = ci.Month;
                                sd.StartDate = ci.StartDate;
                                sd.Type = ci.Type;
                                sd.WeekOfMonth = ci.WeekOfMonth;
                                sd.DayOfMonth = ci.DayOfMonth;
#if DEBUG
                                System.Diagnostics.Trace.TraceInformation(string.Format("start:{0} end:{1} type:{2} Elapsed:{3}",
                                sd.StartDate, sd.EndDate,
                                sd.Type == CalendarItemType.SingleDate ? "single" : (sd.Type == CalendarItemType.DateRange ? "range" : "WeekNDay"), sd.Elapsed));
#endif
                                se.ExceptionsCalendar.Add(sd);
                            }

                            se.CreateDataLayer(currConn);

                            currentSchedules.Add(se);


                            if (mapNodeIdToNodeState.ContainsKey(se.NodeId))
                            {
                                NodeState baseObject = mapNodeIdToNodeState[se.NodeId];
                                if (baseObject != null)
                                {
                                    if (baseObject is BaseVariableState)
                                    {
                                        var variable = baseObject as BaseVariableState;
                                        variable.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, true, variable.Parent);
                                        variable.Parent.AddNotifier(SystemContext, ReferenceTypeIds.HasEventSource, false, variable);
                                    }
                                }
                            }
                        }
                        }
                    }
                }
                
                //Update schedulers with runtime settings
                LoadRuntimeConfiguration();

                lock(lockObj)
                {
                    foreach (var se in currentSchedules)
                    {
                        se.LoadSchedulerStatus();

                        se.PrepareExecution(schedulingsessionname);
                        if (se.Enable && s.ScheduleEngine != null)
                            s.ScheduleEngine.PostEvent(se);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteToEventLog(Properties.Resources.LoggerSource, e.ToString(), System.Diagnostics.EventLogEntryType.Error, LoggerDestination.Scheduler);
                Opc.Ua.Utils.Trace(e, "Unexpected error in Creating the address space");
                server.UpdateServerState(ServerState.Failed);
            }
        }

        private bool SaveRuntimeConfiguration(SimpleScheduledEvent scheduler)
        {
            var s = this.server as MSUAServer;
            InMemoryDataStore datastore;
            try
            { 
                IDataLayer rtLayer = s.GetRuntimeDataLayer(out datastore);
                using (var ufw = new UnitOfWork(rtLayer))
                {
                    var rootTags = (from tag in new XPQuery<MSScheduledActionRuntime>(ufw).AsParallel()
                                    select tag).ToList();

                    MSScheduledActionRuntime sAct = null;
                    if (rootTags.Count > 0)
                    {
                        sAct = rootTags.Find((a) => {
                            return a.NodeId == scheduler.Guid;
                            });
                    }

                    if (sAct == null)
                        sAct = new MSScheduledActionRuntime(ufw)
                        {
                            NodeId = scheduler.Guid
                        };
                    UpdateScheduledActionRuntime(sAct, scheduler);
                    //ufw.PurgeDeletedObjects();
                    ufw.CommitChanges();
                    

                    ufw.Disconnect();

                }

                if(datastore != null)
                    s.SaveToFile(datastore);
            
                rtLayer.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void LoadRuntimeConfiguration(bool running = false)
        {
            var s = this.server as MSUAServer;
            InMemoryDataStore ds;
            IDataLayer rtLayer = s.GetRuntimeDataLayer(out ds);
            using (var ufw = new UnitOfWork(rtLayer))
            {
                var rootTags = (from tag in new XPQuery<MSScheduledActionRuntime>(ufw).AsParallel()
                                select tag).ToList();

                lock(lockObj)
                {
                    if (rootTags.Count > 0)
                    {
                        bool modified = false;

                        SchedulingDate sd;
                        ScheduledEvent se;
                        List<ScheduledEvent> reconnectList = new List<ScheduledEvent>();
                        foreach (var sa in rootTags)
                        {
                            //bool reconnect = false;
                            NodeId idcmp = new NodeId(sa.NodeId, NamespaceIndex);
                            se = currentSchedules.Find((o) => { return o.NodeId == idcmp; });
                            if (se == null)
                            {
                                //not present, create (for future implementation)
                                continue;
                            }
                            modified = (se.Date != sa.Date || se.Time != sa.Time || se.DateOff != sa.DateOff || se.TimeOff != sa.TimeOff);
                            se.Date = sa.Date;
                            se.Time = sa.Time;
                            se.DateOff = sa.DateOff;
                            se.TimeOff = sa.TimeOff;
                            if (sa.ExecOnAtStartup.HasValue)
                                se.ExecOnAtStartup = sa.ExecOnAtStartup.Value;
                            if (sa.ExecOffAtStartup.HasValue)
                                se.ExecOffAtStartup = sa.ExecOffAtStartup.Value;

                            if (se.Type == ScheduleType.calendar)
                            {
                                modified = true;
                                se.Calendar.Clear();
                                foreach (var ci in sa.Calendar)
                                {
                                    sd = new SchedulingDate();
                                    sd.DayOfWeek = ci.DayOfWeek;
                                    sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                                    sd.Month = ci.Month;
                                    sd.StartDate = ci.StartDate;
                                    sd.Type = ci.Type;
                                    sd.WeekOfMonth = ci.WeekOfMonth;
                                    sd.DayOfMonth = ci.DayOfMonth;
                                    se.Calendar.Add(sd);
                                }
                            }
                            else if (se.Type == ScheduleType.weeklyPlan)
                            {
                                modified = true;
                                se.WeeklyCalendar.Clear();
                                foreach (var ci in sa.WeeklyCalendar)
                                {
                                    sd = new SchedulingDate();
                                    sd.DayOfWeek = MSModel.Utils.GetCalendarDayType(ci.StartDate.DayOfWeek);
                                    sd.EndDate = ci.EndDate;
                                    sd.StartDate = ci.StartDate;
                                    se.WeeklyCalendar.Add(sd);
                                }
                            }
                            se.ExceptionsCalendar.Clear();
                            foreach (var ci in sa.ExceptionsCalendar)
                            {
                                sd = new SchedulingDate();
                                sd.DayOfWeek = ci.DayOfWeek;
                                sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                                sd.Month = ci.Month;
                                sd.StartDate = ci.StartDate;
                                sd.Type = ci.Type;
                                sd.WeekOfMonth = ci.WeekOfMonth;
                                sd.DayOfMonth = ci.DayOfMonth;

                                se.ExceptionsCalendar.Add(sd);
                            }
                            se.SetUp();
                            //if (running && reconnect)
                            //{
                            //    se.PrepareExecution(schedulingsessionname);
                            //}
                            if (running && modified)
                            {
                                if (se.Enable && s.ScheduleEngine != null)
                                    s.ScheduleEngine.PostEvent(se);
                            }

                        }

                    }
                }
                
                ufw.Disconnect();
            }
            rtLayer.Dispose();
        }

        private bool UpdateScheduledEvent(ScheduledEvent se, SimpleScheduledEvent sa)
        {
            {
                SchedulingDate sd;
                bool modified = (se.Date != sa.Date || se.Time != sa.Time || se.DateOff != sa.DateOff || se.TimeOff != sa.TimeOff);
                se.Date = sa.Date;
                se.Time = sa.Time;
                se.DateOff = sa.DateOff;
                se.TimeOff = sa.TimeOff;
                se.ExecOnAtStartup = sa.ExecOnAtStartup;
                se.ExecOffAtStartup = sa.ExecOffAtStartup;

                if (se.Type == ScheduleType.calendar)
                {
                    modified = true;
                    se.Calendar.Clear();
                    foreach (var ci in sa.Calendar)
                    {
                        sd = new SchedulingDate();
                        sd.DayOfWeek = ci.DayOfWeek;
                        sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                        sd.Month = ci.Month;
                        sd.StartDate = ci.StartDate;
                        sd.Type = ci.Type;
                        sd.WeekOfMonth = ci.WeekOfMonth;
                        sd.DayOfMonth = ci.DayOfMonth;
                        se.Calendar.Add(sd);
                    }
                }
                else if (se.Type == ScheduleType.weeklyPlan)
                {
                    modified = true;
                    se.WeeklyCalendar.Clear();
                    foreach (var ci in sa.WeeklyCalendar)
                    {
                        sd = new SchedulingDate();
                        sd.DayOfWeek = MSModel.Utils.GetCalendarDayType(ci.StartDate.DayOfWeek);
                        sd.EndDate = ci.EndDate;
                        sd.StartDate = ci.StartDate;
                        se.WeeklyCalendar.Add(sd);
                    }
                }                
                se.ExceptionsCalendar.Clear();
                modified = true;
                foreach (var ci in sa.ExceptionsCalendar)
                {
                    sd = new SchedulingDate();
                    sd.DayOfWeek = ci.DayOfWeek;
                    sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                    sd.Month = ci.Month;
                    sd.StartDate = ci.StartDate;
                    sd.Type = ci.Type;
                    sd.WeekOfMonth = ci.WeekOfMonth;
                    sd.DayOfMonth = ci.DayOfMonth;

                    se.ExceptionsCalendar.Add(sd);
                }
                se.SetUp();
                se.SetExecuteCommandOff();
                return modified;
            }
        }

        private void UpdateScheduledActionRuntime(MSScheduledActionRuntime sa, SimpleScheduledEvent se)
        {
            //bool modified = (se.Date != sa.Date || se.Time != sa.Time || se.DateOff != sa.DateOff || se.TimeOff != sa.TimeOff);
            sa.Date = se.Date;
            sa.Time = se.Time;
            sa.DateOff = se.DateOff;
            sa.TimeOff = se.TimeOff;
            sa.ExecOnAtStartup = se.ExecOnAtStartup;
            sa.ExecOffAtStartup = se.ExecOffAtStartup;

            if (se.Type == ScheduleType.calendar)
            {
                //modified = true;
                while (sa.Calendar.Count > 0)
                    sa.Calendar[0].Delete();

                foreach (var ci in se.Calendar)
                {
                    CalendarItem sd = new CalendarItem(sa.Session);
                    sd.DayOfWeek = ci.DayOfWeek;
                    sd.MSScheduledActionRuntime = sa;
                    sd.EndDate = ci.EndDate;
                    sd.Month = ci.Month;
                    sd.StartDate = ci.StartDate;
                    sd.Type = ci.Type;
                    sd.WeekOfMonth = ci.WeekOfMonth;
                    sd.DayOfMonth = ci.DayOfMonth;
                    sa.Calendar.Add(sd);
                }
            }
            else if (se.Type == ScheduleType.weeklyPlan)
            {
                //modified = true;

                while (sa.WeeklyCalendar.Count > 0)
                    sa.WeeklyCalendar[0].Delete();

                foreach(var ci in se.WeeklyCalendar)
                {
                    WeeklyCalendarItem sd = new WeeklyCalendarItem(sa.Session);
                    sd.EndDate = ci.EndDate;
                    sd.StartDate = ci.StartDate;
                    sd.MSScheduledActionRuntime = sa;
                    sa.WeeklyCalendar.Add(sd);
                }
            }

            while (sa.ExceptionsCalendar.Count > 0)
                sa.ExceptionsCalendar[0].Delete();

            foreach (var ci in se.ExceptionsCalendar)
            {
                ExceptionsCalendarItem sd = new ExceptionsCalendarItem(sa.Session);
                sd.DayOfWeek = ci.DayOfWeek;
                sd.MSScheduledActionRuntime = sa;
                sd.EndDate = ci.EndDate;
                sd.Month = ci.Month;
                sd.StartDate = ci.StartDate;
                sd.Type = ci.Type;
                sd.WeekOfMonth = ci.WeekOfMonth;
                sd.DayOfMonth = ci.DayOfMonth;
                sa.ExceptionsCalendar.Add(sd);
            }
            //return modified;
        }

        public override ServiceResult OnMethodCall(ISystemContext context,
                                                    MethodState method,
                                                    IList<object> inputArguments,
                                                    IList<object> outputArguments)
        {
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
            }
            catch (Exception e)
            {
                return StatusCodes.BadArgumentsMissing;
            }


            return ServiceResult.Good;
        }

        protected override void Dispose(bool disposing)
        {
            var s = this.server as MSUAServer;
            if(s != null)
                s.OServer.SystemEvent -= MSUANodeManager_SystemEvent;

            base.Dispose(disposing);
        }

        #endregion
        #region Methods
        BaseInstanceState CreateUtilsTagAndMethod()
        {
            //Create SchedulerUtils folder
            var folder = new FolderState(null) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            folder.NodeId = FromGuidToNodeId(MSServerInfo.Guids.ServiceFolderGuid);
            folder.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.Server);

            //Create Reload variable
            BaseDataVariableState variable = null;

            variable = new DataItemState(folder);
            string name = MSServerInfo.MSServerInfo.GetReloadVariableName();
            // set the symbolic name and reference types.
            Guid newGuid;
            if (!Guid.TryParse(MSServerInfo.MSServerInfo.GetReloadVariableGuid(), out newGuid))
                newGuid = Guid.NewGuid();
            variable.NodeId = FromGuidToNodeId(newGuid);
            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // initialize the variable from the type model.
            variable.Create(
                SystemContext,
                variable.NodeId,
                new QualifiedName(name, NamespaceIndex),
                null,
                false);


            if (folder == null)
            {
                variable.AddReference(ReferenceTypeIds.Organizes, true, folder.NodeId);
            }
            else
            {
                if (folder is BaseObjectState)
                    (folder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                folder.AddChild(variable);
            }

            variable.Description = ReloadVariableDescription;

            variable.DataType = DataTypeIds.Boolean;
            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            variable.Value = TypeInfo.GetDefaultValue(builtinType);
            variable.Timestamp = DateTime.UtcNow;
            variable.ValueRank = ValueRanks.Scalar;
            variable.ArrayDimensions = null;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            variable.Historizing = false;

            variable.OnSimpleWriteValue = OnWriteTagValue;
            mapNodeIdToNodeState.Add(variable.NodeId, variable);

            //Methods for the runtime editor of the schedulers
            CreateGetSchedulersListMethod(folder.NodeId, folder);
            CreateSetSchedulerSettingsMethod(folder.NodeId, folder);

            AddPredefinedNode(SystemContext, folder);
            mapNodeIdToNodeState.Add(folder.NodeId, folder);

            return variable;
        }

        public ServiceResult OnWriteTagValue(
           ISystemContext context,
           NodeState node,
           ref object value)
        {
            var variable = node as DataItemState;
            if (variable != null)
            {
                if (variable.SymbolicName == MSServerInfo.MSServerInfo.GetReloadVariableName())
                { 
                    
                    if (variable.DataType.IdType == IdType.Numeric 
                        && (uint)variable.DataType.Identifier == Opc.Ua.DataTypes.Boolean)
                    {
                        if (Convert.ToBoolean(value) == true)
                        {
                            //reload scheduler configuration.
                            //copy new runtime file on temp
                            
                            LoadRuntimeConfiguration(true);
                            value = false;
                        }
                    }
                }
                return StatusCodes.Good;
            }
            return StatusCodes.BadNodeIdInvalid;
        }

        BaseInstanceState CreateTag(MSScheduledAction ufuaTag, NodeId parent,
                                    NodeState parentFolder = null,
                                    bool bAssignNodeId = false, bool creatingType = false, bool forceRetentive = false)
        {
            BaseDataVariableState variable = null;

            switch (ufuaTag.ModelType)
            {
                case UFUAModel.ModelType.Variable:
                    {
                        variable = new DataItemState(parentFolder);
                        break;
                    }

                case UFUAModel.ModelType.Digital:
                    {
                        variable = new TwoStateDiscreteState(parentFolder);
                        break;
                    }
                case UFUAModel.ModelType.Enumerated:
                    {
                        variable = new MultiStateDiscreteState(parentFolder);
                        break;
                    }
                case UFUAModel.ModelType.Analog:
                    {
                        variable = new AnalogItemState(parentFolder);
                        break;
                    }
            }

            // set the symbolic name and reference types.
            if (!bAssignNodeId)
                variable.NodeId = FromGuidToNodeId(ufuaTag.NodeId);
            variable.SymbolicName = ufuaTag.Name;
            variable.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // initialize the variable from the type model.
            variable.Create(
                SystemContext,
                variable.NodeId,
                new QualifiedName(ufuaTag.Name, NamespaceIndex),
                null,
                bAssignNodeId);

            #region Add Properties
            if (ufuaTag.ModelType == UFUAModel.ModelType.Variable)
            {
                var node = variable as DataItemState;
                var property = node.AddProperty<bool>(MSServerInfo.MSServerInfo.GetEnableStateName(), DataTypeIds.Boolean, ValueRanks.Scalar);
                property.NodeId = ModelUtils.ConstructIdForComponent(property, NamespaceIndex);
                var propertyOff = node.AddProperty<bool>(MSServerInfo.MSServerInfo.GetExecuteOffName(), DataTypeIds.Boolean, ValueRanks.Scalar);
                propertyOff.NodeId = ModelUtils.ConstructIdForComponent(propertyOff, NamespaceIndex);
            }
            #endregion

            if (parentFolder == null)
            {
                variable.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(variable);
            }

            switch (ufuaTag.DataType)
            {
                case UFUAModel.DataType.Boolean: { variable.DataType = DataTypeIds.Boolean; break; }
                case UFUAModel.DataType.SByte: { variable.DataType = DataTypeIds.SByte; break; }
                case UFUAModel.DataType.Byte: { variable.DataType = DataTypeIds.Byte; break; }
                case UFUAModel.DataType.Int16: { variable.DataType = DataTypeIds.Int16; break; }
                case UFUAModel.DataType.UInt16: { variable.DataType = DataTypeIds.UInt16; break; }
                case UFUAModel.DataType.Int32: { variable.DataType = DataTypeIds.Int32; break; }
                case UFUAModel.DataType.UInt32: { variable.DataType = DataTypeIds.UInt32; break; }
                case UFUAModel.DataType.Int64: { variable.DataType = DataTypeIds.Int64; break; }
                case UFUAModel.DataType.UInt64: { variable.DataType = DataTypeIds.UInt64; break; }
                case UFUAModel.DataType.Float: { variable.DataType = DataTypeIds.Float; break; }
                case UFUAModel.DataType.Double: { variable.DataType = DataTypeIds.Double; break; }
                case UFUAModel.DataType.String: { variable.DataType = DataTypeIds.String; break; }
            }

            BuiltInType builtinType = TypeInfo.GetBuiltInType(variable.DataType, Server.TypeTree);
            variable.Value = TypeInfo.GetDefaultValue(builtinType);
            variable.StatusCode = StatusCodes.UncertainInitialValue;
            variable.Timestamp = DateTime.UtcNow;
            variable.ValueRank = ValueRanks.Scalar;
            variable.ArrayDimensions = null;
            variable.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.MinimumSamplingInterval = MinimumSamplingIntervals.Continuous;
            variable.Historizing = false;

            variable.OnSimpleWriteValue = OnWriteTagValue;
            mapNodeIdToNodeState.Add(variable.NodeId, variable);

            return variable;
        }

        protected virtual BaseObjectState CreateFolder(MSFolder ufuaFolder, NodeId parent,
                                     BaseObjectState parentFolder = null,
                                     bool bAssignNodeId = false, bool creatingType = false,
                                     bool bAssignFolderId = false, bool forceRetentive = false)
        {
            var folder = new FolderState(parentFolder) { TypeDefinitionId = ObjectTypeIds.FolderType };
            folder.BrowseName = new QualifiedName(ufuaFolder.Name, NamespaceIndex);
            folder.DisplayName = folder.BrowseName.Name;
            folder.SymbolicName = folder.BrowseName.Name;
            if (!bAssignFolderId)
                folder.NodeId = FromGuidToNodeId(ufuaFolder.NodeId);
            else
            {
                folder.NodeId = SystemContext.NodeIdFactory.New(SystemContext, folder);
                if (parentFolder != null)
                    parentFolder.AddChild(folder);
            }
            folder.AddReference(ReferenceTypeIds.Organizes, true, parent);

            foreach (var ufuainFolder in ufuaFolder.MSFolders)
            {
                CreateFolder(ufuainFolder, folder.NodeId, folder, bAssignNodeId, creatingType, bAssignFolderId, forceRetentive);
            }

            foreach (var ufuatag in ufuaFolder.MSScheduledActions)
            {
                CreateTag(ufuatag, folder.NodeId, folder, bAssignNodeId, creatingType, forceRetentive);
            }
            AddPredefinedNode(SystemContext, folder);
            mapNodeIdToNodeState.Add(folder.NodeId, folder);

            return folder;
        }

        BaseInstanceState CreateGetSchedulersListMethod(NodeId parent, NodeState parentFolder = null)
        {
            var methodName = MSServerInfo.MSServerInfo.GetSchedulersListMethodName();
            Guid newGuid;
            if (!Guid.TryParse(MSServerInfo.MSServerInfo.GetSchedulersListMethodGuid(), out newGuid))
                newGuid = Guid.NewGuid();

            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = FromGuidToNodeId(newGuid);
            methodBase.Description = GetSchedulersListMethodDescription;

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, false);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;

            List<Argument> args = new List<Argument>();

            args.Add(new Argument()
            {
                Name = "SchedulerName",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);

            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            
            args.Add(new Argument()
            {
                Name = "List",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.OneDimension
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            if (parentFolder == null)
            {
                methodBase.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(methodBase);
            }

            // set up method handlers. 
            methodBase.OnCallMethod = OnGetSchedulersList;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

            //AddCommunicationDriverToNodeStateDictionary(methodBase, driverName);

            return methodBase;
        }

        public virtual ServiceResult OnGetSchedulersList(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) || 
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;

            int i = 0;
            foreach (var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }

            var schedulerName = inputArguments[0].ToString();

            try
            {
                var s = this.server as MSUAServer;
                using (var dl = s.GetDataLayerActive())
                {
                    using (var ufw = new UnitOfWork(dl))
                    {
                        var rootTags = (from tag in new XPQuery<MSScheduledAction>(ufw).AsParallel()
                                        where tag.RuntimeSelectable == true
                                        select tag).ToList();

                        outputArguments.Clear();
                        if (!string.IsNullOrEmpty(schedulerName))
                        {

                            var org = rootTags.Find((o) => { return o.FullName == schedulerName; });
                            if (org != null)
                            {
                                NodeId idcmp = new NodeId(org.NodeId, NamespaceIndex);

                                lock (lockObj)
                                {
                                    ScheduledEvent sEvt = currentSchedules.Find((o) => { return o.NodeId == idcmp; });

                                    var se = PrepareSimpleScheduledEvent(org, sEvt);

                                    outputArguments.Add(se.ToXml());//fills a list of scheduler settings
                                }
                            }
                        }
                        else
                        {
                            foreach (var sa in rootTags)
                            {
                                NodeId idcmp = new NodeId(sa.NodeId, NamespaceIndex);
                                ScheduledEvent sEvt = currentSchedules.Find((o) => { return o.NodeId == idcmp; });
                                var se = PrepareSimpleScheduledEvent(sa, sEvt);
                                outputArguments.Add(se.ToXml());//fills a list of scheduler settings
                            }
                        }
                    }
                }                
            }
            catch(Exception ex)
            {
                return StatusCodes.Bad;
            }
            return ServiceResult.Good;
        }

        SimpleScheduledEvent PrepareSimpleScheduledEvent(MSScheduledAction sa, ScheduledEvent sevt)
        {
            SimpleScheduledEvent se = new SimpleScheduledEvent()
            {
                Guid = sa.NodeId,
                NodeId = new NodeId(sa.NodeId, NamespaceIndex),
                FullName = sa.FullName,
                Enable = sa.Enable.Value,
                Type = sa.Type,
                ValueOn = sa.ValueOn,
                ValueOff = sa.ValueOff,
                AccessLevels = sa.AccessLevels,
                AccessMasks = sa.AccessMasks,
                AccessRole = sa.AccessRole,
                ScheduleVariable = sa.ScheduleItem,
                EnableVariable = sa.EnableVariable,
            };
            if (sevt != null)
            {
                se.Date = sevt.Date;
                se.Time = sevt.Time;
                se.DateOff = sevt.DateOff;
                se.TimeOff = sevt.TimeOff;
                se.ExecOnAtStartup = sevt.ExecOnAtStartup;
                se.ExecOffAtStartup = sevt.ExecOffAtStartup;
                if (se.Type == ScheduleType.calendar)
                {
                    foreach(var ci in sevt.Calendar)
                    {
                        SimpleSchedulingDate sd = new SimpleSchedulingDate();
                        sd.DayOfWeek = ci.DayOfWeek;
                        sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                        sd.Month = ci.Month;
                        sd.StartDate = ci.StartDate;
                        sd.Type = ci.Type;
                        sd.WeekOfMonth = ci.WeekOfMonth;
                        sd.DayOfMonth = ci.DayOfMonth;
                        se.Calendar.Add(sd);
                    }
                }
                else if (se.Type == ScheduleType.weeklyPlan)
                {
                    foreach(var ci in sevt.WeeklyCalendar)
                    {
                        SimpleSchedulingDate sd = new SimpleSchedulingDate();
                        sd.DayOfWeek = MSModel.Utils.GetCalendarDayType(ci.StartDate.DayOfWeek);
                        sd.EndDate = ci.EndDate;
                        sd.StartDate = ci.StartDate;
                        se.WeeklyCalendar.Add(sd);
                    }
                }
                foreach (var ci in sevt.ExceptionsCalendar)
                {
                    SimpleSchedulingDate sd = new SimpleSchedulingDate();
                    sd.DayOfWeek = ci.DayOfWeek;
                    sd.EndDate = (ci.Type == CalendarItemType.SingleDate ? ci.StartDate.AddMinutes(1) : ci.EndDate);
                    sd.Month = ci.Month;
                    sd.StartDate = ci.StartDate;
                    sd.Type = ci.Type;
                    sd.WeekOfMonth = ci.WeekOfMonth;
                    sd.DayOfMonth = ci.DayOfMonth;
                    se.ExceptionsCalendar.Add(sd);
                }
            }
            else
                se.UpdateAction(sa);

            return se;
        }

        BaseInstanceState CreateSetSchedulerSettingsMethod(NodeId parent, NodeState parentFolder = null)
        {
            var methodName = MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName();
            Guid newGuid;
            if (!Guid.TryParse(MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodGuid(), out newGuid))
                newGuid = Guid.NewGuid();

            var methodBase = new MethodState(parentFolder);
            methodBase.BrowseName = new QualifiedName(methodName, NamespaceIndex);
            methodBase.SymbolicName = methodBase.BrowseName.Name;
            methodBase.DisplayName = methodBase.BrowseName.Name;
            methodBase.ReferenceTypeId = ReferenceTypeIds.HasComponent;
            methodBase.UserExecutable = true;
            methodBase.Executable = true;
            methodBase.NodeId = FromGuidToNodeId(newGuid);
            methodBase.Description = SetSchedulerSettingsMethodDescription;

            // initialize the variable from the type model.
            methodBase.Create(
                SystemContext,
                methodBase.NodeId,
                new QualifiedName(methodName, NamespaceIndex),
                null, false);

            // add input arguments.
            methodBase.InputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.InputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.InputArguments), NamespaceIndex);
            methodBase.InputArguments.BrowseName = BrowseNames.InputArguments;
            methodBase.InputArguments.DisplayName = methodBase.InputArguments.BrowseName.Name;
            methodBase.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.InputArguments.DataType = DataTypeIds.Argument;
            methodBase.InputArguments.ValueRank = ValueRanks.OneDimension;


            List<Argument> args = new List<Argument>();

            args.Add(new Argument()
            {
                Name = "Settings",
                DataType = DataTypeIds.String,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.InputArguments.Value = args.ToArray();
            mapNodeIdToInputArgs.Add(methodBase.NodeId, methodBase.InputArguments.Value);


            // add output arguments.
            methodBase.OutputArguments = new PropertyState<Argument[]>(methodBase);
            methodBase.OutputArguments.NodeId = new NodeId(String.Format("{0}.{1}", methodName, BrowseNames.OutputArguments), NamespaceIndex);
            methodBase.OutputArguments.BrowseName = BrowseNames.OutputArguments;
            methodBase.OutputArguments.DisplayName = methodBase.OutputArguments.BrowseName.Name;
            methodBase.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            methodBase.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            methodBase.OutputArguments.DataType = DataTypeIds.Argument;
            methodBase.OutputArguments.ValueRank = ValueRanks.OneDimension;

            args = new List<Argument>();
            args.Add(new Argument()
            {
                Name = "Result",
                DataType = DataTypeIds.Boolean,
                ValueRank = ValueRanks.Scalar,
                ArrayDimensions = null
            });

            methodBase.OutputArguments.Value = args.ToArray();
            mapNodeIdToOutputArgs.Add(methodBase.NodeId, methodBase.OutputArguments.Value);

            if (parentFolder == null)
            {
                methodBase.AddReference(ReferenceTypeIds.Organizes, true, parent);
            }
            else
            {
                if (parentFolder is BaseObjectState)
                    (parentFolder as BaseObjectState).ReferenceTypeId = ReferenceTypeIds.HasComponent;
                parentFolder.AddChild(methodBase);
            }

            // set up method handlers. 
            methodBase.OnCallMethod = OnSetSchedulerSettings;
            mapNodeIdToNodeState.Add(methodBase.NodeId, methodBase);

            return methodBase;
        }
        public virtual ServiceResult OnSetSchedulerSettings(
            ISystemContext context,
            MethodState method,
            IList<object> inputArguments,
            IList<object> outputArguments)
        {

            if (method.InputArguments != null && !mapNodeIdToInputArgs.ContainsKey(method.NodeId) ||
                method.OutputArguments != null && !mapNodeIdToOutputArgs.ContainsKey(method.NodeId))
                return StatusCodes.BadNodeIdInvalid;

            if (mapNodeIdToInputArgs.Count > 0 && mapNodeIdToInputArgs.ContainsKey(method.NodeId) &&
                inputArguments.Count < mapNodeIdToInputArgs[method.NodeId].Length)
                return StatusCodes.BadArgumentsMissing;


            int i = 0;
            foreach (var arg in inputArguments)
            {
                if (TypeInfo.GetDataTypeId(arg) != mapNodeIdToInputArgs[method.NodeId][i].DataType ||
                    TypeInfo.GetValueRank(arg) != mapNodeIdToInputArgs[method.NodeId][i].ValueRank)
                    return StatusCodes.BadTypeMismatch;
                ++i;
            }

            SimpleScheduledEvent scheduler;
            var schedulerSettings = inputArguments[0].ToString();
            try
            {
                scheduler = schedulerSettings.FromXml<SimpleScheduledEvent>();
            }
            catch (Exception e)
            {
                return StatusCodes.BadInvalidArgument;
            }
            outputArguments.Clear();
            outputArguments.Add(false);
            lock(lockObj)
            {
                var runningScheduler = currentSchedules.Find((o) => { return o.NodeId == scheduler.NodeId; });
                if (runningScheduler != null)
                {
                    var s = this.server as MSUAServer;
                    if (UpdateScheduledEvent(runningScheduler, scheduler) && runningScheduler.Enable && s.ScheduleEngine != null)
                    {
                        s.ScheduleEngine.PostEvent(runningScheduler);
                        //save permanently
                        if(SaveRuntimeConfiguration(scheduler))
                        {
                            outputArguments.Clear();
                            outputArguments.Add(true);
                        }
                    }
                }
            }

            return ServiceResult.Good;
        }


        #endregion
    }
}
