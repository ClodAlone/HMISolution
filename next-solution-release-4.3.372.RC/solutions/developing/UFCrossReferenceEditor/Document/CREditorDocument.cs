using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
using System.Windows.Controls;
using VFS;
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using UFCrossReferenceEditor.ComponentService;
using System.Windows;
using System.Xml;
using Utilities;
using PropertyControl.ComponentService;
using UFCrossReferenceModel;
using UFProjectManager.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using System.Threading;
using OPCUAViewModel;
using System.Windows.Threading;
using ExpressionManager;
using StringManager.ComponentService;

namespace UFCrossReferenceEditor.Document
{   
    //[DataContract(Name = "CREditorDocument", Namespace = Namespaces.UriProgea)]
    public class CREditorDocument : ViewModelBase, IDocument, ICloneable
    {
        #region Declarations

        UnitOfWork uow;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemory;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dl;
        IDataLayer dlClipboard;
        String connectionString;
        String fileBase;

        private readonly ILocalTagParser _localTagParser = new LocalTagParser();
        IUFProjectManager iUFProjectManager;
        IStringEditorManager stringeditorManager;
        List<string> tagList = new List<string>();
        Dictionary<string, List<string>> resourceList = null;
        List<string> stringList = null;
        Dictionary<String, List<String>> prototypelist = null;
        List<string> dataSincInterfaces = new List<string>();
        Dictionary<string, List<string>> mapDataSinkTagList = new Dictionary<string, List<string>>();
        Dictionary<string, DataSinkInterface> mapDataSinkInterfaces = new Dictionary<string, DataSinkInterface>();
        #endregion
        #region Methods
        IDataLayer GetDataLayer(ref InMemoryDataStore inMemoryDataStore, bool readFromFile = true)
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFCrossReferenceModel.UFCrossReferenceTag).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                //dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                return new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);
            }
            else
            {
                lock (lockObject)
                {
                    if (inMemoryDataStore == null)
                    {
                        inMemoryDataStore = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        if (readFromFile && File.Exists(fileBase))
                        {
                            if (Protected || !Utilities.IO.FileSystem.IsXmlFile(fileBase))
                            {
                                var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                                using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                                {
                                    var xmlreader = XmlReader.Create(reader);
                                    try
                                    {
                                        inMemoryDataStore.ReadXml(xmlreader);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            else
                                try
                                {
                                    inMemoryDataStore.ReadXml(fileBase);
                                }
                                catch (Exception ex)
                                {

                                }
                        }
                    }
                }
                return new DevExpress.Xpo.ThreadSafeDataLayer(dict, inMemoryDataStore);
            }
        }
        void CreateDataLayer()
        {
            //DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            //dict.GetDataStoreSchema(typeof(UFCrossReferenceModel.UFCrossReferenceTag).Assembly);

            //dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);

            dl = GetDataLayer(ref InMemory);
            uow = new UnitOfWork(dl);
            //uowCloner = new UnitOfWork(dl);
            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);

            //InitDataSyncReferences();
        }
        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        #region WinClipboard

        internal void CopyInMemoryDataToWinClipboard()
        {
            Clipboard.Clear();
            using (var xml = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(xml) { Formatting = System.Xml.Formatting.Indented })
                {
                    InMemoryClipboard.WriteXml(xmlTextWriter);
                    xmlTextWriter.Flush();
                    xmlTextWriter.Close();
                }

                Clipboard.SetText(xml.ToString());
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData(bool force = false)
        {
            try
            {
                if (force || Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
                    Clipboard.GetText(TextDataFormat.UnicodeText) != LastClipboardUnicodeText)
                {
                    CleanClipbaord();
                    LastClipboardUnicodeText = Clipboard.GetText(TextDataFormat.UnicodeText);
                    using (var xml = new StringReader(Clipboard.GetText(TextDataFormat.UnicodeText)))
                    {
                        using (var xmlTextReader = new XmlTextReader(xml))
                        {
                            var tempds = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                            //using (var tempdl = new SimpleDataLayer(tempds))
                            {
                                bool bValid = true;
                                try
                                {
                                    tempds.ReadXml(xmlTextReader);
                                }
                                catch
                                {
                                    bValid = false;
                                }

                                if (bValid)
                                {
                                    InMemoryClipboard.ReadFromInMemoryDataStore(tempds);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();

            /*
            var listFolders = (from c in new XPQuery<UFCrossReferenceTag>(uowClipboard).AsParallel() select c).ToList();
            listFolders.ForEach(entry => { entry.Delete(); });
            
            var listTags = (from c in new XPQuery<UFCrossReferenceEntity>(uowClipboard).AsParallel() select c).ToList();
            listTags.ForEach(entry => { entry.Delete(); });

            var listAlarms = (from c in new XPQuery<UFUAModel.UFUAAlarmDefinition>(uowClipboard).AsParallel() select c).ToList();
            listAlarms.ForEach(entry => { entry.Delete(); });

            var listPrototype = (from c in new XPQuery<UFCrossReferenceEntityPrototype>(uowClipboard).AsParallel() select c).ToList();
            listPrototype.ForEach(entry => { entry.Delete(); });

            uowClipboard.CommitChanges();
            uowClipboard.PurgeDeletedObjects();
            */
        }

        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, uow, checkattributes);
            return cloneHelper.Clone(obj, false);
        }

        #endregion

        static String GetConnectionString(String path, FileSystemProviderBase vfs)
        {
            String connString = null;
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        static String GetBaseFilename(String path, FileSystemProviderBase vfs)
        {
            if (vfs != null)
                return null;

            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, UFInterfaces.IWorkspace work = null, IDocumentManager manager = null)
        {
            string ret = string.Empty;
            using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
            {
                if (sourceDoc == null)
                    return;

                // TODO: code for copying all document content
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
        }

        public static void RenameFile(String fullPath, String oldName, String newName,
            FileSystemProviderBase fileSystemProvider = null)
        {
        }

        public static void RemoveFile(string fullPath, IDocument parent, IDocumentManager manager = null)
        {
            FileSystemProviderBase fileSystemProvider = parent.fileSystemProviderBase;
            var file = GetBaseFilename(fullPath, fileSystemProvider);
            if (!String.IsNullOrEmpty(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
                {
                    if (sourceDoc == null)
                        return;

                    // TODO: code for removing all document content
                }
            }
        }

        public static CREditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

                String connString = null;
                String xmlfile = null;
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
                if (vfs != null && vfs is DataSourceFileSystemProvider)
                {
                    connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
                }
                else
                {
                    xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                        Properties.Settings.Default.TypeLabel,
                                        Properties.Settings.Default.DefaultProjectName,
                                        Properties.Settings.Default.DefaultFileExt);
                    if (!bCreateNew && !File.Exists(xmlfile))
                        return null;
                    connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
                }

                var doc = new CREditorDocument(null)
                {
                    connectionString = connString,
                    EditorManagerComponent = c as CrossReferenceEditorManagerComponent,
                    fileBase = xmlfile,
                    Parent = parent
                };

                doc.CreateDataLayer();
                if (!bCreateNew && bCheckEmpty && doc.IsEmpty)
                {
                    doc.Dispose();
                    return null;
                }
                else if (vfs == null && !doc.IsBelongFromParent(parent))
                {
                    doc.Dispose();
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
                    return null;
                }

                return doc;
            }
            catch
            {
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
                return null;
            }
        }

        internal bool SaveToFile(bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return true;
            try
            {
                bool bSave = NeedsSave;
                NeedsSave = false;

                if (!bForceSave && !bSave)
                    return true;

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (forceEncryption || Protected)
                        XpoHelpers.XpoHelper.AddProtectionCode(uow, Id);
                    else
                        XpoHelpers.XpoHelper.RemoveProtectionCode(uow);
                }

                uow.CommitChanges();

                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (File.Exists(fileBase))
                    {
                        try
                        {
                            File.Delete(fileBase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (forceEncryption || Protected)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            var writer = XmlWriter.Create(memoryStream);
                            InMemory.WriteXml(writer);
                            writer.Flush();
                            writer.Close();
                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            File.WriteAllText(fileBase, toWrite);
                        }
                    }
                    else
                        InMemory.WriteXml(fileBase);
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

        internal void RemoveString(UFCrossReferenceString tag)
        {
            tag.Delete();
        }

        internal CRTagFolder AddNewCRTagFolder(string path)
        {
            CRTagFolder _lfolder;
            _lfolder = (from folder in tagFolderRefMap
                        where folder.Key == path
                        select folder.Value).FirstOrDefault();


            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            CRTagFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewCRTagFolder(parent);

            var foldername = System.IO.Path.GetFileName(path);
            var _folder = new CRTagFolder() { Name = foldername, IsNotInUse = true };
            tagFolderRefMap.Add(path, _folder);
            if (parentfolder != null)
            {
                parentfolder.UFUAFolders.Add(_folder);
                _folder.UFUAFolderAss = parentfolder;
            }
            return _folder;
        }

        //internal CRStringFolder AddNewCRStringFolder(string path)
        //{
        //    CRStringFolder _lfolder;
        //    _lfolder = (from folder in stringFolderRefMap
        //                where folder.Key == path
        //                select folder.Value).FirstOrDefault();

        //    if (_lfolder != null)
        //        return _lfolder;

        //    var parent = System.IO.Path.GetDirectoryName(path);
        //    CRStringFolder parentfolder = null;
        //    if (!string.IsNullOrEmpty(parent))
        //        parentfolder = AddNewCRStringFolder(parent);

        //    var foldername = System.IO.Path.GetFileName(path);
        //    var _folder = new CRStringFolder() { Name = foldername, IsNotInUse = true };
        //    if (parentfolder != null)
        //    {
        //        parentfolder.UFUAFolders.Add(_folder);
        //        _folder.UFUAFolderAss = parentfolder;
        //    }
        //    return _folder;
        //}

        internal CRScreenFolder AddNewCRScreenFolder(string path)
        {
            CRScreenFolder _lfolder;
            _lfolder = (from folder in screenFolderRefMap
                        where folder.Key == path
                        select folder.Value).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            CRScreenFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewCRScreenFolder(parent);

            var foldername = System.IO.Path.GetFileName(path);
            var _folder = new CRScreenFolder() { Name = foldername };
            if (parentfolder != null)
            {
                parentfolder.UFUAFolders.Add(_folder);
                _folder.UFUAFolderAss = parentfolder;
            }
            return _folder;
        }

        internal CRConnectionFolder AddNewCRConnectionFolder(string path)
        {
            CRConnectionFolder _lfolder;
            _lfolder = (from folder in connectionFolderRefMap
                        where folder.Key == path
                        select folder.Value).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            //var parent = System.IO.Path.GetDirectoryName(path);
            //CRConnectionFolder parentfolder = null;
            //if (!string.IsNullOrEmpty(parent))
            //    parentfolder = AddNewCRConnectionFolder(parent);

            var foldername = path; // System.IO.Path.GetFileName(path);
            var _folder = new CRConnectionFolder() { Name = foldername };
            //if (parentfolder != null)
            //{
            //    parentfolder.UFUAFolders.Add(_folder);
            //    _folder.UFUAFolderAss = parentfolder;
            //}
            return _folder;
        }


        internal UFCrossReferenceTagFolder AddNewTagFolder(string path, UnitOfWork uow)
        {
            UFCrossReferenceTagFolder _lfolder;
            _lfolder = (from folder in new XPQuery<UFCrossReferenceTagFolder>(uow, true)//.AsParallel()
                        select folder).ToList().Where(folder => folder.GetRelativeName() == path).FirstOrDefault();


            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            UFCrossReferenceTagFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewTagFolder(parent, uow);

            var foldername = System.IO.Path.GetFileName(path);
            var _folder = new UFCrossReferenceTagFolder(uow) { Name = foldername};
            if (parentfolder != null)
                parentfolder.UFUAFolders.Add(_folder);
            return _folder;
        }

        internal UFCrossReferenceStringFolder AddNewStringFolder(string path, UnitOfWork uow)
        {
            UFCrossReferenceStringFolder _lfolder;
            _lfolder = (from folder in new XPQuery<UFCrossReferenceStringFolder>(uow, true)//.AsParallel()
                        select folder).ToList().Where(folder => folder.GetRelativeName() == path).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            UFCrossReferenceStringFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewStringFolder(parent, uow);

            var foldername = System.IO.Path.GetFileName(path);
            var _folder = new UFCrossReferenceStringFolder(uow) { Name = foldername };
            if (parentfolder != null)
                parentfolder.UFUAFolders.Add(_folder);
            return _folder;
        }

        internal UFCrossReferenceScreenFolder AddNewScreenFolder(string path, UnitOfWork uow)
        {
            UFCrossReferenceScreenFolder _lfolder;
            _lfolder = (from folder in new XPQuery<UFCrossReferenceScreenFolder>(uow, true)//.AsParallel()
                        select folder).ToList().Where(folder => folder.GetRelativeName() == path).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            var parent = System.IO.Path.GetDirectoryName(path);
            UFCrossReferenceScreenFolder parentfolder = null;
            if (!string.IsNullOrEmpty(parent))
                parentfolder = AddNewScreenFolder(parent,uow);

            var foldername = System.IO.Path.GetFileName(path);
            var _folder = new UFCrossReferenceScreenFolder(uow) { Name = foldername };
            if (parentfolder != null)
                parentfolder.UFUAFolders.Add(_folder);
            return _folder;
        }

        internal UFCrossReferenceConnectionFolder AddNewConnectionFolder(string path, UnitOfWork uow)
        {
            UFCrossReferenceConnectionFolder _lfolder;
            _lfolder = (from folder in new XPQuery<UFCrossReferenceConnectionFolder>(uow, true)//.AsParallel()
                        select folder).ToList().Where(folder => folder.GetRelativeName() == path).FirstOrDefault();

            if (_lfolder != null)
                return _lfolder;

            //var parent = System.IO.Path.GetDirectoryName(path);
            //UFCrossReferenceConnectionFolder parentfolder = null;
            //if (!string.IsNullOrEmpty(parent))
            //    parentfolder = AddNewConnectionFolder(parent, uow);

            var foldername = path; // System.IO.Path.GetFileName(path);
            var _folder = new UFCrossReferenceConnectionFolder(uow) { Name = foldername };
            //if (parentfolder != null)
            //    parentfolder.UFUAFolders.Add(_folder);
            return _folder;
        }

        internal string GetRelativePath(string value, 
            out string typeTitle,
            out string localExpressionAppName,
            out string expressionLocalTagRelativePath)
        {
            typeTitle = null;
            localExpressionAppName = null;
            expressionLocalTagRelativePath = null;

            if (!string.IsNullOrEmpty(value))
            {
                string newValue = "";
                if (value.Contains("Tags/") || value.Contains("Tag/"))
                    newValue = NamespaceTableConverter.GetSanitizedValue(value);
                else
                    newValue = value;

                if (newValue.StartsWith("Tags/", StringComparison.OrdinalIgnoreCase))
                    newValue = newValue.Remove(0, 5);
                else if (newValue.StartsWith("Tag/", StringComparison.OrdinalIgnoreCase))
                    newValue = newValue.Remove(0, 4);
                else
                {
                    IDocumentManager manager = null;

                    try
                    {
                        manager = CrossReferenceEditorManagerComponent.UriRisolver.ResolveUri(new Uri(value,
                            UriKind.RelativeOrAbsolute)) as IDocumentManager;
                    }
                    catch
                    {
                    }

                    if (manager != null)
                    {
                        typeTitle = manager.TypeTitle;
                        if (newValue.StartsWith(String.Format("{0}/", manager.TypeLabel), StringComparison.OrdinalIgnoreCase))
                            newValue = newValue.Substring(manager.TypeLabel.Length + 1);
                    }
                    else
                    {
                        var entityRef = _localTagParser.Parse(value);
                        if (entityRef != null && entityRef.IsSystemVariable)
                        {
                            newValue = entityRef.HumanReadableNoProject;
                            localExpressionAppName = value.Split('.')[0];
                            expressionLocalTagRelativePath = value;
                        }
                        else if (entityRef != null)
                        {
                            newValue = entityRef.RelativePath;
                            localExpressionAppName = entityRef.AppName;
                            expressionLocalTagRelativePath = entityRef.RelativePath;
                        }
                    }
                }

                return newValue;
            }

            return String.Empty;
        }
        static public bool ContainsAlias(String val)
        {
            return !String.IsNullOrEmpty(val) && val.IndexOf(Properties.Settings.Default.OpenTagAlias) != -1;
        }
        Dictionary<string, CRTag> tagRefMap = new Dictionary<string, CRTag>();
        Dictionary<string, CRConnection> connectionRefMap = new Dictionary<string, CRConnection>();
        Dictionary<string, CRScreen> screenRefMap = new Dictionary<string, CRScreen>();
        Dictionary<string, CRTagFolder> tagFolderRefMap = new Dictionary<string, CRTagFolder>();
        List<CREntity> crEntities = new List<CREntity>();
        Dictionary<string, CRConnectionFolder> connectionFolderRefMap = new Dictionary<string, CRConnectionFolder>();
        Dictionary<string, CRScreenFolder> screenFolderRefMap = new Dictionary<string, CRScreenFolder>();
        //Dictionary<string, CRStringFolder> stringFolderRefMap = new Dictionary<string, CRStringFolder>();
        Dictionary<string, CRString> stringRefMap = new Dictionary<string, CRString>(); 
        internal void AddEntities(List<UFInterfaces.Editors.CrossReferenceResultModel> rawdata, CancellationToken ct)
        {
            if (rawdata == null)
                return;

            rawdata.RemoveAll((data) => data == null);
            while (!ct.IsCancellationRequested && rawdata.Count > 0)
            {
                //using (var uow = new UnitOfWork(dlCompiler))
                {
                    var relativePath = GetRelativePath(rawdata[0].RelativePath,
                        out var typeTitle,
                        out var expressionLocalTagAppName,
                        out var expressionLocalTagRelativePath);

                    if (!string.IsNullOrEmpty(expressionLocalTagAppName) &&
                        !string.IsNullOrEmpty(expressionLocalTagRelativePath))
                    {
                        rawdata[0].AppName = expressionLocalTagAppName;
                        rawdata[0].RelativePath = expressionLocalTagRelativePath;
                    }

                    string appName = rawdata[0].AppName;
                    string endpointUrl = rawdata[0].EndpointUrl;
                    string name = string.Empty;

                    string comparePath = rawdata[0].RelativePath;
                    CrossReferenceType compareCReferenceType = rawdata[0].CReferenceType;
                    string compareAppName = rawdata[0].AppName;
                    string compareEndpointUrl = rawdata[0].EndpointUrl;


                    var aggregated = (from data in rawdata//.AsParallel()
                                        where data.RelativePath == comparePath && 
                                            data.CReferenceType == compareCReferenceType &&
                                            data.AppName == compareAppName &&
                                            data.EndpointUrl == compareEndpointUrl
                                      select data).ToList();
                    if (aggregated.Count > 0)
                    {
                        if(rawdata[0].CReferenceType == CrossReferenceType.Tags && !string.IsNullOrEmpty(relativePath))
                        {
                            relativePath = relativePath?.Replace('/','\\');
                            relativePath = relativePath?.Replace("\\\\", "\\");
                            if (relativePath.StartsWith("\\"))
                            {
                                if(!defAppName.Equals(appName))
                                    relativePath = $"{appName}{relativePath}";
                                else
                                    relativePath = relativePath.Remove(0,1);
                            }
                            string tagfolderpath = string.Empty;
                            name = relativePath;
                            if (!ContainsAlias(relativePath))
                            {
                                tagfolderpath = System.IO.Path.GetDirectoryName(relativePath);
                                name = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                            }

                            string readablePathNoProject = relativePath;
                            bool isDatasync = false;
                            if (dataSincInterfaces.Contains(appName))
                            {
                                isDatasync = true;
                                readablePathNoProject = relativePath?.Replace('&', '\\');
                                relativePath = mapDataSinkInterfaces.ContainsKey(appName)
                                    ? mapDataSinkInterfaces[appName].DataSynkName + "\\" + readablePathNoProject
                                    : string.Empty + "\\" + readablePathNoProject;

                                tagfolderpath = string.Empty;
                                name = relativePath;

                                if (!ContainsAlias(relativePath))
                                {
                                    tagfolderpath = System.IO.Path.GetDirectoryName(relativePath);
                                    name = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                                }
                            }

                            bool bCreateFolder = tagfolderpath?.Length > 0;

                            relativePath = string.Format("{0} ({1})", relativePath, rawdata[0].AppName);

                            CRTagFolder folder = null;
                            CRTag entity = null;

                            //using (var uow = new UnitOfWork(dlCompiler))
                            {
                                if (bCreateFolder)
                                {
                                    lock (tagFolderRefMap)
                                    {
                                        if (tagFolderRefMap.ContainsKey(tagfolderpath))
                                            folder = tagFolderRefMap[tagfolderpath];

                                        if (folder == null)
                                            folder = AddNewCRTagFolder(tagfolderpath);

                                        tagFolderRefMap[tagfolderpath] = folder;
                                    }
                                }

                                lock (tagRefMap)
                                {
                                    string key = $"{relativePath}\\{endpointUrl}";
                                    if (tagRefMap.ContainsKey(key))
                                        entity = tagRefMap[key];
                                    if (entity == null)
                                    {
                                        entity = new CRTag()
                                        {
                                            Name = name,
                                            EndpointUrl = endpointUrl,
                                            ReadablePathNoProject = readablePathNoProject,
                                            ReadablePath = relativePath,
                                            TypeIcon = "AddressSpace",
                                            AppName = appName,
                                            UFUAFolder = folder,
                                            IsNotValid = true,
                                            IsNotInUse = true
                                        };
                                        if (folder != null)
                                            folder.Tags.Add(entity);
                                    }

                                    if (rawdata[0].HasPrototypeModel)
                                        entity.HasPrototypeModel = true;

                                    tagRefMap[key] = entity;
                                }

                                if (entity.HasPrototypeModel)
                                    lock (tagFolderRefMap)
                                    {
                                        CRTagFolder newfolder = null;
                                        tagfolderpath = tagfolderpath?.Length > 0 ? $"{tagfolderpath}\\{name}" : name;
                                        if (tagFolderRefMap.ContainsKey(tagfolderpath))
                                            newfolder = tagFolderRefMap[tagfolderpath];
                                        if (newfolder == null)
                                            newfolder = AddNewCRTagFolder(tagfolderpath);
                                        tagFolderRefMap[tagfolderpath] = newfolder;
                                        if (!string.IsNullOrEmpty(rawdata[0].TypeDefinition))
                                            newfolder.TypeDefinition = rawdata[0].TypeDefinition;
                                        else
                                            newfolder.TypeDefinition = "AddressSpace";

                                        entity.PrototypeFolder = newfolder;
                                        newfolder.HasPrototypeModel = true;
                                    }

                                if (!entity.IsInitialized && isDatasync && entity.IsNotValid)
                                {
                                    entity.IsNotValid = !mapDataSinkTagList.ContainsKey(entity.AppName) || !mapDataSinkTagList[entity.AppName].Contains(entity.ReadablePathNoProject.Replace('\\', '&'));
                                    entity.IsInitialized = true;
                                }

                                foreach (var entry in aggregated)
                                {
                                    if (!entity.IsInitialized && entry.IconType == DocManagerType.UFUAServer.ToString())
                                    {
                                        entity.IsNotValid = false;
                                        entity.IsInitialized = true;
                                    }
                                    else if (!entity.IsInitialized && IsServerTag(entry.ReferencedNodeId))
                                    {
                                        entity.IsNotValid = false;
                                        entity.IsInitialized = true;
                                        entity.IsNotInUse = false;
                                        if (folder != null && folder.IsNotInUse)
                                            ResetNoInUse<CRTagFolder>(folder);
                                    }
                                    else if (!isDatasync || entry.IconType != DocManagerType.UFUAServer.ToString())
                                    {
                                        if (folder != null && folder.IsNotInUse)
                                            ResetNoInUse<CRTagFolder>(folder);
                                        entity.IsNotInUse = false;
                                    }

                                    Entity newEntity = new Entity()
                                    {
                                        Appname = appName,
                                        Name = name,
                                        RelativePathNoProject = $"{readablePathNoProject}",
                                        EndpointUrl = endpointUrl,
                                        RelativePath = $"{relativePath}",
                                        ReferencedNodeID = entry.ReferencedNodeId,
                                        ContainerDoc = entry.ContainerDoc,
                                        Crtype = entry.CReferenceType,
                                        Description = entry.Description,
                                        Settings = entry.Settings,
                                        EType = entry.IconType,
                                    };
                                    if (!string.IsNullOrEmpty(entry.TypeDefinition))
                                    {
                                        entity.TypeDefinition = entry.TypeDefinition;
                                        entity.HasPrototypeModel = entry.HasPrototypeModel;
                                    }
                                    AddNewCREntity(newEntity, entity);
                                }
                            }
                        }
                        else if (rawdata[0].CReferenceType == CrossReferenceType.Resources)
                        {
                            string tagfolderpath = relativePath;
                            name = relativePath;

                            if (!ContainsAlias(relativePath))
                            {
                                tagfolderpath = System.IO.Path.GetDirectoryName(relativePath);
                                if (!String.IsNullOrEmpty(typeTitle))
                                {
                                    if (!String.IsNullOrEmpty(tagfolderpath))
                                        tagfolderpath = String.Format("{0}\\{1}", typeTitle, tagfolderpath);
                                    else
                                        tagfolderpath = typeTitle;
                                }
                                name = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                            }

                            bool bCreateFolder = tagfolderpath?.Length > 0;

                            CRScreenFolder folder = null;
                            CRScreen screenentity = null;
                            //using (var uow = new UnitOfWork(dlCompiler))
                            {
                                if (bCreateFolder)
                                {
                                    lock (screenFolderRefMap)
                                    {
                                        if (screenFolderRefMap.ContainsKey(tagfolderpath))
                                            folder = screenFolderRefMap[tagfolderpath];
                                        if (folder == null)
                                            folder = AddNewCRScreenFolder(tagfolderpath);
                                        screenFolderRefMap[tagfolderpath] = folder;
                                    }
                                }

                                var relativePathPrj = string.Format("{0} ({1})", relativePath, appName);

                                lock (screenRefMap)
                                {
                                    if (screenRefMap.ContainsKey(relativePath))
                                        screenentity = screenRefMap[relativePath];
                                    if (screenentity == null)
                                        screenentity = new CRScreen() { Name = name, ReadablePath = relativePath, TypeIcon = "ScreenManager", ReadablePathProject = relativePathPrj, UFUAFolder = folder };
                                    screenRefMap[relativePath] = screenentity;
                                }

                                foreach (var entry in aggregated)
                                {
                                    Entity newEntity = new Entity()
                                    {
                                        Appname = appName,
                                        Name = name,
                                        RelativePathNoProject = relativePath,
                                        RelativePath = string.Format("{0} ({1})", relativePath, entry.AppName),
                                        ContainerDoc = entry.ContainerDoc,
                                        Crtype = entry.CReferenceType,
                                        Description = entry.Description,
                                        Settings = entry.Settings,
                                        EType = entry.IconType
                                    };

                                    AddNewCREntity(newEntity, screenentity);
                                }
                            }
                        }
                        else if (rawdata[0].CReferenceType == CrossReferenceType.Connections)
                        {
                            relativePath = relativePath.ToLower();
                            if (!relativePath.EndsWith(";"))
                                relativePath = $"{relativePath};";
                            bool bCreateFolder = relativePath?.Length > 0;
                            CRConnectionFolder folder = null;
                            //using (var uow = new UnitOfWork(dlCompiler))
                            {
                                if (bCreateFolder)
                                {
                                    lock (connectionFolderRefMap)
                                    {
                                        if (connectionFolderRefMap.ContainsKey(relativePath))
                                            folder = connectionFolderRefMap[relativePath];
                                        if (folder == null)
                                            folder = AddNewCRConnectionFolder(relativePath);
                                        connectionFolderRefMap[relativePath] = folder;
                                    }
                                }

                                var relativePathPrj = string.Format("{0} ({1})", relativePath, appName);
                                CRConnection connectionentity = null;

                                foreach (var entry in aggregated)
                                {
                                    name = entry.Name;
                                    string key = $"{relativePath}\\{name}";
                                    lock (connectionRefMap)
                                    {
                                        if (connectionRefMap.ContainsKey(key))
                                            connectionentity = connectionRefMap[key];
                                        if (connectionentity == null)
                                            connectionentity = new CRConnection() { Name = name, ReadablePath = relativePath, TypeIcon = "ScreenManager", ReadablePathProject = relativePathPrj, UFUAFolder = folder };
                                        connectionRefMap[key] = connectionentity;
                                    }

                                    Entity newEntity = new Entity()
                                    {
                                        Appname = appName,
                                        Name = name,
                                        RelativePathNoProject = relativePath,
                                        RelativePath = string.Format("{0} ({1})", relativePath, entry.AppName),
                                        ContainerDoc = entry.ContainerDoc,
                                        Crtype = entry.CReferenceType,
                                        Description = entry.Description,
                                        Settings = entry.Settings,
                                        EType = entry.IconType
                                    };

                                    AddNewCREntity(newEntity, connectionentity);
                                }
                            }
                        }
                        else if (rawdata[0].CReferenceType == CrossReferenceType.Strings)
                        {
                            if (!string.IsNullOrEmpty(relativePath))
                            {
                                string tagfolderpath = relativePath;
                                name = relativePath;

                                //if (!ContainsAlias(relativePath))
                                //{
                                //    tagfolderpath = System.IO.Path.GetDirectoryName(relativePath);
                                //    name = System.IO.Path.GetFileNameWithoutExtension(relativePath);
                                //}

                                //bool bCreateFolder = tagfolderpath?.Length > 0;

                                CRStringFolder folder = null;
                                CRString stringentity = null;
                                //using (var uow = new UnitOfWork(dlCompiler))
                                {
                                    //if (bCreateFolder)
                                    //{
                                    //    lock (stringFolderRefMap)
                                    //    {
                                    //        if (stringFolderRefMap.ContainsKey(tagfolderpath))
                                    //            folder = stringFolderRefMap[tagfolderpath];
                                    //        if (folder == null)
                                    //            folder = AddNewCRStringFolder(tagfolderpath);
                                    //        stringFolderRefMap[tagfolderpath] = folder;
                                    //    }
                                    //}

                                    var relativePathPrj = string.Format("{0} ({1})", relativePath, appName);

                                    lock (stringRefMap)
                                    {
                                        if (stringRefMap.ContainsKey(relativePath))
                                            stringentity = stringRefMap[relativePath];
                                        if (stringentity == null)
                                        {
                                            stringentity = new CRString()
                                            {
                                                Name = name,
                                                ReadablePath = relativePath,
                                                TypeIcon = "StringManager",
                                                ReadablePathProject = relativePathPrj,
                                                UFUAFolder = folder,
                                                IsNotValid = true,
                                                IsNotInUse = true
                                            };
                                            if (folder != null)
                                                folder.Screens.Add(stringentity);
                                        }
                                        stringRefMap[relativePath] = stringentity;
                                    }

                                    foreach (var entry in aggregated)
                                    {
                                        if (entry.IconType == DocManagerType.StringManager.ToString())
                                        {
                                            stringentity.IsNotValid = false;
                                            stringentity.IsInitialized = true;
                                        }
                                        else
                                        {
                                            if (folder != null && folder.IsNotInUse)
                                                ResetNoInUse<CRStringFolder>(folder);
                                            stringentity.IsNotInUse = false;
                                        }

                                        Entity newEntity = new Entity()
                                        {
                                            Appname = appName,
                                            Name = name,
                                            RelativePathNoProject = relativePath,
                                            RelativePath = string.Format("{0} ({1})", relativePath, entry.AppName),
                                            ContainerDoc = entry.ContainerDoc,
                                            Crtype = entry.CReferenceType,
                                            Description = entry.Description,
                                            Settings = entry.Settings,
                                            EType = entry.IconType
                                        };

                                        AddNewCREntity(newEntity, stringentity);
                                    }
                                }
                            }
                        }
                        rawdata.RemoveAll((data) => data.RelativePath == comparePath &&
                                            data.CReferenceType == compareCReferenceType &&
                                            data.AppName == compareAppName &&
                                            data.EndpointUrl == compareEndpointUrl);


                    }
                }
            }
        }

        private bool IsServerTag(string referencedNodeId)
        {
            if (referencedNodeId == null)
                return false;
            return referencedNodeId.StartsWith(UFUAServerInfo.Guids.SystemTagsGuid.ToString()) ||
                referencedNodeId.StartsWith(UFUAServerInfo.Guids.RootTagsGuid.ToString()) ||
                referencedNodeId.StartsWith(UFUAServerInfo.Guids.RootDriversGuid.ToString()) ||
                referencedNodeId.StartsWith(UFUAServerInfo.Guids.RootAlarmsGuid.ToString()) || 
                referencedNodeId.StartsWith(UFUAServerInfo.Guids.RootDiagnosticGuid.ToString());
        }

        private void ResetNoInUse<T>(T folder)
        {
            Type type = typeof(T);
            if(type == typeof(CRTagFolder))
            {
                (folder as CRTagFolder).IsNotInUse = false;
                if ((folder as CRTagFolder).UFUAFolderAss != null)
                    ResetNoInUse((folder as CRTagFolder).UFUAFolderAss);
            }
            else if (type == typeof(CRStringFolder))
            {
                (folder as CRStringFolder).IsNotInUse = false;
                if ((folder as CRStringFolder).UFUAFolderAss != null)
                    ResetNoInUse((folder as CRStringFolder).UFUAFolderAss);
            }
        }

        internal class CREventArgs
        {
            public string Message { get; set; }
            public string TypeScheme { get; set; }
            public int Counter { get; set; }
            public int MaxValue { get; set; }
        }
        internal event EventHandler<CREventArgs> CRDocError;
        protected void OnError(string message, string typescheme)
        {
            CRDocError?.Invoke(this, new CREventArgs(){Message = message, TypeScheme = typescheme });
        }
        internal bool AddNewCREntity(Entity newEntity, CRTag entity)
        {
            try
            {
                
                if (entity != null)
                {
                    var setting = new CREntity()
                    {
                        CrossReferenceTag = entity
                    };
                    entity.Entities.Add(setting);
                    setting.Name = newEntity.Description;
                    setting.DynamicSettings = newEntity.Settings;
                    setting.Container = newEntity.ContainerDoc;
                    setting.TypeIcon = newEntity.EType;
                    setting.EndpointUrl = newEntity.EndpointUrl;
                    setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                    crEntities.Add(setting);
                    return true;
                }
                else 
                    return false;
            }
            catch (Exception ex)
            {
                OnError(ex.Message, newEntity.EType);
                return false;
            }
        }
        internal bool AddNewCREntity(Entity newEntity, CRString entity)
        {
            //var d = GetDataLayer();
            {
                //using (var uow = new UnitOfWork(dlCompiler))
                {
                    try
                    {
                        if (entity != null)
                        {
                            var setting = new CREntity()
                            {
                                CrossReferenceString = entity
                            };
                            entity.Entities.Add(setting);
                            setting.Name = newEntity.Description;
                            setting.DynamicSettings = newEntity.Settings;
                            setting.Container = newEntity.ContainerDoc;
                            setting.TypeIcon = newEntity.EType;

                            return true;// OnEntityInserted(newEntity.EType);
                        }
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message, newEntity.EType);
                        return false;
                    }
                }
            }
        }
        internal bool AddNewCREntity(Entity newEntity, CRScreen entity)
        {
            //var d = GetDataLayer();
            {
                //using (var uow = new UnitOfWork(dlCompiler))
                {
                    try
                    {
                        if (entity != null)
                        {
                            var setting = new CREntity()
                            {
                                CrossReferenceScreen = entity
                            };
                            entity.Entities.Add(setting);
                            setting.Name = newEntity.Description;
                            setting.DynamicSettings = newEntity.Settings;
                            setting.Container = newEntity.ContainerDoc;
                            setting.TypeIcon = newEntity.EType;

                            return true;// OnEntityInserted(newEntity.EType);
                        }
                        else 
                            return false;
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message, newEntity.EType);
                        return false;
                    }
                }
            }
        }
        internal bool AddNewCREntity(Entity newEntity, CRConnection entity)
        {
            //var d = GetDataLayer();
            {
                //using (var uow = new UnitOfWork(dlCompiler))
                {
                    try
                    {
                        if (entity != null)
                        {
                            var setting = new CREntity()
                            {
                                CrossReferenceConnection = entity
                            };
                            entity.Entities.Add(setting);
                            setting.Name = newEntity.Description;
                            setting.DynamicSettings = newEntity.Settings;
                            setting.Container = newEntity.ContainerDoc;
                            setting.TypeIcon = newEntity.EType;

                            return true;// OnEntityInserted(newEntity.EType);
                        }
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        OnError(ex.Message, newEntity.EType);
                        return false;
                    }
                }
            }
        }
        string defAppName;
        internal void InitDataSyncReferences()
        {
            defAppName = string.Empty;
            if (EditorManagerComponent.UFUAEditorManager != null)
                defAppName = EditorManagerComponent.UFUAEditorManager.GetDefApplicationName(parent);

            dataSincInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            foreach(var d in dataSincInterfaces)
            {
                if(!mapDataSinkInterfaces.ContainsKey(d))
                    mapDataSinkInterfaces.Add(d,OPCUAEntityReference.GetDataSinkInterface(d));
                if (!mapDataSinkTagList.ContainsKey(d))
                    mapDataSinkTagList.Add(d, OPCUAEntityReference.GetDataSinkInterface(d).GetVariables());
            }
        }

        internal void UpdateIsOtherEntitiesReferencedBy(CRTag item, List<CREntity> entities)
        {
            if (item.IsNotInUse && item.Entities[0].ReferencedNodeID != null)
            {
                var serverTypeIcon = DocManagerType.UFUAServer.ToString();
                item.IsOtherEntitiesReferencedBy = false;
                if (item.Entities[0].ReferencedNodeID == null)
                    return;

                if(item.HasPrototypeModel && !item.Entities[0].ReferencedNodeID.Contains('?'))
                    item.IsOtherEntitiesReferencedBy = (from e in entities
                                                    where e != item.Entities[0] &&
                                                    e.ReferencedNodeID != null &&
                                                    e.ReferencedNodeID.Contains(item.Entities[0].ReferencedNodeID)
                                                    select e).FirstOrDefault() != null;
                else
                    item.IsOtherEntitiesReferencedBy = (from e in entities
                                                        where e != item.Entities[0] &&
                                                        e.ReferencedNodeID == item.Entities[0].ReferencedNodeID &&
                                                        e.TypeIcon != serverTypeIcon
                                                        select e).FirstOrDefault() != null;
            }
        }

        internal void UpdateItemExistence(CRTag item)
        {
            List<string> dynamicList = null;
            DataSinkInterface _datasinc = null;
            if (dataSincInterfaces.Contains(item.AppName))
            {
                if (!mapDataSinkTagList.ContainsKey(item.AppName))
                    mapDataSinkTagList[item.AppName] = new List<string>();
                dynamicList = mapDataSinkTagList[item.AppName];
                _datasinc = mapDataSinkInterfaces[item.AppName];
            }
            else
                dynamicList = tagList;

            string _readablepath = item.ReadablePathNoProject;
            if (_datasinc != null)
            {
                if (dynamicList.Contains(_readablepath))
                {
                    item.IsNotValid = true;
                    return;
                }
                else
                {
                    item.IsNotValid = _datasinc.CheckVariable(_readablepath.Replace('\\', '&'));
                    if (item.IsNotValid)
                        dynamicList.Add(_readablepath);
                    return;
                }
            }
            else
            {
                string tagFound = null;
                bool invalid = EditorManagerComponent.UFUAEditorManager.CheckVariable(Parent, _readablepath, item.EndpointUrl, prototypelist, out tagFound, clearCache: clearCacheTags);
                clearCacheTags = false;
                if (invalid)
                {
                    dynamicList.Add(_readablepath);
                    item.IsNotValid = true;
                    return;
                }
                else
                {
                    item.IsNotValid = false; 
                    return;
                }                
            }
        }

        Dictionary<CrossReferenceType, int> isNotValidCountMap;
        Dictionary<CrossReferenceType, int> IsNotValidCountMap
        {
            get
            {
                if (isNotValidCountMap == null)
                {
                    isNotValidCountMap = new Dictionary<CrossReferenceType, int>();
                    Enum.GetValues(typeof(CrossReferenceType)).Cast<CrossReferenceType>().ToList().ForEach(d =>
                    isNotValidCountMap.Add(d, 0));
                }
                return isNotValidCountMap;
            }
            set
            {
                if (isNotUsedCountMap == value)
                    return;

                isNotUsedCountMap = value;
            }
        }
        Dictionary<CrossReferenceType, int> isNotUsedCountMap;
        Dictionary<CrossReferenceType, int> IsNotUsedCountMap
        {
            get
            {
                if (isNotUsedCountMap == null)
                {
                    isNotUsedCountMap = new Dictionary<CrossReferenceType, int>();
                    Enum.GetValues(typeof(CrossReferenceType)).Cast<CrossReferenceType>().ToList().ForEach(d =>
                    isNotUsedCountMap.Add(d, 0));
                }
                return isNotUsedCountMap;
            }
            set
            {
                if (isNotUsedCountMap == value)
                    return;

                isNotUsedCountMap = value;
            }
        }

        internal void UpdateSummary(bool bReset = false)
        {
            IsNotValidCountMap.Keys.ToList().ForEach(key =>
            {
                if (bReset)
                    IsNotValidCountMap[key] = 0;
                var item = (from c in new XPQuery<NotValidItem>(uow)/*.AsParallel()*/ where c.CRType == key select c).FirstOrDefault();
                if (item == null)
                {
                    item = new NotValidItem(uow) { CRType = key };
                }
                item.CRValue = IsNotValidCountMap[key];
            });
            IsNotUsedCountMap.Keys.ToList().ForEach(key =>
            {
                if (bReset)
                    IsNotUsedCountMap[key] = 0;
                var item = (from c in new XPQuery<NotUsedItem>(uow)/*.AsParallel()*/ where c.CRType == key select c).FirstOrDefault();
                if (item == null)
                {
                    item = new NotUsedItem(uow) { CRType = key };
                }
                item.CRValue = IsNotUsedCountMap[key];
            });
        }

        public void InitDocSummary()
        {
            var notValidItems = (from c in new XPQuery<NotValidItem>(uow)/*.AsParallel()*/ select c).ToList();
            var notUsedItems = (from c in new XPQuery<NotUsedItem>(uow)/*.AsParallel()*/ select c).ToList();
            notValidItems?.ToList<NotValidItem>().ForEach(item =>
            IsNotValidCountMap[item.CRType] = item.CRValue);
            notUsedItems?.ToList<NotUsedItem>().ForEach(item =>
            IsNotUsedCountMap[item.CRType] = item.CRValue);
        }

        internal void UpdateItemExistence(CRString item)
        {
            if (stringList == null)
            {
                if (stringeditorManager == null)
                {
                    stringeditorManager = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                }
                if (stringeditorManager != null)
                {
                    var list = stringeditorManager.GetListStringIDs(parent, true);
                    stringList = list != null ? list.ToList() : new List<string>();
                }
            }

            item.IsNotValid = stringList == null || !stringList.Contains(item.ReadablePath);
        }

        internal void UpdateItemExistence(CRScreen item)
        {
            bool isValid = false;
            if (iUFProjectManager == null)
                iUFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (iUFProjectManager != null)
            {
                IDocumentManager manager = null;
                try
                {
                    manager = CrossReferenceEditorManagerComponent.UriRisolver.ResolveUri(new Uri(item.ReadablePath, UriKind.RelativeOrAbsolute)) as IDocumentManager;
                }
                catch 
                { }
                if (manager != null)
                {
                    lock (lockObject)
                    {
                        if (resourceList == null)
                            resourceList = new Dictionary<string, List<string>>();
                        if (!resourceList.ContainsKey(manager.TypeLabel))
                        {
                            resourceList.Add(manager.TypeLabel, new List<string>());
                            resourceList[manager.TypeLabel] = iUFProjectManager.GetResourceList(parent, manager.TypeScheme).ToList();
                        }
                    }

                    isValid = resourceList[manager.TypeLabel].Contains(String.Format("{0}/{1}", manager.TypeLabel, item.ReadablePath));
                }
            }

            item.IsNotValid = !isValid;
        }
        internal void UpdateItemExistence(UFCrossReferenceConnection item)
        {
            item.IsNotValid = true;
        }
        internal void ClearCRMaps()
        {
            tagList.Clear();
            mapDataSinkTagList.Clear();
            resourceList = null;
            stringList = null;
            prototypelist = null;
            crEntities.Clear();
            tagRefMap.Clear();
            screenRefMap.Clear();
            connectionRefMap.Clear();
            stringRefMap.Clear();
            tagFolderRefMap.Clear();
            screenFolderRefMap.Clear();
            connectionFolderRefMap.Clear();
            //stringFolderRefMap.Clear();
        }
        internal void CopyFromCompiler()
        {
#if DEBUG
            using (var reswatch = new StopWatcher($"CR CopyFromCompiler took : " + "{0}"))
#endif
            {
                Dictionary<CRTagFolder, UFCrossReferenceTagFolder> tagFolderMap = new Dictionary<CRTagFolder, UFCrossReferenceTagFolder>();
                Dictionary<CRConnectionFolder, UFCrossReferenceConnectionFolder> connectionFolderMap = new Dictionary<CRConnectionFolder, UFCrossReferenceConnectionFolder>();
                Dictionary<CRScreenFolder, UFCrossReferenceScreenFolder> screenFolderMap = new Dictionary<CRScreenFolder, UFCrossReferenceScreenFolder>();
                Dictionary<CRStringFolder, UFCrossReferenceStringFolder> stringFolderMap = new Dictionary<CRStringFolder, UFCrossReferenceStringFolder>();
                Dictionary<CRTag, UFCrossReferenceTag> tagMap = new Dictionary<CRTag, UFCrossReferenceTag>();
                Dictionary<CRConnection, UFCrossReferenceConnection> connectionMap = new Dictionary<CRConnection, UFCrossReferenceConnection>();
                Dictionary<CRScreen, UFCrossReferenceScreen> screenMap = new Dictionary<CRScreen, UFCrossReferenceScreen>();
                Dictionary<CRString, UFCrossReferenceString> stringMap = new Dictionary<CRString, UFCrossReferenceString>();
                List<UFCrossReferenceEntity> entities = new List<UFCrossReferenceEntity>();

                entities.Clear();

                tagFolderRefMap.Keys.ToList().ForEach(key =>
                {
                    if (tagFolderRefMap[key].HasPrototypeModel)
                    {
                        if(tagFolderRefMap[key].Tags.TrueForAll(tag => tag.IsOtherEntitiesReferencedBy))
                        {
                            var relName = tagFolderRefMap[key].GetRelativeName();
                            var tag = (from t in tagRefMap.Values where t.ReadablePathNoProject == relName select t).FirstOrDefault();
                            if(tag != null)
                            {
                                var entity = (from e in crEntities 
                                              where e.ReferencedNodeID != null && tag.Entities[0].ReferencedNodeID != null &&
                                              e.ReferencedNodeID.Contains(tag.Entities[0].ReferencedNodeID) && 
                                              e.CrossReferenceTag != null && 
                                              e.CrossReferenceTag.ReadablePathNoProject != relName &&
                                              !e.CrossReferenceTag.ReadablePathNoProject.Contains($"{relName}\\")
                                              select e).ToList();
                                if (entity.Count > 0)
                                    return;
                            }
                        }
                    }
                    
                    var folder = AddNewTagFolder(key, uow);
                    folder.TypeDefinition = tagFolderRefMap[key].TypeDefinition;
                    folder.IsNotInUse = tagFolderRefMap[key].IsNotInUse;
                    folder.IsNotValid = tagFolderRefMap[key].IsNotValid;
                    folder.HasPrototypeModel = tagFolderRefMap[key].HasPrototypeModel;
                    tagFolderMap.Add(tagFolderRefMap[key], folder);
                });
                screenFolderRefMap.Keys.ToList().ForEach(key =>
                {
                    var folder = AddNewScreenFolder(key, uow);
                    screenFolderMap.Add(screenFolderRefMap[key], folder);
                    folder.IsNotValid = screenFolderRefMap[key].IsNotValid;
                });
                //stringFolderRefMap.Keys.ToList().ForEach(key =>
                //{
                //    var folder = AddNewStringFolder(key, uow);
                //    stringFolderMap.Add(stringFolderRefMap[key], folder);
                //    folder.IsNotInUse = stringFolderRefMap[key].IsNotInUse;
                //    folder.IsNotValid = stringFolderRefMap[key].IsNotValid;
                //});
                connectionFolderRefMap.Keys.ToList().ForEach(key =>
                {
                    var folder = AddNewConnectionFolder(key, uow);
                    connectionFolderMap.Add(connectionFolderRefMap[key], folder);
                });
                tagRefMap.Values.ToList().ForEach(entity =>
                {
                    if (entity.HasPrototypeModel)
                    {
                        UFCrossReferenceTagFolder folder = entity.PrototypeFolder != null && tagFolderMap.ContainsKey(entity.PrototypeFolder) ? tagFolderMap[entity.PrototypeFolder] : null;
                        if (folder != null)
                            entity.Entities.ForEach(newEntity =>
                            {
                                var setting = new UFCrossReferenceEntity(uow)
                                {
                                    CrossReferenceTagFolder = folder
                                };

                                setting.Name = newEntity.Name;
                                setting.DynamicSettings = newEntity.DynamicSettings;
                                setting.Container = newEntity.Container;
                                setting.TypeIcon = newEntity.TypeIcon;
                                setting.EndpointUrl = newEntity.EndpointUrl;
                                setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                                entities.Add(setting);
                            });
                    }
                    else if (!tagMap.ContainsKey(entity) && !entity.IsOtherEntitiesReferencedBy)
                    {
                        UFCrossReferenceTagFolder folder = entity.UFUAFolder != null && tagFolderMap.ContainsKey(entity.UFUAFolder) ? tagFolderMap[entity.UFUAFolder] : null;
                        var item = new UFCrossReferenceTag(uow)
                        {
                            Name = entity.Name,
                            ReadablePathNoProject = entity.ReadablePathNoProject,
                            EndpointUrl = entity.EndpointUrl,
                            ReadablePath = entity.ReadablePath,
                            TypeIcon = entity.TypeIcon,
                            AppName = entity.AppName,
                            UFUAFolder = folder,
                            TypeDefinition = entity.TypeDefinition,
                            HasPrototypeModel = entity.HasPrototypeModel,
                            IsNotInUse = entity.IsNotInUse,
                            IsNotValid = entity.IsNotValid
                        };
                        tagMap[entity] = item;
                        entity.Entities.ForEach(newEntity =>
                        {
                            var setting = new UFCrossReferenceEntity(uow)
                            {
                                CrossReferenceTag = item
                            };

                            setting.Name = newEntity.Name;
                            setting.DynamicSettings = newEntity.DynamicSettings;
                            setting.Container = newEntity.Container;
                            setting.TypeIcon = newEntity.TypeIcon;
                            setting.EndpointUrl = newEntity.EndpointUrl;
                            setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                        });
                    }
                });
                screenRefMap.Values.ToList().ForEach(entity =>
                {
                    UFCrossReferenceScreenFolder folder = entity.UFUAFolder != null && screenFolderMap.ContainsKey(entity.UFUAFolder) ? screenFolderMap[entity.UFUAFolder] : null;
                    if (!screenMap.ContainsKey(entity))
                    {
                        var item = new UFCrossReferenceScreen(uow)
                        {
                            Name = entity.Name,
                            ReadablePath = entity.ReadablePath,
                            TypeIcon = entity.TypeIcon,
                            ReadablePathProject = entity.ReadablePathProject,
                            UFUAFolder = folder,
                            IsNotValid = entity.IsNotValid
                        };
                        screenMap[entity] = item;
                        entity.Entities.ForEach(newEntity =>
                        {
                            var setting = new UFCrossReferenceEntity(uow)
                            {
                                CrossReferenceScreen = item
                            };

                            setting.Name = newEntity.Name;
                            setting.DynamicSettings = newEntity.DynamicSettings;
                            setting.Container = newEntity.Container;
                            setting.TypeIcon = newEntity.TypeIcon;
                            setting.EndpointUrl = newEntity.EndpointUrl;
                            setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                        });
                    }
                });
                connectionRefMap.Values.ToList().ForEach(entity =>
                {
                    UFCrossReferenceConnectionFolder folder = entity.UFUAFolder != null && connectionFolderMap.ContainsKey(entity.UFUAFolder) ? connectionFolderMap[entity.UFUAFolder] : null;
                    if (!connectionMap.ContainsKey(entity))
                    {
                        var item = new UFCrossReferenceConnection(uow)
                        {
                            Name = entity.Name,
                            ReadablePath = entity.ReadablePath,
                            TypeIcon = entity.TypeIcon,
                            ReadablePathProject = entity.ReadablePathProject,
                            UFUAFolder = folder
                        };
                        connectionMap[entity] = item;
                        entity.Entities.ForEach(newEntity =>
                        {
                            var setting = new UFCrossReferenceEntity(uow)
                            {
                                CrossReferenceConnection = item
                            };

                            setting.Name = newEntity.Name;
                            setting.DynamicSettings = newEntity.DynamicSettings;
                            setting.Container = newEntity.Container;
                            setting.TypeIcon = newEntity.TypeIcon;
                            setting.EndpointUrl = newEntity.EndpointUrl;
                            setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                        });
                    }
                });
                stringRefMap.Values.ToList().ForEach(entity =>
                {
                    UFCrossReferenceStringFolder folder = entity.UFUAFolder != null && stringFolderMap.ContainsKey(entity.UFUAFolder) ? stringFolderMap[entity.UFUAFolder] : null;
                    if (!stringMap.ContainsKey(entity))
                    {
                        var item = new UFCrossReferenceString(uow)
                        {
                            Name = entity.Name,
                            ReadablePath = entity.ReadablePath,
                            TypeIcon = entity.TypeIcon,
                            ReadablePathProject = entity.ReadablePathProject,
                            UFUAFolder = folder,
                            IsNotInUse = entity.IsNotInUse,
                            IsNotValid = entity.IsNotValid
                        };
                        stringMap[entity] = item;
                        entity.Entities.ForEach(newEntity =>
                        {
                            var setting = new UFCrossReferenceEntity(uow)
                            {
                                CrossReferenceString = item
                            };

                            setting.Name = newEntity.Name;
                            setting.DynamicSettings = newEntity.DynamicSettings;
                            setting.Container = newEntity.Container;
                            setting.TypeIcon = newEntity.TypeIcon;
                            setting.EndpointUrl = newEntity.EndpointUrl;
                            setting.ReferencedNodeID = newEntity.ReferencedNodeID;
                        });
                    }
                });

                entities.Clear();
                tagFolderMap.Clear();
                connectionFolderMap.Clear();
                screenFolderMap.Clear();
                stringFolderMap.Clear();
                tagMap.Clear();
                connectionMap.Clear();
                screenMap.Clear();
                stringMap.Clear();
            }
        }

        bool clearCacheTags = false;
        public void UpdateItemExistence(CancellationToken ct, bool hastTypedef)
        {
#if DEBUG
            using (var reswatch = new StopWatcher($"CR UpdateItemExistence took : " + "{0}"))
#endif
            {
                clearCacheTags = true;
                tagRefMap.Values.AsParallel().ForAll(tag =>//.ToList().ForEach(tag =>
                {
                    if (ct.IsCancellationRequested)
                        return;
                    UpdateIsOtherEntitiesReferencedBy(tag, crEntities);
                    if (!tag.IsNotInUse && tag.IsNotValid && !tag.IsInitialized)
                    {
                        if(!hastTypedef)
                        {
                            UpdateItemExistence(tag);
                            tag.IsInitialized = true;
                        }
                        else if (tag.ReadablePathNoProject.EndsWith(NamespaceTableConverter.WholeFolderWildChar))
                        {
                            var key = tag.ReadablePathNoProject.Substring(0, tag.ReadablePathNoProject.LastIndexOf(NamespaceTableConverter.WholeFolderWildChar));
                            tag.IsNotValid = (from e in crEntities where e.EntityReadablePath.StartsWith(key) && e.TypeIcon == DocManagerType.UFUAServer.ToString() select e).FirstOrDefault() == null;
                        }
                    }
                });
                IsNotValidCountMap[CrossReferenceType.Tags] = (from t in tagRefMap.Values where t.IsNotValid select t).Count();
                IsNotUsedCountMap[CrossReferenceType.Tags] = (from t in tagRefMap.Values where t.IsNotInUse && !t.IsOtherEntitiesReferencedBy select t).Count();
                tagFolderRefMap.Values.AsParallel().ForAll(folder =>//.ToList().ForEach(folder =>
                {
                    if (ct.IsCancellationRequested)
                        return;
                    UpdateItemExistence(folder);
                });
                screenRefMap.Values.AsParallel().ForAll(tag =>//.ToList().ForEach(tag =>
                {
                    if (ct.IsCancellationRequested)
                        return;
                    UpdateItemExistence(tag);
                    tag.IsInitialized = true;
                });
                IsNotValidCountMap[CrossReferenceType.Resources] = (from t in screenRefMap.Values where t.IsNotValid select t).Count();
                screenFolderRefMap.Values.AsParallel().ForAll(folder =>//.ToList().ForEach(folder =>
                {
                    if (ct.IsCancellationRequested)
                        return;
                    UpdateItemExistence(folder);
                });
                stringRefMap.Values.AsParallel().ForAll(tag =>//.ToList().ForEach(tag =>
                {
                    if (ct.IsCancellationRequested)
                        return;
                    if (!tag.IsNotInUse && tag.IsNotValid && !tag.IsInitialized)
                        UpdateItemExistence(tag);
                    tag.IsInitialized = true;
                });
                IsNotValidCountMap[CrossReferenceType.Strings] = (from t in stringRefMap.Values where t.IsNotValid select t).Count();
                IsNotUsedCountMap[CrossReferenceType.Strings] = (from t in stringRefMap.Values where t.IsNotInUse select t).Count();
                //stringFolderRefMap.Values.AsParallel().ForAll(folder =>//.ToList().ForEach(folder =>
                //{
                //    if (ct.IsCancellationRequested)
                //        return;
                //    UpdateItemExistence(folder);
                //});
            }
        }

        public List<string> GetTranslatable()
        {
            return (from tag in new XPQuery<UFCrossReferenceString>(uow, true)/*.AsParallel()*/
                    where !string.IsNullOrEmpty(tag.Name) && tag.IsNotValid
                    select tag.Name).ToList();
        }

        public void UpdateTranslatable()
        {
            (from tag in new XPQuery<UFCrossReferenceString>(uow, true)/*.AsParallel()*/
             where !string.IsNullOrEmpty(tag.Name) && tag.IsNotValid
             select tag).ToList().ForEach(tag => tag.IsNotValid = false);
        }

        public int GetInvalidReferences(CrossReferenceType filterType)
        {
            try
            {
                return IsNotValidCountMap[filterType];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int GetNotUsedReferences(CrossReferenceType filterType)
        {
            try
            {
                return IsNotUsedCountMap[filterType];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private bool UpdateItemExistence(CRStringFolder folder)
        {
            bool isNotValid = false;

            if (folder.Screens.Count > 0)
            {
                var tagNotValid = (from tag in folder.Screens where tag.IsNotValid select tag).FirstOrDefault();
                isNotValid = tagNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }
            if (folder.UFUAFolders.Count > 0)
            {
                var folderNotValid = (from f in folder.UFUAFolders where UpdateItemExistence(f) select f).FirstOrDefault();
                isNotValid = folderNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }

            folder.IsNotValid = false;
            return isNotValid;
        }

        private bool UpdateItemExistence(CRScreenFolder folder)
        {
            bool isNotValid = false;

            if (folder.Screens.Count > 0)
            {
                var tagNotValid = (from tag in folder.Screens where tag.IsNotValid select tag).FirstOrDefault();
                isNotValid = tagNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }
            if (folder.UFUAFolders.Count > 0)
            {
                var folderNotValid = (from f in folder.UFUAFolders where UpdateItemExistence(f) select f).FirstOrDefault();
                isNotValid = folderNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }

            folder.IsNotValid = false;
            return isNotValid;
        }

        private bool UpdateItemExistence(CRTagFolder folder)
        {
            bool isNotValid = false;

            if (folder.Tags.Count > 0)
            {
                var tagNotValid = (from tag in folder.Tags where tag.IsNotValid select tag).FirstOrDefault();
                isNotValid = tagNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }
            if (folder.UFUAFolders.Count > 0)
            {
                var folderNotValid = (from f in folder.UFUAFolders where UpdateItemExistence(f) select f).FirstOrDefault();
                isNotValid = folderNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }

            folder.IsNotValid = false;
            return isNotValid;
        }

        private bool UpdateItemExistence(UFCrossReferenceTagFolder folder)
        {
            bool isNotValid = false;

            if (folder.Tags.Count > 0)
            {
                var tagNotValid = (from tag in folder.Tags where tag.IsNotValid select tag).FirstOrDefault();
                isNotValid = tagNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }
            if (folder.UFUAFolders.Count > 0)
            {
                var folderNotValid = (from f in folder.UFUAFolders where UpdateItemExistence(f) select f).FirstOrDefault();
                isNotValid = folderNotValid != null;
                if (isNotValid)
                {
                    folder.IsNotValid = true;
                    return true;
                }
            }

            folder.IsNotValid = false;
            return isNotValid;
        }

        private bool IsInUse(UFCrossReferenceTagFolder folder)
        {
            bool isReferencedBy = false;

            if (folder.Tags.Count > 0)
            {
                var tagInUse = (from tag in folder.Tags where !tag.IsNotInUse select tag).FirstOrDefault();
                isReferencedBy = tagInUse != null;
                if (isReferencedBy)
                    return true;
            }
            if (folder.UFUAFolders.Count > 0)
            {
                var folderInUse = (from f in folder.UFUAFolders where IsInUse(f) select f).FirstOrDefault();
                isReferencedBy = folderInUse != null;
                if (isReferencedBy)
                    return true;
            }

            folder.IsNotInUse = true;
            return isReferencedBy;
        }

        internal void ClearList()
        {
#if DEBUG
            using (var reswatch = new StopWatcher($"CR ClearList took : " + "{0}"))
#endif
            {
                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (dl != null)
                        dl.Dispose();
                    if (uow != null)
                    {
                        uow.Disconnect();
                        uow.Dispose();
                    }

                    InMemory = null;
                    dl = GetDataLayer(ref InMemory, false);
                    uow = new UnitOfWork(dl);
                }
                else
                {
                    (from tag in new XPQuery<UFCrossReferenceTagFolder>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceScreenFolder>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceConnectionFolder>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceStringFolder>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());

                    (from tag in new XPQuery<UFCrossReferenceTag>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceScreen>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceConnection>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceString>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                    (from tag in new XPQuery<UFCrossReferenceEntity>(uow, true)
                     select tag).ToList().ForEach(x => x.Delete());
                }
            }
        }
        
        internal IOrderedEnumerable<UFCrossReferenceEntity> GetFlatList()
        {
            return (from tag in new XPQuery<UFCrossReferenceEntity>(uow, true)//.AsParallel()
                    where tag.CrossReferenceTag != null
                    select tag).ToList().OrderBy(o => o.TagName);
        }
        
        internal IOrderedEnumerable<UFCrossReferenceEntity> GetStringFlatList()
        {
            return (from tag in new XPQuery<UFCrossReferenceEntity>(uow, true)//.AsParallel()
                    where tag.CrossReferenceString != null
                    select tag).ToList().OrderBy(o => o.TagName);
        }
        internal IOrderedEnumerable<UFCrossReferenceEntity> GetScreenFlatList()
        {
            return (from tag in new XPQuery<UFCrossReferenceEntity>(uow, true)//.AsParallel()
                    where tag.CrossReferenceScreen != null
                    select tag).ToList().OrderBy(o => o.TagName);
        }
        internal IOrderedEnumerable<UFCrossReferenceEntity> GetConnectionFlatList()
        {
            return (from tag in new XPQuery<UFCrossReferenceEntity>(uow, true)//.AsParallel()
                    where tag.CrossReferenceConnection != null
                    select tag).ToList().OrderBy(o => o.TagName);
        }
        
        internal IList<XPObject> GetTagCollection()
        {
            List<XPObject> list =  (from tag in new XPQuery<UFCrossReferenceTag>(uow, true)//.AsParallel()
                    orderby tag.Name ascending
                    select (XPObject)tag).ToList();
            list.AddRange((from tag in new XPQuery<UFCrossReferenceTagFolder>(uow, true)//.AsParallel()
                    where tag.HasPrototypeModel
                    orderby tag.Name ascending
                    select tag).ToList());
            return list;
        }

        internal IList<UFCrossReferenceTagFolder> GetTagFolderCollection(UFCrossReferenceTagFolder root = null)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFCrossReferenceTagFolder>(uow, true)//.AsParallel()
                        where folder.UFUAFolderAss == null
                        orderby folder.Name ascending
                        select folder).ToList();
            }

            return (from folder in new XPQuery<UFCrossReferenceTagFolder>(uow, true)//.AsParallel()
                    where folder.UFUAFolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }

        internal IList<UFCrossReferenceScreenFolder> GetScreenFolderCollection(UFCrossReferenceScreenFolder root = null)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFCrossReferenceScreenFolder>(uow, true)//.AsParallel()
                        where folder.UFUAFolderAss == null
                        orderby folder.Name ascending
                        select folder).ToList();
            }

            return (from folder in new XPQuery<UFCrossReferenceScreenFolder>(uow, true)//.AsParallel()
                    where folder.UFUAFolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }

        internal IList<UFCrossReferenceStringFolder> GetStringFolderCollection(UFCrossReferenceStringFolder root = null)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFCrossReferenceStringFolder>(uow, true)//.AsParallel()
                        where folder.UFUAFolderAss == null
                        orderby folder.Name ascending
                        select folder).ToList();
            }

            return (from folder in new XPQuery<UFCrossReferenceStringFolder>(uow, true)//.AsParallel()
                    where folder.UFUAFolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }


        internal IList<UFCrossReferenceConnectionFolder> GetConnectionFolderCollection(UFCrossReferenceConnectionFolder root = null)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFCrossReferenceConnectionFolder>(uow, true)//.AsParallel()
                        where folder.UFUAFolderAss == null
                        orderby folder.Name ascending
                        select folder).ToList();
            }

            return (from folder in new XPQuery<UFCrossReferenceConnectionFolder>(uow, true)//.AsParallel()
                    where folder.UFUAFolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }

        internal IList<UFCrossReferenceStringFolder> GetConnectionFolderCollection(UFCrossReferenceStringFolder root = null)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<UFCrossReferenceStringFolder>(uow, true)//.AsParallel()
                        where folder.UFUAFolderAss == null
                        orderby folder.Name ascending
                        select folder).ToList();
            }

            return (from folder in new XPQuery<UFCrossReferenceStringFolder>(uow, true)//.AsParallel()
                    where folder.UFUAFolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }

        internal IList<UFCrossReferenceTag> GetCRTags(UFCrossReferenceTagFolder root = null)
        {
            if (root == null)
            {
                return (from p in new XPQuery<UFCrossReferenceTag>(uow, true)//.AsParallel()
                        where p.UFUAFolder == null
                        orderby p.Name
                        select p).ToList();
            }

            return (from p in new XPQuery<UFCrossReferenceTag>(uow, true)//.AsParallel()
                    where p.UFUAFolder == root
                    orderby p.Name
                    select p).ToList();
        }
        internal IList<UFCrossReferenceString> GetStringCRTags(UFCrossReferenceStringFolder root = null)
        {
            if (root == null)
            {
                return (from p in new XPQuery<UFCrossReferenceString>(uow, true)//.AsParallel()
                        where p.UFUAFolder == null
                        orderby p.Name
                        select p).ToList();
            }

            return (from p in new XPQuery<UFCrossReferenceString>(uow, true)//.AsParallel()
                    where p.UFUAFolder == root
                    orderby p.Name
                    select p).ToList();
        }
        internal IList<UFCrossReferenceScreen> GetScreenCRTags(UFCrossReferenceScreenFolder root = null)
        {
            if (root == null)
            {
                return (from p in new XPQuery<UFCrossReferenceScreen>(uow, true)//.AsParallel()
                        where p.UFUAFolder == null
                        orderby p.Name
                        select p).ToList();
            }

            return (from p in new XPQuery<UFCrossReferenceScreen>(uow, true)//.AsParallel()
                    where p.UFUAFolder == root
                    orderby p.Name
                    select p).ToList();
        }
        internal IList<UFCrossReferenceConnection> GetConnectionCRTags(UFCrossReferenceConnectionFolder root = null)
        {
            if (root == null)
            {
                return (from p in new XPQuery<UFCrossReferenceConnection>(uow, true)//.AsParallel()
                        where p.UFUAFolder == null
                        orderby p.Name
                        select p).ToList();
            }

            return (from p in new XPQuery<UFCrossReferenceConnection>(uow, true)//.AsParallel()
                    where p.UFUAFolder == root
                    orderby p.Name
                    select p).ToList();
        }
        internal void CopyListFoldersToClipbaord(List<UFCrossReferenceTag> list)
        {
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
            });
            uowClipboard.CommitChanges();
        }
        internal void CopyListFoldersToClipbaord(List<UFCrossReferenceScreen> list)
        {
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
            });
            uowClipboard.CommitChanges();
        }
        internal void CopyListFoldersToClipbaord(List<UFCrossReferenceConnection> list)
        {
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListTagsToClipbaord(List<UFCrossReferenceEntity> list)
        {
            list.ForEach(tag =>
            {
                CloneToClipboard(tag, false);
            });
            uowClipboard.CommitChanges();
        }
        #endregion

        #region Properties

        IPropertyControl propertyControl;
        public IPropertyControl PropertyControl
        {
            get
            {
                if (propertyControl == null)
                    propertyControl = GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyControl;
            }
        }

        [Browsable(false)]
        bool _NeedsSave;
        public bool NeedsSave
        {
            get
            {
                return _NeedsSave;
            }
            set
            {
                if (_NeedsSave == value)
                    return;
                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

        CrossReferenceEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public CrossReferenceEditorManagerComponent EditorManagerComponent
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

        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
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

            // return new Uri(Path.GetDirectoryName(FullPath) + "\\").MakeRelativeUri(absolute);
            return null;
        }

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

        [Browsable(false)]
        public String rootBase
        {
            get
            {
                if (Parent != null)
                    return Parent.rootBase;
                return Path.GetDirectoryName(fileBase);
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
                return Path.GetFileNameWithoutExtension(fileBase);
            }
        }

        [Browsable(false)]
        public String FilePath
        {
            get
            {
                return fileBase;
            }
        }

        [Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return true;
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

        #region ICloneable Members

        CREditorDocument(CREditorDocument template)
        {
            if (template == null)
                return;
        }

        public object Clone()
        {
            return new CREditorDocument(this);
        }

        #endregion

        #region IDisposable Members

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            OnDisposing(this);

            base.OnDispose();

            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }

            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
            if (dlClipboard != null)
            {
                dlClipboard.Dispose();
                dlClipboard = null;
            }
        }
        #endregion
    }
}
