// <copyright file="IMalloc.cs" company="Syncfusion">
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
    /// C# representation of the IMalloc interface.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000002-0000-0000-C000-000000000046")]
    public interface IMalloc
    {
        /// <summary>
        /// Allocates the specified cb.
        /// </summary>
        /// <param name="cb">The cb int type.</param>
        /// <returns>int value type</returns>
        [PreserveSig]
        IntPtr Alloc([In] int cb);
        
        /// <summary>
        /// Reallocates the specified pv.
        /// </summary>
        /// <param name="pv">The pv IntPtr.</param>
        /// <param name="cb">The cb IntPtr.</param>
        /// <returns>IntPtr type </returns>
        [PreserveSig]
        IntPtr Realloc([In] IntPtr pv, [In] int cb);
        
        /// <summary>
        /// Frees the specified pv.
        /// </summary>
        /// <param name="pv">The pv IntPtr.</param>
        [PreserveSig]
        void Free([In] IntPtr pv);
        
        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <param name="pv">The pv IntPtr.</param>
        /// <returns>int value type</returns>
        [PreserveSig]
        int GetSize([In] IntPtr pv);
        
        /// <summary>
        /// Dids the alloc.
        /// </summary>
        /// <param name="pv">The pv IntPtr.</param>
        /// <returns>int value type</returns>
        [PreserveSig]
        int DidAlloc(IntPtr pv);
        
        /// <summary>
        /// Heaps the minimize.
        /// </summary>
        [PreserveSig]
        void HeapMinimize();
    }
}
