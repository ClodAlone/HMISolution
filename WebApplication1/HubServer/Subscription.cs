using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubServer
{
    internal class Subscription : IDisposable
    {
        public void Dispose()
        {
            
        }

        internal bool IsAlive()
        {
            return true;
        }
    }
}
