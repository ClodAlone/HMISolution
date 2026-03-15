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
using ADEditor.ComponentService;
using ADModel;
using System.Diagnostics;
using System.ServiceModel;
using Utilities;
using UIMsgBoxAlertService.ComponentService;
using Opc.Ua;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using System.Reflection;
using ADPluginSettingsInterface;
using System.Text;
using WPFUtilities.Extensions;
using System.Threading.Tasks;
using WPFUtilities;
using UFUAEditor.ComponentService;
using OPCUAViewModelService.ComponentService;
using System.Windows.Threading;
using Utilities.WPF;
using OPCUAViewModel;
using ADServerCMS;
using log4net;

namespace ADEditor.Document
{
    public class ADEditorDocument : ViewModelBase, ICloneable, IDocument
    {
        #region Declarations
        
        UnitOfWork uow;
        //UnitOfWork uowCloner;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemory;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dl;
        IDataLayer dlClipboard;
        String defaultApplicationName;
        String connectionString;
        String fileBase;
        
        readonly Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper> UndoRedoHelper = new Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper>();
        readonly Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer> mapdlUndoRedo = new Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapuowUndoRedo = new Dictionary<IDataLayer, UnitOfWork>();

        internal static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);

        #endregion

        #region Methods
        internal void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            IUFUAEditorManager UfuaEditorService = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UfuaEditorService == null)
                return;
            string defaultlocalendpoint = UfuaEditorService.GetDefaultLocalEndpoint(this, true);
            var endpointslist = UfuaEditorService.GetEndpoints(this);
            string aplicationName = UfuaEditorService.GetAplicationName(this, true);
            if (ADEditorManagerComponent.adeditorManagerComponent.UfuaEditorService != null)
            {
                using (var cursor = new WaitCursor())
                {
                    List<ADModel.ADNotification> taglist = GetCRFlatTagCollection();
                    var list = new List<OPCUAViewModel.OPCUAEntityReference>();
                    taglist.ForEach(item =>
                    {
                        list.AddRange(GetOPCReferenceList(item));
                    });
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var mapNodes = ADEditorManagerComponent.adeditorManagerComponent.UfuaEditorService.GetListNodeNames(this, nodelist);
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
                        found.ForEach(fitem =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            taglist.ForEach(item =>
                            {
                                bool bTag = false;
                                try { bTag = item.NotificationItem.Contains(fitem.ResolvedNodeId.ToString()); } catch { };

                                bool changeEndpoint = !endpointslist.Contains(fitem.EndpointUrl);
                                if (changeEndpoint)
                                {
                                    fitem.EndpointUrl = fitem.EndpointUrl.Replace(fitem.AppName, aplicationName);
                                    if (!endpointslist.Contains(fitem.EndpointUrl))
                                        fitem.EndpointUrl = defaultlocalendpoint;
                                }
                                fitem.AppName = aplicationName;
                                var shortname = mapNodes[node];
                                var newName = String.Format("{0} ({1})", shortname, fitem.AppName);
                                if (fitem.HumanReadable != newName || changeEndpoint)
                                {
                                    fitem.HumanReadable = newName;
                                    fitem.ReadablePath = CrossReferenceHelper.Helper.GetNewPath(shortname, fitem.ReadablePath);
                                    fitem.RelativePath = CrossReferenceHelper.Helper.GetNewPath(shortname, fitem.RelativePath);
                                }
                                string _xmltag = fitem.ToXml();
                                if (bTag)
                                {
                                    bDirty = true;
                                    item.NotificationItem = _xmltag;
                                }
                            });

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
        private List<OPCUAEntityReference> GetOPCReferenceList(ADModel.ADNotification eventitem)
        {
            var list = new List<OPCUAViewModel.OPCUAEntityReference>();
            if (!string.IsNullOrEmpty(eventitem.NotificationItem))
            {
                var tag = eventitem.NotificationItem.FromXml<OPCUAEntityReference>();
                list.Add(tag);
            }
            return list;
        }
        internal List<ADNotification> GetCRFlatTagCollection()
        {
            return (from tag in new XPQuery<ADModel.ADNotification>(uow, true).AsParallel()
                    select tag).ToList();
        }

        public static OPCUAEntityReference BrowseLocalItem(ADModel.ADNotification tag, OPCUAEntityReference item)
        {
            if (item.Edit(sync:true, noDataSinks: true))
            {
                ADEditorDocument.UpdateNotificationItem(tag, item);
                return item;
            }
            return item;
        }

        public static OPCUAEntityReference BrowseServerAlarm(ADEditorDocument doc, ADModel.ADNotification a)
        {
            OPCUAEntityReference item = null;
            IUFUAEditorManager editor = null;
            OPCUAViewModelComponent.QueryInterfaces();
            if (OPCUAViewModelComponent.ufuaEditorServiceAvailable &&
                OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                editor = OPCUAViewModelComponent.ufuaEditorService;
            }

            OPCUAViewModelComponent.workspaceService.IsBusy = true;
            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                OPCUAViewModelComponent.workspaceService.IsBusy = false;
            });

            if (editor == null)
                return null;

            var alrbrowser = editor.GetAlarmListControl(doc, true);
            if (alrbrowser == null)
                return null;

            alrbrowser.ClearValue(FrameworkElement.WidthProperty);
            alrbrowser.ClearValue(FrameworkElement.HeightProperty);
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = alrbrowser;

            GeneralDialogContent wnd = new GeneralDialogContent(scrollViewer)
            {
                DialogKeepContent = true,
                Title = Properties.Resources.AlarmList,
                HelpLink = "BrowseServerAlarm"
            };
            bool bRet = wnd.ShowDialog() == true;
            if (bRet)
            {
                var serverreference = alrbrowser.DataContext;
                if (serverreference != null && (serverreference as object[]) != null && (serverreference as object[]).Length == 2)
                {
                    var serverentity = (serverreference as object[])[0] as string;
                    try
                    {
                        if (serverentity != null)
                        {
                            item = serverentity.FromXml<OPCUAViewModel.OPCUAEntityReference>();
                        }
                    }
                    catch (Exception e)
                    {

                        return null;
                    }

                    if (((serverreference as object[])[1] is List<object>) && ((serverreference as object[])[1] as List<object>).Count > 0)
                    {

                    }
                    if (((serverreference as object[])[1] as List<object>)[0] is UFUAModel.UFUAAlarmThreshold)
                    { }
                    else if (((serverreference as object[])[1] as List<object>)[0] is UFUAModel.UFUAAlarmSource)
                    {
                        var alrlist = (serverreference as object[])[1] as List<object>;
                        if (alrlist.Count > 0)
                        {
                            //ADModel.ADNotification a = DataContext as ADModel.ADNotification;
                            if (a != null)
                            {
                                a.AlarmName = (alrlist[0] as UFUAModel.UFUAAlarmSource).GetRelativeName();
                                ADEditorDocument.UpdateNotificationItem(a, item, alrlist[0] as UFUAModel.UFUAAlarmSource);
                            }
                        }
                    }
                    else if (((serverreference as object[])[1] as List<object>)[0] is UFUAModel.UFUAArea)
                    {
                        var alrlist = (serverreference as object[])[1] as List<object>;
                        if (alrlist.Count > 0)
                        {
                            //ADModel.ADNotification a = DataContext as ADModel.ADNotification;
                            if (a != null)
                            {
                                a.AlarmName = ADEditorDocument.GetCompleteAreaName(alrlist[0] as UFUAModel.UFUAArea);
                                ADEditorDocument.UpdateNotificationItem(a, item, alrlist[0] as UFUAModel.UFUAArea);
                            }
                        }
                    }

                }
                //var abc = alrbrowser.
            }
            return item;
        }
        public static void UpdateNotificationItem(ADModel.ADNotification a, OPCUAEntityReference item, object alrid = null)
        {
            if (a != null && item != null && item.IsValid)
                a.NotificationItem = item.ToXml();
            else
                a.NotificationItem = string.Empty; 
        }
        public static string GetCompleteAreaName(UFUAModel.UFUAArea area)
        {
            StringBuilder s = new StringBuilder(area.Name);
            if (area.UFUAAreaAss != null)
            {
                s.Insert(0, "/");
                s.Insert(0, GetCompleteAreaName(area.UFUAAreaAss));
            }
            return s.ToString();
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

        IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(ADModel.ADNotification).Assembly);
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
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");

                if (e.PropertyName == "ApplicationName")
                {
                    ConfigurationId = Guid.NewGuid();
                }
            };
            //uowCloner = new UnitOfWork(dl);

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);

        }

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }
        internal UnitOfWork GetSession()
        {
            return uow;
        }

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
        #endregion

        #region Clipboard

        internal void CopyListFoldersToClipbaord(List<ADFolder> list)
        {
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListNotificationsToClipbaord(List<ADNotification> list)
        {
            list.ForEach(tag =>
                {
                    CloneToClipboard(tag, false);
                });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsFolders()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<ADFolder>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal bool ClipboardContainsNotifications()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<ADNotification>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal List<ADFolder> PasteClipboardFolders(ADFolder folder, ADFolder parent = null)
        {
            var ret = new List<ADFolder>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetFoldersNodeIdList();
            var listName = GetFoldersNameList(folder);
            var listToCopy = (from c in new XPQuery<ADFolder>(uowClipboard).AsParallel() where c.ADFolderAss == parent orderby c.Oid select c).ToList();
            listToCopy.ForEach(dir =>
            {
                var exist = listNodeId.Contains(dir.NodeId.ToString());
                var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                var newfolder = CloneFromClipboard(dir, exist) as ADFolder;
                newfolder.Name = newName;
                if (folder != null)
                    folder.ADFolders.Add(newfolder);
                ret.Add(newfolder);
            });
            return ret;
        }

        internal List<ADNotification> PasteClipboardNotifications(ADFolder folder, ADFolder parent = null)
        {
            var ret = new List<ADNotification>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetNotificationsNodeIdList();
            var listName = GetNotificationsNameList(folder);
            var listToCopy = (from c in new XPQuery<ADNotification>(uowClipboard).AsParallel() where c.ADFolder == parent orderby c.Oid select c).ToList();
            listToCopy.ForEach(tag =>
                {
                    var exist = listNodeId.Contains(tag.NodeId.ToString());
                    var newName = NewNotificationName(folder, tag.Name, mapStartCounter, listName);
                    var newtag = CloneFromClipboard(tag, exist) as ADNotification;
                    newtag.Name = newName;
                    if(folder != null)
                        newtag.ADFolder = folder;
                    ret.Add(newtag);
                });
            return ret;
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

        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
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

        #region Undo/Redo

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

            if (UndoRedoHelper[owner].CanUndo())
                return UndoRedoHelper[owner].Undo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        internal List<XPObject> RedoAction(UserControl owner, out XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions action)
        {
            if (!UndoRedoHelper.ContainsKey(owner))
                throw new KeyNotFoundException(UndoRedoNotInizialized);

            if (UndoRedoHelper[owner].CanRedo())
                return UndoRedoHelper[owner].Redo(out action);

            action = XpoHelpers.UndoRedoIXPSimpleObjectHelper.UndoRedoActions.None;
            return null;
        }

        internal void AddExistingObject(XPObject obj, object parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);

            if (obj is ADNotification)
            {
                var tag = obj as ADNotification;
                if (parent is ADFolder)
                {
                    var root = parent as ADFolder;
                    AddExistingObject(tag, FindFolderByNodeId(root.NodeId));
                }
            }
            else if (obj is ADFolder)
            {
                var folder = obj as ADFolder;
                if (parent is ADFolder)
                {
                    var root = parent as ADFolder;
                    AddExistingObject(folder, FindFolderByNodeId(root.NodeId));
                }

            }
        }

        void AddExistingObject(ADNotification tag, ADFolder root)
        {
            if (root != null)
                root.ADNotifications.Add(tag);
        }

        void AddExistingObject(ADFolder folder, ADFolder root)
        {
            if (root != null)
                root.ADFolders.Add(folder);
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

        public static ADEditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
            bool bCreateNew = true, bool bCheckEmpty = true)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                    return null;

                FileSystemProviderBase vfs = parent.fileSystemProviderBase;
                String connString = GetConnectionString(path, vfs);
                String xmlfile = GetBaseFilename(path, vfs);

                if (!bCreateNew && !String.IsNullOrEmpty(xmlfile) && !File.Exists(xmlfile))
                    return null;

                /*
                String userconnString = null;
                string userxmlfile = String.Format("{0}/{1}/{2}{3}", path,
                                        Properties.Settings.Default.TypeLabelUser,
                                        Properties.Settings.Default.DefaultProjectNameUser,
                                        Properties.Settings.Default.DefaultFileExtUser);
                if (!bCreateNew && !File.Exists(userxmlfile))
                    return null;

                userconnString = InMemoryDataStore.GetConnectionString(userxmlfile);

                String sconn = null;
                if (path != null && path.Length > 0)
                {
                    string s = String.Format("{0}/{1}/{2}{3}", path,
                                         Properties.Settings.Default.TypeLabelString,
                                         Properties.Settings.Default.DefaultProjectNameString,
                                         Properties.Settings.Default.DefaultFileExtString);
                    sconn = InMemoryDataStore.GetConnectionString(s);
                }
                */
                var doc = new ADEditorDocument(null)
                {
                    connectionString = connString,
                    EditorManagerComponent = c as ADEditorManagerComponent,
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

                var title = Path.GetFileNameWithoutExtension(path);
                if (vfs != null)
                    title = XpoHelpers.XpoHelper.GetDataSourceTitle(connString, true);
                doc.EnsureDefaultSettings(title);
                
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

        public bool CanClose()
        {
            if (ServerCMSHelperSync.IsServerStartedManually)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.StopServerStartedManually,
                        ADServerInfo.ADServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                    else if (res == CustomDialogResults.Yes)
                        ServerCMSHelperSync.StopServer();
                }
            }

            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADModel.ADGeneralSettings GetConfiguration()
        {
            
            var list = (from tag in new XPQuery<ADModel.ADGeneralSettings>(uow, true).AsParallel() select tag).ToList();
            if (list.Count == 0)
                return new ADModel.ADGeneralSettings(uow);
            return list[0];
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return true;

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
                    return true;

                var isProtected = forceEncryption || Protected;
                if (!String.IsNullOrEmpty(fileBase))
                {
                    if (isProtected)
                        XpoHelpers.XpoHelper.AddProtectionCode(uow, Id);
                    else
                        XpoHelpers.XpoHelper.RemoveProtectionCode(uow);
                }

                uow.CommitChanges();

                if (!String.IsNullOrEmpty(fileBase))
                {
                    bool alreadyProtected = false;

                    if (File.Exists(fileBase))
                    {
                        alreadyProtected = !Utilities.IO.FileSystem.IsXmlFile(fileBase);
                        try
                        {
                            File.Delete(fileBase);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    /*
                     * If Protected changes, save also the plugins settings file.
                     */
                    if (isProtected != alreadyProtected)
                    {
                        var pList = GetPluginCollection();
                        foreach (var pl in pList)
                        {
                            var uidll = ADServerInfo.ADServerInfo.GetPluginUIName(string.Format("{0}{1}", ADServerInfo.ADServerInfo.GetServerFolder(), pl.AssemblyName));

                            IPluginWpfEditing pluginWpfEditing = null;
                            try
                            {
                                var types = Assembly.LoadFile(uidll).GetTypes();
                                var list = (from t in types.AsParallel()
                                            where !t.IsAbstract && typeof(IPluginWpfEditing).IsAssignableFrom(t)
                                            select (IPluginWpfEditing)Activator.CreateInstance(t)).ToList();

                                pluginWpfEditing = list[0];

                                if (pluginWpfEditing != null && pluginWpfEditing.GeneralSettingsEditor != null)
                                {
                                    var control = pluginWpfEditing.GeneralSettingsEditor;
                                    List<string> lista = new List<string>() { ConnectionString, isProtected.ToString() };
                                    control.DataContext = lista;

                                    pluginWpfEditing.SaveSettings(control);
                                    if (control is IDisposable)
                                        (control as IDisposable).Dispose();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (EditorManagerComponent.UIInterface != null)
                                    EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.PluginSaveFailed, uidll));
                            }
                        }
                    }

                    if (isProtected)
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

        public void ServiceManager()
        {
            if (editorManagerComponent.UserManager != null)
                editorManagerComponent.UserManager.EnsureCredentialProvider(this);
            AddHttpAccessRules(true);

            string serverConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                serverConn = InMemoryDataStore.GetConnectionString(fileBase);
                if (editorManagerComponent.StringEditor != null)
                    stringConn = editorManagerComponent.StringEditor.GetConnectionStringFromFile(rootBase);
                if (editorManagerComponent.UserManager != null)
                    userConn = editorManagerComponent.UserManager.GetConnectionStringFromFile(rootBase);
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
                ADServerCMS.ADServerCSMHelpers.OpenServiceManager(applicationName, serverConn, stringConn, userConn, docPath);
        }

        internal ADGeneralSettings GetGeneralSettings()
        {
            var list = (from tag in new XPQuery<ADGeneralSettings>(uow, true).AsParallel() select tag).ToList();
            if (list.Count == 0)
                return new ADGeneralSettings(uow);
            return list[0];
        }

        internal String GetAplicationName()
        {
            {
                var ufuaConfiguration = (from tag in new XPQuery<ADGeneralSettings>(uow, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new ADGeneralSettings(uow);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                var ret = ufuaConfiguration.ApplicationName;

                return ret;
            }
        }

        internal String GetServiceName()
        {
            var serverName = ADServerInfo.ADServerInfo.GetServerName();
            if (uow == null)
                return serverName;

            return String.Format("{0} ({1})", serverName, GetGeneralSettings().ApplicationName);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADNotification GetNotification(string name, ADFolder folder = null)
        {
            List<ADNotification> list;
            if (folder == null)
                list = (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                        where tag.ADFolder == null && tag.Name == name
                        select tag).ToList();
            else
                list = (from tag in folder.ADNotifications.AsParallel()
                        where tag.Name == name select tag).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        internal IList<ADNotification> GetNotifications()
        {
            return (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                    orderby tag.Oid
                    select tag).ToList();
        }

        internal IList<ADFolder> GetFolderCollection(ADFolder root = null)
        {
            if (root == null)
            {
                return (from c in new XPQuery<ADModel.ADFolder>(uow, true).AsParallel()
                 where c.ADFolderAss == null select c).ToList();
            }

            return (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                    where folder.ADFolderAss == root &&
                    root != null 
                    orderby folder.Oid
                    select folder).ToList();
        }

        internal IList<ADNotification> GetNotificationsCollection(ADFolder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                        where tag.ADFolder == null
                        orderby tag.Oid 
                        select tag).ToList();
            }

            var folders = (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                           //where folder.Oid == root.Oid
                           where folder.NodeId == root.NodeId
                           select folder).ToList();
            if (folders.Count == 0)
                return new List<ADNotification>();
            else
                return folders[0].ADNotifications;
        }

        IList<String> GetNotificationsNodeIdList()
        {
            return (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                    select tag.NodeId.ToString()).ToList();
        }

        public IList<String> GetNotificationsNameList(ADFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                        where tag.ADFolder == null
                        select tag.Name).ToList();
            }

            return (from c in root.ADNotifications.AsParallel() select c.Name).ToList();
        }

        IList<String> GetFoldersNodeIdList()
        {
            return (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                    select folder.NodeId.ToString()).ToList();
        }

        public IList<String> GetFoldersNameList(ADFolder root)
        {
            if (root == null)
            {
                return (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                        where folder.ADFolderAss == null
                        select folder.Name).ToList();
            }

            return (from c in root.ADFolders.AsParallel() select c.Name).ToList();
        }

        internal ADFolder FindFolderByNodeId(Guid nodeId)
        {
            var list = (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                        where folder.NodeId == nodeId
                        select folder).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal ADNotification FindNotificationByNodeId(Guid nodeId)
        {
            var list = (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                        where tag.NodeId == nodeId
                        select tag).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal ADPlugin FindPluginByNodeId(Guid nodeId)
        {
            var list = (from plugin in new XPQuery<ADPlugin>(uow, true).AsParallel()
                        where plugin.NodeId == nodeId
                        select plugin).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal IList<ADPlugin> GetPluginCollection()
        {
            return (from folder in new XPQuery<ADPlugin>(uow, true).AsParallel()
                    //where folder.UFUAFolderAss == root &&
                    //(root != null || folder.UFUATagPrototype == null)
                    orderby folder.Oid
                    select folder).ToList();
        }

        IList<String> GetPluginsNameList()
        {
            return (from hs in new XPQuery<ADModel.ADPlugin>(uow, true).AsParallel()
                    select hs.Name).ToList();
        }

        internal ADPlugin AddNewPlugin(ADGeneralSettings root)
        {
            var folder = new ADPlugin(uow) { Name = Properties.Settings.Default.DefaultPluginName, NodeId = Guid.NewGuid() };
            if (root != null)
                root.ADPlugins.Add(folder);
            return folder;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADPlugin AddNewPlugin()
        {
            var driver = new ADPlugin(uow);
            return driver;
        }

        internal string GetServerIOConfigurationId()
        {
            if (ADEditorManagerComponent.adeditorManagerComponent.UfuaEditorService != null)
                return ADEditorManagerComponent.adeditorManagerComponent.UfuaEditorService.GetServerConfigurationId(this);
            return string.Empty;
        }

        public bool StartServer(bool bSave = true, bool manually = false)
        {
            return StartServer(null, null, bSave, manually);
        }

        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool bSave = true, bool manually = false)
        {

            string serverConfigurationId = GetServerIOConfigurationId();

            if (bSave && NeedsSave)
            {
                if (EditorManagerComponent.UIInterface != null)
                {
                    var res = EditorManagerComponent.UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                        ADServerInfo.ADServerInfo.GetServerName()), CustomDialogIcons.Question);
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
                if (editorManagerComponent.UserManager != null)
                    userConn = editorManagerComponent.UserManager.GetConnectionStringFromFile(rootBase);
            }
            else
                serverConn = userConn = stringConn = ConnectionString;

            bool suspended = false;
#if !DEBUG
            if(MSZ.MSZView.IsSuspended())
                suspended = true;
#endif

            var serverCMSHelper = manually ? ServerCMSHelperAsync : ServerCMSHelperSync;
            return serverCMSHelper.StartServer(textBlock, scroll, suspended, manually, serverConn, stringConn, userConn, serverConfigurationId);
        }

        public ADNotification AddNewNotification(ADModel.ADFolder root)
        {
            var notif = new ADNotification(uow)
            {
                Name = NewNotificationName(root),
                NodeId = Guid.NewGuid(),
                Recipient = String.Empty,
                ADFolder = root
            };
            if (root != null)
                root.ADNotifications.Add(notif);
            return notif;
        }

        public String NewNotificationName(ADFolder root = null, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultTagName;

            if (listname == null)
                listname = GetNotificationsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool NotificationNameExists(String name, ADFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<ADNotification>(uow, true).AsParallel()
                        where tag.Name == name //&& tag.ADFolder == null
                        select tag).ToList().Count > 0;
            }

            return (from c in root.ADNotifications.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }

        public ADFolder AddNewFolder(ADFolder root)
        {
            var folder = new ADFolder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid() };
            if (root != null)
                root.ADFolders.Add(folder);
            return folder;
        }

        public String NewFolderName(ADFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool FolderNameExists(String name, ADFolder root)
        {
            return (from folder in new XPQuery<ADFolder>(uow, true).AsParallel()
                    where folder.ADFolderAss == root && folder.Name == name
                    select folder).ToList().Count > 0;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADFolder GetFolder(string name, ADFolder folder = null)
        {
            List<ADFolder> list;
            if (folder == null)
            {
                list = (from f in new XPQuery<ADFolder>(uow, true).AsParallel()
                        where f.ADFolderAss == null && f.Name == name select f).ToList();
            }
            else
            list = (from c in folder.ADFolders.AsParallel() where c.Name == name select c).ToList();

            if (list.Count > 0)
                return list[0];
            return null;
        }
        internal IList<UFUserModel.UFRole> GetRoles()
        {
            var ret = new List<UFUserModel.UFRole>();
            if (EditorManagerComponent != null && EditorManagerComponent.UserManager != null)
            {
                var list = EditorManagerComponent.UserManager.GetRoles(this);
                if (list != null)
                {
                    foreach (var element in list)
                    {
                        if (element is UFUserModel.UFRole)
                            ret.Add(element as UFUserModel.UFRole);
                    }
                }
            }

            return ret;
        }
        #endregion

        #region Properties

        [Browsable(false)]
        public bool NeedsSave
        {
            get { return uow != null && uow.TrackingChanges; }
        }

        [Browsable(false)]
        public bool IsDisposed
        {
            get { return bObjectDisposed; }
        }

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

        ADEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public ADEditorManagerComponent EditorManagerComponent
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
        Guid ConfigurationId
        {
            get
            {
                if (configurationId == Guid.Empty)
                {
                    var d = GetDataLayer();
                    {
                        using (var uow = new UnitOfWork(d))
                        {
                            var list = (from tag in new XPQuery<ADModel.ADGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                            if (list.Count == 0)
                                return Guid.Empty;

                            configurationId = list[0].ConfigurationId;
                        }
                    }
                }

                return configurationId;
            }
            set
            {
                if (configurationId == value)
                    return;

                var list = (from tag in new XPQuery<ADModel.ADGeneralSettings>(uow, true).AsParallel() select tag).ToList();
                if (list.Count > 0)
                {
                    list[0].ConfigurationId = value;
                    configurationId = value;
                }
            }
        }

        ADServerCSMHelpers serverCMSHelperSync;
        [Browsable(false)]
        internal ADServerCSMHelpers ServerCMSHelperSync
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
                    serverCMSHelperSync = new ADServerCSMHelpers(ConfigurationId.ToString());

                return serverCMSHelperSync;
            }
        }

        ADServerCSMHelpers serverCMSHelperAsync;
        [Browsable(false)]
        internal ADServerCSMHelpers ServerCMSHelperAsync
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
                    serverCMSHelperAsync = new ADServerCSMHelpers(ConfigurationId.ToString());
                    serverCMSHelperAsync.StartServerStatusInBackground();
                }

                return serverCMSHelperAsync;
            }
        }

        #endregion

        #region ICloneable Members

        ADEditorDocument(ADEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new ADEditorDocument(this);
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
                return GetNotifications().Count == 0;
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

            //EditorManagerComponent.Workspace.ContextObject = null;

            if (uowContext != null)
            {
                uowContext.Dispose();
                uowContext = null;
            }

            if (uow != null)
            {
                uow.Disconnect();
                uow.Dispose();
                uow = null;
            }
            //if (uowCloner != null)
            //{
            //    uowCloner.Disconnect();
            //    uowCloner.Dispose();
            //    uowCloner = null;
            //}
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
        }

        #endregion

        #region Plugins

        internal void CopyPluginsToClipbaord(List<ADModel.ADPlugin> list)
        {
            list.ForEach(plugin =>
            {
                CloneToClipboard(plugin, false);
            });
            uowClipboard.CommitChanges();
        }

        internal bool ClipboardContainsPlugins()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<ADModel.ADPlugin>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal List<ADModel.ADPlugin> PasteClipboardPlugins()
        {
            var ret = new List<ADModel.ADPlugin>();
            var listName = GetPluginsNameList();
            var listToCopy = (from c in new XPQuery<ADModel.ADPlugin>(uowClipboard).AsParallel() orderby c.Oid select c).ToList();
            listToCopy.ForEach(plugin =>
            {
                if (!listName.Contains(plugin.Name))
                {
                    var newplugin = CloneFromClipboard(plugin, false) as ADModel.ADPlugin;
                    ret.Add(newplugin);
                }
            });
            return ret;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsPluginNameUsed(String name)
        {
            return (from tag in new XPQuery<ADModel.ADNotification>(uow, true).AsParallel()
                    where tag.PluginName != null && tag.PluginName == name
                    select tag).ToList().Count > 0;
        }

        internal static void EditPluginSettings(ADModel.ADPlugin plugin, ADEditorDocument doc)
        {
            var uidll = ADServerInfo.ADServerInfo.GetPluginUIName(string.Format("{0}{1}", ADServerInfo.ADServerInfo.GetServerFolder(), plugin.AssemblyName));

            IPluginWpfEditing pluginWpfEditing = null;
            try
            {
                var types = Assembly.LoadFile(uidll).GetTypes();
                var list = (from t in types.AsParallel()
                            where !t.IsAbstract && typeof(IPluginWpfEditing).IsAssignableFrom(t)
                            select (IPluginWpfEditing)Activator.CreateInstance(t)).ToList();

                pluginWpfEditing = list[0];
            }
            catch (Exception ex)
            {
                if (doc.EditorManagerComponent.UIInterface != null)
                    doc.EditorManagerComponent.UIInterface.ShowInformation(String.Format(Properties.Resources.PluginNotFound, uidll));
            }

            if (pluginWpfEditing == null || pluginWpfEditing.GeneralSettingsEditor == null)
                return;

            var control = pluginWpfEditing.GeneralSettingsEditor;
            List<string> lista = new List<string>() { doc.ConnectionString, doc.Protected.ToString() };
            control.DataContext = lista;

            GeneralDialogContent Dialog = new GeneralDialogContent(control)
            {
                Title = plugin.Name,
                Owner = Application.Current.Windows.Count > 0 ? Application.Current.Windows[0] : Application.Current.MainWindow,
                HelpLink = "PluginEditor",
                DialogKeepContent = true
            };
            if (Dialog.ShowDialog() == true)
            {
                pluginWpfEditing.SaveSettings(control);
            }

            if (control is IDisposable)
                (control as IDisposable).Dispose();
        }

        #endregion

        #region BaseAddresses

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADBaseAddress AddNewBaseAddress(string transport)
        {
            var ba = new ADBaseAddress(uow)
            {
                Enabled = true,
                Transport = transport,
                Server = "localhost",
                Port = GetConfiguration().GetDefaultPort(transport)
            };

            GetConfiguration().BaseAddresses.Add(ba);

            return ba;
        }

        private bool AddHttpAccessRules(bool silent)
        {
            var urls = (from address in new XPQuery<ADBaseAddress>(uow, true).AsParallel()
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

        internal int GetCurrentBaseAddressPort(string transport)
        {
            return GetBaseAddress(transport).Port.Value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ADBaseAddress GetBaseAddress(string transport)
        {
            return (from address in new XPQuery<ADBaseAddress>(uow, true)
                    where address.Transport == transport
                    select address).SingleOrDefault();
        }

        #endregion

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
                        (from p in new XPQuery<ADModel.ADFolder>(uowTarget, true).AsParallel()
                         where p.ADFolderAss == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<ADModel.ADNotification>(uowTarget, true).AsParallel()
                         where p.ADFolder == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<ADModel.ADGeneralSettings>(uowTarget, true).AsParallel()
                         select p).ToList().ForEach(tag => tag.Delete());

                        // following is only for ensuring the complitely deletion of unassociated object elements
                        (from p in new XPQuery<ADModel.ADPlugin>(uowTarget, true).AsParallel()
                         where p.ADGeneralSettings == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////
                        var folders = sourceDoc.GetFolderCollection();
                        foreach (var folder in folders)
                        {
                            cloneHelper.Clone(folder, false);
                        }
                        /*var plugins = sourceDoc.GetPluginCollection();
                        foreach (var plugin in plugins)
                        {
                            cloneHelper.Clone(plugin, false);
                        }*/
                        var notifications = sourceDoc.GetNotificationsCollection();
                        foreach (var notification in notifications)
                        {
                            cloneHelper.Clone(notification, false);
                        }
                        cloneHelper.Clone(sourceDoc.GetGeneralSettings(), false);

                        uowTarget.CommitChanges();
                        uowTarget.PurgeDeletedObjects();
                    }
                    var pluginlist = sourceDoc.GetGeneralSettings().ADPlugins;
                    if (pluginlist.Count > 0)
                    {
                        foreach (var plugin in pluginlist)
                        {
                            var uidll = ADServerInfo.ADServerInfo.GetPluginUIName(string.Format("{0}{1}", ADServerInfo.ADServerInfo.GetServerFolder(), plugin.AssemblyName));
                            var types = Assembly.LoadFile(uidll).GetTypes();
                            var list = (from t in types.AsParallel()
                                        where !t.IsAbstract && typeof(IPluginWpfEditing).IsAssignableFrom(t)
                                        select (IPluginWpfEditing)Activator.CreateInstance(t)).ToList();
                            list[0].CopyFile(sourceDoc.ConnectionString, targetConn);
                        }
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
                    var plugins = sourceDoc.GetPluginCollection();
                    foreach (var plugin in plugins)
                    {
                        plugin.Delete();
                    }
                    var notifications = sourceDoc.GetNotificationsCollection();
                    foreach (var notification in notifications)
                    {
                        notification.Delete();
                    }
                    sourceDoc.GetGeneralSettings().Delete();
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
    }
}
