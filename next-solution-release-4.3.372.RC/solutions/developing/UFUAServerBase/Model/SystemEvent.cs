using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace UFUAServerBase
{
    public class SystemEvent
    {
        public SystemEvent()
        {
            evtype = ObjectTypeIds.ServerType;
            details = 
                comment = 
                state = 
                //uniqueid = 
                username = String.Empty;
        }

        public NodeId evtype { get; set; }
        public String details { get; set; }
        public String comment { get; set; }
        public String state { get; set; }
        //public String uniqueid { get; set; }
        public String username { get; set; }
        public String logentry { get; set; }
        public int logdestination { get; set; }
    }
}
