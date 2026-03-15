using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.IO;

namespace UFProject
{
    class ResourceFolderWatcher : IDisposable
    {
        #region Declarations
        FileSystemWatcher watcher;
        #endregion

        #region Constructors
        public ResourceFolderWatcher(String path, String f)
        {
            if (!Directory.Exists(path))
                return;

            filter = f;
            //Go through all the files in the plugin directory
            Array.ForEach(Directory.GetFiles(path, filter), fileOn =>
            {
                ListResources.Add(new Uri(fileOn));
            });

            Array.ForEach(Directory.GetDirectories(path), dirOn =>
            {
                ListFolders.Add(new ResourceFolderWatcher(dirOn, Filter));
            });

            watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = false
            };

            watcher.Created += (o, e) =>
                {
                    FileOrFolderCreated(e.FullPath);
                };
            watcher.Deleted += (o, e) =>
                {
                    FileOrFolderDeleted(e.FullPath);
                };
            watcher.Renamed += (o, e) =>
                {
                    FileOrFolderDeleted(e.OldFullPath);
                    FileOrFolderCreated(e.FullPath);
                };

            watcher.NotifyFilter = NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.Attributes |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Security |
                                   NotifyFilters.Size;
            watcher.EnableRaisingEvents = true;
        }
        #endregion

        #region Methods
        void FileOrFolderCreated(String fullPath)
        {
            if (File.Exists(e.FullPath))
            {
                if (System.IO.Path.GetExtension(fullPath) == Filter)
                    ListResources.Add(new Uri(fullPath));
            }
            else
            {
                ListFolders.Add(new ResourceFolderWatcher(fullPath, Filter));
            }
        }

        void FileOrFolderDeleted(String fullPath)
        {
            if (File.Exists(fullPath))
            {
                var toRemove = (from c in ListResources where c.AbsolutePath == fullPath select c).ToList();
                toRemove.ForEach(rfw =>
                {
                    ListResources.Remove(rfw);
                });
            }
            else
            {
                var toRemove = (from c in ListFolders where c.Path == fullPath select c).ToList();
                toRemove.ForEach(rfw =>
                    {
                        ListFolders.Remove(rfw);
                    });
            }
        }
        #endregion

        #region Properties
        public String Path
        {
            get
            {
                return watcher.Path;
            }
        }

        String filter;
        public String Filter
        {
            get
            {
                return filter;
            }
        }

        SafeObservableCollection<Uri> listResources;
        public SafeObservableCollection<Uri> ListResources
        {
            get
            {
                if (listResources == null)
                    listResources = new SafeObservableCollection<Uri>();
                return listResources;
            }
            set
            {
                if (listResources == value)
                    return;
                listResources = value;
            }
        }

        SafeObservableCollection<ResourceFolderWatcher> listFolders;
        public SafeObservableCollection<ResourceFolderWatcher> ListFolders
        {
            get
            {
                if (listFolders == null)
                    listFolders = new SafeObservableCollection<ResourceFolderWatcher>();
                return listFolders;
            }
            set
            {
                if (listFolders == value)
                    return;
                listFolders = value;
            }
        }


        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (watcher == null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }
        }

        #endregion
    }
}
