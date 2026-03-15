using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Opc.Ua;
using System.Threading;
using System.Globalization;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using System.IO;
using System.Text.RegularExpressions;
using XpoHelpers;
using Amib.Threading;
using log4net;
using Utilities;
using StatDef;

namespace UFUAAlarm
{
    public class SourceState : BaseObjectState
    {
        #region Declarations
        readonly ISystemContext context;
        readonly Dictionary<NodeId, AlarmConditionState> alarms;
        readonly Dictionary<String, AlarmConditionState> events;
        readonly Dictionary<NodeId, AlarmConditionState> branches;
        readonly Dictionary<NodeId, Timer> mapNodeToDelayTimer = new Dictionary<NodeId, Timer>();
        readonly Dictionary<NodeId, Timer> mapNodeToRateOfChangeTimer = new Dictionary<NodeId, Timer>();
        readonly List<AlarmStatus> pendingAlarmChanged = new List<AlarmStatus>();
        readonly Dictionary<NodeId, AlarmStatus> changedAlarmStatus = new Dictionary<NodeId, AlarmStatus>();
        readonly Dictionary<NodeId, DateTime> lastSavedTimes = new Dictionary<NodeId, DateTime>();
        readonly List<String> corruptFileNames = new List<String>();
        readonly List<AlarmStatus> listTotalTimeOn = new List<AlarmStatus>();

        DialogConditionState dialog;

        const double MAX_DUE_TIME_MSEC = 4294967294.0;

        static List<SourceState> saveAlarmStatusSources;
        static Timer saveAlarmStatusExecuter;
        static bool saveAlarmStatusExecuting;

        static Object lockStaticObject = new Object();
        static List<SourceState> totalTimeOnAlarmSources;
        static Dictionary<NodeId, DateTime> listDelayTime;
        static double lastDelayTime;
        static TimeSpan maxDelayTime = TimeSpan.FromMilliseconds(MAX_DUE_TIME_MSEC);
        static Timer totalTimeOnAlarmExecuter;

#if !NET_STANDARD
        static readonly ILog logServer = LogManager.GetLogger(Properties.Resources.Server);
#else
        static readonly ILog logServer = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.Server);
#endif

        string sourcePath;
        string dlConnString;
        int maxBranches;

        static SmartThreadPool smartThreadPool;

        readonly Object lockObject = new Object();
        #endregion

        #region Static Constructors / Methods

        static SourceState()
        {
            // The worker threads was forced to '1' to ensure that
            // all the actions are executed in the right order
            var startupInfo = new STPStartInfo()
            {
                ThreadPoolName = "UFUAAlarm.SourceState",
                MaxWorkerThreads = 1
            };

            smartThreadPool = new SmartThreadPool(startupInfo);

            saveAlarmStatusSources = new List<SourceState>();
        }

        public static void ExitPendingThreads()
        {
            if (smartThreadPool != null)
            {
                smartThreadPool.WaitForIdle();
                smartThreadPool.Shutdown();
            }

            AutoResetEvent waitHandle = null;
            lock (lockStaticObject)
            {
                if (totalTimeOnAlarmExecuter != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    totalTimeOnAlarmExecuter.Change(0, System.Threading.Timeout.Infinite);
                    totalTimeOnAlarmExecuter.Dispose(waitHandle);
                }
            }
            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
                waitHandle = null;
            }

            lock (saveAlarmStatusSources)
            {
                if (saveAlarmStatusExecuter != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    saveAlarmStatusExecuter.Change(0, System.Threading.Timeout.Infinite);
                    saveAlarmStatusExecuter.Dispose(waitHandle);
                }
            }
            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
                waitHandle = null;
            }

            lock (saveAlarmStatusSources)
            {
                while (saveAlarmStatusSources.Count > 0)
                {
                    saveAlarmStatusSources[0].SaveAlarmStatus(TimeSpan.Zero);
                    saveAlarmStatusSources.RemoveAt(0);
                }
            }
        }

        #endregion

        public SourceState(NodeState parent, String name, String path, NodeId nodeid, ISystemContext c)
            : base(parent)
        {
            Initialize(c);

            context = c;

            // create the table of conditions.
            alarms = new Dictionary<NodeId, AlarmConditionState>();
            events = new Dictionary<String, AlarmConditionState>();
            branches = new Dictionary<NodeId, AlarmConditionState>();

            // save source path for creating/updating alarm condition states
            sourcePath = path;

            // initialize the area with the fixed metadata.
            SymbolicName = name;
            NodeId = nodeid;
            BrowseName = new QualifiedName(Utils.Format("{0}", name), nodeid.NamespaceIndex);
            DisplayName = new LocalizedText(BrowseName.Name, String.Empty, BrowseName.Name);
            Description = null;
            ReferenceTypeId = null;
            TypeDefinitionId = ObjectTypeIds.BaseObjectType;
            EventNotifier = EventNotifiers.None;
        }

        public void Init(String connectionString, int maxBranches)
        {
            // create a dialog.
            dialog = CreateDialog(Properties.Resources.DialogName);
            dlConnString = connectionString;
            this.maxBranches = maxBranches;
        }

        #region UpdatedStatusEvent
        public event EventHandler<UpdatedStatusEventArgs> UpdatedStatusEvent;

        public virtual void OnUpdatedStatusEvent(UpdatedStatusEventArgs ea)
        {
            var t = UpdatedStatusEvent;
            if (t != null)
                t(this, ea);
        }
        #endregion

        #region AlarmsStatusChanged Event
        public event EventHandler<ChangedAlarmsArgs> AlarmsStatusChanged;
        public virtual void OnAlarmsStatusChanged(List<AlarmStatus> listChanged)
        {
            var t = AlarmsStatusChanged;
            if (t != null)
            {
                t(this, new ChangedAlarmsArgs() { nodeId = NodeId, alarmsStatus = listChanged });
            }
        }
        #endregion

        #region UpdateStatusChanged Event
        public event EventHandler<AlarmStateChangedArgs> AlarmStateChanged;

        Dictionary<NodeId, AlarmState> mapAlarmState;
        public virtual void OnAlarmStateChanged(NodeId nodeId, AlarmState newState, bool isMessage, bool canPlay, bool canAck, bool canReset)
        {
            AlarmState oldState = AlarmState.Undefined;
            lock (lockObject)
            {
                if (mapAlarmState == null)
                    mapAlarmState = new Dictionary<NodeId, AlarmState>();
                if (mapAlarmState.ContainsKey(nodeId))
                    oldState = mapAlarmState[nodeId];
                mapAlarmState[nodeId] = newState;
            }

            var t = AlarmStateChanged;
            if (t != null)
            {
                t(this, new AlarmStateChangedArgs()
                {
                    nodeId = nodeId,
                    oldState = oldState,
                    newState = newState,
                    isMessage = isMessage,
                    canPlay = canPlay,
                    canAck = canAck,
                    canReset = canReset
                });
            }
        }
        #endregion

        #region Access Rights
        /// <summary>
        /// Raised when the source node wants to know the access level of the condition.
        /// </summary>
        public NodeAttributeEventHandler<byte> OnReadUserAccessLevel;

        //bool CanUserReadNodeState(ISystemContext context, NodeState node)
        //{
        //    if (this.context == context)
        //        return true;

        //    var level = Opc.Ua.AccessLevels.CurrentReadOrWrite;
        //    var onReadUserAccessLevel = OnReadUserAccessLevel;
        //    if (onReadUserAccessLevel != null)
        //    {
        //        onReadUserAccessLevel(context, node, ref level);
        //    }

        //    return (level & Opc.Ua.AccessLevels.CurrentRead) == Opc.Ua.AccessLevels.CurrentRead ||
        //        (level & Opc.Ua.AccessLevels.CurrentReadOrWrite) == Opc.Ua.AccessLevels.CurrentReadOrWrite;
        //}

        bool CanUserWriteNodeState(ISystemContext context, NodeState node)
        {
            if (this.context == context)
                return true;

            var level = Opc.Ua.AccessLevels.CurrentReadOrWrite;
            var onReadUserAccessLevel = OnReadUserAccessLevel;
            if (onReadUserAccessLevel != null)
            {
                onReadUserAccessLevel(context, node, ref level);
            }

            return (level & Opc.Ua.AccessLevels.CurrentWrite) == Opc.Ua.AccessLevels.CurrentWrite ||
                (level & Opc.Ua.AccessLevels.CurrentReadOrWrite) == Opc.Ua.AccessLevels.CurrentReadOrWrite;
        }
        #endregion

        #region Persistence

        private IDataLayer CreateDataLayer(AlarmStatus alarmstatus, out IDisposable[] objectsToDisposeOnDisconnect, bool retry = true, string filename = null)
        {
            string conn = null;
            string file = null;
            try
            {
                string baseName = String.Format("{0}.{1}",this.sourcePath.Replace("/","."), alarmstatus.Name);
                conn = XpoHelper.GetConnectionString(dlConnString, Properties.Settings.Default.TypeLabel, filename ?? baseName, Properties.Settings.Default.DefaultFileExt);
                file = XpoHelper.GetDataSourceFilePath(conn);
                if (file != null && corruptFileNames.Contains(file))
                {
                    corruptFileNames.Remove(file);
                    if (File.Exists(file))
                    {
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                }

                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                dict.GetDataStoreSchema(typeof(AlarmPersistence).Assembly);
                return XpoDefault.GetDataLayer(conn, dict, AutoCreateOption.DatabaseAndSchema, out objectsToDisposeOnDisconnect);
                /* http://www.devexpress.com/Support/Center/Question/Details/Q535243
                var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                var store = XpoDefault.GetConnectionProvider(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                dict.GetDataStoreSchema(typeof(AlarmPersistence).Assembly);
                return new ThreadSafeDataLayer(dict, store);
                */
            }
            catch (PathTooLongException e)
            {
                if (filename == null && alarmstatus.Name != null)
                {
                    var index = alarmstatus.Name.LastIndexOf('.');
                    if (index != -1)
                        return CreateDataLayer(alarmstatus, out objectsToDisposeOnDisconnect, filename: alarmstatus.Name.Substring(index + 1));
                }
                else
                {
                    var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, alarmstatus.Name, e.Message);
                    var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                    Console.WriteLine(logMessage);

                    logServer.Error(message);
                }
            }
            catch (Exception e)
            {
                if (file != null && File.Exists(file))
                {
                    try
                    {
                        if (e.InnerException != null && e.InnerException is System.UnauthorizedAccessException)
                        {
                            File.SetAttributes(file, FileAttributes.Normal);
                            if (retry)
                                return CreateDataLayer(alarmstatus, out objectsToDisposeOnDisconnect, false);
                        }
                        File.Copy(file, file + ".bak", true);
                        File.Delete(file);
                    }
                    catch
                    {
                        corruptFileNames.Add(file);
                    }
                }

                var message = String.Format(Properties.Resources.ErrorCreatingDataLayer, alarmstatus.Name, e.InnerException != null ? e.InnerException.Message : e.Message);
                var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                Console.WriteLine(logMessage);

                logServer.Error(message);
            }

            objectsToDisposeOnDisconnect = new IDisposable[0];

            return null;
        }

        public void LoadAlarmStatus(AlarmStatus alarmstatus, DateTime minTimeStamp)
        {
            var conditions = new List<AlarmConditionState>();

            lock (lockObject)
            {
                alarmstatus.Time = alarmstatus.lastTimeUpdated = DateTime.UtcNow;

                IDisposable[] objectsToDisposeOnDisconnect;
                var dl = CreateDataLayer(alarmstatus, out objectsToDisposeOnDisconnect);
                if (dl != null)
                {
                    try
                    {
                        using (UnitOfWork ufw = new UnitOfWork(dl))
                        {
                            var alarm = new XPCollection<AlarmPersistence>(ufw)
                                .FirstOrDefault(p => p.NodeId == alarmstatus.nodeId.ToString() &&
                                                     p.ParentId == null);

                            if (alarm != default)
                            {
                                alarmstatus.Comment = alarm.Comment;
                                alarmstatus.UserName = alarm.UserName;
                                alarmstatus.TagAliasShowValues = alarm.TagAliasValue;
                                alarmstatus.Time = alarm.TimeStamp;
                                alarmstatus.parentId = alarm.ParentId;
                                alarmstatus.Occurence = alarm.Occurence;
                                alarmstatus.Sequence = alarm.Sequence;

                                alarmstatus.lastValue = alarm.Value;
                                alarmstatus.lastQuality = alarm.Quality;
                                alarmstatus.lastState = alarmstatus.state;
                                alarmstatus.lastTimeStateChanged = alarmstatus.lastTimeUpdated;
                                alarmstatus.lastTimeUpdated = alarm.LastTimeUpdated;
                                alarmstatus.ChangeStateTime = alarm.ChangeStateTime;


                                if (minTimeStamp == DateTime.UtcNow ||
                                    alarm.TimeStamp == DateTime.UtcNow ||
                                    alarm.TimeStamp > minTimeStamp)
                                {
                                    alarmstatus.state = alarm.State;
                                    alarmstatus.EnableTime = alarm.EnableTime;
                                    alarmstatus.AcknowledgeTime = alarm.AcknowledgeTime;
                                    alarmstatus.ConfirmTime = alarm.ConfirmTime;
                                    alarmstatus.SuppressTime = alarm.SuppressTime;
                                    alarmstatus.ActiveTime = alarm.ActiveTime;
                                    alarmstatus.ShelvingTime = alarm.ShelvingTime;
                                    alarmstatus.TimeOnShelf = alarm.TimeOnShelf;
                                    alarmstatus.isOffline = alarm.Offline;
                                }
                                else
                                    alarmstatus.SetStateBits(AlarmState.Deleted, true);


                                //var branchstatus = alarmstatus.CreateSnapshot();

                                // find the alarm node and map the system information to the UA defined alarm.
                                AlarmConditionState node = GetOrCreateAlarmConditionState(alarmstatus);

                                alarmstatus.SetStateBits(AlarmState.Shelved, false);

                                if (DateTime.UtcNow.Ticks - alarmstatus.ShelvingTime.Ticks < (long)alarmstatus.TimeOnShelf * TimeSpan.TicksPerMillisecond)
                                {
                                    double residualTime = alarmstatus.TimeOnShelf - (DateTime.UtcNow.Ticks - alarmstatus.ShelvingTime.Ticks) / TimeSpan.TicksPerMillisecond;
                                    OnShelve(context, node, true, false, residualTime);
                                }

                                conditions.Add(node);
                                UpdateAlarm(node, alarmstatus);
                            }

                            alarmstatus.CheckResetCounters(alarm == default);

                            var alarmbranches = new XPQuery<AlarmPersistence>(ufw)
                                .Where(p => p.ParentId == alarmstatus.nodeId.ToString());

                            foreach (var alarmbranch in alarmbranches)
                            {
                                var branchstatus = alarmstatus.CreateArchiveSnapshot();

                                branchstatus.Comment = alarmbranch.Comment;
                                branchstatus.UserName = alarmbranch.UserName;
                                branchstatus.lastValue = alarmbranch.Value;
                                branchstatus.TagAliasShowValues = alarmbranch.TagAliasValue;
                                branchstatus.lastQuality = alarmbranch.Quality;
                                branchstatus.lastTimeUpdated = alarmbranch.LastTimeUpdated;
                                branchstatus.Time = alarmbranch.TimeStamp;
                                branchstatus.EnableTime = alarmbranch.EnableTime;
                                branchstatus.AcknowledgeTime = alarmbranch.AcknowledgeTime;
                                branchstatus.ConfirmTime = alarmbranch.ConfirmTime;
                                branchstatus.SuppressTime = alarmbranch.SuppressTime;
                                branchstatus.ActiveTime = alarmbranch.ActiveTime;
                                branchstatus.ShelvingTime = alarmbranch.ShelvingTime;
                                branchstatus.TimeOnShelf = alarmbranch.TimeOnShelf;
                                branchstatus.state = alarmbranch.State;
                                branchstatus.isOffline = alarmbranch.Offline;
                                branchstatus.nodeId = alarmbranch.NodeId;

                                branchstatus.lastState = branchstatus.state;
                                branchstatus.lastTimeStateChanged = branchstatus.lastTimeUpdated;
                                branchstatus.ChangeStateTime = alarmbranch.ChangeStateTime;


                                // find the alarm branch.
                                if (NodeId.IsNull(branchstatus.nodeId))
                                    branchstatus.nodeId = new NodeId(Guid.NewGuid(), BrowseName.NamespaceIndex);
                                AlarmConditionState branch = GetOrCreateAlarmConditionState(branchstatus, branchstatus.nodeId);
                                conditions.Add(branch);
                                // map the system information to the UA defined alarm.
                                UpdateAlarm(branch, branchstatus);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = String.Format(Properties.Resources.ErrorOnLoading, alarmstatus.Name, ex.Message);
                        var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                        Console.WriteLine(logMessage);

                        logServer.Error(message);
                    }
                    finally
                    {
                        dl.Dispose();
                        foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                            obj.Dispose();
                    }
                }
            }

            // Reports the changes to the alarm.
            smartThreadPool.QueueWorkItem(() =>
            {
                ReportChanges(conditions);
            });
        }

        void SaveAlarmStatusDelayed(AlarmStatus alarmstatus)
        {
            lock (changedAlarmStatus)
            {
                changedAlarmStatus[alarmstatus.nodeId] = alarmstatus;
            }

            PromoteSaveAlarmStatusExecution();
        }

        void PromoteSaveAlarmStatusExecution()
        {
            lock (saveAlarmStatusSources)
            {
                if (!saveAlarmStatusSources.Contains(this))
                    saveAlarmStatusSources.Add(this);

                var delay = Properties.Settings.Default.DelaySavePersistence;
                PromoteSaveAlarmStatusExecution(TimeSpan.FromMilliseconds(delay));
            }
        }

        static void PromoteSaveAlarmStatusExecution(TimeSpan delayTime)
        {
            lock (saveAlarmStatusSources)
            {
                if (saveAlarmStatusExecuting || saveAlarmStatusExecuter != null)
                    return;

                saveAlarmStatusExecuter = new Timer((o) =>
                {
                    try
                    {
                        var sources = new List<SourceState>();
                        lock (saveAlarmStatusSources)
                        {
                            if (saveAlarmStatusExecuter != null)
                            {
                                saveAlarmStatusExecuter.Dispose();
                                saveAlarmStatusExecuter = null;
                            }

                            saveAlarmStatusExecuting = true;
                            sources.AddRange(saveAlarmStatusSources);
                            saveAlarmStatusSources.Clear();
                        }
#if DEBUG
                        var startingTime = DateTime.UtcNow;
                        var watcher = new System.Diagnostics.Stopwatch();
                        watcher.Start();
#endif
                        while (sources.Count > 0)
                        {
                            sources[0].SaveAlarmStatus(delayTime);
                            sources.RemoveAt(0);
                        }
#if DEBUG
                        watcher.Stop();
                        System.Diagnostics.Debug.WriteLine(String.Format("SourceState - Saved Alarms Status, started {0}, executed in {1}", startingTime, watcher.Elapsed));
#endif
                    }
                    finally
                    {
                        lock (saveAlarmStatusSources)
                        {
                            saveAlarmStatusExecuting = false;
                            if (saveAlarmStatusSources.Count > 0)
                                PromoteSaveAlarmStatusExecution(delayTime);
                        }
                    }
                }, null, delayTime, TimeSpan.FromMilliseconds(-1));
            }
        }

        void SaveAlarmStatus(TimeSpan delayTime)
        {
            var changedAlarms = new List<AlarmStatus>();
            lock (changedAlarmStatus)
            {
                changedAlarms.AddRange(changedAlarmStatus.Values);
                changedAlarmStatus.Clear();
            }

            var unsavedAlarms = new List<AlarmStatus>();
            while (changedAlarms.Count > 0)
            {
                var alarm = changedAlarms[0];
                changedAlarms.RemoveAt(0);

                if (delayTime == TimeSpan.Zero ||
                    !lastSavedTimes.ContainsKey(alarm.nodeId) ||
                    (DateTime.UtcNow - lastSavedTimes[alarm.nodeId] > delayTime))
                {
                    SaveAlarmStatus(alarm);
                    lastSavedTimes[alarm.nodeId] = DateTime.UtcNow;
                }
                else
                    unsavedAlarms.Add(alarm);
            }

            if (unsavedAlarms.Count > 0)
            {
                lock (changedAlarmStatus)
                {
                    while (unsavedAlarms.Count > 0)
                    {
                        var alarm = unsavedAlarms[0];
                        unsavedAlarms.RemoveAt(0);

                        if (!changedAlarmStatus.ContainsKey(alarm.nodeId))
                            changedAlarmStatus.Add(alarm.nodeId, alarm);
                    }
                }

                PromoteSaveAlarmStatusExecution();
            }
        }

        public void SaveAlarmStatus(AlarmStatus alarmstatus)
        {
            //lock (lockObject)
            {
                IDisposable[] objectsToDisposeOnDisconnect;
                var dl = CreateDataLayer(alarmstatus, out objectsToDisposeOnDisconnect);
                if (dl != null)
                {
                    try
                    {
                        using (UnitOfWork ufw = new UnitOfWork(dl))
                        {
                            var parentId = NodeId.IsNull(alarmstatus.parentId) ? null : alarmstatus.parentId.ToString();
                            var alarm = (from persistence in new XPQuery<AlarmPersistence>(ufw)/*.AsParallel()*/
                                         where persistence.NodeId == alarmstatus.nodeId.ToString() &&
                                         persistence.ParentId == parentId
                                         select persistence).ToList();

                            ufw.Delete(alarm);

                            if (NodeId.IsNull(alarmstatus.parentId) || (alarmstatus.state & AlarmState.Deleted) == 0)
                            {
                                AlarmPersistence alarmpersistence = new AlarmPersistence(ufw)
                                {
                                    NodeId = alarmstatus.nodeId.ToString(),
                                    Comment = alarmstatus.Comment,
                                    UserName = alarmstatus.UserName,
                                    Value = alarmstatus.lastValue,
                                    TagAliasValue = alarmstatus.TagAliasShowValues,
                                    Quality = alarmstatus.lastQuality,
                                    LastTimeUpdated = alarmstatus.lastTimeUpdated,
                                    TimeStamp = alarmstatus.Time,
                                    EnableTime = alarmstatus.EnableTime,
                                    AcknowledgeTime = alarmstatus.AcknowledgeTime,
                                    ConfirmTime = alarmstatus.ConfirmTime,
                                    SuppressTime = alarmstatus.SuppressTime,
                                    ActiveTime = alarmstatus.ActiveTime,
                                    ShelvingTime = alarmstatus.ShelvingTime,
                                    TimeOnShelf = alarmstatus.TimeOnShelf,
                                    State = alarmstatus.state,
                                    Offline = alarmstatus.isOffline,
                                    ParentId = parentId,
                                    Occurence = alarmstatus.Occurence,
                                    Sequence = alarmstatus.Sequence,
                                    ChangeStateTime = alarmstatus.ChangeStateTime
                                };
                            }

                            ufw.CommitChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = String.Format(Properties.Resources.ErrorOnSaving, alarmstatus.Name, ex.Message);
                        var logMessage = String.Format("{0} - {1}", DateTime.Now, message);
                        Console.WriteLine(logMessage);

                        logServer.Error(message);
                    }
                    finally
                    {
                        dl.Dispose();
                        foreach (IDisposable obj in objectsToDisposeOnDisconnect)
                            obj.Dispose();
                    }
                }
            }
        }

        #endregion

        #region TotalTimeOn Updater
        static void StartTotalTimeOnUpdater(SourceState source, AlarmStatus settings)
        {
            lock (lockStaticObject)
            {
                if (totalTimeOnAlarmSources == null)
                    totalTimeOnAlarmSources = new List<SourceState>();
                if (!totalTimeOnAlarmSources.Contains(source))
                    totalTimeOnAlarmSources.Add(source);
                if (listDelayTime == null)
                    listDelayTime = new Dictionary<NodeId, DateTime>();
                listDelayTime[settings.nodeId] = DateTime.UtcNow.AddSeconds(Math.Max(settings.ActivationValue - settings.lastValue, 0.0));
                StartTotalTimeOnUpdater();
            }
        }

        static void StartTotalTimeOnUpdater()
        {
            lock (lockStaticObject)
            {
                if (totalTimeOnAlarmExecuter != null)
                {
                    var minDelayTime = Math.Max((listDelayTime.Values.Min<DateTime>() - DateTime.UtcNow).TotalSeconds, 0.0);
                    if (minDelayTime < lastDelayTime)
                    {
                        lastDelayTime = minDelayTime;
                        totalTimeOnAlarmExecuter.Change(TimeSpan.FromSeconds(lastDelayTime), TimeSpan.FromMilliseconds(-1));
                    }
                    return;
                }

                lastDelayTime = Math.Max((listDelayTime.Values.Min<DateTime>() - DateTime.UtcNow).TotalSeconds, 0.0);
                var dueTime = TimeSpan.FromSeconds(lastDelayTime);
                if (dueTime > maxDelayTime)
                    dueTime = maxDelayTime;
                totalTimeOnAlarmExecuter = new Timer((o) =>
                {
                    var sources = new List<SourceState>();
                    lock (lockStaticObject)
                    {
                        if (totalTimeOnAlarmExecuter != null)
                        {
                            totalTimeOnAlarmExecuter.Dispose();
                            totalTimeOnAlarmExecuter = null;
                        }

                        if (totalTimeOnAlarmSources != null)
                            sources.AddRange(totalTimeOnAlarmSources);
                    }

                    //var oldPriority = Thread.CurrentThread.Priority;

                    try
                    {
                        //try
                        //{
                        //    Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
                        //}
                        //catch { }

#if DEBUG
                        var startingTime = DateTime.UtcNow;
                        var watcher = new System.Diagnostics.Stopwatch();
                        watcher.Start();
#endif

                        sources.ForEach((source) =>
                        {
                            source.UpdateTimeOnAlarms();
                        });
#if DEBUG
                        watcher.Stop();
                        System.Diagnostics.Debug.WriteLine(String.Format("SourceState - Update Total Time On Alarms, started {0}, executed in {1}", startingTime, watcher.Elapsed));
#endif
                    }
                    finally
                    {
                        //try
                        //{
                        //    Thread.CurrentThread.Priority = oldPriority;
                        //}
                        //catch { }

                        lock (lockStaticObject)
                        {
                            if (totalTimeOnAlarmSources.Count > 0)
                                StartTotalTimeOnUpdater();
                        }
                    }
                }, null, dueTime, TimeSpan.FromMilliseconds(-1));
            }
        }

        void StartTotalTimeOnUpdater(AlarmStatus settings)
        {
            if (!settings.isTotalTimeOn || settings.StatisticsData == null)
                return;

            bool isAdded = false;
            bool isRemoved = false;
            bool isEmpty = false;
            lock (lockObject)
            {
                bool wasEmpty = listTotalTimeOn.Count == 0;
                if (!settings.StatisticsData.IsTotalTimeOnActive || (settings.state & AlarmState.Active) != 0)
                    isRemoved = listTotalTimeOn.Remove(settings);
                else if (!listTotalTimeOn.Contains(settings))
                {
                    isAdded = true;
                    listTotalTimeOn.Add(settings);
                }

                isEmpty = !wasEmpty && listTotalTimeOn.Count == 0;
            }

            if (isAdded)
            {
                StartTotalTimeOnUpdater(this, settings);
            }
            else if (isEmpty)
            {
                lock (lockStaticObject)
                {
                    if (listDelayTime != null)
                        listDelayTime.Remove(settings.nodeId);
                    if (totalTimeOnAlarmSources != null)
                        totalTimeOnAlarmSources.Remove(this);
                    if (totalTimeOnAlarmSources.Count == 0)
                    {
                        if (totalTimeOnAlarmExecuter != null)
                        {
                            totalTimeOnAlarmExecuter.Dispose();
                            totalTimeOnAlarmExecuter = null;
                        }
                    }
                }
            }
            else if (isRemoved)
            {
                lock (lockStaticObject)
                {
                    if (listDelayTime != null)
                        listDelayTime.Remove(settings.nodeId);
                }
            }
        }

        void UpdateTimeOnAlarms()
        {
            var nodeIds = new List<NodeId>();
            lock (lockStaticObject)
            {
                nodeIds.AddRange(from key in listDelayTime.Keys
                                 where listDelayTime[key] <= DateTime.UtcNow
                                 select key);
            }

            var pending = new List<AlarmStatus>();
            lock (lockObject)
            {
                pending.AddRange(from c in listTotalTimeOn
                                 where nodeIds.Contains(c.nodeId)
                                 select c);
            }

            foreach (var settings in pending)
            {
                UpdateAlarmStatus(settings,
                    new DataValue(new Variant(settings.StatisticsData.TotalTimeOn.ToString(null,
                    System.Globalization.CultureInfo.InvariantCulture)), settings.lastQuality));
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Returns the last event produced for any conditions belonging to the node or its chilren.
        /// </summary>
        /// <param name="context">The system context.</param>
        /// <param name="events">The list of condition events to return.</param>
        /// <param name="includeChildren">Whether to recursively report events for the children.</param>
        public override void ConditionRefresh(ISystemContext context, List<IFilterTarget> events, bool includeChildren)
        {
            // need to check if this source has already been processed during this refresh operation.
            for (int ii = 0; ii < events.Count; ii++)
            {
                InstanceStateSnapshot e = events[ii] as InstanceStateSnapshot;

                if (e != null && Object.ReferenceEquals(e.Handle, this))
                {
                    return;
                }
            }

            // report the dialog.
            if (dialog != null)
            {
                // do not refresh dialogs that are not active.
                if (dialog.Retain.Value)
                {
                    // create a snapshot.
                    InstanceStateSnapshot e = new InstanceStateSnapshot();
                    e.Initialize(context, dialog);

                    // set the handle of the snapshot to check for duplicates.
                    e.Handle = this;

                    events.Add(e);
                }
            }

            // the alarm objects act as a cache for the last known state and are used to generate refresh events.
            lock (lockObject)
            {
                if (disposed)
                    return;

                foreach (AlarmConditionState alarm in alarms.Values)
                {
                    // do not refresh alarms that are not in an interesting state.
                    if (!alarm.Retain.Value)
                    {
                        continue;
                    }

                    // create a snapshot.
                    InstanceStateSnapshot e = new InstanceStateSnapshot();
                    e.Initialize(context, alarm);

                    // set the handle of the snapshot to check for duplicates.
                    e.Handle = this;

                    events.Add(e);
                }

                // report any active branches.
                foreach (AlarmConditionState alarm in branches.Values)
                {
                    // create a snapshot.
                    InstanceStateSnapshot e = new InstanceStateSnapshot();
                    e.Initialize(context, alarm);

                    // set the handle of the snapshot to check for duplicates.
                    e.Handle = this;

                    events.Add(e);
                }
            }
        }
        #endregion

        #region Private Methods
        private void OnAlarmChanged()
        {
            var listToAdd = new List<AlarmStatus>();
            var list = new List<AlarmStatus>();
            lock (lockObject)
            {
                list.AddRange(pendingAlarmChanged.ToList());
                pendingAlarmChanged.Clear();

                listToAdd.AddRange((from c in list where !alarms.ContainsKey(c.nodeId) && !branches.ContainsKey(c.nodeId) select c).ToList());
            }

            foreach (var alarm in listToAdd)
            {
                if (!NodeId.IsNull(alarm.parentId) && NodeId.IsNull(alarm.nodeId))
                    alarm.nodeId = new NodeId(Guid.NewGuid(), BrowseName.NamespaceIndex);

                GetOrCreateAlarmConditionState(alarm, alarm.nodeId);
            }

            var conditions = new List<AlarmConditionState>();
            var listToRemove = new List<NodeId>();
            lock (lockObject)
            {
                var listTimeAscending = (from c in list orderby c.lastTimeUpdated ascending select c).ToList();

                foreach (var alarm in listTimeAscending)
                {
                    if (!NodeId.IsNull(alarm.parentId))
                    {
                        if (NodeId.IsNull(alarm.nodeId))
                            alarm.nodeId = new NodeId(Guid.NewGuid(), BrowseName.NamespaceIndex);
                        // find or create branch
                        AlarmConditionState branch = GetOrCreateAlarmConditionState(alarm, alarm.nodeId);
                        conditions.Add(branch);
                        // map the system information to the UA defined alarm.
                        UpdateAlarm(branch, alarm);
                        SaveAlarmStatusDelayed(alarm);

                        if (alarm.notify && alarm.SaveEventsLog && alarm.isLogEntry)
                        {
                            OnUpdatedStatusEvent(new UpdatedStatusEventArgs()
                            {
                                NodeId = alarm.nodeId,
                                Condition = branch,
                                EventComment = alarm.EventComment,
                                EventUserName = alarm.EventUserName,
                                Occurence = alarm.Occurence,
                                Sequence = alarm.Sequence,
                                Duration = alarm.activeStateDuration
                            });
                        }

                        // delete the branch.
                        if ((alarm.state & AlarmState.Deleted) != 0)
                        {
                            listToRemove.Add(alarm.nodeId);
                        }
                        continue;
                    }

                    AlarmConditionState node = GetOrCreateAlarmConditionState(alarm);
                    conditions.Add(node);
                    // map the system information to the UA defined alarm.
                    bool wasActive = node.ActiveState.Id.Value;
                    UpdateAlarm(node, alarm);
                    SaveAlarmStatusDelayed(alarm);

                    if (alarm.notify && alarm.SaveEventsLog && alarm.isLogEntry)
                    {
                        OnUpdatedStatusEvent(new UpdatedStatusEventArgs()
                        {
                            NodeId = alarm.nodeId,
                            Condition = node,
                            EventComment = alarm.EventComment,
                            EventUserName = alarm.EventUserName,
                            Occurence = alarm.Occurence,
                            Sequence = alarm.Sequence,
                            Duration = alarm.activeStateDuration,
                            UseTimeStamp = alarm.UseTimeStamp && wasActive != node.ActiveState.Id.Value
                        });
                    }
                }
            }

            if (listToRemove.Count > 0)
            {
                lock (lockObject)
                {
                    listToRemove.ForEach((item) =>
                    {
                        AlarmConditionState branch = null;
                        if (branches.TryGetValue(item, out branch))
                        {
                            events.Remove(Utils.ToHexString(branch.EventId.Value));
                        }
                        branches.Remove(item);
                    });
                }
            }

            ReportChanges(conditions);

            var listToNotify = (from c in list where c.notify select c).ToList();
            if (listToNotify.Count > 0)
            {
                OnAlarmsStatusChanged(listToNotify);
            }
        }

        private void UpdateAlarm(AlarmConditionState node, AlarmStatus alarm)
        {
            // remove old event.
            if (node.EventId.Value != null)
            {
                lock (lockObject)
                {
                    events.Remove(Utils.ToHexString(node.EventId.Value));
                }
            }

            // update the basic event information (include generating a unique id for the event).
            if (alarm.serverEventId != null)
                node.EventId.Value = alarm.serverEventId;
            else
                node.EventId.Value = Guid.NewGuid().ToByteArray();
            alarm.serverEventId = node.EventId.Value;
            node.Time.Value = DateTime.UtcNow;
            node.ReceiveTime.Value = node.Time.Value;

            lock (lockObject)
            {
                // save the event for later lookup.
                events[Utils.ToHexString(node.EventId.Value)] = node;
            }

            // determine the retain state.
            node.Retain.Value = true;

            node.Time.Value = alarm.Time;

            // update the states.
            node.SetEnableState(context, (alarm.state & AlarmState.Enabled) != 0);
            node.SetConfirmedState(context, (alarm.state & AlarmState.Confirmed) != 0);
            node.SetAcknowledgedState(context, (alarm.state & AlarmState.Acknowledged) != 0);
            node.SetActiveState(context, (alarm.state & AlarmState.Active) != 0);
            node.SetSuppressedState(context, (alarm.state & AlarmState.Suppressed) != 0);

            // update other information.
            if (node.BranchId == null || node.BranchId.Value == null || NodeId.IsNull(node.BranchId.Value))
            {
                if(node.SuppressedState != null)
                    node.SuppressedState.TransitionTime.Value = alarm.SuppressTime;
                if(node.ShelvingState != null)
                    node.ShelvingState.LastTransition.TransitionTime.Value = alarm.ShelvingTime;
            }

            if (!String.IsNullOrEmpty(alarm.Description))
            {
                if (alarm.TagAliasShowValues != null && alarm.TagAliasShowValues.Length > 0)
                {
                    var args = new List<object>(alarm.TagAliasShowValues);
                    args.Add(alarm.Description);
                    node.Message.Value = new LocalizedText(alarm.Message, string.Empty, String.Format("{0} - {1}", alarm.Description, alarm.Message), args.ToArray());
                }
                else
                    node.Message.Value = new LocalizedText(alarm.Message, string.Empty, String.Format("{0} - {1}", alarm.Description, alarm.Message), alarm.Description);
            }
            else
            {
                node.Message.Value = new LocalizedText(alarm.Message, string.Empty, alarm.Message, alarm.TagAliasShowValues);
            }
            node.SetComment(context, alarm.Comment, alarm.UserName);
            node.SetSeverity(context, alarm.Severity);

            // update quality information.
            if (node.Quality.Value != alarm.lastQuality)
            {
                node.Quality.Value = alarm.lastQuality;
                node.Quality.Timestamp = DateTime.UtcNow;
            }

            // check for deleted items.
            if ((alarm.state & AlarmState.Deleted) != 0)
            {
                node.Retain.Value = false;
            }

            // handle non exclusive high-low alarms.
            NonExclusiveLimitAlarmState highLowAlarm = node as NonExclusiveLimitAlarmState;

            if (highLowAlarm != null)
            {
                if (alarm.Limits[0].HasValue)
                    highLowAlarm.HighHighLimit.Value = alarm.Limits[0].Value;
                if (alarm.Limits[1].HasValue)
                    highLowAlarm.HighLimit.Value = alarm.Limits[1].Value;
                if (alarm.Limits[2].HasValue)
                    highLowAlarm.LowLimit.Value = alarm.Limits[2].Value;
                if (alarm.Limits[3].HasValue)
                    highLowAlarm.LowLowLimit.Value = alarm.Limits[3].Value;

                LimitAlarmStates limit = LimitAlarmStates.Inactive;

                if ((alarm.state & AlarmState.HighHigh) != 0)
                {
                    limit |= LimitAlarmStates.HighHigh;
                }

                if ((alarm.state & AlarmState.High) != 0)
                {
                    limit |= LimitAlarmStates.High;
                }

                if ((alarm.state & AlarmState.Low) != 0)
                {
                    limit |= LimitAlarmStates.Low;
                }

                if ((alarm.state & AlarmState.LowLow) != 0)
                {
                    limit |= LimitAlarmStates.LowLow;
                }

                highLowAlarm.SetLimitState(context, limit);
            }

            // handle exclusive high-low alarms.
            ExclusiveLimitAlarmState ehighLowAlarm = node as ExclusiveLimitAlarmState;

            if (ehighLowAlarm != null)
            {
                if (alarm.Limits[0].HasValue)
                    ehighLowAlarm.HighHighLimit.Value = alarm.Limits[0].Value;
                if (alarm.Limits[1].HasValue)
                    ehighLowAlarm.HighLimit.Value = alarm.Limits[1].Value;
                if (alarm.Limits[2].HasValue)
                    ehighLowAlarm.LowLimit.Value = alarm.Limits[2].Value;
                if (alarm.Limits[3].HasValue)
                    ehighLowAlarm.LowLowLimit.Value = alarm.Limits[3].Value;

                LimitAlarmStates limit = LimitAlarmStates.Inactive;

                if ((alarm.state & AlarmState.HighHigh) != 0)
                {
                    limit |= LimitAlarmStates.HighHigh;
                }

                if ((alarm.state & AlarmState.High) != 0)
                {
                    limit |= LimitAlarmStates.High;
                }

                if ((alarm.state & AlarmState.Low) != 0)
                {
                    limit |= LimitAlarmStates.Low;
                }

                if ((alarm.state & AlarmState.LowLow) != 0)
                {
                    limit |= LimitAlarmStates.LowLow;
                }

                ehighLowAlarm.SetLimitState(context, limit);
            }

            // handle non exclusive deviation alarms.
            NonExclusiveDeviationAlarmState devAlarm = node as NonExclusiveDeviationAlarmState;

            if (devAlarm != null)
            {
                // TODO : public PropertyState<NodeId> SetpointNode
            }

            // handle exclusive deviation alarms.
            ExclusiveDeviationAlarmState edevAlarm = node as ExclusiveDeviationAlarmState;

            if (devAlarm != null)
            {
                // TODO : public PropertyState<NodeId> SetpointNode
            }

            // handle trip alarms.
            TripAlarmState tripAlarm = node as TripAlarmState;
            if (tripAlarm != null)
            {
                // TODO : PropertyState<NodeId> NormalState
            }

            if (node.EnabledState != null && node.EnabledState.TransitionTime != null)
                node.EnabledState.TransitionTime.Value = alarm.EnableTime;
            if (node.AckedState != null && node.AckedState.TransitionTime != null)
                node.AckedState.TransitionTime.Value = alarm.AcknowledgeTime;
            if (node.ConfirmedState != null && node.ConfirmedState.TransitionTime != null)
                node.ConfirmedState.TransitionTime.Value = alarm.ConfirmTime;
            if (node.ActiveState != null && node.ActiveState.TransitionTime != null)
                node.ActiveState.TransitionTime.Value = alarm.ActiveTime;
            if (node.ActiveState != null && node.ActiveState.EffectiveTransitionTime != null)
                node.ActiveState.EffectiveTransitionTime.Value = alarm.ChangeStateTime;

            if (node.BranchId == null || node.BranchId.Value == null || NodeId.IsNull(node.BranchId.Value))
                OnAlarmStateChanged(alarm.nodeId, alarm.state, alarm.Severity == 0, alarm.BeepEnabled, alarm.SupportAck, alarm.SupportReset);
        }

        /// <summary>
        /// Creates a new dialog condition
        /// </summary>
        private DialogConditionState CreateDialog(string dialogName)
        {
            DialogConditionState node = new DialogConditionState(this);

            node.SymbolicName = dialogName;

            // specify optional fields.
            node.EnabledState = new TwoStateVariableState(node);
            node.EnabledState.TransitionTime = new PropertyState<DateTime>(node.EnabledState);
            node.EnabledState.EffectiveDisplayName = new PropertyState<LocalizedText>(node.EnabledState);
            node.EnabledState.Create(context, null, BrowseNames.EnabledState, null, false);

#if NET_STANDARD
            node.LocalTime = new PropertyState<TimeZoneDataType>(node);
#endif

            // specify reference type between the source and the alarm.
            node.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // This call initializes the condition from the type model (i.e. creates all of the objects
            // and variables requried to store its state). The information about the type model was 
            // incorporated into the class when the class was created.
            node.Create(
                context,
                null,
                new QualifiedName(dialogName, BrowseName.NamespaceIndex),
                null,
                true);

            AddChild(node);

            // initialize event information.
            node.EventId.Value = Guid.NewGuid().ToByteArray();
            node.EventType.Value = node.TypeDefinitionId;
            node.SourceNode.Value = NodeId;
            node.SourceName.Value = sourcePath;
            node.ConditionName.Value = node.SymbolicName;
            node.Time.Value = DateTime.UtcNow;
            node.ReceiveTime.Value = node.Time.Value;
            node.LocalTime.Value = Utils.GetTimeZoneInfo();
            node.Message.Value = new LocalizedText(Properties.Resources.DialogMessageActivated, string.Empty, Properties.Resources.DialogMessageActivated);
            node.Retain.Value = true;

            node.SetEnableState(context, true);
            node.SetSeverity(context, EventSeverity.Low);

            // initialize the dialog information.
            node.Prompt.Value = Properties.Resources.DialogPromptMessage;
            node.ResponseOptionSet.Value = s_ResponseOptions;
            node.DefaultResponse.Value = 2;
            node.CancelResponse.Value = 2;
            node.OkResponse.Value = 0;

            // set up method handlers.
            node.OnRespond = OnRespond;

            // this flag needs to be set because the underlying system does not produce these events.
            node.AutoReportStateChanges = true;

            // activate the dialog.
            node.Activate(context);

            // prevent to show dialog message on refresh command
            node.Retain.Value = false;

            // return the new node.
            return node;
        }

        /// <summary>
        /// The responses used with the dialog condition.
        /// </summary>
        private LocalizedText[] s_ResponseOptions = new LocalizedText[]
        {
            new LocalizedText("Online", string.Empty, "Online"),
            new LocalizedText("Offline", string.Empty, "Offline"),
            new LocalizedText("No Change", string.Empty, "No Change")
        };


        /// <summary>
        /// Called when the alarm is enabled or disabled.
        /// </summary>
        private ServiceResult OnEnableDisableAlarm(
            ISystemContext context,
            ConditionState condition,
            bool enabling)
        {
            var alarmStatus = condition.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;

            if (!CanUserWriteNodeState(context, condition))
                return StatusCodes.BadUserAccessDenied;

            var list = alarmStatus.EnableAlarm(enabling);

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : enabling ? StatusCodes.BadConditionAlreadyEnabled : StatusCodes.BadConditionAlreadyDisabled;
        }

        /// <summary>
        /// Called when the alarm has a comment added.
        /// </summary>
        private ServiceResult OnAddComment(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            AlarmConditionState alarm = FindAlarmByEventId(eventId);
            if (alarm == null)
                return StatusCodes.BadEventIdUnknown;
            var alarmStatus = alarm.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;

            if (!CanUserWriteNodeState(context, condition))
                return StatusCodes.BadUserAccessDenied;

            var list = alarmStatus.CommentAlarm(GetRecordNumber(alarm), comment, GetUserName(context));

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : StatusCodes.BadNotSupported;
        }

        /// <summary>
        /// Called when the alarm is acknowledged.
        /// </summary>
        private ServiceResult OnAcknowledge(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            AlarmConditionState alarm = FindAlarmByEventId(eventId);
            if (alarm == null)
                return StatusCodes.BadEventIdUnknown;
            var alarmStatus = alarm.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;
            if (!alarmStatus.SupportAck)
                return StatusCodes.BadEventNotAcknowledgeable;

            if (!CanUserWriteNodeState(context, condition))
                return StatusCodes.BadUserAccessDenied;

            var list = alarmStatus.AcknowledgeAlarm(GetRecordNumber(alarm), comment, GetUserName(context));

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : StatusCodes.BadConditionBranchAlreadyAcked;
        }

        /// <summary>
        /// Called when the alarm is confirmed.
        /// </summary>
        private ServiceResult OnConfirm(
            ISystemContext context,
            ConditionState condition,
            byte[] eventId,
            LocalizedText comment)
        {
            AlarmConditionState alarm = FindAlarmByEventId(eventId);
            if (alarm == null)
                return StatusCodes.BadEventIdUnknown;
            var alarmStatus = alarm.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;
            if (!alarmStatus.SupportReset)
                return StatusCodes.BadEventNotAcknowledgeable;

            if (!CanUserWriteNodeState(context, condition))
                return StatusCodes.BadUserAccessDenied;

            var list = alarmStatus.ConfirmAlarm(GetRecordNumber(alarm), comment, GetUserName(context));

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : StatusCodes.BadConditionBranchAlreadyConfirmed;
        }

        /// <summary>
        /// Called when the alarm is shelved.
        /// </summary>
        private ServiceResult OnShelve(
            ISystemContext context,
            AlarmConditionState alarm,
            bool shelving,
            bool oneShot,
            double shelvingTime)
        {
            var alarmStatus = alarm.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;

            if (!CanUserWriteNodeState(context, alarm))
                return StatusCodes.BadUserAccessDenied;

            alarm.SetShelvingState(context, shelving, oneShot, shelvingTime);
            alarmStatus.TimeOnShelf = shelvingTime;

            var list = alarmStatus.ShelveAlarm(shelving);

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : StatusCodes.BadConditionAlreadyShelved;
        }

        /// <summary>
        /// Called when the alarm is shelved.
        /// </summary>
        private ServiceResult OnTimedUnshelve(
            ISystemContext context,
            AlarmConditionState alarm)
        {
            var alarmStatus = alarm.Handle as AlarmStatus;
            if (alarmStatus == null)
                return StatusCodes.BadEventIdUnknown;

            if (!CanUserWriteNodeState(context, alarm))
                return StatusCodes.BadUserAccessDenied;

            // update the alarm state and produce and event.
            alarm.SetShelvingState(context, false, false, 0);
            alarmStatus.TimeOnShelf = 0;

            var list = alarmStatus.TimeUnshelveAlarm();

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0 ? ServiceResult.Good : StatusCodes.BadConditionNotShelved;
        }

        /// <summary>
        /// Called when the dialog receives a response.
        /// </summary>
        private ServiceResult OnRespond(
            ISystemContext context,
            DialogConditionState dialog,
            int selectedResponse)
        {
            lock (lockObject)
            {
                foreach (AlarmConditionState alarm in alarms.Values)
                {
                    var alarmStatus = alarm.Handle as AlarmStatus;
                    if (alarmStatus == null)
                        return StatusCodes.BadEventIdUnknown;

                    // response 0 means set the source online.
                    if (selectedResponse == 0)
                    {
                        alarmStatus.SetOfflineState(false);
                    }

                    // response 1 means set the source offine.
                    if (selectedResponse == 1)
                    {
                        alarmStatus.SetOfflineState(true);
                    }

                    // other responses mean do nothing.
                    dialog.SetResponse(context, selectedResponse);

                    // dialog no longer interesting once it is deactivated.
                    dialog.Message.Value = new LocalizedText(Properties.Resources.DialogMessageDeactivated, string.Empty, Properties.Resources.DialogMessageDeactivated);
                    dialog.Retain.Value = false;
                }
            }

            return ServiceResult.Good;
        }

        /// <summary>
        /// Reports the changes to the alarm.
        /// </summary>
        private void ReportChanges(IList<AlarmConditionState> alarms)
        {
            //lock (lockObject)
            {
                if (disposed)
                    return;

                foreach (AlarmConditionState alarm in alarms)
                {
                    // report changes to node attributes.
                    alarm.ClearChangeMasks(context, true);

                    // check if events are being monitored for the source.
                    if (AreEventsMonitored)
                    {
                        // create a snapshot.
                        InstanceStateSnapshot e = new InstanceStateSnapshot();
                        e.Initialize(context, alarm);

                        // report the event.
                        ReportEvent(context, e);
                    }
                }
            }
        }

        /// <summary>
        /// Finds the alarm by event id.
        /// </summary>
        /// <param name="eventId">The event id.</param>
        /// <returns>The alarm. Null if not found.</returns>
        private AlarmConditionState FindAlarmByEventId(byte[] eventId)
        {
            if (eventId == null)
            {
                return null;
            }

            AlarmConditionState alarm = null;
            lock (lockObject)
            {
                if (!events.TryGetValue(Utils.ToHexString(eventId), out alarm))
                {
                    return null;
                }
            }

            return alarm;
        }

        /// <summary>
        /// Gets the record number associated with tge alarm.
        /// </summary>
        /// <param name="alarm">The alarm.</param>
        /// <returns>The record number; 0 if the alarm is not an archived alarm.</returns>
        private NodeId GetRecordNumber(AlarmConditionState alarm)
        {
            if (alarm == null || alarm.BranchId == null || alarm.BranchId.Value == null)
            {
                return NodeId.Null;
            }

            return alarm.BranchId.Value;
        }
        /// <summary>
        /// Gets the user name associated with the context.
        /// </summary>
        private static String GetUserName(ISystemContext context)
        {
            if (context.UserIdentity != null)
            {
                return context.UserIdentity.DisplayName;
            }

            return null;
        }

        private bool IsDeviationAlarm(AlarmStatus settings)
        {
            return (settings.type == UFUAModel.AlarmType.ExclusiveDeviation || settings.type == UFUAModel.AlarmType.NonExclusiveDeviation);
        }

        private bool IsRateOfChangeAlarm(AlarmStatus settings)
        {
            return (settings.type == UFUAModel.AlarmType.ExclusiveRateOfChange || settings.type == UFUAModel.AlarmType.NonExclusiveRateOfChange);
        }

        private bool IsHysteresisSupported(AlarmStatus settings)
        {
            return (settings.type == UFUAModel.AlarmType.ExclusiveDeviation || settings.type == UFUAModel.AlarmType.NonExclusiveDeviation ||
                    settings.type == UFUAModel.AlarmType.ExclusiveRateOfChange || settings.type == UFUAModel.AlarmType.NonExclusiveRateOfChange) &&
                    (settings.ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfRange || settings.instrumentRange != null) &&
                    (settings.ExceptionDeviationFormat != ExceptionDeviationFormat.PercentOfEURange || settings.range != null);
        }

        private bool IsAlarmStateChanged(AlarmStatus settings)
        {
            return ((settings.state & AlarmState.Active) != (settings.lastState & AlarmState.Active)) ||
                    ((settings.state & AlarmState.High) != (settings.lastState & AlarmState.High)) ||
                    ((settings.state & AlarmState.HighHigh) != (settings.lastState & AlarmState.HighHigh)) ||
                    ((settings.state & AlarmState.Low) != (settings.lastState & AlarmState.Low)) ||
                    ((settings.state & AlarmState.LowLow) != (settings.lastState & AlarmState.LowLow));
        }

        private bool CheckHysteresis(AlarmStatus settings, double doubleValue, double doublePrevValue, double deadband)
        {
            if (!IsHysteresisSupported(settings))
                return false;

            var deadBand = new Utilities.Maths.Deadband(doublePrevValue, doubleValue , deadband) { TreatEqualLikeFalse = true };
            switch (settings.ExceptionDeviationFormat)
            {
                case ExceptionDeviationFormat.AbsoluteValue:
                    return deadBand.IsExceeded();
                case ExceptionDeviationFormat.PercentOfRange:
                    return deadBand.IsExceeded(settings.instrumentRange.High - settings.instrumentRange.Low);
                case ExceptionDeviationFormat.PercentOfValue:
                    return deadBand.IsExceeded(doublePrevValue);
                case ExceptionDeviationFormat.PercentOfEURange:
                    return deadBand.IsExceeded(settings.range.High - settings.range.Low);
                default:
                    return false;
            }
        }

        #endregion

        #region Public Methods

        public AlarmConditionState GetOrCreateAlarmConditionState(AlarmStatus AlarmStatus, NodeId branchId = null, string conditionaName = null)
        {
            AlarmConditionState alarm = null;
            lock (lockObject)
            {
                if (branchId == null && alarms.TryGetValue(AlarmStatus.nodeId, out alarm))
                    return alarm;
                else if (branchId != null && branches.TryGetValue(AlarmStatus.nodeId, out alarm))
                    return alarm;
            }

            switch (AlarmStatus.type)
            {
                case UFUAModel.AlarmType.TripAlarm:
                    {
                        var alarmState = new TripAlarmState(this);
                        alarmState.NormalState = new PropertyState<NodeId>(alarmState);

                        alarm = alarmState;
                        break;
                    }

                //case UFUAModel.AlarmType.ExclusiveLimit:
                case UFUAModel.AlarmType.ExclusiveLevel:
                    {
                        var alarmState = new ExclusiveLevelAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;

                        alarm = alarmState;
                        break;
                    }

                //case UFUAModel.AlarmType.NonExclusiveLimit:
                case UFUAModel.AlarmType.NonExclusiveLevel:
                    {
                        var alarmState = new NonExclusiveLevelAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighState = new TwoStateVariableState(alarmState);
                        alarmState.HighState = new TwoStateVariableState(alarmState);
                        alarmState.LowState = new TwoStateVariableState(alarmState);
                        alarmState.LowLowState = new TwoStateVariableState(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;

                        alarm = alarmState;
                        break;
                    }
                case UFUAModel.AlarmType.ExclusiveDeviation:
                    {
                        var alarmState = new ExclusiveDeviationAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.None;

                        //alarmState.SetpointNode = new PropertyState<NodeId>(alarmState);

                        alarm = alarmState;
                        break;
                    }

                case UFUAModel.AlarmType.NonExclusiveDeviation:
                    {
                        var alarmState = new NonExclusiveDeviationAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighState = new TwoStateVariableState(alarmState);
                        alarmState.HighState = new TwoStateVariableState(alarmState);
                        alarmState.LowState = new TwoStateVariableState(alarmState);
                        alarmState.LowLowState = new TwoStateVariableState(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;

                        //alarmState.SetpointNode = new PropertyState<NodeId>(alarmState);

                        alarm = alarmState;
                        break;
                    }

                case UFUAModel.AlarmType.ExclusiveRateOfChange:
                    {
                        var alarmState = new ExclusiveRateOfChangeAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;

                        alarm = alarmState;
                        break;
                    }

                case UFUAModel.AlarmType.NonExclusiveRateOfChange:
                    {
                        var alarmState = new NonExclusiveRateOfChangeAlarmState(this);
                        alarmState.HighHighLimit = new PropertyState<double>(alarmState);
                        alarmState.HighLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLimit = new PropertyState<double>(alarmState);
                        alarmState.LowLowLimit = new PropertyState<double>(alarmState);

                        alarmState.HighHighState = new TwoStateVariableState(alarmState);
                        alarmState.HighState = new TwoStateVariableState(alarmState);
                        alarmState.LowState = new TwoStateVariableState(alarmState);
                        alarmState.LowLowState = new TwoStateVariableState(alarmState);

                        alarmState.HighHighLimit.AccessLevel = AlarmStatus.Limits[0].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.HighLimit.AccessLevel = AlarmStatus.Limits[1].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLimit.AccessLevel = AlarmStatus.Limits[2].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;
                        alarmState.LowLowLimit.AccessLevel = AlarmStatus.Limits[3].HasValue ? AccessLevels.CurrentReadOrWrite : AccessLevels.CurrentWrite;

                        alarm = alarmState;
                        break;
                    }

                default:
                    {
                        alarm = new AlarmConditionState(this);
                        break;
                    }
            }

            alarm.SymbolicName = AlarmStatus.Name;

            // add optional components.
            alarm.ClientUserId = new PropertyState<String>(alarm);

            alarm.AckedState = new TwoStateVariableState(alarm);
            if (AlarmStatus.CreateAlrMemberAckedState)
            {
                alarm.AckedState.TransitionTime = new PropertyState<DateTime>(alarm.AckedState);
                alarm.AckedState.EffectiveDisplayName = new PropertyState<LocalizedText>(alarm.AckedState);
                alarm.AckedState.Create(context, null, BrowseNames.AckedState, null, false);
            }


            if(AlarmStatus.CreateAlrMemberConfirmedState)
            {
                alarm.ConfirmedState = new TwoStateVariableState(alarm);
                alarm.ConfirmedState.TransitionTime = new PropertyState<DateTime>(alarm.ConfirmedState);
                alarm.ConfirmedState.EffectiveDisplayName = new PropertyState<LocalizedText>(alarm.ConfirmedState);
                alarm.ConfirmedState.Create(context, null, BrowseNames.ConfirmedState, null, false);

            }

            alarm.Comment = new ConditionVariableState<LocalizedText>(alarm);
            alarm.Acknowledge = new AddCommentMethodState(alarm);
            if(AlarmStatus.CreateAlrMemberConfirm)
                alarm.Confirm = new AddCommentMethodState(alarm);
            alarm.AddComment = new AddCommentMethodState(alarm);

            if (NodeId.IsNull(branchId))
            {
                if(AlarmStatus.CreateAlrMemberSuppressedState)
                {
                    alarm.SuppressedState = new TwoStateVariableState(alarm);
                    alarm.SuppressedState.TransitionTime = new PropertyState<DateTime>(alarm.SuppressedState);
                    alarm.SuppressedState.EffectiveDisplayName = new PropertyState<LocalizedText>(alarm.SuppressedState);
                    alarm.SuppressedState.Create(context, null, BrowseNames.SuppressedState, null, false);
                }

                if(AlarmStatus.CreateAlrMemberShelvingState)
                {
                    alarm.ShelvingState = new ShelvedStateMachineState(alarm);
                    alarm.ShelvingState.LastTransition = new FiniteTransitionVariableState(alarm.ShelvingState);
                    alarm.ShelvingState.LastTransition.TransitionTime = new PropertyState<DateTime>(alarm.ShelvingState);
                }
            }

            // adding optional components to children is a little more complicated since the 
            // necessary initilization strings defined by the class that represents the child.
            // in this case we pre-create the child, add the optional components
            // and call create without assigning NodeIds. The NodeIds will be assigned when the
            // parent object is created.
            alarm.EnabledState = new TwoStateVariableState(alarm);
            if (AlarmStatus.CreateAlrMemberEnabledState)
            {
                alarm.EnabledState.TransitionTime = new PropertyState<DateTime>(alarm.EnabledState);
                alarm.EnabledState.EffectiveDisplayName = new PropertyState<LocalizedText>(alarm.EnabledState);
                alarm.EnabledState.Create(context, null, BrowseNames.EnabledState, null, false);
            }

            // same procedure add optional components to the ActiveState component.
            alarm.ActiveState = new TwoStateVariableState(alarm);
            if (AlarmStatus.CreateAlrMemberActiveState)
            {
                alarm.ActiveState.TransitionTime = new PropertyState<DateTime>(alarm.ActiveState);
                alarm.ActiveState.EffectiveTransitionTime = new PropertyState<DateTime>(alarm.ActiveState);
                alarm.ActiveState.EffectiveDisplayName = new PropertyState<LocalizedText>(alarm.ActiveState);
                alarm.ActiveState.Create(context, null, BrowseNames.ActiveState, null, false);
            }

            // adding Quality mandatory component
            alarm.Quality = new ConditionVariableState<StatusCode>(alarm);
            if (AlarmStatus.CreateAlrMemberQuality)
            {
                alarm.Quality.StatusCode = new StatusCode(StatusCodes.Good);
                alarm.Quality.Create(context, null, BrowseNames.Quality, null, false);
            }

#if NET_STANDARD
            if(AlarmStatus.CreateAlrMemberLocalTime)
                alarm.LocalTime = new PropertyState<TimeZoneDataType>(alarm);
#endif

            // specify reference type between the source and the alarm.
            alarm.ReferenceTypeId = ReferenceTypeIds.HasComponent;

            // This call initializes the condition from the type model (i.e. creates all of the objects
            // and variables requried to store its state). The information about the type model was 
            // incorporated into the class when the class was created.
            //
            // This method also assigns new NodeIds to all of the components by calling the INodeIdFactory.New
            // method on the INodeIdFactory object which is part of the system context. The NodeManager provides
            // the INodeIdFactory implementation used here.
            //alarm.Create(
            //    context,
            //    NodeId.IsNull(branchId) ? null : new NodeId(Guid.NewGuid()),
            //    new QualifiedName(AlarmStatus.name, BrowseName.NamespaceIndex),
            //    null,
            //    NodeId.IsNull(branchId));

            alarm.Create(
                context,
                null,
                new QualifiedName(AlarmStatus.Name, BrowseName.NamespaceIndex),
                null,
                true);

            // don't add branches to the address space.
            if (NodeId.IsNull(branchId))
            {
                AddChild(alarm);
            }

            // initialize event information.node
            alarm.EventType.Value = alarm.TypeDefinitionId;
            alarm.SourceNode.Value = NodeId;
            alarm.SourceName.Value = sourcePath;
            alarm.ConditionName.Value = conditionaName ?? alarm.SymbolicName;
            alarm.Time.Value = DateTime.UtcNow;
            alarm.ReceiveTime.Value = alarm.Time.Value;
            alarm.LocalTime.Value = Utils.GetTimeZoneInfo();
            alarm.BranchId.Value = branchId;

            // set up method handlers.
            alarm.OnEnableDisable = OnEnableDisableAlarm;
            alarm.OnAcknowledge = OnAcknowledge;
            alarm.OnAddComment = OnAddComment;
            alarm.OnConfirm = OnConfirm;
            alarm.OnShelve = OnShelve;
            alarm.OnTimedUnshelve = OnTimedUnshelve;
            alarm.Handle = AlarmStatus;

            lock (lockObject)
            {
                if (branchId == null)
                    alarms[AlarmStatus.nodeId] = alarm;
                else
                    branches[AlarmStatus.nodeId] = alarm;
            }

            return alarm;
        }

        public bool EnableAlarmStatus(AlarmStatus settings, DataValue value)
        {
            if (value.Value == null)
                return false;

            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (TypeInfo.IsNumericType(value.WrappedValue.TypeInfo.BuiltInType) || value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                {
                    var enabling = Convert.ToDouble(value.Value) > 0.0;
                    list.AddRange(settings.EnableAlarm(enabling));
                }
            }

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0;
        }

        /// <summary>
        /// Update Alarm Severity in case the Data Value is a valid parameter
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="value"></param>
        /// <returns>True in case one or more alarm status are updated</returns>
        public bool UpdateAlarmSeverity(AlarmStatus settings, DataValue value)
        {
            if (value.Value == null)
                return false;

            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (TypeInfo.IsNumericType(value.WrappedValue.TypeInfo.BuiltInType) || value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                    list.AddRange(settings.UpdateSeverity((EventSeverity)Convert.ToInt16(value.Value)));

            }

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0;
        }

        public bool UpdateAlarmStatus(AlarmStatus settings, DataValue value)
        {
            return UpdateAlarmStatus(settings, value, false);
        }


        bool UpdateAlarmStatus(AlarmStatus settings, DataValue value, bool bDelayed)
        {
            var list = new List<AlarmStatus>();
            lock (lockObject)
            {
                bool bRateOfChange = false;
                if (mapNodeToRateOfChangeTimer.ContainsKey(settings.nodeId))
                {
                    bRateOfChange = true;
                    mapNodeToRateOfChangeTimer[settings.nodeId].Dispose();
                    mapNodeToRateOfChangeTimer.Remove(settings.nodeId);
                }

                if ((settings.state & AlarmState.Enabled) == 0)
                    return false;

                var quality = StatusCodes.Good;
                if (StatusCode.IsNotGood(value.StatusCode))
                    quality = value.StatusCode.Code;
                else if (settings.ActivationLowValueQuality.HasValue && StatusCode.IsNotGood(settings.ActivationLowValueQuality.Value))
                    quality = settings.ActivationLowValueQuality.Value;
                else if (settings.ActivationValueQuality.HasValue && StatusCode.IsNotGood(settings.ActivationValueQuality.Value))
                    quality = settings.ActivationValueQuality.Value;
                else if (settings.LimitsQuality[0].HasValue && StatusCode.IsNotGood(settings.LimitsQuality[0].Value))
                    quality = settings.LimitsQuality[0].Value;
                else if (settings.LimitsQuality[1].HasValue && StatusCode.IsNotGood(settings.LimitsQuality[1].Value))
                    quality = settings.LimitsQuality[1].Value;
                else if (settings.LimitsQuality[2].HasValue && StatusCode.IsNotGood(settings.LimitsQuality[2].Value))
                    quality = settings.LimitsQuality[2].Value;
                else if (settings.LimitsQuality[3].HasValue && StatusCode.IsNotGood(settings.LimitsQuality[3].Value))
                    quality = settings.LimitsQuality[3].Value;
                // check for quality in order to change the alarm state to undefined with bad quality
                if (settings.lastQuality != quality && (!settings.QualityGoodOnly || StatusCode.IsGood(quality)))
                {
                    if (settings.state != AlarmStatus.initialState && (settings.state & AlarmState.Deleted) == 0)
                    {
                        if (settings.SaveBranches)
                        {
                            var branchstatus = settings.CreateArchiveSnapshot();
                            branchstatus.Time = DateTime.UtcNow;
                            branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedQuality, branchstatus.nodeId);
                            branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                            branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                            list.Add(branchstatus);
                            list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                        }

                        settings.lastQuality = quality;
                        settings.lastTimeUpdated = settings.Time = DateTime.UtcNow;
                        list.Add(settings.CreateSnapshot(false));
                    }
                    else
                        settings.lastQuality = quality;
                }

                if (value.Value != null && value.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                {
                    if (settings.isTotalTimeOn && value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.String ||
                        TypeInfo.IsNumericType(value.WrappedValue.TypeInfo.BuiltInType) || value.WrappedValue.TypeInfo.BuiltInType == BuiltInType.Boolean)
                    {
                        double doubleValue = 0.0;
                        if (settings.isTotalTimeOn)
                        {
                            TimeSpan timeSpan;
                            if (TimeSpan.TryParse(value.Value as String, out timeSpan))
                                doubleValue = timeSpan.TotalSeconds;
                        }
                        else
                            doubleValue = Convert.ToDouble(value.Value);

                        //bool elapsedRateOfChangeTimer = (IsRateOfChangeAlarm(settings) && settings.lastTimeStateChanged + TimeSpan.FromMilliseconds(settings.TimeUnit) <= DateTime.UtcNow);
                        //if (!IsRateOfChangeAlarm(settings) || elapsedRateOfChangeTimer)
                        {
                            // Check the value only if the alarm was set for ingnoring the quality code or quality code is good.
                            if (!settings.QualityGoodOnly || StatusCode.IsGood(quality))
                            {
                                bool bStateChanged = false;
                                bool bActiveChanged = false;
                                bool bActive = (settings.state & AlarmState.Active) != 0;
                                switch (settings.type)
                                {
                                    //case UFUAModel.AlarmType.ExclusiveLimit:
                                    //case UFUAModel.AlarmType.NonExclusiveLimit:
                                    case UFUAModel.AlarmType.ExclusiveLevel:
                                    case UFUAModel.AlarmType.NonExclusiveLevel:
                                        {
                                            bool bHighHigh = false;
                                            bool bHigh = false;
                                            bool bLow = false;
                                            bool bLowLow = false;
                                            if (settings.Limits[0].HasValue && doubleValue > settings.Limits[0]) // HighHighLimit
                                            {
                                                bActive = true;
                                                bHighHigh = true;
                                                bHigh = settings.type != UFUAModel.AlarmType.ExclusiveLevel;
                                            }
                                            else if (settings.Limits[1].HasValue && doubleValue > settings.Limits[1]) // HighLimit
                                            {
                                                bActive = true;
                                                bHigh = true;
                                            }
                                            else if (settings.Limits[3].HasValue && doubleValue < settings.Limits[3]) // LowLowLimit
                                            {
                                                bActive = true;
                                                bLow = settings.type != UFUAModel.AlarmType.ExclusiveLevel;
                                                bLowLow = true;
                                            }
                                            else if (settings.Limits[2].HasValue && doubleValue < settings.Limits[2]) // LowLimit
                                            {
                                                bActive = true;
                                                bLow = true;
                                            }
                                            else
                                            {
                                                bActive = false;
                                            }

                                            if (!bDelayed && (settings.DelayTimeOn > 0.0 || settings.DelayTimeOff > 0.0))
                                            {
                                                bool isActive = (settings.state & AlarmState.Active) != 0;
                                                if (isActive != bActive)
                                                {
                                                    if (bActive && settings.DelayTimeOn > 0.0 || !bActive && settings.DelayTimeOff > 0.0)
                                                    {
                                                        if (!mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                        {
                                                            bActiveChanged = true;
                                                            break;
                                                        }

                                                        return false;
                                                    }
                                                }
                                                else if (isActive && settings.DelayTimeOff > 0.0 || !isActive && settings.DelayTimeOn > 0.0)
                                                {
                                                    if (mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                    {
                                                        mapNodeToDelayTimer[settings.nodeId].Dispose();
                                                        mapNodeToDelayTimer.Remove(settings.nodeId);
                                                    }
                                                }
                                            }

                                            bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, bActive && bHighHigh);
                                            bStateChanged |= settings.SetStateBits(AlarmState.High, bActive && bHigh);
                                            bStateChanged |= settings.SetStateBits(AlarmState.Low, bActive && bLow);
                                            bStateChanged |= settings.SetStateBits(AlarmState.LowLow, bActive && bLowLow);

                                            // Check for adding branch alarm to update list.
                                            if (settings.SaveBranches && !bActive && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedStatus, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }
                                            else if (settings.SaveBranches && bStateChanged && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.state = settings.lastState;
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedState, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }

                                            if (settings.lastValue != doubleValue)
                                            {
                                                settings.lastValue = doubleValue;
                                                settings.lastTimeUpdated = DateTime.UtcNow;
                                            }

                                            // Check for adding snapshot alarm to update list.
                                            if (settings.SetStateBits(AlarmState.Active, bActive))
                                            {
                                                bActiveChanged = true;
                                                settings.Time = DateTime.UtcNow;
                                                settings.TagAliasShowValues = settings.TagAliasLastValues;
                                                var activeTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.ChangeStateTime = activeTime;
                                                var activeDuration = !bActive ? activeTime - settings.ActiveTime : TimeSpan.Zero;
                                                if (bActive)
                                                {
                                                    settings.ActiveTime = activeTime;
                                                    settings.SetStateBits(AlarmState.Deleted, false);
                                                    settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                    settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                }
                                                else if (AlarmStatus.CanRemove(settings))
                                                    settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(true, activeDuration));
                                            }
                                            else if (bStateChanged)
                                            {
                                                settings.Time = DateTime.UtcNow;
                                                settings.ChangeStateTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.SetStateBits(AlarmState.Deleted, false);
                                                settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                list.Add(settings.CreateSnapshot(true));
                                            }
                                            else if (!bActive && AlarmStatus.CanRemove(settings))
                                            {
                                                settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(false));
                                            }

                                            break;
                                        }
                                    case UFUAModel.AlarmType.ExclusiveDeviation:
                                    case UFUAModel.AlarmType.NonExclusiveDeviation:
                                        {
                                            bool bHighHigh = false;
                                            bool bHigh = false;
                                            bool bLow = false;
                                            bool bLowLow = false;
                                            if (settings.Limits[0].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[0].Value))               // HighHighLimit
                                            {
                                                bActive = true;
                                                bHighHigh = true;
                                                bHigh = settings.type != UFUAModel.AlarmType.ExclusiveDeviation;
                                            }
                                            else if (settings.Limits[1].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[1].Value))          // HighLimit
                                            {
                                                bActive = true;
                                                bHigh = true;
                                            }
                                            else if (settings.Limits[3].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[3].Value))         // LowLowLimit
                                            {
                                                bActive = true;
                                                bLow = settings.type != UFUAModel.AlarmType.ExclusiveDeviation;
                                                bLowLow = true;
                                            }
                                            else if (settings.Limits[2].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[2].Value))         // LowLimit
                                            {
                                                bActive = true;
                                                bLow = true;
                                            }
                                            else
                                            {
                                                bActive = false;
                                            }

                                            if (!bDelayed && (settings.DelayTimeOn > 0.0 || settings.DelayTimeOff > 0.0))
                                            {
                                                bool isActive = (settings.state & AlarmState.Active) != 0;
                                                if (isActive != bActive)
                                                {
                                                    if (bActive && settings.DelayTimeOn > 0.0 || !bActive && settings.DelayTimeOff > 0.0)
                                                    {
                                                        if (!mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                        {
                                                            bActiveChanged = true;
                                                            break;
                                                        }

                                                        return false;
                                                    }
                                                }
                                                else if (isActive && settings.DelayTimeOff > 0.0 || !isActive && settings.DelayTimeOn > 0.0)
                                                {
                                                    if (mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                    {
                                                        mapNodeToDelayTimer[settings.nodeId].Dispose();
                                                        mapNodeToDelayTimer.Remove(settings.nodeId);
                                                    }
                                                }
                                            }

                                            bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, bActive && bHighHigh);
                                            bStateChanged |= settings.SetStateBits(AlarmState.High, bActive && bHigh);
                                            bStateChanged |= settings.SetStateBits(AlarmState.Low, bActive && bLow);
                                            bStateChanged |= settings.SetStateBits(AlarmState.LowLow, bActive && bLowLow);


                                            // Check for adding branch alarm to update list.
                                            if (settings.SaveBranches && !bActive && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedStatus, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }
                                            else if (settings.SaveBranches && bStateChanged && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.state = settings.lastState;
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedState, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }

                                            if (settings.lastValue != doubleValue)
                                            {
                                                settings.lastValue = doubleValue;
                                                settings.lastTimeUpdated = DateTime.UtcNow;
                                            }

                                            // Check for adding snapshot alarm to update list.
                                            if (settings.SetStateBits(AlarmState.Active, bActive))
                                            {
                                                bActiveChanged = true;
                                                settings.Time = DateTime.UtcNow;
                                                settings.TagAliasShowValues = settings.TagAliasLastValues;
                                                var activeTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.ChangeStateTime = activeTime;
                                                var activeDuration = !bActive ? activeTime - settings.ActiveTime : TimeSpan.Zero;
                                                if (bActive)
                                                {
                                                    settings.ActiveTime = activeTime;
                                                    settings.SetStateBits(AlarmState.Deleted, false);
                                                    settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                    settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                }
                                                else if (AlarmStatus.CanRemove(settings))
                                                    settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(true, activeDuration));
                                            }
                                            else if (bStateChanged)
                                            {
                                                settings.Time = DateTime.UtcNow;
                                                settings.ChangeStateTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.SetStateBits(AlarmState.Deleted, false);
                                                settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                list.Add(settings.CreateSnapshot(true));
                                            }
                                            else if (!bActive && AlarmStatus.CanRemove(settings))
                                            {
                                                settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(false));
                                            }

                                            break;
                                        }
                                    case UFUAModel.AlarmType.ExclusiveRateOfChange:
                                    case UFUAModel.AlarmType.NonExclusiveRateOfChange:
                                        {
                                            if (settings.Limits[0].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[0].Value))                 // HighHighLimit
                                            {
                                                bActive = true;
                                                bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, true);
                                                bStateChanged |= settings.SetStateBits(AlarmState.High, settings.type != UFUAModel.AlarmType.ExclusiveRateOfChange);
                                                bStateChanged |= settings.SetStateBits(AlarmState.Low, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.LowLow, false);
                                            }
                                            else if (settings.Limits[1].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[1].Value))            // HighLimit
                                            {
                                                bActive = true;
                                                bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.High, true);
                                                bStateChanged |= settings.SetStateBits(AlarmState.Low, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.LowLow, false);
                                            }
                                            else if (settings.Limits[3].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[3].Value))           // LowLowLimit
                                            {
                                                bActive = true;
                                                bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.High, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.Low, settings.type != UFUAModel.AlarmType.ExclusiveRateOfChange);
                                                bStateChanged |= settings.SetStateBits(AlarmState.LowLow, true);
                                            }
                                            else if (settings.Limits[2].HasValue && CheckHysteresis(settings, doubleValue, settings.lastValue, settings.Limits[2].Value))           // LowLimit
                                            {
                                                bActive = true;
                                                bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.High, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.Low, true);
                                                bStateChanged |= settings.SetStateBits(AlarmState.LowLow, false);
                                            }
                                            else if ((settings.state & AlarmState.Acknowledged) != 0)
                                            {
                                                bActive = false;
                                                bStateChanged |= settings.SetStateBits(AlarmState.HighHigh, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.High, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.Low, false);
                                                bStateChanged |= settings.SetStateBits(AlarmState.LowLow, false);
                                            }

                                            // Check for adding branch alarm to update list.
                                            if (settings.SaveBranches && !bActive && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedStatus, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }
                                            else if (settings.SaveBranches && bStateChanged && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.state = settings.lastState;
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedState, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }

                                            // Check for adding snapshot alarm to update list.
                                            if (settings.SetStateBits(AlarmState.Active, bActive))
                                            {
                                                bActiveChanged = true;
                                                settings.Time = DateTime.UtcNow;
                                                settings.TagAliasShowValues = settings.TagAliasLastValues;
                                                var activeTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.ChangeStateTime = activeTime;
                                                var activeDuration = !bActive ? activeTime - settings.ActiveTime : TimeSpan.Zero;
                                                if (bActive)
                                                {
                                                    settings.ActiveTime = activeTime;
                                                    settings.SetStateBits(AlarmState.Deleted, false);
                                                    settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                    settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                }
                                                else if (AlarmStatus.CanRemove(settings))
                                                    settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(true, activeDuration));
                                            }
                                            else if (bStateChanged)
                                            {
                                                settings.Time = DateTime.UtcNow;
                                                settings.ChangeStateTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.SetStateBits(AlarmState.Deleted, false);
                                                settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                list.Add(settings.CreateSnapshot(true));
                                            }
                                            else if (!bActive && AlarmStatus.CanRemove(settings))
                                            {
                                                settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(false));
                                            }

                                            System.Diagnostics.Debug.WriteLine("{0} : RateOfChangeTimer - lastvalue = {1}, currentvalue = {2}", DateTime.UtcNow, settings.lastValue, doubleValue);

                                            if (bStateChanged || (settings.lastTimeStateChanged + TimeSpan.FromMilliseconds(settings.TimeUnit) <= DateTime.UtcNow))
                                            {
                                                settings.lastTimeStateChanged = DateTime.UtcNow;

                                                if (bStateChanged || bRateOfChange)
                                                {
                                                    settings.lastValue = doubleValue;
                                                    settings.lastTimeUpdated = DateTime.UtcNow;
                                                }

                                                if (bActive || !bRateOfChange)
                                                {
                                                    TimeSpan dueTime = TimeSpan.FromMilliseconds(settings.TimeUnit) - (DateTime.UtcNow - settings.lastTimeStateChanged);
                                                    var t = new Timer((o) =>
                                                    {
                                                        if (disposed)
                                                            return;

                                                        UpdateAlarmStatus(settings, value, true);
                                                    },
                                                    settings,
                                                    dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                                    TimeSpan.FromMilliseconds(-1));
                                                    mapNodeToRateOfChangeTimer[settings.nodeId] = t;
                                                }
                                            }
                                            else if (settings.TimeUnit > 0.0)
                                            {
                                                TimeSpan dueTime = TimeSpan.FromMilliseconds(settings.TimeUnit) - (DateTime.UtcNow - settings.lastTimeStateChanged);
                                                var t = new Timer((o) =>
                                                {
                                                    if (disposed)
                                                        return;

                                                    UpdateAlarmStatus(settings, value, true);
                                                },
                                                settings,
                                                dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                                TimeSpan.FromMilliseconds(-1));
                                                mapNodeToRateOfChangeTimer[settings.nodeId] = t;
                                            }
                                            break;
                                        }
                                    case UFUAModel.AlarmType.TripAlarm:
                                        {
                                            switch (settings.TripCondition)
                                            {
                                                case TripCondition.Equals: bActive = (doubleValue == settings.ActivationValue); break;
                                                case TripCondition.GreaterThan: bActive = (doubleValue > settings.ActivationValue); break;
                                                case TripCondition.GreaterThanOrEqual: bActive = (doubleValue >= settings.ActivationValue); break;
                                                case TripCondition.LessThan: bActive = (doubleValue < settings.ActivationValue); break;
                                                case TripCondition.LessThanOrEqual: bActive = (doubleValue <= settings.ActivationValue); break;
                                                case TripCondition.NotEqual: bActive = (doubleValue != settings.ActivationValue); break;
                                                case TripCondition.Between: bActive = (doubleValue >= settings.ActivationLowValue && doubleValue <= settings.ActivationValue); break;
                                                default: bActive = (doubleValue == settings.ActivationValue); break;
                                            }

                                            if (!bDelayed && (settings.DelayTimeOn > 0.0 || settings.DelayTimeOff > 0.0))
                                            {
                                                bool isActive = (settings.state & AlarmState.Active) != 0;
                                                if (isActive != bActive)
                                                {
                                                    if (bActive && settings.DelayTimeOn > 0.0 || !bActive && settings.DelayTimeOff > 0.0)
                                                    {
                                                        if (!mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                        {
                                                            bActiveChanged = true;
                                                            break;
                                                        }

                                                        return false;
                                                    }
                                                }
                                                else if (isActive && settings.DelayTimeOff > 0.0 || !isActive && settings.DelayTimeOn > 0.0)
                                                {
                                                    if (mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                                    {
                                                        mapNodeToDelayTimer[settings.nodeId].Dispose();
                                                        mapNodeToDelayTimer.Remove(settings.nodeId);
                                                    }
                                                }
                                            }

                                            // Check for adding branch alarm to update list.
                                            if (settings.SaveBranches && !bActive && (settings.state & AlarmState.Active) != 0)
                                            {
                                                var branchstatus = settings.CreateArchiveSnapshot();
                                                branchstatus.Time = DateTime.UtcNow;
                                                branchstatus.Comment = String.Format(Properties.Resources.AlarmChangedStatus, branchstatus.nodeId);
                                                branchstatus.SetStateBits(AlarmState.Acknowledged, false);
                                                branchstatus.SetStateBits(AlarmState.Active | AlarmState.Confirmed, true);
                                                list.Add(branchstatus);
                                                list.AddRange(settings.CheckMaxSizeArchiveAndRemove(maxBranches));
                                            }

                                            if (settings.lastValue != doubleValue)
                                            {
                                                settings.lastValue = doubleValue;
                                                settings.lastTimeUpdated = DateTime.UtcNow;
                                            }

                                            // Check for adding snapshot alarm to update list.
                                            if (settings.SetStateBits(AlarmState.Active, bActive))
                                            {
                                                bActiveChanged = true;
                                                settings.Time = DateTime.UtcNow;
                                                settings.TagAliasShowValues = settings.TagAliasLastValues;
                                                var activeTime = settings.UseTimeStamp ? value.SourceTimestamp : settings.Time;
                                                settings.ChangeStateTime = activeTime;
                                                var activeDuration = !bActive ? activeTime - settings.ActiveTime : TimeSpan.Zero;
                                                if (bActive)
                                                {
                                                    settings.ActiveTime = activeTime;
                                                    settings.SetStateBits(AlarmState.Deleted, false);
                                                    settings.SetStateBits(AlarmState.Acknowledged, !settings.SupportAck);
                                                    settings.SetStateBits(AlarmState.Confirmed, settings.SupportAck || !settings.SupportReset);
                                                }
                                                else if (AlarmStatus.CanRemove(settings))
                                                    settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(true, activeDuration));
                                            }
                                            else if (!bActive && AlarmStatus.CanRemove(settings))
                                            {
                                                settings.SetStateBits(AlarmState.Deleted, true);
                                                list.Add(settings.CreateSnapshot(false));
                                            }
                                            break;
                                        }
                                }

                                if (!bDelayed && bActiveChanged && !IsRateOfChangeAlarm(settings))
                                {
                                    if (mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                    {
                                        mapNodeToDelayTimer[settings.nodeId].Dispose();
                                        mapNodeToDelayTimer.Remove(settings.nodeId);
                                    }
                                    else
                                        settings.lastTimeStateChanged = DateTime.UtcNow;

                                    if (settings.DelayTimeOn > 0.0 || settings.DelayTimeOff > 0.0)
                                    {
                                        TimeSpan dueTime = TimeSpan.Zero;
                                        if (bActive && settings.DelayTimeOn > 0.0)
                                            dueTime = TimeSpan.FromMilliseconds(settings.DelayTimeOn) - (DateTime.UtcNow - settings.lastTimeStateChanged);
                                        else if (!bActive && settings.DelayTimeOff > 0.0)
                                            dueTime = TimeSpan.FromMilliseconds(settings.DelayTimeOff) - (DateTime.UtcNow - settings.lastTimeStateChanged);

                                        if (dueTime != TimeSpan.Zero)
                                        {
                                            var t = new Timer((o) =>
                                            {
                                                if (disposed)
                                                    return;

                                                lock (lockObject)
                                                {
                                                    AlarmStatus alarm = o as AlarmStatus;
                                                    if (mapNodeToDelayTimer.ContainsKey(alarm.nodeId))
                                                    {
                                                        mapNodeToDelayTimer[alarm.nodeId].Dispose();
                                                        mapNodeToDelayTimer.Remove(alarm.nodeId);
                                                        UpdateAlarmStatus(alarm, value, true);
                                                    }
                                                }
                                            },
                                            settings,
                                            dueTime > TimeSpan.Zero ? dueTime : TimeSpan.Zero,
                                            TimeSpan.FromMilliseconds(-1));
                                            mapNodeToDelayTimer[settings.nodeId] = t;
                                        }
                                    }
                                }

                                if (settings.isTotalTimeOn)
                                    StartTotalTimeOnUpdater(settings);

                                if (mapNodeToDelayTimer.ContainsKey(settings.nodeId))
                                    return false;

                                bool settingsstatechanged = IsAlarmStateChanged(settings);
                                settings.lastState = settings.state;

                                if (!settingsstatechanged && list.Count == 0)
                                    return false;
                            }
                        }
                    }
                }
            }

            if (list.Count > 0)
            {
                lock (lockObject)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    pendingAlarmChanged.AddRange(list);
                }
            }

            return list.Count > 0;
        }

        public void UpdateEventId(AlarmStatus alarmStatus)
        {
            AlarmConditionState node = null;
            lock (lockObject)
            {
                if (!alarms.TryGetValue(alarmStatus.nodeId, out node))
                    branches.TryGetValue(alarmStatus.nodeId, out node);
            }

            if (node != null)
                alarmStatus.serverEventId = node.EventId.Value;
        }

        public void UpdateAlarmStatus(IList<AlarmStatus> alarmsStatus, bool notify = true)
        {
            lock (lockObject)
            {
                var changedAlarms = new List<AlarmStatus>();

                foreach (var status in alarmsStatus)
                {
                    AlarmStatus alarm = null;
                    AlarmStatus parent = null;

                    if (alarms.ContainsKey(status.nodeId))
                        alarm = alarms[status.nodeId].Handle as AlarmStatus;
                    else if (!NodeId.IsNull(status.parentId) && branches.ContainsKey(status.nodeId))
                        alarm = branches[status.nodeId].Handle as AlarmStatus;

                    if (alarm != null)
                        alarm.UpdateStatus(status);

                    if (!NodeId.IsNull(status.parentId))
                    {
                        if (!NodeId.IsNull(status.parentId) && alarms.ContainsKey(status.parentId))
                            parent = alarms[status.parentId].Handle as AlarmStatus;
                        if (parent != null)
                        {
                            parent.UpdateArchive(status);
                            status.Name = parent.Name;
                            status.Message = parent.Message;
                        }
                    }

                    if (alarm != null)
                        changedAlarms.Add((AlarmStatus)alarm.Clone());
                    else if (parent != null)
                        changedAlarms.Add(status);
                }

                if (changedAlarms.Count > 0)
                {
                    if (pendingAlarmChanged.Count == 0)
                    {
                        smartThreadPool.QueueWorkItem(() =>
                        {
                            OnAlarmChanged();
                        });
                    }
                    changedAlarms.ForEach((alarm) => alarm.notify = notify);
                    pendingAlarmChanged.AddRange(changedAlarms);
                }
            }
        }

        #endregion

        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        /// 

        bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    var waitHandles = new List<WaitHandle>();
                    lock (lockObject)
                    {
                        foreach (var timer in mapNodeToDelayTimer.Values)
                        {
                            var ev = new AutoResetEvent(false);
                            timer.Change(0, System.Threading.Timeout.Infinite);
                            timer.Dispose(ev);
                            waitHandles.Add(ev);
                        }
                        mapNodeToDelayTimer.Clear();

                        foreach (var timer in mapNodeToRateOfChangeTimer.Values)
                        {
                            var ev = new AutoResetEvent(false);
                            timer.Change(0, System.Threading.Timeout.Infinite);
                            timer.Dispose(ev);
                            waitHandles.Add(ev);
                        }
                        mapNodeToRateOfChangeTimer.Clear();

                        pendingAlarmChanged.Clear();

                        if (mapAlarmState != null)
                            mapAlarmState.Clear();
                    }

                    if (waitHandles.Count > 0)
                    {
                        WaitHandle.WaitAll(waitHandles.ToArray());
                        waitHandles.ForEach((notify) => notify.Dispose());
                    }
                }
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
