using System;
using System.Collections.Generic;
using System.Linq;

namespace UFUAHistorian
{
    public class FlushedDataSafelyArgs : EventArgs
    {
        public String Name { get; set; }
        public int Count { get; set; }
        public String Connection { get; set; }
        public bool Result { get; set; }
    }
}
