using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerManager
{
    public class ErrorStateChangedEventArgs : EventArgs
    {
        public String DataLoggerName { get; set; }
        public bool bErrorState { get; set; }
        public String ErrorMessage { get; set; }
    }
}
