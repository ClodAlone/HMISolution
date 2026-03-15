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
    // Summary:
    //     Provides data for the System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    //     event.
    public class MonitoreItemListChangedEventArgs : EventArgs
    {
        public MonitoreItemListChangedEventArgs(Dictionary<NodeId, MonitoredItemViewModel> added)
        {
            _mapAdded = added;
        }

        Dictionary<NodeId, MonitoredItemViewModel> _mapAdded;

        public bool HasMonitoredItem(NodeId nodeId)
        {
            return _mapAdded.ContainsKey(nodeId);
        }

        public MonitoredItemViewModel GetMonitoredItemViewModel(NodeId nodeId)
        {
            if (_mapAdded.ContainsKey(nodeId))
                return _mapAdded[nodeId];
            return null;
        }
    }
}
