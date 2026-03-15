using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;

namespace UFUAHistorianModel
{
    [DeferredDeletion(false)]
    public class UFUAAuditLogItem : XPObject, IRedundancyDataSync
    {
        #region Constructors

        public UFUAAuditLogItem(Session session)
            : base(session)
        { }

        public UFUAAuditLogItem(Session session, UFUAAuditLogItem template)
            : base(session)
        {
            EventId = template.EventId;
            EventType = template.EventType;
            SourceNode = template.SourceNode;
            SourceName = template.SourceName;
            EventDateTime = template.EventDateTime;
            EventDateTimeUtc = template.EventDateTimeUtc;
            EventMessage = template.EventMessage;
            EventDetails = template.EventDetails;
            EventComment = template.EventComment;
            EventState = template.EventState;
            EventDuration = template.EventDuration;
            //EventUniqueId = template.EventUniqueId;
            EventOccurence = template.EventOccurence;
            EventSequence = template.EventSequence;
            Severity = template.Severity;
            UserName = template.UserName;
            RedundancySyncTime = template.RedundancySyncTime;
        }

        #endregion

        #region Properties
        
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

        private String _EventType;
        public String EventType
        {
            get
            {
                return _EventType;
            }
            set
            {
                if (value != null && value.Length > SizeAttribute.DefaultStringMappingFieldSize)
                    value = value.Substring(0, SizeAttribute.DefaultStringMappingFieldSize);

                SetPropertyValue("EventType", ref _EventType, value);
            }
        }

        private String _SourceNode;
        public String SourceNode
        {
            get
            {
                return _SourceNode;
            }
            set
            {
                if (value != null && value.Length > SizeAttribute.DefaultStringMappingFieldSize)
                    value = value.Substring(0, SizeAttribute.DefaultStringMappingFieldSize);

                SetPropertyValue("SourceNode", ref _SourceNode, value);
            }
        }

        private string _SourceName;
        [Size(SizeAttribute.Unlimited)]
        public string SourceName
        {
            get
            {
                return _SourceName;
            }
            set
            {
                SetPropertyValue("SourceName", ref _SourceName, value);
            }
        }

        private DateTime _EventDateTime;
        [Indexed(Unique = false)]
        public DateTime EventDateTime
        {
            get
            {
                return _EventDateTime;
            }
            set
            {
                SetPropertyValue("EventDateTime", ref _EventDateTime, value);
            }
        }

        private DateTime _EventDateTimeUtc;
        [Indexed(Unique = false)]
        public DateTime EventDateTimeUtc
        {
            get
            {
                return _EventDateTimeUtc;
            }
            set
            {
                SetPropertyValue("EventDateTimeUtc", ref _EventDateTimeUtc, value);
            }
        }

        private string _EventMessage;
        [Size(SizeAttribute.Unlimited)]
        public string EventMessage
        {
        	get
        	{
                return _EventMessage;
        	}
        	set
        	{
                SetPropertyValue("EventMessage", ref _EventMessage, value);
        	}
        }

        private string _EventDetails;
        [Size(SizeAttribute.Unlimited)]
        public string EventDetails
        {
        	get
        	{
                return _EventDetails;
        	}
        	set
        	{
                SetPropertyValue("EventDetails", ref _EventDetails, value);
        	}
        }

        private string _EventState;
        public string EventState
        {
            get
            {
                return _EventState;
            }
            set
            {
                if (value != null && value.Length > SizeAttribute.DefaultStringMappingFieldSize)
                    value = value.Substring(0, SizeAttribute.DefaultStringMappingFieldSize);

                SetPropertyValue("EventState", ref _EventState, value);
            }
        }

        private ushort _Severity;
        public ushort Severity
        {
            get
            {
                return _Severity;
            }
            set
            {
                SetPropertyValue("Severity", ref _Severity, value);
            }
        }

        private string _EventComment;
        [Size(SizeAttribute.Unlimited)]
        public string EventComment
        {
            get
            {
                return _EventComment;
            }
            set
            {
                SetPropertyValue("EventComment", ref _EventComment, value);
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

        private TimeSpan _EventDuration;
        public TimeSpan EventDuration
        {
            get
            {
                return _EventDuration;
            }
            set
            {
                SetPropertyValue("EventDuration", ref _EventDuration, value);
            }
        }

        //private String _EventUniqueId;
        //[Size(SizeAttribute.Unlimited)]
        //public String EventUniqueId
        //{
        //    get
        //    {
        //        return _EventUniqueId;
        //    }
        //    set
        //    {
        //        SetPropertyValue("EventUniqueId", ref _EventUniqueId, value);
        //    }
        //}

        private ulong _EventOccurence;
        public ulong EventOccurence
        {
            get
            {
                return _EventOccurence;
            }
            set
            {
                SetPropertyValue("EventOccurence", ref _EventOccurence, value);
            }
        }

        private ulong _EventSequence;
        public ulong EventSequence
        {
            get
            {
                return _EventSequence;
            }
            set
            {
                SetPropertyValue("EventSequence", ref _EventSequence, value);
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
        #endregion

        #region IRedundancyDataSync Interface
        [PersistentAlias("EventDateTimeUtc")]
        public DateTime UtcRecordingTime
        {
            get 
            { 
                return EventDateTimeUtc; 
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
