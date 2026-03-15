using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;

namespace VFS
{

    public class DataSourceFileSystemProvider : FileSystemProviderBase, IDisposable
    {
        #region Declarations
        IDataLayer dl;
        UnitOfWork uow;

        public override event EventHandler<RenamedEventArgs> Renamed;
        public override event EventHandler<FileSystemEventArgs> Created;
        public override event EventHandler<FileSystemEventArgs> Deleted;

        Object lockObject = new Object();
        #endregion

        public DataSourceFileSystemProvider(string rootFolder)
            : base(rootFolder)
        {
        }

        private string _ConnectionString;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                lock (lockObject)
                {
                    if (_ConnectionString != value)
                    {
                        _ConnectionString = value;

                        DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
                        dict.GetDataStoreSchema(typeof(VFSFolder).Assembly);

                        if (uow != null)
                        {
                            uow.Disconnect();
                            uow.Dispose();
                            uow = null;
                        }
                        if (dl != null)
                        {
                            dl.Dispose();
                            dl = null;
                        }
                    }
                }
            }
        }

        void EnsureDataConnection()
        {
            if (dl != null)
                return;

            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(_ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(VFSFolder).Assembly);
            dl = new ThreadSafeDataLayer(dict, store);

            // dl = XpoDefault.GetDataLayer(_ConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            uow = new UnitOfWork(dl);
        }

        private static readonly String DataSourceHeader = "data source";
        public String Title 
        {
            get
            {
                var helper = new ConnectionStringParser(ConnectionString);
                return helper.GetPartByName(DataSourceHeader);           
            }
        }

        public override IEnumerable<FileManagerFolder> GetFolders(FileManagerFolder parentFolder)
        {
            EnsureDataConnection();

            lock (lockObject)
            {
                if (parentFolder == null)
                {
                    var folders = (from c in new XPQuery<VFSFolder>(uow)// .AsParallel()
                                  where c.VFSFolderAss == null
                                  orderby c.Name
                                  select c).ToList();
                    if (folders.Count == 0)
                        return new List<FileManagerFolder>();

                    return (from c in folders// .AsParallel()
                            orderby c.Name
                            select new FileManagerFolder(this, c.Name)).ToList();
                }
                else
                {
                    var folder = FindFolder(parentFolder.FullName, false);
                    if (folder == null)
                        return new List<FileManagerFolder>();
                    return (from c in folder.Folders// .AsParallel()
                            orderby c.Name
                            select new FileManagerFolder(this, String.Format("{0}\\{1}", parentFolder.FullName, c.Name))).ToList();
                }
            }
        }

        public override IEnumerable<FileManagerFile> GetFiles(FileManagerFolder parentFolder)
        {
            EnsureDataConnection();

            lock (lockObject)
            {
                if (parentFolder == null)
                {
                    var folder = (from c in new XPQuery<VFSFolder>(uow)// .AsParallel()
                                  where c.VFSFolderAss == null
                                  select c).ToList();
                    if (folder.Count == 0)
                        return new List<FileManagerFile>();

                    return (from c in folder[0].Files// .AsParallel()
                            orderby c.Name
                            select new FileManagerFile(this, c.Name)).ToList();
                }
                else
                {
                    var folder = FindFolder(parentFolder.FullName, false);
                    if (folder == null)
                        return new List<FileManagerFile>();
                    return (from c in folder.Files// .AsParallel()
                            orderby c.Name
                            select new FileManagerFile(this, String.Format("{0}\\{1}", parentFolder.FullName, c.Name))).ToList();
                }
            }
        }

        internal VFSFolder FindFolder(String path, bool bThrowException = true)
        {
            EnsureDataConnection();

            lock (lockObject)
            {
                var folders = path.Split('\\');

                VFSFolder currentFolder = null;
                foreach (var folderSegment in folders)
                {
                    if (currentFolder == null)
                    {
                        try
                        {
                            currentFolder = (from c in new XPQuery<VFSFolder>(uow)// .AsParallel()
                                             where c.Name == folderSegment
                                             select c).Single();
                        }
                        catch (Exception ex)
                        {
                            if (bThrowException)
                                throw new Exception(String.Format("{0} : {1}", folderSegment,
                                    Properties.Resources.FileManager_ErrorFolderNotFound));
                            currentFolder = null;
                            break;
                        }
                    }
                    else
                    {
                        try
                        {
                            currentFolder = (from c in currentFolder.Folders// .AsParallel()
                                             where c.Name == folderSegment
                                             select c).Single();
                        }
                        catch (Exception ex)
                        {
                            if (bThrowException)
                                throw new Exception(String.Format("{0} : {1}", folderSegment,
                                    Properties.Resources.FileManager_ErrorFolderNotFound));
                            currentFolder = null;
                            break;
                        }
                    }
                }

                if (currentFolder == null)
                {
                    if (bThrowException)
                        throw new Exception(Properties.Resources.FileManager_ErrorFolderNotFound);
                    return null;
                }

                return currentFolder;
            }
        }

        internal VFSFile FindFile(String path, bool bThrowException = true)
        {
            EnsureDataConnection();

            lock (lockObject)
            {
                var fileName = Path.GetFileName(path);
                var folderName = Path.GetDirectoryName(path);

                if (String.IsNullOrEmpty(folderName))
                {
                    try
                    {
                        var file = (from c in new XPQuery<VFSFile>(uow)// .AsParallel()
                                    where c.VFSFolderAss == null && c.Name == fileName
                                    select c).Single();
                        return file;
                    }
                    catch (Exception ex)
                    {
                        if (bThrowException)
                            throw new Exception(String.Format("{0} : {1}", fileName,
                                Properties.Resources.FileManager_ErrorFileNotFound));
                        return null;
                    }
                }

                var folders = folderName.Split('\\');

                VFSFolder currentFolder = null;
                foreach (var folderSegment in folders)
                {
                    if (currentFolder == null)
                    {
                        try
                        {
                            currentFolder = (from c in new XPQuery<VFSFolder>(uow)// .AsParallel()
                                             where c.Name == folderSegment
                                             select c).Single();
                        }
                        catch (Exception ex)
                        {
                            if (bThrowException)
                                throw new Exception(String.Format("{0} : {1}", folderSegment,
                                    Properties.Resources.FileManager_ErrorFolderNotFound));
                            currentFolder = null;
                            break;
                        }
                    }
                    else
                    {
                        try
                        {
                            currentFolder = (from c in currentFolder.Folders// .AsParallel()
                                             where c.Name == folderSegment
                                             select c).Single();
                        }
                        catch (Exception ex)
                        {
                            if (bThrowException)
                                throw new Exception(String.Format("{0} : {1}", folderSegment,
                                    Properties.Resources.FileManager_ErrorFolderNotFound));
                            currentFolder = null;
                            break;
                        }
                    }
                }

                if (currentFolder == null)
                {
                    if (bThrowException)
                        throw new Exception(Properties.Resources.FileManager_ErrorFolderNotFound);
                    return null;
                }

                try
                {
                    var file = (from c in currentFolder.Files// .AsParallel()
                                where c.Name == fileName
                                select c).Single();
                    return file;
                }
                catch (Exception ex)
                {
                    if (bThrowException)
                        throw new Exception(String.Format("{0} : {1}", fileName,
                            Properties.Resources.FileManager_ErrorFileNotFound));
                    return null;
                }
            }
        }

        public override bool Exists(FileManagerFile file)
        {
            EnsureDataConnection();
            return FindFile(file.RelativeName, false) != null;
        }
        public override bool Exists(FileManagerFolder folder)
        {
            EnsureDataConnection();
            return FindFolder(folder.RelativeName, false) != null;
        }
        public override DateTime GetLastWriteTime(FileManagerFile file)
        {
            EnsureDataConnection();
            return FindFile(file.RelativeName).LastWriteTime;
        }
        public override byte[] ReadFile(FileManagerFile file)
        {
            return FindFile(file.RelativeName).Data;
        }

        public override void DeleteFile(FileManagerFile file)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                FindFile(file.RelativeName).Delete();
                uow.CommitChanges();

                var t = Deleted;
                if (t != null)
                {
                    t(this, new FileSystemEventArgs(WatcherChangeTypes.Deleted,
                        Path.GetDirectoryName(file.FullName), Path.GetFileName(file.FullName)));
                }
            }
        }
        public override void DeleteFolder(FileManagerFolder folder)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                foreach (var file in GetFiles(folder))
                    DeleteFile(file);
                foreach (var file in GetFolders(folder))
                    DeleteFolder(file);

                FindFolder(folder.RelativeName).Delete();
                uow.CommitChanges();

                var t = Deleted;
                if (t != null)
                {
                    t(this, new FileSystemEventArgs(WatcherChangeTypes.Deleted,
                        Path.GetDirectoryName(folder.FullName), Path.GetFileName(folder.FullName)));
                }
            }
        }
        public override void RenameFile(FileManagerFile file, string name)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                var vfsFile = FindFile(file.RelativeName);
                var oldName = vfsFile.Name;
                vfsFile.Name = name;
                vfsFile.LastWriteTime = DateTime.UtcNow;
                uow.CommitChanges();

                var t = Renamed;
                if (t != null)
                {
                    t(this, new RenamedEventArgs(WatcherChangeTypes.Renamed,
                        Path.GetDirectoryName(file.FullName), name, oldName));
                }
            }
        }
        public override void RenameFolder(FileManagerFolder folder, string name)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                var vfsFolder = FindFolder(folder.RelativeName);
                var oldName = vfsFolder.Name;
                vfsFolder.Name = name;
                vfsFolder.LastWriteTime = DateTime.UtcNow;
                uow.CommitChanges();

                var t = Renamed;
                if (t != null)
                {
                    t(this, new RenamedEventArgs(WatcherChangeTypes.Renamed,
                        Path.GetDirectoryName(folder.FullName), name, oldName));
                }
            }
        }
        public override void MoveFile(FileManagerFile file, FileManagerFolder newParentFolder)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                var vfsFile = FindFile(file.RelativeName);
                vfsFile.VFSFolderAss.Files.Remove(vfsFile);
                FindFolder(newParentFolder.RelativeName).Files.Add(vfsFile);
                uow.CommitChanges();

                var t = Deleted;
                if (t != null)
                {
                    t(this, new FileSystemEventArgs(WatcherChangeTypes.Deleted,
                        Path.GetDirectoryName(file.FullName), Path.GetFileName(file.FullName)));
                }
                var t2 = Created;
                if (t2 != null)
                {
                    t2(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                        newParentFolder.FullName, Path.GetFileName(file.FullName)));
                }
            }
        }
        public override void MoveFolder(FileManagerFolder folder, FileManagerFolder newParentFolder)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                var vfsFolder = FindFolder(folder.RelativeName);
                vfsFolder.VFSFolderAss.Folders.Remove(vfsFolder);
                FindFolder(newParentFolder.RelativeName).Folders.Add(vfsFolder);
                uow.CommitChanges();

                var t = Deleted;
                if (t != null)
                {
                    t(this, new FileSystemEventArgs(WatcherChangeTypes.Deleted,
                        Path.GetDirectoryName(folder.FullName), Path.GetFileName(folder.FullName)));
                }
                var t2 = Created;
                if (t2 != null)
                {
                    t2(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                        newParentFolder.FullName, Path.GetFileName(folder.FullName)));
                }
            }
        }
        public override void CreateFolder(FileManagerFolder parent, string name)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                if (parent != null && !Exists(parent))
                    CreateFolder(null, parent.FullName);

                var folders = name.Split('\\');

                foreach (var f in folders)
                {
                    if (String.IsNullOrEmpty(f))
                        continue;

                    if (parent == null)
                    {
                        var list = (from c in new XPQuery<VFSFolder>(uow)// .AsParallel()
                                    where c.VFSFolderAss == null && c.Name == f
                                    select c).ToList();
                        if (list.Count == 0)
                        {
                            // throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
                            var folder = new VFSFolder(uow) { Name = f, LastWriteTime = DateTime.UtcNow };
                            uow.CommitChanges();

                            var t2 = Created;
                            if (t2 != null)
                            {
                                t2(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                                    parent == null ? "" : parent.RelativeName, Path.GetFileName(f)));
                            }
                        }
                        parent = new FileManagerFolder(this, f);
                    }
                    else
                    {
                        var vfsFolder = FindFolder(parent.RelativeName);
                        var newlist = (from c in vfsFolder.Folders// .AsParallel()
                                       where c.Name == f
                                       select c).ToList();
                        if (newlist.Count == 0)
                        {
                            // throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
                            var newfolder = new VFSFolder(uow) { Name = f, LastWriteTime = DateTime.UtcNow };
                            vfsFolder.Folders.Add(newfolder);

                            uow.CommitChanges();

                            var t3 = Created;
                            if (t3 != null)
                            {
                                t3(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                                    parent == null ? "" : parent.RelativeName, Path.GetFileName(f)));
                            }
                        }

                        if (parent == null)
                            parent = new FileManagerFolder(this, f);
                        else
                            parent = new FileManagerFolder(this, String.Format("{0}\\{1}",
                                                            parent.RelativeName, f));
                    }
                }
            }
        }

        public override void UploadFile(FileManagerFolder folder, string fileName, byte[] content)
        {
            EnsureDataConnection();
            lock (lockObject)
            {
                if (fileName.StartsWith("\\"))
                    fileName = fileName.Remove(0, 1);
                var folderName = Path.GetDirectoryName(fileName);
                fileName = Path.GetFileName(fileName);

                if (!String.IsNullOrEmpty(folderName))
                {
                    CreateFolder(folder, folderName);
                    if (folder == null)
                        folder = new FileManagerFolder(this, folderName);
                    else
                        folder = new FileManagerFolder(this, String.Format("{0}\\{1}", folder.RelativeName,
                                                                                        folderName));
                }

                if (folder == null)
                {
                    var list = (from c in new XPQuery<VFSFile>(uow)// .AsParallel()
                                where c.VFSFolderAss == null && c.Name == fileName
                                select c).ToList();
                    if (list.Count > 0)
                    {
                        // throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
                        list[0].Data = content;
                        list[0].LastWriteTime = DateTime.UtcNow;
                    }
                    else
                    {
                        var file = new VFSFile(uow)
                        {
                            Name = fileName,
                            LastWriteTime = DateTime.UtcNow,
                            Data = content
                        };

                        var t2 = Created;
                        if (t2 != null)
                        {
                            t2(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                                folderName, fileName));
                        }
                    }
                    uow.CommitChanges();
                }
                else
                {
                    var vfsFolder = FindFolder(folder.RelativeName);
                    var newlist = (from c in vfsFolder.Files// .AsParallel()
                                   where c.Name == fileName
                                   select c).ToList();
                    if (newlist.Count > 0)
                    {
                        // throw new FileManagerIOException(FileManagerErrors.AlreadyExists);
                        newlist[0].Data = content;
                        newlist[0].LastWriteTime = DateTime.UtcNow;
                    }
                    else
                    {
                        var file = new VFSFile(uow)
                        {
                            Name = fileName,
                            LastWriteTime = DateTime.UtcNow,
                            Data = content
                        };
                        vfsFolder.Files.Add(file);

                        var t2 = Created;
                        if (t2 != null)
                        {
                            t2(this, new FileSystemEventArgs(WatcherChangeTypes.Created,
                                folderName, fileName));
                        }
                    }
                    uow.CommitChanges();
                }
            }
        }

        public void Dispose()
        {
            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }

            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
        }
    }
}
