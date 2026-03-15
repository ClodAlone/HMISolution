using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.IO;
using ViewModelLib;
using System.ComponentModel;
using System.Windows.Input;
#if !WINDOWS_UWP && !NET_STANDARD
using VFS;
using System.Windows.Threading;
#elif NET_STANDARD
using System.Windows.Media;
#else
using Windows.UI;
#endif
using DocumentManager.ComponentService;
using System.Text.RegularExpressions;

namespace UFProjectManager
{
    public class ResourceFolderWatcher : Observable, IScreenController, IDisposable
    {
        #region Declarations
#if !WINDOWS_UWP && !NET_STANDARD
        FileSystemWatcher watcher;
        readonly FileSystemProviderBase fileSystemProviderBase;
        readonly Dispatcher dispatcher;
#endif
        readonly IScreenController parentController;
        String innerPath;
        readonly bool bWatchChanges;
#endregion

#region Constructors
        public ResourceFolderWatcher(IScreenController parent, String path, String f, String sc, bool isR = false,
#if !WINDOWS_UWP && !NET_STANDARD

                                     FileSystemProviderBase fileSystem = null, 
#endif
            String rt = null, bool bWatch = true)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            fileSystemProviderBase = fileSystem;
#endif
            fileType = f;
            filter = "*" + f;
            scheme = sc;
            rootTitle = rt;
            isRoot = isR;
            parentController = parent;
            innerPath = path;

#if !WINDOWS_UWP && !NET_STANDARD
            dispatcher = Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
#endif
            bWatchChanges = bWatch;

            Discover();
        }
#endregion

#region Methods

        void Discover()
        {
            DisableFileWatcher();
            ListRelativeResources.Clear();
            ListResources.Clear();
            ListFolders.Clear();

#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
            {
                var fmfPath = new FileManagerFolder(fileSystemProviderBase, innerPath);
                var listFiles = fileSystemProviderBase.GetFiles(fmfPath);
                foreach (var file in listFiles)
                {
                    if (System.IO.Path.GetExtension(file.FullName) != fileType)
                        continue;

                    var u = new Uri(file.FullName, UriKind.RelativeOrAbsolute);
                    // var relative = new Uri(path, UriKind.RelativeOrAbsolute).MakeRelativeUri(u);
                    ListRelativeResources.Add(u);
                    ListResources.Add(u);
                }

                var listFolders = fileSystemProviderBase.GetFolders(fmfPath);
                foreach (var folder in listFolders)
                {
                    try
                    {
                        ListFolders.Add(new ResourceFolderWatcher(parentController, folder.FullName + "\\", fileType, Scheme, false, fileSystemProviderBase, bWatch: bWatchChanges));
                    }
                    catch
                    { }
                }

                fileSystemProviderBase.Created += (o, e) =>
                {
                    var action = new Action(() => 
                    {
                        var dirName = System.IO.Path.GetDirectoryName(e.FullPath);
                        if (innerPath == dirName || innerPath == (dirName + "\\"))
                            FileOrFolderCreated(e.FullPath);
                    });

                    if (dispatcher != null)
                        dispatcher.BeginInvokeIfRequired(action);
                    else
                        action();
                };
                fileSystemProviderBase.Deleted += (o, e) =>
                {
                    var action = new Action(() =>
                    {
                        var dirName = System.IO.Path.GetDirectoryName(e.FullPath);
                        if (innerPath == dirName || innerPath == (dirName + "\\"))
                            FileOrFolderDeleted(e.FullPath);
                    });

                    if (dispatcher != null)
                        dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                    else
                        action();
                };
                fileSystemProviderBase.Renamed += (o, e) =>
                {
                    var action = new Action(() =>
                    {
                        var dirName = System.IO.Path.GetDirectoryName(e.FullPath);
                        if (innerPath == dirName || innerPath == (dirName + "\\"))
                        {
                            FileOrFolderDeleted(e.OldFullPath);
                            FileOrFolderCreated(e.FullPath);
                        }
                    });

                    if (dispatcher != null)
                        dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                    else
                        action();
                };
            }
            else
#endif
            {
                if (!Directory.Exists(innerPath))
                    return;

                //Go through all the files in the plugin directory
                string[] files = null;
#if !WINDOWS_UWP
                int i = 0;
                while (i++ < 10)
#endif
                {
                    try
                    {
                        files = Directory.GetFiles(innerPath, filter);
#if !WINDOWS_UWP
                        break;
#endif
                    }
                    catch
                    {
#if !WINDOWS_UWP
                        System.Threading.Thread.Sleep(50);
#endif
                    }
                }

                if (files != null)
                {
                    foreach(var fileOn in files)
                    {
                        var u = new Uri(fileOn);
                        var relative = new Uri(innerPath
#if !NET_STANDARD
                            , UriKind.RelativeOrAbsolute
#endif
                            ).MakeRelativeUri(u);
                        ListRelativeResources.Add(relative);
                        ListResources.Add(u);
                    }
                }

                string[] folders = null;
#if !WINDOWS_UWP
                i = 0;
                while (i++ < 10)
#endif
                {
                    try
                    {
                        folders = Directory.GetDirectories(innerPath);
#if !WINDOWS_UWP
                        break;
#endif
                    }
                    catch (Exception)
                    {
#if !WINDOWS_UWP
                        System.Threading.Thread.Sleep(50);
#endif
                    }
                }

                if (folders != null)
                {
                    foreach(var dirOn in folders)
                    {
                        try
                        {
                            ListFolders.Add(new ResourceFolderWatcher(parentController, dirOn + System.IO.Path.DirectorySeparatorChar, fileType, Scheme, bWatch: bWatchChanges));
                        }
                        catch
                        { }
                    }
                }

                CreateFileWatcher(innerPath);
            }
        }

        void CreateFileWatcher(String path)
        {
            if (!bWatchChanges)
                return;

#if !WINDOWS_UWP && !NET_STANDARD
            watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = false
            };

            watcher.Created += (o, e) =>
            {
                var action = new Action(() =>
                {
                    FileOrFolderCreated(e.FullPath);
                });

                if (dispatcher != null)
                    dispatcher.BeginInvokeIfRequired(action);
                else
                    action();
            };
            watcher.Deleted += (o, e) =>
            {
                var action = new Action(() =>
                {
                    FileOrFolderDeleted(e.FullPath);
                });

                if (dispatcher != null)
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                else
                    FileOrFolderDeleted(e.FullPath);
            };
            watcher.Renamed += (o, e) =>
            {
                var action = new Action(() =>
                {
                    FileOrFolderDeleted(e.OldFullPath);
                    FileOrFolderCreated(e.FullPath);

                    OnPropertyChanged("Title");
                });

                if (dispatcher != null)
                    dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
                else
                    action();
            };

            watcher.NotifyFilter = NotifyFilters.FileName |
                                   NotifyFilters.DirectoryName |
                                   NotifyFilters.Attributes |
                                   NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite |
                                   NotifyFilters.Security |
                                   NotifyFilters.Size;
            watcher.EnableRaisingEvents = true;
#endif
        }

        void DisableFileWatcher()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (watcher == null)
                return;
            watcher.EnableRaisingEvents = false;
            bool bWatcherDisposed = false;
            watcher.Disposed += (o, e) =>
            {
                bWatcherDisposed = true;
            };
            watcher.Dispose();
            while (!bWatcherDisposed)
                System.Threading.Thread.Sleep(50);

            watcher = null;

            foreach (var folder in ListFolders)
                folder.DisableFileWatcher();
#endif
        }

        void FileOrFolderCreated(String fullPath)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
            {
                if (fileSystemProviderBase.Exists(new FileManagerFile(fileSystemProviderBase, fullPath)))
                {
                    if (System.IO.Path.GetExtension(fullPath).ToLower() == System.IO.Path.GetExtension(Filter).ToLower())
                    {
                        var u = new Uri(fullPath, UriKind.RelativeOrAbsolute);
                        // var relative = new Uri(Path).MakeRelativeUri(u);
                        ListRelativeResources.Add(u);
                        ListResources.Add(u);
                    }
                }
                else if (fileSystemProviderBase.Exists(new FileManagerFolder(fileSystemProviderBase, fullPath)))
                {
                    var toRemoveFolders = (from c in ListFolders/*.AsParallel()*/
                                           where c.Path == fullPath || c.Path == String.Format("{0}\\", fullPath)
                                           select c).ToList();
                    if (toRemoveFolders.Count == 0)
                        ListFolders.Add(new ResourceFolderWatcher(parentController, fullPath, fileType, Scheme, false, fileSystemProviderBase, bWatch:bWatchChanges));
                }
            }
            else
#endif
            {
                if (File.Exists(fullPath))
                {
                    if (System.IO.Path.GetExtension(fullPath).ToLower() == System.IO.Path.GetExtension(Filter).ToLower())
                    {
                        var u = new Uri(fullPath);
                        var relative = new Uri(Path, UriKind.RelativeOrAbsolute).MakeRelativeUri(u);
                        ListRelativeResources.Add(relative);
                        ListResources.Add(u);
                    }
                }
                else
                {
                    var toRemoveFolders = (from c in ListFolders/*.AsParallel()*/
                                           where c.Path == fullPath || c.Path == String.Format("{0}\\", fullPath)
                                           select c).ToList();
                    if (toRemoveFolders.Count == 0)
                        ListFolders.Add(new ResourceFolderWatcher(parentController, fullPath, fileType, Scheme, bWatch: bWatchChanges));
                }
            }
        }

        void FileOrFolderDeleted(String fullPath)
        {
            var toRemove = (from c in ListResources// .AsParallel() 
                            where (c.IsAbsoluteUri && c.LocalPath == fullPath || 
                                    c.OriginalString == fullPath || 
                                    String.Format("{0}{1}", Path, c.OriginalString) == fullPath)
                            select c).ToList();
            
            var toRemoveFolders = (from c in ListFolders/*.AsParallel()*/
                                   where c.Path == fullPath || c.Path == String.Format("{0}\\", fullPath)
                                   select c).ToList();

#if !WINDOWS_UWP && !NET_STANDARD
            //dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
#endif
            {
                toRemove.ForEach(rfw =>
                {
                    ListResources.Remove(rfw);
                    ListRelativeResources.Remove(rfw);
                });
                toRemoveFolders.ForEach(rfw =>
                {
                    ListFolders.Remove(rfw);
                    rfw.Dispose();
                });
            }
#if !WINDOWS_UWP && !NET_STANDARD            
            //));
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD

        internal bool RenameFolder(String oldName, String newName)
        {
            String path = Path;
            var parentDirName = System.IO.Path.GetDirectoryName(path.TrimEnd(System.IO.Path.DirectorySeparatorChar));
            path = String.Format("{0}{1}", parentDirName, Path.Replace(parentDirName, String.Empty).Replace(oldName, newName));

            try
            {
                if (fileSystemProviderBase != null)
                {
                    fileSystemProviderBase.RenameFolder(new FileManagerFolder(fileSystemProviderBase, Path), newName);
                }
                else
                {
                    DisableFileWatcher();
                    int i = 0;
                    bool bOk = false;
                    bool bDifferentCase = String.Compare(Path, path, true) == 0;
                    while (i++ < 10)
                    {
                        try
                        {
                            MoveFolder(Path, path, bDifferentCase);
                            bOk = true;
                            break;
                        }
                        catch
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                    }
                    if (!bOk)
                        MoveFolder(Path, path, bDifferentCase);
                    else
                        innerPath = Path;

                    Discover();
                }
            }
            catch 
            {
                DisableFileWatcher();
                Discover();
                return false;
            }

            return true;
        }

        void MoveFolder(string oldPath, string newPath, bool bDifferentCase)
        {
            if (bDifferentCase)
            {
                var dir = new DirectoryInfo(oldPath);
                string tempDirName = Guid.NewGuid().ToString();
                string tempDirPath = newPath;
                try
                {
                    tempDirPath = String.Format("{0}\\{1}", Directory.GetParent(String.Format("{0}/../", dir.FullName)).FullName, tempDirName);
                    while (Directory.Exists(tempDirPath))
                    {
                        tempDirName = Guid.NewGuid().ToString();
                        tempDirPath = String.Format("{0}\\{1}", Directory.GetParent(String.Format("{0}/../", dir.FullName)).FullName, tempDirName);
                    }
                }
                catch { }
                dir.MoveTo(tempDirPath);
                dir.MoveTo(newPath);
            }
            else
                System.IO.Directory.Move(oldPath, newPath);
        }
#endif
        internal void CreateFolder(String path)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
                fileSystemProviderBase.CreateFolder(null, path);
            else
#endif
                System.IO.Directory.CreateDirectory(path);
        }

        internal void DeleteFolder(String path)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
                fileSystemProviderBase.DeleteFolder(new FileManagerFolder(fileSystemProviderBase, path));
            else
#endif
                System.IO.Directory.Delete(path, true);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal String CreateNewName(String name)
        {
            ulong counter;
            string fmtzero = "0";
            string baseName = name;

            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            String filePath = String.Format("{0}\\{1}{2}", innerPath, name, fileType);
            if (fileSystemProviderBase != null)
            {
                while (fileSystemProviderBase.Exists(new FileManagerFile(fileSystemProviderBase, filePath)))
                {
                    name = String.Format("{0}{1}", baseName, (counter++).ToString(fmtzero));
                    filePath = String.Format("{0}\\{1}{2}", innerPath, name, fileType);
                }
            }
            else
            {
                while (File.Exists(filePath))
                {
                    name = String.Format("{0}{1}", baseName, (counter++).ToString(fmtzero));
                    filePath = String.Format("{0}\\{1}{2}", innerPath, name, fileType);
                }
            }

            return name;
            /*
            int i = 0;
            string b = string.Empty;
            int val;
            for (int j = name.Length - 1; j >= 0; j--)
            {
                if (Char.IsDigit(name[j]))
                    b = b.Insert(0, name[j].ToString());
                else
                    break;
            }
            if (b.Length > 0)
            {
                i = int.Parse(b);
                name = name.Replace(b, "");
            }
            var newName = name + b;
            String filePath = String.Format("{0}\\{1}{2}", innerPath, newName, fileType);
            if (fileSystemProviderBase != null)
            {
                while (fileSystemProviderBase.Exists(new FileManagerFile(fileSystemProviderBase, filePath)))
                {
                    newName = String.Format("{0}{1}", name, ++i, fileType);
                    filePath = String.Format("{0}\\{1}{2}", innerPath, newName, fileType);
                } 
            }
            else
            {
                while (File.Exists(filePath))
                {
                    newName = String.Format("{0}{1}", name, ++i, fileType);
                    filePath = String.Format("{0}\\{1}{2}", innerPath, newName, fileType);
                } 
            }

            return newName;
            */
        }
#endif
#endregion

#region Properties
        public String Path
        {
            get
            {
                return innerPath;
            }
        }

        public String Title
        {
            get
            {
                if (isRoot && !String.IsNullOrEmpty(rootTitle))
                    return rootTitle;
                
                var di = new DirectoryInfo(innerPath);
                return di.Name;
            }
        }

        readonly String fileType;
        public String FileType
        {
            get
            {
                return fileType;
            }
        }

        readonly String filter;
        public String Filter
        {
            get
            {
                return filter;
            }
        }

        readonly String scheme;
        public String Scheme
        {
            get
            {
                return scheme;
            }
        }

        readonly String rootTitle;
        public String RootTitle
        {
            get
            {
                return rootTitle;
            }
        }

        readonly bool isRoot;
        public bool IsRoot
        {
            get
            {
                return isRoot;
            }
        }

        SafeObservableCollection<Uri> listResources;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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

        SafeObservableCollection<Uri> listRelativeResources;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public SafeObservableCollection<Uri> ListRelativeResources
        {
            get
            {
                if (listRelativeResources == null)
                    listRelativeResources = new SafeObservableCollection<Uri>();
                return listRelativeResources;
            }
            set
            {
                if (listRelativeResources == value)
                    return;
                listRelativeResources = value;
            }
        }

        SafeObservableCollection<ResourceFolderWatcher> listFolders;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
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
                if (listFolders != null)
                {
                    foreach (var folder in listFolders)
                    {
                        folder.Dispose();
                    }
                }

                listFolders = value;
            }
        }
        public bool EnablePageChangeGesture
        {
            get => parentController.EnablePageChangeGesture; 
        }

#endregion

#region Commands

#if !WINDOWS_UWP && !NET_STANDARD
        RelayCommand _deleteCommand;
        [Browsable(false)]
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        param => OnDeleteCommand(EventArgs.Empty),
                        param => !IsRoot
                        );
                }
                return _deleteCommand;
            }
        }

        RelayCommand _newCommand;
        [Browsable(false)]
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                {
                    _newCommand = new RelayCommand(
                        param => OnNewCommand(EventArgs.Empty)
                        );
                }
                return _newCommand;
            }
        }

        RelayCommand _newFolderCommand;
        [Browsable(false)]
        public ICommand NewFolderCommand
        {
            get
            {
                if (_newFolderCommand == null)
                {
                    _newFolderCommand = new RelayCommand(
                        param => OnNewFolderCommand(EventArgs.Empty)
                        );
                }
                return _newFolderCommand;
            }
        }

        RelayCommand _renameFolderCommand;
        [Browsable(false)]
        public ICommand RenameFolderCommand
        {
            get
            {
                if (_renameFolderCommand == null)
                {
                    _renameFolderCommand = new RelayCommand(
                        param => OnRenameFolderCommand(EventArgs.Empty),
                        param => !IsRoot
                        );
                }
                return _renameFolderCommand;
            }
        }
#endif
#endregion

#region Events
        public event EventHandler deleteCommandEvent;
        public event EventHandler newCommandEvent;
        public event EventHandler newFolderCommandEvent;
        public event EventHandler renameFolderCommandEvent;
        void OnDeleteCommand(EventArgs ea)
        {
            var e = deleteCommandEvent;
            if (e != null)
                e(this, ea);
        }
        void OnNewCommand(EventArgs ea)
        {
            var e = newCommandEvent;
            if (e != null)
                e(this, ea);
        }
        void OnNewFolderCommand(EventArgs ea)
        {
            var e = newFolderCommandEvent;
            if (e != null)
                e(this, ea);
        }
        void OnRenameFolderCommand(EventArgs ea)
        {
            var e = renameFolderCommandEvent;
            if (e != null)
                e(this, ea);
        }
#endregion

#region IDisposable Members

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

#if !WINDOWS_UWP && !NET_STANDARD
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                watcher = null;
            }
#endif
            foreach (var folder in ListFolders)
            {
                folder.Dispose();
            }
        }

#endregion

#region IScreenController
        public string GetTitle()
        {
            return Title;
        }

        public StartType GetStartType()
        {
            return parentController.GetStartType();
        }

        public TouchType GetTouchType()
        {
            return parentController.GetTouchType();
        }

        public bool GetHasSpeechEnabled()
        {
            return parentController.GetHasSpeechEnabled();            
        }
        
        public double GetSpeechConfidenceLevel()
        {
            return parentController.GetSpeechConfidenceLevel();
        }

        public String GetSpeechCulture()
        {
            return parentController.GetSpeechCulture();
        }

        public String GetProjectCulture()
        {
            return parentController.GetProjectCulture();
        }

        public String GetProjectConverter()
        {
            return parentController.GetProjectConverter();
        }

        public List<String> GetDefaultSpeechCommands()
        {
            return parentController.GetDefaultSpeechCommands();
        }

        public ThemeType GetTheme()
        {
            return parentController.GetTheme();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public BingMapKind GetBingMapKind()
        {
            return parentController.GetBingMapKind();
        }

        public List<IScreenController> GetScreenControllers()
        {
            return (from c in ListFolders// .AsParallel() 
                    where IsVisible(new Uri(c.Path, UriKind.RelativeOrAbsolute)) select c as IScreenController).ToList();
        }

        public List<IScreenController> GetScreenControllers(IDocumentManager idoc)
        {
            return GetScreenControllers();
        }

        public List<Uri> GetScreenLists()
        {
            return (from c in ListResources// .AsParallel() 
                    where IsVisible(c) select c).ToList();
        }

        public List<Uri> GetScreenLists(IDocumentManager idoc)
        {
            return GetScreenLists();
        }

        public Uri GetStartupScreen()
        {
            return parentController.GetStartupScreen();
        }

        public List<Uri> GetGadgetScreens()
        {
            return parentController.GetGadgetScreens();
        }

        public List<Uri> GetAutoLoadScreens()
        {
            return parentController.GetAutoLoadScreens();
        }

        public Uri GetAppBarTopScreen()
        {
            return parentController.GetAppBarTopScreen();
        }

        public Uri GetAppBarLeftScreen()
        {
            return parentController.GetAppBarLeftScreen();
        }

        public Uri GetAppBarBottomScreen()
        {
            return parentController.GetAppBarBottomScreen();
        }

        public Uri GetAppBarRightScreen()
        {
            return parentController.GetAppBarRightScreen();
        }

        public Uri GetTopScreen()
        {
            return parentController.GetTopScreen();            
        }
        
        public Uri GetLeftScreen()
        {
            return parentController.GetLeftScreen();
        }

        public Uri GetRightScreen()
        {
            return parentController.GetRightScreen();
        }

        public Uri GetBottomScreen()
        {
            return parentController.GetBottomScreen();
        }

        public bool GetLayoutScreenOrder()
        {
            return parentController.GetLayoutScreenOrder();
        }

        public bool GetHasGeoCoordinates()
        {
            return GetHasGeoCoordinates(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public double GetLatitude()
        {
            return GetLatitude(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public double GetLongitude()
        {
            return GetLongitude(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            Color GetIdentityColor()
        {
            return GetIdentityColor(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public bool IsVisible()
        {
            return IsVisible(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public TileSize TileSize()
        {
            return TileSize(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public double GetMapMinZoomLevelVisibility()
        {
            return GetMapMinZoomLevelVisibility(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public double GetMapMaxZoomLevelVisibility()
        {
            return GetMapMaxZoomLevelVisibility(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public String GetUsersVisibility()
        {
            return GetUsersVisibility(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public String GetRolesVisibility()
        {
            return GetRolesVisibility(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public string GetDescription()
        {
            return parentController.GetDescription(new Uri(Path, UriKind.RelativeOrAbsolute));
        }

        public string GetIconSource()
        {
            return parentController.GetIconSource();
        }

        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            Color GetIdentityColor(Uri uri)
        {
            return parentController.GetIdentityColor(uri);
        }

        public bool IsVisible(Uri uri)
        {
            return parentController.IsVisible(uri);
        }

        public TileSize TileSize(Uri uri)
        {
            return parentController.TileSize(uri);
        }

        public double GetMapMinZoomLevelVisibility(Uri uri)
        {
            return parentController.GetMapMinZoomLevelVisibility(uri);
        }

        public double GetMapMaxZoomLevelVisibility(Uri uri)
        {
            return parentController.GetMapMaxZoomLevelVisibility(uri);
        }

        public String GetUsersVisibility(Uri uri)
        {
            return parentController.GetUsersVisibility(uri);
        }

        public String GetRolesVisibility(Uri uri)
        {
            return parentController.GetRolesVisibility(uri);
        }

        public String GetDescription(Uri uri)
        {
            return parentController.GetDescription(uri);
        }

        public bool GetHasGeoCoordinates(Uri uri)
        {
            return parentController.GetHasGeoCoordinates(uri);
        }

        public double GetLatitude(Uri uri)
        {
            return parentController.GetLatitude(uri);
        }

        public double GetLongitude(Uri uri)
        {
            return parentController.GetLongitude(uri);
        }

        public string GetLatitudeTag(Uri uri)
        {
            return parentController.GetLatitudeTag(uri);
        }

        public string GetLongitudeTag(Uri uri)
        {
            return parentController.GetLongitudeTag(uri);
        }

#endregion
    }
}
