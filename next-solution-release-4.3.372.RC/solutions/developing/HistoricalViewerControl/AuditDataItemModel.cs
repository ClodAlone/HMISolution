using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalViewerControl
{
    public class AuditDataItemModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public double? dValue { get; set; }
        public string ValueBefore { get; set; }
        public double? dValueBefore { get; set; }
        public DateTime RecordDateTime { get; set; }
        public DateTime SourceTimeStamp { get; set; }
        public ushort SourcePicoseconds { get; set; }
        public DateTime ServerTimeStamp { get; set; }
        public ushort ServerPicoseconds { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
    }
}
