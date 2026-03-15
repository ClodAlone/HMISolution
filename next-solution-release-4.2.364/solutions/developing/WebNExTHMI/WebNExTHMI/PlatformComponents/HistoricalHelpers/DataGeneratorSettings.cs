using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPenHelpers
{
    public class DataGeneratorSettings
    {
        public String ConnectionString { get; set; }

        public bool DlrSource { get; set; }

        public String DlrName { get; set; }

        public String ColName { get; set; }

        public String UtcTimeColumnName { get; set; }

        public int ArrayIndex { get; set; }

        public int HDataCount { get; set; }

        public TimeSpan ClientTimezoneOffset { get; set; }

        public TimeSpan DeadBandInterval { get; set; }

        public TimeSpan DeadBandTimeFrame { get; set; }

        public virtual bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(ConnectionString) && HDataCount > 0;
            }
        }
    }
}
