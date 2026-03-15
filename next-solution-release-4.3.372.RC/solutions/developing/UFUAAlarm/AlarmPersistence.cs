using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Utilities;

namespace UFUAAlarm
{
    #region Converters for XPObject
    public class ConvertDataValue : ValueConverter
    {

        public override object ConvertFromStorageType(object value)
        {
            var xaml = value as String;
            if (xaml == null)
                return null;

            return xaml.FromXml<Opc.Ua.DataValue[]>();
        }

        public override object ConvertToStorageType(object value)
        {
            var datavalue = value as Opc.Ua.DataValue[];
            if (datavalue == null)
                return null;

            return datavalue.ToXml();
        }

        public override Type StorageType
        {
            get
            {
                return typeof(string);
            }
        }
    }
    #endregion

    [DeferredDeletion(false)]
    public class AlarmPersistence : XPObject
    {
        #region Constructors

        public AlarmPersistence(Session session)
            : base(session)
        {
            
        }

        #endregion

        #region Properties

        private string _NodeId;
        [Size(SizeAttribute.Unlimited)]
        public string NodeId
        {
            get
            {
                return _NodeId;
            }
            set
            {
                SetPropertyValue("NodeId", ref _NodeId, value);
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

        private string _Message;
        [Size(SizeAttribute.Unlimited)]
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

        private string _Comment;
        [Size(SizeAttribute.Unlimited)]
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

        private string _UserName;
        [Size(SizeAttribute.Unlimited)]
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                SetPropertyValue("UserName", ref _UserName, value);
            }
        }

        private double _Value;
        public double Value
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

        private Opc.Ua.DataValue[] _TagAliasValue;
        [ValueConverter(typeof(ConvertDataValue))]
        [Size(SizeAttribute.Unlimited)]
        public Opc.Ua.DataValue[] TagAliasValue
        {
            get
            {
                return _TagAliasValue;
            }
            set
            {
                SetPropertyValue("TagAliasValue", ref _TagAliasValue, value);
            }
        }

        private uint _Quality;
        public uint Quality
        {
            get
            {
                return _Quality;
            }
            set
            {
                SetPropertyValue("Quality", ref _Quality, value);
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

        private DateTime _EnableTime;
        public DateTime EnableTime
        {
            get
            {
                return _EnableTime;
            }
            set
            {
                SetPropertyValue("EnableTime", ref _EnableTime, value);
            }
        }

        private DateTime _AcknowledgeTime;
        public DateTime AcknowledgeTime
        {
            get
            {
                return _AcknowledgeTime;
            }
            set
            {
                SetPropertyValue("AcknowledgeTime", ref _AcknowledgeTime, value);
            }
        }

        private DateTime _ConfirmTime;
        public DateTime ConfirmTime
        {
            get
            {
                return _ConfirmTime;
            }
            set
            {
                SetPropertyValue("ConfirmTime", ref _ConfirmTime, value);
            }
        }

        private DateTime _SuppressTime;
        public DateTime SuppressTime
        {
            get
            {
                return _SuppressTime;
            }
            set
            {
                SetPropertyValue("SuppressTime", ref _SuppressTime, value);
            }
        }

        private DateTime _ActiveTime;
        public DateTime ActiveTime
        {
            get
            {
                return _ActiveTime;
            }
            set
            {
                SetPropertyValue("ActiveTime", ref _ActiveTime, value);
            }
        }

        private DateTime _ShelvingTime;
        public DateTime ShelvingTime
        {
            get
            {
                return _ShelvingTime;
            }
            set
            {
                SetPropertyValue("ShelvingTime", ref _ShelvingTime, value);
            }
        }
        
        private double _TimeOnShelf;
        public double TimeOnShelf
        {
            get
            {
                return _TimeOnShelf;
            }
            set
            {
                SetPropertyValue("TimeOnShelf", ref _TimeOnShelf, value);
            }
        }        

        private DateTime _ChangeStateTime;
        public DateTime ChangeStateTime
        {
            get
            {
                return _ChangeStateTime;
            }
            set
            {
                SetPropertyValue("ChangeStateTime", ref _ChangeStateTime, value);
            }
        }

        private AlarmState _State;
        public AlarmState State
        {
            get
            {
                return _State;
            }
            set
            {
                SetPropertyValue("State", ref _State, value);
            }
        }

        private bool _Offline;
        public bool Offline
        {
            get
            {
                return _Offline;
            }
            set
            {
                SetPropertyValue("Offline", ref _Offline, value);
            }
        }

        private string _ParentId;
        [Size(SizeAttribute.Unlimited)]
        public string ParentId
        {
            get
            {
                return _ParentId;
            }
            set
            {
                SetPropertyValue("ParentId", ref _ParentId, value);
            }
        }

        private DateTime _LastTimeUpdated;
        public DateTime LastTimeUpdated
        {
            get
            {
                return _LastTimeUpdated;
            }
            set
            {
                SetPropertyValue("LastTimeUpdated", ref _LastTimeUpdated, value);
            }
        }

        private ulong _Occurence;
        public ulong Occurence
        {
            get 
            {
                return _Occurence; 
            }
            set
            {
                SetPropertyValue("Occurence", ref _Occurence, value);
            }
        }

        private ulong _Sequence;
        public ulong Sequence
        {
            get
            {
                return _Sequence;
            }
            set
            {
                SetPropertyValue("Sequence", ref _Sequence, value);
            }
        }
        
        #endregion

    }
}
