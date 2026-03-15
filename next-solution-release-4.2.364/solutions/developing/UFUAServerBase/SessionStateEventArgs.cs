using Opc.Ua.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAServerBase
{
    public class SessionStateEventArgs
    {
        public String SessionName;
        public String SessionIdentity;
        public String ClientName;
        public String ClientIdentity;
        public String OldUserNameIndentity;
        public String NewUserNameIndentity;
        public SessionEventReason SessionState;
    }
}
