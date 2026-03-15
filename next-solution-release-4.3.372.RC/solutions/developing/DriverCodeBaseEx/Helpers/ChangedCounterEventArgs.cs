using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriverCodeBaseEx.Helpers
{
    public class ChangedCounterEventArgs : EventArgs
    {
        public String Key { get; set; }
        public Opc.Ua.Variant newValue { get; set; }
    }
}
