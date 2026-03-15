using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAAlarm
{
    public class ChangedAlarmsArgs : EventArgs
    {
        public NodeId nodeId;
        public List<UFUAAlarm.AlarmStatus> alarmsStatus;
    }
}
