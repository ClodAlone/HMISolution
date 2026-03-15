using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;
using UFUAAlarm;

namespace ADServer
{
    [DeferredDeletion(false)]
    public class NotificationPersistence : XPObject
    {
        #region Constructors

        public NotificationPersistence(Session session)
            : base(session)
        {
            
        }

        #endregion

        #region Properties
        private double _LastValue;
        public double LastValue
        { 
            get { return _LastValue; } 
            set { SetPropertyValue("LastValue", ref _LastValue, value); } 
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
        private bool _svrActive;
        public bool svrActive
        {
            get { return _svrActive; }
            set { SetPropertyValue("svrActive", ref _svrActive, value); }
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
        private byte _CurrentState;
        public byte CurrentState
        {
            get { return _CurrentState; }
            set { SetPropertyValue("CurrentState", ref _CurrentState, value); }
        }
        private ushort _CurrentWState;
        public ushort CurrentWState
        {
            get { return _CurrentWState; }
            set { SetPropertyValue("CurrentWState", ref _CurrentWState, value); }
        }

        private uint _flMessageSent;
        public uint flMessageSent
        {
            get { return _flMessageSent; }
            set { SetPropertyValue("flMessageSent", ref _flMessageSent, value); }
        }

        private DateTime? _ServerTime;
        public DateTime? ServerTime
        {
            get { return _ServerTime; }
            set { _ServerTime = value; }
        }
        #endregion
    }
}
