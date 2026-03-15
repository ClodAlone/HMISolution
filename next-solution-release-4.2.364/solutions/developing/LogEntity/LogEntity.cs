using System;
using DevExpress.Xpo;
using System.Data;

namespace LogEntity
{
    public class LogEntity : XPObject
    {
        #region Constructor
        public LogEntity()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LogEntity(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }
        #endregion

        #region Properties
        private string _Message;
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                SetPropertyValue("Message", ref _Message, value);
            }
        }

        private DateTime _TimeStamp;
        public DateTime TimeStamp
        {
            get
            {
                return _TimeStamp;
            }
            set
            {
                SetPropertyValue("TimeStamp", ref _TimeStamp, value);
            }
        }

        private TimeSpan _Duration;
        public TimeSpan Duration
        {
            get
            {
                return _Duration;
            }
            set
            {
                SetPropertyValue("Duration", ref _Duration, value);
            }
        }

        private int _SequenceId;
        public int SequenceId
        {
            get
            {
                return _SequenceId;
            }
            set
            {
                SetPropertyValue("TransactionId", ref _SequenceId, value);
            }
        }

        private Guid _UniqueId;
        public Guid UniqueId
        {
            get
            {
                return _UniqueId;
            }
            set
            {
                SetPropertyValue("UniqueId", ref _UniqueId, value);
            }
        }

        private string _SourceName;
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

        private int _Severity;
        public int Severity
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

        private int _EventId;
        public int EventId
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

        private int _EventType;
        public int EventType
        {
            get
            {
                return _EventType;
            }
            set
            {
                SetPropertyValue("EventType", ref _EventType, value);
            }
        }

        private string _Comment;
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                SetPropertyValue("Comment", ref _Comment, value);
            }
        }

        private string _Culture;
        public string Culture
        {
            get
            {
                return _Culture;
            }
            set
            {
                SetPropertyValue("Culture", ref _Culture, value);
            }
        }

        private string _Area;
        public string Area
        {
            get
            {
                return _Area;
            }
            set
            {
                SetPropertyValue("Area", ref _Area, value);
            }
        }

        private string _Domain;
        public string Domain
        {
            get
            {
                return _Domain;
            }
            set
            {
                SetPropertyValue("Domain", ref _Domain, value);
            }
        }

        private string _User;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }
        #endregion
    }
}