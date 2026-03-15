using System;
using System.Collections.Generic;
using System.Text;
using IIS7Manager.Exceptions;
using Microsoft.Web.Administration;

namespace IIS7Manager
{
    /// <summary>
    /// This class represent a IIS virtual directory.
    /// </summary>
    public class IISWebVirturalDir
    {
        // Directory entry for this dir
        private VirtualDirectory _entry = null;

        /// <summary>
        /// Internal this constructor so that user of this dll can not create a instance of IISWebVirtualDir directly.
        /// To get a instance of IISWebVirtualDir, please use IISWebVirtualDir.OpenSubVirtualDir or IISWebVirtualDir.CreateSubVirtualDir.
        /// </summary>
        /// <param name="entry"></param>
        internal IISWebVirturalDir(VirtualDirectory entry)
        {
            this._entry = entry;
        }


        #region Properties

        /// <summary>
        /// Get or set the physical path of this virtual directory
        /// </summary>
        public string PhysicalPath
        {
            get
            {
                return this._entry.PhysicalPath;
            }
            set
            {
                this._entry.PhysicalPath = value;
            }
        }

        /// <summary>
        /// Get name of this virtual path.
        /// </summary>
        public string Path
        {
            get
            {
                return this._entry.Path;
            }
            set
            {
                this._entry.Path = value;
            }
        }

        #endregion
    }
}
