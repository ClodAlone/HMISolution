using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAServerBase
{
    public class AccessSettings
    {
        public int UserReadAccessMask { get; set; }
        public int UserWriteAccessMask { get; set; }
        public int UserAccessLevel { get; set; }
        public bool IsAuditTraceEnabled { get; set; }
        public bool IsCommentRequiredOnAudit { get; set; }
        public bool IsPasswordRequiredOnAudit { get; set; }
        public int MinAccessLevelRequiredOnAudit { get; set; }
        public string LastUserNameOnAudit { get; set; }
        public string LastCommentOnAudit { get; set; }
    }
}
