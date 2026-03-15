using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UFUAHistorian
{
    public class RecyclingDataArgs : EventArgs
    {
        public String nodeId { get; set; }
        public String name { get; set; }
        public int count { get; set; }
    }
}