using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UFInterfaces;
using Opc.Ua;
using ViewModelLib;
using DocumentManager.ComponentService;
using System.ComponentModel;
using System.Threading;
using DocumentManager.ComponentService.Helpers;
#if !WINDOWS_UWP
#if !NET_STANDARD
using DevExpress.Xpf.Core;
using OPCUAViewModelService.ComponentService;
using System.Windows;
using OPCUABrowser.ComponentService;
using System.Windows.Media.Animation;
using UFUAEditor.ComponentService;
using System.Windows.Controls;
using UFInterfaces.Editors;
using System.Windows.Threading;
using Utilities.WPF;
#endif
using Utilities;
#endif

namespace OPCUAViewModel
{
    [DataContract(Name = "OPCUADataReference", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class OPCUAEntityReference : ViewModelBase
#if !WINDOWS_UWP && !NET_STANDARD
        , ICloneable, IFormattable
#endif
    {
#region Declarations
        RealTimeConnectionManagerViewModel realTimeModel;
        String sessionname;
        bool bIsDataSink;

        bool bUseOnlyEmbeddedItems;
        List<OPCUAEntityReference> embeddedRedundantList;
        bool bIsEmbeddedRedundant;
        bool bPropagatingMonitoredItemModel;
        bool bForceDiscoverConnectedEmbedded;
        bool bIsTemporary;
        bool bIsEmbeddedRedundantElaborated;
        bool bIsEmbeddedRealTimeModelConnected;
        String embeddedRedundantHostName;
        MonitoredItemViewModel embeddedMonitoredItemViewModel;
        NodeIdViewModel embeddedStartingNodeIdViewModel;
        NodeIdViewModel embeddedNodeIdViewModel;
        RealTimeConnectionManagerViewModel embeddedRealTimeModel;
        //PropertyObserver<RealTimeConnectionManagerViewModel> observerRedundancyUriArray;
        //PropertyObserver<SessionViewModel> observerSessionConnected;

#endregion

        public OPCUAEntityReference(bool isRedundancyEnabled = true)
        {
            IsRedundancyEnabled = isRedundancyEnabled;
        }

        public OPCUAEntityReference(OPCUAEntityReference template)
        {
            if (template == null)
                return;

            HostName = template.HostName;
            AppName = template.AppName;
            EndpointUrl = template.EndpointUrl;
            RelativePath = template.RelativePath;
            ReadablePath = template.ReadablePath;
            StartingAddress = template.StartingAddress;
            ResolvedNodeId = template.ResolvedNodeId;
            ResolvedStartingNodeId = template.ResolvedStartingNodeId;
            ParentNodeId = template.ParentNodeId;
            TypeDefinitionNodeId = template.TypeDefinitionNodeId;
            TypeDefinitionName = template.TypeDefinitionName;
            ParentTypeDefinitionNodeId = template.ParentTypeDefinitionNodeId;
            ParentTypeDefinitionName = template.ParentTypeDefinitionName;
            HumanReadable = template.HumanReadable;
            IsRedundancyEnabled = template.IsRedundancyEnabled;

            NormalizeHost();
            //if (IsValid)
            //    PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
        }

        public void UpdateValue(OPCUAEntityReference template)
        {
            HostName = template.HostName;
            AppName = template.AppName;
            EndpointUrl = template.EndpointUrl;
            RelativePath = template.RelativePath;
            ReadablePath = template.ReadablePath;
            StartingAddress = template.StartingAddress;
            ResolvedNodeId = template.ResolvedNodeId;
            ResolvedStartingNodeId = template.ResolvedStartingNodeId;
            ParentNodeId = template.ParentNodeId;
            TypeDefinitionNodeId = template.TypeDefinitionNodeId;
            TypeDefinitionName = template.TypeDefinitionName;
            ParentTypeDefinitionNodeId = template.ParentTypeDefinitionNodeId;
            ParentTypeDefinitionName = template.ParentTypeDefinitionName;
            ParentTypeDefinitionNameList = template.ParentTypeDefinitionNameList;
            ParentTypeDefinitionIdList = template.ParentTypeDefinitionIdList;
            HumanReadable = template.HumanReadable;
            IsRedundancyEnabled = template.IsRedundancyEnabled;

            NormalizeHost();
        }

        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            isRedundancyEnabled = true;
        }

        private bool CheckForCoerence(OPCUAEntityReference currvalue)
        {
            return true;
        }

        public bool MatchTypeDefintion(OPCUAEntityReference value)
        {
            if (value.ParentTypeDefinitionNodeId == TypeDefinitionNodeId)
                return true;
            if (value.ParentTypeDefinitionIdList != null && 
                value.ParentTypeDefinitionIdList.Contains(TypeDefinitionNodeId))
                return true;
            return false;
        }

        public void Merge(OPCUAEntityReference value)
        {
            AppName = value.AppName;
            HostName = value.HostName;
            EndpointUrl = value.EndpointUrl;
            ResolvedNodeId = null;
            ResolvedItem = true;

            //if (ParentTypeDefinitionNodeId == value.TypeDefinitionNodeId)
                StartingAddress = value.RelativePath;
                ReadablePath = String.Format("{0}/{1}", StartingAddress, RelativePath);
            //else
            //{
            //    var index = ParentTypeDefinitionIdList.IndexOf(value.TypeDefinitionNodeId);
            //    if (index >= 0)
            //    {
            //        index = (ParentTypeDefinitionIdList.Count - 1) - index; // revert
            //        var split = StartingAddress.Split('/');

            //        var builder = new StringBuilder();
            //        var substitute = value.RelativePath.Replace("/", "");
            //        for (int i = 0; i < split.Length; ++i)
            //        {
            //            if (i == index)
            //            {
            //                if (builder.Length > 0)
            //                    builder.Append("/");
            //                builder.Append(substitute);
            //            }
            //            else
            //            {
            //                if (builder.Length > 0)
            //                    builder.Append("/");
            //                builder.Append(split[i]);
            //            }
            //        }

            //        StartingAddress = builder.ToString();
            //    }
            //}
        }

        public OPCUAEntityReference(String host, String appName, String relativePath, String endpointUrl, 
                                    String startingAddess, NodeId resolved, NodeId resolvedStarting, String humanReadable,
                                    ExpandedNodeId typeDefinitionNodeId, String typeDefinitionName, String readablePath)
        {
            HostName = host;
            AppName = appName;
            RelativePath = relativePath;
            ReadablePath = readablePath;
            StartingAddress = startingAddess;
            ResolvedNodeId = resolved;
            ResolvedStartingNodeId = resolvedStarting;
            TypeDefinitionNodeId = typeDefinitionNodeId;
            TypeDefinitionName = typeDefinitionName;
            EndpointUrl = endpointUrl;
            HumanReadable = humanReadable;
            IsRedundancyEnabled = true;

            NormalizeHost();
            //if (IsValid)
            //    PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
        }
        public OPCUAEntityReference(String host, String appName, String endpointUrl, String relativePath,
                                    NodeId resolved, String humanReadable, ExpandedNodeId typeDefinitionNodeId)
        {
            HostName = host;
            AppName = appName;
            RelativePath = relativePath;
            ResolvedNodeId = resolved;
            EndpointUrl = endpointUrl;
            TypeDefinitionNodeId = typeDefinitionNodeId;
            HumanReadable = humanReadable;
            IsRedundancyEnabled = true;

            NormalizeHost();
        }

        public OPCUAEntityReference(String host, String appName, String endpointUrl, String relativePath,
                                    NodeId resolved, String humanReadable, ExpandedNodeId typeDefinitionNodeId, String readablePath, bool bAllowAutoRedundancy = true)
        {
            HostName = host;
            AppName = appName;
            RelativePath = relativePath;
            ReadablePath = readablePath;
            ResolvedNodeId = resolved;
            EndpointUrl = endpointUrl;
            TypeDefinitionNodeId = typeDefinitionNodeId;
            HumanReadable = humanReadable;

            IsRedundancyEnabled = bAllowAutoRedundancy;

            NormalizeHost();
            //if (IsValid)
            //    PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
        }

#region Methods

        readonly static String dnsHostName;
        static OPCUAEntityReference()
        {
#if WINDOWS_UWP
            dnsHostName = Utilities.NetworkHelpers.GetHostName();
#else
            dnsHostName = System.Net.Dns.GetHostName();
#endif
        }

        void NormalizeHost()
        {
            var hostname = dnsHostName;

            if (HostName == hostname)
                HostName = null;

            // ignore nulls.
            if (String.IsNullOrEmpty(EndpointUrl))
                return;

            try
            {
                var uri = new Uri(EndpointUrl);
                if (uri.Host == hostname)
                {
                    var newUriBuilder = new UriBuilder(uri);
                    newUriBuilder.Host = "localhost";
                    uri = newUriBuilder.Uri;
                    EndpointUrl = uri.OriginalString;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        //[OnDeserializing]
        //private void Initialize(StreamingContext context)
        //{
        //    if (IsValid)
        //        PromoteIdleExecution(System.Windows.Threading.DispatcherPriority.Invalid);
        //}

/*
static NodeId ConvertToCommonRelativeTypeDefinition(List<OPCUAEntityReference> list)
{
    NodeId ret = null;
    list.ForEach(reference =>
        {
            Node FindParent
        });

    return ret; //cannot find any type definition in common
}
*/

#if !WINDOWS_UWP && !NET_STANDARD
        private object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;

            return null;
        }

        static Dictionary<IDocument, GeneralDialogContent> mapEditors = new Dictionary<IDocument, GeneralDialogContent>();
        static Dictionary<GeneralDialogContent, bool> mapEditing = new Dictionary<GeneralDialogContent, bool>();
        static UserControl emptyTabControl;
        static bool? result;
        static object localServerTag = new object();
        static object opcuaBrowserTag = new object();
        public bool Edit(string title = null, bool sync = false, FilterType filter = FilterType.None, bool localserver = false, bool noDataSinks = false, bool noLocalServer = false)
        {
            OPCUAViewModelComponent.QueryInterfaces();
            if (Editor == null && 
                OPCUAViewModelComponent.ufuaEditorServiceAvailable &&
                OPCUAViewModelComponent.workspaceServiceAvailable)
            {
                Editor = OPCUAViewModelComponent.ufuaEditorService;
                Document = OPCUAViewModelComponent.workspaceService.ContextDocument;
            }

            var document = Document;
            if (document == null)
                return false;
            OPCUAViewModelComponent.workspaceService.IsBusy = true;
            Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    OPCUAViewModelComponent.workspaceService.IsBusy = false;
                });
            GeneralDialogContent wnd = FindEditor(document);
            if (wnd != null && mapEditing.ContainsKey(wnd) && mapEditing[wnd] == true)
            {
                wnd.Activate();
                return true;
            }

            if (Editor != null)
            {
                result = null;
                if (wnd == null)
                {
                    //var tabControl = new TabControlExt()
                    //{
                    //    IsLazyLoaded = true,
                    //    HideHeaderOnSingleChild = true,
                    //    CloseButtonType = CloseButtonType.Hide
                    //};
                    var tabControl = new DXTabControl();
                    var ufuaeditor = Editor.GetAddressSpaceControl(Document, bNewControl: true);
                    if (ufuaeditor == null)
                        return false;
                    ufuaeditor.DataContext = this;

                    if (ufuaeditor.Tag is IDocument)
                        document = ufuaeditor.Tag as IDocument;

                    ufuaeditor.ClearValue(FrameworkElement.WidthProperty);
                    ufuaeditor.ClearValue(FrameworkElement.HeightProperty);

                    var tab1 = new DXTabItem()
                    {
                        IsSelected = true,
                        Header = (Editor as IDocumentManager)?.TypeTitle,
                        Content = ufuaeditor,
                        Tag = localServerTag
                    };
                    tab1.InitItemTemplate();
                    tabControl.Items.Add(tab1);
                    var mapTabs = new Dictionary<DXTabItem, String>();

                    lock (mapDataSinkInterfaces)
                    {
                        mapDataSinkInterfaces.Keys.ToList().ForEach(name =>
                        {
                            var tab = new DXTabItem()
                            {
                                IsSelected = false,
                                Header = GetDataSinkInterface(name).HumanReadableName,
                                Tag = GetDataSinkInterface(name)
                            };
                            tab.InitItemTemplate();
                            tabControl.Items.Add(tab);

                            mapTabs.Add(tab, name);

                            mapDataSinkInterfaces[name].SetDocumentParent(Document);
                        });
                    }

                    if (mapTabs.Count > 0 || OPCUAViewModelComponent.opcuaBrowserServiceAvailable)
                    {
                        var tab2 = new DXTabItem()
                        {
                            IsSelected = false,
                            Header = Properties.Resource.OPCUABrowser,
                            Tag = opcuaBrowserTag
                        };
                        tab2.InitItemTemplate();
                        tabControl.Items.Add(tab2);

                        tabControl.SelectionChanged += (o, e) =>
                        {
                            using (new WaitCursor())
                            {
                                if (e.NewSelectedItem == tab2)
                                {
                                    if (tab2.Content == null && OPCUAViewModelComponent.opcuaBrowserServiceAvailable)
                                    {
                                        var editor = OPCUAViewModelComponent.opcuaBrowserService.Editor;
                                        editor.DataContext = this;

                                        editor.ClearValue(FrameworkElement.WidthProperty);
                                        editor.ClearValue(FrameworkElement.HeightProperty);

                                        tab2.Content = editor;
                                    }
                                }

                                mapTabs.Keys.ToList().ForEach(tab =>
                                    {
                                        if (e.NewSelectedItem == tab)
                                        {
                                            if (tab.Content == null || tab.Content == emptyTabControl)
                                            {
                                                lock (mapDataSinkInterfaces)
                                                {
                                                    var editor = mapDataSinkInterfaces[mapTabs[tab]].Editor();
                                                    if (editor != null)
                                                    {
                                                        editor.DataContext = this;

                                                        editor.ClearValue(FrameworkElement.WidthProperty);
                                                        editor.ClearValue(FrameworkElement.HeightProperty);

                                                        tab.Content = editor;
                                                    }
                                                    else
                                                    {
                                                        if (emptyTabControl == null)
                                                            emptyTabControl = new WPFUtilities.Controls.EmptyTab();
                                                        (emptyTabControl as WPFUtilities.Controls.EmptyTab).InfoMessage = String.Format(Properties.Resource.NullEditorMessage, mapDataSinkInterfaces[mapTabs[tab]].HumanReadableName);
                                                        tab.Content = emptyTabControl;
                                                    }
                                                }
                                            }
                                        }
                                    });
                            }
                        };
                    }
                    wnd = new GeneralDialogContent(tabControl)
                    {
                        DialogKeepContent = true,
                        Title = Properties.Resource.BrowseForAnyTypeTitle,
                        HelpLink = "Browse"
                    };
                    AddEditor(document, wnd);
                }

                wnd.Title = Properties.Resource.BrowseForAnyTypeTitle;
                if (!String.IsNullOrEmpty(title))
                    wnd.Title = String.Format("{0} - {1}", Properties.Resource.BrowseForAnyTypeTitle, title);

                var tabbers = wnd.DialogContent as DXTabControl;
                if (tabbers != null)
                {
                    Dictionary<DXTabItem, string> ts = new Dictionary<DXTabItem, string>();
                    bool clearSelection = false;
                    const string browserTab = "@opcuaBrowserTab";
                    foreach (var t in tabbers.Items)
                    {
                        var tt = t as DXTabItem;
                        if (tt == null)
                            continue;

                        tt.Visibility = Visibility.Visible;
                        if (tt.Tag == opcuaBrowserTag)
                        {
                            tt.Visibility = !localserver ? Visibility.Visible : Visibility.Collapsed;
                            if (tt.Visibility == Visibility.Visible)
                                ts.Add(tt, browserTab);
                        }
                        else if (tt.Tag == localServerTag)
                        {
                            tt.Visibility = !noLocalServer ? Visibility.Visible : Visibility.Collapsed;
                            if (tt.Visibility == Visibility.Visible)
                                ts.Add(tt, Editor.GetAplicationName(Document));
                        }
                        else if (tt.Tag is DataSinkInterface)
                        {
                            var dataSync = tt.Tag as DataSinkInterface;
                            if (dataSync.IsProjectTypeAware(Document.ProjectType))
                                tt.Visibility = !noDataSinks ? Visibility.Visible : Visibility.Collapsed;
                            else
                                tt.Visibility = Visibility.Collapsed;
 
                            if (tt.Visibility == Visibility.Visible)
                                ts.Add(tt, dataSync.DataSynkName);
                        }

                        if (tt.Visibility != Visibility.Visible && tt.IsSelected)
                            clearSelection = true;

                        if (tt.Content is IAddressSpaceControl)
                        {
                            var tcontrol = tt.Content as IAddressSpaceControl;
                            if(filter != tcontrol.FilterType)
                                tcontrol.FilterType = filter;
                        }

                        if (tt.Tag == localServerTag && tt.Visibility == Visibility.Visible)
                        {
                            tt.Header = (Editor as IDocumentManager)?.TypeTitle;
                            var projectManager = Document.GetService(typeof(UFProjectManager.ComponentService.IUFProjectManager)) as UFProjectManager.ComponentService.IUFProjectManager;
                            if (projectManager != null && projectManager.GetActiveProjects().ToList().Count > 1)
                                tt.Header = String.Format("{0} ({1})", tt.Header, Document.Parent != null ? Document.Parent.Title : Document.Title);
                        }
                    }

                    if (clearSelection)
                        tabbers.SelectedIndex = -1;
                    else
                    {
                        var tt = (from t in ts where t.Value == appName select t.Key).FirstOrDefault();
                        //if (tt == null && ts.ContainsValue(browserTab))
                        //    tt = (from t in ts where t.Value == browserTab select t.Key).FirstOrDefault();

                        if (tt != null)
                        {
                            tt.IsSelected = true;
                            ISelectEntityReference control = tt.Content as ISelectEntityReference;
                            if (control != null)
                                control.BringIntoView(this);
                        }
                    }
                }

                mapEditing[wnd] = true;
                bool bRet = wnd.ShowDialog() == true;
                /*
                if (tabbers != null && tabbers.Items.Count > 0)
                {
                    foreach (var t in tabbers.Items)
                    {
                        var tt = t as DXTabItem;
                        if(tt != null)
                            tt.Visibility = Visibility.Visible;
                    }
                }
                */

                mapEditing[wnd] = false;
                if (result != null)
                    bRet = result.Value;
                if (bRet)
                {
                    var tabControl = wnd.DialogContent as DXTabControl;
                    var updateValueAction = new Action(() =>
                    {
                        ScheduleValueUpdate((tabControl.SelectedItem as DXTabItem).Content as ISelectEntityReference);
                    });

                    if (sync)
                        Dispatcher.CurrentDispatcher.InvokeIfRequired(updateValueAction);
                    else
                        Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(updateValueAction);
                }
                return bRet;
            }
            else
            {
                if (!OPCUAViewModelComponent.opcuaBrowserServiceAvailable)
                    throw new NotImplementedException("Expecting the missing IOPCUABrowser Interface");

                var editor = OPCUAViewModelComponent.opcuaBrowserService.Editor;
                editor.DataContext = this;

                result = null;
                if (wnd == null)
                {
                    wnd = new GeneralDialogContent(editor)
                    {
                        DialogKeepContent = true,
                        Title = Properties.Resource.BrowseForAnyTypeTitle,
                        HelpLink = "Browse"
                    };
                    AddEditor(document, wnd);
                }

                if (String.IsNullOrEmpty(HumanReadable))
                {
                    wnd.Title += " - ";
                    wnd.Title += HumanReadable;
                }

                mapEditing[wnd] = true;
                bool bRet = wnd.ShowDialog() == true;
                mapEditing[wnd] = false;
                if (result != null)
                    bRet = result.Value;
                if (bRet)
                {
                    var updateValueAction = new Action(() =>
                    {
                        ScheduleValueUpdate(editor as ISelectEntityReference);
                    });

                    if (sync)
                        Dispatcher.CurrentDispatcher.InvokeIfRequired(updateValueAction);
                    else
                        Dispatcher.CurrentDispatcher.BeginInvokeAsynchronouslyInBackground(updateValueAction);
                    return true;
                }
            }

            return false;
        }

        void ScheduleValueUpdate(ISelectEntityReference editor)
        {
            OPCUAEntityReference currvalue = null;
            if (editor.SelectedReferences != null && editor.SelectedReferences.Count > 0 && (editor.SelectedReferences.First() as TagIdentifier)?.TagReference is OPCUAEntityReference)
                currvalue = (OPCUAEntityReference)(editor.SelectedReferences.First() as TagIdentifier).TagReference;
            else if (editor.SelectedReference != null && editor.SelectedReference is OPCUAEntityReference)
                currvalue = (OPCUAEntityReference)editor.SelectedReference;

            if (currvalue != null)
                UpdateValue(currvalue);
        }

        GeneralDialogContent FindEditor(IDocument doc)
        {
            var rootParent = DocumentHelper.GetRootParent(doc, traverse: false);
            if (mapEditors.ContainsKey(rootParent))
                return mapEditors[rootParent];

            return null;
        }

        void AddEditor(IDocument doc, GeneralDialogContent dialog)
        {
            var rootParent = DocumentHelper.GetRootParent(doc, traverse: false);
            if (!mapEditors.ContainsKey(rootParent))
            {
                mapEditors.Add(rootParent, dialog);

                dialog.Closing += dialog_Closing;
                rootParent.Disposing += document_Disposing;
                if (doc != rootParent)
                    doc.Disposing += document_Disposing;
            }
        }

        void RemoveEditor(IDocument doc)
        {
            var rootParent = DocumentHelper.GetRootParent(doc, traverse: false);
            if (mapEditors.ContainsKey(rootParent))
            {
                var dialog = mapEditors[rootParent];
                mapEditors.Remove(rootParent);
                mapEditing.Remove(dialog);

                if (dialog.DialogContent is DXTabControl)
                {
                    var tabControl = dialog.DialogContent as DXTabControl;
                    if (tabControl.Items != null)
                    {
                        foreach (var item in tabControl.Items)
                        {
                            var tabItem = item as DXTabItem;
                            if (tabItem == null)
                                continue;

                            if (tabItem.Content is IDisposable)
                                (tabItem.Content as IDisposable).Dispose();
                        }
                    }
                }

                dialog.Closing -= dialog_Closing;
                dialog.DialogKeepContent = false;
                try
                {
                    dialog.Close();
                }
                catch
                { }
                rootParent.Disposing -= document_Disposing;
                if (doc != rootParent)
                    doc.Disposing -= document_Disposing;
            }
        }

        void dialog_Closing(object sender, CancelEventArgs e)
        {
            var dialog = sender as GeneralDialogContent;
            if (mapEditing.ContainsKey(dialog) && mapEditing[dialog] == true)
            {
                if (result == null)
                    result = dialog.DialogResult;
                e.Cancel = true;
                Dispatcher.CurrentDispatcher.BeginInvokeAsynchronously(() =>
                {
                    dialog.Hide();
                });
            }
            else
            {
                var foundDoc = (from c in mapEditors where c.Value == dialog select c.Key).ToList();
                foreach (var doc in foundDoc)
                    RemoveEditor(doc);
            }
        }

        void document_Disposing(object sender, EventArgs e)
        {
            var doc = sender as IDocument;
            if (doc == null)
                return;
            RemoveEditor(doc);
        }
#endif
        public SessionViewModel GetSession(String sessionName)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (realTimeModel == null)
                        realTimeModel = RealTimeConnectionManagerViewModel.FindOrCreate(sessionName, HostName, AppName, EndpointUrl, false, !bIsEmbeddedRedundant);
                    if (realTimeModel != null)
                        return realTimeModel.SessionViewModel;
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

            return null;
        }

        public RealTimeConnectionManagerViewModel GetRealTimeModel()
        {
            // lock (lockObject)
            {
                return realTimeModel;
            }
        }

        public bool CreateNodeIdViewModel(String sessionName)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (NodeIdViewModel == null)
                    {
                        var session = GetSession(sessionName);
                        if (session == null)
                            return false;
                        var datetime = DateTime.Now;
                        while (!session.Connected)
                        {
                            if (datetime.AddSeconds(5) < DateTime.Now)
                                break;
#if !WINDOWS_UWP
                            Thread.Sleep(100);
#else
                            System.Threading.Tasks.Task.Delay(100).Wait();
#endif
                        }

                        NodeIdViewModel = new NodeIdViewModel(ResolvedNodeId, session);
                    }

                    return true;
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

            return false;
        }

        [DataMember]
        String savedStartingAddress;
        [DataMember]
        String savedRelativePath;
        public bool UpdatePathFromMap(Dictionary<string, string> mapItems)
        {
            if (savedStartingAddress != null)
                StartingAddress = savedStartingAddress;
            if (savedRelativePath != null)
                RelativePath = savedRelativePath;

            bool bRet = false;
            if (!String.IsNullOrEmpty(StartingAddress) && mapItems.ContainsKey(StartingAddress))
            {
                savedStartingAddress = StartingAddress;
                StartingAddress = mapItems[StartingAddress];
                ResolvedNodeId = null;
                HasResolvedNodeId = false;
                bRet = true;
            }

            if (!String.IsNullOrEmpty(RelativePath) && mapItems.ContainsKey(RelativePath))
            {
                savedRelativePath = RelativePath;
                RelativePath = mapItems[RelativePath];
                ResolvedNodeId = null;
                HasResolvedNodeId = false;
                bRet = true;
            }
            else if (!String.IsNullOrEmpty(RelativePath))
            {
                var path = NamespaceTableConverter.GetSanitizedReadableValue(RelativePath);
                var index = path.LastIndexOf('\\');
                while (index > 0)
                {
                    var key = String.Format("{0}{1}", path.Substring(0, index), NamespaceTableConverter.WholeFolderWildChar);
                    if (mapItems.ContainsKey(key))
                    {
                        var relativePath = mapItems[key];
                        var searchPath = NamespaceTableConverter.GetRelativePathValue(key);
                        var replacePath = NamespaceTableConverter.GetRelativePathValue(mapItems[key]);
                        if (searchPath != replacePath)
                            relativePath = RelativePath.Replace(searchPath, replacePath);

                        savedRelativePath = RelativePath;
                        RelativePath = relativePath;
                        ResolvedNodeId = null;
                        HasResolvedNodeId = false;
                        bRet = true;
                        break;
                    }
                    index = path.LastIndexOf('\\', index - 1);
                }
            }

            return bRet;
        }

#region Data Sink Interfaces 
        static Dictionary<String, DataSinkInterface> mapDataSinkInterfaces = new Dictionary<String, DataSinkInterface>();

        static public void RegisterDataSinkInterface(String name, DataSinkInterface data)
        {
            lock(mapDataSinkInterfaces)
            {
                if (mapDataSinkInterfaces.ContainsKey(name))
                    mapDataSinkInterfaces.Remove(name);
                mapDataSinkInterfaces.Add(name, data);
            }
        }

        static public void SetDocumentParent(IDocument parent)
        {
            lock (mapDataSinkInterfaces)
            {
                mapDataSinkInterfaces.Values.ToList().ForEach(data => data.SetDocumentParent(parent));
            }
        }

        static public void DisposingDocumentParent(IDocument parent)
        {
            lock (mapDataSinkInterfaces)
            {
                mapDataSinkInterfaces.Values.ToList().ForEach(data => data.DisposingDocumentParent(parent));
            }
        }

        static public void StartDataSinkInterfaces()
        {
            lock (mapDataSinkInterfaces)
            {
                mapDataSinkInterfaces.Values.ToList().ForEach(data => data.Start());
            }
        }

        static public void StopDataSinkInterfaces()
        {
            lock (mapDataSinkInterfaces)
            {
                mapDataSinkInterfaces.Values.ToList().ForEach(data => data.Stop());
            }
        }

        static public List<String> GetDataSinkInterfaces()
        {
            lock (mapDataSinkInterfaces)
            {
                return mapDataSinkInterfaces.Keys.ToList();
            }
        }

        static public DataSinkInterface GetDataSinkInterface(String name)
        {
            lock (mapDataSinkInterfaces)
            {
                if (mapDataSinkInterfaces.ContainsKey(name))
                    return mapDataSinkInterfaces[name];
                return null;
            }
        }

        static internal void RefreshAllDataSinkVariables()
        {
            lock (mapDataSinkInterfaces)
            {
                mapDataSinkInterfaces.Values.ToList().ForEach(data =>
                {
                    var variables = data.GetRunningVariables();
                    if (variables != null)
                        variables.ForEach(model => model.ForceValuePropertyChanges(true));
                });
            }
        }

        static public void UpdateLicenseSerialNumber()
        {
#if !DEBUG
            var sysVariables = OPCUAEntityReference.GetDataSinkInterface(SysVariables.SysNames.dataSynkName);
            if (sysVariables != null)
            {
                var serialNumber = MSZ.MSZView.GetSerialInfo(shortVersion: true);
                sysVariables.UpdateVariable(SysVariables.SysNames.LicenseSerialNumber, new DataValue(new Variant(serialNumber), StatusCodes.Good));
            }
#endif
        }
        #endregion

        bool bSubscribed;
        bool bFirstTime;
        DateTime lastTimeUsed;
        DateTime temporaryTimeout;
        bool bCheckedIsValid;

#if DEBUGTRACE
        static int currentInUse;
        static int currentToBeDisposed;
#endif
        public bool Resolve(String sessionName, IDocument parent)
        {
            return Resolve(sessionName, null, parent);
        }

        public bool Resolve(String sessionName, MonitoredItemViewModel temporary = null, IDocument parent = null)
        {
#if !WINDOWS_UWP
            if (!bCheckedIsValid && !IsValid || bDisposed)
                return false;
            bCheckedIsValid = true;
#else
            if (bDisposed)
                return false;
#endif

            lock (mapDataSinkInterfaces)
            {
                if (mapDataSinkInterfaces.ContainsKey(AppName))
                {
                    bIsDataSink = true;
                    MonitoredItemViewModel = mapDataSinkInterfaces[AppName].GetVariable(RelativePath, parent);
                    if (MonitoredItemViewModel != null)
                    {
                        OnPropertyChanged("NodeIdViewModel");
                        return true;
                    }

                    return false;
                }
            }

            if (!bFirstTime && temporary == null && MonitoredItemViewModel == null)
            {
                NormalizeHost();
                MonitoredItemViewModel = GetRealTimeMonitoredItem(sessionName);
                if (MonitoredItemViewModel != null)
                    NodeIdViewModel = GetRealTimeNodeIdItem(sessionName);
            }

            if (temporary != null && MonitoredItemViewModel == null)
            {
                bPropagatingMonitoredItemModel = true;
                try
                {
                    MonitoredItemViewModel = temporary;
                    bIsTemporary = true;
                    temporaryTimeout = DateTime.UtcNow + TimeSpan.FromSeconds(30);
                    OnPropertyChanged("NodeIdViewModel");
                }
                finally
                {
                    bPropagatingMonitoredItemModel = false;
                }
            }

            if (bIsTemporary && temporaryTimeout < DateTime.UtcNow)
            {
                MonitoredItemViewModel.DataValue = new DataValue(StatusCodes.BadResourceUnavailable);
                temporaryTimeout = DateTime.MaxValue;
            }

            if (!bFirstTime)
            {
                sessionname = sessionName;
                bFirstTime = true;
                lastTimeUsed = DateTime.UtcNow;
                PromoteIdleExecution(100, true);
                return true;
            }

            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (bIsResolvingNodeId/* || HasResolvedNodeId*/)
                    {
                        if (Interlocked.Read(ref inUseCounter) == 0)
                        {
                            // lock (lockObject)
                            {
                                if (listSubscribers != null)
                                    listSubscribers.Clear();
                                if (listUnsubscribers != null)
                                    listUnsubscribers.Clear();
                            }

                            if (bSubscribed)
                            {
                                bSubscribed = false;
                                realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                                realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                                realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                                realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;
                            }
                            return false;
                        }

                        //PromoteIdleExecution(500, true);
                        //return true;
                    }

                    if (!bIsTemporary && MonitoredItemViewModel != null)
                    {
                        ElaborateRedundancy(sessionName);
                        DiscoverConnectedEmbedded();
                        return true;
                    }

                    NormalizeHost();
#if DEBUGTRACE
                    System.Diagnostics.Debug.WriteLine(String.Format("Resolving StartingAddress {0}, RelativePath {1}, Session {2}",
                        StartingAddress, RelativePath, sessionName));
#endif
                    ElaborateRedundancy(sessionName);

                    HasResolvedNodeId = false;
                    if (realTimeModel == null && !bUseOnlyEmbeddedItems)
                        realTimeModel = RealTimeConnectionManagerViewModel.FindOrCreate(sessionName, HostName, AppName, 
                            EndpointUrl, bPublishEnabled: !bIsEmbeddedRedundant);
                    if (realTimeModel != null)
                    {
                        ElaborateRedundancy(sessionName);

                        if (bUseOnlyEmbeddedItems)
                        {
                            if (bSubscribed)
                            {
                                bSubscribed = false;
                                realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                                realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                                realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                                realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;
                            }
                            realTimeModel = null;

                            if (Interlocked.Read(ref inUseCounter) > 0)
                                PromoteIdleExecution(500, true);

                            return true;
                        }

//                        if (observerRedundancyUriArray == null)
//                        {
//#if DEBUGTRACE
//                            System.Diagnostics.Debug.WriteLine(String.Format("Observing {0}", HumanReadable));
//#endif

//                            observerRedundancyUriArray = new PropertyObserver<RealTimeConnectionManagerViewModel>(realTimeModel)
//                                .RegisterHandler(n => n.ServerUriArray,
//                                                    n =>
//                                                    {
//                                                        lock (lockObject)
//                                                        {
//                                                            if (observerRedundancyUriArray != null)
//                                                            {
//                                                                observerRedundancyUriArray.Dispose();
//                                                                observerRedundancyUriArray = null;
//                                                            }
//                                                        }
//                                                        ElaborateRedundancy(sessionName);
//                                                        PromoteIdleExecution(500, true);
//                                                    });
//                        }

                        var path = RelativePath;
                        if (String.IsNullOrEmpty(path) && !NodeId.IsNull(ResolvedNodeId))
                        {
                            if (String.IsNullOrEmpty(RelativePath))
                                path = ResolvedNodeId.ToString();
                            else
                                path = String.Format("{0}-{1}", RelativePath, ResolvedNodeId.ToString());
                        }

                        var mi = realTimeModel.FindMonitoredItem(StartingAddress, path);
                        if (mi == null && !NodeId.IsNull(ResolvedNodeId))
                        {
                            if (NodeIdViewModel != null)
                            {
                                if (!NodeIdViewModel.Exist)
                                {
                                    if (bIsTemporary)
                                    {
                                        MonitoredItemViewModel.DataValue = new DataValue(StatusCodes.BadNodeIdUnknown);
                                        bIsTemporary = false;
                                    }
                                    else
                                        MonitoredItemViewModel = new MonitoredItemViewModel() { DataValue = new DataValue(StatusCodes.BadNodeIdUnknown) };

                                    return false;
                                }

                                if (!NodeIdViewModel.IsVariable && !NodeIdViewModel.IsEventNotifier)
                                {
                                    if (bSubscribed)
                                    {
                                        bSubscribed = false;
                                        realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                                        realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                                        realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                                        realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;
                                    }
                                    return true;
                                }
                            }

                            mi = realTimeModel.FindMonitoredItem(ResolvedNodeId);
                        }

                        if (mi != null)
                        {
#if DEBUGTRACE
                            System.Diagnostics.Debug.WriteLine(String.Format("MonitoredItem Found StartingAddress {0}, RelativePath {1}, Session {2}",
                                StartingAddress, RelativePath, sessionName));
#endif

                            NodeIdViewModel = realTimeModel.FindNodeIdItem(StartingAddress, path);
                            MonitoredItemViewModel = mi;
                            if (NodeIdViewModel == null && !NodeId.IsNull(ResolvedNodeId))
                            {
                                NodeIdViewModel = realTimeModel.FindNodeIdItem(ResolvedNodeId);
                            }
                            if (NodeIdViewModel == null)
                            {
                                bIsResolvingNodeId = true;
                                if (String.IsNullOrEmpty(StartingAddress) && !NodeId.IsNull(ResolvedNodeId))
                                    realTimeModel.UpdateNodeMapRequest(StartingAddress, path, ResolvedNodeId);
                            }
                            else
                            {
                                if (bSubscribed)
                                {
                                    bSubscribed = false;
                                    realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                                    realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                                    realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                                    realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;
                                }

                                DiscoverConnectedEmbedded();
                                return true;
                            }
                        }
                        else
                        {
#if DEBUGTRACE
                            System.Diagnostics.Debug.WriteLine(String.Format("Waiting for MonitoredItem Found StartingAddress {0}, RelativePath {1}, Session {2}",
                                StartingAddress, RelativePath, sessionName));
#endif

                            if (!bSubscribed)
                            {
                                bSubscribed = true;
                                realTimeModel.MonitoringItemListchanged += realTimeModel_MonitoringItemListchanged;
                                realTimeModel.NodeIdDiscovered += realTimeModel_NodeIdDiscovered;
                                realTimeModel.BlackListChanged += realTimeModel_BlackListChanged;
                                realTimeModel.PropertyChanged += RealTimeModel_PropertyChanged;
                            }

                            if (NodeIdViewModel == null)
                                NodeIdViewModel = realTimeModel.FindNodeIdItem(StartingAddress, path);
                            if (NodeIdViewModel == null && !NodeId.IsNull(ResolvedNodeId))
                            {
                                NodeIdViewModel = realTimeModel.FindNodeIdItem(ResolvedNodeId);
                            }
                            if (NodeIdViewModel == null)
                            {
                                bIsResolvingNodeId = true;
                                if (String.IsNullOrEmpty(StartingAddress) && !NodeId.IsNull(ResolvedNodeId))
                                    realTimeModel.UpdateNodeMapRequest(StartingAddress, path, ResolvedNodeId);
                                else
                                    realTimeModel.DiscoverNodeId(StartingAddress, path);
                            }
                        }
                        // return true;
                    }
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

            if (realTimeModel != null || Interlocked.Read(ref inUseCounter) > 0)
                PromoteIdleExecution(500, true);

            return false;
        }

        private void RealTimeModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ServerUriArray")
            {
                if (realTimeModel.ServerUriArray != null && embeddedRedundantList == null)
                {
                    bIsEmbeddedRedundantElaborated = false;
                    ElaborateRedundancy(sessionname);
                    PromoteIdleExecution(500, true);
#if !NET_STANDARD
                    try
                    {
                        ApplicationPropertiesHelper.SetProperty(realTimeModel.Title, realTimeModel.ServerUriArray.ToXml());
                    }
                    catch { }
#endif
                }
            }
        }

        bool bIdleExecuting;
        protected override void IdleExecution()
        {
            if (bIsDataSink || bDisposed)
                return;

            bIdleExecuting = true;

            try
            {
                Resolve(sessionname);
                ProcessInUse();
                /*
                if (Resolve(sessionname))
                {
                    // lock (lockObject)
                    {
                        ProcessInUse();
                    }
                }
                */

                DiscoverConnectedEmbedded();
            }
            finally
            {
                bIdleExecuting = false;
            }
        }

        long inUseCounter;
        List<IEntityReference> listSubscribers;
        List<IEntityReference> listUnsubscribers;
        public bool SetInUse(IEntityReference sub, bool InUse)
        {
            lastTimeUsed = DateTime.UtcNow;

            if (bIsDataSink)
                return true;

#if DEBUGTRACE
            lock(lockObject)
            {
                if (InUse)
                    ++currentInUse;
                else
                {
                    --currentInUse;
                    ++currentToBeDisposed;
                }
            }
            System.Diagnostics.Debug.WriteLine(String.Format("SetInUse for {0}, inUse {1}, currentInUse {2}", HumanReadable, InUse, currentInUse));
#endif
            ElaborateEmbeddedInUse(sub, InUse);

            if (InUse)
            {
                Interlocked.Increment(ref inUseCounter);
                lock (lockObject)
                {
                    if (listSubscribers == null)
                        listSubscribers = new List<IEntityReference>();
                    if (!listSubscribers.Contains(sub))
                        listSubscribers.Add(sub);
                }
            }
            else
            {
                var counter = Interlocked.Decrement(ref inUseCounter);
                if (counter < 0)
                    return false;
                lock (lockObject)
                {
                    //if (counter == 0)
                    //{
                        //if (realTimeModel == null)
                        //{
                        //    OnDispose();
                        //    return false;
                        //}
                        //if (bSubscribed)
                        //{
                        //    bSubscribed = false;
                        //    realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                        //    realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                        //    realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                        //    realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;
                        //}
                    //}

                    if (listUnsubscribers == null)
                        listUnsubscribers = new List<IEntityReference>();
                    if (!listUnsubscribers.Contains(sub))
                        listUnsubscribers.Add(sub);

                    if (!bIdleExecuting && counter == 0)
                        ProcessInUse();
                }
            }

            // ProcessInUse();
            if (!String.IsNullOrEmpty(sessionname))
                PromoteIdleExecution(100, true);
            return true;
        }

        void ProcessInUse()
        {
            if (!Monitor.TryEnter(lockObject))
            {
                PromoteIdleExecution(100, true);
                return;
            }

            var listtemp1 = new List<IEntityReference>();
            var listtemp2 = new List<IEntityReference>();
            var rtm = realTimeModel;
            var nid = NodeIdViewModel;
            NodeId nodeId = null;
            if (nid != null)
                nodeId = (NodeId)nid.nodeId;

            try
            {
                if (rtm == null || nodeId == null)
                {
                    if (Interlocked.Read(ref inUseCounter) <= 0)
                    {
                        if (lastTimeUsed.AddSeconds(10) < DateTime.UtcNow)
                            OnDispose();
                        else
                            PromoteIdleExecution(100, true);
                    }
                    else
                        PromoteIdleExecution(500, true);
                    return;
                }

                var listMatch = new List<IEntityReference>();
                if (listUnsubscribers != null && listSubscribers != null)
                {
                    foreach (var sub in listUnsubscribers)
                    {
                        if (listSubscribers.Contains(sub))
                        {
                            listMatch.Add(sub);
                        }
                    }

                    listMatch.ForEach(c =>
                        {
                            listUnsubscribers.Remove(c);
                            listSubscribers.Remove(c);
                        });
                }

                if (listSubscribers != null)
                {
                    listtemp1.AddRange(listSubscribers);
                    listSubscribers.Clear();
                }

                if (listUnsubscribers != null)
                {
                    listtemp2.AddRange(listUnsubscribers);
                    listUnsubscribers.Clear();
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            if (rtm != null && nodeId != null && !bUseOnlyEmbeddedItems)
            {
                listtemp1.ForEach(sub =>
                {
                    rtm.SetInUse(nodeId, true, !bIsEmbeddedRedundant ? sub : null);
                });

                listtemp2.ForEach(sub =>
                {
                    rtm.SetInUse(nodeId, false, !bIsEmbeddedRedundant ? sub : null);
                });
            }

            if (Interlocked.Read(ref inUseCounter) <= 0)
            {
                if (lastTimeUsed.AddSeconds(10) < DateTime.UtcNow)
                    OnDispose();
                else
                    PromoteIdleExecution(100, true);
            }
            else
                DiscoverConnectedEmbedded();
        }

        public void UpdateNodeIds()
        {
            if (realTimeModel != null)
                return;

            ResolvedStartingNodeId = realTimeModel.GetNodeId(StartingAddress);
            ResolvedNodeId = realTimeModel.GetNodeId(RelativePath);
            HasResolvedNodeId = true;
        }

        void realTimeModel_MonitoringItemListchanged(object sender, MonitoreItemListChangedEventArgs e)
        {
            if (bDisposed || sender != realTimeModel)
                return;
            if (NodeId.IsNull(ResolvedNodeId))
                UpdateNodeIds();
            if (NodeId.IsNull(ResolvedNodeId))
                return;

            if (e.HasMonitoredItem(resolvedNodeId))
            {
#if DEBUGTRACE
                System.Diagnostics.Debug.WriteLine(String.Format("Received MonitoredItem FOUND StartingAddress {0}, RelativePath {1}, Session {2}",
                    StartingAddress, RelativePath, sessionname));
#endif

                MonitoredItemViewModel = e.GetMonitoredItemViewModel(ResolvedNodeId);
                ProcessInUse();
            }
#if DEBUGTRACE
            else
                System.Diagnostics.Debug.WriteLine(String.Format("Received MonitoredItem NOTFOUND StartingAddress {0}, RelativePath {1}, Session {2}",
                    StartingAddress, RelativePath, sessionname));
#endif
        }

        void realTimeModel_BlackListChanged(object sender, BlackListChangedEventArgs e)
        {
            if (bDisposed)
                return;
            if (e.IsInstanceDiscovering && e.IsOnBlackList(StartingAddress))
            {
                lastMessage = String.Format(Properties.Resource.NodeNotFoundOnServer, StartingAddress);
                //ResolvedStartingNodeId = null;
                //StartingNodeIdViewModel = null;
            }
            else if (!String.IsNullOrEmpty(RelativePath) && e.IsOnBlackList(RelativePath))
            {
                lastMessage = String.Format(Properties.Resource.NodeNotFoundOnServer, RelativePath);
                //ResolvedNodeId = null;
                //NodeIdViewModel = null;
            }
        }

        void realTimeModel_NodeIdDiscovered(object sender, NodeIdDiscoveredEventArgs e)
        {
            if (bDisposed)
                return;

            if (e.IsInstanceDiscovering && e.HasNodeId(StartingAddress))
            {
                ResolvedStartingNodeId = e.GetNodeId(StartingAddress);
                StartingNodeIdViewModel = e.GetNodeIdViewModel(StartingAddress);
            }
            else if (ResolvedNodeId != null && e.HasNodeId(ResolvedNodeId))
            {
                NodeIdViewModel = e.GetNodeIdViewModel(ResolvedNodeId);
            }
            else if (!String.IsNullOrEmpty(RelativePath) && e.HasNodeId(RelativePath))
            {
                ResolvedNodeId = e.GetNodeId(RelativePath);
                NodeIdViewModel = e.GetNodeIdViewModel(RelativePath);
            }
            else if (e.HasNodeId(RealTimeConnectionManagerViewModel.GetItemInstanceString(RelativePath, StartingAddress)))
            {
                ResolvedNodeId = e.GetNodeId(RealTimeConnectionManagerViewModel.GetItemInstanceString(RelativePath, StartingAddress));
                NodeIdViewModel = e.GetNodeIdViewModel(RealTimeConnectionManagerViewModel.GetItemInstanceString(RelativePath, StartingAddress));
            }

            if (ResolvedNodeId != null/* && (ResolvedStartingNodeId != null || StartingAddress == null)*/)
            {
                bIsResolvingNodeId = false;
                HasResolvedNodeId = true;
                ProcessInUse();
            }
        }

        public void ReplaceHostName(String replaceFrom, String replaceTo)
        {
            if (String.IsNullOrEmpty(replaceFrom))
            {
                if (!String.IsNullOrEmpty(HostName))
                {
                    HostName = replaceTo;
                }

                try
                {
                    var uri = new UriBuilder(EndpointUrl) { Host = replaceTo };
                    EndpointUrl = uri.Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                if (!String.IsNullOrEmpty(HostName))
                {
                    HostName = HostName.Replace(replaceFrom, replaceTo);
                }

                try
                {
                    var uri = new UriBuilder(EndpointUrl);
                    uri.Host = uri.Host.Replace(replaceFrom, replaceTo);
                    EndpointUrl = uri.Uri.AbsoluteUri;
                }
                catch (Exception ex)
                {

                }
            }
        }

        MonitoredItemViewModel GetRealTimeMonitoredItem(String sessionName)
        {
            if (realTimeModel == null && !bUseOnlyEmbeddedItems)
                realTimeModel = RealTimeConnectionManagerViewModel.Find(sessionName, HostName, AppName, EndpointUrl);
            if (realTimeModel != null)
            {
                var path = RelativePath;
                if (String.IsNullOrEmpty(path) && !NodeId.IsNull(ResolvedNodeId))
                    path = ResolvedNodeId.ToString();

                var mi = realTimeModel.FindMonitoredItem(StartingAddress, path);
                if (mi == null && !NodeId.IsNull(ResolvedNodeId))
                    mi = realTimeModel.FindMonitoredItem(ResolvedNodeId);
                return mi;
            }

            return null;
        }

        NodeIdViewModel GetRealTimeNodeIdItem(String sessionName)
        {
            if (realTimeModel == null && !bUseOnlyEmbeddedItems)
                realTimeModel = RealTimeConnectionManagerViewModel.Find(sessionName, HostName, AppName, EndpointUrl);
            if (realTimeModel != null)
            {
                var path = RelativePath;
                if (String.IsNullOrEmpty(path) && !NodeId.IsNull(ResolvedNodeId))
                    path = ResolvedNodeId.ToString();

                var nodeId = realTimeModel.FindNodeIdItem(StartingAddress, path);
                if (nodeId == null && !NodeId.IsNull(ResolvedNodeId))
                    nodeId = realTimeModel.FindNodeIdItem(ResolvedNodeId);
                return nodeId;
            }

            return null;
        }

#endregion

#region Properties

        public String Name { get; set; }
        public bool IsLocalVariable
        {
            get
            {
                return bIsDataSink;
            }
        }
#if !WINDOWS_UWP && !NET_STANDARD
        public IUFUAEditorManager Editor { get; set; }
#endif
        public IDocument Document { get; set; }

        [DataMember]
        String hostName;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                if (hostName == value)
                    return;
                hostName = value;
                OnPropertyChanged("HostName");
            }
        }

        [DataMember]
        String appName;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String AppName
        {
            get
            {
                if (appName != null)
                    return appName.Trim();
                
                return null;
            }
            set
            {
                if (appName == value)
                    return;
                appName = value;
                OnPropertyChanged("AppName");
            }
        }

        [DataMember]
        String endpointUrl;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String EndpointUrl
        {
            get
            {
                if (endpointUrl != null)
                    return endpointUrl.Trim();

                return null;
            }
            set
            {
                if (endpointUrl == value)
                    return;
                endpointUrl = value;
                OnPropertyChanged("EndpointUrl");
            }
        }

        [DataMember]
        String relativePath;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String RelativePath
        {
            get
            {
                return relativePath;
            }
            set
            {
                if (relativePath == value)
                    return;
                relativePath = value;
                OnPropertyChanged("RelativePath");
            }
        }

        [DataMember]
        String readablPath;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ReadablePath
        {
            get
            {
                return readablPath;
            }
            set
            {
                if (readablPath == value)
                    return;
                readablPath = value;
                OnPropertyChanged("ReadablePath");
            }
        }

        [DataMember]
        String startingAddress;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String StartingAddress
        {
            get
            {
                return startingAddress;
            }
            set
            {
                if (startingAddress == value)
                    return;
                startingAddress = value;
                OnPropertyChanged("StartingAddress");
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsRelative
        {
            get
            {
                return !string.IsNullOrEmpty(StartingAddress);
            }
        }

        [DataMember]
        bool isRedundancyEnabled = true;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsRedundancyEnabled
        {
            get
            {
                return isRedundancyEnabled;
            }
            set
            {
                if (isRedundancyEnabled == value)
                    return;

                isRedundancyEnabled = value;
                OnPropertyChanged("IsRedundancyEnabled");
            }
        }

        [DataMember]
        NodeId resolvedNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public NodeId ResolvedNodeId
        {
            get
            {
                return resolvedNodeId;
            }
            set
            {
                if (resolvedNodeId == value)
                    return;
                resolvedNodeId = value;
                OnPropertyChanged("ResolvedNodeId");
            }
        }

        [DataMember]
        NodeId resolvedStartingNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public NodeId ResolvedStartingNodeId
        {
            get
            {
                return resolvedStartingNodeId;
            }
            set
            {
                if (resolvedStartingNodeId == value)
                    return;
                resolvedStartingNodeId = value;
                OnPropertyChanged("ResolvedStartingNodeId");
            }
        }

        [DataMember]
        NodeId parentNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public NodeId ParentNodeId
        {
            get
            {
                return parentNodeId;
            }
            set
            {
                if (parentNodeId == value)
                    return;
                parentNodeId = value;
                OnPropertyChanged("ParentNodeId");
            }
        }

        [DataMember]
        ExpandedNodeId typeDefinitionNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public ExpandedNodeId TypeDefinitionNodeId
        {
            get
            {
                return typeDefinitionNodeId;
            }
            set
            {
                if (typeDefinitionNodeId == value)
                    return;
                typeDefinitionNodeId = value;
                OnPropertyChanged("TypeDefinitionNodeId");
            }
        }

        
        [DataMember]
        String typeDefinitionName;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String TypeDefinitionName
        {
            get
            {
                return typeDefinitionName;
            }
            set
            {
                if (typeDefinitionName == value)
                    return;
                typeDefinitionName = value;
                OnPropertyChanged("TypeDefinitionName");
            }
        }


        [DataMember]
        ExpandedNodeId parentTypeDefinitionNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public ExpandedNodeId ParentTypeDefinitionNodeId
        {
            get
            {
                return parentTypeDefinitionNodeId;
            }
            set
            {
                if (parentTypeDefinitionNodeId == value)
                    return;
                parentTypeDefinitionNodeId = value;
                OnPropertyChanged("ParentTypeDefinitionNodeId");
            }
        }


        [DataMember]
        String parentTypeDefinitionName;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ParentTypeDefinitionName
        {
            get
            {
                return parentTypeDefinitionName;
            }
            set
            {
                if (parentTypeDefinitionName == value)
                    return;
                parentTypeDefinitionName = value;
                OnPropertyChanged("ParentTypeDefinitionName");
            }
        }

        [DataMember]
        List<String> parentTypeDefinitionNameList;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public List<String> ParentTypeDefinitionNameList
        {
            get
            {
                return parentTypeDefinitionNameList;
            }
            set
            {
                if (parentTypeDefinitionNameList == value)
                    return;
                parentTypeDefinitionNameList = value;
                OnPropertyChanged("ParentTypeDefinitionNameList");
            }
        }

        [DataMember]
        List<ExpandedNodeId> parentTypeDefinitionIdList;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public List<ExpandedNodeId> ParentTypeDefinitionIdList
        {
            get
            {
                return parentTypeDefinitionIdList;
            }
            set
            {
                if (parentTypeDefinitionIdList == value)
                    return;
                parentTypeDefinitionIdList = value;
                OnPropertyChanged("ParentTypeDefinitionIdList");
            }
        }

        [DataMember]
        bool resolvedItem;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool ResolvedItem
        {
            get
            {
                return resolvedItem;
            }
            set
            {
                if (resolvedItem == value)
                    return;
                resolvedItem = value;
                OnPropertyChanged("ResolvedItem");
            }
        }

        public bool HasValidValue
        {
            get
            {
                return !String.IsNullOrEmpty(HumanReadable) && HumanReadable != Properties.Resource.NotSetYet;
            }
        }

        [DataMember]
        String humanReadable;
        public String HumanReadable
        {
            get
            {
                return String.IsNullOrEmpty(humanReadable) ? Properties.Resource.NotSetYet : humanReadable;
            }
            set
            {
                if (humanReadable == value)
                    return;
                humanReadable = value;
                OnPropertyChanged("HumanReadable");
            }
        }

        public String HumanReadableNoProject
        {
            get
            {
                if (String.IsNullOrEmpty(humanReadable))
                    return String.Empty;
                var splitted = humanReadable.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries);
                return splitted[0];
            }
        }

        public String StringRepresentation
        {
            get
            {
                return TagPathHelper.GetTagPath(HumanReadableNoProject, ReadablePath);
            }
        }

        public String StringRepresentationWithProject
        {
            get
            {
                if (String.IsNullOrEmpty(StringRepresentation) || String.IsNullOrEmpty(humanReadable))
                    return String.Empty;
                var splitted = humanReadable.Split(new String[] { " (" }, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Length == 0)
                    return String.Empty;
                return splitted.Length == 1 ? StringRepresentation : String.Format("{0} ({1}", StringRepresentation, splitted[1]);
            }
        }

        MonitoredItemViewModel monitoredItemViewModel;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public MonitoredItemViewModel MonitoredItemViewModel
        {
            get
            {
                return monitoredItemViewModel;
            }
            private set
            {
                if (monitoredItemViewModel == value)
                    return;
                bIsTemporary = false;
                if (embeddedMonitoredItemViewModel == null && !bPropagatingMonitoredItemModel)
                {
                    embeddedMonitoredItemViewModel = value;
                    bForceDiscoverConnectedEmbedded = true;
                }
                monitoredItemViewModel = value;
                if (monitoredItemViewModel != null)
                    monitoredItemViewModel.EndpointUrl = EndpointUrl;
                OnPropertyChanged("MonitoredItemViewModel");
                PromoteIdleExecution(500, true);
            }
        }

        NodeIdViewModel startingNodeIdViewModel;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public NodeIdViewModel StartingNodeIdViewModel
        {
            get
            {
                return startingNodeIdViewModel;
            }
            private set
            {
                if (startingNodeIdViewModel == value)
                    return;
                if (startingNodeIdViewModel != null)
                    startingNodeIdViewModel.Dispose();
                if (embeddedStartingNodeIdViewModel == null && !bPropagatingMonitoredItemModel)
                {
                    embeddedStartingNodeIdViewModel = value;
                    bForceDiscoverConnectedEmbedded = true;
                }
                startingNodeIdViewModel = value;
                OnPropertyChanged("StartingNodeIdViewModel");
            }
        }

        NodeIdViewModel nodeIdViewModel;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public NodeIdViewModel NodeIdViewModel
        {
            get
            {
                return nodeIdViewModel;
            }
            internal set
            {
                lock (lockObject)
                {
                    if (nodeIdViewModel == value)
                        return;
                    if (nodeIdViewModel != null)
                        nodeIdViewModel.Dispose();
                    if (embeddedNodeIdViewModel == null && !bPropagatingMonitoredItemModel)
                    {
                        embeddedNodeIdViewModel = value;
                        bForceDiscoverConnectedEmbedded = true;
                    }
                    nodeIdViewModel = value;

                    if (nodeIdViewModel != null && String.IsNullOrEmpty(HumanReadable))
                        HumanReadable = nodeIdViewModel.DisplayName.ToString();
                    if (String.IsNullOrEmpty(ReadablePath) && nodeIdViewModel != null && !String.IsNullOrEmpty(nodeIdViewModel.ReadablePath))
                        ReadablePath = nodeIdViewModel.ReadablePath;
                }
                OnPropertyChanged("NodeIdViewModel");
            }
        }

        bool bIsResolvingNodeId;
        bool bResolvedNodeId;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool HasResolvedNodeId
        {
            get
            {
                return bResolvedNodeId;
            }
            set
            {
                if (value == bResolvedNodeId)
                    return;
                bResolvedNodeId = value;
                OnPropertyChanged("HasResolvedNodeId");
            }

        }

        String lastMessage;
        public String LastMessage
        {
            get
            {
                if (!String.IsNullOrEmpty(lastMessage))
                    return lastMessage;

                if (realTimeModel != null)
                {
                    if (realTimeModel.SessionViewModel != null)
                        return realTimeModel.SessionViewModel.LastMessage;
                    else
                        return realTimeModel.LastMessage;
                }

                return String.Format(Properties.Resource.SearchingFor, HumanReadable);
            }
        }

        public static string CompletePathToHumanReadable(string completePath)
        {
            string hr = "";
            NamespaceTable n = new Opc.Ua.NamespaceTable();
            UInt16 ns = (UInt16)(n.Count + 2 - 1);
            string oldChars = string.Format("{0}:", ns);
            if (!String.IsNullOrEmpty(completePath))
            {
                hr = completePath.Replace(oldChars, "").Replace("/", "\\");
                var tags = "\\Tags\\";
                if (hr.StartsWith(tags))
                    hr = hr.Substring(tags.Length);
            }
            return hr;
        }

#endregion

#if !WINDOWS_UWP && !NET_STANDARD
#region ICloneable Members

        public object Clone()
        {
            return new OPCUAEntityReference(this);
        }

#endregion
#endif

#region IDisposable Members
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This class object cannot be disposed because is disposed internally.", true)]
        public new void Dispose()
        { }

        bool bDisposed;
        protected override void OnDispose()
        {
            if (bDisposed)
                return;
#if DEBUGTRACE
            --currentToBeDisposed;
            System.Diagnostics.Debug.WriteLine(String.Format("Disposing {0}, currentDisposed {1}", HumanReadable, currentToBeDisposed));
#endif

            bDisposed = true;
            base.OnDispose();
            lock (lockObject)
            {
                //if (observerRedundancyUriArray != null)
                //{
                //    observerRedundancyUriArray.Dispose();
                //    observerRedundancyUriArray = null;
                //}

                //if (observerSessionConnected != null)
                //{
                //    observerSessionConnected.Dispose();
                //    observerSessionConnected = null;
                //}
                if (embeddedRealTimeModel != null &&
                    embeddedRealTimeModel.SessionViewModel != null)
                {
                    bIsEmbeddedRealTimeModelConnected = false;
                    embeddedRealTimeModel.SessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
                    embeddedRealTimeModel = null;
                }

                if (realTimeModel != null)
                {
                    // ProcessInUse();
                    bSubscribed = false;
                    realTimeModel.MonitoringItemListchanged -= realTimeModel_MonitoringItemListchanged;
                    realTimeModel.NodeIdDiscovered -= realTimeModel_NodeIdDiscovered;
                    realTimeModel.BlackListChanged -= realTimeModel_BlackListChanged;
                    realTimeModel.PropertyChanged -= RealTimeModel_PropertyChanged;

                    realTimeModel = null;
                }

                if (listSubscribers != null)
                    listSubscribers.Clear();

                if (listUnsubscribers != null)
                    listUnsubscribers.Clear();

                if (embeddedRedundantList != null)
                {
                    embeddedRedundantList.ForEach(el => el.OnDispose());
                    embeddedRedundantList = null;
                }

                if (embeddedNodeIdViewModel != null)
                {
                    embeddedNodeIdViewModel.Dispose();
                    embeddedNodeIdViewModel = null;
                }

                if (nodeIdViewModel != null)
                {
                    nodeIdViewModel.Dispose();
                    nodeIdViewModel = null;
                }

                if (startingNodeIdViewModel != null)
                {
                    startingNodeIdViewModel.Dispose();
                    startingNodeIdViewModel = null;
                }

                if (embeddedStartingNodeIdViewModel != null)
                {
                    embeddedStartingNodeIdViewModel.Dispose();
                    embeddedStartingNodeIdViewModel = null;
                }
            }
        }

#endregion

#if !WINDOWS_UWP
#region Validations
        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "AppName")
            {
                if (String.IsNullOrEmpty(AppName))
                    return Properties.Resource.AppName_Invalid;
            }
            else if (propertyName == "EndpointUrl")
            {
                if (String.IsNullOrEmpty(EndpointUrl) && String.IsNullOrEmpty(AppName))
                    return Properties.Resource.EndpointUrl_Invalid;
            }
            else if (propertyName == "RelativePath")
            {
                if (String.IsNullOrEmpty(RelativePath) && NodeId.IsNull(ResolvedNodeId))
                    return Properties.Resource.RelativePath_Invalid;
            }

            return base.PerformValidation(propertyName);
        }
#endregion
#endif

#region Override Methods
        public override string ToString()
        {
            if (!String.IsNullOrEmpty(HumanReadableNoProject))
                return HumanReadableNoProject;
            
            return base.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (!String.IsNullOrEmpty(ReadablePath))
                return ReadablePath;

            return base.ToString();
        }

#endregion

#region Redundancy
        void ElaborateRedundancy(String sessionName)
        {
            if (!IsRedundancyEnabled || bIsEmbeddedRedundant || bIsEmbeddedRedundantElaborated || String.IsNullOrEmpty(sessionName))
                return;
            
            var sessionSettings = RealTimeConnectionManagerViewModel.GetSession(sessionName);
            String[] serverArray = null;
            if (sessionSettings.ServerArray != null)
            {
                bIsEmbeddedRedundantElaborated = true;
                serverArray = sessionSettings.ServerArray;
            }
            else if (realTimeModel != null)
            {
                bIsEmbeddedRedundantElaborated = true;
                if (realTimeModel.ServerUriArray != null)
                    serverArray = realTimeModel.ServerUriArray;
#if !NET_STANDARD
                else
                {
                    try
                    {
                        var array = ApplicationPropertiesHelper.GetProperty<String>(realTimeModel.Title);
                        serverArray = array.FromXml<String[]>();
                    }
                    catch { }
                }
#endif
            }

            if (serverArray == null)
                return;
#if !WINDOWS_UWP
            var hostName = System.Net.Dns.GetHostName().ToLower();
#else
            var hostName = Utilities.NetworkHelpers.GetHostName().ToLower();
#endif

            bool bFoundHostName = false;
            var list = (from c in serverArray.AsParallel() select c.ToLower()).ToList();
            if (list.Contains(hostName))
            {
                bFoundHostName = true;
                list.Remove(hostName);
            }
            serverArray = list.ToArray();
            if (serverArray.Length == 0)
                return;

            if (!bFoundHostName)
                bUseOnlyEmbeddedItems = true;

            lock (lockObject)
            {
                embeddedRedundantList = new List<OPCUAEntityReference>();
                for (int i = 0; i < serverArray.Length; ++i)
                {
                    var entity = new OPCUAEntityReference(this);
                    entity.HostName = serverArray[i];
                    entity.embeddedRedundantHostName = entity.HostName;
                    entity.bIsEmbeddedRedundant = true;
                    embeddedRedundantList.Add(entity);
                    entity.Resolve(sessionName);

                    if (listSubscribers != null)
                        listSubscribers.ForEach(entityReference => entity.SetInUse(entityReference, true));
                }
            }
        }

        void ElaborateEmbeddedInUse(IEntityReference sub, bool InUse)
        {
            lock (lockObject)
            {
                if (!IsRedundancyEnabled || bIsEmbeddedRedundant || !bIsEmbeddedRedundantElaborated || embeddedRedundantList == null)
                    return;
                embeddedRedundantList.ForEach(el => el.SetInUse(sub, InUse));
            }
        }

        void DiscoverConnectedEmbedded()
        {
            lock (lockObject)
            {
                if (!IsRedundancyEnabled || bIsEmbeddedRedundant || (bIsEmbeddedRealTimeModelConnected && !bForceDiscoverConnectedEmbedded) || !bIsEmbeddedRedundantElaborated || embeddedRedundantList == null)
                    return;
                bForceDiscoverConnectedEmbedded = false;
                if (Interlocked.Read(ref inUseCounter) <= 0)
                {
                    // PromoteIdleExecution(500, true);
                    return;
                }

                var list = new List<OPCUAEntityReference>(embeddedRedundantList);
                list.Add(this);
                var found = (from c in list
                             where c.realTimeModel != null && c.realTimeModel.SessionViewModel != null &&
                             c.realTimeModel.SessionViewModel.Connected && (c.MonitoredItemViewModel != null || c.NodeIdViewModel != null && (c.NodeIdViewModel.IsMethod || c.NodeIdViewModel.IsObject))
                             orderby c.realTimeModel.ServiceLevel descending
                             select c).ToList();
                if (found.Count > 0)
                {
                    var local = (from c in found where c.realTimeModel.HostName == null && c.realTimeModel.ServiceLevel > 127 select c).ToList();
                    if (local.Count > 0)
                    {
                        found.Clear();
                        found.AddRange(local);
                    }
                    RealTimeConnectionManagerViewModel.NotifyRedundancyHostNameUsed(
                        !String.IsNullOrEmpty(found[0].HostName) ? found[0].HostName : dnsHostName);
                    bPropagatingMonitoredItemModel = true;
                    try
                    {
                        if (found[0] == this)
                        {
                            if (embeddedRealTimeModel != null)
                            {
                                embeddedRealTimeModel.SetPublishingEnabled(false);

                                bIsEmbeddedRealTimeModelConnected = false;
                                if (embeddedRealTimeModel.SessionViewModel != null)
                                    embeddedRealTimeModel.SessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
                            }
                            realTimeModel.SetPublishingEnabled(true);
                            embeddedRealTimeModel = realTimeModel;

                            if (embeddedNodeIdViewModel != null)
                                NodeIdViewModel = embeddedNodeIdViewModel;
                            if (embeddedMonitoredItemViewModel != null)
                                MonitoredItemViewModel = embeddedMonitoredItemViewModel;
                            if (embeddedStartingNodeIdViewModel != null)
                                StartingNodeIdViewModel = embeddedStartingNodeIdViewModel;
                        }
                        else
                        {
                            if (embeddedRealTimeModel != null)
                            {
                                embeddedRealTimeModel.SetPublishingEnabled(false);

                                bIsEmbeddedRealTimeModelConnected = false;
                                if (embeddedRealTimeModel.SessionViewModel != null)
                                    embeddedRealTimeModel.SessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
                            }

                            found[0].realTimeModel.SetPublishingEnabled(true);
                            embeddedRealTimeModel = found[0].realTimeModel;

                            NodeIdViewModel = found[0].NodeIdViewModel;
                            MonitoredItemViewModel = found[0].MonitoredItemViewModel;
                            StartingNodeIdViewModel = found[0].StartingNodeIdViewModel;
                        }
                    }
                    finally
                    {
                        bPropagatingMonitoredItemModel = false;
                    }
                }
            }

            if ((MonitoredItemViewModel == null && (NodeIdViewModel == null || (!NodeIdViewModel.IsMethod && !NodeIdViewModel.IsObject))) || 
                embeddedRealTimeModel == null || embeddedRealTimeModel.SessionViewModel == null || !embeddedRealTimeModel.SessionViewModel.Connected)
                PromoteIdleExecution(500, true);
            else if (!bIsEmbeddedRealTimeModelConnected)
            {
                bIsEmbeddedRealTimeModelConnected = true;
                embeddedRealTimeModel.SessionViewModel.PropertyChanged += SessionViewModel_PropertyChanged;
            //    if (observerSessionConnected != null)
            //        observerSessionConnected.Dispose();
            //    observerSessionConnected = new PropertyObserver<SessionViewModel>(embeddedRealTimeModel.SessionViewModel)
            //        .RegisterHandler(n => n.Connected,
            //                            n =>
            //                            {
            //                                if (!n.Connected)
            //                                {
            //                                    if (embeddedRealTimeModel != null)
            //                                    {
            //                                        embeddedRealTimeModel.SetPublishingEnabled(false);
            //                                        embeddedRealTimeModel = null;
            //                                    }
            //                                    PromoteIdleExecution(500, true);
            //                                }
            //                            });
            }
        }

        private void SessionViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected")
            {
                lock (lockObject)
                {
                    if (embeddedRealTimeModel != null &&
                        embeddedRealTimeModel.SessionViewModel != null && !embeddedRealTimeModel.SessionViewModel.Connected)
                    {
                        embeddedRealTimeModel.SetPublishingEnabled(false);

                        bIsEmbeddedRealTimeModelConnected = false;
                        embeddedRealTimeModel.SessionViewModel.PropertyChanged -= SessionViewModel_PropertyChanged;
                        embeddedRealTimeModel = null;
                        PromoteIdleExecution(500, true);
                    }
                }
            }
        }

#endregion
    }
}
