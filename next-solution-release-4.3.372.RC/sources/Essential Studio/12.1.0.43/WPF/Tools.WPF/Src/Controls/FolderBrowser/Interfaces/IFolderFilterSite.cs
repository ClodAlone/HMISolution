// <copyright file="IFolderFilterSite.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Syncfusion.Windows.Tools.Controls
{   
    /// <summary>
    /// Exported by a host to allow clients to specify how to filter a Shell folder enumeration.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("C0A651F5-B48B-11d2-B5ED-006097C686F6")]
    public interface IFolderFilterSite
    {
        /// <summary>
        /// Exposed by a host to allow clients to pass the host their IUnknown interface pointers.
        /// </summary>
        /// <param name="filterRef">A pointer to the client's IUnknown interface. To notify the host to terminate 
        /// filtering and stop calling your IFolderFilter interface, set this parameter to NULL.</param>
        /// <returns>int type filter</returns>
        [PreserveSig]
        int SetFilter(
            [MarshalAs(UnmanagedType.Interface)]
            Object filterRef);
    }
}
