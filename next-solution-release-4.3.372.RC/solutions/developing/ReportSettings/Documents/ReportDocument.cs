using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using VFS;
using System.Windows.Controls;
using ReportSettings.PropertyDataTemplate;
using ReportManager.ComponentService;
using System.Windows.Media;
using UIMsgBoxAlertService.ComponentService;
using System.Windows;
#else
using log4net;
#endif
using System.ComponentModel;
using ViewModelLib;
using System.IO;
using UFInterfaces.Constants;
using System.Xml;
using DataReader;
using UFInterfaces;
using DocumentManager.ComponentService.Helpers;

namespace ReportSettings.Documents
{
    [DataContract(Name = "ReportDocument", Namespace = Namespaces.UriProgea)]
    public class ReportDocument : ViewModelBase,
#if !NET_STANDARD
        ICloneable, 
#endif
        IDocument, IEntityReference
    {
#region Declarations

        static int TempFilenameCount = 0;

#if NET_STANDARD
        static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.ReportExecuter);
#endif

#endregion

#region Constructors

        public ReportDocument()
        {
        }

#endregion

#region Persistance

        [DataMember]
        DataReaderModel readerItemSources;
        [DataMember]
        byte[] _ReportData;
        [DataMember]
        byte[] _ReportDataXML;
        [DataMember]
        Dictionary<String, byte[]> _SubReportDataXML;
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
        WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterScreen;
        [DataMember]
        WindowState windowState = WindowState.Maximized;
        [DataMember]
        WindowStyle windowStyle = WindowStyle.None;
#endif
        [DataMember]
        double width;
        [DataMember]
        double height;
        [DataMember]
        Guid id;

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            showInTaskbar = true;
#if !NET_STANDARD
            resizeMode = ResizeMode.CanResize;
            windowStartupLocation = WindowStartupLocation.CenterScreen;
            windowState = WindowState.Maximized;
            windowStyle = WindowStyle.None;
#endif
        }

#endregion

#region Methods

        bool IsBelongFromParent(IDocument parent)
        {
            return id == Guid.Empty || id == parent.Id;
        }
#if !NET_STANDARD
        public bool SaveCurrentDocument()
        {
            if (!NeedsSave)
                return true;

            return SaveToFile();
        }

        private bool WriteProjectDataStream(Stream ostrm)
        {
            /*
            var reportEditor = ActiveView as ReportEditorUI;
            if (reportEditor != null)
            {
                reportEditor.SaveCurrentDocument();
            }
            */

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
                    var serializer = new DataContractSerializer(typeof(ReportDocument));
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
                        if (!WriteProjectDataStream(memoryStream))
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
                        if (!WriteProjectDataStream(memoryStream))
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
                        return WriteProjectDataStream(ostrm);
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
#endif
#endregion

#region Static Methods

        static ReportDocument ReadFromStream(Stream stream)
        {
            try
            {
                var formatter = new DataContractSerializer(typeof(ReportDocument));
                var document = formatter.ReadObject(stream) as ReportDocument;
                return document;
            }
            catch (Exception ex)
            {
                return null;
            }
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

        public static ReportDocument FromFile(String fullPath, IDocument parent,
            bool embedded = false, bool create = true)
        {
            try
            {
                ReportDocument document = null;
#if !NET_STANDARD
                FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
                if (!embedded && fileSystemProvider != null)
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

                    if (document == null && create)
                    {
                        document = new ReportDocument()
                        {
                            FullPath = fullPath
                        };
                    }

                    return document;
                }
#endif

                if (File.Exists(fullPath))
                {
                    try
                    {
                        bool isXml = Utilities.IO.FileSystem.IsXmlFile(fullPath);
                        using (var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            if (embedded)
                                using (var streamReader = new StreamReader(fileStream))
                                {
                                    string result = WPFUtilities.CryptString.CryptString.DecryptString(streamReader.ReadToEnd());
                                    Encoding utf8noBOM = new UTF8Encoding(true);
                                    using (MemoryStream output = new MemoryStream(utf8noBOM.GetBytes(result)))
                                    {
                                        document = ReadFromStream(output);
                                        if (document == null)
                                        {
#if !NET_STANDARD
                                            var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                            if (uiMsgBox != null)
                                                uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                            log.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                                        }
                                        else
                                            document.FullPath = fullPath;
                                    }
                                }
                            else
                            {
                                if (parent.Protected || !isXml)
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
                                            log.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
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
                                            log.ErrorFormat(Properties.Resources.ErrorValidatingDocument, fullPath);
#endif
                                            return null;
                                        }
                                        else
                                            document.FullPath = fullPath;
                                    }
                                }
                                else
                                {
                                    document = ReadFromStream(fileStream);
                                    if (document == null)
                                    {
#if !NET_STANDARD
                                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                        if (uiMsgBox != null)
                                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                                        log.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                                    }
                                    else
                                        document.FullPath = fullPath;
                                }
                            }
                        }
                    }
                    catch
                    {
#if !NET_STANDARD
                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                        if (uiMsgBox != null)
                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, fullPath));
#else
                        log.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
#endif
                    }
                }

                if (document == null && create)
                {
                    document = new ReportDocument()
                    {
                        FullPath = fullPath
                    };
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
                log.ErrorFormat(Properties.Resources.ErrorReadingDocument, fullPath);
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
#if !NET_STANDARD
                OnPropertyChanged("NeedsSave");
#endif
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

        [Browsable(false)]
        public DataReaderModel ReaderItemSources
        {
            get 
            {
                if (readerItemSources == null)
                    readerItemSources = new DataReaderModel();

                return readerItemSources; 
            }
            set
            {
                /*
                if (value == readerItemSources)
                    return;
                */
                readerItemSources = value;
                if (readerItemSources != null)
                {
                    readerItemSources.DataSourceName =
                        readerItemSources.DataSourceDisplayName = String.Format("{0} {1}", Title, "Data Source");
                }
#if !NET_STANDARD
                OnPropertyChanged("ReaderItemSources");
                NeedsSave = true;
#endif
            }
        }

#if !NET_STANDARD
        DataReaderReference readerItemSourceReference;
        [Browsable(true)]
        public DataReaderReference ReaderItemSourceReference
        {
            get
            {
                if (readerItemSourceReference == null)
                    readerItemSourceReference = new DataReaderReference(this);

                return readerItemSourceReference;
            }
        }
#endif

        [Browsable(false)]
        public byte[] ReportData
        {
            get
            {
                return _ReportData;
            }
            set
            {
                if (ReportData == value)
                    return;
                _ReportData = value;
#if !NET_STANDARD
                OnPropertyChanged("ReportData");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public byte[] ReportDataXML
        {
            get
            {
                return _ReportDataXML;
            }
            set
            {
                if (ReportDataXML == value)
                    return;
                _ReportDataXML = value;
#if !NET_STANDARD
                OnPropertyChanged("ReportDataXML");
                NeedsSave = true;
#endif
            }
        }

        [Browsable(false)]
        public Dictionary<String, byte[]> SubReportDataXML
        {
            get
            {
                return _SubReportDataXML;
            }
            set
            {
                if (_SubReportDataXML == value)
                    return;
                _SubReportDataXML = value;
#if !NET_STANDARD
                OnPropertyChanged("SubReportDataXML");
                NeedsSave = true;
#endif
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

        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                if (top == value)
                    return;
                top = value;
#if !NET_STANDARD
                OnPropertyChanged("Top");
                NeedsSave = true;
#endif
            }
        }

        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                if (left == value)
                    return;
                left = value;
#if !NET_STANDARD
                OnPropertyChanged("Left");
                NeedsSave = true;
#endif
            }
        }

        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                if (width == value)
                    return;
                width = value;
#if !NET_STANDARD
                OnPropertyChanged("Width");
                NeedsSave = true;
#endif
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                if (height == value)
                    return;
                height = value;
#if !NET_STANDARD
                OnPropertyChanged("Height");
                NeedsSave = true;
#endif
            }
        }

#if !NET_STANDARD
        public ResizeMode ResizeMode
        {
            get
            {
                return resizeMode;
            }
            set
            {
                if (resizeMode == value)
                    return;
                resizeMode = value;
                OnPropertyChanged("ResizeMode");
                NeedsSave = true;
            }
        }

        public WindowStartupLocation WindowStartupLocation
        {
            get
            {
                return windowStartupLocation;
            }
            set
            {
                if (windowStartupLocation == value)
                    return;
                windowStartupLocation = value;
                OnPropertyChanged("WindowStartupLocation");
                NeedsSave = true;
            }
        }

        public WindowState WindowState
        {
            get
            {
                return windowState;
            }
            set
            {
                if (windowState == value)
                    return;
                windowState = value;
                OnPropertyChanged("WindowState");
                NeedsSave = true;
            }
        }

        public WindowStyle WindowStyle
        {
            get
            {
                return windowStyle;
            }
            set
            {
                if (windowStyle == value)
                    return;
                windowStyle = value;
                OnPropertyChanged("WindowStyle");
                NeedsSave = true;
            }
        }
#endif
#endregion

#region ICloneable Members

        ReportDocument(ReportDocument template)
        {
            if (template == null)
                return;
            /*
            ConnectionSettings = template.ConnectionSettings;
            DataSourceName = template.DataSourceName;
            TableInfoTableName = template.TableInfoTableName;
            TableInfoSchemaName = template.TableInfoSchemaName;
            TableInfoColumns = template.TableInfoColumns;
            */
            ReaderItemSources = template.ReaderItemSources;
            ReportData = template.ReportData;
            ReportDataXML = template.ReportDataXML;
            SubReportDataXML = template.SubReportDataXML;
        }

#if !NET_STANDARD
        public object Clone()
        {
            return new ReportDocument(this);
        }
#endif
#endregion

#region Validations
#if !NET_STANDARD
        public override string Error
        {
            get
            {
                return null;
            }
        }

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
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

        String lastSession;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public string SessionString
        {
            get
            {
                if (!String.IsNullOrEmpty(lastSession))
                {
                    return lastSession;
                }

                var docParent = Parent;
                if (Parent != null)
                    docParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                lastSession = docParent != null ? docParent.Title : Title;
                return lastSession;
            }
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

        public new String Title
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
                return ReportData == null || ReportData.Length == 0;
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

        #region IEntityReference Members

        [Browsable(false)]
        public ImageSource CollapsedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ImageSource ExpandedImageSource
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public ContextMenu contextMenu
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object Tooltip
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object ContainedObject
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public object EntityParent
        {
            get
            {
                return null;
            }
        }

        [Browsable(false)]
        public string TypeDefinitionString
        {
            get
            {
                return null;
            }
        }

#endregion
    }
}
