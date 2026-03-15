using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSModel
{
    public class ChangedSchedArgs : EventArgs
    {
        public SimpleScheduledEvent schedEvent;
        public Guid nodeid;
        public bool result;
    }
}
