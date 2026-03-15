using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using VFS;
using System.Windows.Controls;
using PropertyControl.ComponentService;
using UIMsgBoxAlertService.ComponentService;
#endif
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using System.Windows;
using System.Xml;
using Utilities;
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using log4net;
using TempVariablesModel;
using UFInterfaces;
using UFProjectManager.ComponentService;

namespace TempVariablesManager.Document
{
    public class TempVariablesPersistence : ViewModelBase, IDocument, ICloneable
    {
        #region Declarations
        internal UnitOfWork uow;
        InMemoryDataStore InMemory;
        IDataLayer dl;
#if !NET_STANDARD
        //UnitOfWork uowCloner;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dlClipboard;
#endif
        IUFProjectManager iUFProjectManager;
        String connectionString;
        String fileBase;
#if !NET_STANDARD
        readonly Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper> UndoRedoHelper = new Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper>();
        readonly Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer> mapdlUndoRedo = new Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapuowUndoRedo = new Dictionary<IDataLayer, UnitOfWork>();

        static readonly ILog logGeneral = LogManager.GetLogger(Properties.Resources.GeneralLog);
#else
        static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
#endif
        #endregion

        #region Methods
        IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(Variable).Assembly);
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

        void CreateDataLayer()
        {
            dl = GetDataLayer();
            uow = new UnitOfWork(dl);
#if !NET_STANDARD
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.AfterRollbackTransaction += (o,e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.AfterBeginTrackingChanges += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            uow.ObjectDeleting += (o, e) =>
            {
                NeedToReloadAddressSpace = true;
            };

            uow.ObjectsSaved += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            //uowCloner = new UnitOfWork(dl);

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

        internal IList<Variable> GetFlatTagCollection()
        {
            return (from tag in new XPQuery<Variable>(uow, true)
                    orderby tag.Oid
                    select tag).ToList();
        }
        internal IList<Variable> GetTagCollection(Folder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<Variable>(uow, true).AsParallel()
                        where tag.Folder == null
                        orderby tag.Name ascending
                        select tag).ToList();
            }

            var folders = (from folder in new XPQuery<Folder>(uow, true).AsParallel()
                               //where folder.Oid == root.Oid
                           where folder.NodeId == root.NodeId
                           orderby folder.Name ascending
                           select folder).ToList();
            if (folders.Count == 0)
                return new List<Variable>();
            else
                return (from c in folders[0].Variables.AsParallel() orderby c.Name ascending select c).ToList();
        }

#if !NET_STANDARD
        internal void CreateUndoRedoHelper(UserControl owner)
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

        internal void CleanUndoRedoHelper(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                return;

            var helper = UndoRedoHelper[owner];
            var idl = mapdlUndoRedo[helper];
            var uow = mapuowUndoRedo[idl];

            UndoRedoHelper.Remove(owner);
            uow.Disconnect();
            uow.Dispose();
            idl.Dispose();
        }

        internal IList<Folder> GetFolderCollection(Folder root = null)
        {
            return (from folder in new XPQuery<Folder>(uow, true).AsParallel()
                    where folder.FolderAss == root
                    orderby folder.Name ascending
                    select folder).ToList();
        }
#endif

#region WinClipboard
#if !NET_STANDARD
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
#endif
#endregion

#region Undo/Redo
#if !NET_STANDARD
        static String UndoRedoNotInizialized = "The operation cannot be performed because the undo-redo manager has not been initialized for this control.";
        internal void AddUndoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(list, action);
        }

        internal void AddRedoAction(UserControl owner, IList<IXPSimpleObject> list, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(list, action);
        }

        internal void AddUndoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddUndoAction(obj, action);
        }

        internal void AddRedoAction(UserControl owner, XPObject obj, XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].AddRedoAction(obj, action);
        }

        internal bool UndoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanUndo();
        }

        internal bool RedoContainsSomething(UserControl owner)
        {
            if (bObjectDisposed || !UndoRedoHelper.ContainsKey(owner))
                return false;

            return UndoRedoHelper[owner].CanRedo();
        }

        internal void CleanUndoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeUndoActions(true);
        }

        internal void CleanRedoActions(UserControl owner)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            UndoRedoHelper[owner].PurgeRedoActions(true);
        }

        internal List<XPObject> UndoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            List<XPObject> objectsToDelete;
            if (UndoRedoHelper[owner].CanUndo())
            {
                var ret = UndoRedoHelper[owner].Undo(out action, out objectsToDelete);
                if (objectsToDelete != null && objectsToDelete.Count > 0)
                {
                    CopyAssociationReferences(objectsToDelete, ret);
                    objectsToDelete.ForEach((obj) => obj.Delete());
                    UndoRedoHelper[owner].PurgeUndoActions();
                }
                return ret;
            }

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }
        public void CopyAssociationReferences(List<XPObject> sources, List<XPObject> targets)
        {
            if (sources.Count != targets.Count)
                return;

            for (int ii = 0; ii < sources.Count; ii++)
                CopyAssociationReferences(sources[ii], targets[ii]);
        }
        public void CopyAssociationReferences(XPObject source, XPObject target)
        {
            if (source.GetType() != target.GetType())
                return;

            if (source is Folder)
                CopyAssociationReferences(source as Folder, target as Folder);
        }
        public void CopyAssociationReferences(Folder source, Folder target)
        {
            foreach (var sourcefolder in source.Folders)
            {
                foreach (var targetfolder in target.Folders)
                {
                    if (sourcefolder.GetRelativeName() == targetfolder.GetRelativeName())
                    {
                        CopyAssociationReferences(sourcefolder as Folder, targetfolder as Folder);
                        break;
                    }
                }
            }
        }
        internal List<XPObject> RedoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            List<XPObject> objectsToDelete;
            if (UndoRedoHelper[owner].CanRedo())
            {
                var ret = UndoRedoHelper[owner].Redo(out action, out objectsToDelete);
                if (objectsToDelete != null && objectsToDelete.Count > 0)
                {
                    CopyAssociationReferences(objectsToDelete, ret);
                    objectsToDelete.ForEach((obj) => obj.Delete());
                    UndoRedoHelper[owner].PurgeRedoActions();
                }
                return ret;
            }

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        internal void AddExistingObject(XPObject obj, object parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);

            if (obj is Variable)
            {
                var tag = obj as Variable;
                if (parent is Folder)
                {
                    var root = parent as Folder;
                    AddExistingObject(tag, FindFolderByNodeId(root.NodeId));
                }
            }
            else if (obj is Folder)
            {
                var folder = obj as Folder;
                if (parent is Folder)
                {
                    var root = parent as Folder;
                    AddExistingObject(folder, FindFolderByNodeId(root.NodeId));
                }
            }
        }
        void AddExistingObject(Variable tag, Folder root)
        {
            if (root != null)
                root.Variables.Add(tag);
        }
        void AddExistingObject(Folder folder, Folder root)
        {
            if (root != null)
                root.Folders.Add(folder);
        }
#endif
#endregion

#region Nested Objects
#if !NET_STANDARD
        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
        }

        internal XPObject GetNestedObject(object obj)
        {
            if (obj == null || !(obj is XPObject) || (obj as XPObject).IsLoading || (obj as XPObject).IsDeleted)
                return null;

            UowContext = BeginNestedUnitOfWork();
            return UowContext.GetNestedObject(obj as XPObject);
        }

        internal List<XPObject> GetNestedObjects(System.Collections.IList objects)
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
#endif
#endregion

        static String GetConnectionString(String path,
#if !NET_STANDARD
            FileSystemProviderBase vfs, 
#endif
            bool bForceCreate = false)
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
                                    Properties.Settings.Default.TypeScheme,
                                    Properties.Settings.Default.DefaultProjectName,
                                    Properties.Settings.Default.DefaultFileExt);

                connString = InMemoryDataStore.GetConnectionString(String.Format("\"{0}\"", xmlfile));

                if (bForceCreate)
                {
                    var absolute = Path.GetDirectoryName(xmlfile);
                    Directory.CreateDirectory(absolute);
                }
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
                                Properties.Settings.Default.TypeScheme,
                                Properties.Settings.Default.DefaultProjectName,
                                Properties.Settings.Default.DefaultFileExt);
        }

#if !NET_STANDARD
        public static void CopyFile(String fullPath, String newPath, bool bCopy,
            IDocument parent, UFInterfaces.IWorkspace work = null)
        {
            string ret = string.Empty;
            using (var sourceDoc = FromFile(fullPath, parent, false, false))
            {
                if (sourceDoc == null)
                    return;

                // TODO: code for copying all document content
                var targetConn = newPath;
                if (!XpoHelpers.XpoHelper.IsDataSource(targetConn))
                    targetConn = GetConnectionString(newPath, null,true);
                using (var dlTarget = XpoDefault.GetDataLayer(targetConn, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema))
                {
                    using (var uowTarget = new UnitOfWork(dlTarget))
                    {
                        var cloneHelper = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, false, true, true);

                        ////////////////////////////////////////////////////////////////////////////
                        // delete all first
                        var cursor = new XPCursor(uowTarget, typeof(Folder),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("FolderAss")));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        cursor = new XPCursor(uowTarget, typeof(Variable),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("Folder")));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        uowTarget.CommitChangesAndFreeMemory();
                        ////////////////////////////////////////////////////////////////////////////

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(Folder),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("FolderAss")));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        cursor = new XPCursor(sourceDoc.GetSession(), typeof(Variable),
                            new GroupOperator(GroupOperatorType.And,
                                new NullOperator("Folder")));
                        foreach (XPBaseObject item in cursor)
                        {
                            cloneHelper.Clone(item, false);
                        }

                        uowTarget.CommitChangesAndFreeMemory();
                    }
                }
            }

            if (!bCopy)
                RemoveFile(fullPath, parent);
        }

        public static void RenameFile(String fullPath, String oldName, String newName,
            FileSystemProviderBase fileSystemProvider = null)
        {
        }

        public static void RemoveFile(string fullPath, IDocument parent)
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
                using (var sourceDoc = FromFile(fullPath, parent, false, false))
                {
                    if (sourceDoc == null)
                        return;

                    // TODO: code for removing all document content
                    var tags = sourceDoc.GetFlatTagCollection();
                    foreach (var tag in tags)
                    {
                        tag.Delete();
                    }
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
#endif

        internal UnitOfWork GetSession()
        {
            return uow;
        }

        public static TempVariablesPersistence FromFile(String path, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true, bool bThrowExceptions = false)
        {
            String connString = null;
            String xmlfile = null;
            if (parent == null)
                return null;
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
#endif
                connString = GetConnectionString(path
#if !NET_STANDARD
                    , vfs
#endif
                    );
                xmlfile = GetBaseFilename(path
#if !NET_STANDARD
                    , vfs
#endif
                    );

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                var doc = new TempVariablesPersistence(null)
                {
                    connectionString = connString,
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
                    if (bThrowExceptions)
                        throw new UnauthorizedAccessException(String.Format(Properties.Resources.ErrorValidatingDocument, xmlfile ?? connString));

#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, xmlfile ?? connString));
                    logGeneral.ErrorFormat(Properties.Resources.ErrorValidatingDocument, xmlfile ?? connString);
                    var iUFProjectManager = parent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                    if (iUFProjectManager != null)
                        iUFProjectManager.AddLogEntity(parent, Properties.Resources.GeneralLog, 
                            DateTime.UtcNow,string.Format(Properties.Resources.ErrorValidatingDocument, xmlfile ?? connString), 
                            System.Diagnostics.EventLogEntryType.Error);
#endif
                    return null;
                }

                return doc;
            }
            catch
            {
                if (bThrowExceptions)
                    throw;

#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, xmlfile ?? connString));
                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, xmlfile ?? connString);
                var iUFProjectManager = parent.GetService(typeof(IUFProjectManager)) as IUFProjectManager;
                if (iUFProjectManager != null)
                    iUFProjectManager.AddLogEntity(parent, Properties.Resources.GeneralLog, 
                        DateTime.UtcNow,string.Format(Properties.Resources.ErrorReadingDocument, xmlfile ?? connString), 
                        System.Diagnostics.EventLogEntryType.Error);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        internal bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return false;

            List<XPObject> parentObjects = null;
            if (Workspace != null && ActiveView != null)
            {
                if(Workspace.ActiveWindow == ActiveView)
                {
                    Workspace.UpdateContextNow();
                    var objects = Workspace.ContextObjects;
                    if (Workspace.ContextObject != null)
                    {
                        if (objects == null)
                            objects = new List<object>();
                        objects.Add(Workspace.ContextObject);
                    }

                    if (objects != null)
                        parentObjects = GetParentObjects(objects);
                }
            }

            try
            {
                bool bSave = NeedsSave;
                if (discargechanges &&
                    uow.TryPurgeDeletedObjects(logGeneral) > 0)
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
                    if (!Directory.Exists(Path.GetDirectoryName(fileBase)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fileBase));
                    }

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
                if (Environment.UserInteractive)
                    MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                        Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                OnPropertyChanged("NeedsSave");
            }

            if (parentObjects != null && parentObjects.Count > 0)
            {
                if (parentObjects.Count == 1)
                    Workspace.ContextObject = GetNestedObject(parentObjects[0]);
                else
                    Workspace.ContextObject = GetNestedObjects(parentObjects);
            }

            return true;
        }

        internal void CopyListTagsToClipbaord(List<Variable> list)
        {
            list.ForEach(item =>
            {
                CloneToClipboard(item, false);
            });
            uowClipboard.CommitChanges();
        }
        internal void CopyListFoldersToClipbaord(List<Folder> list)
        {
            list.ForEach(item =>
            {
                CloneToClipboard(item, false);
            });
            uowClipboard.CommitChanges();
        }
        internal bool ClipboardContainsFolders()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<Folder>(uowClipboard).AsParallel() select c).ToList().Count > 0 || (from c in new XPQuery<UFUAModel.UFUAFolder>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal bool ClipboardContainsTags()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<Variable>(uowClipboard).AsParallel() select c).ToList().Count > 0 || (from c in new XPQuery<UFUAModel.UFUATag>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }
       
        public Variable AddNewTag(Folder root, int orderid = -1)
        {
            var name = String.Empty;
            var tag = new Variable(uow) { Name = NewTagName(root, name), NodeId = Guid.NewGuid(), MemberOrderId = orderid};
            if (root != null)
                root.Variables.Add(tag);
            return tag;
        }

        public Folder AddNewFolder(Folder root, int orderid = -1)
        {
            var folder = new Folder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid(), MemberOrderId = orderid };
            if (root != null)
                root.Folders.Add(folder);
            return folder;
        }
        public Folder FindFolderByName(String name, Folder folder = null)
        {
            if (folder == null)
            {
                return (from fldr in new XPQuery<Folder>(uow, true).AsParallel()
                        where fldr.FolderAss == null && fldr.Name == name
                        select fldr).FirstOrDefault();
            }
            return (from c in folder.Folders.AsParallel() where c.Name == name select c).FirstOrDefault();
        }

        internal Folder FindFolderByNodeId(Guid guid)
        {
            return (from folder in new XPQuery<Folder>(uow, true).AsParallel()
                    where folder.NodeId == guid
                    select folder).FirstOrDefault();
        }

        internal List<object> PasteClipboardUFUAFoldersAndTags(Folder folder, UFUAModel.UFUAFolder parent = null)
        {
            var ret = new List<object>();
            var listUFUAFolderToCopy = (from c in new XPQuery<UFUAModel.UFUAFolder>(uowClipboard).AsParallel()
                                        where c.UFUAFolderAss == parent
                                        //orderby c.Oid
                                        select c).ToList();
            if (listUFUAFolderToCopy.Count > 0)
            {
                var mapStartCounter = new Dictionary<string, ulong>();
                var listName = GetFoldersNameList(folder);
                listUFUAFolderToCopy.ForEach(dir =>
                {
                    var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                    var newfolder = AddNewCopyFolder(folder);
                    newfolder.Name = newName;
                    if (dir.UFUAFolders.Count > 0)
                        dir.UFUAFolders.ToList().ForEach(f => PasteClipboardUFUAFoldersAndTags(newfolder, dir));

                    if (newfolder != null)
                    {
                        if (folder != null)
                            folder.Folders.Add(newfolder);
                        ret.Add(newfolder);

                        var listUFUATagToCopy = (from c in new XPQuery<UFUAModel.UFUATag>(uowClipboard).AsParallel()
                                                 where c.UFUAFolder == dir
                                                 //orderby c.Oid
                                                 select c).ToList();
                        if (listUFUATagToCopy.Count > 0)
                        {
                            var mapStartTagsCounter = new Dictionary<string, ulong>();
                            var listTagsName = GetTagsNameList(folder);
                            listUFUATagToCopy.ForEach(tag =>
                            {
                                var newTagName = NewTagName(newfolder, tag.Name, mapStartTagsCounter, listTagsName);
                                var newtag = AddNewTag(newfolder);
                                newtag.Name = newTagName;
                                try
                                {
                                    newtag.Description = tag.Description;
                                    newtag.DataType = tag.DataType.HasValue ? tag.DataType.Value : newtag.DataType;
                                    newtag.InitialValue = tag.InitialValue;
                                    newtag.ArrayDimension = (int)tag.ArrayDimension;
                                }
                                catch (Exception)
                                {
                                }

                                if (newfolder != null)
                                    newfolder.Variables.Add(newtag);
                            });
                        }
                    }
                });
            }
            if (parent == null)
            {
                var listmainUFUATagToCopy = (from c in new XPQuery<UFUAModel.UFUATag>(uowClipboard).AsParallel()
                                             where c.UFUAFolder == null
                                             //orderby c.Oid
                                             select c).ToList();
                if (listmainUFUATagToCopy.Count > 0)
                {
                    var mapStartTagsCounter = new Dictionary<string, ulong>();
                    var listTagsName = GetTagsNameList(folder);
                    listmainUFUATagToCopy.ForEach(tag =>
                    {
                        var newTagName = NewTagName(folder, tag.Name, mapStartTagsCounter, listTagsName);
                        var newtag = AddNewTag(folder);
                        newtag.Name = newTagName;
                        try
                        {
                            newtag.Description = tag.Description;
                            newtag.DataType = tag.DataType.HasValue ? tag.DataType.Value : newtag.DataType;
                            newtag.InitialValue = tag.InitialValue;
                            newtag.ArrayDimension = (int)tag.ArrayDimension;
                        }
                        catch (Exception)
                        {
                        }

                        if (folder != null)
                            folder.Variables.Add(newtag);

                        ret.Add(newtag);
                    });
                }
            }
            return ret;
        }
        internal Folder AddNewCopyFolder(Folder root, int orderid = -1)
        {
            var folder = new Folder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid(), MemberOrderId = orderid };
            if (root != null)
                root.Folders.Add(folder);
            return folder;
        }

        internal List<Folder> PasteClipboardFolders(Folder folder, Folder parent = null)
        {
            var ret = new List<Folder>();
            var listToCopy = (from c in new XPQuery<Folder>(uowClipboard).AsParallel()
                              where c.FolderAss == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new Helpers.GuidHelper(uow, Helpers.TypeObject.Folder);
            var listName = GetFoldersNameList(folder);
            listToCopy.ForEach(dir =>
            {
                var exist = guidHelper.Contains(dir.NodeId);
                var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                var newfolder = CloneFromClipboard(dir, exist) as Folder;
                CopyAssociationReferences(dir, newfolder); // Must be called before changing the name of the cloned object.
                newfolder.Name = newName;
                if (folder != null)
                    folder.Folders.Add(newfolder);
                ret.Add(newfolder);
                if (!exist)
                    guidHelper.EnsureValidGuid(newfolder);
            });
            return ret;
        }

        internal IList<String> GetFoldersNameList(Folder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<Folder>(uow, true).AsParallel()
                        where folder.FolderAss == null
                        select folder.Name).ToList();
            }

            return (from c in root.Folders.AsParallel() select c.Name).ToList();
        }

        internal String NewFolderName(Folder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool FolderNameExists(String name, Folder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<Folder>(uow, true).AsParallel()
                        where folder.FolderAss == null && folder.Name == name
                        select folder).ToList().Count > 0;
            }

            return (from c in root.Folders.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        internal List<Variable> PasteClipboardTags(Folder folder, Folder parent = null)
        {
            var ret = new List<Variable>();
            var listToCopy = (from c in new XPQuery<Variable>(uowClipboard).AsParallel()
                              where c.Folder == parent
                              //orderby c.Oid
                              select c).ToList();
            if (listToCopy.Count == 0)
                return ret;
            var mapStartCounter = new Dictionary<string, ulong>();
            var guidHelper = new Helpers.GuidHelper(uow, Helpers.TypeObject.Variable);
            var listName = GetTagsNameList(folder);
            listToCopy.ForEach(tag =>
            {
                var exist = guidHelper.Contains(tag.NodeId);
                var newName = NewTagName(folder, tag.Name, mapStartCounter, listName);
                var newtag = CloneFromClipboard(tag, exist) as Variable;
                newtag.Name = newName;
                if (folder != null)
                    folder.Variables.Add(newtag);
                ret.Add(newtag);
            });
            return ret;
        }
        internal IList<String> GetTagsNameList(Folder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<Variable>(uow, true).AsParallel()
                        where tag.Folder == null
                        select tag.Name).ToList();
            }

            return (from c in root.Variables.AsParallel() select c.Name).ToList();
        }

        internal String NewTagName(Folder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultTagName;

            if (listname == null)
                listname = GetTagsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }
        
        public Variable GetTag(String name, Folder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<Variable>(uow, true).AsParallel()
                        where tag.Folder == null && tag.Name == name
                        select tag).FirstOrDefault();
            }

            return (from c in root.Variables.AsParallel() where c.Name == name select c).FirstOrDefault();
        }

        public Folder GetFolder(String name, Folder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<Folder>(uow, true).AsParallel()
                        where tag.FolderAss == null && tag.Name == name
                        select tag).FirstOrDefault();
            }

            return (from c in root.Folders.AsParallel() where c.Name == name select c).FirstOrDefault();
        }
        bool TagNameExists(String name, Folder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<Variable>(uow, true).AsParallel()
                        where tag.Folder == null && tag.Name == name
                        select tag).ToList().Count > 0;
            }

            return (from c in root.Variables.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }
#endif       
#endregion

#region Properties
#if !NET_STANDARD                
        [Browsable(false)]
        public bool NeedsSave
        {
            get { return uow != null && uow.TrackingChanges; }
        }

        bool needToReloadAddressSpace;
        [Browsable(false)]
        public bool NeedToReloadAddressSpace
        {
            get
            {
                return needToReloadAddressSpace;
            }
            set
            {
                if (needToReloadAddressSpace == value)
                    return;

                needToReloadAddressSpace = value;
                OnPropertyChanged("NeedToReloadAddressSpace");
            }
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
        internal NestedUnitOfWork UowContext
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

        IWorkspace workspace;
        public IWorkspace Workspace
        {
            get
            {
                if (workspace == null)
                    workspace = GetService(typeof(IWorkspace)) as IWorkspace;
                return workspace;
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

#if !NET_STANDARD
        IUIMsgBoxAlertService uIInterface;
        internal IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uIInterface == null)
                    uIInterface = this.Parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uIInterface;
            }
        }

        IPropertyControl propertyControl;
        internal IPropertyControl PropertyControl
        {
            get
            {
                if (propertyControl == null)
                    propertyControl = this.Parent.GetService(typeof(IPropertyControl)) as IPropertyControl;
                return propertyControl;
            }
        }
#endif
#endregion

#region ICloneable Members

        TempVariablesPersistence(TempVariablesPersistence template)
        {
            iUFProjectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;
            if (template == null)
                return;
        }

        public object Clone()
        {
            return new TempVariablesPersistence(this);
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

#if !NET_STANDARD
            SaveToFile(true);

            if (uowContext != null)
            {
                uowContext.Dispose();
                uowContext = null;
            }
#endif

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
#endif
        }

        internal Variable GetVariable(string path)
        {
#if !NET_STANDARD
            var sep = '&';
            if(path.IndexOf(sep) > 0)
            {
                var parts = path.Split(sep);
                Folder root = null;
                for (int i = 0; i < parts.Count() - 1; i++)
                {
                    Folder folder = GetFolder(parts[i], root);
                    if (folder != null)
                    {
                        root = folder;
                    }
                    else
                        break;
                }
                return GetTag(parts.LastOrDefault(), root);
            }
            else
            {
                return GetTag(path, null);
            }
#else
            return null;
#endif
        }

        #endregion
    }
}
