using System;
using System.Collections.Generic;
using System.Linq;
using DocumentManager.ComponentService;
#if !NET_STANDARD
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
using Utilities.WPF;
using WPFUtilities.Extensions;
#endif
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using UFEventModel;
using System.Diagnostics;
using System.Windows;
using System.Xml;
using System.Text.RegularExpressions;
using UFEventEditor.ComponentService;
using UFUAEditor.ComponentService;
using Utilities;
using OPCUAViewModel;
using log4net;
using UFProjectManager;
using UFInterfaces.PropertyControl;
using DocumentManager.ComponentService.Helpers;

namespace UFEventEditor.Document
{
    public class EventEditorDocument : ViewModelBase, ICloneable,
#if !NET_STANDARD
        INotifyPropertyVisibilityChanged,
#endif
        IDocument
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

#if !NET_STANDARD
        readonly Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper> UndoRedoHelper = new Dictionary<UserControl, XpoHelpers.UndoRedoIXPSimpleObjectHelper>();
        readonly Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer> mapdlUndoRedo = new Dictionary<XpoHelpers.UndoRedoIXPSimpleObjectHelper, IDataLayer>();
        readonly Dictionary<IDataLayer, UnitOfWork> mapuowUndoRedo = new Dictionary<IDataLayer, UnitOfWork>();

        internal static readonly ILog log = LogManager.GetLogger(Properties.Resources.EventManager);
#else
        internal static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.EventManager);
#endif
        private const string defaultessionname = "EventsManager";

        #endregion

        #region Methods
#if !NET_STANDARD
        internal void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (EventEditorManagerComponent.eventeditorManagerComponent.UfuaEditorService != null)
            {
                string defaultlocalendpoint = EventEditorManagerComponent.eventeditorManagerComponent.UfuaEditorService.GetDefaultLocalEndpoint(this, true);
                var endpointslist = EventEditorManagerComponent.eventeditorManagerComponent.UfuaEditorService.GetEndpoints(this);
                string aplicationName = EventEditorManagerComponent.eventeditorManagerComponent.UfuaEditorService.GetAplicationName(this, true);

                using (var cursor = new WaitCursor())
                {
                    List<UFEventModel.UFEventObject> eventlist = GetCRFlatTagCollection();
                    var list = new List<OPCUAViewModel.OPCUAEntityReference>();
                    bool bDirty = false;
                    eventlist.ForEach(item =>
                    {
                        if (model.QuitEvent.IsCancellationRequested)
                            return;

                        list.AddRange(GetOPCReferenceList(item, model));

                        if (model.RenamedMap.Count > 0)
                        {
                            var commandList = new CommandManager.CommandManagerList(item.CommandList as CommandManager.CommandManagerList);
                            foreach (var command in commandList)
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;

                                CommandManager.OpenScreenCommand scommand = command as CommandManager.OpenScreenCommand;
                                var path = scommand?.ScreenName.GetPathString();
                                if (scommand != null && !string.IsNullOrEmpty(path))
                                {
                                    if (model.RenamedMap.ContainsKey(path))
                                    {
                                        scommand.ScreenName = new Uri(model.RenamedMap[path], UriKind.RelativeOrAbsolute);
                                        bDirty = true;
                                    }
                                }
                            }
                            if (bDirty)
                                item.CommandList = commandList;
                        }
                    });
                    var nodelist = (from c in list
                                    where c.ResolvedNodeId != null
                                    select c.ResolvedNodeId.ToString()).Distinct().ToList();
                    var mapNodes = EventEditorManagerComponent.eventeditorManagerComponent.UfuaEditorService.GetListNodeNames(this, nodelist);
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
                        found.ForEach(fitem =>
                        {
                            if (model.QuitEvent.IsCancellationRequested)
                                return;

                            var preChanged = fitem.ToXml();
                            var postChanged = preChanged;
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
                                postChanged = fitem.ToXml();
                                bDirty = true;
                            }

                            eventlist.ForEach(item =>
                            {
                                if (model.QuitEvent.IsCancellationRequested)
                                    return;

                                bool bTag = false;
                                bool bETag = false;
                                bool bVTag = false;
                                if (!string.IsNullOrEmpty(item.Tag) ){ bTag = item.Tag.Contains(fitem.ResolvedNodeId.ToString()); }
                                if (!string.IsNullOrEmpty(item.EnableTag)) { bETag = item.EnableTag.Contains(fitem.ResolvedNodeId.ToString()); };
                                if (!string.IsNullOrEmpty(item.ValueTag)) { bVTag = item.ValueTag.Contains(fitem.ResolvedNodeId.ToString()); };

                                if (bTag)
                                {
                                    bDirty = true;
                                    item.Tag = postChanged;
                                }
                                if (bETag)
                                {
                                    bDirty = true;
                                    item.EnableTag = postChanged;
                                }
                                if (bVTag)
                                {
                                    bDirty = true;
                                    item.ValueTag = postChanged;
                                }

                                var listCommands = item.CommandList as CommandManager.CommandManagerList;
                                listCommands.ForEach(command =>
                                {
                                    command.UpdateTags(preChanged.FromXml<OPCUAEntityReference>(), postChanged.FromXml<OPCUAEntityReference>());
                                });

                                item.EventCommandList = listCommands.ToXml();
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

        private List<OPCUAEntityReference> GetOPCReferenceList(UFEventObject eventitem, UFInterfaces.Editors.CrossReferenceModel model)
        {
            var list = new List<OPCUAViewModel.OPCUAEntityReference>();
            if (!string.IsNullOrEmpty(eventitem.Tag))
            {
                var tag = eventitem.Tag.FromXml<OPCUAEntityReference>();
                list.Add(tag);
            }
            if (!string.IsNullOrEmpty(eventitem.EnableTag))
            {
                var tag = eventitem.EnableTag.FromXml<OPCUAEntityReference>();
                list.Add(tag);
            }
            if (!string.IsNullOrEmpty(eventitem.ValueTag))
            {
                var tag = eventitem.ValueTag.FromXml<OPCUAEntityReference>();
                list.Add(tag);
            }

            var cRMapsHeler = new CRMapsHelper() { CrossReferenceTypes = model.CRManagement.CrossReferenceTypeList };
            eventitem.GetAllSourceEntityReferencesDetails(cRMapsHeler, true);
            if (cRMapsHeler.Tags != null)
            {
                cRMapsHeler.Tags.ToList().ForEach(tag =>
                {
                    IDictionary<String, OPCUAViewModel.OPCUAEntityReference> refdetails = tag as IDictionary<String, OPCUAViewModel.OPCUAEntityReference>;
                    if (refdetails.Count > 0 && refdetails.First().Value != null)
                        list.Add(refdetails.First().Value);
                });
            }

            return list;
        }
        internal List<UFEventObject> GetCRFlatTagCollection()
        {
            return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                    orderby tag.Oid
                    select tag).ToList();
        }
#endif
        internal void UpdateSessionSettings()
        {
            if (String.IsNullOrEmpty(SessionName))
                return;

            var prgMan = Parent as UFProjectDocument;
            if (prgMan == null || String.IsNullOrEmpty(prgMan.Title))
                return;

            String[] serverUriArray = null;
#if !WINDOWS_UWP
            var ufuaEditor = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (ufuaEditor != null)
                serverUriArray = ufuaEditor.GetServerUriArray(this);
#endif
            RealTimeConnectionManagerViewModel.AddSessionSettings(SessionString, new SessionSettings()
            {
                ParentTitle = prgMan.Title,
                RemoveDisabledItemAfterSecs = prgMan.RemoveDisabledItemAfterSecs,
                MaxCleanCount = prgMan.MaxCleanCount,
                UseAlwaysSecureConnections = prgMan.UseAlwaysSecureConnections,
                SlowSamplingInterval = prgMan.SlowSamplingInterval,
                DisableWhenNotUsed = prgMan.DisableWhenNotUsed,
                PublishingInterval = prgMan.PublishingInterval,
                ServerArray = serverUriArray,
                FastSamplingInterval = prgMan.FastSamplingInterval
            });
        }

        public void LogMessage(string msg, EventLogEntryType EntryType)
        {
            switch (EntryType)
            {
                case EventLogEntryType.Error:
                    log.Error(msg); break;
                case EventLogEntryType.FailureAudit:
                    log.Warn(msg); break;
                case EventLogEntryType.Information:
                    log.Info(msg); break;
                case EventLogEntryType.Warning:
                    log.Warn(msg); break;
                case EventLogEntryType.SuccessAudit:
                    log.Info(msg); break;
            }
        }

        void CreateDataLayer()
        {
            if (String.IsNullOrEmpty(fileBase))
            {
                dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            }
            else
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
                dl = new SimpleDataLayer(InMemory);
            }

            uow = new UnitOfWork(dl);
            uow.ObjectChanged += (o, e) =>
            {
                OnPropertyChanged("NeedsSave");
            };

            InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
            dlClipboard = new SimpleDataLayer(InMemoryClipboard);
            uowClipboard = new UnitOfWork(dlClipboard);
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
#endif

        void EnsureDefaultSettings()
        {
        }

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
#endif
        #endregion

        #region Clipboard
#if !NET_STANDARD
        internal void CopyListFoldersToClipbaord(List<UFEventFolder> list)
        {
            list.ForEach(folder =>
            {
                CloneToClipboard(folder, false);
            });
            uowClipboard.CommitChanges();
        }

        internal void CopyListEventsToClipbaord(List<UFEventObject> list)
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
                return (from c in new XPQuery<UFEventFolder>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal bool ClipboardContainsEvents()
        {
            if (uowClipboard == null)
                return false;

            try
            {
                return (from c in new XPQuery<UFEventObject>(uowClipboard).AsParallel() select c).ToList().Count > 0;
            }
            catch
            {
                return false;
            }
        }

        internal List<UFEventFolder> PasteClipboardFolders(UFEventFolder folder, UFEventFolder parent = null)
        {
            var ret = new List<UFEventFolder>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetFoldersNodeIdList();
            var listName = GetFoldersNameList(folder);
            var listToCopy = (from c in new XPQuery<UFEventFolder>(uowClipboard).AsParallel() where c.UFEventFolderAss == parent orderby c.Oid select c).ToList();
            listToCopy.ForEach(dir =>
            {
                var exist = listNodeId.Contains(dir.NodeId.ToString());
                var newName = NewFolderName(folder, dir.Name, mapStartCounter, listName);
                var newfolder = CloneFromClipboard(dir, exist) as UFEventFolder;
                newfolder.Name = newName;
                if (folder != null)
                    folder.UFEventFolders.Add(newfolder);
                ret.Add(newfolder);
            });
            return ret;
        }

        internal List<UFEventObject> PasteClipboardEvents(UFEventFolder folder, UFEventFolder parent = null)
        {
            var ret = new List<UFEventObject>();
            var mapStartCounter = new Dictionary<string, ulong>();
            var listNodeId = GetEventsNodeIdList();
            var listName = GetEventsNameList(folder);
            var listToCopy = (from c in new XPQuery<UFEventObject>(uowClipboard).AsParallel() where c.UFEventFolder == parent orderby c.Oid select c).ToList();
            listToCopy.ForEach(tag =>
                {
                    var exist = listNodeId.Contains(tag.NodeId.ToString());
                    var newName = NewEventName(folder, tag.Name, mapStartCounter, listName);
                    var newtag = CloneFromClipboard(tag, exist) as UFEventObject;
                    newtag.Name = newName;
                    if(folder != null)
                        newtag.UFEventFolder = folder;
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
#endif
        #endregion
#if !NET_STANDARD
        internal NestedUnitOfWork BeginNestedUnitOfWork()
        {
            return uow.BeginNestedUnitOfWork();
        }

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

            if (obj is UFEventObject)
            {
                var tag = obj as UFEventObject;
                if (parent is UFEventFolder)
                {
                    var root = parent as UFEventFolder;
                    AddExistingObject(tag, FindFolderByNodeId(root.NodeId));
                }
            }
            else if (obj is UFEventFolder)
            {
                var folder = obj as UFEventFolder;
                if (parent is UFEventFolder)
                {
                    var root = parent as UFEventFolder;
                    AddExistingObject(folder, FindFolderByNodeId(root.NodeId));
                }

            }
        }

        void AddExistingObject(UFEventObject tag, UFEventFolder root)
        {
            if (root != null)
                root.UFEventObjects.Add(tag);
        }

        void AddExistingObject(UFEventFolder folder, UFEventFolder root)
        {
            if (root != null)
                root.UFEventFolders.Add(folder);
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

        public static EventEditorDocument FromFile(String path, IDocumentManager c, IDocument parent, 
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

                var doc = new EventEditorDocument(null)
                {
                    connectionString = connString,
#if !NET_STANDARD
                    EditorManagerComponent = c as EventEditorManagerComponent,
#endif
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
#else
                    log.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                    return null;
                }

                doc.EnsureDefaultSettings();

                return doc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                log.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                return null;
            }
        }

#if !NET_STANDARD
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
                    uow.TryPurgeDeletedObjects(log) > 0)
                    bSave = true;

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

        internal IList<EventObject> GetExecutionEvents()
        {
            List<EventObject> run = new List<EventObject>();
            var list = (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        orderby tag.Oid
                        select tag).ToList();
            foreach (var ev in list)
            {
                var ne = new EventObject()
                {
                    Name = ev.Name,
                    ActivationValue = ev.ActivationValue.Value,
                    ActivationStringValue = ev.ActivationStringValue,
                    ConditionType = ev.ConditionType.Value,
                    CommandList = ev.CommandList,
                    Enable = ev.Enable.Value,
                    NodeId = ev.NodeId,
                    Type = ev.Type.Value, 
                    SchedType=ev.SchedType, Date=ev.Date, 
                    Time=ev.Time,
                    EnableExpression = ev.EnableExpression,
                    ValueExpression = ev.ValueExpression,
                    Expression = ev.Expression
                };
                try
                {
                    OPCUAEntityReference opc = null;
                    if (ev.Tag != null && ev.Tag.Length > 0)
                    {
                        opc = ev.Tag.FromXml<OPCUAEntityReference>();
                        ne.Tag = opc;
                    }
                    if (ev.EnableTag != null && ev.EnableTag.Length > 0)
                    {
                        opc = ev.EnableTag.FromXml<OPCUAEntityReference>();
                        ne.EnableTag = opc;
                    }
                    if (ev.ValueTag != null && ev.ValueTag.Length > 0)
                    {
                        opc = ev.ValueTag.FromXml<OPCUAEntityReference>();
                        ne.ValueTag = opc;
                    }
                }
                catch (Exception e)
                {
                    if (ne.Tag == null)
                        continue;
                }
                run.Add(ne);
            }

            return run;
        }
        internal IList<UFEventObject> GetEvents()
        {
            return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                    orderby tag.Oid
                    select tag).ToList();
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFEventObject GetEvent(string name, UFEventFolder folder = null)
        {
            List<UFEventObject> list;
            if (folder == null)
                list = (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        where tag.UFEventFolder == null && tag.Name == name
                        select tag).ToList();
            else
                list = (from tag in folder.UFEventObjects.AsParallel()
                        where tag.Name == name
                        select tag).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFEventFolder GetFolder(string name, UFEventFolder folder = null)
        {
            List<UFEventFolder> list;
            if (folder == null)
            {
                list = (from f in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                        where f.UFEventFolderAss == null && f.Name == name
                        select f).ToList();
            }
            else
                list = (from c in folder.UFEventFolders.AsParallel() where c.Name == name select c).ToList();

            if (list.Count > 0)
                return list[0];
            return null;
        }

        internal IList<UFEventFolder> GetFolderCollection(UFEventFolder root = null)
        {
            if (root == null)
            {
                return (from c in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                 where c.UFEventFolderAss == null select c).ToList();
            }

            return (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                    where folder.UFEventFolderAss == root &&
                    root != null 
                    orderby folder.Oid
                    select folder).ToList();
        }

        internal UFEventFolder FindFolderByNodeId(Guid nodeId)
        {
            var list = (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                        where folder.NodeId == nodeId
                        select folder).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }

        internal IList<UFEventObject> GetEventsCollection(UFEventFolder root = null)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        where tag.UFEventFolder == null
                        orderby tag.Oid
                        select tag).ToList();
            }

            var folders = (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                           where folder.NodeId == root.NodeId
                           select folder).ToList();
            if (folders.Count == 0)
                return new List<UFEventObject>();
            else
                return folders[0].UFEventObjects;
        }

        internal UFEventObject FindEventByNodeId(Guid nodeId)
        {
            var list = (from folder in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        where folder.NodeId == nodeId
                        select folder).ToList();

            if (list.Count > 0)
                return list[0];

            return null;
        }



        IList<String> GetEventsNodeIdList()
        {
            return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                    select tag.NodeId.ToString()).ToList();
        }

        public IList<String> GetEventsNameList(UFEventFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        where tag.UFEventFolder == null
                        select tag.Name).ToList();
            }

            return (from c in root.UFEventObjects.AsParallel() select c.Name).ToList();
        }

        IList<String> GetFoldersNodeIdList()
        {
            return (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                    select folder.NodeId.ToString()).ToList();
        }

        public IList<String> GetFoldersNameList(UFEventFolder root)
        {
            return (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                    where folder.UFEventFolderAss == root
                    select folder.Name).ToList();

            if (root == null)
            {
                return (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                        where folder.UFEventFolderAss == null
                        select folder.Name).ToList();
            }

            return (from c in root.UFEventFolders.AsParallel() select c.Name).ToList();
        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFEventObject AddNewEvent(UFEventFolder root)
        {
            var notif = new UFEventObject(uow)
            {
                Name = NewEventName(root),
                NodeId = Guid.NewGuid(),
                Date = DateTime.Now.Date,
                UFEventFolder = root
            };
            if (root != null)
                root.UFEventObjects.Add(notif);
            return notif;
        }

        public String NewEventName(UFEventFolder root = null, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultEventName;

            if (listname == null)
                listname = GetEventsNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool EventNameExists(String name, UFEventFolder root)
        {
            if (root == null)
            {
                return (from tag in new XPQuery<UFEventObject>(uow, true).AsParallel()
                        where tag.Name == name
                        select tag).ToList().Count > 0;
            }

            return (from c in root.UFEventObjects.AsParallel() where c.Name == name select c).ToList().Count > 0;
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UFEventFolder AddNewFolder(UFEventFolder root)
        {
            var folder = new UFEventFolder(uow) { Name = NewFolderName(root), NodeId = Guid.NewGuid() };
            if (root != null)
                root.UFEventFolders.Add(folder);
            return folder;
        }
        public String NewFolderName(UFEventFolder root, String name = null, IDictionary<string, ulong> mapcounter = null, IList<String> listname = null, String format = "{0}{1}")
        {
            if (String.IsNullOrEmpty(name))
                name = Properties.Settings.Default.DefaultFolderName;

            if (listname == null)
                listname = GetFoldersNameList(root);

            return Utilities.NewNameHelper.FindNewName(name, listname, mapcounter, format);
        }

        bool FolderNameExists(String name, UFEventFolder root)
        {
            return (from folder in new XPQuery<UFEventFolder>(uow, true).AsParallel()
                    where folder.UFEventFolderAss == root && folder.Name == name
                    select folder).ToList().Count > 0;
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

        EventEditorManagerComponent editorManagerComponent;
        [Browsable(false)]
        public EventEditorManagerComponent EditorManagerComponent
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

        public String ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        string sessionName;
        public string SessionName
        {
            get { return sessionName; }
            set
            {
                if (value == sessionName)
                    return;
                sessionName = value;
                UpdateSessionSettings();
                OnPropertyChanged("SessionName");
#if !NET_STANDARD
                OnPropertyVisiblityChanged("SessionName");
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
        #endregion

        #region ICloneable Members

        EventEditorDocument(EventEditorDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

        public object Clone()
        {
            return new EventEditorDocument(this);
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
                if (!String.IsNullOrEmpty(fileBase))
                    return Path.GetFileNameWithoutExtension(fileBase);
                if (Parent != null)
                    return Path.GetFileNameWithoutExtension(Parent.rootBase);
                return Properties.Resources.RootName;
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
                return GetEvents().Count == 0;
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

        public object GetService(Type type)
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

        #region IDisposable

        protected bool bObjectDisposed;
        protected override void OnDispose()
        {
            if (bObjectDisposed)
                return;
            bObjectDisposed = true;

            OnDisposing(this);

            base.OnDispose();
#if !NET_STANDARD
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
            if (dlClipboard != null)
            {
                dlClipboard.Dispose();
                dlClipboard = null;
            }
#if !NET_STANDARD
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

        #endregion
#if !NET_STANDARD
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
                        (from p in new XPQuery<UFEventFolder>(uowTarget, true).AsParallel()
                         where p.UFEventFolderAss == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        (from p in new XPQuery<UFEventObject>(uowTarget, true).AsParallel()
                         where p.UFEventFolder == null
                         select p).ToList().ForEach(tag => tag.Delete());
                        ////////////////////////////////////////////////////////////////////////////
                        var folders = sourceDoc.GetFolderCollection();
                        foreach (var folder in folders)
                        {
                            cloneHelper.Clone(folder, false);
                        }
                        
                        var notifications = sourceDoc.GetEventsCollection();
                        foreach (var notification in notifications)
                        {
                            cloneHelper.Clone(notification, false);
                        }

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
                { }
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
                    var notifications = sourceDoc.GetEventsCollection();
                    foreach (var notification in notifications)
                    {
                        notification.Delete();
                    }
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
#endif
    }
}
