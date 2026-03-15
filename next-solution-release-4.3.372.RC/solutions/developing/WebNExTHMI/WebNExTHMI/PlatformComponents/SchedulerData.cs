using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSSchedulerSettings.Document;
using MSModel;
using UFUserEditor.ComponentService;
using DevExpress.Xpo;
using OPCUAViewModel;
using ViewModelLib;
using Opc.Ua;
using Utilities;
using UFInterfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace WebNExTHMI.PlatformComponents
{
    public class SchedulerData : IEntityReference, IDisposable
    {
        SchedulerEditorDocument schedulerDocument;
        DevExpress.Xpo.NestedUnitOfWork schedulerUow;
        OPCUAEntityReference GetSettingsListMethod;
        OPCUAEntityReference SetSettingsMethod;
        PropertyObserver<OPCUAEntityReference> observerGetList;
        readonly bool enableUserManager;
        VariantCollection outputParameters;
        const string defaultISORegion = "US";

        public SchedulerData(bool enableUserManager)
        {
            this.enableUserManager = enableUserManager;
        }

        public void Scheduler_InitServerConnection(IClientProxy caller, int idreference, OPCUAEntityReference reference, string connectionString, string sessionname, CancellationToken ct)
        {
            //var userEditor = screenDocument.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;

            //if (userEditor != null)
            //{
            //    try
            //    {
            //        enableUserManager = userEditor.GetEnableUserManager(screenDocument);
            //        /*...*/
            //    }
            //    catch (Exception)
            //    {
            //    }
            //}

            var server = reference.MonitoredItemViewModel;
            //string sessionname = string.Empty;
            //var doc = screenDocument;
            //if (doc != null && !string.IsNullOrEmpty(doc.SessionString))
            //    sessionname = doc.SessionString;

            schedulerDocument = InitSchedulerDocument(XpoHelpers.XpoHelper.NormalizeConnectionString(connectionString, PlatformComponents.GetProjectDocument()?.rootBase));

            if (ct.IsCancellationRequested)
                return;

            if (server != null)
            {
                string appName = string.Empty;
                var subscription = server.Parent as SubscriptionViewModel;
                if (subscription != null && subscription.Parent != null && subscription.Parent is SessionViewModel)
                    appName = (subscription.Parent as SessionViewModel).AppName;

                if (string.IsNullOrEmpty(appName))
                    return;

                if (GetSettingsListMethod == null)
                    GetSettingsListMethod = SchedulerEditorDocument.GetGeneralOPCUAEntityReference(appName, server.EndpointUrl,
                    string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSchedulersListMethodName()),
                    MSServerInfo.MSServerInfo.GetSchedulersListMethodGuid(),
                    MSServerInfo.MSServerInfo.GetSchedulersListMethodName());

                if (SetSettingsMethod == null)
                    SetSettingsMethod = SchedulerEditorDocument.GetGeneralOPCUAEntityReference(appName, server.EndpointUrl,
                    string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName()),
                    MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodGuid(),
                    MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName());

                if (string.IsNullOrEmpty(sessionname))
                    sessionname = GetSettingsListMethod.AppName;

                if (ct.IsCancellationRequested)
                    return;

                PrepareExecution(sessionname, idreference, caller);
            }
            else if (schedulerDocument != null)
            {
                GetSettingsListMethod = schedulerDocument.GetNodeIdOPCUAEntityReference(
                    string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSchedulersListMethodName()),
                    MSServerInfo.MSServerInfo.GetSchedulersListMethodGuid());
                SetSettingsMethod = schedulerDocument.GetNodeIdOPCUAEntityReference(
                    string.Format("{0}/{1}", MSServerInfo.MSServerInfo.GetSchedulerUtilsFolder(), MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodName()),
                MSServerInfo.MSServerInfo.GetSetSchedulerSettingsMethodGuid());

                if (string.IsNullOrEmpty(sessionname))
                    sessionname = GetSettingsListMethod.AppName;
                //sessionname = "daNExT_2976_Scheduler";

                if (ct.IsCancellationRequested)
                    return;

                PrepareExecution(sessionname, idreference, caller);
            }
        }

        public class SimpleScheduledEvent_Web
        {
            public string FullName { get; set; }
            public string SchedulerType { get; set; }
            public long? Time { get; set; }
            public long? TimeOff { get; set; }
            public string Tag { get; set; }
            public string ValueOn { get; set; }
            public string ValueOff { get; set; }
            public string EnableVar { get; set; }
            public bool ExecOn { get; set; }
            public bool ExecOff { get; set; }
            public string AccessRole { get; set; }
            public int AccessLevel { get; set; }
            public int AccessMasks { get; set; }
            public SimpleScheduledEvent_Web() { }
        }

        public class ScheduledEventWebData
        {
            public string NodeId { get; set; }
            public long? Time { get; set; }
            public long? TimeOff { get; set; }
            public bool ExecOn { get; set; }
            public bool ExecOff { get; set; }
        }

        public class MSScheduledAction_Web
        {
            public string eventNodeId;
            public string FullName { get; set; }
            public string NodeId { get; set; }
            public string Type { get; set; }
            public long? Time { get; set; }
            public long? TimeOff { get; set; }
            public bool ExecOn { get; set; }
            public bool ExecOff { get; set; }
            public string ScheduledEventNodeId
            {
                get
                {
                    return eventNodeId;
                }
                set
                {
                    if (value != eventNodeId)
                    {
                        NodeId = new NodeId(value).ToString();
                        eventNodeId = value;
                    }
                }
            }
            public List<CalendarItem_Web> CalendarItems = new List<CalendarItem_Web>();
            public List<CalendarItem_Web> ExceptionCalendarItems = new List<CalendarItem_Web>();
            public List<CalendarItem_Web> WeeklyCalendarItems = new List<CalendarItem_Web>();
            public MSScheduledAction_Web() { }
            public MSScheduledAction_Web(Tuple<MSScheduledAction, ScheduledEventWebData> ms)
            {
                ScheduledEventNodeId = ms.Item2.NodeId;
                ExecOn = ms.Item2.ExecOn;
                ExecOff = ms.Item2.ExecOff;
                Time = ms.Item2.Time;
                TimeOff = ms.Item2.TimeOff;
                FullName = ms.Item1.FullName;
                Type = ms.Item1.Type.ToString();
                foreach (var item in ms.Item1.Calendar)
                    CalendarItems.Add(new CalendarItem_Web(item));
                foreach (var item in ms.Item1.ExceptionsCalendar)
                    ExceptionCalendarItems.Add(new CalendarItem_Web(item));
                foreach (var item in ms.Item1.WeeklyCalendar)
                    CalendarItems.Add(new CalendarItem_Web(item));
            }
        }

        public class CalendarItem_Web
        {
            public long StartDate { get; set; }
            public long EndDate { get; set; }
            public string Month { get; set; }
            public string WeekOfMonth { get; set; }
            public string DayOfWeek { get; set; }
            public string ItemType { get; set; }
            public int DayOfMonth { get; set; }
            public CalendarItem_Web() { }
            public CalendarItem_Web(CalendarItem item)
            {
                StartDate = item.StartDate.Ticks;
                EndDate = item.EndDate.Ticks;
                Month = item.Month.ToString();
                WeekOfMonth = item.WeekOfMonth.ToString();
                ItemType = item.Type.ToString();
                DayOfWeek = item.DayOfWeek.ToString();
                DayOfMonth = item.DayOfMonth;
            }
            public CalendarItem_Web(ExceptionsCalendarItem item)
            {
                StartDate = item.StartDate.Ticks;
                EndDate = item.EndDate.Ticks;
                Month = item.Month.ToString();
                WeekOfMonth = item.WeekOfMonth.ToString();
                ItemType = item.Type.ToString();
                DayOfWeek = item.DayOfWeek.ToString();
                DayOfMonth = item.DayOfMonth;
            }
            public CalendarItem_Web(WeeklyCalendarItem item)
            {
                StartDate = item.StartDate.Ticks;
                EndDate = item.EndDate.Ticks;
            }
        }

        public List<SimpleScheduledEvent_Web> InitSchedulerOnRuntime(string schedulerName, string username, CancellationToken ct)
        {
            List<SimpleScheduledEvent_Web> lista = new List<SimpleScheduledEvent_Web>();
            if (GetSettingsListMethod != null && GetSettingsListMethod.NodeIdViewModel != null)
            {
                try
                {
                    outputParameters = GetSettingsListMethod.NodeIdViewModel.CallMethod(schedulerName == null ? string.Empty : schedulerName);
                }
                catch (Exception ex)
                {
                    //var syslog = LogManager.GetLogger(containerName);
                    //syslog.Error(String.Format(Properties.Resources.GetSettingsListMethodException), ex);
                    return null;
                }
                if (outputParameters.Count > 0)
                {
                    //System.Diagnostics.Trace.TraceInformation("GetServerListSchedulers lista {0}.{1}", DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond.ToString());
                    foreach (var s in outputParameters)
                    {
                        if (ct.IsCancellationRequested)
                            return null;

                        string scheddef = s.ToString();
                        var sched = scheddef.FromXml<SimpleScheduledEvent>();
                        if (sched != null)
                            lista.Add(new SimpleScheduledEvent_Web()
                            {
                                FullName = sched.FullName, //.Replace("\\", "/"),
                                SchedulerType = sched.Type.ToString(),
                                Time = sched.Time == DateTime.MinValue || sched.Time == DateTime.MaxValue ? new DateTime(1, 0, 0).Ticks : sched.Time.Ticks,
                                TimeOff = sched.Time == DateTime.MinValue || sched.Time == DateTime.MaxValue ? new DateTime(1, 0, 0).Ticks : sched.TimeOff.Ticks,
                                Tag = sched.ScheduleVariable != null ? sched.ScheduleVariable.FromXml<OPCUAEntityReference>().HumanReadable : string.Empty,
                                ValueOn = sched.ValueOn,
                                ValueOff = sched.ValueOff,
                                EnableVar = sched.EnableVariable != null ? sched.EnableVariable.FromXml<OPCUAEntityReference>().HumanReadable : string.Empty,
                                ExecOn = sched.ExecOnAtStartup,
                                ExecOff = sched.ExecOffAtStartup,
                                AccessRole = sched.AccessRole,
                                AccessLevel = sched.AccessLevels,
                                AccessMasks = sched.AccessMasks
                            });
                    }
                }
            }

            if (ct.IsCancellationRequested)
                return null;

            if (enableUserManager)
            {
                string currAccessRole = null;
                var currAccessLevel = -1;
                var currAccessMask = 0;
                if (username != null)
                {
                    currAccessRole = PlatformComponents.GetUserRole(username);
                    currAccessLevel = PlatformComponents.GetUserAccessLevel(username);
                    currAccessMask = PlatformComponents.GetUserAccessMask(username);
                }
                var filteredlist = (from tt in lista
                                    where (tt.AccessRole == null || (tt.AccessRole.Length == 0 || (tt.AccessRole == currAccessRole))) &&
                                        (tt.AccessLevel == 0 || (tt.AccessLevel <= currAccessLevel)) &&
                                        (tt.AccessMasks == 0 || (tt.AccessMasks & currAccessMask) != 0)
                                    orderby tt.FullName ascending
                                    select tt).ToList();
                return filteredlist;
            }
            else
                return lista.OrderBy(s => s.FullName).ToList();
        }

        public void PrepareExecution(String sessionname, int idreference, IClientProxy caller)
        {

            if (GetSettingsListMethod != null /*&& GetSettingsListMethod.IsValid*/)
            {
                if (observerGetList != null)
                {
                    observerGetList.UnregisterHandler(p => p.NodeIdViewModel);
                    observerGetList.Dispose();
                }

                observerGetList = new PropertyObserver<OPCUAEntityReference>(GetSettingsListMethod);
                observerGetList.RegisterHandler(n => n.NodeIdViewModel, n =>
                {
                    observerGetList.UnregisterHandler(p => p.NodeIdViewModel);
                    caller.SendAsync("GetSettingsListMethodResolved_" + /*screenId + "_" +*/ idreference, n.NodeIdViewModel.ToString());
                    //Dispatcher.BeginInvokeIfRequired(() =>
                    //{
                    //    SetEntityError(null);
                    //    InitSchedulerOnRuntime();
                    //});
                });

                GetSettingsListMethod.Resolve(sessionname, PlatformComponents.GetProjectDocument() /*screenDocument*/);
                GetSettingsListMethod.SetInUse(this, true);
            }

            if (SetSettingsMethod != null /*&& SetSettingsMethod.IsValid*/)
            {
                SetSettingsMethod.Resolve(sessionname, PlatformComponents.GetProjectDocument() /*screenDocument*/);
                SetSettingsMethod.SetInUse(this, true);
            }

        }

        SchedulerEditorDocument InitSchedulerDocument(string connectionString)
        {
            if (/*screenDocument == null ||*/ schedulerDocument != null)
                return schedulerDocument ?? null;

            if (!String.IsNullOrEmpty(connectionString))
            {
                schedulerDocument = MSSchedulerSettings.Document.SchedulerEditorDocument.FromConnectionString(connectionString, null);
            }
            else
            {
                var projectDoc = PlatformComponents.GetProjectDocument();
                //var uri = new Uri(screenDocument.Parent.rootBase, UriKind.RelativeOrAbsolute);
                var uri = new Uri(projectDoc.rootBase, UriKind.RelativeOrAbsolute);
                schedulerDocument = MSSchedulerSettings.Document.SchedulerEditorDocument.FromFile(uri.GetPathString(), null, projectDoc /*screenDocument*/, bCreateNew: true, bCheckEmpty: true);
            }

            if (schedulerDocument != null)
                schedulerUow = schedulerDocument.BeginNestedUnitOfWork();
            return schedulerDocument;
        }

        public void FillMSAction(MSScheduledAction ma, MSScheduledAction_Web wa)
        {
            //Name = se.FullName;
            //NodeId = se.Guid;
            //Enable = se.Enable;
            //Type = se.Type;
            //Date = se.Date;
            //DateOff = se.DateOff;
            //EnableVariable = se.EnableVariable;
            //ScheduleItem = se.ScheduleVariable;
            //ValueOn = se.ValueOn;
            //ValueOff = se.ValueOff;
            //AccessRole = se.AccessRole;
            //AccessLevels = se.AccessLevels;
            //AccessMasks = se.AccessMasks;
            ma.ExecOnAtStartup = wa.ExecOn;
            ma.ExecOffAtStartup = wa.ExecOff;
            if (wa.Time != null)
                ma.Time = new DateTime((long)wa.Time);
            if (wa.TimeOff != null)
                ma.TimeOff = new DateTime((long)wa.TimeOff);

            while (ma.WeeklyCalendar.Count > 0)
            {
                ma.WeeklyCalendar[0].Delete();
                if (ma.Calendar.Count > 0)
                    ma.Calendar[0].Delete();
            }
            foreach (var w in wa.WeeklyCalendarItems)
            {
                ma.WeeklyCalendar.Add(new WeeklyCalendarItem(schedulerUow)
                {
                    EndDate = new DateTime(w.EndDate),
                    StartDate = new DateTime(w.StartDate)
                });
            }

            while (ma.Calendar.Count > 0)
                ma.Calendar[0].Delete();
            if (wa.Type != ScheduleType.weeklyPlan.ToString())
            {
                foreach (var c in wa.CalendarItems)
                    ma.Calendar.Add(new CalendarItem(schedulerUow)
                    {
                        DayOfWeek = (CalendarDayType)Enum.Parse(typeof(CalendarDayType), c.DayOfWeek),
                        EndDate = new DateTime(c.EndDate),
                        Month = (CalendarMonthType)Enum.Parse(typeof(CalendarMonthType), c.Month),
                        StartDate = new DateTime(c.StartDate),
                        Type = (CalendarItemType)Enum.Parse(typeof(CalendarItemType), c.ItemType),
                        WeekOfMonth = (CalendarWeekType)Enum.Parse(typeof(CalendarWeekType), c.WeekOfMonth),
                        DayOfMonth = c.DayOfMonth
                    });
            }
            else
            {
                foreach (var w in wa.WeeklyCalendarItems)
                {
                    ma.Calendar.Add(new CalendarItem(schedulerUow)
                    {
                        DayOfWeek = (CalendarDayType)Enum.Parse(typeof(CalendarDayType), w.DayOfWeek),
                        EndDate = new DateTime(w.EndDate),
                        Month = CalendarMonthType.Any,
                        StartDate = new DateTime(w.StartDate),
                        Type = CalendarItemType.WeekNDate,
                        WeekOfMonth = CalendarWeekType.Any,
                        DayOfMonth = (int)MonthDayAny.NoDate
                    });
                }
            }

            while (ma.ExceptionsCalendar.Count > 0)
                ma.ExceptionsCalendar[0].Delete();
            foreach (var c in wa.ExceptionCalendarItems)
                ma.ExceptionsCalendar.Add(new ExceptionsCalendarItem(schedulerUow)
                {
                    DayOfWeek = c.DayOfWeek == null ? 0 : (CalendarDayType)Enum.Parse(typeof(CalendarDayType), c.DayOfWeek),
                    EndDate = new DateTime(c.EndDate),
                    Month = c.Month == null ? 0 : (CalendarMonthType)Enum.Parse(typeof(CalendarMonthType), c.Month),
                    StartDate = new DateTime(c.StartDate),
                    Type = (CalendarItemType)Enum.Parse(typeof(CalendarItemType), c.ItemType),
                    WeekOfMonth = c.WeekOfMonth == null ? 0 : (CalendarWeekType)Enum.Parse(typeof(CalendarWeekType), c.WeekOfMonth),
                    DayOfMonth = c.DayOfMonth
                });
        }

        public MSScheduledAction_Web AddMissingYearHolidays(string ISORegion, MSScheduledAction_Web clientAction, CancellationToken ct)
        {
            IEnumerable<Nager.Date.Model.PublicHoliday> publicHolidays;
            try
            {
                publicHolidays = Nager.Date.DateSystem.GetPublicHoliday(DateTime.Now.Year, (Nager.Date.CountryCode)Enum.Parse(typeof(Nager.Date.CountryCode), ISORegion));
            }
            catch (ArgumentException)
            {
                publicHolidays = Nager.Date.DateSystem.GetPublicHoliday(DateTime.Now.Year, (Nager.Date.CountryCode)Enum.Parse(typeof(Nager.Date.CountryCode), defaultISORegion));
            }
            var existingExceptions = clientAction.ExceptionCalendarItems;

            //merging with current client activities (no duplicates)
            foreach (var h in publicHolidays)
            {
                if (ct.IsCancellationRequested)
                    return null;

                CalendarItem_Web calItem = null;
                var endDate = new DateTime(h.Date.Year, h.Date.Month, h.Date.Day, 23, 59, 59);
                CalendarItem_Web stillPresent = null;
                if (!h.Fixed)
                {
                    if (clientAction != null)
                        stillPresent = (from item in existingExceptions.AsParallel()
                                        where
                                        item.ItemType == MSModel.CalendarItemType.DateRange.ToString() &&
                                        new DateTime(item.StartDate) == h.Date &&
                                        new DateTime(item.EndDate) == endDate
                                        select item
                        ).FirstOrDefault();
                    if (stillPresent == null)
                    {
                        calItem = new CalendarItem_Web()
                        {
                            ItemType = MSModel.CalendarItemType.DateRange.ToString(),
                            StartDate = h.Date.Ticks,
                            EndDate = endDate.Ticks
                        };
                    }
                }
                else
                {
                    if (existingExceptions != null)
                        stillPresent = (from item in existingExceptions.AsParallel()
                                        where
                                        item.ItemType == MSModel.CalendarItemType.WeekNDate.ToString() &&
                                        item.DayOfMonth == h.Date.Day + 2 &&
                                        item.Month == ((MSModel.CalendarMonthType)h.Date.Month).ToString() &&
                                        new DateTime(item.StartDate) == h.Date &&
                                        new DateTime(item.EndDate) == endDate
                                        select item
                        ).FirstOrDefault();
                    if (stillPresent == null)
                    {
                        calItem = new CalendarItem_Web()
                        {
                            ItemType = MSModel.CalendarItemType.WeekNDate.ToString(),
                            DayOfMonth = h.Date.Day + 2,
                            Month = ((MSModel.CalendarMonthType)h.Date.Month).ToString(),
                            StartDate = h.Date.Ticks,
                            EndDate = endDate.Ticks
                        };
                    }
                }
                if (calItem != null)
                {
                    clientAction.ExceptionCalendarItems.Add(calItem);
                    //var newcal = new NewCalendarItem(_stringlist, stringPlaceolder, runningOnServer, SchedEvent != null && SchedEvent.Type == MSModel.ScheduleType.weeklyPlan, true) { DataContext = calItem, CurrentCulture = CurrentCulture };
                }
            }

            return clientAction;
        }

        public bool SaveCurrentScheduler(MSScheduledAction_Web clientAction)
        {
            MSScheduledAction action;
            var p = (from tag in new XPQuery<MSModel.MSScheduledAction>(schedulerUow, true)
                     where tag.MSFolderAss == null && tag.Name == clientAction.FullName
                     select tag).ToList();
            if (p.Count == 0)
                return false;
            action = p[0];

            //updating MSScheduledAction with client data (MSScheduledAction_Web)
            FillMSAction(action, clientAction);
            //updating MSScheduledAction with client data (MSScheduledAction_Web)

            SimpleScheduledEvent se = new SimpleScheduledEvent();
            se.UpdateAction(action, false);
            //if (currSimpleScheduler != null)
            se.NodeId = new NodeId(clientAction.ScheduledEventNodeId); //currSimpleScheduler.NodeId;

            Exception exCaptured = null;
            //save se
            //SetBusy(true);
            //var task1 = Task.Factory.StartNew(() =>
            //{
            try
            {
                outputParameters = SetSettingsMethod.NodeIdViewModel.CallMethod(se.ToXml());
            }
            catch (Exception e)
            {
                exCaptured = e;
                return false;
            }
            //});

            //.............//

            return true;
        }

        //The selected scheduler was changed by the client: asking server for scheduler's data
        public Tuple<MSScheduledAction, ScheduledEventWebData> UpdateScheduler(string schedulerFullName)
        {
            //if (scheduler == null)
            //{
            //    if (currentEvent != null)
            //    {
            //        currentEvent.DataContext = null;
            //        currentEvent.UpdateData();
            //    }
            //    return;
            //}

            //this.scheduler = scheduler;

            if (GetSettingsListMethod?.NodeIdViewModel == null)
                return null;
            var sc = GetSettingsListMethod.NodeIdViewModel.CallMethod(schedulerFullName);
            if (sc == null || sc.Count == 0)
                return null;

            var schedulerStr = sc[0].ToString();
            var scheduler = schedulerStr.FromXml<SimpleScheduledEvent>();

            MSScheduledAction nScheduler;

            var p = (from tag in new XPQuery<MSModel.MSScheduledAction>(schedulerUow, true)
                     where tag.MSFolderAss == null && tag.Name == schedulerFullName
                     select tag).ToList();
            if (p.Count > 0)
                nScheduler = p[0];
            else
                nScheduler = new MSScheduledAction(schedulerUow);

            nScheduler.Fill(scheduler);

            return new Tuple<MSScheduledAction, ScheduledEventWebData>(nScheduler, new ScheduledEventWebData()
                {
                    NodeId = scheduler.NodeId.ToString(),
                    ExecOff = scheduler.ExecOffAtStartup,
                    ExecOn = scheduler.ExecOnAtStartup,
                    Time = scheduler.Time == DateTime.MinValue || scheduler.Time == DateTime.MaxValue ? new DateTime(1, 0, 0).Ticks : scheduler.Time.Ticks,
                    TimeOff = scheduler.Time == DateTime.MinValue || scheduler.Time == DateTime.MaxValue ? new DateTime(1, 0, 0).Ticks : scheduler.TimeOff.Ticks
                }
            );

            //var currSched = currentEvent != null ? currentEvent.DataContext as MSScheduledAction : null;
            //bool bConfirmed = false;
            //bool bShowDialog = !bRefreshing && !bDesignmode && currSimpleScheduler != null && currSched != null && currSched.NodeId == currSimpleScheduler.Guid && currSimpleScheduler.Modified(currSched, RunningOnServer);
            //if (bShowDialog)
            //{
            //    if (!RunningOnServer)
            //    {
            //        bConfirmed = System.Windows.Forms.MessageBox.Show(Properties.Resources.SaveQuestion, Properties.Resources.SchedulerSave, System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;
            //        if (bConfirmed)
            //            SaveCurrentScheduler(currSched, nScheduler, scheduler);
            //    }
            //    else
            //        WebDialogUC.Show(Properties.Resources.SaveQuestion);
            //}
            //if (!RunningOnServer && !bConfirmed && currentEvent != null)
            //    UpdateCurrentSchedulerData();
            //else if (RunningOnServer && !bShowDialog)
            //    UpdateCurrentSchedulerData();
        }

        public static int GetDayOfMonthIndex(string dayOfMonth)
        {
            var prefixes = new List<String> { "NoDate", "First", "Last" };
            var test = prefixes.IndexOf(dayOfMonth);
            return test != -1 ? test : int.Parse(dayOfMonth) + prefixes.Count - 1;
        }

        public static MSScheduledAction_Web GetMSScheduledActionWeb(string currentSchedulerFullName, string currentSchedulerNodeId, string currentSchedulerType, long? time, long? timeOff, bool execOn, bool execOff, List<object> calendar, List<object> holidays, List<object> week)
        {
            var action = new MSScheduledAction_Web()
            {
                FullName = currentSchedulerFullName,
                ScheduledEventNodeId = currentSchedulerNodeId,
                Type = currentSchedulerType,
                Time = time,
                TimeOff = timeOff,
                ExecOn = execOn,
                ExecOff = execOff
            };
            if (calendar != null)
                foreach (Dictionary<object, object> item in calendar)
                {
                    var dates = ParseClientDateTime(item["StartDate"], item["EndDate"]);
                    var startDate = dates.Item1;
                    var endDate = dates.Item2;
                    action.CalendarItems.Add(new CalendarItem_Web()
                    {
                        StartDate = startDate.Add(TimeSpan.FromMilliseconds((uint)item["StartDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        EndDate = endDate.Add(TimeSpan.FromMilliseconds((uint)item["EndDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        Month = (string)item["Month"],
                        WeekOfMonth = (string)item["WeekOfMonth"],
                        ItemType = (string)item["ItemType"],
                        DayOfWeek = (string)item["DayOfWeek"],
                        DayOfMonth = GetDayOfMonthIndex(item["DayOfMonth"].ToString())
                    });
                }
            if (holidays != null)
                foreach (Dictionary<object, object> item in holidays)
                {
                    var dates = ParseClientDateTime(item["StartDate"], item["EndDate"]);
                    var startDate = dates.Item1;
                    var endDate = dates.Item2;
                    action.ExceptionCalendarItems.Add(new CalendarItem_Web()
                    {
                        StartDate = startDate.Add(TimeSpan.FromMilliseconds((uint)item["StartDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        EndDate = endDate.Add(TimeSpan.FromMilliseconds((uint)item["EndDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        Month = (string)item["Month"],
                        WeekOfMonth = (string)item["WeekOfMonth"],
                        ItemType = (string)item["ItemType"],
                        DayOfWeek = (string)item["DayOfWeek"],
                        DayOfMonth = GetDayOfMonthIndex(item["DayOfMonth"].ToString())
                    });
                }
            if (week != null)
                foreach (Dictionary<object, object> item in week)
                {
                    var dates = ParseClientDateTime(item["StartDate"], item["EndDate"]);
                    var startDate = dates.Item1;
                    var endDate = dates.Item2;
                    action.WeeklyCalendarItems.Add(new CalendarItem_Web()
                    {
                        StartDate = startDate.Add(TimeSpan.FromMilliseconds((uint)item["StartDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        EndDate = endDate.Add(TimeSpan.FromMilliseconds((uint)item["EndDateOffset"])).Ticks, //.Subtract(TimeSpan.FromMilliseconds(clientTimezoneOffset)).Ticks,
                        DayOfWeek = item["DayOfWeek"].ToString()
                    });
                }
            return action;
        }

        public static Tuple<DateTime, DateTime> ParseClientDateTime(object sDate, object eDate)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            if (sDate as ulong? != null)
                startDate = new DateTime(PlatformComponents.timestampToTicks((ulong)sDate));
            if (eDate as ulong? != null)
                endDate = new DateTime(PlatformComponents.timestampToTicks((ulong)eDate));
            if (startDate == DateTime.MinValue)
                startDate = (sDate as DateTime?) != null ? (DateTime)sDate : (DateTime)((Dictionary<object, object>)sDate)["_d"];
            if (endDate == DateTime.MinValue)
                endDate = (eDate as DateTime?) != null ? (DateTime)eDate : (DateTime)((Dictionary<object, object>)eDate)["_d"];
            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        #region IEntityReference Members

        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

        #endregion

        public void Dispose()
        {
            if (schedulerUow != null)
            {
                schedulerUow.Disconnect();
                schedulerUow.Dispose();
                schedulerUow = null;
            }

            if (schedulerDocument != null)
                schedulerDocument.Dispose();
            schedulerDocument = null;

            if (GetSettingsListMethod != null /*&& GetSettingsListMethod.IsValid*/)
                GetSettingsListMethod.SetInUse(this, false);

            if (SetSettingsMethod != null /*&& SetSettingsMethod.IsValid*/)
                SetSettingsMethod.SetInUse(this, false);
        }
    }
}
