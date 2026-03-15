using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracing.Model
{
    public class LogItem
    {
        public DateTime TimeStamp { get; set; }
        public Uri Source { get; set; }
        public String Message { get; set; }
        public int Severity { get; set; }
    }
}
