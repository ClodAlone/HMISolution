using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedundancyService
{
    public class CallMethodArgs : EventArgs
    {
        public NodeId methodNodeId;
        public CallMethodRequest methodRequest;
        public CallMethodResult methodResult;
        public ServiceResult serviceResult;
    }
}
