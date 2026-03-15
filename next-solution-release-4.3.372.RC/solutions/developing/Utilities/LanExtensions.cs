using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class LanExtensions
    {
        public static bool IsLanIP(String address)
        {
            foreach (var ip in Dns.GetHostAddresses(address))
            {
                if (IsLanIP(ip))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for IP address or well known hostnames that map to the computer.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <returns>The hostname to use for URL filtering.</returns>
        public static string NormalizeHostname(string hostname)
        {
            string computerName = System.Net.Dns.GetHostName();

            // substitute the computer name for localhost if localhost used by client.
            if (AreDomainsEqual(hostname, "localhost"))
            {
                return computerName.ToUpper();
            }

            // check if client is using an ip address.
            System.Net.IPAddress address = null;

            if (System.Net.IPAddress.TryParse(hostname, out address))
            {
                if (System.Net.IPAddress.IsLoopback(address))
                {
                    return computerName.ToUpper();
                }

                // substitute the computer name for any local IP if an IP is used by client.
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());

                for (int ii = 0; ii < addresses.Length; ii++)
                {
                    if (addresses[ii].Equals(address))
                    {
                        return computerName.ToUpper();
                    }
                }

                // not a localhost IP address.
                return hostname.ToUpper();
            }

            // check for aliases.
            System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(computerName);

            for (int ii = 0; ii < entry.Aliases.Length; ii++)
            {
                if (AreDomainsEqual(hostname, entry.Aliases[ii]))
                {
                    return computerName.ToUpper();
                }
            }

            // return normalized hostname.
            return hostname.ToUpper();
        }

        /// <summary>
        /// Checks if the domains are equal.
        /// </summary>
        /// <param name="domain1">The first domain to compare.</param>
        /// <param name="domain2">The second domain to compare.</param>
        /// <returns>True if they are equal.</returns>
        public static bool AreDomainsEqual(string domain1, string domain2)
        {
            if (String.IsNullOrEmpty(domain1) || String.IsNullOrEmpty(domain2))
            {
                return false;
            }

            if (String.Compare(domain1, domain2, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }

        private static bool IsLanIP(IPAddress address)
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var iface in interfaces)
            {
                var properties = iface.GetIPProperties();
                foreach (var ifAddr in properties.UnicastAddresses)
                {
                    if (ifAddr.IPv4Mask != null &&
                        ifAddr.Address.AddressFamily == AddressFamily.InterNetwork &&
                        CheckMask(ifAddr.Address, ifAddr.IPv4Mask, address))
                        return true;
                }
            }
            return false;
        }

        private static bool CheckMask(IPAddress address, IPAddress mask, IPAddress target)
        {
            if (mask == null)
                return false;

            var ba = address.GetAddressBytes();
            var bm = mask.GetAddressBytes();
            var bb = target.GetAddressBytes();

            if (ba.Length != bm.Length || bm.Length != bb.Length)
                return false;

            for (var i = 0; i < ba.Length; i++)
            {
                int m = bm[i];

                int a = ba[i] & m;
                int b = bb[i] & m;

                if (a != b)
                    return false;
            }

            return true;
        }
    }
}
