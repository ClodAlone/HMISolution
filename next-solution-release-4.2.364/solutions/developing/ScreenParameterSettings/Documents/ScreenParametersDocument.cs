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
//using System.Windows.Controls.Primitives;
using System.Xml;
using System.Xml.Linq;
using DocumentManager.ComponentService;
using log4net;
using ScreenParameterSettings;
//using MenuSettings.MenuModel;
using UFInterfaces.Constants;
using VFS;
using ViewModelLib;
using UIMsgBoxAlertService.ComponentService;

namespace ScreenParametersSettings.Documents
{
    [DataContract(Name = "ScreenParametersDocument", Namespace = Namespaces.UriProgea)]
    public class ScreenParametersDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations

        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;

        private static readonly ILog log = LogManager.GetLogger(ScreenParameterSettings.Properties.Resources.ScreenParameterManager);

        //Dictionary<String, String> mapParameters = new Dictionary<string, string>();
        List<ParameterItem> ParamList = new List<ParameterItem>();

        Guid id = Guid.Empty;
        #endregion

        #region Constructors

        public ScreenParametersDocument()
        {
        }

        #endregion

        #region Methods

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        public ParameterItem AddParameter(ParameterItem parameter)
        {
            var p = GetParameterItem(parameter.Guid);
            var newPar = p == null ? (ParameterItem)parameter.Clone() : new ParameterItem(parameter.ID, parameter.text, this);
            ParamList.Add(newPar);
            return newPar;
        }

        public ParameterItem AddParameter(string id, string text, bool fload = false)
        {
            var p = new ParameterItem(id, text, this);
            ParamList.Add(p);
            if(!fload)
                NeedsSave = true;
            return p;
        }

        public IList<ParameterItem> GetParametersList()
        {
            return ParamList;
        }

        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            return SaveToFile();
        }

        bool WriteDataStream(Stream ostrm, bool addProjectId)
        {
            XDocument doc = new XDocument();

            XElement desc = new XElement("parameters");

            if (addProjectId)
            {
                XAttribute guid = new XAttribute("guid", Id);
                desc.Add(guid);
            }

            foreach (var el in ParamList/*mapParameters.Keys*/)
            {
                if (String.IsNullOrEmpty(el.ID) || String.IsNullOrEmpty(el.text))
                    continue;

                XElement e = new XElement("ID", el.ID);
                XAttribute a = new XAttribute("text", el.text/*mapParameters[el]*/);
                e.Add(a);
                desc.Add(e);
            }
            doc.Add(desc);
            doc.Save(ostrm);
            NeedsSave = false;
            
            return true;
        }

        public void RemoveParameterItem(ParameterItem p)
        {
            ParamList.Remove(p);
            NeedsSave = true;
        }

        public ParameterItem GetParameterItem(Guid guid)
        {
            return (from p in ParamList where p.Guid == guid select p).FirstOrDefault();
        }

        public bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteDataStream(memoryStream, addProjectId: false))
                            return false;

                        fileSystemProviderBase.UploadFile(null, FullPath, memoryStream.ToArray());
                        return true;
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FullPath));

                String settingsFileName = FullPath;
                if (forceEncryption || Protected)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteDataStream(memoryStream, addProjectId: true))
                            return false;

                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(settingsFileName, toWrite);
                    }
                }
                else
                {
                    using (var ostrm = File.Open(settingsFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        if (!WriteDataStream(ostrm, addProjectId: false))
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(ScreenParameterSettings.Properties.Resources.ErrorSavingDocument, ex.Message),
                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion

        #region Static Methods
        static ScreenParametersDocument ReadFromStream(Stream stream)
        {
            //var formatter = new DataContractSerializer(typeof(UFMenuDocument));
            //var document = formatter.ReadObject(stream) as UFMenuDocument;
            //return document;

            //read parameter file...

            try
            {
                var document = new ScreenParametersDocument();
                using (StreamReader d = new StreamReader(stream))
                {
                    var content = d.ReadToEnd();

                    XDocument doc = new XDocument();
                    doc = XDocument.Parse(content);
                    var parameters = doc.Descendants("parameters").ToList();
                    if (parameters.Count > 0)
                    {
                        Guid guid;
                        var attribute = parameters[0].Attribute("guid");
                        if (attribute != null && Guid.TryParse(attribute.Value, out guid))
                            document.id = guid;
                    }

                    var keyexpandolist = Utilities.XmlHelper.GetExpandoAttributeFromXml(content, "parameters", true);
                    if (keyexpandolist.Count() != 0)
                    {
                        keyexpandolist.ToList().ForEach(e =>
                        {
                            var regkeydictionary = e as IDictionary<string, object>;
                            regkeydictionary.ToList().ForEach(r =>
                            {
                                document.AddParameter/*mapParameters.Add*/(r.Value.ToString(), r.Key.ToString(), true);
                            });
                        });
                    }

                    return document;
                }
            }
            catch (Exception e)
            {
                return null;
            }           
        }

        public string NewText(string text)
        {
            int i = 0;
            string newname = text;
            
            bool nak;
            do
            {
                nak = (from p in ParamList where p.text == newname select p).ToList().Count > 0;
                if(nak)
                {
                    i++;
                    newname = string.Format("{0}_{1}", text, i);
                }
            }
            while (nak);
            return newname;
        }

        public static String DefaultParameterName { get { return ScreenParameterSettings.Properties.Settings.Default.DefaultScreenParameterName; } }

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

        public static ScreenParametersDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                ScreenParametersDocument document = null;
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
                                    uiMsgBox.ShowError(String.Format(ScreenParameterSettings.Properties.Resources.ErrorReadingDocument, fullPath));
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
                                    uiMsgBox.ShowError(String.Format(ScreenParameterSettings.Properties.Resources.ErrorReadingDocument, fullPath));
                            }
                            else if (!document.IsBelongFromParent(parent))
                            {
                                document.Dispose();
                                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(ScreenParameterSettings.Properties.Resources.ErrorValidatingDocument, fullPath));
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
                                    uiMsgBox.ShowError(String.Format(ScreenParameterSettings.Properties.Resources.ErrorReadingDocument, fullPath));
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
                    uiMsgBox.ShowError(String.Format(ScreenParameterSettings.Properties.Resources.ErrorReadingDocument, fullPath));
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
                //MenuEntity.Name = Title;
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
        
        #endregion

        #region ICloneable Members

        ScreenParametersDocument(ScreenParametersDocument template)
        {
            if (template == null)
                return;

            //MenuEntity = template.MenuEntity;
        }

        public object Clone()
        {
            return new ScreenParametersDocument(this);
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
        //void TerminateActive(List<UFMenuItemEntity> list)
        //{
        //    list.ForEach(entity =>
        //    {
        //        foreach (CommandManager.CommandManager command in entity.CommandList)
        //            command.Terminate();
        //        TerminateActive(entity.MenuItems);
        //    });
        //}

        public void TerminateActive()
        {
            //MenuEntity.MenuItems.ForEach(entity =>
            //    {
            //        foreach (CommandManager.CommandManager command in entity.CommandList)
            //            command.Terminate();
            //        TerminateActive(entity.MenuItems);
            //    });
        }

        //public void PrepareSubMenu(string sessionName, IDictionary<String, String> mapCulture, 
        //    MenuItem menu, List<UFMenuItemEntity> list, bool bTest = false)
        //{
        //    list.Sort(CompareMenuItemEntityById);
        //    list.ForEach(entity =>
        //    {
        //        switch (entity.MenuItemType)
        //        {
        //            case MenuType.Separator:
        //                menu.Items.Add(new Separator());
        //                break;
        //            case MenuType.Popup:
        //            case MenuType.Item:
        //                var menuItem = new MenuItem();
        //                if (mapCulture != null && mapCulture.ContainsKey(entity.Name))
        //                    menuItem.Header = mapCulture[entity.Name];
        //                else
        //                    menuItem.Header = entity.Name;
        //                if (!String.IsNullOrEmpty(entity.Image))
        //                {
        //                    var mp = new MediaElement()
        //                    {
        //                        Source = new Uri(entity.Image, UriKind.RelativeOrAbsolute),
        //                        Width = 32,
        //                        Height = 32
        //                    };
        //                    Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(mp);
        //                    menuItem.Icon = mp;
        //                }
        //                menu.Items.Add(menuItem);

        //                if (!bTest)
        //                {
        //                    foreach (CommandManager.CommandManager command in entity.CommandList)
        //                        command.Init(MenuEntity, this, sessionName);
        //                    menuItem.Click += (o, e) =>
        //                    {
        //                        foreach (CommandManager.CommandManager command in entity.CommandList)
        //                        {
        //                            try
        //                            {
        //                                command.Execute();
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                MessageBox.Show(String.Format(Properties.Resources.ErrorExecutingCommand, ex.Message),
        //                                                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
        //                            }
        //                        }
        //                    };
        //                }

        //                PrepareSubMenu(sessionName, mapCulture, menuItem, entity.MenuItems, bTest);
        //                break;
        //        }
        //    });
        //}

        //private static int CompareMenuItemEntityById(UFMenuItemEntity x, UFMenuItemEntity y)
        //{
        //    if (x == null)
        //    {
        //        if (y == null)
        //            return 0; //==
        //        else
        //            return -1;// x < y
        //    }
        //    else
        //    {
        //        //x!= null
        //        if (y == null)
        //            return 1; //x > y
        //        else
        //        {
        //            return (x.OID > y.OID ? 1 : (x.OID == y.OID ? 0 : -1));
        //        }
        //    }
        //}
        //public MenuBase PrepareActive(string sessionName, IDictionary<String, String> mapCulture, bool bContextMenu, bool bTest = false)
        //{
        //    MenuBase menu = null;
        //    if (bContextMenu)
        //        menu = new ContextMenu();
        //    else
        //        menu = new Menu()
        //        {
        //            IsMainMenu = true
        //        };
        //    MenuEntity.MenuItems.Sort(CompareMenuItemEntityById);
        //    MenuEntity.MenuItems.ForEach(entity =>
        //    {
        //        switch (entity.MenuItemType)
        //        {
        //            case MenuType.Separator:
        //                menu.Items.Add(new Separator());
        //                break;
        //            case MenuType.Popup:
        //            case MenuType.Item:
        //                var menuItem = new MenuItem();
        //                if (mapCulture != null && mapCulture.ContainsKey(entity.Name))
        //                    menuItem.Header = mapCulture[entity.Name];
        //                else
        //                    menuItem.Header = entity.Name;
        //                if (!String.IsNullOrEmpty(entity.Image))
        //                {
        //                    var mp = new MediaElement()
        //                    {
        //                        Source = new Uri(entity.Image, UriKind.RelativeOrAbsolute),
        //                        Width = 32,
        //                        Height = 32
        //                    };
        //                    Utilities.ResourceDictionaryExtensions.SetMediaElementAutoStart(mp);
        //                    menuItem.Icon = mp;
        //                }
        //                menu.Items.Add(menuItem);

        //                if (!bTest)
        //                {
        //                    foreach (CommandManager.CommandManager command in entity.CommandList)
        //                        command.Init(MenuEntity, this, sessionName);
        //                    menuItem.Click += (o, e) =>
        //                    {
        //                        foreach (CommandManager.CommandManager command in entity.CommandList)
        //                            command.Execute();
        //                    };
        //                }

        //                PrepareSubMenu(sessionName, mapCulture, menuItem, entity.MenuItems, bTest);
        //                break;
        //        }
        //    });

        //    return menu;
        //}

        #endregion
    }
}
