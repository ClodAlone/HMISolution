using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using OPCUAViewModel;

namespace UFProjectManager.ScriptHelpers
{
    public class ItemConnectedEventArgs : EventArgs
    {
        public ItemConnectedEventArgs(String applicationName, String endpoint, String path,
                                        NodeId nodeId, MonitoredItemViewModel model)
        {
            ApplicationName = applicationName;
            Endpoint = endpoint;
            Path = path;
            NodeId = nodeId;
            Model = model;
        }

        public MonitoredItemViewModel Model { get; private set; }
        public String ApplicationName { get; private set; }
        public String Endpoint { get; private set; }
        public String Path { get; private set; }
        public NodeId NodeId { get; private set; }
    }

    public class ItemDiscoveredEventArgs : EventArgs
    {
        public ItemDiscoveredEventArgs(String applicationName, String endpoint, String path,
                                        NodeId nodeId, NodeIdViewModel model)
        {
            ApplicationName = applicationName;
            Endpoint = endpoint;
            Path = path;
            NodeId = nodeId;
            Model = model;
        }

        public NodeIdViewModel Model { get; private set; }
        public String ApplicationName { get; private set; }
        public String Endpoint { get; private set; }
        public String Path { get; private set; }
        public NodeId NodeId { get; private set; }
    }
}
