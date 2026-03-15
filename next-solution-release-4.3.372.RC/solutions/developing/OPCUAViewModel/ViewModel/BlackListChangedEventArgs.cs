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
    public class BlackListChangedEventArgs : EventArgs
    {
        bool isInstanceDiscovering;
        List<String> listBlack;
        public BlackListChangedEventArgs(List<String> list, bool isInstance)
        {
            listBlack = list;
            isInstanceDiscovering = isInstance;
        }

        public bool IsOnBlackList(String item)
        {
            if (item == null)
                return false;
            return listBlack.Contains(item);
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
