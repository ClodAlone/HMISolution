using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using VFS;
using System.ComponentModel;
using ViewModelLib;
using System.IO;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using Utilities;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Threading.Tasks;
using System.Reflection;
using UFShortcutSettings.ShortcutModel;
using System.Windows.Input;
using System.Globalization;
using log4net;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUserEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using UFUAEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace UFShortcutSettings.Documents
{
    [DataContract(Name = "UFShortcutDocument", Namespace = Namespaces.UriProgea)]
    public class UFShortcutDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations

        static string TempFilenamePreface = "default";
        static int TempFilenameCount = 0;

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.ShortcutManager);

        string currentUserAccessRole;
        int currentUserAccessLevel;
        int currentUserAccessMask;
        IAuthenticationCredentialsProvider authenticationProvider;
        #endregion

        #region Constructors

        public UFShortcutDocument()
        {
        }

        #endregion

        #region Persistance
        [DataMember]
        ShortcutModel.UFShortcutEntity shortcutEntity;
        [DataMember]
        Guid id;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
        }

        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            ShortcutEntity.Document = this;
        }
        #endregion

        #region Methods

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

        public List<UFKeyCommandEntity> GetShortcutItemCollection()
        {
            return (from c in ShortcutEntity.KeyCommands orderby c.OID ascending select c).ToList();
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
                    var serializer = new DataContractSerializer(typeof(UFShortcutDocument));
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

        #endregion

        #region Static Methods

        static UFShortcutDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(UFShortcutDocument));
                var document = formatter.ReadObject(stream) as UFShortcutDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static String DefaultRecipeName { get { return Properties.Settings.Default.DefaultShortcutName; } }

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

        public static UFShortcutDocument FromFile(String fullPath, IDocument parent)
        {
            try
            {
                UFShortcutDocument document = null;
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
        public ShortcutModel.UFShortcutEntity ShortcutEntity
        {
            get
            {
                if (shortcutEntity == null)
                    shortcutEntity = new ShortcutModel.UFShortcutEntity() { NodeId = Guid.NewGuid(), Document = this };

                return shortcutEntity;
            }
            set
            {
                if (shortcutEntity == value)
                    return;
                shortcutEntity = value;
                OnPropertyChanged("ShortcutEntity");
                NeedsSave = true;
            }
        }
        
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
                ShortcutEntity.Name = Title;
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
                    /*
                    if (TempFilenameCount == 0)
                    {
                        temp = TempFilenamePreface + ".xaml";
                    }
                    else
                    {
                        temp = TempFilenamePreface + TempFilenameCount + ".xaml";
                    }
                    */
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

        #region Shortcuts
        public List<UFKeyCommandEntity> GetKeyCommandCollection()
        {
            return (from c in ShortcutEntity.KeyCommands orderby c.OID ascending select c).ToList();
        }

        public UFKeyCommandEntity AddNewKeyCommand(String name = null)
        {
            return new UFKeyCommandEntity() {
                KeyName = NewKeyCommandName(name), 
                UFShortcutAss = ShortcutEntity, 
                OID = ShortcutEntity.KeyCommands.Count, 
                NodeId = Guid.NewGuid() };
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFKeyCommandEntity GetShortcutKey(string name)
        {
            var list = (from c in ShortcutEntity.KeyCommands where c.Name == name select c).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        String NewKeyCommandName(String name = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultKeyCommandName;

            ulong counter = 1;
            string fmtzero = "0";
            string baseName = name;
            Utilities.NewNameHelper.ParseName(name, out baseName, out fmtzero, out counter);

            while (KeyCommandNameExists(name))
                name = String.Format(format, baseName, (counter++).ToString(fmtzero));
            return name;
        }
        bool KeyCommandNameExists(String name)
        {
            return (from p in ShortcutEntity.KeyCommands.AsParallel()
                    where p.Name == name
                    select p).ToList().Count > 0;

        }

        #endregion

        #region ICloneable Members

        UFShortcutDocument(UFShortcutDocument template)
        {
            if (template == null)
                return;

            ShortcutEntity = template.ShortcutEntity;
        }

        public object Clone()
        {
            return new UFShortcutDocument(this);
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
                return false;//ShortcutEntity.GetCommandCollection().Count == 0;
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

        #region Execution
        public void TerminateActive()
        {
            if (authenticationProvider != null)
                authenticationProvider.UserOnline -= AuthenticationProvider_UserOnline;

            ShortcutEntity.KeyCommands.ForEach(entity =>
                {
                    if (!entity.Enabled)
                        return;

                    foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                        command.Terminate();
                    entity.FreeRuntimeCommandList();
                    entity.TerminateExecution();
                });
        }

        public List<String> PrepareActive(string sessionName)
        {
            if (authenticationProvider == null)
                authenticationProvider = GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;

            if (authenticationProvider != null)
            {
                authenticationProvider.UserOnline += AuthenticationProvider_UserOnline;
                authenticationProvider.RefreshCurrentUser(DocumentHelper.GetRootParent(this, traverse: true).Title);
            }

            var ret = new List<String>();
            ShortcutEntity.KeyCommands.ForEach(entity =>
            {
                if (!entity.Enabled)
                    return;

                ConvertKey(entity);

                if (!String.IsNullOrEmpty(entity.SpeechCommand))
                    ret.Add(entity.SpeechCommand);

                foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                    command.Init(ShortcutEntity, Parent, sessionName);
                
               entity.PrepareExecution(sessionName, this);
            });

            return ret;
        }

        ModifierKeys modifiers = ModifierKeys.None;

        bool CheckSystemKey(KeyEventArgs e)
        {
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                modifiers |= ModifierKeys.Alt;
                return true;
            }
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                modifiers |= ModifierKeys.Shift;
                return true;
            }
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                modifiers |= ModifierKeys.Control;
                return true;
            }
            else if (e.Key == Key.LWin || e.Key == Key.RWin)
            {
                modifiers |= ModifierKeys.Windows;
                return true;
            }

            return false;
        }
        public bool ExecuteCommand(String Command)
        {
            var founds = (from c in ShortcutEntity.KeyCommands
                          where c.SpeechCommand == Command
                          select c).ToList();

            if (founds.Count > 0)
            {
                founds.ForEach(entity =>
                {
                    if (!entity.Enabled)
                        return;

                    if (!HasAccessLevel(entity.AccessRole, entity.WritableAccessLevel, entity.WritableAccessMask))
                        return;

                    if (entity.RuntimeEnableTag != null && entity.RuntimeEnableTag.IsValid
                                        && entity.RuntimeEnableTag.MonitoredItemViewModel != null
                                        && Convert.ToBoolean(entity.RuntimeEnableTag.MonitoredItemViewModel.DataValue.Value) == false)
                        return;
                    List<CommandManager.CommandManager> lst = new List<CommandManager.CommandManager>();
                    lst.AddRange(entity.RuntimeCommandList as List<CommandManager.CommandManager>);
                    lst.ForEach(command =>
                    {
                        try
                        {
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
                    //        command.Execute();
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        MessageBox.Show(String.Format(Properties.Resources.ErrorExecutingCommand, ex.Message),
                    //                                Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    //    }
                    //}
                });
                return true;
            }

            return false;
        }

        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            currentUserAccessLevel = 0;
            currentUserAccessMask = 0;
            currentUserAccessRole = null;

            var userEditor = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
            if (userEditor != null)
            {
                if (!String.IsNullOrEmpty(e.User))
                {
                    currentUserAccessLevel = userEditor.GetUserAccessLevel(this, e.User);
                    currentUserAccessMask = userEditor.GetUserAccessMask(this, e.User);
                    currentUserAccessRole = userEditor.GetUserRole(this, e.User);
                }
            }
        }

        public bool ExecuteDown(System.Windows.Input.KeyEventArgs e)
        {
            if (CheckSystemKey(e))
                return false;

            var key = e.Key;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                key = e.SystemKey;

            var founds = (from c in ShortcutEntity.KeyCommands
                          where c.IsValid && c.Down && c.keycode == key && c.modifiers == Keyboard.Modifiers 
                          select c).ToList();
            if (founds.Count > 0)
            {
                founds.ForEach(entity =>
                    {
                        if (!entity.Enabled)
                            return;

                        if (!HasAccessLevel(entity.AccessRole, entity.WritableAccessLevel, entity.WritableAccessMask))
                            return;

                        if (entity.RuntimeEnableTag != null && entity.RuntimeEnableTag.IsValid
                                        && entity.RuntimeEnableTag.MonitoredItemViewModel != null
                                        && Convert.ToBoolean(entity.RuntimeEnableTag.MonitoredItemViewModel.DataValue.Value) == false)
                            return;
                        List<CommandManager.CommandManager> lst = new List<CommandManager.CommandManager>();
                        lst.AddRange(entity.RuntimeCommandList as List<CommandManager.CommandManager>);
                        lst.ForEach(command => 
                            {
                                try
                                {
                                    command.BlindExecute();
                                }
                                catch (Exception ex)
                                {
                                    if (UIInterface != null)
                                        UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingCommand, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                                }
                            });
                    });
                return true;
            }

            return false;
        }
        public bool HasAccessLevel(string accessRole, int accessLevel, int accessMask)
        {
            if ((accessLevel != 0 && currentUserAccessLevel < accessLevel) ||
                (!string.IsNullOrEmpty(accessRole) && (string.IsNullOrEmpty(currentUserAccessRole) || !currentUserAccessRole.Equals(accessRole))) ||
                (accessMask != 0 && (currentUserAccessMask & accessMask) == 0))
            {
                LoginUser(accessRole, accessLevel);
                return false;
            }

            return true;
        }
        private void LoginUser(string accessRole, int accessLevel)
        {
            authenticationProvider.ValidateUsingCredentialsProvider(Parent, Parent.Title, accessRole, accessLevel);
        }

        public bool ExecuteUp(System.Windows.Input.KeyEventArgs e)
        {
            /*
            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
                modifiers &= ~(ModifierKeys.Alt);
            else if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                modifiers &= ~(ModifierKeys.Shift);
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                modifiers &= ~(ModifierKeys.Control);
            else if (e.Key == Key.LWin || e.Key == Key.RWin)
                modifiers &= ~(ModifierKeys.Windows);
            */

            var key = e.Key;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                key = e.SystemKey;

            var founds = (from c in ShortcutEntity.KeyCommands
                          where c.IsValid && !c.Down && c.keycode == key && c.modifiers == Keyboard.Modifiers
                          select c).ToList();
            if (founds.Count > 0)
            {
                founds.ForEach(entity =>
                {
                    if (!entity.Enabled)
                        return;

                    if (!HasAccessLevel(entity.AccessRole, entity.WritableAccessLevel, entity.WritableAccessMask))
                        return;


                    if (entity.RuntimeEnableTag != null && entity.RuntimeEnableTag.IsValid
                                        && entity.RuntimeEnableTag.MonitoredItemViewModel != null
                                        && Convert.ToBoolean(entity.RuntimeEnableTag.MonitoredItemViewModel.DataValue.Value) == false)
                        return;

                    foreach (CommandManager.CommandManager command in entity.RuntimeCommandList)
                    {
                        try
                        {
                            command.BlindExecute();
                        }
                        catch (Exception ex)
                        {
                            if (UIInterface != null)
                                UIInterface.ShowError(String.Format(Properties.Resources.ErrorExecutingCommand, ex.InnerException != null ? ex.InnerException.Message : ex.Message));
                        }
                    }
                });
                return true;
            }

            modifiers = ModifierKeys.None;
            return false;
        }

        void ConvertKey(UFKeyCommandEntity entity)
        {
            entity.IsValid = false;
            if (String.IsNullOrEmpty(entity.ShortcutKey))
                return;

            String key, modifiers;
            var index = entity.ShortcutKey.LastIndexOf('+');
            if (index >= 0)
            {
                modifiers = entity.ShortcutKey.Substring(0, index);
                key = entity.ShortcutKey.Substring(index + 1);
            }
            else
            {
                modifiers = string.Empty;
                key = entity.ShortcutKey;
            }
            ModifierKeys none = ModifierKeys.None;
            Key keycode = Key.None;
            var keyConverter = new KeyConverter();
            var obj1 = keyConverter.ConvertFrom(null, CultureInfo.InvariantCulture, key);
            if (obj1 != null)
            {
                keycode = (Key)obj1;

                var modifiersKeyConverters = new ModifierKeysConverter();
                object obj2 = modifiersKeyConverters.ConvertFrom(null, CultureInfo.InvariantCulture, modifiers);
                if (obj2 != null)
                {
                    none = (ModifierKeys)obj2;
                }

                entity.modifiers = none;
                entity.keycode = keycode;
                entity.IsValid = true;
            }
            else
                log.Error(String.Format(Properties.Resources.ErrorInShortcut, entity.ShortcutKey, entity.KeyName, FullPath));
        }

        public void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            using (var cursor = new WaitCursor())
            {
                List<UFKeyCommandEntity> keytemlist = GetShortcutItemCollection();
                bool bDirty = false;
                var list = new List<OPCUAViewModel.OPCUAEntityReference>();
                keytemlist.ForEach(item =>
                {
                    if (model.QuitEvent.IsCancellationRequested)
                        return;

                    list.AddRange(GetOPCReferenceList(item, model.CRManagement.CrossReferenceTypeList));

                    if (model.ApplyNewNames)
                    {
                        var commandList = item.CommandList as CommandManager.CommandManagerList;
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
                            keytemlist.ForEach(shortcut =>
                            {
                                var listCommands = new CommandManager.CommandManagerList();
                                listCommands.AddRange(shortcut.CommandList as CommandManager.CommandManagerList);
                                listCommands.ForEach(command =>
                                {
                                    command.UpdateTags(preChanged.FromXml<OPCUAViewModel.OPCUAEntityReference>(), postChanged.FromXml<OPCUAViewModel.OPCUAEntityReference>());
                                });

                                shortcut.CommandList = listCommands;
                            });
                        });
                    }
                }
                if (bDirty)
                    SaveToFile();
            }
        }

        private IEnumerable<OPCUAViewModel.OPCUAEntityReference> GetOPCReferenceList(UFKeyCommandEntity shorcut, List<CrossReferenceType> crlist)
        {
            var list = new List<OPCUAViewModel.OPCUAEntityReference>();
            if (shorcut.EnableTag != null)
                list.Add(shorcut.EnableTag);
            IUFUAEditorManager service = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            var cRMapsHeler = new CRMapsHelper() { CrossReferenceTypes = crlist };
            shorcut.GetAllSourceEntityReferencesDetails(this, service, cRMapsHeler);
            if (cRMapsHeler != null)
            {
                cRMapsHeler.Tags?.ToList().ForEach(tag =>
                {
                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                    if (refdetails.Count > 0 && refdetails.First().Value != null)
                        list.Add(refdetails.First().Value);
                });
            }
            return list;

        }
        #endregion
    }
}
