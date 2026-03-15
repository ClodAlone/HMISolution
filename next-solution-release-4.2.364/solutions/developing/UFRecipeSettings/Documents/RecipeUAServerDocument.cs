using System;
using System.Collections.Generic;
using System.Linq;
#if !NET_STANDARD
using System.Windows.Media;
using System.Windows.Controls;
using VFS;
using UIMsgBoxAlertService.ComponentService;
#endif
using System.ComponentModel;
using ViewModelLib;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.Diagnostics;
using Utilities;
using System.IO;
using System.Windows;
using System.Xml;
using OPCUAViewModel;
using Opc.Ua;
using System.Reflection;
using log4net;
using System.Threading;
using XpoHelpers;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
using StringManager.ComponentService;
using UFUserEditor.ComponentService;
using UFUAEditor.ComponentService;
using RecipeServiceCMS;
#endif
using DocumentManager.ComponentService;
using UFProjectManager.ComponentService;
using UFRecipeEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;
using UFInterfaces;

namespace UFRecipeSettings.Documents
{

    public class RecipeUAServerDocument : ViewModelBase,
#if !NET_STANDARD
        ICloneable, 
#endif
        IDocument, IEntityReference, IXpoDocument
    {
        #region Declarations

        UnitOfWork uow;
        CachedUnitOfWork cachedUow;
        InMemoryDataStore InMemory;
        IDataLayer dl;
#if !NET_STANDARD
        //UnitOfWork uowCloner;
        UnitOfWork uowClipboard;
        InMemoryDataStore InMemoryClipboard;
        IDataLayer dlClipboard;
#endif
        String defaultApplicationName;
        String connectionString;
        String fileBase;

        OPCUAEntityReference serverSubscriber;

#if !NET_STANDARD
        UndoRedoXpoManager undoRedoHelper;
        IDataLayer dlUndoRedo;
        UnitOfWork uowUndoRedo;
#endif

        readonly CachedUnitOfWorks cachedUnitOfWorks;
        
        readonly Dictionary<Thread, String> cachedDefaultLocalEndpoint = new Dictionary<Thread, String>();
        readonly Dictionary<Thread, String> cachedDefaultAppName = new Dictionary<Thread, String>();

#if !NET_STANDARD
        internal static readonly ILog logGeneral = LogManager.GetLogger(Utilities.Properties.Resources.GeneralLogName);
#else
        internal static readonly ILog logGeneral = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Utilities.Properties.Resources.GeneralLogName);
#endif

        readonly static string[] transportOrderByRelevance = new string[]
        {
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeNetPipe,
            Opc.Ua.Utils.UriSchemeNetTcp,
#endif
            Opc.Ua.Utils.UriSchemeOpcTcp,
            Opc.Ua.Utils.UriSchemeHttps,
#if !NET_STANDARD
            Opc.Ua.Utils.UriSchemeHttp,
            Opc.Ua.Utils.UriSchemeNoSecurityHttp
#endif
		};

#if !NET_STANDARD
        object tagsListLock = new object();
#endif
        #endregion

        #region Constructors
        public RecipeUAServerDocument()
        {
            cachedUnitOfWorks = new CachedUnitOfWorks(this);
        }
        #endregion

        #region Methods

        public void PreSubscribeServerSession(String sessionName)
        {
            if (serverSubscriber == null)
            {
                serverSubscriber = GetRecipeUAServerEntityReference();
                serverSubscriber.SetInUse(this, true);
                serverSubscriber.Resolve(sessionName);
            }
        }

        public void UnsubscribeServerSession()
        {
            if (serverSubscriber != null)
            {
                serverSubscriber.SetInUse(this, false);
                serverSubscriber = null;
            }
        }

        #region Configuration
        public IDataLayer GetDataLayer()
        {
            DevExpress.Xpo.Metadata.XPDictionary dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            dict.GetDataStoreSchema(typeof(UFRecipeModel.RecipeUAConfiguration).Assembly);
            dict.GetDataStoreSchema(typeof(XpoHelpers.ProtectionFile));

            if (String.IsNullOrEmpty(fileBase))
            {
                // dl = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var store = DevExpress.Xpo.XpoDefault.GetConnectionProvider(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
                var ret = new DevExpress.Xpo.ThreadSafeDataLayer(dict, store);

                return ret;
            }
            else
            {
                lock (lockObject)
                {
                    if (InMemory == null)
                    {
                        InMemory = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                        var fileInfo = new FileInfo(fileBase);
                        if (fileInfo.Exists && fileInfo.Length > 0)
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
                                        File.Copy(fileBase, String.Format("{0}.bak", fileBase), true);
                                        File.Delete(fileBase);
#if !NET_STANDARD
                                        var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                        if (uiMsgBox != null)
                                            uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorCorruptedDocument.Replace("-newline-", Environment.NewLine), fileBase));
#endif
                                        logGeneral.Error(String.Format(Properties.Resources.ErrorReadingDocument, fileBase), ex);
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
                                    File.Copy(fileBase, String.Format("{0}.bak", fileBase), true);
                                    File.Delete(fileBase);
#if !NET_STANDARD
                                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                                    if (uiMsgBox != null)
                                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorCorruptedDocument.Replace("-newline-", Environment.NewLine), fileBase));
#endif
                                    logGeneral.Error(String.Format(Properties.Resources.ErrorReadingDocument, fileBase), ex);
                                }
                        }
                    }
                }

                return new DevExpress.Xpo.ThreadSafeDataLayer(dict, InMemory);
            }
        }

#if !NET_STANDARD
        bool trackingChangesDisabled;
#endif
        void CreateDataLayer(bool bUseCacheUow)
        {
            dl = GetDataLayer();
            if (bUseCacheUow)
            {
                cachedUow = cachedUnitOfWorks.BeginUnitOfWork();
                cachedUow.KeepUnitOfWork = true;
                uow = cachedUow.UnitOfWork;
            }
            else
                uow = new UnitOfWork(dl);
#if !NET_STANDARD
            uow.ObjectChanged += (o, e) =>
                {
                    if (trackingChangesDisabled || !e.Session.TrackingChanges)
                        return;
                    NeedsSave = true;

                    if (e.PropertyName == "ApplicationName")
                    {
                        ConfigurationId = Guid.NewGuid();
                    }
                };

            // check if the address space need to reload because some treview item has been deleted
            uow.ObjectDeleting += (o, e) =>
            {
                if (trackingChangesDisabled || !e.Session.TrackingChanges)
                    return;
                NeedsSave = true;
            };

            uow.ObjectsSaved += (o, e) =>
            {
                if (trackingChangesDisabled)
                    return;
                NeedsSave = false;
            };

            //uowCloner = new UnitOfWork(dl);
            if (!bUseCacheUow)
            {
                InMemoryClipboard = new InMemoryDataStore(DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema, true);
                dlClipboard = new SimpleDataLayer(InMemoryClipboard);
                uowClipboard = new UnitOfWork(dlClipboard);
            }
#endif
        }

#if !NET_STANDARD
        NestedUnitOfWork lastNestedUnitOfWork;
        public NestedUnitOfWork BeginNestedUnitOfWork()
        {
            lastNestedUnitOfWork = uow.BeginNestedUnitOfWork();
            lastNestedUnitOfWork.Disposed += (s, e) => { lastNestedUnitOfWork = null; };
            return lastNestedUnitOfWork;
        }

        public XPObject GetNestedObject(object obj)
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

        bool IsBelongFromParent(IDocument parent)
        {
            var id = XpoHelpers.XpoHelper.GetProtectionCode(uow);
            return id == Guid.Empty || id == parent.Id;
        }

        public UnitOfWork GetSession()
        {
            return uow;
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

        public bool RemoveBaseAddress(string transport)
        {
            var list = (from b in GetConfiguration().BaseAddresses
                        where b.Transport == transport
                        select b).ToList();
            if (list.Count > 0)
            {
                while(list.Count > 0)
                {
                    var ba = list[0];
                    if (ba != null)
                        ba.Delete();
                    list.RemoveAt(0);
                }
                return true;
            }
            
            return false;
        }
        public UFRecipeModel.RecipeUABaseAddress AddNewBaseAddress(string transport)
        {
            var ba = new UFRecipeModel.RecipeUABaseAddress(uow)
                {
                    Enabled = true,
                    Transport = transport,
                    Server = "localhost",
                    Port = GetConfiguration().GetDefaultPort(transport)
                };

            GetConfiguration().BaseAddresses.Add(ba);

            return ba;
        }
        public List<UFRecipeModel.RecipeUABaseAddress> GetBaseAddressList()
        {
            return (from address in new XPQuery<UFRecipeModel.RecipeUABaseAddress>(uow, true).AsParallel()
                        select address).ToList();
        }
        public UFRecipeModel.RecipeUABaseAddress GetBaseAddress(string transport)
        {
            var list = (from address in new XPQuery<UFRecipeModel.RecipeUABaseAddress>(uow, true).AsParallel()
                        where address.Transport == transport
                        select address).ToList();
            if (list.Count > 0)
                return list[0];
            return null;
        }

        void SetDefaultLocalEndpoint(String str)
        {
            lock (lockObject)
            {
                if (cachedDefaultLocalEndpoint.ContainsKey(Thread.CurrentThread))
                    cachedDefaultLocalEndpoint.Remove(Thread.CurrentThread);
                cachedDefaultLocalEndpoint.Add(Thread.CurrentThread, str);
            }
        }

        internal List<string> GetEndpoints()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFRecipeModel.RecipeUAConfiguration(task.UnitOfWork);
                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);

                var endpoints = (from ba in ufuaConfiguration.BaseAddresses/*.AsParallel()*/
                                 where ba.Enabled == true
                                 select ba.Path).ToList();

                return endpoints;
            }
        }

        internal string GetDefaultLocalEndpoint(bool refresh = false)
        {
            if (!refresh)
                lock (lockObject)
                {
                    if (cachedDefaultLocalEndpoint.ContainsKey(Thread.CurrentThread))
                        return cachedDefaultLocalEndpoint[Thread.CurrentThread];
                }

            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFRecipeModel.RecipeUAConfiguration(task.UnitOfWork);
                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);

                var endpoints = (from ba in ufuaConfiguration.BaseAddresses/*.AsParallel()*/
                                 where ba.Enabled == true
                                 select ba).ToList();

                if (endpoints.Count > 0)
                {
                    foreach (var transport in transportOrderByRelevance)
                    {
                        var endpoint = (from ba in endpoints
                                        where ba.Transport == transport
                                        select ba).ToList();

                        if (endpoint.Count > 0)
                        {
                            SetDefaultLocalEndpoint(endpoint[0].Path);
                            return endpoint[0].Path;
                        }
                    }

                    if (endpoints.Count > 0)
                    {
                        SetDefaultLocalEndpoint(endpoints[0].Path);
                        return endpoints[0].Path;
                    }
                }

                var listadd = RecipeUAServerInfo.RecipeUAServerInfo.GetCurrentApplicationBaseAddresses();
                if (listadd != null && listadd.Count > 0)
                {
                    SetDefaultLocalEndpoint(listadd[0]);
                    return listadd[0];
                }

                return string.Empty;
            }
        }

#if !NET_STANDARD
        bool AddHttpAccessRules(bool silent)
        {
            var urls = (from address in new XPQuery<UFRecipeModel.RecipeUABaseAddress>(uow, true).AsParallel()
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
                    if (UIInterface != null)
                    {
                        return UIInterface.ShowYesNo(message, CustomDialogIcons.Warning) == CustomDialogResults.Yes;
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
        public UFRecipeModel.RecipeUAConfiguration GetConfiguration(bool inExecution = false)
        {
            if (inExecution)
            {
                using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                {
                    var list = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                    if (list.Count == 0)
                        return new UFRecipeModel.RecipeUAConfiguration(task.UnitOfWork);
                    return list[0];
                }
            }
            else
            {
                var list = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                if (list.Count == 0)
                    return new UFRecipeModel.RecipeUAConfiguration(uow);
                return list[0];
            }
        }

        internal String GetServiceName()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var serverName = RecipeUAServerInfo.RecipeUAServerInfo.GetServerName();
                var list = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                if (list.Count == 0)
                    return serverName;

                return String.Format("{0} ({1})", serverName, list[0].ApplicationName);
            }
        }

        void SetDefaultAppName(String str)
        {
            lock (lockObject)
            {
                if (cachedDefaultAppName.ContainsKey(Thread.CurrentThread))
                    cachedDefaultAppName.Remove(Thread.CurrentThread);
                cachedDefaultAppName.Add(Thread.CurrentThread, str);
            }
        }

        public String GetAplicationName(bool refresh = false)
        {
            if (!refresh)
                lock (lockObject)
                {
                    if (cachedDefaultAppName.ContainsKey(Thread.CurrentThread))
                        return cachedDefaultAppName[Thread.CurrentThread];
                }

            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true)/*.AsParallel()*/ select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFRecipeModel.RecipeUAConfiguration(task.UnitOfWork);

                ufuaConfiguration.EnsureDefaultSettings(defaultApplicationName);
                var ret = ufuaConfiguration.ApplicationName;
                SetDefaultAppName(ret);

                return ret;
            }
        }

#if !NET_STANDARD
        internal bool NeedToRunAsCFR21UserIndentity()
        {
            using (var task = cachedUnitOfWorks.BeginUnitOfWork())
            {
                var ufuaConfiguration = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).FirstOrDefault();
                if (ufuaConfiguration == null)
                    ufuaConfiguration = new UFRecipeModel.RecipeUAConfiguration(task.UnitOfWork);

                if (ufuaConfiguration.EnableEventDataProtection)
                    return true;

                return false;
            }
        }
#endif
        #endregion

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

                LastClipboardUnicodeText = xml.ToString();
                Clipboard.SetText(LastClipboardUnicodeText);
            }
        }

        string LastClipboardUnicodeText = String.Empty;
        internal void CopyWinClipboardToInMemoryData()
        {
            try
            {
                if (Clipboard.ContainsText(TextDataFormat.UnicodeText) &&
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
            { }
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

        internal void CleanClipbaord()
        {
            if (uowClipboard == null)
                return;

            uowClipboard.ClearDatabase();
        }
#endif
        #endregion

        #region Undo/Redo
#if !NET_STANDARD
        internal void CreateUndoRedoHelper()
        {
            CreateUndoRedoHelper(Properties.Settings.Default.MaxUndoRedoActions);
        }

        internal void CreateUndoRedoHelper(short numactions)
        {
            if (undoRedoHelper != null)
                return;

            dlUndoRedo = XpoDefault.GetDataLayer(InMemoryDataStore.GetConnectionStringInMemory(true), DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            uowUndoRedo = new UnitOfWork(dlUndoRedo);
            undoRedoHelper = numactions > 0 ? new UndoRedoXpoManager(uow, uowUndoRedo, numactions) : new UndoRedoXpoManager(uow, uowUndoRedo);
        }

        public void AddUndoAction(UserControl owner, IXPSimpleObject source, UndoRedoAction action)
        {
            AddUndoAction(owner, new UndoRedoXpoData(source, owner), action);
        }

        public void AddUndoAction(UserControl owner, IList<IXPSimpleObject> list, UndoRedoAction action)
        {
            var data = new UndoRedoXpoDataCollection();
            foreach (var source in list)
                data.Add(new UndoRedoXpoData(source, owner));
            AddUndoAction(owner, data, action);
        }

        internal void AddUndoAction(UserControl owner, UndoRedoXpoDataCollection list, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddUndoAction(list, action);
        }

        internal void AddUndoAction(UserControl owner, UndoRedoXpoData obj, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddUndoAction(obj, action);
        }

        internal void AddRedoAction(UserControl owner, IXPSimpleObject source, UndoRedoAction action)
        {
            AddRedoAction(owner, new UndoRedoXpoData(source, owner), action);
        }

        internal void AddRedoAction(UserControl owner, IList<IXPSimpleObject> list, UndoRedoAction action)
        {
            var data = new UndoRedoXpoDataCollection();
            foreach (var source in list)
                data.Add(new UndoRedoXpoData(source, owner));
            AddRedoAction(owner, data, action);
        }

        internal void AddRedoAction(UserControl owner, UndoRedoXpoDataCollection list, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddRedoAction(list, action);
        }

        internal void AddRedoAction(UserControl owner, UndoRedoXpoData obj, UndoRedoAction action)
        {
            if (undoRedoHelper == null)
                return;

            undoRedoHelper.AddRedoAction(obj, action);
        }

        internal bool UndoContainsSomething()
        {
            if (bObjectDisposed)
                return false;

            return undoRedoHelper != null && undoRedoHelper.CanUndo();
        }

        internal bool RedoContainsSomething()
        {
            if (bObjectDisposed)
                return false;

            return undoRedoHelper != null && undoRedoHelper.CanRedo();
        }

        internal String GetNextUndoOwner()
        {
            if (bObjectDisposed)
                return null;

            if (undoRedoHelper != null)
            {
                var data = undoRedoHelper.PeekNextUndo();
                if (data != null && data.Count > 0)
                    return data[0].OwnerTypeName;
            }
            return null;
        }

        internal String GetNextRedoOwner()
        {
            if (bObjectDisposed)
                return null;

            if (undoRedoHelper != null)
            {
                var data = undoRedoHelper.PeekNextRedo();
                if (data != null && data.Count > 0)
                    return data[0].OwnerTypeName;
            }
            return null;
        }

        internal UndoRedoXpoDataCollection UndoAction(UserControl owner, out UndoRedoAction action)
        {
            if (undoRedoHelper == null)
            {
                action = UndoRedoAction.None;
                return new UndoRedoXpoDataCollection();
            }

            if (undoRedoHelper.CanUndo())
            {
                var ret = undoRedoHelper.Undo(out action);
                undoRedoHelper.PurgeUndoActions();

                if (ret.Count > 0)
                {
                    switch (action)
                    {
                        case UndoRedoAction.Added:
                            break;

                        case UndoRedoAction.Changed:
                        case UndoRedoAction.Replaced:
                            break;

                        case UndoRedoAction.Removed:
                            foreach (var obj in ret)
                                AddExistingObject(obj);
                            break;
                    }
                }

                return ret;
            }

            action = UndoRedoAction.None;
            return null;
        }

        internal UndoRedoXpoDataCollection RedoAction(UserControl owner, out UndoRedoAction action)
        {
            if (undoRedoHelper == null)
            {
                action = UndoRedoAction.None;
                return new UndoRedoXpoDataCollection();
            }

            if (undoRedoHelper.CanRedo())
            {
                var ret = undoRedoHelper.Redo(out action);
                undoRedoHelper.PurgeRedoActions();

                if (ret.Count > 0)
                {
                    switch (action)
                    {
                        case UndoRedoAction.Added:
                            foreach (var obj in ret)
                                AddExistingObject(obj);
                            break;

                        case UndoRedoAction.Changed:
                        case UndoRedoAction.Replaced:
                            break;

                        case UndoRedoAction.Removed:
                            break;
                    }
                }

                return ret;
            }

            action = UndoRedoAction.None;
            return null;
        }

        internal XPObject GetParentObject(UndoRedoXpoData data)
        {
            return null;
        }

        void AddExistingObject(UndoRedoXpoData source)
        {
            AddExistingObject(source.Source, GetParentObject(source));
        }

        void AddExistingObject(XPObject obj, XPObject parent)
        {
            if (obj.IsDeleted)
                obj.SetMemberValue("GCRecord", null);
        }
#endif
        #endregion

        public static String GetConnectionString(String path
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
                                    Properties.Settings.Default.DefaultUAFileName,
                                    Properties.Settings.Default.DefaultUAFileExt);

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
                                Properties.Settings.Default.DefaultUAFileName,
                                Properties.Settings.Default.DefaultUAFileExt);
        }

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
                        var cursor = new XPCursor(uowTarget, typeof(UFRecipeModel.RecipeUAConfiguration));
                        foreach (XPBaseObject item in cursor)
                            item.Delete();

                        uowTarget.CommitChangesAndFreeMemory();
                        ////////////////////////////////////////////////////////////////////////////

                        var cloneHelperConf = new XpoHelpers.CloneIXPSimpleObjectHelper(sourceDoc.GetSession(), uowTarget, true, true, true);
                        cloneHelperConf.Clone(sourceDoc.GetConfiguration(), false);

                        uowTarget.CommitChangesAndFreeMemory();
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
                    sourceDoc.GetConfiguration().Delete();
                    sourceDoc.GetSession().CommitChanges();
                }
            }
        }
#endif

        public static RecipeUAServerDocument FromFile(String path, IDocumentManager c, IDocument parent,
            bool bCreateNew = true, bool bCheckEmpty = true, bool bUseCacheUow = false)
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

                var serverDoc = new RecipeUAServerDocument()
                {
                    connectionString = connString,
                    fileBase = xmlfile,
                    Parent = parent
                };

                serverDoc.CreateDataLayer(bUseCacheUow);
                if (!bCreateNew && bCheckEmpty && serverDoc.IsEmpty)
                {
                    serverDoc.Dispose();
                    return null;
                }
                else if (
#if !NET_STANDARD
                    vfs == null &&
#endif
                    !serverDoc.IsBelongFromParent(parent))
                {
                    serverDoc.Dispose();
#if !NET_STANDARD
                    var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                    if (uiMsgBox != null)
                        uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorValidatingDocument, path));
#else
                    logGeneral.ErrorFormat(Properties.Resources.ErrorValidatingDocument, path);
#endif
                    return null;
                }

                var title = Path.GetFileNameWithoutExtension(path);
#if !NET_STANDARD
                if (vfs != null)
                    title = XpoHelpers.XpoHelper.GetDataSourceTitle(connString, true);
#endif
                serverDoc.EnsureDefaultSettings(title);

#if !NET_STANDARD
                serverDoc.NeedsSave = false;
#endif
                return serverDoc;
            }
            catch
            {
#if !NET_STANDARD
                var uiMsgBox = parent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                if (uiMsgBox != null)
                    uiMsgBox.ShowError(String.Format(Properties.Resources.ErrorReadingDocument, path));
#else
                logGeneral.ErrorFormat(Properties.Resources.ErrorReadingDocument, path);
#endif
                return null;
            }
        }

#if !NET_STANDARD
        public bool CanClose()
        {
            if (ServerCMSHelperSync.IsServerStartedManually)
            {
                if (UIInterface != null)
                {
                    var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.StopServerStartedManually,
                        RecipeUAServerInfo.RecipeUAServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                    else if (res == CustomDialogResults.Yes)
                        ServerCMSHelperSync.StopServer();
                }
            }

            return true;
        }
#endif

        public bool SaveToFile(bool discargechanges = false, bool bForceSave = false, bool forceEncryption = false)
        {
            if (uow == null)
                return false;

#if !NET_STANDARD
            List<XPObject> parentObjects = null;
            if (ActiveView != null && Workspace != null &&
                Workspace.ActiveWindow == ActiveView)
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
#endif

            try
            {
#if !NET_STANDARD
                if (discargechanges)
                {
                    if (uow.TryPurgeDeletedObjects(logGeneral) > 0)
                        bForceSave = true;
                }

                if (!bForceSave && !NeedsSave)
                    return false;
#endif

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
#if !NET_STANDARD
                if (UIInterface != null)
                    UIInterface.ShowError(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message));
#endif
                return false;
            }

#if !NET_STANDARD
            if (parentObjects != null && parentObjects.Count > 0)
            {
                if (parentObjects.Count == 1)
                    Workspace.ContextObject = GetNestedObject(parentObjects[0]);
                else
                    Workspace.ContextObject = GetNestedObjects(parentObjects);
            }
#endif

            return true;
        }

#if !NET_STANDARD
        public void CertificateChecker()
        {
            // /SUFSolution /EE:\PRIVATE\12-0-Drivers\UFSolution\bin\Debug\UFSolution.Config.xml /C client
            // /SUFUAServer /EE:\PRIVATE\12-0-Drivers\UFSolution\bin\Debug\UFUAServer.UAServer.Config.xml
            string arguments = string.Format(Properties.Settings.Default.CertificateCheckerArgs/*""/SPlatform.NExT IOServer" "/E{0}" /I"*/,
                RecipeUAServerInfo.RecipeUAServerInfo.GetServerConfigFile(), GetAplicationName(), ApplicationPropertiesHelper.GetProperty("CurrentSkin"));
            string path = Properties.Settings.Default.CertificateChecker/*"CertificateChecker.exe"*/;
            Assembly callingMainAssembly = Assembly.GetEntryAssembly();
            if (callingMainAssembly != null)
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);
            Process.Start(path, arguments);
        }

        public void ServiceManager()
        {
            AddHttpAccessRules(true);

            string projectConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                var rootParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                projectConn = rootParent.FilePath;
                if (StringEditor != null)
                    stringConn = StringEditor.GetConnectionStringFromFile(rootBase);
                if (UserEditor != null)
                    userConn = UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                projectConn = userConn = stringConn = ConnectionString;

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
            //    RecipeServiceCMS.RecipeServiceCSMHelpers.OpenServiceManagerWithLogInInformation(applicationName, username, password, serverConn, stringConn, userConn, docPath);
            //}
            //else
            {
                var dependencies = new List<String>();
                if (UfuaEditorService != null)
                    dependencies.Add(UfuaEditorService.GetServiceName(Parent));

                RecipeServiceCMS.RecipeServiceCSMHelpers.OpenServiceManager(applicationName, dependencies.ToArray(), projectConn, stringConn, userConn, docPath);
            }
        }
#endif

#if !NET_STANDARD
        public bool StartServer(bool bSave = true, bool manually = false)
        {
            return StartServer(null, null, bSave, manually);
        }
        public bool StartServer(TextBlock textBlock, ScrollViewer scroll, bool bSave = true, bool manually = false)
        {
            if (bSave && NeedsSave)
            {
                if (UIInterface != null)
                {
                    var res = UIInterface.ShowYesNoCancel(String.Format(Properties.Resources.SaveDoc,
                        RecipeUAServerInfo.RecipeUAServerInfo.GetServerName()), CustomDialogIcons.Question);
                    if (res == CustomDialogResults.Cancel || res == CustomDialogResults.None)
                        return false;
                }

                SaveToFile();
            }

            if (!AddHttpAccessRules(false))
                return false;

            string projectConn = String.Empty;
            string stringConn = String.Empty;
            string userConn = String.Empty;
            if (fileBase != null)
            {
                var rootParent = DocumentHelper.GetRootParent(Parent, traverse: false);
                projectConn = rootParent.FilePath;
                if (StringEditor != null)
                    stringConn = StringEditor.GetConnectionStringFromFile(rootBase);
                if (UserEditor != null)
                    userConn = UserEditor.GetConnectionStringFromFile(rootBase);
            }
            else
                projectConn = userConn = stringConn = ConnectionString;

            var docPath = GetSpecialFolder(SpecialFolders.Documents).GetPathString();
            docPath = docPath.Trim('\\', '/');

            bool suspended = false;
#if !DEBUG
            if(MSZ.MSZView.IsSuspended())
                suspended = true;
#endif
            var needToRunAsCFR21UserIndentity = false;
            //var needToRunAsCFR21UserIndentity = NeedToRunAsCFR21UserIndentity();
            //if (needToRunAsCFR21UserIndentity && FilePath != null)
            //{
            //    try
            //    {
            //        var rootPath = FilePath;
            //        if (Parent != null)
            //            rootPath = Parent.FilePath;

            //        DirectoryInfo dInfo = new DirectoryInfo(Path.GetDirectoryName(rootPath));
            //        var accessRule = new System.Security.AccessControl.FileSystemAccessRule(
            //            UFUAServerInfo.UFUAServerInfo.GetCFR21UserNameSetting(),
            //            System.Security.AccessControl.FileSystemRights.Read |
            //            System.Security.AccessControl.FileSystemRights.Write |
            //            System.Security.AccessControl.FileSystemRights.Delete |
            //            System.Security.AccessControl.FileSystemRights.Modify,
            //            System.Security.AccessControl.InheritanceFlags.ContainerInherit |
            //            System.Security.AccessControl.InheritanceFlags.ObjectInherit,
            //            System.Security.AccessControl.PropagationFlags.InheritOnly,
            //            System.Security.AccessControl.AccessControlType.Allow);

            //        System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl();
            //        dSecurity.RemoveAccessRuleAll(accessRule);
            //        dSecurity.AddAccessRule(accessRule);
            //        dInfo.SetAccessControl(dSecurity);
            //    }
            //    catch { }
            //}

            var serverCMSHelper = manually ? ServerCMSHelperAsync : ServerCMSHelperSync;
            return serverCMSHelper.StartServer(textBlock, scroll, suspended, manually, needToRunAsCFR21UserIndentity, projectConn, stringConn, userConn, docPath);
        }
#endif

        public OPCUAEntityReference GetRecipeUAServerEntityReference()
        {
            var applicationName = GetAplicationName();
            return new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            null, ObjectIds.Server, ObjectIds.Server.ToString(), null, null);
        }

        public OPCUAEntityReference GetRecipeUAServerEntityReference(String relativePath, String nodeID)
        {
            String relativepath = null;
            Opc.Ua.NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            String tag = null;

            Opc.Ua.NodeId nodeid = null;
            Guid guid;
            if (Guid.TryParse(nodeID, out guid))
                nodeid = new Opc.Ua.NodeId(guid, ns);
            else
                nodeid = new Opc.Ua.NodeId(nodeID, ns);
            if (relativePath.Contains('/'))
            {
                var rel = new System.Text.StringBuilder();
                foreach (String s in relativePath.Split('/').ToList())
                {
                    rel.AppendFormat("/{1}:{0}", s, ns);
                }
                relativepath = rel.ToString();
            }
            else
            {
                tag = relativePath;
                relativepath = String.Format("/{1}:{0}", relativePath, ns);
            }


            var applicationName = GetAplicationName();
            OPCUAEntityReference entityreference = null;
            entityreference = new OPCUAEntityReference(null, applicationName, GetDefaultLocalEndpoint(),
                                            relativepath, nodeid, string.Format("{0} ({1})", tag, applicationName), null, relativepath);
            return entityreference;
        }
        #endregion

        #region Properties
#if !NET_STANDARD
        [Browsable(false)]
        bool needsSave;
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

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        IStringEditorManager stringEditor;
        public IStringEditorManager StringEditor
        {
            get
            {
                if (stringEditor == null)
                    stringEditor = GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringEditor;
            }
        }

        IUFUserEditorManager userEditor;
        public IUFUserEditorManager UserEditor
        {
            get
            {
                if (userEditor == null)
                    userEditor = GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                return userEditor;
            }
        }

        IUFUAEditorManager ufuaEditorService;
        public IUFUAEditorManager UfuaEditorService
        {
            get
            {
                if (ufuaEditorService == null)
                    ufuaEditorService = GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;

                return ufuaEditorService;
            }
        }
#endif

        IUFProjectManager ufprojectManager;
        public IUFProjectManager UFProjectManager
        {
            get
            {
                if (ufprojectManager == null)
                    ufprojectManager = GetService(typeof(IUFProjectManager)) as IUFProjectManager;

                return ufprojectManager;
            }
        }

        IRecipeEditorManager recipeManager;
        public IRecipeEditorManager RecipeManager
        {
            get
            {
                if (recipeManager == null)
                    recipeManager = GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;

                return recipeManager;
            }
        }

#if !NET_STANDARD
        UFInterfaces.IWorkspace workspace;
        UFInterfaces.IWorkspace Workspace
        {
            get
            {
                if (workspace == null)
                    workspace = GetService(typeof(UFInterfaces.IWorkspace)) as UFInterfaces.IWorkspace;

                return workspace;
            }
        }
#endif


        [Browsable(false)]
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
                    using (var task = cachedUnitOfWorks.BeginUnitOfWork())
                    {
                        var list = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(task.UnitOfWork, true).AsParallel() select tag).ToList();
                        if (list.Count == 0)
                            return Guid.Empty;

                        configurationId = list[0].ConfigurationId;
                    }
                }

                return configurationId;
            }
            set
            {
                if (configurationId == value)
                    return;

                var list = (from tag in new XPQuery<UFRecipeModel.RecipeUAConfiguration>(uow, true).AsParallel() select tag).ToList();
                if (list.Count > 0)
                {
                    list[0].ConfigurationId = value;
                    configurationId = value;
                }
            }
        }

#if !NET_STANDARD
        RecipeServiceCSMHelpers serverCMSHelperSync;
        [Browsable(false)]
        public RecipeServiceCSMHelpers ServerCMSHelperSync
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
                    serverCMSHelperSync = new RecipeServiceCSMHelpers(ConfigurationId.ToString(), logGeneral);

                return serverCMSHelperSync;
            }
        }

        RecipeServiceCSMHelpers serverCMSHelperAsync;
        [Browsable(false)]
        public RecipeServiceCSMHelpers ServerCMSHelperAsync
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
                    serverCMSHelperAsync = new RecipeServiceCSMHelpers(ConfigurationId.ToString(), logGeneral);
                    serverCMSHelperAsync.StartServerStatusInBackground();
                }

                return serverCMSHelperAsync;
            }
        }
#endif
        #endregion

        #region ICloneable Members

        RecipeUAServerDocument(RecipeUAServerDocument template)
        {
            if (template == null)
                return;

            throw new NotImplementedException();
        }

#if !NET_STANDARD
        public object Clone()
        {
            return new RecipeUAServerDocument(this);
        }
#endif
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

        public override String Title
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
#if !NET_STANDARD
                var docManager = RecipeManager as IDocumentManager;
                if (docManager == null || UFProjectManager == null)
                    return false;
                return UFProjectManager.GetResourceList(this, docManager.TypeScheme).Count() == 0;
#else
                return false;
#endif
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
            // EditorManagerComponent.Workspace.ContextObject = null;

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
#endif

            if (cachedUnitOfWorks != null)
                cachedUnitOfWorks.Dispose();

            lock (lockObject)
            {
                cachedDefaultLocalEndpoint.Clear();
                cachedDefaultAppName.Clear();
            }

#if !NET_STANDARD
            if (undoRedoHelper != null)
            {
                undoRedoHelper.PurgeUndoActions();
                undoRedoHelper.PurgeRedoActions();
                undoRedoHelper = null;
            }

            if (uowUndoRedo != null)
            {
                uowUndoRedo.Disconnect();
                uowUndoRedo.Dispose();
                uowUndoRedo = null;
            }

            if (dlUndoRedo != null)
            {
                dlUndoRedo.Dispose();
                dlUndoRedo = null;
            }

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
    }
}
