using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    public class ServerStateArgs : EventArgs
    {
        public RedundancyServerState serverState;
    }

    public enum RedundancyServerState
    {
        StartPolling,
        Activating,
        Deactivating,
        Starting,
        Stopping
    }
}
