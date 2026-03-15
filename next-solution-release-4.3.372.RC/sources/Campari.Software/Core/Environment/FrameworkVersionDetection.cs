// ---------------------------------------------------------------------------
// Campari Software
//
// FrameworkVersionDetection.cs
//
// For more information on, look at:
//
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnnetdep/html/dotnetfxref.asp
// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnnetdep/html/redistdeploy1_1.asp
// http://blogs.msdn.com/astebner/archive/2004/09/14/229802.aspx
// http://blogs.msdn.com/astebner/archive/2004/09/14/229574.aspx
// http://blogs.msdn.com/astebner/archive/2006/11/09/clarification-about-net-framework-3-0-detection-registry-key-on-a-64-bit-os.aspx
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
    #region class FrameworkVersionDetection
    /// <summary>
    /// Provides support for determining if a specific version of the .NET
    /// Framework runtime is installed and the service pack level for the
    /// runtime version.
    /// </summary>
    public static class FrameworkVersionDetection
    {
        #region events

        #endregion

        #region class-wide fields

        const string Netfx10RegKeyName = "Software\\Microsoft\\.NETFramework\\Policy\\v1.0";
        const string Netfx10RegKeyValue = "3705";
        const string Netfx10SPxMSIRegKeyName = "Software\\Microsoft\\Active Setup\\Installed Components\\{78705f0d-e8db-4b2d-8193-982bdda15ecd}";
        const string Netfx10SPxOCMRegKeyName = "Software\\Microsoft\\Active Setup\\Installed Components\\{FDC11A6F-17D1-48f9-9EA3-9051954BAA24}";
        const string Netfx10SPxRegValueName = "Version";
        const string Netfx11RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v1.1.4322";
        const string Netfx20RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v2.0.50727";
        const string Netfx30RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.0\\Setup";
        const string Netfx35RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v3.5";
        const string Netfx45RegKeyName = "Software\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full";
        const string Netfx11PlusRegValueName = "Install";
        const string Netfx30PlusRegValueName = "InstallSuccess";
        const string Netfx11PlusSPxRegValueName = "SP";
        const string Netfx20PlusBuildRegValueName = "Increment";
        const string Netfx30PlusVersionRegValueName = "Version";
        const string Netfx35PlusBuildRegValueName = "Build";
        const string Netfx45PlusRegValueName = "Install";
        const string Netfx45PlusVersionRegValueName = "Version";
        const string Netfx30PlusWCFRegKeyName = Netfx30RegKeyName + "\\Windows Communication Foundation";
        const string Netfx30PlusWPFRegKeyName = Netfx30RegKeyName + "\\Windows Presentation Foundation";
        const string Netfx30PlusWFRegKeyName = Netfx30RegKeyName + "\\Windows Workflow Foundation";
        const string Netfx30PlusWFPlusVersionRegValueName = "FileVersion";
        const string CardSpaceServicesRegKeyName = "System\\CurrentControlSet\\Services\\idsvc";
        const string CardSpaceServicesPlusImagePathRegName = "ImagePath";

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
                    }
                }
            }
            return success;
        }
        #endregion

        #region IsNetfxInstalled functions

        #region IsNetfx10Installed
        private static bool IsNetfx10Installed()
        {
            string regValue = string.Empty;
            return (GetRegistryValue(RegistryHive.LocalMachine, Netfx10RegKeyName, Netfx10RegKeyValue, RegistryValueKind.String, out regValue));
        }
        #endregion

        #region IsNetfx11Installed
        private static bool IsNetfx11Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsNetfx20Installed
        private static bool IsNetfx20Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsNetfx30Installed
        private static bool IsNetfx30Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsNetfx35Installed
        private static bool IsNetfx35Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region IsNetfx45Installed
        private static bool IsNetfx45Installed()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx45RegKeyName, Netfx45PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #endregion

        #region GetNetfxSPLevel functions

        #region GetNetfx10SPLevel
        private static int GetNetfx10SPLevel()
        {
            bool foundKey = false;
            int servicePackLevel = -1;
            string regValue;

            if (IsTabletOrMediaCenter())
            {
                foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxOCMRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
            }
            else
            {
                foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxMSIRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
            }

            if (foundKey)
            {
                // This registry value should be of the format
                // #,#,#####,# where the last # is the SP level
                // Try to parse off the last # here
                int index = regValue.LastIndexOf(',');
                if (index > 0)
                {
                    Int32.TryParse(regValue.Substring(index + 1), out servicePackLevel);
                }
            }

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx11SPLevel
        private static int GetNetfx11SPLevel()
        {
            int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            {
                servicePackLevel = regValue;
            }

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx20SPLevel
        private static int GetNetfx20SPLevel()
        {
            int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            {
                servicePackLevel = regValue;
            }

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx30SPLevel
        // This code is MOST LIKELY correct but will need to be verified.
        //
        // Currently, there are no service packs available for version 3.0 of 
        // the framework, so we always return -1. When a service pack does
        // become available, this method will need to be revised to correctly
        // determine the service pack level.
        private static int GetNetfx30SPLevel()
        {
            //int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            //{
            //    servicePackLevel = regValue;
            //}

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx35SPLevel
        private static int GetNetfx35SPLevel()
        {
            int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            {
                servicePackLevel = regValue;
            }

            return servicePackLevel;
        }
        #endregion

        #endregion

        #region GetNetfxExactVersion functions

        #region GetNetfx10ExactVersion
        private static Version GetNetfx10ExactVersion()
        {
            bool foundKey = false;
            Version fxVersion = new Version();
            string regValue;

            if (IsTabletOrMediaCenter())
            {
                foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxOCMRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
            }
            else
            {
                foundKey = GetRegistryValue(RegistryHive.LocalMachine, Netfx10SPxMSIRegKeyName, Netfx10SPxRegValueName, RegistryValueKind.String, out regValue);
            }

            if (foundKey)
            {
                // This registry value should be of the format
                // #,#,#####,# where the last # is the SP level
                // Try to parse off the last # here
                int index = regValue.LastIndexOf(',');
                if (index > 0)
                {
                    string[] tokens = regValue.Substring(0, index).Split(',');
                    if (tokens.Length == 3)
                    {
                        fxVersion = new Version(Convert.ToInt32(tokens[0], NumberFormatInfo.InvariantInfo), Convert.ToInt32(tokens[1], NumberFormatInfo.InvariantInfo), Convert.ToInt32(tokens[2], NumberFormatInfo.InvariantInfo));
                    }
                }
            }

            return fxVersion;
        }
        #endregion

        #region GetNetfx11ExactVersion
        private static Version GetNetfx11ExactVersion()
        {
            int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx11RegKeyName, Netfx11PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    // In the strict sense, we are cheating here, but the registry key name itself
                    // contains the version number.
                    string[] tokens = Netfx11RegKeyName.Split(new string[] { "NDP\\v" }, StringSplitOptions.None);
                    if (tokens.Length == 2)
                    {
                        fxVersion = new Version(tokens[1]);
                    }
                }
            }

            return fxVersion;
        }
        #endregion

        #region GetNetfx20ExactVersion
        private static Version GetNetfx20ExactVersion()
        {
            string regValue = String.Empty;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx20RegKeyName, Netfx20PlusBuildRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    // In the strict sense, we are cheating here, but the registry key name itself
                    // contains the version number.
                    string[] versionTokens = Netfx20RegKeyName.Split(new string[] { "NDP\\v" }, StringSplitOptions.None);
                    if (versionTokens.Length == 2)
                    {
                        string[] tokens = versionTokens[1].Split('.');
                        if (tokens.Length == 3)
                        {
                            fxVersion = new Version(Convert.ToInt32(tokens[0], NumberFormatInfo.InvariantInfo), Convert.ToInt32(tokens[1], NumberFormatInfo.InvariantInfo), Convert.ToInt32(tokens[2], NumberFormatInfo.InvariantInfo), Convert.ToInt32(regValue, NumberFormatInfo.InvariantInfo));
                        }
                    }
                }
            }

            return fxVersion;
        }
        #endregion

        #region GetNetfx30ExactVersion
        private static Version GetNetfx30ExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30RegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #region GetNetfx35ExactVersion
        private static Version GetNetfx35ExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx35RegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #region GetNetfx45ExactVersion
        private static Version GetNetfx45ExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx45RegKeyName, Netfx45PlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #region WindowsFounationLibrary functions

        #region CardSpace

        #region IsNetfx30CardSpaceInstalled
        private static bool IsNetfx30CardSpaceInstalled()
        {
            bool found = false;
            string regValue = String.Empty;

            if (GetRegistryValue(RegistryHive.LocalMachine, CardSpaceServicesRegKeyName, CardSpaceServicesPlusImagePathRegName, RegistryValueKind.ExpandString, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region GetNetfx30CardSpaceSPLevel
        // Currently, there are no service packs available for version 3.0 of 
        // the framework, so we always return -1. When a service pack does
        // become available, this method will need to be revised to correctly
        // determine the service pack level. Based on the current method for
        // determining if CardSpace is installed, it may not be possible to
        // correctly determine the Service Pack level for CardSpace.
        private static int GetNetfx30CardSpaceSPLevel()
        {
            int servicePackLevel = -1;
            return servicePackLevel;
        }
        #endregion

        #region GetNetfx30CardSpaceExactVersion
        private static Version GetNetfx30CardSpaceExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, CardSpaceServicesRegKeyName, CardSpaceServicesPlusImagePathRegName, RegistryValueKind.ExpandString, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(regValue.Trim('"'));
                    int index = fileVersionInfo.FileVersion.IndexOf(' ');
                    fxVersion = new Version(fileVersionInfo.FileVersion.Substring(0, index));
                }
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #region Windows Communication Foundation

        #region IsNetfx30WCFInstalled
        private static bool IsNetfx30WCFInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region GetNetfx30WCFSPLevel
        // This code is MOST LIKELY correct but will need to be verified.
        //
        // Currently, there are no service packs available for version 3.0 of 
        // the framework, so we always return -1. When a service pack does
        // become available, this method will need to be revised to correctly
        // determine the service pack level.
        private static int GetNetfx30WCFSPLevel()
        {
            //int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            //{
            //    servicePackLevel = regValue;
            //}

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx30WCFExactVersion
        private static Version GetNetfx30WCFExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWCFRegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #region Windows Presentation Foundation

        #region IsNetfx30WPFInstalled
        private static bool IsNetfx30WPFInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region GetNetfx30WPFSPLevel
        // This code is MOST LIKELY correct but will need to be verified.
        //
        // Currently, there are no service packs available for version 3.0 of 
        // the framework, so we always return -1. When a service pack does
        // become available, this method will need to be revised to correctly
        // determine the service pack level.
        private static int GetNetfx30WPFSPLevel()
        {
            //int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            //{
            //    servicePackLevel = regValue;
            //}

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx30WPFExactVersion
        private static Version GetNetfx30WPFExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWPFRegKeyName, Netfx30PlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #region Windows Workflow Foundation

        #region IsNetfx30WFInstalled
        private static bool IsNetfx30WFInstalled()
        {
            bool found = false;
            int regValue = 0;

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx30PlusRegValueName, RegistryValueKind.DWord, out regValue))
            {
                if (regValue == 1)
                {
                    found = true;
                }
            }

            return found;
        }
        #endregion

        #region GetNetfx30WFSPLevel
        // This code is MOST LIKELY correct but will need to be verified.
        //
        // Currently, there are no service packs available for version 3.0 of 
        // the framework, so we always return -1. When a service pack does
        // become available, this method will need to be revised to correctly
        // determine the service pack level.
        private static int GetNetfx30WFSPLevel()
        {
            //int regValue = 0;

            // We can only get -1 if the .NET Framework is not
            // installed or there was some kind of error retrieving
            // the data from the registry
            int servicePackLevel = -1;

            //if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx11PlusSPxRegValueName, RegistryValueKind.DWord, out regValue))
            //{
            //    servicePackLevel = regValue;
            //}

            return servicePackLevel;
        }
        #endregion

        #region GetNetfx30WFExactVersion
        private static Version GetNetfx30WFExactVersion()
        {
            string regValue = String.Empty;

            // We can only get the default version if the .NET Framework
            // is not installed or there was some kind of error retrieving
            // the data from the registry
            Version fxVersion = new Version();

            if (GetRegistryValue(RegistryHive.LocalMachine, Netfx30PlusWFRegKeyName, Netfx30PlusWFPlusVersionRegValueName, RegistryValueKind.String, out regValue))
            {
                if (!String.IsNullOrEmpty(regValue))
                {
                    fxVersion = new Version(regValue);
                }
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #endregion

        #region IsTabletOrMediaCenter
        private static bool IsTabletOrMediaCenter()
        {
            return ((SafeNativeMethods.GetSystemMetrics(SystemMetric.SM_TABLETPC) != 0) || (SafeNativeMethods.GetSystemMetrics(SystemMetric.SM_MEDIACENTER) != 0));
        }
        #endregion

        #endregion

        #endregion

        #region public properties and methods

        #region properties

        #endregion

        #region methods

        #region IsInstalled

        #region IsInstalled(FrameworkVersion frameworkVersion)
        /// <summary>
        /// Determines if the specified .NET Framework version is installed
        /// on the local computer.
        /// </summary>
        /// <param name="frameworkVersion">One of the
        /// <see cref="FrameworkVersion"/> values.</param>
        /// <returns><see langword="true"/> if the specified .NET Framework
        /// version is installed; otherwise <see langword="false"/>.</returns>
        public static bool IsInstalled(FrameworkVersion frameworkVersion)
        {
            bool ret = false;

            switch (frameworkVersion)
            {
                case FrameworkVersion.Fx10:
                    ret = IsNetfx10Installed();
                    break;

                case FrameworkVersion.Fx11:
                    ret = IsNetfx11Installed();
                    break;

                case FrameworkVersion.Fx20:
                    ret = IsNetfx20Installed();
                    break;

                case FrameworkVersion.Fx30:
                    ret = IsNetfx30Installed();
                    break;

                case FrameworkVersion.Fx35:
                    ret = IsNetfx35Installed();
                    break;

                case FrameworkVersion.Fx45:
                    ret = IsNetfx45Installed();
                    break;

                default:
                    break;
            }

            return ret;
        }
        #endregion

        #region IsInstalled(WindowsFoundationLibrary foundationLibrary)
        /// <summary>
        /// Determines if the specified .NET Framework Foundation Library is
        /// installed on the local computer.
        /// </summary>
        /// <param name="foundationLibrary">One of the
        /// <see cref="WindowsFoundationLibrary"/> values.</param>
        /// <returns><see langword="true"/> if the specified .NET Framework
        /// Foundation Library is installed; otherwise <see langword="false"/>.</returns>
        public static bool IsInstalled(WindowsFoundationLibrary foundationLibrary)
        {
            bool ret = false;

            switch (foundationLibrary)
            {
                case WindowsFoundationLibrary.CardSpace:
                    ret = IsNetfx30CardSpaceInstalled();
                    break;

                case WindowsFoundationLibrary.WCF:
                    ret = IsNetfx30WCFInstalled();
                    break;

                case WindowsFoundationLibrary.WF:
                    ret = IsNetfx30WFInstalled();
                    break;

                case WindowsFoundationLibrary.WPF:
                    ret = IsNetfx30WPFInstalled();
                    break;

                default:
                    break;
            }

            return ret;
        }
        #endregion

        #endregion

        #region GetServicePackLevel

        #region GetServicePackLevel(FrameworkVersion frameworkVersion)
        /// <summary>
        /// Retrieves the service pack level for the specified .NET Framework
        /// version.
        /// </summary>
        /// <param name="frameworkVersion">One of the
        /// <see cref="FrameworkVersion"/> values.</param>
        /// <returns>An <see cref="Int32">integer</see> value representing
        /// the service pack level for the specified .NET Framework version. If
        /// the specified .NET Frameowrk version is not found, -1 is returned.
        /// </returns>
        public static int GetServicePackLevel(FrameworkVersion frameworkVersion)
        {
            int servicePackLevel = -1;

            switch (frameworkVersion)
            {
                case FrameworkVersion.Fx10:
                    servicePackLevel = GetNetfx10SPLevel();
                    break;

                case FrameworkVersion.Fx11:
                    servicePackLevel = GetNetfx11SPLevel();
                    break;

                case FrameworkVersion.Fx20:
                    servicePackLevel = GetNetfx20SPLevel();
                    break;

                case FrameworkVersion.Fx30:
                    servicePackLevel = GetNetfx30SPLevel();
                    break;

                case FrameworkVersion.Fx35:
                    servicePackLevel = GetNetfx35SPLevel();
                    break;

                case FrameworkVersion.Fx45:
                    throw new NotImplementedException();

                default:
                    break;
            }

            return servicePackLevel;
        }
        #endregion

        #region GetServicePackLevel(WindowsFoundationLibrary foundationLibrary)
        /// <summary>
        /// Retrieves the service pack level for the specified .NET Framework
        /// Foundation Library.
        /// </summary>
        /// <param name="foundationLibrary">One of the
        /// <see cref="WindowsFoundationLibrary"/> values.</param>
        /// <returns>An <see cref="Int32">integer</see> value representing
        /// the service pack level for the specified .NET Framework Foundation
        /// Library. If the specified .NET Frameowrk Foundation Library is not
        /// found, -1 is returned.
        /// </returns>
        public static int GetServicePackLevel(WindowsFoundationLibrary foundationLibrary)
        {
            int servicePackLevel = -1;

            switch (foundationLibrary)
            {
                case WindowsFoundationLibrary.CardSpace:
                    servicePackLevel = GetNetfx30CardSpaceSPLevel();
                    break;

                case WindowsFoundationLibrary.WCF:
                    servicePackLevel = GetNetfx30WCFSPLevel();
                    break;

                case WindowsFoundationLibrary.WF:
                    servicePackLevel = GetNetfx30WFSPLevel();
                    break;

                case WindowsFoundationLibrary.WPF:
                    servicePackLevel = GetNetfx30WPFSPLevel();
                    break;

                default:
                    break;
            }

            return servicePackLevel;
        }
        #endregion

        #endregion

        #region GetExactVersion

        #region GetExactVersion(FrameworkVersion frameworkVersion)
        /// <summary>
        /// Retrieves the exact version number for the specified .NET Framework
        /// version.
        /// </summary>
        /// <param name="frameworkVersion">One of the
        /// <see cref="FrameworkVersion"/> values.</param>
        /// <returns>A <see cref="Version">version</see> representing
        /// the exact version number for the specified .NET Framework version.
        /// If the specified .NET Frameowrk version is not found, a 
        /// <see cref="Version"/> is returned that represents a 0.0.0.0 version
        /// number.
        /// </returns>
        public static Version GetExactVersion(FrameworkVersion frameworkVersion)
        {
            Version fxVersion = new Version();

            switch (frameworkVersion)
            {
                case FrameworkVersion.Fx10:
                    fxVersion = GetNetfx10ExactVersion();
                    break;

                case FrameworkVersion.Fx11:
                    fxVersion = GetNetfx11ExactVersion();
                    break;

                case FrameworkVersion.Fx20:
                    fxVersion = GetNetfx20ExactVersion();
                    break;

                case FrameworkVersion.Fx30:
                    fxVersion = GetNetfx30ExactVersion();
                    break;

                case FrameworkVersion.Fx35:
                    fxVersion = GetNetfx35ExactVersion();
                    break;

                case FrameworkVersion.Fx45:
                    fxVersion = GetNetfx45ExactVersion();
                    break;

                default:
                    break;
            }

            return fxVersion;
        }
        #endregion

        #region GetExactVersion(WindowsFoundationLibrary foundationLibrary)
        /// <summary>
        /// Retrieves the exact version number for the specified .NET Framework
        /// Foundation Library.
        /// </summary>
        /// <param name="foundationLibrary">One of the
        /// <see cref="WindowsFoundationLibrary"/> values.</param>
        /// <returns>A <see cref="Version">version</see> representing
        /// the exact version number for the specified .NET Framework Foundation
        /// Library. If the specified .NET Frameowrk Foundation Library is not
        /// found, a <see cref="Version"/> is returned that represents a 
        /// 0.0.0.0 version number.
        /// </returns>
        public static Version GetExactVersion(WindowsFoundationLibrary foundationLibrary)
        {
            Version fxVersion = new Version();

            switch (foundationLibrary)
            {
                case WindowsFoundationLibrary.CardSpace:
                    fxVersion = GetNetfx30CardSpaceExactVersion();
                    break;

                case WindowsFoundationLibrary.WCF:
                    fxVersion = GetNetfx30WCFExactVersion();
                    break;

                case WindowsFoundationLibrary.WF:
                    fxVersion = GetNetfx30WFExactVersion();
                    break;

                case WindowsFoundationLibrary.WPF:
                    fxVersion = GetNetfx30WPFExactVersion();
                    break;

                default:
                    break;
            }

            return fxVersion;
        }
        #endregion

        #endregion

        #endregion

        #endregion
    }
    #endregion
}
