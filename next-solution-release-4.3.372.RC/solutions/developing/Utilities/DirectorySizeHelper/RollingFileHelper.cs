using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.DirectorySizeHelper
{
    /// <summary>
    /// A class for handling the rolling file over size.
    /// </summary>
    public class RollingFileHelper
    {
        #region Declaration
        readonly string fileName;
        readonly string filePath;
        readonly string backupFormat;
        #endregion

        #region Constructors
        public RollingFileHelper(string fullFilePath) : 
            this(fullFilePath, @".{0}")
        {
        }

        public RollingFileHelper(string fullFilePath, string backupFormat)
        {
            this.fileName = System.IO.Path.GetFileName(fullFilePath);
            this.filePath = System.IO.Path.GetDirectoryName(fullFilePath);
            this.backupFormat = backupFormat;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Roll over files.
        /// </summary>
        public void RollOverFiles()
        {
            var fullFilePath = Path.Combine(FilePath, FileName);
            RollOverFiles(fullFilePath, MaxSizeRollBackups, BackupFormat);
        }

        public static void RollOverFiles(string fullFilePath, int maxSizeRollBackups = 10, string backupFormat = ".{0}")
        {
            var index = 0;
            var format = String.Format("{0}{1}", fullFilePath, backupFormat);
            var backupFile = String.Format(format, maxSizeRollBackups);
            if (File.Exists(backupFile))
            {
                while (++index <= maxSizeRollBackups)
                {
                    var backupFile1 = String.Format(format, index);
                    var backupFile2 = String.Format(format, index + 1);
                    if(File.Exists(backupFile1))
                        File.Delete(backupFile1);
                    if (File.Exists(backupFile2))
                        File.Move(backupFile2, backupFile1);
                }

                File.Move(fullFilePath, backupFile);
            }
            else
            {
                index = maxSizeRollBackups;
                while (--index > 0)
                {
                    backupFile = String.Format(format, index);
                    if (File.Exists(backupFile))
                        break;
                }

                backupFile = String.Format(format, index + 1);
                File.Move(fullFilePath, backupFile);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Get the file path used by this instance of RollingFileSize.
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        /// <summary>
        /// Get the file name used by this instance of RollingFileSize.
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        /// <summary>
        /// Get the backup format used for creating a new backup file.
        /// </summary>
        public string BackupFormat
        {
            get
            {
                return backupFormat;
            }
        }

        int maxSizeRollBackups = 10;
        /// <summary>
        /// Get the maximum number of backup files.
        /// </summary>
        public int MaxSizeRollBackups
        {
            get
            {
                return maxSizeRollBackups;
            }
            set
            {
                MaxSizeRollBackups = value;
            }
        }
        #endregion
    }
}
