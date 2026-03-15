// <copyright file="IFolderFilter.cs" company="Syncfusion">
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
    /// IFolder Filter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("9CC22886-DC8E-11d2-B1D0-00C04F8EEB3E")]
    [CLSCompliant(false)]
    public interface IFolderFilter
    {
        /// <summary>
        /// Allows a client to specify which individual items should be enumerated.
        /// Note: The host calls this method for each item in the folder.         
        /// </summary>
        /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
        /// <param name="pidlFolder">The folder's PIDL.</param>
        /// <param name="pidlItem">The item's PIDL.</param>
        /// <returns>Return S_OK (0), to have the item enumerated.      
        /// Return S_FALSE (1) to prevent the item from being enumerated.</returns>
        [PreserveSig]
        int ShouldShow(
            [MarshalAs(UnmanagedType.Interface)]Object psf,
            IntPtr pidlFolder,
            IntPtr pidlItem);

        /// <summary>
        /// Allows a client to specify which classes of objects in a Shell folder should be enumerated.
        /// </summary>
        /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
        /// <param name="pidlFolder">The folder's PIDL.</param>
        /// <param name="phwnd">A pointer to the host's window handle.</param>
        /// <param name="pgrfFlags">One or more SHCONTF values that specify which classes of objects to enumerate.</param>
        /// <returns> int type IntrPtr</returns>
        [PreserveSig]
        int GetEnumFlags(
            [MarshalAs(UnmanagedType.Interface)]Object psf,
            IntPtr pidlFolder,
            IntPtr phwnd,
            out UInt32 pgrfFlags);
    }
}
