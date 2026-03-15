using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentManager.ComponentService;
using System.ComponentModel;
using ViewModelLib;
using System.IO;
using OPCUAViewModel;
using log4net;
using ClientEditor.ComponentService;
using System.Runtime.Serialization;
#if !NET_STANDARD
using UIMsgBoxAlertService.ComponentService;
using TempVarriables.ComponentService;
using System.Windows.Controls;
using VFS;
#endif
using Utilities;
using System.Xml;
using System.Windows;
using UFProjectManager.ComponentService;
using DocumentManager.ComponentService.Helpers;
using UFInterfaces.Constants;
using UFUAEditor.ComponentService;

using System.Text.RegularExpressions;

namespace ClientEditor.Document
{
    [DataContract(Name = "ClientData", Namespace = Namespaces.UriProgea)]
    public class ClientDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations
        static int TempFilenameCount = 0;
#if !NET_STANDARD
        internal static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        internal static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif
        #endregion

        #region events
#if !NET_STANDARD
        public event EventHandler<VariableEventArgs> CreatingVariable;
        virtual public void OnCreatingVariable(Object sender, VariableEventArgs args)
        {
            var t = CreatingVariable;
            if (t != null)
                t(sender, args);
        }

        public event EventHandler<VariableEventArgs> VariableCreated;
        virtual public void OnVariableCreated(Object sender, VariableEventArgs args)
        {
            var t = VariableCreated;
            if (t != null)
                t(sender, args);
        }
#endif
        #endregion

        #region ctor
        ClientDocument()
        {
        }
        #endregion

        #region Persistance
        [DataMember]
        Dictionary<String, AppNameSettings> mapAppNameSettings = new Dictionary<String, AppNameSettings>();
        [DataMember]
        Guid id;
        #endregion

        #region Properties
#if !NET_STANDARD
        bool needsSave;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool NeedsSave
        {
            get
            {
                return needsSave;
            }
            set
            {
                if (needsSave == value)
                    return;

                needsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, AppNameSettings> MapAppNameSettings
        {
            get
            {
                return mapAppNameSettings;
            }
#if !NET_STANDARD
            set
            {
                mapAppNameSettings.Clear();
                foreach (var entry in value.Keys)
                {
                    if (value[entry].HasOverriddenString())
                        mapAppNameSettings.Add(value[entry].ToString(), value[entry]);
                    else
                        mapAppNameSettings.Add(entry, value[entry]);
                }

                OnPropertyChanged("MapAppNameSettings");
                NeedsSave = true;
            }
#endif
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

#if !NET_STANDARD
        ClientEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public ClientEditorManagerComponent EditorManagerComponent
        {
            get
            {
                return editorManagerComponent;
            }
            private set
            {
                editorManagerComponent = value;
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

        #region methods  
        void EnsureValidValues()
        {
            if (mapAppNameSettings == null)
                mapAppNameSettings = new Dictionary<String, AppNameSettings>();
#if !NET_STANDARD
            IUFProjectManager uFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (uFProjectManager != null)
            {
                var map = uFProjectManager.GetMapAppNameSettings(this);
                if (map != null && map.Count > 0)
                {
                    var _map = map.ToDictionary(s => s.Key, s => (AppNameSettings)s.Value);
                    MapAppNameSettings = new Dictionary<string, AppNameSettings>(_map);
                    uFProjectManager.ClearMapAppNameSettings(this);
                }
            }
#endif
        }

#if !NET_STANDARD
        static char flatSeparator = ':';
        internal Dictionary<String, String> CheckAndUpdateVariableListSettingsFlat(String flat)
        {
            var editView = ActiveView as ClientEditorControl;

            var renamed = new Dictionary<String, String>();
            try
            {
                var tempVariables = flat.FromXml<Dictionary<String, String>>();
                if (tempVariables.Count > 0)
                    editView.ActivateTempVariableTab();

                foreach (var pair in tempVariables)
                {
                    var split = pair.Key.Split(new Char[] { flatSeparator });
                    var dsInterface = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface(split[0]);
                    if (dsInterface != null)
                    {
                        var editor = editView.GetSelectedTabEditor() as ITempVarControl;
                        var name = split[1];
                        long counter;
                        string fmtzero = "0";
                        var baseName = Regex.Replace(name, NewNameHelper.NewNameRegExpr, "");
                        var match = Regex.Match(name, NewNameHelper.NewNameRegExpr);
                        if (match.Success && !String.IsNullOrEmpty(match.Value))
                            fmtzero = new String('0', match.Value.Length);
                        counter = 1;

                        var local = dsInterface.GetVariable(name, this);
                        while (local != null)
                        {
                            name = String.Format("{0}{1}", baseName, (counter++).ToString(fmtzero));
                            local = dsInterface.GetVariable(name, this);
                        }

                        var folder = String.Empty;
                        int foundFolder = name.LastIndexOf('&');
                        if (foundFolder != -1)
                        {
                            folder = name.Remove(foundFolder);
                            name = name.Substring(foundFolder + 1);
                        }

                        var args = new VariableEventArgs() { Name = name, isLocal = true };
                        OnCreatingVariable(this, args);
                        if (!args.Cancel)
                        {
                            if (args.Name != name)
                            {
                                name = String.Format("{0}&{1}", folder, args.Name);

                                baseName = Regex.Replace(name, NewNameHelper.NewNameRegExpr, "");
                                match = Regex.Match(name, NewNameHelper.NewNameRegExpr);
                                if (match.Success && !String.IsNullOrEmpty(match.Value))
                                    fmtzero = new String('0', match.Value.Length);
                                counter = 1;

                                local = dsInterface.GetVariable(name, this);
                                while (local != null)
                                {
                                    name = String.Format("{0}{1}", baseName, (counter++).ToString(fmtzero));
                                    local = dsInterface.GetVariable(name, this);
                                }
                                foundFolder = name.LastIndexOf('&');
                                if (foundFolder != -1)
                                {
                                    folder = name.Remove(foundFolder);
                                    name = name.Substring(foundFolder + 1);
                                }
                                args.Name = name;
                            }

                            if (editView != null)
                            {
                                var newVarName = String.IsNullOrEmpty(folder) ? args.Name : String.Format("{0}&{1}", folder, args.Name);
                                editor.AddNewTag(newVarName, pair.Value);

                                if (args.Name != split[1])
                                {
                                    renamed.Add(dsInterface.GetReference(split[1]).ToXml(),
                                                dsInterface.GetReference(newVarName).ToXml());
                                }
                            }

                            OnVariableCreated(this, args);
                        }
                    }
                }
            }
            catch
            {
            }

            return renamed;
        }
#endif

        static ClientDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(ClientDocument));
                var document = formatter.ReadObject(stream) as ClientDocument;
                return document;
            }
            catch (Exception ex)
            {
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
                string sourcePath = GetFilePath(fullPath);
                string destPath = GetFilePath(newPath);
                if (fileSystemProvider != null)
                {
                    var fileManagerFile = new FileManagerFile(fileSystemProvider, sourcePath);
                    if (fileSystemProvider.Exists(fileManagerFile))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFile);
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, sourcePath, data);
                        }
                        else
                        {
                            File.WriteAllBytes(destPath, data);
                        }

                        if (!bCopy)
                            fileSystemProvider.DeleteFile(fileManagerFile);
                    }
                }
                else
                {
                    if (File.Exists(sourcePath))
                    {
                        if (targetVFS != null)
                        {
                            targetVFS.UploadFile(null, sourcePath.Replace(parent.rootBase, parent.rootBaseDB),
                                File.ReadAllBytes(sourcePath));
                        }
                        else
                            File.Copy(sourcePath, destPath, true);
                    }

                    if (!bCopy)
                        RemoveFile(sourcePath);

                }
            }
            finally
            {
                if (bDisposeTargetVFS && targetVFS != null && targetVFS is DataSourceFileSystemProvider)
                    (targetVFS as DataSourceFileSystemProvider).Dispose();
            }
        }
#endif

        static String GetFilePath(String path)
        {
            String filePath = String.Format("{0}{4}{1}{4}{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt,
                                Path.DirectorySeparatorChar);

            return filePath;
        }

        public static ClientDocument FromFile(String path, IDocumentManager c, IDocument parent, 
        bool bCreateNew = true)
        {
            var p = DocumentHelper.GetRootParent(parent, traverse: false);
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

                ClientDocument document = null;
#if !NET_STANDARD
                FileSystemProviderBase fileSystemProvider = p.fileSystemProviderBase;
#endif
                string fullPath = GetFilePath(path);
#if !NET_STANDARD
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
                                var uiMsgBox = p.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
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
                    if (p.Protected || !Utilities.IO.FileSystem.IsXmlFile(fullPath))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fullPath));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            document = ReadFromStream(reader);
                            if (document == null)
                            {
#if !NET_STANDARD
                                var uiMsgBox = p.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
                            else if (!document.IsBelongFromParent(p))
                            {
                                document.Dispose();
#if !NET_STANDARD
                                var uiMsgBox = p.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
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
                                var uiMsgBox = p.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                if (uiMsgBox != null)
                                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                            }
                            else
                                document.FullPath = fullPath;
                        }
                    }
                }
                if (bCreateNew && document == null)
                    document = new ClientDocument() { FullPath = fullPath
#if !NET_STANDARD
                        , NeedsSave = true
#endif
                    };
                if(document != null)
                {
                    document.Parent = p;
                    document.EnsureValidValues();
                }
                return document;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = p.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, path);
#endif
                return null;
            }
        }

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }

#if !NET_STANDARD
        public bool SaveToFile(bool forceEncryption = false)
        {
            try
            {
                if (fileSystemProviderBase != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        if (!WriteDataStream(memoryStream))
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
                        if (!WriteDataStream(memoryStream))
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
                        if (!WriteDataStream(ostrm))
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

        bool WriteDataStream(Stream ostrm)
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
                    var serializer = new DataContractSerializer(typeof(ClientDocument));
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
#endif
        #endregion

        #region ICloneable Members

        ClientDocument(ClientDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new ClientDocument(this);
        }
        #endregion

        #region IDocument

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            var temp = Disposing;
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

        [Browsable(false)]
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

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
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

            // return absolute.MakeRelativeUri(new Uri(Path.GetDirectoryName(FullPath) + "\\"));
            return null;
        }

#if !NET_STANDARD
        [Browsable(false)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                if (Parent != null)
                    return Parent.fileSystemProviderBase;
                return new PhysicalFileSystemProvider("");
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

        [Browsable(false)]
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
                return false;
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

        #region IDisposable

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            OnDisposing(this);

            base.OnDispose();
        }

        #endregion
    }
}
