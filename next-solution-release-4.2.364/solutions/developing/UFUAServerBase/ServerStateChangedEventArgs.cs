using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UFUAServerBase
{
    public class ServerStateChangedEventArgs : EventArgs
    {
        public String ServerName;
        public Opc.Ua.ServerState OldState;
        public Opc.Ua.ServerState NewState;
    }
}
