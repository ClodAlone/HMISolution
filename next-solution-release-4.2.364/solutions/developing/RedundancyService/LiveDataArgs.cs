using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    public class LiveDataArgs : EventArgs
    {
        public Dictionary<NodeId, LiveDataValue> liveData;
        public String hostName;
        public LiveDataArgs()
        {
        }
    }
}
