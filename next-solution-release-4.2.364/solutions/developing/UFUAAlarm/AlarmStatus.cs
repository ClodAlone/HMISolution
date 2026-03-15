using System;
using System.Collections.Generic;
using Opc.Ua;
using System.Threading.Tasks;
using StringModel;
using System.Threading;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;
using StatDef;

namespace UFUAAlarm
{
    public class AlarmStatus : ICloneable
    {
        #region Declarations
        readonly internal static AlarmState initialState = AlarmState.Enabled | AlarmState.Acknowledged | AlarmState.Confirmed;
        
        Object lockObject;
        Dictionary<NodeId, AlarmStatus> archive;
        AlarmStatus parent;

        public NodeId nodeId;
        public String Name;
        public String Message;
        public UFUAModel.AlarmType type;
        public TripCondition TripCondition;
        public ExceptionDeviationFormat ExceptionDeviationFormat;
        public double ActivationLowValue;
        public double ActivationValue;
        public uint? ActivationValueQuality;
        public uint? ActivationLowValueQuality;
        public double?[] Limits;
        public uint?[] LimitsQuality;
        public double TimeUnit;
        public double DelayTimeOn;
        public double DelayTimeOff;
        public EventSeverity Severity;
        public Range range;
        public Range instrumentRange;
        public bool SaveEventsLog;
        public bool SaveBranches;
        public bool SupportAck;
        public bool SupportReset;
        public bool BeepEnabled;
        public string SoundFile;
        public bool RepeatSoundContinuously;
        public bool UseTimeStamp;
        public bool QualityGoodOnly;
        public String Expression;
        public bool isTotalTimeOn;
        public IStatisticsData StatisticsData;

        // Runtime values
        public AlarmState state = initialState;
        public String Comment;
        public ulong Occurence = ulong.MinValue;
        public ulong Sequence = ulong.MinValue;

        public String UserName;

        public String EventComment;
        public String EventUserName;

        public DataValue[] TagAliasLastValues;
        public DataValue[] TagAliasShowValues;

        public DateTime Time = DateTime.MinValue;
        public DateTime EnableTime = DateTime.MinValue;
        public DateTime AcknowledgeTime = DateTime.MinValue;
        public DateTime ConfirmTime = DateTime.MinValue;
        public DateTime SuppressTime = DateTime.MinValue;
        public DateTime ActiveTime = DateTime.MinValue;
        public DateTime ShelvingTime = DateTime.MinValue;
        public double TimeOnShelf = 0;

        public DateTime lastTimeUpdated = DateTime.MinValue;
        public DateTime lastTimeStateChanged = DateTime.MinValue;
        public double lastValue = 0.0;
        public uint lastQuality = StatusCodes.Good;
        internal AlarmState lastState = initialState;
        internal TimeSpan activeStateDuration;

        internal bool isLogEntry = false;
        public bool isOffline = false;
        internal bool notify = true;

        public byte[] serverEventId; // when null the event id will be regenerated.
        public NodeId parentId;

        public bool CreateAlrMemberAckedState;
        public bool CreateAlrMemberConfirmedState;
        public bool CreateAlrMemberConfirm;
        public bool CreateAlrMemberSuppressedState;
        public bool CreateAlrMemberShelvingState;
        public bool CreateAlrMemberEnabledState;
        public bool CreateAlrMemberActiveState;
        public bool CreateAlrMemberQuality;
#if NET_STANDARD
        public bool CreateAlrMemberLocalTime;
#endif
        #endregion

        #region Persistance
        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            Initialize();
        }
        #endregion

        #region Constructors
        public AlarmStatus()
        {
            Initialize();
        }

        public AlarmStatus(AlarmStatus alarm)
            : this()
        {
            if (alarm == null) throw new ArgumentNullException("alarm");

            this.nodeId = alarm.nodeId;
            this.Name = alarm.Name;
            this.Message = alarm.Message;
            this.type = alarm.type;
            this.TripCondition = alarm.TripCondition;
            this.ExceptionDeviationFormat = alarm.ExceptionDeviationFormat;
            this.ActivationLowValue = alarm.ActivationLowValue;
            this.ActivationValue = alarm.ActivationValue;
            this.Limits = alarm.Limits;
            this.TimeUnit = alarm.TimeUnit;
            this.DelayTimeOn = alarm.DelayTimeOn;
            this.DelayTimeOff = alarm.DelayTimeOff;
            this.Severity = alarm.Severity;
            this.range = alarm.range;
            this.instrumentRange = alarm.instrumentRange;
            this.SaveEventsLog = alarm.SaveEventsLog;
            this.SaveBranches = alarm.SaveBranches;
            this.BeepEnabled = alarm.BeepEnabled;
            this.SoundFile = alarm.SoundFile;
            this.RepeatSoundContinuously = alarm.RepeatSoundContinuously;
            this.UseTimeStamp = alarm.UseTimeStamp;
            this.QualityGoodOnly = alarm.QualityGoodOnly;
            this.Expression = alarm.Expression;
            this.SupportAck = alarm.SupportAck;
            this.SupportReset = alarm.SupportReset;

            this.CreateAlrMemberAckedState = alarm.CreateAlrMemberAckedState;
            this.CreateAlrMemberConfirmedState = alarm.CreateAlrMemberConfirmedState;
            this.CreateAlrMemberConfirm = alarm.CreateAlrMemberConfirm;
            this.CreateAlrMemberSuppressedState = alarm.CreateAlrMemberSuppressedState;
            this.CreateAlrMemberShelvingState = alarm.CreateAlrMemberShelvingState;
            this.CreateAlrMemberEnabledState = alarm.CreateAlrMemberEnabledState;
            this.CreateAlrMemberActiveState = alarm.CreateAlrMemberActiveState;
            this.CreateAlrMemberQuality = alarm.CreateAlrMemberQuality;
#if NET_STANDARD
            this.CreateAlrMemberLocalTime = alarm.CreateAlrMemberLocalTime;
#endif

            // Runtimes.
            this.state = lastState = alarm.state;
            this.lastValue = alarm.lastValue;
            this.lastQuality = alarm.lastQuality;
            this.Comment = alarm.Comment;
            this.UserName = alarm.UserName;
            this.EventComment = alarm.EventComment;
            this.EventUserName = alarm.EventUserName;
            this.Time = alarm.Time;
            this.EnableTime = alarm.EnableTime;
            this.AcknowledgeTime = alarm.AcknowledgeTime;
            this.ConfirmTime = alarm.ConfirmTime;
            this.SuppressTime = alarm.SuppressTime;
            this.ActiveTime = alarm.ActiveTime;
            this.ShelvingTime = alarm.ShelvingTime;
            this.TimeOnShelf = alarm.TimeOnShelf;
            this.lastTimeUpdated = alarm.lastTimeUpdated;
            this.lastTimeStateChanged = alarm.lastTimeStateChanged;
            this.Occurence = alarm.Occurence;
            this.Sequence = alarm.Sequence;
            this.isOffline = alarm.isOffline;
            this.parentId = alarm.parentId;
            this.serverEventId = alarm.serverEventId;
            this.TagAliasShowValues = alarm.TagAliasShowValues;
        }

        void Initialize()
        {
            if (lockObject == null)
                lockObject = new Object();
            if (archive == null)
                archive = new Dictionary<NodeId, AlarmStatus>();
        }
        #endregion

        #region ICloneable
        public object Clone()
        {
            return new AlarmStatus(this);
        }
        #endregion

        #region Public Static Methods

        public static NodeId ConstructNodeId(NodeId nodeid, NodeId parent, UFUAModel.UFUAAlarmDefinition alarm, ushort namespaceIndex, String suffix = null)
        {
            if (String.IsNullOrEmpty(suffix))
                return new NodeId(String.Format("{0}?{1}/{2}/{3}/{4}",
                                                parent.Identifier, nodeid.Identifier,
                                                alarm.UFUAAlarmDefinitions.UFUAArea.Name,
                                                alarm.UFUAAlarmDefinitions.Name,
                                                alarm.Name), namespaceIndex);
            else
                return new NodeId(String.Format("{0}?{1}/{2}/{3}/{4}{5}",
                                parent.Identifier, nodeid.Identifier,
                                alarm.UFUAAlarmDefinitions.UFUAArea.Name,
                                alarm.UFUAAlarmDefinitions.Name,
                                alarm.Name, suffix), namespaceIndex);
        }

        internal bool CanRemove()
        {
            return CanRemove(this);
        }

        internal static bool CanRemove(AlarmStatus status)
        {
            if ((status.state & AlarmState.Active) != 0 ||
                (status.state & AlarmState.Deleted) != 0 ||
                status.archive.Count > 0)
                return false;

            return status.Severity == 0 || 
                (!status.SupportAck || (status.state & AlarmState.Acknowledged) != 0) &&
                (!status.SupportReset || (status.state & AlarmState.Confirmed) != 0);
        }

        #endregion

        #region Public Methods
        public List<AlarmStatus> GetBranches()
        {
            var ret = new List<AlarmStatus>();
            lock (lockObject)
            {
                if (archive.Values.Count > 0)
                    ret.AddRange(archive.Values);
                return ret;
            }
        }
        #endregion

        #region Implementation
        internal AlarmStatus CreateSnapshot(bool logEntry)
        {
            return CreateSnapshot(logEntry, TimeSpan.Zero);
        }
        internal AlarmStatus CreateSnapshot(bool logEntry, TimeSpan activeDuration)
        {
            if (logEntry)
                Sequence = GetNewCounter(Sequence);
            var ret = MemberwiseClone() as AlarmStatus;
            ret.serverEventId = null;
            ret.isLogEntry = logEntry;
            ret.activeStateDuration = activeDuration;

            EventComment = EventUserName = null;

            return ret;
        }

        internal AlarmStatus CreateArchiveSnapshot()
        {
            var ret = MemberwiseClone() as AlarmStatus;
            ret.nodeId = new NodeId(Guid.NewGuid());
            ret.SupportAck = true;
            ret.SupportReset = false;
            ret.serverEventId = null;
            ret.isLogEntry = false;
            ret.parentId = nodeId;
            ret.parent = this;
            lock (lockObject)
            {
                archive[ret.nodeId] = ret;
            }

            EventComment = EventUserName = null;

            return ret;
        }

        internal IList<AlarmStatus> CheckMaxSizeArchiveAndRemove(int maxsize)
        {
            lock (lockObject)
            {
                var list = new List<AlarmStatus>();

                if (archive.Count > maxsize)
                {
                    var alarms = archive.Values.OrderBy((c) => c.Time).Take(archive.Count - maxsize).ToList();
                    list.AddRange(alarms);
                    for (int ii = 0; ii < alarms.Count; ii++)
                    {
                        var branch = alarms[ii];
                        branch.SetStateBits(AlarmState.Deleted, true);
                        archive.Remove(branch.nodeId);
                    }
                }

                return list;
            }
        }

        internal void UpdateArchive(AlarmStatus branch)
        {
            if ((branch.state & AlarmState.Deleted) == 0)
            {
                archive[branch.nodeId] = branch;
                branch.parentId = nodeId;
                branch.parent = this;
            }
            else
            {
                archive.Remove(branch.nodeId);
            }
        }

        internal void UpdateStatus(AlarmStatus status)
        {
            state = lastState = status.state;
            lastValue = status.lastValue;
            lastQuality = status.lastQuality;
            if (status.Message != null)
                Message = status.Message;
            Comment = status.Comment;
            UserName = status.UserName;
            Time = status.Time;
            EnableTime = status.EnableTime;
            AcknowledgeTime = status.AcknowledgeTime;
            ConfirmTime = status.ConfirmTime;
            SuppressTime = status.SuppressTime;
            ActiveTime = status.ActiveTime;
            ShelvingTime = status.ShelvingTime;
            TimeOnShelf = status.TimeOnShelf;
            lastTimeUpdated = status.lastTimeUpdated;
            lastTimeStateChanged = status.lastTimeStateChanged;
            Occurence = status.Occurence;
            Sequence = status.Sequence;
            isOffline = status.isOffline;
            serverEventId = status.serverEventId;
            TagAliasShowValues = status.TagAliasShowValues;

            // set the deleted flag for inactive alarm.
            if (CanRemove())
            {
                SetStateBits(AlarmState.Deleted, true);
            }
        }

        internal void CheckResetCounters(bool bForce = false)
        {
            if (bForce || CanRemove())
            {
                Occurence = GetNewCounter(Occurence);
                Sequence = ulong.MinValue;
            }
        }

        ulong GetNewCounter(ulong sequence)
        {
            if (sequence >= ulong.MaxValue)
                sequence = ulong.MinValue;
            else
                sequence += 1;

            return sequence;
        }

        /// <summary>
        /// Sets or clears the bits in the alarm state mask.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <param name="isSet">if set to <c>true</c> the bits are set; otherwise they are cleared.</param>
        /// <returns>True if the state changed as a result of setting the bits.</returns>
        internal bool SetStateBits(AlarmState bits, bool isSet)
        {
            if (isSet)
            {
                bool currentlySet = ((state & bits) == bits);
                state |= bits;
                return !currentlySet;
            }

            bool currentlyCleared = ((state & ~bits) == state);
            state &= ~bits;
            return !currentlyCleared;
        }

        AlarmStatus FindAlarmStatus(NodeId recordNumber)
        {
            if (NodeId.IsNull(recordNumber) || nodeId == recordNumber)
                return this;

            lock (lockObject)
            {
                return archive[recordNumber];
            }
        }

        internal IList<AlarmStatus> EnableAlarm(bool enabling)
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (SetStateBits(AlarmState.Enabled, enabling))
                {
                    EnableTime = Time = DateTime.UtcNow;
                    list.Add(CreateSnapshot(true));
                }

                Parallel.ForEach(archive.Values, alarmStatus =>
                {
                    lock (archive)
                    {
                        if (alarmStatus.SetStateBits(AlarmState.Enabled, enabling))
                        {
                            alarmStatus.EnableTime = DateTime.UtcNow;
                            list.Add(alarmStatus.CreateSnapshot(false));
                        }
                    }
                });
            }

            return list;
        }

        internal IList<AlarmStatus> ShelveAlarm(bool shelving)
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (SetStateBits(AlarmState.Shelved, shelving))
                {
                    ShelvingTime = Time = DateTime.UtcNow;
                    list.Add(CreateSnapshot(true));
                }
            }

            return list;
        }

        internal IList<AlarmStatus> TimeUnshelveAlarm()
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (SetStateBits(AlarmState.Shelved, false))
                {
                    ShelvingTime = Time = DateTime.UtcNow;
                    list.Add(CreateSnapshot(true));
                }
            }

            return list;
        }
        
        internal IList<AlarmStatus> CommentAlarm(NodeId recordNumber, LocalizedText comment, String userName)
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                AlarmStatus alarm = FindAlarmStatus(recordNumber);
                alarm.Time = DateTime.UtcNow;
                alarm.Comment = Utils.Format("{0}", comment);
                alarm.UserName = userName;
                list.Add(alarm.CreateSnapshot(false));
            }

            return list;
        }

        internal IList<AlarmStatus> AcknowledgeAlarm(NodeId recordNumber, LocalizedText comment, String userName)
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                AlarmStatus alarm = FindAlarmStatus(recordNumber);
                if (alarm.SetStateBits(AlarmState.Acknowledged, true))
                {
                    alarm.Time = alarm.AcknowledgeTime = DateTime.UtcNow;
                    alarm.EventComment = Utils.Format("{0}", comment);
                    alarm.EventUserName = userName;
                    // remove branch.
                    if (!NodeId.IsNull(recordNumber))
                    {
                        alarm.SetStateBits(AlarmState.Deleted, true);
                        archive.Remove(recordNumber);
                        if (parent != null)
                            parent.archive.Remove(recordNumber);
                        // delete only if no braches exist.
                        if (archive.Count == 0 && parent != null && CanRemove(parent))
                        {
                            parent.SetStateBits(AlarmState.Deleted, true);
                            list.Add(alarm.parent.CreateSnapshot(false));
                        }
                    }
                    else
                        alarm.SetStateBits(AlarmState.Confirmed, !alarm.SupportReset);
                    // remove deleted branches from archive.
                    if (archive.Count > 0)
                    {
                        List<NodeId> keys = new List<NodeId>();
                        foreach (var key in archive.Keys)
                        {
                            if ((archive[key].state & AlarmState.Deleted) != 0)
                                keys.Add(key);
                        }
                        keys.ForEach((key) => archive.Remove(key));
                    }
                    // delete only if no braches exist.
                    if (archive.Count == 0 && CanRemove(alarm))
                        alarm.SetStateBits(AlarmState.Deleted, true);
                    list.Add(alarm.CreateSnapshot(NodeId.IsNull(recordNumber)));

                    // preset counters
                    alarm.CheckResetCounters();
                }
            }

            return list;
        }

        internal IList<AlarmStatus> ConfirmAlarm(NodeId recordNumber, LocalizedText comment, String userName)
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                AlarmStatus alarm = FindAlarmStatus(recordNumber);
                if ((alarm.state & AlarmState.Active) == 0 && alarm.SetStateBits(AlarmState.Confirmed, true))
                {
                    alarm.Time = alarm.ConfirmTime = DateTime.UtcNow;
                    alarm.EventComment = Utils.Format("{0}", comment);
                    alarm.EventUserName = userName;
                    // remove deleted branches from archive.
                    if (archive.Count > 0)
                    {
                        List<NodeId> keys = new List<NodeId>();
                        foreach (var key in archive.Keys)
                        {
                            if ((archive[key].state & AlarmState.Deleted) != 0)
                                keys.Add(key);
                        }
                        keys.ForEach((key) => archive.Remove(key));
                    }
                    // delete only if no braches exist.
                    if (archive.Count == 0 && CanRemove(alarm))
                        alarm.SetStateBits(AlarmState.Deleted, true);
                    list.Add(alarm.CreateSnapshot(NodeId.IsNull(recordNumber)));

                    // preset counters
                    alarm.CheckResetCounters();
                }
            }

            return list;
        }

        internal IList<AlarmStatus> Refresh()
        {
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                foreach (var el in archive.Values)
                    list.Add(el.CreateSnapshot(false));

                list.Add(this);
            }

            return list;
        }

        internal IList<AlarmStatus> SetOfflineState(bool offline)
        {
            isOffline = offline;
            var list = new List<AlarmStatus>();

            lock (lockObject)
            {
                if (SetStateBits(AlarmState.Suppressed, offline))
                {
                    SuppressTime = Time = DateTime.UtcNow;
                    list.Add(CreateSnapshot(false));
                }
            }

            return list;
        }
        #endregion
    }
}
