using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
using RecipeServiceCMS;
using WPFUtilities.Extensions;
#endif
using MSSchedulerSettings.ComponentService;
using System.Text.RegularExpressions;
using Utilities;
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.ServiceModel;
using System.Xml;
using OPCUAViewModel;
using Opc.Ua;
using DevExpress.Xpo.DB.Helpers;
using System.Text;
using UFRecipeSettings.UFRecipeModel;
using System.Threading.Tasks;
using UFUAEditor.ComponentService;
using log4net;

namespace UFRecipeSettings.Documents
{
    public class RecipeEditorDocument : ViewModelBase, ICloneable, IDocument
    {
#region Declarations

        UnitOfWork uow;
        InMemoryDataStore InMemory;
        IDataLayer dl;

#if !NET_STANDARD
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dlClipboard;
#endif
        String defaultApplicationName;
        String connectionString;
        String fileBase;

#if !NET_STANDARD
        readonly Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper> UndoRedoHelper = new Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper>();
        readonly Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer> mapdlUndoRedo = new Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapuowUndoRedo = new Dictionary<IDataLayer, UnitOfWork>();
#endif

#if !NET_STANDARD
        internal static readonly ILog logGeneral = LogManager.GetLogger(Utilities.Properties.Resources.RecipeService);
#else
        internal static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Utilities.Properties.Resources.RecipeService);
#endif

        #endregion

        #region Methods
        void CreateDataLayer()
        {
            dl = GetDataLayer();
            uow = new UnitOfWork(dl);
#if !NET_STANDARD
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");

                if (e.PropertyName == "ApplicationName")
                {
                    ConfigurationId = Guid.NewGuid();
                }
            };

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);
#endif
        }

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        public UnitOfWork GetSession()
        {
            return uow;
        }

#if !NET_STANDARD
        public void CreateUndoRedoHelper(UserControl owner)
        {
            CreateUndoRedoHelper(owner, Properties.Settings.Default.MaxUndoRedoActions);
        }

        internal void CreateUndoRedoHelper(UserControl owner, short numactions)
        {
            if (UndoRedoHelper.ContainsKey(owner))
                return;

            var dlundoredo = XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionStringInMemory(true), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            var uowundoredo = new UnitOfWork(dlundoredo);
            var helper = numactions > 0 ? new XpoHelpers.UndoRedoIXPSimpleObjectHelper(uow, uowundoredo, numactions) : new XpoHelpers.UndoRedoIXPSimpleObjectHelper(uow, uowundoredo);
            UndoRedoHelper[owner] = helper;
            mapdlUndoRedo[helper] = dlundoredo;
            mapuowUndoRedo[dlundoredo] = uowundoredo;
        }
#endif

        void EnsureDefaultSettings(string title)
        {
            defaultApplicationName = String.Format("{0}_{1}", title, Properties.Settings.Default.AppNameSuffix);
            EnsureDefaultSettings();
        }

        void EnsureDefaultSettings()
        {
            var configuration = GetConfiguration();
            configuration.EnsureDefaultSettings(defaultApplicationName);
        }

#region WinClipboard
#if !NET_STANDARD
        public void CopyInMemoryDataToWinClipboard()
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
        public void CopyWinClipboardToInMemoryData(bool force = false)
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

        public void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();
        }
#endif
#endregion

#region Clipboard
#if !NET_STANDARD
        internal XPObject CloneToClipboard(XPObject obj, bool checkattributes = false)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uow, uowClipboard, checkattributes, true, true);
            return cloneHelper.Clone(obj, false);
        }

        internal XPObject CloneFromClipboard(XPObject obj, bool checkattributes = false)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowClipboard, uow, checkattributes);
            return cloneHelper.Clone(obj, false);
        }
#endif
#endregion

        public NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
        }
#if !NET_STANDARD
        public XPObject GetNestedObject(object obj)
        {
            if (obj == null || !(obj is XPObject) || (obj as XPObject).IsLoading || (obj as XPObject).IsDeleted)
                return null;

            UowContext = BeginNestedUnitOfWork();
            return UowContext.GetNestedObject(obj as XPObject);
        }

        public List<XPObject> GetNestedObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count == 0)
                return null;

            UowContext = BeginNestedUnitOfWork();

            var ret = new List<XPObject>(list.Count);
            list.ForEach(obj =>
            {
                ret.Add(UowContext.GetNestedObject(obj));
            });

            return ret;
        }

        List<XPObject> GetParentObjects(System.Collections.IList objects)
        {
            var list = (from c in objects.OfType<XPObject>() where !c.IsLoading && !c.IsDeleted select c).ToList();
            if (list.Count == 0 || UowContext == null)
                return null;

            var ret = new List<XPObject>(list.Count);
            list.ForEach(obj =>
            {
                if (XpoHelpers.XpoHelper.IsSessionObject(obj, UowContext))
                    ret.Add(UowContext.GetParentObject(obj));
            });

            return ret;
        }

        public static XPObject UpdateEdit(XPObject source, XPObject target, UnitOfWork uowc, UnitOfWork uowo)
        {
            var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowc, uowo);
            return cloneHelper.Clone(source, true, target);
        }
        public static XPObject CloneEdit(XPObject obj, UnitOfWork uowc, UnitOfWork uowo)
        {
            XpoHelpers.CloneIXPSimpleObjectHelper cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(uowc, uowo);
            return cloneHelper.Clone(obj, true);
        }
#endif
#region Undo/Redo
#if !NET_STANDARD
        static String UndoRedoNotInizialized = "The operation cannot be performed because the undo-redo manager has not been initialized for this control.";
        public void AddUndoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(list, action);
        }

        public void AddRedoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(list, action);
        }

        public void AddUndoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(obj, action);
        }

        public void AddRedoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(obj, action);
        }

        public bool UndoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanUndo();
        }

        public bool RedoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanRedo();
        }

        public void CleanUndoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeUndoActions(true);
        }

        public void CleanRedoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeRedoActions(true);
        }

        public List<XPObject> UndoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            if (UndoRedoHelper[owner].CanUndo())
                return UndoRedoHelper[owner].Undo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        public List<XPObject> RedoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            if (UndoRedoHelper[owner].CanRedo())
                return UndoRedoHelper[owner].Redo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        public void AddExistingObject(XPObject obj, object parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);
        }
#endif
#endregion

        static String GetConnectionString(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
            String connString = null;
#if !NET_STANDARD
            if (vfs != null && vfs is DataSourceFileSystemProvider)
            {
                connString = (vfs as DataSourceFileSystemProvider).ConnectionString;
            }
            else
#endif
            {
                var xmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                    Properties.Settings.Default.TypeLabel,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));
            }

            return connString;
        }

        static String GetBaseFilename(String path
#if !NET_STANDARD
            , FileSystemProviderBase vfs
#endif
            )
        {
#if !NET_STANDARD
            if (vfs != null)
                return null;
#endif

            return String.Format("{0}/{1}/{2}{3}", path,
                                Properties.Settings.Default.TypeLabel,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

        public static RecipeEditorDocument FromConnectionString(string conn, IDocument parent)
        {
            try
            {
                if (String.IsNullOrEmpty(conn))
                    return null;

                var doc = new RecipeEditorDocument(null)
                {
                    connectionString = conn
                };

                doc.CreateDataLayer();
                if (doc.IsEmpty)
                    return null;

                return doc;
            }
            catch
            {
                return null;
            }
        }

        public static RecipeEditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
#endif
                String connString = GetConnectionString(path
#if !NET_STANDARD
                    , vfs
#endif
                    );
                String xmlfile = GetBaseFilename(path
#if !NET_STANDARD
                    , vfs
#endif
                    );

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                var doc = new RecipeEditorDocument(null)
                {
                    connectionString = connString,
                    EditorManagerComponent = c as ISchedulerEditorManager,
                    fileBase = xmlfile,
                    Parent = parent
                };

                doc.CreateDataLayer();
                if (!bCreateNew && bCheckEmpty && doc.IsEmpty)
                {
                    doc.Dispose();
                    return null;
                }
                else if (
#if !NET_STANDARD
                    vfs == null && 
#endif
                    !doc.IsBelongFromParent(parent))
                {
                    doc.Dispose();
#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
#endif
                    logGeneral.Error(String.Format(Properties.Resources.ErrorValidatingDocument, path));
                    return null;
                }

                var title = Path.GetFileNameWithoutExtension(path);
#if !NET_STANDARD
                if (vfs != null)
#endif
                    title = XpoHelpers.XpoHelper.GetDataSourceTitle(connString, true);
                doc.EnsureDefaultSettings(title);

                return doc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#endif
                logGeneral.Error(String.Format(Properties.Resources.ErrorReadingDocument, path));
                return null;
            }
        }

#if !NET_STANDARD
        public bool CanClose()
        {
            if (ServerCMSHelperSync.IsServerStartedManually)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.StopServerStartedManually,
                        MSServerInfo.MSServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                    else if (res == CustomDialogResults.Yes)
                        ServerCMSHelperSync.StopServer();
                }
            }

            return true;
        }
#endif

        public MSModel.MSGeneralSettings GetConfiguration()
        {
            var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
            if (list.Count == 0)
                return new MSModel.MSGeneralSettings(uow);
            return list[0];
        }

#if !NET_STANDARD
        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return false;

            List<XPObject> parentObjects = null;
            if (ActiveView != null && EditorManagerComponent.Workspace != null &&
                EditorManagerComponent.Workspace.ActiveWindow == ActiveView)
            {
                EditorManagerComponent.Workspace.UpdateContextNow();
                var objects = EditorManagerComponent.Workspace.ContextObjects;
                if (EditorManagerComponent.Workspace.ContextObject != null)
                {
                    if (objects == null)
                        objects = new List<object>();
                    objects.Add(EditorManagerComponent.Workspace.ContextObject);
                }

                if (objects != null)
                    parentObjects = GetParentObjects(objects);
            }

            try
            {
                bool bSave = NeedsSave;
                if (discargechanges &&
                    uow.TryPurgeDeletedObjects() > 0)
                    bSave = true;

                if (!bForceSave && !bSave)
                    return false;

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

            if (parentObjects != null && parentObjects.Count > 0)
            {
                if (parentObjects.Count == 1)
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObject(parentObjects[0]);
                else
                    EditorManagerComponent.Workspace.ContextObject = GetNestedObjects(parentObjects);
            }

            return true;
        }
#endif

        IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(MSModel.MSScheduledAction).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                return XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            }
            else
            {
                lock (lockObject)
                {
                    if (InMemory == null)
                    {
                        InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        if (File.Exists(fileBase))
                        {
                            if (Protected || !Utilities.IO.FileSystem.IsXmlFile(fileBase))
                            {
                                var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(fileBase));
                                using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                                {
                                    var xmlreader = XmlReader.Create(reader);
                                    try
                                    {
                                        InMemory.ReadXml(xmlreader);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            else
                                try
                                {
                                    InMemory.ReadXml(fileBase);
                                }
                                catch (Exception ex)
                                {

                                }
                        }
                    }
                }

                return new DevExpress.Xpo.ThreadSafeDataLayer(dict, InMemory);
            }
        }

        public OPCUAEntityReference GetNodeIdOPCUAEntityReference(String tagName, String nodeID, bool member = false)
        {
            using (var d = GetDataLayer())
            {
                using (var uow = new UnitOfWork(d))
                {
                    String relativepath = null;
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);
                    String tag = null;

                    Opc.Ua.NodeId nodeid;

                    if (tagName.Contains('/'))
                    {
                        if (member)
                            nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", nodeID, tagName.Split('/').Last()), ns);
                        else
                            nodeid = new Opc.Ua.NodeId(new Guid(nodeID), ns);
                        StringBuilder rel = new StringBuilder();
                        foreach (String s in tagName.Split('/').ToList())
                        {
                            if (rel.Length == 0)
                                rel.AppendFormat("{0}", s);
                            else
                                rel.AppendFormat("/{1}:{0}", s, ns);
                        }
                        relativepath = String.Format("/{1}:{0}", rel.ToString(), ns);
                    }
                    else
                    {
                        tag = tagName;
                        nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", nodeID, tagName), ns);
                        relativepath = String.Format("/{1}:{0}", tagName, ns);
                    }


                    var applicationName = GetAplicationName();
                    OPCUAEntityReference entityreference = null;
                    entityreference = new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                                    relativepath, nodeid, string.Format("{0} ({1})", tag, GetConfiguration().ApplicationName), null, relativepath);
                    return entityreference;
                }
            }
        }

        public OPCUAEntityReference GetSchedulerEntityReference(String nodeId)
        {
            Guid guid;
            if (Guid.TryParse(nodeId, out guid))
            {
                var actionFound = FindActionByNodeId(guid);
                if (actionFound != null)
                {
                    Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
                    UInt16 ns = (UInt16)(n.Count + 2 - 1);

                    String relativepath = null;

                    var nodeid = new Opc.Ua.NodeId(guid, ns);
                    var actionName = actionFound.GetRelativeName();
                    var humanReadable = actionFound.GetRelativeName().Replace('/', '\\');

                    if (actionName.Contains('/'))
                    {
                        StringBuilder rel = new StringBuilder();
                        foreach (String s in actionName.Split('/').ToList())
                        {
                            rel.AppendFormat("/{1}:{0}", s, ns);
                        }
                        relativepath = String.Format("{2}:{1}{0}", rel.ToString(), MSServerInfo.MSServerInfo.GetTagsRootName(), ns);
                    }
                    else
                    {
                        relativepath = String.Format("{2}:{1}/{2}:{0}", actionName, MSServerInfo.MSServerInfo.GetTagsRootName(), ns);
                    }

                    var applicationName = GetAplicationName();
                    return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                        relativepath, nodeid, string.Format("{0} ({1})", humanReadable, applicationName), null, relativepath);
                }
            }

            return null;
        }

        public IList<String> GetFlatEventsList(bool runtime = false, bool asc = true)
        {
            if (runtime)
                return (asc ?
                        (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                         where tag.RuntimeSelectable == true
                         orderby tag.FullName ascending
                         select tag.FullName).ToList()
                        :
                        (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                         where tag.RuntimeSelectable == true
                         orderby tag.FullName descending
                         select tag.FullName).ToList()
                        );

            return (asc ?
                    (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                     orderby tag.FullName ascending
                     select tag.FullName).ToList()
                    :
                    (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                     orderby tag.FullName descending
                     select tag.FullName).ToList()
                    );
        }

#if !NET_STANDARD
        public void ServiceManager()
        {
            if (editorManagerComponent.UserEditor != null)
                editorManagerComponent.UserEditor.EnsureCredentialProvider(this);
            AddHttpAccessRules(true);

            string serverConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                serverConn = InMemoryDataStore.GetConnectionString(fileBase);
                if (editorManagerComponent.StringEditor != null)
                    stringConn = editorManagerComponent.StringEditor.GetConnectionStringFromFile(rootBase);
                if (editorManagerComponent.UserEditor != null)
                    userConn = editorManagerComponent.UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                serverConn = userConn = stringConn = ConnectionString;

            var docPath = GetSpecialFolder(SpecialFolders.Documents).GetPathString();
            docPath = docPath.Trim('\\', '/');

            var applicationName = GetAplicationName();
            //if (NeedToRunAsCFR21UserIndentity())
            //{
            //    var username = UFUAServerInfo.UFUAServerInfo.GetCFR21UserName();
            //    var domainName = UFUAServerInfo.UFUAServerInfo.GetCFR21DomainName();
            //    if (!String.IsNullOrEmpty(domainName))
            //        username = String.Format("{0}\\{1}", domainName, username);
            //    var password = "ie4HZN8u9uW4NJOdnRNFbMcs9wfWnAaGICcU3qs6fcW8IpbPVp273NEpPMaAlV8B"/* "{45F928C5_1EF2_48D1_9505_14ACf7AD6AF8}" */;
            //    UFUAServerCMS.UFUAServerCSMHelpers.OpenServiceManagerWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, docPath);
            //}
            //else
            MSServerCMSHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn, docPath);
        }
#endif

        public String GetAplicationName()
        {
            using (var uow = new UnitOfWork(dl))
            {
                var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                if (list.Count == 0)
                {
                    var conf = new MSModel.MSGeneralSettings(uow);
                    conf.EnsureDefaultSettings(defaultApplicationName);
                    return conf.ApplicationName;
                }
                return list[0].ApplicationName;
            }
        }

        internal string GetDefaultLocalEndpoint()
        {
            using (var uow = new UnitOfWork(dl))
            {
                var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                if (list.Count > 0)
                {
                    var endpoints = (from ba in list[0].BaseAddresses.AsParallel()
                                     where ba.Enabled == true
                                     select ba).ToList();

                    if (endpoints.Count > 0)
                    {
                        foreach (var transport in UFInterfaces.Constants.TransportOrder.transportOrderByRelevance)
                        {
                            var endpoint = (from ba in endpoints
                                            where ba.Transport == transport
                                            select ba).ToList();

                            if (endpoint.Count > 0)
                                return endpoint[0].Path;
                        }

                        if (endpoints.Count > 0)
                            return endpoints[0].Path;
                    }
                }

                var listadd = MSServerInfo.MSServerInfo.GetCurrentApplicationBaseAddresses();
                if (listadd != null && listadd.Count > 0)
                    return listadd[0];

                return string.Empty;
            }
        }

        public static OPCUAEntityReference GetGeneralOPCUAEntityReference(string applicationName, string endpointurl, string tagName, string nodeID, string human, bool member = false)
        {
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            String relativepath = null;
            Opc.Ua.NodeId nodeid;
            if (tagName.Contains('/'))
            {
                if (member)
                    nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", nodeID, tagName.Split('/').Last()), ns);
                else
                    nodeid = new Opc.Ua.NodeId(new Guid(nodeID), ns);
                StringBuilder rel = new StringBuilder();
                foreach (String s in tagName.Split('/').ToList())
                {
                    if (rel.Length == 0)
                        rel.AppendFormat("{0}", s);
                    else
                        rel.AppendFormat("/{1}:{0}", s, ns);
                }
                relativepath = String.Format("/{1}:{0}", rel.ToString(), ns);
            }
            else
            {
                nodeid = new Opc.Ua.NodeId(String.Format("{0}?{1}", nodeID, tagName), ns);
                relativepath = String.Format("/{1}:{0}", tagName, ns);
            }

            return new OPCUAEntityReference(null, applicationName, endpointurl,
                                            relativepath, nodeid,
                                            string.Format("{0} {1}", human, applicationName), null, relativepath);
        }

        public OPCUAEntityReference GetGeneralOPCUAEntityReference(string endpointurl, string tagName, string nodeID, string human, bool member = false)
        {
            var applicationName = GetAplicationName();
            return GetGeneralOPCUAEntityReference(applicationName, endpointurl, tagName, nodeID, human, member);
        }

        public OPCUAEntityReference GetServerOPCUAEntityReference()
        {
            var applicationName = GetAplicationName();
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            null, ObjectIds.Server, ObjectIds.Server.ToString(), null, null);
        }

        public MSModel.MSGeneralSettings GetGeneralSettings()
        {
            var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
            if (list.Count == 0)
                return new MSModel.MSGeneralSettings(uow);
            return list[0];
        }

        public String GetServiceName()
        {
            var serverName = MSServerInfo.MSServerInfo.GetServerName();
            if (uow == null)
                return serverName;

            return String.Format("{0} ({1})", serverName, GetGeneralSettings().ApplicationName);
        }

        internal IList<MSModel.MSScheduledAction> GetNotifications()
        {
            return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                    orderby tag.Oid
                    select tag).ToList();
        }

        public IList<MSModel.MSFolder> GetFolderCollection(MSModel.MSFolder root = null)
        {
            return (from folder in new XPQuery<MSModel.MSFolder>(uow, true).AsParallel()
                    where folder.MSFolderAss == root
                    orderby folder.Oid
                    select folder).ToList();
        }

        public IList<MSModel.MSScheduledAction> GetEventsCollection(MSModel.MSFolder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                        where tag.MSFolderAss == null
                        orderby tag.Oid
                        select tag).ToList();
            }

            var folders = (from folder in new XPQuery<MSModel.MSFolder>(uow, true).AsParallel()
                           //where folder.Oid == root.Oid
                           where folder.NodeId == root.NodeId
                           select folder).ToList();
            if (folders.Count == 0)
                return new List<MSModel.MSScheduledAction>();
            else
                return folders[0].MSScheduledActions;
        }

        public MSFolder FindFolderByNodeId(Guid nodeId)
        {
            var list = (from folder in new XPQuery<MSFolder>(uow, true).AsParallel()
                        where folder.NodeId == nodeId
                        select folder).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public MSScheduledAction FindActionByNodeId(Guid nodeId)
        {
            var list = (from folder in new XPQuery<MSScheduledAction>(uow, true).AsParallel()
                        where folder.NodeId == nodeId
                        select folder).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        public IList<MSModel.MSScheduledAction> GetCompleteEventsListOrdered(bool runtime = false, bool asc = true)
        {
            if (runtime)
                return (asc ?
                    (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                     where tag.RuntimeSelectable == true
                     orderby tag.FullName ascending
                     select tag).ToList()
                     :
                     (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                      where tag.RuntimeSelectable == true
                      orderby tag.FullName descending
                      select tag).ToList()
                    );


            return (asc ?
                    (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                     orderby tag.FullName ascending
                     select tag).ToList()
                    :
                    (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                     orderby tag.FullName descending
                     select tag).ToList()
                    );
        }

        public IList<MSModel.MSScheduledAction> GetCompleteEventsList(bool runtime = false)
        {
            if (runtime)
                return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                        where tag.RuntimeSelectable == true
                        select tag).ToList();

            return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                    select tag).ToList();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MSScheduledAction GetEvent(string name, MSFolder folder = null)
        {
            List<MSScheduledAction> list;
            if (folder == null)
                list = (from tag in new XPQuery<MSScheduledAction>(uow, true).AsParallel()
                        where tag.MSFolderAss == null && tag.Name == name
                        select tag).ToList();
            else
                list = (from tag in folder.MSScheduledActions.AsParallel()
                        where tag.Name == name
                        select tag).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        IList<String> GetEventsNodeIdList()
        {
            return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                    select tag.NodeId.ToString()).ToList();
        }

        public IList<String> GetEventsNameList(MSModel.MSFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                        where tag.MSFolderAss == null
                        select tag.Name).ToList();
            }

            return (from c in root.MSScheduledActions.AsParallel() select c.Name).ToList();
        }

        IList<String> GetFoldersNodeIdList()
        {
            return (from tag in new XPQuery<MSModel.MSFolder>(uow, true).AsParallel()
                    select tag.NodeId.ToString()).ToList();
        }

        public IList<String> GetFoldersNameList(MSModel.MSFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<MSModel.MSFolder>(uow, true).AsParallel()
                        where tag.MSFolderAss == null
                        select tag.Name).ToList();
            }

            return (from c in root.MSFolders.AsParallel() select c.Name).ToList();
        }

#if !NET_STANDARD
        public bool StartServer(bool bSave = true, bool manually = false)
        {
            return StartServer(null, null, bSave, manually);
        }

        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool bSave = true, bool manually = false)
        {
            if (bSave && NeedsSave)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                        MSServerInfo.MSServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                }

                SaveToFile();
            }

            if (!AddHttpAccessRules(false))
                return false;

            String serverConn = String.Empty;
            String stringConn = String.Empty;
            String userConn = String.Empty;
            if (fileBase != null)
            {
                serverConn = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", fileBase));

                if (editorManagerComponent.StringEditor != null)
                    stringConn = editorManagerComponent.StringEditor.GetConnectionStringFromFile(rootBase);
                if (editorManagerComponent.UserEditor != null)
                    userConn = editorManagerComponent.UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                serverConn = stringConn = userConn = ConnectionString;

            bool suspended = false;
#if !DEBUG
            if(MSZ.MSZView.IsSuspended())
                suspended = true;
#endif
            var serverCMSHelper = manually ? ServerCMSHelperAsync : ServerCMSHelperSync;
            return serverCMSHelper.StartServer(textBlock, scroll, suspended, manually, serverConn, stringConn, userConn);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MSModel.CalendarItem AddNewCalendarItem(MSModel.MSScheduledAction sched)
        {
            var cal = new MSModel.CalendarItem(uow) { MSScheduledAction = sched };
            return cal;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ExceptionsCalendarItem AddNewExceptionCalendarItem(MSScheduledAction sched)
        {
            return new ExceptionsCalendarItem(uow) { MSScheduledAction = sched };
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public WeeklyCalendarItem AddNewWeeklyCalendarItem(MSScheduledAction sched)
        {
            return new WeeklyCalendarItem(uow) { MSScheduledAction = sched };
        }
        public MSModel.MSScheduledAction AddNewEvent(MSModel.MSFolder root)
        {
            var notif = new MSModel.MSScheduledAction(uow)
            {
                Name = NewEventName(root),
                NodeId = Guid.NewGuid(),
            };
            if (root != null)
                root.MSScheduledActions.Add(notif);
            return notif;
        }

        public String NewEventName(MSModel.MSFolder root = null, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultEventName;

            if (listname == null)
                listname = GetEventsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool EventNameExists(String name, MSModel.MSFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<MSModel.MSScheduledAction>(uow, true).AsParallel()
                        where tag.MSFolderAss == null && tag.Name == name
                        select tag).ToList().Count > 0;
            }

            return (from c in root.MSScheduledActions.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        public MSModel.MSFolder AddNewFolder(MSModel.MSFolder root)
        {
            var folder = new MSModel.MSFolder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid() };
            if (root != null)
                root.MSFolders.Add(folder);
            return folder;
        }

        public String NewFolderName(MSModel.MSFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool FolderNameExists(String name, MSModel.MSFolder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<MSModel.MSFolder>(uow, true).AsParallel()
                        where folder.MSFolderAss == null && folder.Name == name
                        select folder).ToList().Count > 0;
            }

            return (from c in root.MSFolders.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MSFolder GetFolder(string name, MSFolder folder = null)
        {
            List<MSFolder> list;
            if (folder == null)
            {
                list = (from f in new XPQuery<MSFolder>(uow, true).AsParallel()
                        where f.MSFolderAss == null && f.Name == name
                        select f).ToList();
            }
            else
                list = (from c in folder.MSFolders.AsParallel() where c.Name == name select c).ToList();

            if (list.Count > 0)
                return list[0];
            return null;
        }
#endif
#endregion

#region Users
        internal IList<string> GetRolesNames(bool bRuntime = false)
        {
            var ret = new List<string>();
            if (EditorManagerComponent != null && EditorManagerComponent.UserEditor != null)
            {
                var roles = EditorManagerComponent.UserEditor.GetListRoleNames(this, bRefresh: !bRuntime);
                if (roles != null)
                    ret.AddRange(roles);
            }

            return ret;
        }
#endregion

#region Runtime Datalayer
        private static readonly String DataSourceHeader = "data source";
        private static readonly String CatalogSourceHeader = "initial catalog";
        public static String GetRuntimeConnectionString(String activeconnection)
        {
            string dbExt = Properties.Settings.Default.RuntimeDBExt;
            if (String.IsNullOrEmpty(activeconnection) || String.IsNullOrEmpty(dbExt))
                throw new ArgumentNullException("Parameters cannot be null or empty");

            ConnectionStringParser helper = new ConnectionStringParser(activeconnection);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
            if (providerType == InMemoryDataStore.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
#if !NET_STANDARD
                ds = ds.Replace('/', '\\');
#else
                ds = ds.Replace('\\', Path.DirectorySeparatorChar);
#endif
                ds = String.Format("{0}.{1}", ds, dbExt);
                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
#if !NET_STANDARD
            else if (providerType == AccessConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(DataSourceHeader);
                ds = String.Format("{0}_{1}.mdb", ds, dbExt);

                helper.UpdatePartByName(DataSourceHeader, ds);

                return helper.GetConnectionString();
            }
#endif
            else if (providerType == MSSqlConnectionProvider.XpoProviderTypeString)
            {
                string ds = helper.GetPartByName(CatalogSourceHeader);
                if (!String.IsNullOrEmpty(ds))
                    ds = String.Format("{0}_{1}", ds, dbExt);

                helper.UpdatePartByName(CatalogSourceHeader, ds);

                return helper.GetConnectionString();
            }
            else
                return helper.GetConnectionString();
        }

        public static string GetFileBase(string activeConnection)
        {
            string conn = GetRuntimeConnectionString(activeConnection);
            ConnectionStringParser helper = new ConnectionStringParser(conn);
            return helper.GetPartByName(DataSourceHeader);
        }

        public static IDataLayer GetSpecificDataLayer(string conn, out InMemoryDataStore ds, bool xml = true)
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(MSModel.MSScheduledActionRuntime).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            ds = null;
            IDataLayer dl = null;
            ConnectionStringParser helper = new ConnectionStringParser(conn);
            string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);

            if (providerType != InMemoryDataStore.XpoProviderTypeString)
            {
                dl = XpoDefault.GetDataLayer(conn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            }
            else if (xml)
            {
                string filebase = helper.GetPartByName(DataSourceHeader);
#if NET_STANDARD
                filebase = filebase.Replace('\\', Path.DirectorySeparatorChar);
#endif
                InMemoryDataStore InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                if (File.Exists(filebase))
                {
                    if (!Utilities.IO.FileSystem.IsXmlFile(filebase))
                    {
                        var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(filebase));
                        using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                        {
                            var xmlreader = XmlReader.Create(reader);
                            try
                            {
                                InMemory.ReadXml(xmlreader);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    else
                        try
                        {
                            InMemory.ReadXml(filebase);
                        }
                        catch (Exception ex)
                        {

                        }
                    dl = new ThreadSafeDataLayer(dict, InMemory);
                }
                else
                    dl = new ThreadSafeDataLayer(dict, InMemory);

                ds = InMemory;
                
            }
            return dl;
        }
        public static InMemoryDataStore GetDataStore(string filebase)
        {
            InMemoryDataStore InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            if (File.Exists(filebase))
            {
                if (!Utilities.IO.FileSystem.IsXmlFile(filebase))
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(filebase));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        var xmlreader = XmlReader.Create(reader);
                        try
                        {
                            InMemory.ReadXml(xmlreader);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                else
                    try
                    {
                        InMemory.ReadXml(filebase);
                    }
                    catch (Exception ex)
                    {

                    }
            }
            return InMemory;
        }

        public static void SaveToFile(string fileBase, InMemoryDataStore inMemory)
        {
            if (!String.IsNullOrEmpty(fileBase))
            {
                bool bProtected = false;
                if (File.Exists(fileBase))
                {
                    bProtected = !Utilities.IO.FileSystem.IsXmlFile(fileBase);
                    try
                    {
                        File.Delete(fileBase);
                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (bProtected)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        var writer = XmlWriter.Create(memoryStream);
                        inMemory.WriteXml(writer);
                        writer.Flush();
                        writer.Close();
                        var str = Convert.ToBase64String(memoryStream.ToArray());
                        var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                        File.WriteAllText(fileBase, toWrite);
                    }
                }
                else
                    inMemory.WriteXml(fileBase);
            }
        }
#endregion

#region Properties
#if !NET_STANDARD
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return uow != null && uow.TrackingChanges; }
        }
#endif

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

#if !NET_STANDARD
        NestedUnitOfWork uowContext;
        [Browsable(false)]
        public NestedUnitOfWork UowContext
        {
            get
            {
                return uowContext;
            }
            set
            {
                if (uowContext == value)
                    return;

                uowContext = value;
                OnPropertyChanged("UowContext");
            }
        }
#endif

        ISchedulerEditorManager editorManagerComponent;
        [Browsable(false)]
        public ISchedulerEditorManager EditorManagerComponent
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

        Guid configurationId;
        [Browsable(false)]
        public Guid ConfigurationId
        {
            get
            {
                if (configurationId == Guid.Empty)
                {
                    var d = GetDataLayer();
                    {
                        using (var uow = new UnitOfWork(d))
                        {
                            var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                            if (list.Count == 0)
                                return Guid.Empty;

                            configurationId = list[0].ConfigurationId;
                        }
                    }
                }

                return configurationId;
            }
            private set
            {
                if (configurationId == value)
                    return;

                var list = (from tag in new XPQuery<MSModel.MSGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                if (list.Count > 0)
                {
                    list[0].ConfigurationId = value;
                    configurationId = value;
                }
            }
        }

#if !NET_STANDARD
        MSServerCMSHelpers serverCMSHelperSync;
        [Browsable(false)]
        public MSServerCMSHelpers ServerCMSHelperSync
        {
            get
            {
                if (serverCMSHelperSync != null &&
                    !serverCMSHelperSync.IsServerStartedManually &&
                    serverCMSHelperSync.InstanceId != ConfigurationId.ToString())
                {
                    serverCMSHelperSync.Dispose();
                    serverCMSHelperSync = null;
                }

                if (serverCMSHelperSync == null)
                    serverCMSHelperSync = new MSServerCMSHelpers(ConfigurationId.ToString());

                return serverCMSHelperSync;
            }
        }

        MSServerCMSHelpers serverCMSHelperAsync;
        [Browsable(false)]
        public MSServerCMSHelpers ServerCMSHelperAsync
        {
            get
            {
                if (serverCMSHelperAsync != null &&
                    !serverCMSHelperAsync.IsServerStartedManually &&
                    serverCMSHelperAsync.InstanceId != ConfigurationId.ToString())
                {
                    serverCMSHelperAsync.Dispose();
                    serverCMSHelperAsync = null;
                }

                if (serverCMSHelperAsync == null)
                {
                    serverCMSHelperAsync = new MSServerCMSHelpers(ConfigurationId.ToString());
                    serverCMSHelperAsync.StartServerStatusInBackground();
                }

                return serverCMSHelperAsync;
            }
        }
#endif
#endregion

#region ICloneable Members

        RecipeEditorDocument(RecipeEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new RecipeEditorDocument(this);
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
                if (parent != null && uow != null && GetConfiguration().ParentApplicationName != parent.Title)
                {
                    GetConfiguration().ParentApplicationName = parent.Title;
                }
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
                return GetCompleteEventsList().Count == 0;
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

            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
#if !NET_STANDARD
            if (uowClipboard != null)
            {
                uowClipboard.Disconnect();
                uowClipboard.Dispose();
                uowClipboard = null;
            }
#endif
            if (dl != null)
            {
                dl.Dispose();
                dl = null;
            }
#if !NET_STANDARD
            if (dlClipboard != null)
            {
                dlClipboard.Dispose();
                dlClipboard = null;
            }

            UndoRedoHelper.Clear();
            foreach (var entry in mapuowUndoRedo.Values)
            {
                entry.Disconnect();
                entry.Dispose();
            }
            mapuowUndoRedo.Clear();

            foreach (var entry in mapdlUndoRedo.Values)
            {
                entry.Dispose();
            }
            mapdlUndoRedo.Clear();

            if (serverCMSHelperAsync != null)
            {
                serverCMSHelperAsync.Dispose();
            }

            if (serverCMSHelperSync != null)
            {
                serverCMSHelperSync.Dispose();
            }
#endif
        }

#endregion

#region BaseAddresses

        public MSBaseAddress AddNewBaseAddress(string transport)
        {
            var ba = new MSModel.MSBaseAddress(uow)
            {
                Enabled = true,
                Transport = transport,
                Server = "localhost",
                Port = GetConfiguration().GetDefaultPort(transport)
            };

            GetConfiguration().BaseAddresses.Add(ba);

            return ba;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MSBaseAddress GetBaseAddress(string transport)
        {
            return (from address in new XPQuery<MSBaseAddress>(uow, true)
                    where address.Transport == transport
                    select address).SingleOrDefault();
        }

#if !NET_STANDARD
        private bool AddHttpAccessRules(bool silent)
        {
            var urls = (from address in new XPQuery<MSBaseAddress>(uow, true).AsParallel()
                        select address.Path).ToArray();

            try
            {
                var httpAccessRules = new HTTPAccessRules(urls);
                httpAccessRules.CheckUrlsAndAddAccessRules();
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    var message = Properties.Resources.HttpRegistrationFailed;
                    message = message.Replace("'newline'", Environment.NewLine);
                    message = String.Format(message, ex.Message);
                    if (editorManagerComponent.UIInterface != null)
                    {
                        return editorManagerComponent.UIInterface.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes;
                    }
                    else
                    {
                        return MessageBox.Show(message, Title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                    }
                }

                return false;
            }

            return true;
        }
#endif
#endregion
#if !NET_STANDARD
        public List<MSScheduledAction> GetCRFlatTagCollection()
        {
            return (from tag in new XPQuery<MSScheduledAction>(uow, true).AsParallel()
                    select tag).ToList();
        }

        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, UFInterfaces.IWorkspace work = null, IDocumentManager manager = null)
        {
            using (var sourceDoc = FromFile(fullPath, manager, parent, false, false))
            {
                if (sourceDoc == null)
                    return;

                var targetConn = newPath;
                if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                    targetConn = GetConnectionString(newPath, null);
                using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uowTarget = new UnitOfWork(dlTarget))
                    {
                        var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, false, true, true);

                        ////////////////////////////////////////////////////////////////////////////
                        // delete all first
                        (from p in new XPQuery<MSModel.MSFolder>(uowTarget, true).AsParallel()
                         where p.MSFolderAss == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<MSModel.MSScheduledAction>(uowTarget, true).AsParallel()
                         where p.MSFolderAss == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<MSModel.MSGeneralSettings>(uowTarget, true).AsParallel()
                         select p).ToList().ForEach(tag => tag.Delete());

                        // following is only for ensuring the completely deletion of unassociated object elements
                        (from p in new XPQuery<MSModel.WeeklyCalendarItem>(uowTarget, true).AsParallel()
                         where p.MSScheduledAction == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<MSModel.CalendarItem>(uowTarget, true).AsParallel()
                         where p.MSScheduledAction == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<MSModel.MSAction>(uowTarget, true).AsParallel()
                         where p.MSScheduledAction == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////
                        var folders = sourceDoc.GetFolderCollection();
                        foreach (var folder in folders)
                        {
                            cloneHelper.Clone(folder, false);
                        }
                        var events = sourceDoc.GetEventsCollection();
                        foreach (var evento in events)
                        {
                            cloneHelper.Clone(evento, false);
                        }

                        var cloneHelperConf = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, true, true, true);
                        cloneHelperConf.Clone(sourceDoc.GetConfiguration(), false);

                        uowTarget.CommitChanges();
                        uowTarget.PurgeDeletedObjects();
                    }
                }
            }
            if (!bCopy)
                RemoveFile(fullPath, parent);
            return;
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
                    var folders = sourceDoc.GetFolderCollection();
                    foreach (var folder in folders)
                    {
                        folder.Delete();
                    }
                    var events = sourceDoc.GetEventsCollection();
                    foreach (var evento in events)
                    {
                        evento.Delete();
                    }
                    sourceDoc.GetGeneralSettings().Delete();
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
#endif
    }
}
