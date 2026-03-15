using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using DocumentManager.ComponentService;
using log4net;
using MenuSettings.MenuModel;
using UFInterfaces.Constants;
using VFS;
using ViewModelLib;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUserEditor.ComponentService;
using System.Windows.Data;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using UFUAEditor.ComponentService;
using System.Drawing;
using DocumentManager.ComponentService.Helpers;
using UFProjectManager.ComponentService;
using System.Reflection;

namespace MenuSettings.Documents
{
    [DataContract(Name = "UFMenuDocument", Namespace = Namespaces.UriProgea)]
    public class UFMenuDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations
        private const string menusessionname = "SchedulerServer";
        string currentUserAccessRole;
        int currentUserAccessLevel;
        int currentUserAccessMask;
        IAuthenticationCredentialsProvider authenticationProvider;
        IUFUserEditorManager uFUserEditorManager;
        IUFProjectManager iUFProjectManager;
        bool? isEnableUserManager;
        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.MenuManager);

        #endregion

        #region Constructors

        public UFMenuDocument()
        {
        }

        #endregion

        #region Persistance
        [DataMember]
        MenuModel.UFMenuEntity menuEntity;
        [DataMember]
        Guid id;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            MenuEntity.Document = this;
        }
        #endregion

        #region Methods
        public List<String> GetControllerDataStrings()
        {
            var list = new List<String>();
            var menuitemlist = GetMenuItemCollection();
            menuitemlist.ForEach(menu =>
            {
                list.AddRange(GetMenuItemStrings(menu));
            });

            return list;
        }

        public List<String> GetMenuItemStrings(UFMenuItemEntity menu)
        {
            var list = new List<String>();
            if (!string.IsNullOrEmpty(menu.Name))
                list.Add(menu.Name);
            menu.MenuItems.ForEach(item =>
            {
                list.AddRange(GetMenuItemStrings(item));
            });

            return list;
        }

        public void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            using (var cursor = new WaitCursor())
            {
                var list = new List<OPCUAViewModel.OPCUAEntityReference>();
                List<MenuSettings.MenuModel.UFMenuItemEntity> menuitemlist = GetMenuItemCollection();
                bool bDirty = false;
                menuitemlist.ForEach(menu =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    list.AddRange(GetOPCReferenceList(menu, model));
                    if (model.ApplyNewNames)
                    {
                        var commandList = menu.CommandList as CommandManager.CommandManagerList;
                        foreach (var command in commandList)
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var properties = CommandManager.Extensions.CommandManagerExtensions.GetBrowsablePropertiesOfType<Uri>(command);
                            foreach (PropertyInfo prop in properties)
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;

                                var uri = prop.GetValue(command) as Uri;
                                if (uri != null)
                                {
                                    var newUri = parent.MakeRelativeUri(MakeAbosoluteUri(uri));
                                    if (newUri != uri)
                                    {
                                        prop.SetValue(command, newUri);
                                        bDirty = true;
                                    }
                                }
                            }
                        }
                    }
                });
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
                            var preChanged = item.ToXml();
                            var postChanged = preChanged;
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
                                postChanged = item.ToXml();
                                bDirty = true;
                            }
                            menuitemlist.ForEach(menu =>
                            {
                                var listCommands = new CommandManager.CommandManagerList();
                                listCommands.AddRange(menu.CommandList as CommandManager.CommandManagerList);
                                listCommands.ForEach(command =>
                                {
                                    command.UpdateTags(preChanged.FromXml<OPCUAViewModel.OPCUAEntityReference>(), postChanged.FromXml<OPCUAViewModel.OPCUAEntityReference>());
                                });

                                menu.CommandList = listCommands;
                            });
                        });
                    }
                }
                if (bDirty)
                    SaveToFile();
            }
        }

        private IEnumerable<OPCUAViewModel.OPCUAEntityReference> GetOPCReferenceList(UFMenuItemEntity menu, UFInterfaces.Editors.CrossReferenceModel model)
        {
            bool getScreens = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Resources);
            bool getTags = model.CRManagement.CrossReferenceTypeList.Contains(CrossReferenceType.Tags);
            var list = new List<OPCUAViewModel.OPCUAEntityReference>();
            if (!getScreens && !getTags)
                return list;
            if (menu.EnableTag != null)
                list.Add(menu.EnableTag);
            if (menu.MarkTag != null)
                list.Add(menu.MarkTag);
            IUFUAEditorManager service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var cRMapsHeler = new CRMapsHelper() { CrossReferenceTypes = model.CRManagement.CrossReferenceTypeList };
            menu.GetAllSourceEntityReferencesDetails(this, service, cRMapsHeler, getScreens, getTags);
            if (cRMapsHeler.Tags != null)
            {
                cRMapsHeler.Tags.ToList().ForEach(tag =>
                {
                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                    if (refdetails.Count > 0 && refdetails.First().Value != null)
                        list.Add(refdetails.First().Value);
                });
            }
            menu.MenuItems.ForEach(item =>
            {
                list.AddRange(GetOPCReferenceList(item, model));
            });

            return list;

        }
        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            return SaveToFile();
        }

        bool WriteMenuDataStream(Stream ostrm)
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
                    var serializer = new DataContractSerializer(typeof(UFMenuDocument));
                    serializer.WriteObject(writer, this);
                    NeedsSave = false;
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
                        if (!WriteMenuDataStream(memoryStream))
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
                        if (!WriteMenuDataStream(memoryStream))
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
                        if (!WriteMenuDataStream(ostrm))
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

        #endregion

        #region Static Methods
        static UFMenuDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(UFMenuDocument));
                var document = formatter.ReadObject(stream) as UFMenuDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static String DefaultRecipeName { get { return Properties.Settings.Default.DefaultMenuName; } }

        public static String GetUniqueTitle(IDocument doc)
        {
            if (doc.Parent == null)
                return doc.Title;
            return String.Format("{0}_{1}", doc.Parent.Title, doc.Title);
        }

        public static bool ExistFile(String fullPath, FileSystemProviderBase fileSystemProvider = null)
        {
            if (fileSystemProvider != null)
                return fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, fullPath));

            return File.Exists(fullPath);
        }

        public static UFMenuDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                UFMenuDocument document = null;
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
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, fullPath));
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
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
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
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
                return null;
            }
        }

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

        #endregion

        #region Properties

        [Browsable(true)]
        public MenuModel.UFMenuEntity MenuEntity
        {
            get
            {
                if (menuEntity == null)
                    menuEntity = new MenuModel.UFMenuEntity() { NodeId = Guid.NewGuid(), Document = this };

                return menuEntity;
            }
            set
            {
                if (menuEntity == value)
                    return;
                menuEntity = value;
                OnPropertyChanged("MenuEntity");
                NeedsSave = true;
            }
        }
        
        public bool PropModified
        { get; set;}
        
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
                MenuEntity.Name = Title;
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
                    OnPropertyChanged("Folder");
                    OnPropertyChanged("FullPath");
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
                    OnPropertyChanged("Filename");
                    OnPropertyChanged("FullPath");
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

        IUIMsgBoxAlertService uiInterface;
        [Browsable(false)]
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

    #endregion

        #region MenuItems
        public List<UFMenuItemEntity> GetMenuItemCollection()
        {
            return (from c in MenuEntity.MenuItems orderby c.OID ascending select c).ToList();
        }

        public List<UFMenuItemEntity> GetMenuItemCollection(UFMenuItemEntity root)
        {
            return (from c in root.MenuItems orderby c.OID ascending select c).ToList();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFMenuItemEntity GetMenuItem(string name, UFMenuItemEntity root = null)
        {
            List<UFMenuItemEntity> list = null;
            if(root == null)
                list = (from c in MenuEntity.MenuItems where c.Name == name select c).ToList();
            else
                list = (from c in root.MenuItems where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFMenuItemEntity GetMenuItem(Guid nodeid, UFMenuItemEntity root = null)
        {
            List<UFMenuItemEntity> list = null;
            if (root == null)
                list = (from c in MenuEntity.MenuItems where c.NodeId == nodeid select c).ToList();
            else
                list = (from c in root.MenuItems where c.NodeId == nodeid select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        public UFMenuItemEntity AddNewMenuItem(UFMenuItemEntity root, String name = null)
        {
            return new UFMenuItemEntity() {
                Name = NewMenuItemName(root, name),
                UFMenuAss = (root == null ? MenuEntity : null),
                UFItemAss = (root != null ? root : null),
                OID = MenuEntity.GetFirstValidMenuItemOrderId(root) + 1, 
                NodeId = Guid.NewGuid() };
        }

        String NewMenuItemName(UFMenuItemEntity parent = null, String name = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultMenuItemName;

            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            while (MenuItemNameExists(name, parent))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            return name;
        }

        bool MenuItemNameExists(String name, UFMenuItemEntity parent = null)
        {
            if(parent != null)
                return (from p in parent.MenuItems.AsParallel()
                        where p.Name == name
                        select p).ToList().Count > 0;

            return (from p in MenuEntity.MenuItems.AsParallel()
                    where p.Name == name
                    select p).ToList().Count > 0;

        }

        public bool MoveEntityUp(UFMenuItemEntity value)
        {
            List<UFMenuItemEntity> last = new List<UFMenuItemEntity>();
            if (value.UFMenuAss != null)
                last.AddRange(value.UFMenuAss.MenuItems.OrderBy(o => o.OID));
            else if (value.UFItemAss != null)
                last.AddRange(value.UFItemAss.MenuItems.OrderBy(o => o.OID));
            else
                last.AddRange(MenuEntity.MenuItems.OrderBy(o => o.OID));

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

        public bool MoveEntityDown(UFMenuItemEntity value)
        {
            List<UFMenuItemEntity> last = new List<UFMenuItemEntity>();
            if (value.UFMenuAss != null)
                last.AddRange(value.UFMenuAss.MenuItems.OrderBy(o => o.OID));
            else if (value.UFItemAss != null)
                last.AddRange(value.UFItemAss.MenuItems.OrderBy(o => o.OID));
            else
                last.AddRange(MenuEntity.MenuItems.OrderBy(o => o.OID));

            if (last.Count > 0)
            {
                int current = last.IndexOf(value);
                if (current != -1 && current < last.Count - 1)
                {
                    int forward = last[current + 1].OID;
                    last[current + 1].OID = last[current].OID;
                    last[current].OID = forward;
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region ICloneable Members

        UFMenuDocument(UFMenuDocument template)
        {
            if (template == null)
                return;

            MenuEntity = template.MenuEntity;
        }

        public object Clone()
        {
            return new UFMenuDocument(this);
        }

        #endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);
        }

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
        public bool IsEmpty
        {
            get
            {
                return false;
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
        public string Theme
        {
            get
            {
                if (Parent != null)
                    return Parent.Theme;
                return String.Empty;
            }
        }

        [Browsable(false)]
        public string FilePath
        {
            get { return FullPath; }
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

        #region Execution
        void TerminateActive(List<UFMenuItemEntity> list)
        {
            list.ForEach(entity =>
            {
                foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                    command.Terminate();
                entity.FreeRuntimeCommandList();
                TerminateActive(entity.MenuItems);
            });
        }

        public void TerminateActive()
        {
            if (authenticationProvider != null)
                authenticationProvider.UserOnline -= AuthenticationProvider_UserOnline;

            MenuEntity.MenuItems.ForEach(entity =>
                {
                    foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                        command.Terminate();
                    entity.FreeRuntimeCommandList();
                    TerminateActive(entity.MenuItems);
                });
        }
        public bool HasAccessLevel(string accessRole, int accessLevel, int accessMask)
        {
            if (!IsEnableUserManager())
                return true;

            if ((accessLevel != 0 && currentUserAccessLevel < accessLevel) ||
                (!string.IsNullOrEmpty(accessRole) && (string.IsNullOrEmpty(currentUserAccessRole) || !currentUserAccessRole.Equals(accessRole))) ||
                (accessMask != 0 && (currentUserAccessMask & accessMask) == 0))
            {
                LoginUser(accessRole, accessLevel);
                return false;
            }

            return true;
        }
        private bool IsEnableUserManager()
        {
            if (isEnableUserManager.HasValue)
                return isEnableUserManager.Value;

            if(uFUserEditorManager != null)
                isEnableUserManager = uFUserEditorManager.GetEnableUserManager(this);
            else
                isEnableUserManager = false;
            return isEnableUserManager.Value;
        }
        private void LoginUser(string accessRole, int accessLevel)
        {
            authenticationProvider.ValidateUsingCredentialsProvider(Parent, Parent.Title, accessRole, accessLevel);
        }

        public void PrepareSubMenu(string sessionName, IDictionary<String, String> mapCulture, 
            MenuItem menu, List<UFMenuItemEntity> list, bool bTest = false)
        {
            list.Sort(CompareMenuItemEntityById);
            list.ForEach(entity =>
            {
                if (entity != null)
                {
                    if (entity.MenuItemType == MenuType.Separator)
                        menu.Items.Add(new Separator());
                    else
                    {
                        var menuItem = new MenuItem();
                        if (mapCulture != null && mapCulture.ContainsKey(entity.Name))
                            menuItem.Header = mapCulture[entity.Name];
                        else
                            menuItem.Header = entity.Name;
                        if (entity.MenuItemImage != null)
                        {
                            var mp = new MediaElement()
                            {
                                Source = GetImageUri(entity.MenuItemImage),
                                Stretch = System.Windows.Media.Stretch.Uniform
                            };
                            if (Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(mp))
                                menuItem.Icon = mp;
                            else
                            {
                                if (iUFProjectManager == null)
                                    iUFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                                if (iUFProjectManager != null)
                                    iUFProjectManager.AddLogEntity(this, Properties.Resources.MenuManager,
                                        DateTime.UtcNow, $"{entity.Name}: {Properties.Resources.InvalidSource}",
                                        System.Diagnostics.EventLogEntryType.Error);
                                log.Error($"{entity.Name}: {Properties.Resources.InvalidSource}");
                            }
                        }

                        menu.Items.Add(menuItem);
                        
                        entity.PrepareExecution(sessionName, menuItem, this);

                        if (!bTest && !entity.IsPopup())
                        {
                            foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                                command.Init(MenuEntity, this, sessionName);
                            menuItem.Click += (o, e) =>
                            {
                                if (!HasAccessLevel(entity.AccessRole, entity.WritableAccessLevel, entity.WritableAccessMask))
                                    return;

                                    if (entity.RuntimeMarkTag != null && entity.RuntimeMarkTag.IsValid && entity.RuntimeMarkTag.MonitoredItemViewModel != null)
                                        entity.RuntimeMarkTag.MonitoredItemViewModel.WriteValue(menuItem.IsChecked);
                                
                                    List<CommandManager.CommandManager> lst = new List<CommandManager.CommandManager>();
                                    lst.AddRange(entity.RuntimeCommandList as List<CommandManager.CommandManager>);
                                    lst.ForEach(command => 
                                    {
                                        try
                                        {
                                            if (command.SessionName != sessionName)
                                                command.Init(MenuEntity, this, sessionName);
                                            command.BlindExecute();
                                        }
                                        catch (Exception ex)
                                        {
                                            if (UIInterface != null)
                                                UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingCommand, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                                        }
                                    });
                                    //foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                                    //{
                                    //    try
                                    //    {
                                    //        if(command.SessionName != sessionName)
                                    //            command.Init(MenuEntity, this, sessionName);
                                    //        command.Execute();
                                    //    }
                                    //    catch (Exception ex)
                                    //    {
                                    //        MessageBox.Show(String.Format(Properties.Resources.ErrorExecutingCommand, ex.Message),
                                    //                                Title, MessageBoxButton.OK, MessageBoxImage.Error);
                                    //    }
                                    //}
                                
                            };
                            
                        }

                        PrepareSubMenu(sessionName, mapCulture, menuItem, entity.MenuItems, bTest);
                    }
                }
            });
        }

        private Uri GetImageUri(Uri uri)
        {
            try
            {
                string specialfolderpath = System.IO.Path.Combine(GetSpecialFolder(SpecialFolders.Images).OriginalString, System.IO.Path.GetFileName(uri.GetPathString()));
                if (System.IO.File.Exists(specialfolderpath))
                {
                    return new Uri(specialfolderpath, UriKind.RelativeOrAbsolute);
                }
                else
                {
                    if (fileSystemProviderBase != null)
                    {
                        return WPFUtilities.Converters.UriToAbsoluteUriConverter.GetFileSystemProviderBaseUri(uri, this, SpecialFolders.Images);
                    }
                    else
                        return uri;
                }
            }
            catch (Exception)
            {
                return uri;
            }
        }

        private static int CompareMenuItemEntityById(UFMenuItemEntity x, UFMenuItemEntity y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    return (x.OID > y.OID ? 1 : (x.OID == y.OID ? 0 : -1));
                }
            }
        }

        public MenuBase PrepareActive(string sessionName, IDictionary<String, String> mapCulture, bool bContextMenu, bool bTest = false)
        {
            MenuBase menu = null;
            if (bContextMenu)
                menu = new ContextMenu();
            else
                menu = new Menu()
                {
                    IsMainMenu = true
                };

            if (uFUserEditorManager == null)
                uFUserEditorManager = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
            if (iUFProjectManager == null)
                iUFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;

            if (authenticationProvider == null)
            {
                authenticationProvider = GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                authenticationProvider.UserOnline += AuthenticationProvider_UserOnline;
                authenticationProvider.RefreshCurrentUser(DocumentHelper.GetRootParent(this, traverse: true).Title);
            }

            MenuEntity.MenuItems.Sort(CompareMenuItemEntityById);
            MenuEntity.MenuItems.ForEach(entity =>
            {
                if (entity != null)
                {
                    if (entity.MenuItemType == MenuType.Separator)
                        menu.Items.Add(new Separator());
                    else
                    {
                        var menuItem = new MenuItem();
                        if (mapCulture != null && mapCulture.ContainsKey(entity.Name))
                            menuItem.Header = mapCulture[entity.Name];
                        else
                            menuItem.Header = entity.Name;
                        if (entity.MenuItemImage != null)
                        {
                            var mp = new MediaElement()
                            {
                                Source = GetImageUri(entity.MenuItemImage),
                                Stretch = System.Windows.Media.Stretch.Uniform
                            };
                            if (Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(mp))
                                menuItem.Icon = mp;
                            else
                                log.Error($"{entity.Name}: {Properties.Resources.InvalidSource}");
                        }

                        menu.Items.Add(menuItem);

                        entity.PrepareExecution(sessionName, menuItem, this);

                        if (!bTest && !entity.IsPopup())
                        {
                            foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                                command.Init(MenuEntity, this, sessionName);
                            MenuEntity.SetContainedObject(menu);
                            menuItem.Click += (o, e) =>
                            {
                                if (!HasAccessLevel(entity.AccessRole, entity.WritableAccessLevel, entity.WritableAccessMask))
                                    return;

                                try
                                {
                                    if (entity.RuntimeMarkTag != null && entity.RuntimeMarkTag.IsValid && entity.RuntimeMarkTag.MonitoredItemViewModel != null)
                                        entity.RuntimeMarkTag.MonitoredItemViewModel.WriteValue(menuItem.IsChecked);
                                }
                                catch (Exception)
                                { }

                                if (entity.RuntimeEnableTag != null && (!entity.RuntimeEnableTag.IsValid
                                    || entity.RuntimeEnableTag.MonitoredItemViewModel == null
                                    || Convert.ToBoolean(entity.RuntimeEnableTag.MonitoredItemViewModel.DataValue.Value) == false))
                                    return;
                                List<CommandManager.CommandManager> lst = new List<CommandManager.CommandManager>();
                                lst.AddRange(entity.RuntimeCommandList as List<CommandManager.CommandManager>);
                                lst.ForEach(command =>
                                {
                                    try
                                    {
                                        if (command.SessionName != sessionName)
                                            command.Init(MenuEntity, this, sessionName);
                                        command.BlindExecute();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (UIInterface != null)
                                            UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingCommand, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                                    }
                                });
                                //foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                                //{
                                //    try
                                //    {
                                //        if (command.SessionName != sessionName)
                                //            command.Init(MenuEntity, this, sessionName);
                                //        command.Execute();
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        MessageBox.Show(String.Format(Properties.Resources.ErrorExecutingCommand, ex.Message),
                                //                                Title, MessageBoxButton.OK, MessageBoxImage.Error);
                                //    }
                                //}

                            };
                        }

                        PrepareSubMenu(sessionName, mapCulture, menuItem, entity.MenuItems, bTest);
                    }
                }
            });

            return menu;
        }

        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUserAccessLevel = 0;
            currentUserAccessMask = 0;
            currentUserAccessRole = null;

            if (uFUserEditorManager != null)
            {
                if (!String.IsNullOrEmpty(e.User))
                {
                    currentUserAccessLevel = uFUserEditorManager.GetUserAccessLevel(this, e.User);
                    currentUserAccessMask = uFUserEditorManager.GetUserAccessMask(this, e.User);
                    currentUserAccessRole = uFUserEditorManager.GetUserRole(this, e.User);
                }
            }
        }


        #endregion
    }
}
