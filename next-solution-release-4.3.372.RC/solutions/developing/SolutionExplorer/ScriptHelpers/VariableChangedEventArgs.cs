using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using OPCUAViewModel;

namespace UFProjectManager.ScriptHelpers
{
    public class VariableChangedEventArgs : EventArgs
    {
        public VariableChangedEventArgs(String applicationName, String endpoint, String path, 
                                        NodeId nodeId, DataValue value)
        {
            ApplicationName = applicationName;
            Endpoint = endpoint;
            Path = path;
            NodeId = nodeId;
            Value = value;
        }

        public DataValue Value { get; private set; }
        public String ApplicationName { get; private set; }
        public String Endpoint { get; private set; }
        public String Path { get; private set; }
        public NodeId NodeId { get; private set; }
    }
}