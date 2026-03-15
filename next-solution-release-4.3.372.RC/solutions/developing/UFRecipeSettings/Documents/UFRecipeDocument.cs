using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
#endif
using System.ComponentModel;
using ViewModelLib;
using System.IO;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using Utilities;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Threading.Tasks;
using UFRecipeSettings.UFRecipeModel;
using UFInterfaces;
using OPCUAViewModel;
using UFInterfaces.PropertyControl;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace UFRecipeSettings.Documents
{
    [DataContract(Name = "UFReciperDocument", Namespace = Namespaces.UriProgea)]
    public class UFRecipeDocument : ViewModelBase, IDocument
#if !NET_STANDARD
        , ICloneable, INotifyPropertyVisibilityChanged
#endif
    {
#region Declarations

        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;
       
#endregion

#region Constructors

        public UFRecipeDocument()
        { }

#endregion

#region Persistance
        [DataMember]
        UFRecipeModel.UFRecipeEntity recipeEntity;

        [DataMember]
        byte[] layoutItems;
        [DataMember]
        string[] labelItems;
        [DataMember]
        bool isLayoutEmpty;

        [DataMember]
        string sessionName;
        [DataMember]
        int removeDisabledItemAfterSecs = 30;
        [DataMember]
        int maxCleanCount = 2;
        [DataMember]
        bool useAlwaysSecureConnections = false;
        [DataMember]
        int slowSamplingInterval = 5000;
        [DataMember]
        bool disableWhenNotUsed = true;
        [DataMember]
        int publishingInterval = 1000;
        [DataMember]
        int fastSamplingInterval = 500;

        //[DataMember]
        //int defaultMaxLength = 256;
        [DataMember]
        bool showInTaskbar = true;
        [DataMember]
        double top;
        [DataMember]
        double left;
#if !NET_STANDARD
        [DataMember]
        ResizeMode resizeMode = ResizeMode.CanResize;
        [DataMember]
        WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner;
        [DataMember]
        WindowState windowState = WindowState.Normal;
        [DataMember]
        WindowStyle windowStyle = WindowStyle.ToolWindow;
#endif
        [DataMember]
        Guid id;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            showInTaskbar = true;

            maxCleanCount = 2;
            removeDisabledItemAfterSecs = 30;
            useAlwaysSecureConnections = false;
            slowSamplingInterval = 5000;
            disableWhenNotUsed = true;
            publishingInterval = 250;
            fastSamplingInterval = 500;

            //defaultMaxLength = 256;
#if !NET_STANDARD
            resizeMode = ResizeMode.CanResize;
            windowStartupLocation = WindowStartupLocation.CenterOwner;
            windowState = WindowState.Normal;
            windowStyle = WindowStyle.ToolWindow;
#endif
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            //RecipeEntity.DefaultMaxLenght = DefaultMaxLength;
            RecipeEntity.Document = this;
#if !NET_STANDARD
            RecipeEntity.PropertyChanged += recipeEntity_PropertyChanged;
#endif
        }
        #endregion

        #region Methods
#if !NET_STANDARD
        public List<String> GetControllerDataStrings()
        {
            var list = new List<String>();
            if (!string.IsNullOrEmpty(RecipeEntity.Description))
                list.Add(RecipeEntity.Description);
            List<UFDataValueEntity> itemlist = GetCRDataValuesCollection();
            itemlist.ForEach(item =>
            {
                if(!string.IsNullOrEmpty(item.Description))
                    list.Add(item.Description);
            });
            List<UFGroupEntity> groupList = GetGroupsCollection();
            groupList.ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item.Description))
                    list.Add(item.Description);
            });

            return list;
        }
        public void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            using (var cursor = new WaitCursor())
            {
                List<UFDataValueEntity> itemlist = GetCRDataValuesCollection() as List<UFDataValueEntity>;
                var list = new List<OPCUAViewModel.OPCUAEntityReference>();
                itemlist.ForEach(item =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    if (IsTagReferenceValid(item.TagDataValue))
                        list.Add(item.TagDataValue);
                    if (IsTagReferenceValid(item.TagIODataValue))
                        list.Add(item.TagIODataValue);
                });
                if (IsTagReferenceValid(RecipeEntity.TagRecipeDelete))
                    list.Add(RecipeEntity.TagRecipeDelete);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeIndex))
                    list.Add(RecipeEntity.TagRecipeIndex);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeList))
                    list.Add(RecipeEntity.TagRecipeList);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeLoad))
                    list.Add(RecipeEntity.TagRecipeLoad);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeRead))
                    list.Add(RecipeEntity.TagRecipeRead);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeSave))
                    list.Add(RecipeEntity.TagRecipeSave);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeState))
                    list.Add(RecipeEntity.TagRecipeState);
                if (IsTagReferenceValid(RecipeEntity.TagRecipeWrite))
                    list.Add(RecipeEntity.TagRecipeWrite);

                IUFUAEditorManager UfuaEditorService = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                if (UfuaEditorService != null)
                {
                    string defaultlocalendpoint = UfuaEditorService.GetDefaultLocalEndpoint(this, true);
                    var endpointslist = UfuaEditorService.GetEndpoints(this);
                    string aplicationName = UfuaEditorService.GetAplicationName(this, true);
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var mapNodes = UfuaEditorService.GetListNodeNames(this, nodelist);
                    if (mapNodes == null)
                        return;
                    bool bDirty = false;
                    foreach (var node in mapNodes.Keys)
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        var found = (from c in list
                                     where c.ResolvedNodeId != null &&
                                     c.ResolvedNodeId.ToString() == node
                                     select c).ToList();
                        found.ForEach(item =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;
                            bool changeEndpoint = !endpointslist.Contains(item.EndpointUrl);
                            if (changeEndpoint)
                            {
                                item.EndpointUrl = item.EndpointUrl.Replace(item.AppName, aplicationName);
                                if (!endpointslist.Contains(item.EndpointUrl))
                                    item.EndpointUrl = defaultlocalendpoint;
                            }
                            item.AppName = aplicationName;
                            var shortname = mapNodes[node];
                            var newName = String.Format("{0} ({1})", shortname, item.AppName);
                            if (item.HumanReadable != newName || changeEndpoint)
                            {
                                item.HumanReadable = newName;
                                item.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.ReadablePath);
                                item.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, item.RelativePath);
                                bDirty = true;
                            }
                        });
                    }
                    if (bDirty)
                        try
                        {
                            SaveToFile();
                        }
                        catch (Exception ex)
                        {
                        }
                }
            }
        }
        private bool IsTagReferenceValid(OPCUAEntityReference tagReference)
        {
            return tagReference != null && tagReference.IsValid;
        }

        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            return SaveToFile();
        }

        bool WriteRecipeDataStream(Stream ostrm)
        {
            var settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (var writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    var serializer = new DataContractSerializer(typeof(UFRecipeDocument));
                    serializer.WriteObject(writer, this);
                    NeedsSave = false;
                    NeedsRebuild = false;
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        public bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteRecipeDataStream(memoryStream))
                            return false;

                        fileSystemProviderBase.UploadFile(null, FullPath, memoryStream.ToArray());
                        return true;
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

                String settingsFileName = FullPath;
                if (forceEncryption || Protected)
                {
                    id = Id;
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteRecipeDataStream(memoryStream))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(settingsFileName, toWrite);
                    }
                }
                else
                {
                    id = Guid.Empty;
                    using (var ostrm = File.Open(settingsFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (!WriteRecipeDataStream(ostrm))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetFriendObjects(UFRecipeDocument document, GetFriendObjectsEventArgs e)
        {
            if (document != this)
                return false;

            if (e.friendList == null)
                e.friendList = new List<UFRecipeModel.UFRecipeEntity>();

            e.friendList.Add(RecipeEntity);

            return true;
        }
#endif

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateSessionSettings()
        {
            if (String.IsNullOrEmpty(SessionName))
                return;

            String[] serverUriArray = null;
#if !WINDOWS_UWP
            var ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
                serverUriArray = ufuaEditor.GetServerUriArray(this);
#endif
            RealTimeConnectionManagerViewModel.AddSessionSettings(SessionString, new SessionSettings()
            {
                ParentTitle = Parent.Title,
                RemoveDisabledItemAfterSecs = this.RemoveDisabledItemAfterSecs,
                MaxCleanCount = this.MaxCleanCount,
                UseAlwaysSecureConnections = this.UseAlwaysSecureConnections,
                SlowSamplingInterval = this.SlowSamplingInterval,
                DisableWhenNotUsed = this.DisableWhenNotUsed,
                PublishingInterval = this.PublishingInterval,
                FastSamplingInterval = this.FastSamplingInterval,
                ServerArray = serverUriArray
            });
        }

#endregion

#region Static Methods

        static UFRecipeDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(UFRecipeDocument));
                var document = formatter.ReadObject(stream) as UFRecipeDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static String DefaultRecipeName { get { return Properties.Settings.Default.DefaultRecipeName; } }

        public static String GetUniqueTitle(IDocument doc)
        {
            if (doc.Parent == null)
                return doc.Title;
            return String.Format("{0}_{1}", doc.Parent.Title, doc.Title);
        }

        public static bool ExistFile(String fullPath
#if !NET_STANDARD
            , FileSystemProviderBase fileSystemProvider = null
#endif
            )
        {
#if !NET_STANDARD
            if (fileSystemProvider != null)
                return fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath));
#endif
            return File.Exists(fullPath);
        }

        public static UFRecipeDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                UFRecipeDocument document = null;
#if !NET_STANDARD
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (fileSystemProvider != null)
                {
                    if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath)))
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, fullPath));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            document = ReadFromStream(memoryStream);
                            if (document == null)
                            {
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                }
#endif
                if (File.Exists(fullPath))
                {
                    if (parent.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fullPath));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            document = ReadFromStream(reader);
                            if (document == null)
                            {
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                var syslog = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
#else
                                var syslog = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                syslog.ErrorFormat(Properties.Resources.ErrorValidatingDocument, fullPath);
#endif
                                return null;
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                    else
                    {
                        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            document = ReadFromStream(fileStream);
                            if (document == null)
                            {
#if !NET_STANDARD
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                var syslog = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                }

                return document;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                var syslog = log4net.LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), parent.Title);
                syslog.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        public static void RemoveFile(string fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.DeleteFile(fileManagerFile);
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        public static String RenameFile(String fullPath, String oldName, String newName, FileSystemProviderBase fileSystemProvider = null)
        {
            var folder = Path.GetDirectoryName(fullPath);
            var newPathName = String.Format("{0}\\{1}{2}", folder, newName, Path.GetExtension(fullPath));

            if (fileSystemProvider != null)
            {
                var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                if (fileSystemProvider.Exists(fileManagerFile))
                {
                    fileSystemProvider.RenameFile(fileManagerFile, String.Format("{0}{1}", newName, fileManagerFile.Extension));
                }
            }
            else
            {
                if (File.Exists(fullPath))
                    File.Move(fullPath, newPathName);
            }

            return newPathName;
        }

        public static void CopyFile(String fullPath, String newPath, bool bCopy, IDocument parent)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            FileSystemProviderBase targetVFS = null;
            bool bDisposeTargetVFS = true;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(newPath);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = newPath
                };
            }
            else
            {
                if (!Path.IsPathRooted(newPath))
                {
                    targetVFS = fileSystemProvider;
                    bDisposeTargetVFS = false;
                }
            }

            try
            {
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new FileManagerFile(fileSystemProvider, fullPath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            if (bDisposeTargetVFS)
                                targetVFS.UploadFile(null, fullPath, data);
                            else
                                targetVFS.UploadFile(null, newPath, data);
                        }
                        else
                            File.WriteAllBytes(newPath, data);

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }
                }
                else
                {
                    if (File.Exists(fullPath))
                    {
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, fullPath.Replace(parent.rootBase, parent.rootBaseDB), 
                                File.ReadAllBytes(fullPath));
                        }
                        else
                            File.Copy(fullPath, newPath, true);
                    }

                    if (!bCopy)
                        RemoveFile(fullPath);
                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }
#endif
#endregion

#region Properties

        [Browsable(false)]
        public UFRecipeModel.UFRecipeEntity RecipeEntity
        {
            get
            {
                if (recipeEntity == null)
                    recipeEntity = new UFRecipeModel.UFRecipeEntity() { NodeId = Guid.NewGuid(), Document = this };

                return recipeEntity;
            }
            set
            {
                if (recipeEntity == value)
                    return;
                recipeEntity = value;
#if !NET_STANDARD
                OnPropertyChanged("RecipeEntity");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public byte[] LayoutItems
        {
            get
            {
                return layoutItems;
            }
            set
            {
                if (layoutItems == value)
                    return;
                layoutItems = value;
#if !NET_STANDARD
                OnPropertyChanged("LayoutItems");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public string[] LabelItems
        {
            get
            {
                return labelItems;
            }
            set
            {
                if (labelItems == value)
                    return;
                labelItems = value;
#if !NET_STANDARD
                OnPropertyChanged("LabelItems");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public bool IsLayoutEmpty
        {
            get
            {
                return isLayoutEmpty;
            }
            set
            {
                if (isLayoutEmpty == value)
                    return;
                isLayoutEmpty = value;
#if !NET_STANDARD
                OnPropertyChanged("IsLayoutEmpty");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public string SessionString
        {
            get
            {
                if (!String.IsNullOrEmpty(sessionName))
                {
                    return sessionName;
                }

                var docParent = Parent;
                if (Parent != null)
                    docParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                return docParent != null ? docParent.Title : Title;
            }
        }

        public string SessionName
        {
            get { return sessionName; }
            set
            {
                if (value == sessionName)
                    return;
                sessionName = value;
#if !NET_STANDARD
                NeedsSave = true;
                UpdateSessionSettings();
                OnPropertyChanged("SessionName");
                OnPropertyVisiblityChanged("SessionName");
#endif
            }
        }

        public int RemoveDisabledItemAfterSecs
        {
            get { return removeDisabledItemAfterSecs; }
            set
            {
                if (removeDisabledItemAfterSecs == value)
                    return;
                removeDisabledItemAfterSecs = value;
#if !NET_STANDARD
                OnPropertyChanged("RemoveDisabledItemAfterSecs");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public int MaxCleanCount
        {
            get { return maxCleanCount; }
            set
            {
                if (maxCleanCount == value)
                    return;
                maxCleanCount = value;
#if !NET_STANDARD
                OnPropertyChanged("MaxCleanCount");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public bool UseAlwaysSecureConnections
        {
            get { return useAlwaysSecureConnections; }
            set
            {
                if (useAlwaysSecureConnections == value)
                    return;
                useAlwaysSecureConnections = value;
#if !NET_STANDARD
                OnPropertyChanged("UseAlwaysSecureConnections");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public int FastSamplingInterval
        {
            get
            {
                return fastSamplingInterval;
            }
            set
            {
                if (fastSamplingInterval == value)
                    return;
                fastSamplingInterval = value;
#if !NET_STANDARD
                OnPropertyChanged("FastSamplingInterval");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public int SlowSamplingInterval
        {
            get
            {
                return slowSamplingInterval;
            }
            set
            {
                if (slowSamplingInterval == value)
                    return;
                slowSamplingInterval = value;
#if !NET_STANDARD
                OnPropertyChanged("SlowSamplingInterval");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public bool DisableWhenNotUsed
        {
            get { return disableWhenNotUsed; }
            set
            {
                if (disableWhenNotUsed == value)
                    return;
                disableWhenNotUsed = value;
#if !NET_STANDARD
                OnPropertyChanged("DisableWhenNotUsed");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public int PublishingInterval
        {
            get
            {
                return publishingInterval;
            }
            set
            {
                if (publishingInterval == value)
                    return;
                publishingInterval = value;
#if !NET_STANDARD
                OnPropertyChanged("PublishingInterval");
                NeedsSave = true;
#endif
                UpdateSessionSettings();
            }
        }

        public bool ShowInTaskbar
        {
            get
            {
                return showInTaskbar;
            }
            set
            {
                if (showInTaskbar == value)
                    return;
                showInTaskbar = value;
#if !NET_STANDARD
                OnPropertyChanged("ShowInTaskbar");
                NeedsSave = true;
#endif
            }
        }

#if !NET_STANDARD
        private bool _NeedsSave = false;
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return _NeedsSave; }
            set
            {
                if (_NeedsSave == value)
                    return;

                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }

        private bool _NeedsRebuild = false;
        [Browsable(false)]
        public bool NeedsRebuild
        {
            get { return _NeedsRebuild; }
            set
            {
                if (_NeedsRebuild == value)
                    return;

                _NeedsRebuild = value;
                OnPropertyChanged("NeedsRebuild");
            }
        }
#endif

        [ReadOnly(true)]
        public string FullPath
        {
            get
            {
                if (String.IsNullOrEmpty(Filename))
                {
                    return Path.Combine(Folder, TemporaryFilename);
                }
                else
                {
                    return Path.Combine(Folder, Filename);
                }
            }
            set
            {
                Folder = Path.GetDirectoryName(value);
                Filename = Path.GetFileName(value);
                RecipeEntity.RecipeName = Title;
            }
        }

        private string _Folder = "";
        [Browsable(false)]
        public string Folder
        {
            get
            {
                return _Folder;
            }
            set
            {
                if (_Folder != value)
                {
                    _Folder = value;
#if !NET_STANDARD
                    OnPropertyChanged("Folder");
                    OnPropertyChanged("FullPath");
#endif
                }
            }
        }

        private string _Filename;
        [Browsable(false)]
        public string Filename
        {
            get
            {
                if (String.IsNullOrEmpty(_Filename))
                {
                    return TemporaryFilename;
                }
                else
                {
                    return _Filename;
                }
            }
            set
            {
                if (_Filename != value)
                {
                    _Filename = value;
#if !NET_STANDARD
                    OnPropertyChanged("Filename");
                    OnPropertyChanged("FullPath");
#endif
                }
            }
        }

        string _TemporaryFilename = "";
        [Browsable(false)]
        public string TemporaryFilename
        {
            get
            {
                if (string.IsNullOrEmpty(_TemporaryFilename))
                {
                    string temp = "";
                    _TemporaryFilename = temp;
                    TempFilenameCount++;
                }
                return _TemporaryFilename;
            }
        }

#endregion

#region Recipes
#if !NET_STANDARD
        public List<UFDataValueEntity> GetCRDataValuesCollection()
        {
            List<UFDataValueEntity> rootlist = GetDataValuesCollection();
            List<UFGroupEntity> groupList = GetGroupsCollection();
            //groupList.ForEach(r => rootlist.AddRange(GetDataValuesCollection(r)));
            Parallel.ForEach(groupList, (r, loopstate) =>
            {
                var dvalue = GetDataValuesCollection(r);
                lock(rootlist)
                    rootlist.AddRange(dvalue);
            });
            return rootlist;
        }

        public List<UFDataValueEntity> GetDataValuesCollection()
        {
            return (from c in RecipeEntity.DataValues orderby c.OID ascending select c).ToList();
        }

        public List<UFDataValueEntity> GetDataValuesCollection(UFGroupEntity root)
        {
            return (from c in root.DataValues orderby c.OID ascending select c).ToList();
        }

        public List<UFGroupEntity> GetGroupsCollection()
        {
            return (from c in RecipeEntity.Groups orderby c.OID ascending select c).ToList();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFGroupEntity GetDataGroup(string name)
        {
            var list = (from c in RecipeEntity.Groups where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFDataValueEntity GetDataValue(string name, UFGroupEntity datagroup = null)
        {
            List<UFDataValueEntity> list = new List<UFDataValueEntity>();
            if(datagroup != null)
                list = (from c in datagroup.DataValues where c.Name == name select c).ToList();
            else
                list = (from c in RecipeEntity.DataValues where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        String NewGroupName(String name = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultGroupName;
            
            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            while (GroupNameExists(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            return name;
        }

        bool GroupNameExists(String name)
        {
            return (from p in RecipeEntity.Groups.AsParallel()
                    where p.GroupName == name
                    select p).ToList().Count > 0;
        }

        bool GroupNodeIdExists(String nodeId)
        {
            return (from p in RecipeEntity.Groups.AsParallel()
                    where p.NodeId.ToString() == nodeId
                    select p).ToList().Count > 0;
        }

        String NewDataValueName(UFRecipeModel.UFGroupEntity root, String name = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultDataValueName;

            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            while (DataValueNameExists(name, root))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            return name;
        }

        bool DataValueNameExists(String name, UFRecipeModel.UFGroupEntity root)
        {
            if (root == null)
            {
                return (from p in RecipeEntity.DataValues.AsParallel()
                        where p.DataValueName == name
                        select p).ToList().Count > 0;
            }

            return (from c in root.DataValues.AsParallel() where c.DataValueName == name select c).ToList().Count > 0;
        }

        bool DataValueNodeIdExists(String nodeId)
        {
            if ((from p in RecipeEntity.DataValues.AsParallel()
                 where p.NodeId.ToString() == nodeId
                 select p).ToList().Count > 0)
                return true;

            bool found = false;
            Parallel.ForEach(RecipeEntity.Groups, item => 
            {
                if ((from p in item.DataValues.AsParallel()
                     where p.NodeId.ToString() == nodeId
                     select p).ToList().Count > 0)
                {
                    found = true;
                }
            });

            return found;
        }

        public UFRecipeModel.UFGroupEntity AddNewGroup(String name = null)
        {
            return new UFRecipeModel.UFGroupEntity 
            {
                GroupName = NewGroupName(name),
                OID = RecipeEntity.GetFirstValidGroupOrderId() + 1,
                NodeId = Guid.NewGuid(), 
                UFRecipeAss = RecipeEntity 
            };
        }

        public UFRecipeModel.UFDataValueEntity AddNewDataValue(UFRecipeModel.UFGroupEntity root = null, String name = null)
        {
            return new UFRecipeModel.UFDataValueEntity(true) 
            {
                DataValueName = NewDataValueName(root, name),
                OID = RecipeEntity.GetFirstValidDataValueOrderId(root) + 1,
                NodeId = Guid.NewGuid(), 
                UFGroupAss = root, 
                UFRecipeAss = (root == null ? RecipeEntity : null) 
            };
        }

        public bool MoveEntityUp(UFRecipeModel.UFDataValueEntity value)
        {
            List<UFRecipeModel.UFDataValueEntity> last = new List<UFRecipeModel.UFDataValueEntity>();
            if (value.UFGroupAss != null)
                last.AddRange(value.UFGroupAss.DataValues.OrderBy(o => o.OID));
            else
                last.AddRange(RecipeEntity.DataValues.OrderBy(o => o.OID));

            if (last.Count > 0)
            {
                int current = last.IndexOf(value);
                if (current > 0)
                {
                    int backward = last[current - 1].OID;
                    last[current - 1].OID = last[current].OID;
                    last[current].OID = backward;
                    return true;
                }
            }

            return false;
        }

        public bool MoveEntityDown(UFRecipeModel.UFDataValueEntity value)
        {
            List<UFRecipeModel.UFDataValueEntity> last = new List<UFRecipeModel.UFDataValueEntity>();
            if (value.UFGroupAss != null)
                last.AddRange(value.UFGroupAss.DataValues.OrderBy(o => o.OID));
            else
                last.AddRange(RecipeEntity.DataValues.OrderBy(o => o.OID));

            if (last.Count > 0)
            {
                int current = last.IndexOf(value);
                if (current < last.Count - 1)
                {
                    int forward = last[current + 1].OID;
                    last[current + 1].OID = last[current].OID;
                    last[current].OID = forward;
                    return true;
                }
            }

            return false;
        }

        public bool MoveEntityUp(UFRecipeModel.UFGroupEntity group)
        {
            List<UFRecipeModel.UFGroupEntity> last = new List<UFRecipeModel.UFGroupEntity>();
            last.AddRange(RecipeEntity.Groups.OrderBy(o => o.OID));

            if (last.Count > 0)
            {
                int current = last.IndexOf(group);
                if (current > 0)
                {
                    int backward = last[current - 1].OID;
                    last[current - 1].OID = last[current].OID;
                    last[current].OID = backward;
                    return true;
                }
            }

            return false;
        }

        public bool MoveEntityDown(UFRecipeModel.UFGroupEntity group)
        {
            List<UFRecipeModel.UFGroupEntity> last = new List<UFRecipeModel.UFGroupEntity>();
            last.AddRange(RecipeEntity.Groups.OrderBy(o => o.OID));
            if (last.Count > 0)
            {
                int current = last.IndexOf(group);
                if (current < last.Count - 1)
                {
                    int forward = last[current + 1].OID;
                    last[current + 1].OID = last[current].OID;
                    last[current].OID = forward;
                    return true;
                }
            }

            return false;
        }

        public void recipeEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "RecipeName" &&
                    e.PropertyName != "Name")
                NeedsSave = true;
            if (e.PropertyName == "MaxLength")
                NeedsRebuild = true;
        }

        List<Guid> GetUniqueIndentifiers()
        {
            var ret = new List<Guid>();
            ret.Add(RecipeEntity.NodeId);
            RecipeEntity.Groups.ForEach((group) => ret.Add(group.NodeId));
            RecipeEntity.GetFlatDataValuesCollection().ForEach((datavalue) => ret.Add(datavalue.NodeId));
            return ret;
        }

        public void RenewUniqueIndentifiers()
        {
            var guids = GetUniqueIndentifiers();
            EnsureUniqueIndentifiers(guids);
        }

        public void EnsureUniqueIndentifiers(List<Guid> guids)
        {
            if (guids.Contains(RecipeEntity.NodeId))
                RecipeEntity.NodeId = Guid.NewGuid();
            guids.Add(RecipeEntity.NodeId);

            RecipeEntity.Groups.ForEach((group) => 
            {
                if (guids.Contains(group.NodeId))
                     group.NodeId = Guid.NewGuid();
                guids.Add(group.NodeId);
            });

            RecipeEntity.GetFlatDataValuesCollection().ForEach((datavalue) => 
            {
                if (guids.Contains(datavalue.NodeId))
                    datavalue.NodeId = Guid.NewGuid();
                guids.Add(datavalue.NodeId);
            });
        }

#endif
#endregion

#region Override Methods

        protected override void OnDispose()
        {
            OnDisposing(this);

            base.OnDispose();

#if !NET_STANDARD
            recipeEntity.PropertyChanged += recipeEntity_PropertyChanged;
#endif
        }

#endregion

#region ICloneable Members

        UFRecipeDocument(UFRecipeDocument template)
        {
            if (template == null)
                return;

            RecipeEntity = template.RecipeEntity;
        }

#if !NET_STANDARD
        public object Clone()
        {
            return new UFRecipeDocument(this);
        }
#endif
#endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

#if !NET_STANDARD
        UserControl activeView;
        [Browsable(false)]
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            set
            {
                activeView = value;
            }
        }

        [Browsable(false)]
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif

        IDocument parent;
        [Browsable(false)]
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        [Browsable(false)]
        public IList<IDocument> Childs
        {
            get
            {
                return null;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectType
        {
            get
            {
                if (Parent != null)
                    return Parent.ProjectType;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public String Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            if (Parent != null)
                return Parent.GetSpecialFolder(specialFolder);
            return null;
        }

        public Uri MakeAbosoluteUri(Uri relative)
        {
            if (relative == null)
                return null;

            if (Parent != null)
                return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return relative;

            return new Uri(new Uri(Path.GetDirectoryName(FullPath) + "\\"), relative);
        }

        public IDocument UpdateParentFromUri(Uri relative)
        {
            if (Parent != null)
                return Parent.UpdateParentFromUri(relative);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (Parent != null)
                return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
        }

#if !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return null;
            }
        }
#endif

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(FullPath);
            }
        }

        [Browsable(false)]
        public String rootBaseDB
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBaseDB;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public bool Protected
        {
            get
            {
                if (Parent != null)
                    return Parent.Protected;
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                if (Parent != null)
                    return Parent.Id;
                return Guid.Empty;
            }
        }

        public String Title
        {
            get
            {
                return Path.GetFileNameWithoutExtension(FullPath);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return FullPath;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return RecipeEntity.GetFlatDataValuesCollection().Count == 0;
            }
        }

        [Browsable(false)]
        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);
            return null;
        }

#endregion

#region INotifyPropertyVisibilityChanged Members
#if !NET_STANDARD
        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "RemoveDisabledItemAfterSecs" ||
                    propertyName == "MaxCleanCount" ||
                    propertyName == "UseAlwaysSecureConnections" ||
                    propertyName == "SlowSamplingInterval" ||
                    propertyName == "DisableWhenNotUsed" ||
                    propertyName == "PublishingInterval" ||
                    propertyName == "FastSamplingInterval")
                {
                    return !String.IsNullOrEmpty(SessionName);
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
#endif
#endregion
    }
}
