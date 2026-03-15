using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilitiesUnitTests
{
    public class DirectoryFilesHelper : IDisposable
    {
        #region Declarations
        readonly int filesNumber;
        readonly int bytesSize;
        readonly string path;
        readonly string extension;

        readonly Byte[] bytes;
        readonly string searchPattern;
        #endregion

        #region Constructors
        public DirectoryFilesHelper(int filesNumber, int bytesSize, string extension)
            : this(filesNumber, bytesSize)
        {
            this.extension = extension;
            searchPattern = String.Format("*.{0}", extension);
        }

        public DirectoryFilesHelper(int filesNumber, int bytesSize)
        {
            this.filesNumber = filesNumber;
            this.bytesSize = bytesSize;

            path = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetRandomFileName());
            path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), path);

            bytes = new Byte[bytesSize];
        }
        #endregion

        #region Methods
        public void AddNewGroupFiles()
        {
            AddNewGroupFiles(0);
        }

        public void AddNewGroupFiles(int millisecondDelay)
        {
            CheckAndCreateOutputDirectory();

            for (int ii = 0; ii < filesNumber; ii++)
            {
                var destFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), path, System.IO.Path.GetRandomFileName());
                if (extension != null)
                    destFile = System.IO.Path.ChangeExtension(destFile, extension);
                using (var fileStream = new System.IO.FileStream(destFile, System.IO.FileMode.CreateNew))
                {
                    fileStream.Write(bytes, 0, bytes.Length);
                }

                if (millisecondDelay > 0)
                    System.Threading.Thread.Sleep(millisecondDelay);
            }
        }

        public void AddNewFileName(string filename)
        {
            CheckAndCreateOutputDirectory();

            var destFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), path, filename);
            using (var fileStream = new System.IO.FileStream(destFile, System.IO.FileMode.CreateNew))
            {
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        public List<DateTime> GetCreationTimesUtc()
        {
            List<DateTime> ret = new List<DateTime>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(searchPattern ?? "*.*");
            foreach (FileInfo info in files)
            {
                ret.Add(info.CreationTimeUtc);
            }

            return ret;
        }

        public List<string> GetWholeFilesName()
        {
            List<string> ret = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo info in files)
            {
                ret.Add(info.Name);
            }

            return ret;
        }

        public List<string> GetFilesNameFilterByExtension()
        {
            List<string> ret = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles(searchPattern);
            foreach (FileInfo info in files)
            {
                ret.Add(info.Name);
            }

            return ret;
        }

        void CheckAndCreateOutputDirectory()
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }
        #endregion

        #region Properties
        public int FilesNumber
        {
            get
            {
                return filesNumber;
            }
        }

        public int BytesSize
        {
            get
            {
                return bytesSize;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
        }

        public string Extension
        {
            get
            {
                return extension;
            }
        }
        #endregion

        #region IDisposable
        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            if (path != null)
                System.IO.Directory.Delete(path, true);
        }
        #endregion
    }
}
