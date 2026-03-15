using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAHistorian
{
    public class ErrorFlushingDataArgs : EventArgs
    {
        public String connection { get; set; }
        public String exception { get; set; }
    }
}
