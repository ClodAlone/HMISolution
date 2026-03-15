using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using Opc.Ua;

namespace UFUATagLogger
{
    [DeferredDeletion(false)]
    public class TagPersistence : XPObject
    {
        #region Constructors

        public TagPersistence(Session session)
            : base(session)
        {

        }

        #endregion

        #region Properties

        private String _NodeId;
        [Size(SizeAttribute.Unlimited)]
        public String NodeId
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

        private string _FriendlyName;
        [Size(SizeAttribute.Unlimited)]
        public string FriendlyName
        {
            get
            {
                return _FriendlyName;
            }
            set
            {
                SetPropertyValue("FriendlyName", ref _FriendlyName, value);
            }
        }

        private string _Value;
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
        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
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

        #region Statistics
        private double? _lastDoubleValue;
        public double? LastDoubleValue
        {
            get
            {
                return _lastDoubleValue;
            }
            set
            {
                SetPropertyValue("LastDoubleValue", ref _lastDoubleValue, value);
            }
        }


        private double? _Min;
        public double? Min
        {
            get
            {
                return _Min;
            }
            set
            {
                SetPropertyValue("Min", ref _Min, value);
            }
        }

        private double? _Max;
        public double? Max
        {
            get
            {
                return _Max;
            }
            set
            {
                SetPropertyValue("Max", ref _Max, value);
            }
        }

        private double? _TotAverage;
        public double? TotAverage
        {
            get
            {
                return _TotAverage;
            }
            set
            {
                SetPropertyValue("TotAverage", ref _TotAverage, value);
            }
        }

        private double? _CountUpdates;
        public double? CountUpdates
        {
            get
            {
                return _CountUpdates;
            }
            set
            {
                SetPropertyValue("CountUpdates", ref _CountUpdates, value);
            }
        }

        private TimeSpan? _TotalTimeOn;
        public TimeSpan? TotalTimeOn
        {
            get
            {
                return _TotalTimeOn;
            }
            set
            {
                SetPropertyValue("TotalTimeOn", ref _TotalTimeOn, value);
            }
        }

        private DateTime? _LastTotalTimeOn;
        public DateTime? LastTotalTimeOn
        {
            get
            {
                return _LastTotalTimeOn;
            }
            set
            {
                SetPropertyValue("LastTotalTimeOn", ref _LastTotalTimeOn, value);
            }
        }

        #endregion

        #endregion
    }
}
