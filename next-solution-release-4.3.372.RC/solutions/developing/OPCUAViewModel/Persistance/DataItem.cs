using System;
using DevExpress.Xpo;

namespace Persistance
{

    [DeferredDeletion(false)]
    public class DataItem : XPObject
    {
        public DataItem()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DataItem(Session session)
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

    }

}