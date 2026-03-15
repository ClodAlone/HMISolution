using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUserEditor
{
    public class UserAuditChange
    {
        public DateTime UtcTime { get; set; }
        public String Message { get; set; }
        public String Details { get; set; }
        public String UserName { get; set; }
    }
}
