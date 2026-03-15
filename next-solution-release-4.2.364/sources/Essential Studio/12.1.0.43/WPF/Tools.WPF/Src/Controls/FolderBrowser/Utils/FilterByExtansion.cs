// <copyright file="FilterByExtansion.cs" company="Syncfusion">
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
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Class for custom filtering.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    [ComVisible(true)]
    [Guid("3766C955-DA6F-4fbc-AD36-311E342EF180")]
    public class FilterByExtension : IFolderFilter
    {
        #region Private members
        /// <summary>
        /// Represents the array of valid extension
        /// </summary>
        private string[] m_validExtension;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the valid extension.
        /// </summary>
        /// <value>The valid extension.</value>
        public string[] ValidExtension
        {
            get
            {
                return m_validExtension;
            }

            set
            {
                m_validExtension = value;
            }
        }
        #endregion

        #region Interface implementation
        /// <summary>
        /// Allows a client to specify which individual items should be enumerated.
        /// Note: The host calls this method for each item in the folder. Return S_OK (0), to have the item enumerated. 
        /// Return S_FALSE (1) to prevent the item from being enumerated.
        /// </summary>
        /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
        /// <param name="pidlFolder">The folder's PIDL.</param>
        /// <param name="pidlItem">The item's PIDL.</param>
        /// <returns>int value of pidlItem</returns>
        public int ShouldShow(Object psf, IntPtr pidlFolder, IntPtr pidlItem)
        {
            IShellFolder isf = (IShellFolder)psf;
            Interop.Shell32.STRRET ptrDisplayName;
            string strDisplay;
            UInt32 iAttrib;

            ////get display name of item
            isf.GetDisplayNameOf(pidlItem, (uint)Interop.Shell32.SHGNO.SHGDN_NORMAL | (uint)Interop.Shell32.SHGNO.SHGDN_FORPARSING, out ptrDisplayName);
            Interop.Shell32.StrRetToBSTR(ref ptrDisplayName, (IntPtr)0, out strDisplay);

            //// check if item is file or folder
            IntPtr[] pidl = new IntPtr[1];
            pidl[0] = pidlItem;
            iAttrib = (uint)Interop.Shell32.SFGAO.SFGAO_FOLDER;
            isf.GetAttributesOf(1, pidl, ref iAttrib);

            //// if item is a folder, accept
            if ((iAttrib & (uint)Interop.Shell32.SFGAO.SFGAO_FOLDER) == (uint)Interop.Shell32.SFGAO.SFGAO_FOLDER)
            {
                return 0;
            }
            //// if item is file, check if it has a valid extension
            for (int i = 0; i < ValidExtension.Length; i++)
            {
                if (strDisplay.ToUpper().EndsWith("." + m_validExtension[i].ToUpper()))
                {
                    return 0;
                }
            }

            return 1;
        }
        
        /// <summary>
        /// Allows a client to specify which classes of objects in a Shell folder should be enumerated.
        /// </summary>
        /// <param name="psf">A pointer to the folder's IShellFolder interface.</param>
        /// <param name="pidlFolder">The folder's PIDL.</param>
        /// <param name="phwnd">A pointer to the host's window handle.</param>
        /// <param name="pgrfFlags">One or more SHCONTF values that specify which classes of objects to enumerate.</param>
        /// <returns>int type pgrfFlags</returns>
        //SA I78477
        [CLSCompliant(false)]
        //EA I78477
        public int GetEnumFlags(Object psf, IntPtr pidlFolder, IntPtr phwnd, out UInt32 pgrfFlags)
        {
            pgrfFlags = (uint)Interop.Shell32.SHCONTF.SHCONTF_FOLDERS | (uint)Interop.Shell32.SHCONTF.SHCONTF_NONFOLDERS;
            return 0;
        }
        #endregion
    }
}
