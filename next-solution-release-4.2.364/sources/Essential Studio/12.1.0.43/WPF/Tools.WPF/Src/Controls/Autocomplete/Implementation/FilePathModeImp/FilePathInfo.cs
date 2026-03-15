// <copyright file="FilePathInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This is static class which has some methods to work with
    /// local network point and local directory or files.
    /// </summary>
    /// <property name="flag" value="Finished"/>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal static class FilePathInfo
    {
        #region Constants

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Buffer size for resource list (MSDN: 16 kilobytes is typical)
        /// </summary>
        private const int EnumBufferSize = 16384;

        #endregion Constants

        #region Enums

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Returns state about how the operation was completed.
        /// </summary>
        private enum NERR
        {
            /// <summary>
            /// Represents Success
            /// </summary>
            Success = 0,

            /// <summary>
            /// Represents Access Denied.
            /// </summary>
            ACCESS_DENIED = 5,

            /// <summary>
            /// Represents Not enough memory
            /// </summary>
            NOT_ENOUGH_MEMORY = 8,

            /// <summary>
            /// Represents Bad Net Path
            /// </summary>
            BAD_NETPATH = 53,

            /// <summary>
            /// Represents NETWORK BUSY
            /// </summary>
            NETWORK_BUSY = 54,

            /// <summary>
            /// Represents INVALID PARAMETER
            /// </summary>
            INVALID_PARAMETER = 87,

            /// <summary>
            /// Represents INVALID LEVEL
            /// </summary>
            INVALID_LEVEL = 124,

            /// <summary>
            /// Represents MORE DATA
            /// </summary>
            MORE_DATA = 234,

            /// <summary>
            /// Represents NO MORE ITEMS
            /// </summary>
            NO_MORE_ITEMS = 259,

            /// <summary>
            /// Represents EXTENDED ERROR
            /// </summary>
            EXTENDED_ERROR = 1208,

            /// <summary>
            /// Represents NO NETWORK
            /// </summary>
            NO_NETWORK = 1222,

            /// <summary>
            /// Represents INVALID HANDLE STATE
            /// </summary>
            INVALID_HANDLE_STATE = 1609,

            /// <summary>
            /// Represents NO BROWSER SERVERS FOUND
            /// </summary>
            NO_BROWSER_SERVERS_FOUND = 6118,
        }

        #endregion Enums

        #region Private struct

        /// <summary>
        /// Represents the struct WKSTA_INFO_100
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct WKSTA_INFO_100
        {
            /// <summary>
            /// Represents the platform Id
            /// </summary>
            public int Wki100_platform_id;

            /// <summary>
            /// Represents the computer name
            /// </summary>
            public IntPtr Wki100_computername;

            /// <summary>
            /// Represents the langroup
            /// </summary>
            public IntPtr Wki100_langroup;

            /// <summary>
            /// Represents the Major Version
            /// </summary>
            public int Wki100_ver_major;

            /// <summary>
            /// Represents the Minor Version
            /// </summary>
            public int Wki100_ver_minor;
        }

        #endregion Private struct

        #region Implemenation

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets the list resource for set net point.
        /// </summary>
        /// <param name="parentResource">Parent resource.</param>
        /// <returns>
        /// List resource for set net point.
        /// </returns>
        internal static List<NetResource> GetResources(NetResource parentResource)
        {
            IntPtr ptrHandle = IntPtr.Zero;
            IntPtr ptrBuffer = IntPtr.Zero;
            List<NetResource> list = new List<NetResource>();

            try
            {
                NERR nerr;
                NetResource netResource;
                nerr = WNetOpenEnum(ResourceScope.GLOBALNET, ResourceType.ANY, ResourceUsage.ALL, parentResource, out ptrHandle);

                if (nerr != NERR.Success)
                {
                    return list;
                }

                ptrBuffer = Marshal.AllocHGlobal(EnumBufferSize);

                while (true)
                {
                    int entries = -1;
                    int size = EnumBufferSize;
                    nerr = WNetEnumResource(ptrHandle, ref entries, ptrBuffer, ref size);

                    if ((nerr != NERR.Success) || (entries < 1))
                    {
                        break;
                    }

                    for (int i = 0, ptr = ptrBuffer.ToInt32(); i < entries; i++, ptr += Marshal.SizeOf(netResource))
                    {
                        netResource = (NetResource)Marshal.PtrToStructure(new IntPtr(ptr), typeof(NetResource));
                        list.Add(netResource);
                    }
                }
            }
            finally
            {
                if (ptrBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrBuffer);
                }

                if (ptrHandle != IntPtr.Zero)
                {
                    WNetCloseEnum(ptrHandle);
                }
            }

            return list;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets items collection for current level.
        /// </summary>
        /// <param name="currentLevel">The current position in the
        /// auto complete tree.</param>
        /// <returns>
        /// The item collection for this level.
        /// </returns>
        internal static AutocompleteItemCollection GetDirectoryList(FilePathLevel currentLevel)
        {
            string path = currentLevel.GetFullPath();
            DirectoryInfo dInfo = new DirectoryInfo(path);
            AutocompleteItemCollection items = new AutocompleteItemCollection();

            if (dInfo.Exists)
            {
                try
                {
                    DirectoryInfo[] dInfos = dInfo.GetDirectories();
                    FileInfo[] fInfos = dInfo.GetFiles();

                    for (int i = 0, cnt = dInfos.Length; i < cnt; ++i)
                    {
                        items.Add(new FilePathLevel(currentLevel, dInfos[i].Name));
                    }

                    for (int i = 0, cnt = fInfos.Length; i < cnt; ++i)
                    {
                        items.Add(new FilePathItem(fInfos[i].Name));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.Print(ex.Message);
                }
                catch (IOException ex)
                {
                    Debug.Print(ex.Message);
                }
            }

            return items;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method gets domain's list for current network.
        /// </summary>
        /// <param name="parentResource">The parent resource.</param>
        /// <param name="list">The list of all previous parent
        /// NetResources </param>
        /// <returns>
        /// The list of domains to current local network.
        /// </returns>
        internal static List<NetResource> GetDomain(NetResource parentResource, List<NetResource> list)
        {
            IntPtr ptrHandle = IntPtr.Zero;
            IntPtr ptrBuffer = IntPtr.Zero;

            try
            {
                NERR nerr;
                NetResource netResource;
                nerr = WNetOpenEnum(ResourceScope.GLOBALNET, ResourceType.ANY, ResourceUsage.ALL, parentResource, out ptrHandle);

                if (nerr != NERR.Success)
                {
                    return list;
                }

                ptrBuffer = Marshal.AllocHGlobal(EnumBufferSize);

                while (true)
                {
                    int entries = -1;
                    int size = EnumBufferSize;
                    nerr = WNetEnumResource(ptrHandle, ref entries, ptrBuffer, ref size);

                    if ((nerr != NERR.Success) || (entries < 1))
                    {
                        break;
                    }

                    for (int i = 0, ptr = ptrBuffer.ToInt32(); i < entries; i++, ptr += Marshal.SizeOf(netResource))
                    {
                        netResource = (NetResource)Marshal.PtrToStructure(new IntPtr(ptr), typeof(NetResource));

                        if (ResourceDisplayType.DOMAIN == netResource.DisplayType)
                        {
                            list.Add(netResource);
                        }
                        else if ((netResource.Usage & ResourceUsage.CONTAINER) == ResourceUsage.CONTAINER)
                        {
                            list = FilePathInfo.GetDomain(netResource, list);
                        }
                    }
                }
            }
            finally
            {
                if (ptrBuffer != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrBuffer);
                }

                if (ptrHandle != IntPtr.Zero)
                {
                    WNetCloseEnum(ptrHandle);
                }
            }

            return list;
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// Gets current work group for this machine.
        /// </summary>
        /// <returns>
        /// This method returns current work group for this machine.
        /// </returns>
        internal static string GetCurrentWorkGroup()
        {
            IntPtr bufptr;
            NetWkstaGetInfo(null, 100, out bufptr);
            WKSTA_INFO_100 info = (WKSTA_INFO_100)Marshal.PtrToStructure(bufptr, typeof(WKSTA_INFO_100));
            NetApiBufferFree(bufptr);

            return Marshal.PtrToStringUni(info.Wki100_langroup);
        }

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method starts an enumeration of network resources or
        /// existing connections.
        /// </summary>
        /// <param name="dwScope">Scope of the enumeration. This
        /// parameter can be one of the
        /// following values.</param>
        /// <param name="dwType">Resource types to be enumerated.
        /// This parameter can be a
        /// combination of the following values.</param>
        /// <param name="dwUsage">Resource usage type to be
        /// enumerated. This parameter can
        /// be a combination of the
        /// following values. </param>
        /// <param name="lpNetResource">Pointer to a NETRESOURCE
        /// structure that specifies the
        /// container to enumerate. If the
        /// dwScope parameter is not
        /// RESOURCE_GLOBALNET, this
        /// parameter must be NULL.</param>
        /// <param name="lphEnum">Pointer to an enumeration handle
        /// that can be used in a subsequent
        /// call to WNetEnumResource. </param>
        /// <returns>
        /// If the function succeeds, the return value is NO_ERROR.
        /// </returns>
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern NERR WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, [In, MarshalAs(UnmanagedType.AsAny)] object lpNetResource, out IntPtr lphEnum);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method continues an enumeration of network resources
        /// that was started by a call to the WNetOpenEnum function.
        /// </summary>
        /// <param name="hEnum">Handle that identifies an
        /// enumeration instance. This handle
        /// must be returned by the
        /// WNetOpenEnum function.</param>
        /// <param name="lpcCount">Pointer to a variable specifying
        /// the number of entries requested.
        /// If the number requested is 1,
        /// the function returns as many
        /// entries as possible.<para></para>If
        /// the function succeeds, on return
        /// the variable pointed to by this
        /// parameter contains the number of
        /// entries actually read.</param>
        /// <param name="lpBuffer">Pointer to the buffer that
        /// receives the enumeration results.
        /// The results are returned as an
        /// array of NETRESOURCE structures.
        /// Note that the buffer you
        /// allocate must be large enough to
        /// hold the structures, plus the
        /// strings to which their members
        /// point. For more information, see
        /// the following Remarks section.<para></para>The
        /// buffer is valid until the next
        /// call using the handle specified
        /// by the hEnum parameter. The order
        /// of NETRESOURCE structures in the
        /// array is not predictable.</param>
        /// <param name="lpBufferSize">Pointer to a variable that
        /// specifies the size of the
        /// lpBuffer parameter, in bytes. If
        /// the buffer is too small to
        /// receive even one entry, this
        /// parameter receives the required
        /// size of the buffer. </param>
        /// <returns>
        /// This function does return correct values.
        /// </returns>
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern NERR WNetEnumResource(IntPtr hEnum, ref int lpcCount, IntPtr lpBuffer, ref int lpBufferSize);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method ends a network resource enumeration started by a
        /// call to the WNetOpenEnum function.
        /// </summary>
        /// <param name="hEnum">Handle to an enumeration instance. This
        /// handle must be returned by the
        /// WNetOpenEnum function.</param>
        /// <returns>
        /// This function does return correct values.
        /// </returns>
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern NERR WNetCloseEnum(IntPtr hEnum);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method returns information about the configuration of a
        /// workstation.
        /// </summary>
        /// <param name="servername">Pointer to a string that specifies
        /// the DNS or NetBIOS name of the
        /// remote server on which the function
        /// is to execute. If this parameter is
        /// NULL, the local computer is used.</param>
        /// <param name="level">Specifies the information level of
        /// the data. This parameter can be one
        /// of the following values. </param>
        /// <param name="bufptr">Pointer to the buffer that receives
        /// the data. The format of this data
        /// depends on the value of the level
        /// parameter. This buffer is allocated
        /// by the system and must be freed
        /// using the NetApiBufferFree
        /// function.</param>
        /// <returns>
        /// If the function succeeds, the return value is NERR.
        /// </returns>
        [DllImport("Netapi32.dll")]
        private static extern int NetWkstaGetInfo(string servername, int level, out IntPtr bufptr);

        /// <property name="flag" value="Finished" />
        /// <summary>
        /// This method frees the memory that the NetApiBufferAllocate
        /// function allocates.
        /// </summary>
        /// <param name="bufptr">Pointer to a buffer returned previously
        /// by another network management function.</param>
        /// <returns>
        /// If the function succeeds, the return value is NERR_Success. If
        /// the function fails, the return value is a system error code.
        /// For a list of error codes
        /// </returns>
        [DllImport("Netapi32.dll")]
        private static extern int NetApiBufferFree(IntPtr bufptr);

        #endregion Implemenation
    }
}