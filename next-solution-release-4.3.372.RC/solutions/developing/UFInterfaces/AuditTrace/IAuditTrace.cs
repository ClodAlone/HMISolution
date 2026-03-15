using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFInterfaces.AuditTrace
{
    public interface IAuditTrace
    {
        event EventHandler<EventArgs> AuditPropertiesFetched;
        bool IsAuditPropertiesFetched { get; }
    }
}
