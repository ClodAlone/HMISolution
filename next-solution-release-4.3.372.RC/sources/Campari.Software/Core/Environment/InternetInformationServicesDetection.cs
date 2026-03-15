// ---------------------------------------------------------------------------
// Campari Software
//
// InternetInformationServicesDetection.cs
//
// For more information on, look at:
//
// http://technet2.microsoft.com/WindowsServer/en/library/5b36c13b-c72e-4488-8bbe-7e4228911c381033.mspx?mfr=true
// http://geekswithblogs.net/sdorman/archive/2007/03/01/107732.aspx
// http://blogs.iis.net:80/chrisad/archive/2006/09/01/Detecting-if-IIS-is-installed_2E002E002E00_.aspx
//
// ---------------------------------------------------------------------------
// Copyright (C) 2006-2007 Campari Software
// All rights reserved.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
// FITNESS FOR A PARTICULAR PURPOSE.
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Microsoft.Win32;

using Campari.Software.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Campari.Software
{
    #region class InternetInformationServicesDetection
    /// <summary>
    /// Provides support for determining if a specific version of the .NET
    /// Framework runtime is installed and the service pack level for the
    /// runtime version.
    /// </summary>
    public static class InternetInformationServicesDetection
    {
        #region events

        #endregion

        #region class-wide fields

        const string IISRegKeyName = "Software\\Microsoft\\InetStp";
        const string IISRegKeyValue = "MajorVersion";
        const string IISRegKeyMinorVersionValue = "MinorVersion";
        const string IISComponentRegKeyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Setup\\Oc Manager\\Subcomponents";
        const string Netfx11RegKeyName = "Software\\Microsoft\\ASP.NET\\1.1.4322.0";
        const string Netfx20RegKeyName = "Software\\Microsoft\\ASP.NET\\2.0.50727.0";
        const string Netfx40RegKeyName = "Software\\Microsoft\\ASP.NET\\4.0.30319.0";
        const string NetRegKeyValue = "DllFullPath";

        #endregion

        #region private and internal properties and methods

        #region properties

        #endregion

        #region methods

        #region GetRegistryValue
        private static bool GetRegistryValue<T>(RegistryHive hive, string key, string value, RegistryValueKind kind, out T data)
        {
            bool success = false;
            data = default(T);

            using (RegistryKey baseKey = RegistryKey.OpenRemoteBaseKey(hive, String.Empty))
            {
                if (baseKey != null)
                {
                    using (RegistryKey registryKey = baseKey.OpenSubKey(key, RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (registryKey != null)
                        {
                            try
                            {
                                // If the key was opened, try to retrieve the value.
                                RegistryValueKind kindFound = registryKey.GetValueKind(value);
                                if (kindFound == kind)
                                {
                                    object regValue = registryKey.GetValue(value, null);
                                    if (regValue != null)
                                    {
                                        data = (T)Convert.ChangeType(regValue, typeof(T), CultureInfo.InvariantCulture);
                                        success = true;
                                    }
                                }
                            }
                            catch (IOException)
                            {
                                // The registry value doesn't exist. Since the
                                // value doesn't exist we have to assume that
                                // the component isn't installed and return
                                // false and leave the data param as the
                                // default value.
                            }
                        }
                    }
                }
            }
            return success;
        }
        #endregion

        #region IsIISInstalled functions

        #region IsIIS4Installed
        private static bool IsIIS4Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 4)
                {
                    found = true;
                }
            }

            return found;
        }

        private static bool IsIIS4OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue >= 4)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsIIS5Installed
        private static bool IsIIS5Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 5)
                {
                    // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS5OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 5)
                {
                    found = true;
                }
                else if (regValue == 5)
                {
                    // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >= 0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #region IsIIS51Installed
        private static bool IsIIS51Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 5)
                {
                    // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 1)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS51OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 5)
                {
                    found = true;
                }
                else if (regValue == 5)
                {
                    // We know that we have either 5.0 or 5.1, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >= 1)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #region IsIIS6Installed
        private static bool IsIIS6Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 6)
                {
                    found = true;
                }
            }

            return found;
        }

        private static bool IsIIS6OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue >= 6)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsIIS7Installed
        private static bool IsIIS7Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 7)
                {
                    // We know that we have either 7.0 or 7.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS7OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 7)
                {
                    found = true;
                }
                else if (regValue == 7)
                {
                    // We know that we have either 7.0 or 7.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >=0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #region IsIIS75Installed
        private static bool IsIIS75Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 7)
                {
                    // We know that we have either 7.0 or 7.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 5)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS75OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 7)
                {
                    found = true;
                }
                else if (regValue == 7)
                {
                    // We know that we have either 7.0 or 7.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >= 5)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #region IsIIS8Installed
        private static bool IsIIS8Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 8)
                {
                    // We know that we have either 8.0 or 8.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS8OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 8)
                {
                    found = true;
                }
                else if (regValue == 8)
                {
                    // We know that we have either 8.0 or 8.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >= 0)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #region IsIIS85Installed
        private static bool IsIIS85Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 8)
                {
                    // We know that we have either 8.0 or 8.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion == 5)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }

        private static bool IsIIS85OrHigherInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyValue, RegistryValueKind.DWord, out regValue))
            {
                if (regValue > 8)
                {
                    found = true;
                }
                else if (regValue == 8)
                {
                    // We know that we have either 8.0 or 8.5, so check the MinorVersion value.
                    int minorVersion = -1;
                    if (GetRegistryValue(RegistryHive.LocalMachine, IISRegKeyName, IISRegKeyMinorVersionValue, RegistryValueKind.DWord, out minorVersion))
                    {
                        if (minorVersion >= 5)
                        {
                            found = true;
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        #endregion

        #region IsIISComponentInstalled
        private static bool IsIISComponentInstalled(string subcomponent)
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, IISComponentRegKeyName, subcomponent, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsAspNetRegistered functions

        #region IsAspNet10Registered
        // TODO: Determine how to detect ASP.NET 1.0
        private static bool IsAspNet10Registered()
        {
            return false;
        }
        #endregion

        #region IsAspNet11Registered
        private static bool IsAspNet11Registered()
        {
            string regValue = string.Empty;
            return (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, NetRegKeyValue, RegistryValueKind.String, out regValue));
        }
        #endregion

        #region IsAspNet20Registered
        private static bool IsAspNet20Registered()
        {
            string regValue = string.Empty;
            return (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, NetRegKeyValue, RegistryValueKind.String, out regValue));
        }
        #endregion

        #region IsAspNet40Registered
        private static bool IsAspNet40Registered()
        {
            string regValue = string.Empty;
            return (GetRegistryValue(RegistryHive.LocalMachine, Netfx40RegKeyName, NetRegKeyValue, RegistryValueKind.String, out regValue));
        }
        #endregion

        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #endregion

        #region methods

        #region IsInstalled

        #region IsInstalled(InternetInformationServicesVersion iisVersion)
        /// <summary>
        /// Determines if the specified version of Internet Information 
        /// Services (IIS) is installed on the local computer.
        /// </summary>
        /// <param name="iisVersion">One of the 
        /// <see cref="InternetInformationServicesVersion"/> values.</param>
        /// <returns><see langword="true"/> if the specified Internet
        /// Information Services version is installed; otherwise
        /// <see langword="false"/>.</returns>
        public static bool IsInstalled(InternetInformationServicesVersion iisVersion)
        {
            bool ret = false;

            switch (iisVersion)
            {
                case InternetInformationServicesVersion.IIS4:
                    ret = IsIIS4Installed();
                    break;

                case InternetInformationServicesVersion.IIS5:
                    ret = IsIIS5Installed();
                    break;

                case InternetInformationServicesVersion.IIS51:
                    ret = IsIIS51Installed();
                    break;

                case InternetInformationServicesVersion.IIS6:
                    ret = IsIIS6Installed();
                    break;

                case InternetInformationServicesVersion.IIS7:
                    ret = IsIIS7Installed();
                    break;

                case InternetInformationServicesVersion.IIS75:
                    ret = IsIIS75Installed();
                    break;

                case InternetInformationServicesVersion.IIS8:
                    ret = IsIIS8Installed();
                    break;

                case InternetInformationServicesVersion.IIS85:
                    ret = IsIIS85Installed();
                    break;

                default:
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Determines if the specified version of Internet Information 
        /// Services (IIS) or higher is installed on the local computer.
        /// </summary>
        /// <param name="iisVersion">One of the 
        /// <see cref="InternetInformationServicesVersion"/> values.</param>
        /// <returns><see langword="true"/> if the specified Internet
        /// Information Services version is installed; otherwise
        /// <see langword="false"/>.</returns>
        public static bool IsEqualOrHigherInstalled(InternetInformationServicesVersion iisVersion)
        {
            bool ret = false;

            switch (iisVersion)
            {
                case InternetInformationServicesVersion.IIS4:
                    ret = IsIIS4OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS5:
                    ret = IsIIS5OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS51:
                    ret = IsIIS51OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS6:
                    ret = IsIIS6OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS7:
                    ret = IsIIS7OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS75:
                    ret = IsIIS75OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS8:
                    ret = IsIIS8OrHigherInstalled();
                    break;

                case InternetInformationServicesVersion.IIS85:
                    ret = IsIIS85OrHigherInstalled();
                    break;

                default:
                    break;
            }

            return ret;
        }
        #endregion

        #region IsInstalled(InternetInformationServicesComponent subcomponent)
        /// <summary>
        /// Determines if the specified Internet Information Services (IIS)
        /// subcomponent is installed on the local computer.
        /// </summary>
        /// <param name="subcomponent">One of the 
        /// <see cref="InternetInformationServicesComponent"/> values.</param>
        /// <returns><see langword="true"/> if the specified Internet
        /// Information Services subcomponent is installed; otherwise
        /// <see langword="false"/>.</returns>
        public static bool IsInstalled(InternetInformationServicesComponent subcomponent)
        {
            bool ret = false;

            switch (subcomponent)
            {
                case InternetInformationServicesComponent.Common:
                    ret = IsIISComponentInstalled("iis_common");
                    break;

                case InternetInformationServicesComponent.ASP:
                    ret = IsIISComponentInstalled("iis_asp");
                    break;

                case InternetInformationServicesComponent.FTP:
                    ret = IsIISComponentInstalled("iis_ftp");
                    break;

                case InternetInformationServicesComponent.InetMgr:
                    ret = IsIISComponentInstalled("iis_inetmgr");
                    break;

                case InternetInformationServicesComponent.InternetDataConnector:
                    ret = IsIISComponentInstalled("iis_internetdataconnector");
                    break;

                case InternetInformationServicesComponent.NNTP:
                    ret = IsIISComponentInstalled("iis_nntp");
                    break;

                case InternetInformationServicesComponent.ServerSideIncludes:
                    ret = IsIISComponentInstalled("iis_serversideincludes");
                    break;

                case InternetInformationServicesComponent.SMTP:
                    ret = IsIISComponentInstalled("iis_smtp");
                    break;

                case InternetInformationServicesComponent.WebDAV:
                    ret = IsIISComponentInstalled("iis_webdav");
                    break;

                case InternetInformationServicesComponent.WWW:
                    ret = IsIISComponentInstalled("iis_www");
                    break;

                case InternetInformationServicesComponent.RemoteAdmin:
                    ret = IsIISComponentInstalled("sakit_web");
                    break;

                case InternetInformationServicesComponent.BitsISAPI:
                    ret = IsIISComponentInstalled("BitsServerExtensionsISAPI");
                    break;

                case InternetInformationServicesComponent.Bits:
                    ret = IsIISComponentInstalled("BitsServerExtensionsManager");
                    break;

                case InternetInformationServicesComponent.FrontPageExtensions:
                    ret = IsIISComponentInstalled("fp_extensions");
                    break;

                case InternetInformationServicesComponent.InternetPrinting:
                    ret = IsIISComponentInstalled("inetprint ");
                    break;

                case InternetInformationServicesComponent.TSWebClient:
                    ret = IsIISComponentInstalled("TSWebClient ");
                    break;

                default:
                    break;
            }


            return ret;
        }
        #endregion

        #endregion

        #region IsAspRegistered
        /// <summary>
        /// Determines if ASP is registered with Internet Information
        /// Services (IIS) on the local computer.
        /// </summary>
        /// <returns><see langword="true"/> if ASP is registered; otherwise
        /// <see langword="false"/>.</returns>
        public static bool IsAspRegistered()
        {
            return IsInstalled(InternetInformationServicesComponent.ASP);
        } 
        #endregion

        #region IsAspNetRegistered
        /// <summary>
        /// Determines if the version of ASP.NET is registered with Internet
        /// Information Services (IIS) on the local computer.
        /// </summary>
        /// <param name="frameworkVersion">One of the
        /// <see cref="FrameworkVersion"/> values.</param>
        /// <returns><see langword="true"/> if the specified ASP.NET version
        /// is registered; otherwise <see langword="false"/>.</returns>
        public static bool IsAspNetRegistered(FrameworkVersion frameworkVersion)
        {
            bool ret = false;

            switch (frameworkVersion)
            {
                case FrameworkVersion.Fx10:
                    ret = IsAspNet10Registered();
                    break;

                case FrameworkVersion.Fx11:
                    ret = IsAspNet11Registered();
                    break;

                case FrameworkVersion.Fx20:
                case FrameworkVersion.Fx30:
                case FrameworkVersion.Fx35:
                    ret = IsAspNet20Registered();
                    break;

                case FrameworkVersion.Fx45:
                    ret = IsAspNet40Registered();
                    break;

                default:
                    break;
            }
            return ret;
        }
        #endregion

        #endregion

        #endregion
    }
    #endregion
}
