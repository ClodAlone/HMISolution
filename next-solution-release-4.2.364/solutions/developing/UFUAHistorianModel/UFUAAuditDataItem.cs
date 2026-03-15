using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Opc.Ua;

namespace UFUAHistorianModel
{

    [DeferredDeletion(false)]
    public class UFUAAuditDataItem : XPObject, IRedundancyDataSync
    {
        #region Constructors
        public UFUAAuditDataItem(Session session)
            : base(session)
        { }

        public UFUAAuditDataItem(Session session, UFUAAuditDataItem template)
            : base(session)
        {
            Name = template.Name;
            Value = template.Value;
            dValue = template.dValue;
            ValueBefore = template.ValueBefore;
            dValueBefore = template.dValueBefore;
            StatusCode = template.StatusCode;
            Status = template.Status;
            RecordDateTimeUtc = template.RecordDateTimeUtc;
            RecordDateTime = template.RecordDateTime;
            RecordDateTimeMilliseconds = template.RecordDateTimeMilliseconds;
            SourceTimeStamp = template.SourceTimeStamp;
            SourcePicoseconds = template.SourcePicoseconds;
            ServerTimeStamp = template.ServerTimeStamp;
            ServerPicoseconds = template.ServerPicoseconds;
            UserName = template.UserName;
            Reason = template.Reason;
            ModificationTime = template.ModificationTime;
            RedundancySyncTime = template.RedundancySyncTime;
            EventId = template.EventId;
        }
        #endregion

        #region Properties
        private string _Name;
        [Size(2048)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != null && value.Length > 2048)
                    value = value.Substring(0, 2048);

                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private string _Value;
        [Size(SizeAttribute.Unlimited)]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                SetPropertyValue("Value", ref _Value, value);
            }
        }

        private double? _dValue;
        public double? dValue
        {
            get
            {
                return _dValue;
            }
            set
            {
                if (value.HasValue && (Double.IsNaN(value.Value) || Double.IsInfinity(value.Value)))
                    value = null;
                SetPropertyValue("dValue", ref _dValue, value);
            }
        }

        private string _ValueBefore;
        [Size(SizeAttribute.Unlimited)]
        public string ValueBefore
        {
            get
            {
                return _ValueBefore;
            }
            set
            {
                SetPropertyValue("ValueBefore", ref _ValueBefore, value);
            }
        }

        private double? _dValueBefore;
        public double? dValueBefore
        {
            get
            {
                return _dValueBefore;
            }
            set
            {
                if (value.HasValue && (Double.IsNaN(value.Value) || Double.IsInfinity(value.Value)))
                    value = null;
                SetPropertyValue("dValueBefore", ref _dValueBefore, value);
            }
        }

        private uint _StatusCode;
        public uint StatusCode
        {
            get
            {
                return _StatusCode;
            }
            set
            {
                SetPropertyValue("StatusCode", ref _StatusCode, value);
            }
        }

        private string _Status;
        [Size(32)]
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (value != null && value.Length > 32)
                    value = value.Substring(0, 32);

                SetPropertyValue("Status", ref _Status, value);
            }
        }

        private DateTime _RecordDateTimeUtc;
        [Indexed(Unique = false)]
        public DateTime RecordDateTimeUtc
        {
            get
            {
                return _RecordDateTimeUtc;
            }
            set
            {
                SetPropertyValue("RecordDateTimeUtc", ref _RecordDateTimeUtc, value);
            }
        }

        private DateTime _RecordDateTime;
        [Indexed(Unique = false)]
        public DateTime RecordDateTime
        {
            get
            {
                return _RecordDateTime;
            }
            set
            {
                SetPropertyValue("RecordDateTime", ref _RecordDateTime, value);
            }
        }

        private ushort _RecordDateTimeMilliseconds;
        public ushort RecordDateTimeMilliseconds
        {
            get
            {
                return _RecordDateTimeMilliseconds;
            }
            set
            {
                SetPropertyValue("RecordDateTimeMilliseconds", ref _RecordDateTimeMilliseconds, value);
            }
        }

        private DateTime _sourceTimeStamp;
        [Indexed(Unique = false)]
        public DateTime SourceTimeStamp
        {
            get
            {
                return _sourceTimeStamp;
            }
            set
            {
                SetPropertyValue("SourceTimeStamp", ref _sourceTimeStamp, value);
            }
        }

        private ushort _sourcePicoseconds;
        public ushort SourcePicoseconds
        {
            get
            {
                return _sourcePicoseconds;
            }
            set
            {
                SetPropertyValue("SourcePicoseconds", ref _sourcePicoseconds, value);
            }
        }

        private DateTime _serverTimeStamp;
        [Indexed(Unique = false)]
        public DateTime ServerTimeStamp
        {
            get
            {
                return _serverTimeStamp;
            }
            set
            {
                SetPropertyValue("ServerTimeStamp", ref _serverTimeStamp, value);
            }
        }

        private ushort _serverPicoseconds;
        public ushort ServerPicoseconds
        {
            get
            {
                return _serverPicoseconds;
            }
            set
            {
                SetPropertyValue("ServerPicoseconds", ref _serverPicoseconds, value);
            }
        }

        private string _UserName;
        [Size(256)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                if (value != null && value.Length > 256)
                    value = value.Substring(0, 256);

                SetPropertyValue("UserName", ref _UserName, value);
            }
        }

        private string _Reason;
        [Size(SizeAttribute.Unlimited)]
        public string Reason
        {
            get
            {
                return _Reason;
            }
            set
            {
                SetPropertyValue("Reason", ref _Reason, value);
            }
        }

        private DateTime _ModificationTime;
        public DateTime ModificationTime
        {
            get
            {
                return _ModificationTime;
            }
            set
            {
                SetPropertyValue("ModificationTime", ref _ModificationTime, value);
            }
        }

        private HistoryUpdateType _ModificationType;
        public HistoryUpdateType ModificationType
        {
            get
            {
                return _ModificationType;
            }
            set
            {
                SetPropertyValue("ModificationType", ref _ModificationType, value);
            }
        }

        private DateTime _RedundancySyncTime;
        [Indexed(Unique = false)]
        public DateTime RedundancySyncTime
        {
            get
            {
                return _RedundancySyncTime;
            }
            set
            {
                SetPropertyValue("RedundancySyncTime", ref _RedundancySyncTime, value);
            }
        }

        private Guid _EventId;
        [Indexed(Unique = true)]
        public Guid EventId
        {
            get
            {
                return _EventId;
            }
            set
            {
                SetPropertyValue("EventId", ref _EventId, value);
            }
        }

        private int dataLogRef;
        [Indexed(Unique = false)]
        public int DataLogRef
        {
            get
            {
                return dataLogRef;
            }
            set
            {
                SetPropertyValue("DataLogRef", ref dataLogRef, value);
            }
        }
        #endregion

        #region IRedundancyDataSync Interface
        [PersistentAlias("RecordDateTimeUtc")]
        public DateTime UtcRecordingTime
        {
            get 
            { 
                return RecordDateTimeUtc; 
            }
        }

        [PersistentAlias("EventId")]
        public Guid RedundancyUniqueId
        {
            get 
            { 
                return EventId; 
            }
        }
        #endregion
    }
}
