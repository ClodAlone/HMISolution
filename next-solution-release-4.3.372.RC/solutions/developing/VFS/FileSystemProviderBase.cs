using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;

namespace VFS
{
    public abstract class FileSystemProviderBase
    {
        public virtual event EventHandler<FileSystemEventArgs> Created;
        public virtual event EventHandler<FileSystemEventArgs> Deleted;
        public virtual event EventHandler<RenamedEventArgs> Renamed;

        static char[] Separators = new char[] { '\\', '/' };
        string rootFolder;
        public FileSystemProviderBase(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }
        public string RootFolder { get { return rootFolder; } }
        public virtual IEnumerable<FileManagerFile> GetFiles(FileManagerFolder folder)
        {
            throw new NotImplementedException();
        }
        public virtual IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] ReadFile(FileManagerFile file)
        {
            throw new NotImplementedException();
        }
        public virtual DateTime GetLastWriteTime(FileManagerFile file)
        {
            return DateTime.Now;
        }
        public virtual string RootFolderDisplayName
        {
            get
            {
                string folderPath = RootFolder.TrimEnd('\\', '/');
                int sepIndex = folderPath.LastIndexOf('\\');
                return sepIndex > -1 ? folderPath.Remove(0, sepIndex + 1) : folderPath;
            }
        }
        public virtual string GetRelativeFolderPath(FileManagerFolder folder, IUrlResolutionService rs)
        {
            return folder.RelativeName.Replace('\\', '/');
        }
        public virtual bool Exists(FileManagerFolder folder)
        {
            return true;
        }
        public virtual bool Exists(FileManagerFile file)
        {
            return true;
        }
        public virtual void CreateFolder(FileManagerFolder parent, string name)
        {
            throw new NotImplementedException();
        }
        public virtual void RenameFile(FileManagerFile file, string name)
        {
            throw new NotImplementedException();
        }
        public virtual void RenameFolder(FileManagerFolder folder, string name)
        {
            throw new NotImplementedException();
        }
        public virtual void DeleteFile(FileManagerFile file)
        {
            throw new NotImplementedException();
        }
        public virtual void DeleteFolder(FileManagerFolder folder)
        {
            throw new NotImplementedException();
        }
        public virtual void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            throw new NotImplementedException();
        }
        public virtual void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            throw new NotImplementedException();
        }
        public virtual void UploadFile(FileManagerFolder folder, string fileName, byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
