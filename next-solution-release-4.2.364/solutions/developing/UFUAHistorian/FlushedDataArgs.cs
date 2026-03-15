using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAHistorian
{
    public class FlushedDataArgs : EventArgs
    {
        public String connection { get; set; }
        public int count { get; set; }
    }
}
