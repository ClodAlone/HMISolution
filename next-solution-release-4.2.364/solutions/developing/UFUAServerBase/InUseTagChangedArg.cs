using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBaseInterfaces;

namespace UFUAServerBase
{
    internal class InUseTagChangedArg
    {
        public ICommunicationDriver Driver;
        public Opc.Ua.NodeId NodeId;
        public double SamplingInterval;
        public bool bInUse;
        public DateTime TimeStamp;
    }
}
