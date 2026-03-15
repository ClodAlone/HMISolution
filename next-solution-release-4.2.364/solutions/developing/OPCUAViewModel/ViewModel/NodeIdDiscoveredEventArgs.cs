using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif
using System.Windows.Input;
using ViewModelLib;
using Opc.Ua;
using Opc.Ua.Client;
using Utilities;
using System.Security.Cryptography.X509Certificates;
using UFInterfaces;
using System.Reflection;
using System.IO;
using System.Collections.ObjectModel;

namespace OPCUAViewModel
{
    public class NodeIdDiscoveredEventArgs : EventArgs
    {
        bool isInstanceDiscovering;
        Dictionary<String, NodeId> mapDiscovered;
        Dictionary<String, NodeIdViewModel> mapDiscoveredViewModel;
        public NodeIdDiscoveredEventArgs(Dictionary<String, NodeId> map,
                                         Dictionary<String, NodeIdViewModel> mapModels, 
                                         bool isInstance)
        {
            mapDiscovered = map;
            mapDiscoveredViewModel = mapModels;
            IsInstanceDiscovering = isInstance;
        }

        public bool HasNodeId(NodeId item)
        {
            var found = (from c in mapDiscovered.Values where c == item select c).ToList();
            return found.Count > 0;
        }

        public bool HasNodeId(String item)
        {
            if (item == null)
                return false;

            return mapDiscovered.ContainsKey(item);
        }

        public bool HasNodeIdViewModel(String item)
        {
            if (item == null)
                return false;

            return mapDiscoveredViewModel.ContainsKey(item);
        }

        public NodeId GetNodeId(NodeId item)
        {
            var found = (from c in mapDiscovered.Values where c == item select c).ToList();
            if (found.Count > 0)
                return found[0];
            return null;
        }

        public NodeIdViewModel GetNodeIdViewModel(NodeId item)
        {
            var found = (from c in mapDiscoveredViewModel.Values where c.nodeId == item select c).ToList();
            if (found.Count > 0)
                return found[0];
            return null;
        }

        public NodeId GetNodeId(String item)
        {
            if (item == null)
                return null;
            
            if (mapDiscovered.ContainsKey(item))
                return mapDiscovered[item];
            return null;
        }

        public NodeIdViewModel GetNodeIdViewModel(String item)
        {
            if (item == null)
                return null;

            if (mapDiscoveredViewModel.ContainsKey(item))
                return mapDiscoveredViewModel[item];
            return null;
        }

        public bool IsInstanceDiscovering
        {
            get
            {
                return isInstanceDiscovering;
            }
            private set
            {
                isInstanceDiscovering = value;
            }
        }
    }
}
