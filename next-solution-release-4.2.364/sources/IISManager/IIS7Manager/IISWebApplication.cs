using System;
using System.Collections.Generic;
using System.Text;
using IIS7Manager.Exceptions;
using System.Linq;
using Microsoft.Web.Administration;

namespace IIS7Manager
{
    /// <summary>
    /// This class represent a IIS Application.
    /// </summary>
    public class IISWebApplication
    {
        // Directory entry for this dir
        private Application _entry = null;

        /// <summary>
        /// Internal this constructor so that user of this dll can not create a instance of IISWebApplication directly.
        /// </summary>
        /// <param name="entry"></param>
        internal IISWebApplication(Application entry)
        {
            this._entry = entry;
        }

        #region Properties

        /// <summary>
        /// Get or set the path of this virtual directory
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

        /// <summary>
        /// Get or set the physical path of this virtual directory
        /// </summary>
        public string PhisicalPath
        {
            get
            {
                if (this._entry.VirtualDirectories.Count > 0)
                    return this._entry.VirtualDirectories[0].PhysicalPath;
                
                return String.Empty;
            }
            set
            {
                if (this._entry.VirtualDirectories.Count > 0)
                    this._entry.VirtualDirectories[0].PhysicalPath = value;
            }
        }

        /// <summary>
        /// Get or set the Application Pool Name of this virtual directory
        /// </summary>
        public string ApplicationPoolName
        {
            get
            {
                return this._entry.ApplicationPoolName;
            }
            set
            {
                this._entry.ApplicationPoolName = value;
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Check whether a virtual directory exists.
        /// </summary>
        /// <param name="name">Name of dir checked</param>
        /// <returns>true if exist. Otherwise false.</returns>
        public bool ExistVirtualDir(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            return _entry.VirtualDirectories.Where(c => c.Path == name).SingleOrDefault() != null;
        }

        /// <summary>
        /// Create a virtual directory
        /// </summary>
        /// <param name="name">Name of the virtual directory</param>
        /// <param name="path">Path of the virtual directory</param>
        /// <returns>A IISWebVirtualDir instance if succeed. Otherwise false.</returns>

        public IISWebVirturalDir CreateSubVirtualDir(string name, string path)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            // already exist
            if (this.ExistVirtualDir(name))
            {
                throw new VirtualDirAlreadyExistException(OpenSubVirtualDir(name), path);
            }

            // validate path
            if (System.IO.Directory.Exists(path) == false)
            {
                throw new DirNotFoundException(path);
            }

            VirtualDirectory virtualDir = this._entry.VirtualDirectories.Add(String.Format("/{0}", name), path);
            return new IISWebVirturalDir(virtualDir);
        }

        /// <summary>
        /// Open a sub virtual directory
        /// </summary>
        /// <param name="name">Name of directory to be opened. Case insensitive.</param>
        /// <returns>A IISWebVirtualDir instance if open successfully done.Otherwise null.</returns>
        public IISWebVirturalDir OpenSubVirtualDir(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            var list = (from c in this._entry.VirtualDirectories where c.Path == name select c).ToList();
            if (list.Count > 0)
                return new IISWebVirturalDir(list[0]);

            return null;
        }

        /// <summary>
        /// Enumerate sub virtual directorys
        /// </summary>
        /// <returns></returns>
        public string[] EnumSubVirtualDirs()
        {
            List<string> ret = new List<string>();
            foreach (VirtualDirectory virDir in this._entry.VirtualDirectories)
            {
                ret.Add(virDir.Path);
            }

            return ret.ToArray();
        }

        /// <summary>
        /// Delete a sub virtual directory
        /// </summary>
        /// <param name="name">Name of the sub virtual directory to be deleted</param>
        /// <returns>true if successfully deleted. Otherwise false.</returns>
        public bool DeleteSubVirtualDir(string name)
        {
            // add back slash if necessary
            if (!name.StartsWith("/"))
                name = String.Format("/{0}", name);

            VirtualDirectory virtualDir = this._entry.VirtualDirectories.Where(c => c.Path == name).SingleOrDefault();
            if (virtualDir != null)
            {
                this._entry.VirtualDirectories.Remove(virtualDir);
                return true;
            }

            return false;
        }

        #endregion Operations
    }
}
