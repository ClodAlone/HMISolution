using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAAlarm
{
    public class AlarmStateChangedArgs : EventArgs
    {
        public NodeId nodeId;
        public AlarmState oldState;
        public AlarmState newState;
        public bool isMessage;
        public bool canPlay;
        public bool canAck;
        public bool canReset;
    }
}
