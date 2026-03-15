using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.DirectorySizeHelper
{
    /// <summary>
    /// A class for handling the folder size.
    /// </summary>
    public class DirectorySize
    {
        #region Declaration
        readonly string path;
        readonly string searchPattern;
        long lastBytesSize;
        #endregion

        #region Constructors
        public DirectorySize(string path) : 
            this(path, @"*.*")
        {
        }

        public DirectorySize(string path, string searchPattern)
        {
            this.path = path;
            this.searchPattern = searchPattern;

            CheckFilesSize();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Check the size of the folder.
        /// </summary>
        public void CheckFilesSize()
        {
            lastBytesSize = 0;
            if (DirectoryHelper.IsValidDirectory(path, true))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles(searchPattern);
                foreach (FileInfo info in files)
                    lastBytesSize += info.Length;
            }
        }

        /// <summary>
        /// Delete oldest file from folder by specifying the bytes site to remove.
        /// </summary>
        /// <param name="bytesToRemove">
        /// Total bytes size of files to remove.
        /// </param>
        /// <returns>
        /// The accurate total bytes size removed.
        /// </returns>
        public long DischargeOldestFiles(long bytesToRemove)
        {
            long totalBytes = 0;
            if (DirectoryHelper.IsValidDirectory(path, true))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] files = dir.GetFiles(searchPattern);

                var filesOrderByCreationTime = (from c in files orderby c.CreationTimeUtc ascending select c).AsParallel().ToList();
                foreach (FileInfo info in filesOrderByCreationTime)
                {
                    if ((totalBytes + info.Length) > bytesToRemove)
                        break;

                    totalBytes += info.Length;

                    try
                    {
                        info.Delete();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("DirectorySize : Cannot delete the following file {0}, error {1}", info.FullName, ex.Message);
                    }
                }
            }

            return totalBytes;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the path used by this instance of DirectorySize.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Get the search pattern used by this instance of DirectorySize.
        /// </summary>
        public string SearchPattern
        {
            get
            {
                return searchPattern;
            }
        }

        /// <summary>
        /// The last files size of the folder after the check.
        /// </summary>
        public long LastBytesSize
        {
            get
            {
                return lastBytesSize;
            }
        }
        #endregion
    }
}
