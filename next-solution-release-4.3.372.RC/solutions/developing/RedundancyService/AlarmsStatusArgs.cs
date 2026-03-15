using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    public class AlarmsStatusArgs : EventArgs
    {
        public Dictionary<NodeId, List<UFUAAlarm.AlarmStatus>> alarmsStatus;
        public String hostName;
    }
}
