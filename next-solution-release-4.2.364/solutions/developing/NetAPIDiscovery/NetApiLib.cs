using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.DirectoryServices;

namespace NetAPIDiscovery
{
    public static class NetApiLib
    {
        public static bool NetworkAvailable
        {
            get
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                return (nics != null && nics.Length > 0);
            }
        }

        #region NetApi Function Declarations
        /// <summary>
        /// Enumerates computers on the local network.
        /// </summary>
        public static string[] EnumComputers(String sDomain, string username = null, string password = null)
        {
            var ret = new List<String>();
            var root = new DirectoryEntry(String.Format("WinNT://{0}", sDomain), username, password);
            foreach (DirectoryEntry entry in root.Children)
            {
                if (entry.SchemaClassName == "Computer")
                {
                    ret.Add(entry.Name);
                }
            }

            return ret.ToArray();
        }

        public static string[] EnumDomains()
        {
            var ret = new List<String>();
            var root = new DirectoryEntry("WinNT:");
            foreach (DirectoryEntry dom in root.Children)
            {
                ret.Add(dom.Name);
            }
            return ret.ToArray();
        }
        #endregion
    }
}
