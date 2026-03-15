using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Management;

namespace MSZ
{
    sealed class MSZControls
    {
        internal static string noMac = "00000000";
        internal static string GetComponent()
        {
            string cpuInfo = string.Empty;
            try
            {
                using (var managClass = new ManagementClass("win32_processor"))
                {
                    using (var managCollec = managClass.GetInstances())
                    {
                        foreach (ManagementObject managObj in managCollec)
                        {
                            if (managObj.Properties["processorID"].Value != null)
                                cpuInfo = managObj.Properties["processorID"].Value.ToString();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }

            if (String.IsNullOrEmpty(cpuInfo))
            {
                try
                {
                    using (var managClass = new ManagementClass("Win32_DiskDrive"))
                    {
                        using (var managCollec = managClass.GetInstances())
                        {
                            foreach (ManagementObject managObj in managCollec)
                            {
                                if (managObj.Properties["Signature"].Value != null)
                                    cpuInfo = managObj.Properties["Signature"].Value.ToString();
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                { }
            }

            var macAddresses = GetMacAddress(null);

            if (string.IsNullOrEmpty(cpuInfo) && string.IsNullOrEmpty(macAddresses))
                throw new InvalidOperationException("Site code cannot be found.");

            if (string.IsNullOrEmpty(macAddresses))
                macAddresses = noMac;

            return string.Format("{0}@{1}", cpuInfo, macAddresses);
        }
        static string GetMacAddress(string mac)
        {
            var macAddresses =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit ||
                nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx ||
                nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT ||
                nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet)
                &&
                !(
                nic.Description.ToLower().Contains("virtual") ||
                nic.Description.ToLower().Contains("usb") ||
                nic.Description.ToLower().Contains("vmware") ||
                nic.Description.ToLower().Contains("xvm") ||
                nic.Description.ToLower().Contains("vpn"))
                &&
                (
                    string.IsNullOrEmpty(mac) ||
                    (!string.IsNullOrEmpty(mac) && nic.GetPhysicalAddress().ToString() == mac)
                )
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();

            if (string.IsNullOrEmpty(macAddresses))
                macAddresses =
                (
                    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    where nic.NetworkInterfaceType != NetworkInterfaceType.Wireless80211
                    &&
                    !(
                    nic.Description.ToLower().Contains("virtual") ||
                    nic.Description.ToLower().Contains("usb") ||
                    nic.Description.ToLower().Contains("vmware") ||
                    nic.Description.ToLower().Contains("xvm") ||
                    nic.Description.ToLower().Contains("vpn"))
                    &&
                    (
                     string.IsNullOrEmpty(mac) || 
                     (!string.IsNullOrEmpty(mac) && nic.GetPhysicalAddress().ToString() == mac)
                     )
                    select nic.GetPhysicalAddress().ToString()
                ).FirstOrDefault();

            return macAddresses;
        }
        internal static bool GetComponent(string macAddress)
        {
            if (string.IsNullOrEmpty(macAddress))
                return false;

            var dmacAddress = macAddress.Split('@');
            var macAddresses = string.Empty;

            if (string.IsNullOrEmpty(macAddress) || dmacAddress.Count() == 0)
                return false;

            if (dmacAddress.Count() > 1)
            {
                if (dmacAddress[1] != noMac)
                    macAddresses = GetMacAddress(dmacAddress[1]);
                    //    (
                    //    from nic in NetworkInterface.GetAllNetworkInterfaces()
                    //    where nic.GetPhysicalAddress().ToString() == dmacAddress[1]
                    //    select nic.GetPhysicalAddress().ToString()
                    //).FirstOrDefault();

                if (!string.IsNullOrEmpty(macAddresses))
                    return true;
                else
                {

                    string cpuInfo = string.Empty;
                    try
                    {
                        using (var managClass = new ManagementClass("win32_processor"))
                        {
                            using (var managCollec = managClass.GetInstances())
                            {
                                foreach (ManagementObject managObj in managCollec)
                                {
                                    if (managObj.Properties["processorID"].Value != null)
                                        cpuInfo = managObj.Properties["processorID"].Value.ToString();
                                    if (cpuInfo.Equals(dmacAddress[0]))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }

                    if (String.IsNullOrEmpty(cpuInfo))
                    {
                        try
                        {
                            using (var managClass = new ManagementClass("Win32_DiskDrive"))
                            {
                                using (var managCollec = managClass.GetInstances())
                                {
                                    foreach (ManagementObject managObj in managCollec)
                                    {
                                        if (managObj.Properties["Signature"].Value != null)
                                            cpuInfo = managObj.Properties["Signature"].Value.ToString();
                                        if (cpuInfo.Equals(dmacAddress[0]))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }

                    return false;
                }
            }
            else
            {
                macAddresses = GetMacAddress(macAddress);
                //(
                //    from nic in NetworkInterface.GetAllNetworkInterfaces()
                //    where nic.GetPhysicalAddress().ToString() == macAddress
                //    select nic.GetPhysicalAddress().ToString()
                //).FirstOrDefault();

                if (!string.IsNullOrEmpty(macAddresses))
                    return true;
                else
                {
                    string cpuInfo = string.Empty;
                    try
                    {
                        using (var managClass = new ManagementClass("win32_processor"))
                        {
                            using (var managCollec = managClass.GetInstances())
                            {
                                foreach (ManagementObject managObj in managCollec)
                                {
                                    if (managObj.Properties["processorID"].Value != null)
                                        cpuInfo = managObj.Properties["processorID"].Value.ToString();
                                    if (cpuInfo.Equals(macAddress))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { }

                    if (String.IsNullOrEmpty(cpuInfo))
                    {
                        try
                        {
                            using (var managClass = new ManagementClass("Win32_DiskDrive"))
                            {
                                using (var managCollec = managClass.GetInstances())
                                {
                                    foreach (ManagementObject managObj in managCollec)
                                    {
                                        if (managObj.Properties["Signature"].Value != null)
                                            cpuInfo = managObj.Properties["Signature"].Value.ToString();
                                        if (cpuInfo.Equals(macAddress))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }

                    return false;
                }
            }
        }
    }
}
