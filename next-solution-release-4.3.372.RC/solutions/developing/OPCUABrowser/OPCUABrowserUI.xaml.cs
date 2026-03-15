using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;
using OPCUAViewModel;
using System.Collections.Specialized;
using ViewModelLib;
using System.Windows;
using NetAPIDiscovery;
using System.Windows.Input;
using Tracing.ComponentService;
using UFInterfaces;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OPCUAEventViewers;
using Utilities;
using Utilities.WPF;
using System.IO.IsolatedStorage;
using Opc.Ua;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading.Tasks;
using OPCUABrowser.ComponentService;
using System.Threading;
using System.DirectoryServices.AccountManagement;
using UIMsgBoxAlertService.ComponentService;
using Utilities.ProgressDialog;
using WPFUtilities;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Grid;
using System.Windows.Data;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using WPFUtilities.PropertyDataTemplate;

namespace OPCUABrowser
{
    /// <summary>
    /// Interaction logic for OPCUABrowserUI.xaml
    /// </summary>
    public partial class OPCUABrowserUI : UserControl, IDisposable
    {
        OPCUADiscoveryViewModel DiscoveryViewModel;
        Dictionary<TreeViewItemViewModel, TreeListNodeCollection> mapModelList = new Dictionary<TreeViewItemViewModel, TreeListNodeCollection>();
        Dictionary<TreeViewItemViewModel, TreeListNode> mapModelItem = new Dictionary<TreeViewItemViewModel, TreeListNode>();

        Dictionary<String, OPCUABrowseServer> mapOpenBrowseServer = new Dictionary<String, OPCUABrowseServer>();
        Dictionary<String, OPCUABrowseServer> mapOpenAreaBrowseServer = new Dictionary<String, OPCUABrowseServer>();

        SafeObservableCollection<EndpointDescriptionViewModel> listRecentEndpoints = new SafeObservableCollection<EndpointDescriptionViewModel>();

        bool isEmbedded = false;
        bool allowMultiSelection;
        OPCUABrowserComponent component;
        IWorkspace workspace;
        ISimpleLogging simpleLogging;
        DockLayoutManager innerDockingManager;
        static Object staticTreeLock = new Object();

        public DockLayoutManager InnerDockingManager
        {
            get
            {
                return innerDockingManager;
            }
            set
            {
                if (innerDockingManager == value)
                    return;
                if (innerDockingManager != null)
                    innerDockingManager.DockItemClosed -= innerDockingManager_CloseButtonClick;

                innerDockingManager = value;
                innerDockingManager.DockItemClosed += innerDockingManager_CloseButtonClick;

                if (DiscoveryViewModel == null)
                {
                    DiscoveryViewModel = new OPCUADiscoveryViewModel(Dispatcher.CurrentDispatcher, null);

                    if (DiscoveryViewModel.NetworkViewModel.Children.Count > 0)
                    {
                        foreach (var v in DiscoveryViewModel.NetworkViewModel.Children)
                            AddTreeItem(v, null);
                    }

                    DiscoveryViewModel.NetworkViewModel.Children.CollectionChanged += CollectionChanged;

                    //DiscoveryViewModel.networkViewModel.RestartDiscovery();

                    workspace_ContentRendered(null, null);
                }
            }
        }

        static String GetTitle(EndpointDescriptionViewModel epd)
        {
            if (epd.endpointDescription.Server.ApplicationName == null)
                return epd.DisplayTitle;

            //ret string contains also the EndpointUrl in order to have potentially 2 windows
            //in OPC UA browser connected to different servers having the same project loaded
            var uri = Utils.ParseUri(epd.EndpointUrl);
            string host = "";
            if (uri != null)
                host = Utils.ParseUri(epd.EndpointUrl).Host;
            var ret = String.Format("{0}_{1} - {2}",
                epd.endpointDescription.Server.ApplicationName.ToString(),
                epd.DisplayTitle, host);
            return ret ;
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (NeedToBeExpanded(e.Node))
            {
                ClearDummyNode(e.Node);
                var treeListView = sender as TreeListView;
                var treeListControl = treeListView.FindParent<TreeListControl>() as TreeListControl;
                if (e.Node.Nodes.Count == 0)
                    treeListControl.AddNode(null, e.Node, TreeListControlHelper.DummyNode);
                if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                    UpdateFolderIcon(e.Node, false);
            }
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            if (e == null)
                return;

            TreeListNode item = e.Node as TreeListNode;
            var treeListView = sender as TreeListView;
            var treeListControl = treeListView.FindParent<TreeListControl>() as TreeListControl;
            if (bExpandingPending || item == null || TreeListControlHelper.WasExpanded(item) || !(item.Tag is TreeViewItemViewModel))
                return;

            if (!TreeListControlHelper.CanBeExpanded(item))
            {
                if (item.Tag is IEntityReference)
                    UpdateFolderIcon(item, true);
                return;
            }

            e.Handled = true;
            bExpandingPending = true;

            //lock (staticTreeLock)
            {
                treeListControl.BeginDataUpdate();
                try
                {
                    item.Nodes.Clear();
                    TreeViewItemViewModel dvm = item.Tag as TreeViewItemViewModel;
                    if (dvm is WorkstationViewModel)
                    {
                        DiscoveryViewModel.DiscoverHostNameNow(dvm.Title);
                        using (new WaitCursor(this))
                        {
                            while (!DiscoveryViewModel.IsHostDiscovered(dvm.Title) ||
                                DiscoveryViewModel.IsHostDiscovering(dvm.Title) ||
                                DiscoveryViewModel.IsHostDiscoveringQueue(dvm.Title))
                                WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, null);

                            foreach (TreeViewItemViewModel advm in DiscoveryViewModel.Children)
                            {
                                if (advm is ApplicationDescriptionViewModel &&
                                    (advm as ApplicationDescriptionViewModel).Workstation == dvm.Title)
                                {
                                    AddTreeItem(advm, item);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!dvm.BackgroundWorkProcessed)
                        {
                            using (new WaitCursor(this))
                            {
                                while (!dvm.BackgroundWorkProcessed)
                                    WaitForPriority.Wait(DispatcherPriority.ApplicationIdle, null);
                            }
                        }

                        using (new WaitCursor(this))
                        {
                            Parallel.ForEach(dvm.Children, advm =>
                            {
                                Dispatcher.BeginInvokeIfRequired(() =>
                                {
                                    AddTreeItem(advm, item);
                                });
                            });
                            //foreach (TreeViewItemViewModel advm in dvm.Children)
                            //{
                            //    AddTreeItem(advm, item);
                            //}
                        }
                    }
                }
                finally
                {
                    bExpandingPending = false;
                    treeListControl.EndDataUpdate();
                }
            }
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && (node.Tag is IEntityReference))
            {
                var collapsedImage = (node.Tag as IEntityReference).CollapsedImageSource;
                var expandedImage = (node.Tag as IEntityReference).ExpandedImageSource;
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? expandedImage == null ? collapsedImage : expandedImage : collapsedImage;
            }
        }

        public OPCUABrowserUI(OPCUABrowserComponent c, bool bIsEmbedded = false, bool allowMultiSelection = false)
        {
            isEmbedded = bIsEmbedded;
            this.allowMultiSelection = allowMultiSelection;
            component = c;
            if (!bIsEmbedded)
                workspace = c.Workspace;
            simpleLogging = c.SimpleLogging;

            InitializeComponent();

            if (!bIsEmbedded)
            {
                Dispatcher.BeginInvokeAsynchronouslyInApplicationIdle(() =>
                {
                    DiscoveryViewModel = new OPCUADiscoveryViewModel(Dispatcher.CurrentDispatcher, null);
                    if (DiscoveryViewModel.NetworkViewModel.Children.Count > 0)
                    {
                        foreach (var v in DiscoveryViewModel.NetworkViewModel.Children)
                            AddTreeItem(v, null);
                    }

                    DiscoveryViewModel.NetworkViewModel.Children.CollectionChanged += CollectionChanged;
                    if (workspace != null)
                    {
                        // workspace.ContentRendered += workspace_ContentRendered;
                        workspace.CloseButtonClick += workspace_CloseButtonClick;
                    }

                    workspace_ContentRendered(null, null);
                });
            }
        }

        void workspace_ContentRendered(object sender, EventArgs e)
        {
            WorkstationViewModel vm = new WorkstationViewModel(Dispatcher.CurrentDispatcher,
                                        System.Net.Dns.GetHostName(), DiscoveryViewModel.NetworkViewModel);
            treeListView.Nodes.Clear();
            treeListViewRecent.Nodes.Clear();

            AddTreeItem(vm, null);
            DiscoveryViewModel.AddHostName(vm.Title);

            LoadRecentEndpoints();

            // DiscoveryViewModel.networkViewModel.RestartDiscovery();
        }

        private void startSearching_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoveryViewModel == null)
                return;

            IsBusyGrid.Visibility = Visibility.Visible;
            treeListControl.Visibility = Visibility.Collapsed;

            if (DiscoveryViewModel.NetworkViewModel.Children.Count > 0)
            {
                foreach (var v in DiscoveryViewModel.NetworkViewModel.Children)
                    RemoveTreeItem(v);
            }
            DiscoveryViewModel.networkViewModel.RestartDiscovery();
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvokeIfRequired(() =>
            {
                if (e.NewItems != null && e.NewItems.Count != 0)
                {
                    IsBusyGrid.Visibility = Visibility.Collapsed;
                    treeListControl.Visibility = Visibility.Visible;

                    foreach (TreeViewItemViewModel dvm in e.NewItems)
                        AddTreeItem(dvm, null);
                }

                if (e.OldItems != null && e.OldItems.Count != 0)
                {
                    foreach (TreeViewItemViewModel dvm in e.OldItems)
                        RemoveTreeItem(dvm);
                }
            });
        }

        TreeListNodeCollection GetTreeListNodeCollection(TreeViewItemViewModel dvm)
        {
            TreeListNodeCollection ret;
            if (!mapModelList.TryGetValue(dvm, out ret))
                ret = treeListView.Nodes;
            return ret;
        }

        private void RemoveTreeItem(TreeViewItemViewModel dvm)
        {
            TreeListNode subitem;
            if (mapModelItem.TryGetValue(dvm, out subitem))
            {
                TreeListNodeCollection ic = GetTreeListNodeCollection(dvm.Parent);
                if (ic.Contains(subitem))
                    ic.Remove(subitem);

                mapModelItem.Remove(dvm);
                treeListView.Nodes.Remove(subitem);
            }

            mapModelList.Remove(dvm);
            // dvm.Children.CollectionChanged -= CollectionChanged;
        }

        void ClearDummyNode(TreeListNode node)
        {
            List<TreeListNode> dummyNodes = (from n in node.Nodes where n.Tag == TreeListControlHelper.DummyNode select n).ToList();
            dummyNodes.ForEach(dnode => { node.Nodes.Remove(dnode); });
        }

        private void AddTreeItem(TreeViewItemViewModel dvm, TreeListNode parent)
        {
            if (dvm == null || mapModelItem.ContainsKey(dvm))
                return;
            object tag = dvm;

            if (parent != null && !parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return;
            }

            object header = tag;
            //lock (staticTreeLock)
            {
                treeListControl.BeginDataUpdate();
                try
                {
                    var ic = new TreeItemControl(header);
                    var newitem = treeListControl.AddNode(ic, parent, tag);
                    if (dvm is EndpointDescriptionViewModel)
                    {
                        ConnectControl treeitem = new ConnectControl(this) { DataContext = dvm };
                        ic.InnerControl = treeitem;
                    }
                    else
                    {
                        TreeItemCustomControl treeitem = new TreeItemCustomControl { DataContext = dvm };
                        ic.InnerControl = treeitem;
                        treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);
                    }

                    mapModelList.Add(dvm, newitem.Nodes);
                    mapModelItem.Add(dvm, newitem);

                    UpdateFolderIcon(newitem, false);
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }
            }
        }

        bool NeedToBeExpanded(TreeListNode item)
        {
            if (item.Tag is IEntityReference)
            {
                return true;
            }
            return false;
        }

        void OnRowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            var treeListView = sender as TreeListView;
            var treeListControl = treeListView.FindParent<TreeListControl>() as TreeListControl;
            
            TreeListNode item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (item == null || item.Tag == null || !(item.Tag is EndpointDescriptionViewModel))
                return;
            EndpointDescriptionViewModel endpointDescriptionViewModel = item.Tag as EndpointDescriptionViewModel;
            Browse(endpointDescriptionViewModel);
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            var treeListControl = e.OriginalSource as TreeListControl;
            TreeListNode item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (workspace != null)
                workspace.ContextObject = null;
            if (item == null || item.Tag == null || !(item.Tag is TreeViewItemViewModel))
            {
                toolbarDiscovering.DataContext = null;
                return;
            }

            toolbarDiscovering.DataContext = item.Tag;
        }

        bool bExpandingPending;

        public class BrowseEventArgs : EventArgs
        {
            public EndpointDescriptionViewModel endpoint { get; set; }
            public bool Cancel { get; set; }
        }

        public void Browse(EndpointDescriptionViewModel endpointDescriptionViewModel, NodeId node = null)
        {
            using (new WaitCursor(this))
            {
                OPCUABrowseServer serverfound;
                if (workspace != null)
                {
                    if (mapOpenBrowseServer.TryGetValue(GetTitle(endpointDescriptionViewModel), out serverfound))
                        workspace.FlashDockedElement(serverfound);
                    else
                    {
                        OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                                component, node ?? ObjectIds.ObjectsFolder, null, false, isEmbedded, allowMultiSelection);
                        workspace.SetDesiredHeightAndWidthInDockedMode(serverbrowser, serverbrowser.Height, serverbrowser.Width);
                        serverbrowser.ClearValue(FrameworkElement.WidthProperty);
                        serverbrowser.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");

                        UFInterfaces.DockSide side = UFInterfaces.DockSide.Tabbed;

                        workspace.AddDockingChildren(serverbrowser, GetTitle(endpointDescriptionViewModel),
                                                        UFInterfaces.DockState.Dock, side, true, true, UFInterfaces.DockSide.Bottom);
                        workspace.SetDockedElementIcon(serverbrowser, new ImageBrush(bm));
                        workspace.ActivateDockedElement(serverbrowser);

                        mapOpenBrowseServer.Add(GetTitle(endpointDescriptionViewModel), serverbrowser);

                        AddRecentEndpoint(endpointDescriptionViewModel);
                    }
                }
                else if (InnerDockingManager != null)
                {
                    if (mapOpenBrowseServer.TryGetValue(GetTitle(endpointDescriptionViewModel), out serverfound))
                    {
                        UFInterfaces.DockState state = (UFInterfaces.DockState)DockingHelper.GetState(serverfound);

                        if (state == UFInterfaces.DockState.AutoHidden)
                        {
                            InnerDockingManager.SetActiveWindow(serverfound);
                            var layoutItem = DockLayoutManager.GetLayoutItem(serverfound);
                            if (layoutItem != null && layoutItem as LayoutPanel != null)
                                (layoutItem as LayoutPanel).AutoHidden = true;
                        }
                        else
                            InnerDockingManager.SetActiveWindow(serverfound);

                        // if (opcuaEntityReferenceCurrent != null && opcuaEntityReferenceCurrent.IsTypeDefinition)
                        {
                            if (node != null)
                                serverfound.Browsefor(node);
                            else
                                serverfound.Browsefor(ObjectIds.ObjectTypesFolder);
                        }
                    }
                    else
                    {
                        // if (opcuaEntityReferenceCurrent != null && opcuaEntityReferenceCurrent.IsTypeDefinition && node == null)
                        //     node = ObjectIds.ObjectTypesFolder;
                        PopupBrowser popupBrowser = InnerDockingManager.FindParent<PopupBrowser>();
                        DockLayoutManager dockingManager = InnerDockingManager;
                        if (popupBrowser == null)
                            return;
                        OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                                component, node ?? ObjectIds.ObjectsFolder, null, false, isEmbedded, allowMultiSelection);
                        serverbrowser.ClearValue(FrameworkElement.WidthProperty);
                        serverbrowser.ClearValue(FrameworkElement.HeightProperty);
                        String header = GetTitle(endpointDescriptionViewModel);
                        var panel = popupBrowser.AddDockedControl(serverbrowser, header, DockSide.Left, DockState.Dock, false, false, DockSide.Left);
                        BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");
                        panel.CaptionImage = bm;
                        dockingManager.UpdateLayout();
                        dockingManager.ActivateDockItem(panel);
                        mapOpenBrowseServer.Add(header, serverbrowser);
                        AddRecentEndpoint(endpointDescriptionViewModel);
                    }
                }
            }
        }

        public void BrowseArea(EndpointDescriptionViewModel endpointDescriptionViewModel)
        {
            using (new WaitCursor(this))
            {
                OPCUABrowseServer serverfound;
                if (workspace != null)
                {
                    if (mapOpenAreaBrowseServer.TryGetValue(GetTitle(endpointDescriptionViewModel), out serverfound))
                        workspace.FlashDockedElement(serverfound);
                    else
                    {
                        OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                                component, null, null, true, isEmbedded, allowMultiSelection);
                        workspace.SetDesiredHeightAndWidthInDockedMode(serverbrowser, serverbrowser.Height, serverbrowser.Width);
                        serverbrowser.ClearValue(FrameworkElement.WidthProperty);
                        serverbrowser.ClearValue(FrameworkElement.HeightProperty);

                        BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");

                        workspace.AddDockingChildren(serverbrowser, 
                                                    String.Format(Properties.Resources.AreaTitleFormat,
                                                    GetTitle(endpointDescriptionViewModel)),
                                                    UFInterfaces.DockState.Dock, UFInterfaces.DockSide.Bottom, 
                                                    itemID: String.Format("{0}_BrowseArea_{1}", nameof(OPCUABrowseServer), GetTitle(endpointDescriptionViewModel)));
                        workspace.SetDockedElementIcon(serverbrowser, new ImageBrush(bm));
                        workspace.ActivateDockedElement(serverbrowser);

                        mapOpenAreaBrowseServer.Add(GetTitle(endpointDescriptionViewModel), serverbrowser);

                        AddRecentEndpoint(endpointDescriptionViewModel);
                    }
                }
                else if (InnerDockingManager != null)
                {
                    if (mapOpenBrowseServer.TryGetValue(GetTitle(endpointDescriptionViewModel), out serverfound))
                    {
                        UFInterfaces.DockState state = (UFInterfaces.DockState)DockingHelper.GetState(serverfound);

                        if (state == UFInterfaces.DockState.AutoHidden)
                        {
                            InnerDockingManager.SetActiveWindow(serverfound);
                            var layoutItem = DockLayoutManager.GetLayoutItem(serverfound);
                            if (layoutItem != null && layoutItem as LayoutPanel != null)
                                (layoutItem as LayoutPanel).AutoHidden = true;
                        }
                        else
                            InnerDockingManager.SetActiveWindow(serverfound);
                    }
                    else
                    {

                        PopupBrowser popupBrowser = InnerDockingManager.FindParent<PopupBrowser>();
                        DockLayoutManager dockingManager = InnerDockingManager;
                        if (popupBrowser == null)
                            return;
                        OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                               component, null, null, true, isEmbedded, allowMultiSelection);
                        serverbrowser.ClearValue(FrameworkElement.WidthProperty);
                        serverbrowser.ClearValue(FrameworkElement.HeightProperty);
                        String header = GetTitle(endpointDescriptionViewModel);
                        var panel = popupBrowser.AddDockedControl(serverbrowser, header, DockSide.Left, DockState.Dock, false, false, DockSide.Left);
                        BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");
                        panel.CaptionImage = bm;
                        dockingManager.UpdateLayout();
                        dockingManager.ActivateDockItem(panel);
                        mapOpenBrowseServer.Add(GetTitle(endpointDescriptionViewModel), serverbrowser);
                        AddRecentEndpoint(endpointDescriptionViewModel);
                    }
                }
            }
        }

        public void ViewAlarms(EndpointDescriptionViewModel endpointDescriptionViewModel)
        {
            using (new WaitCursor(this))
            {
                SessionViewModel sessionViewModel = SessionViewModel.FindOrCreate(Properties.Resources.SessionBrowseName, endpointDescriptionViewModel);
                OPCUAAlarmViewer alarmViewer = new OPCUAAlarmViewer(sessionViewModel, null);

                if (workspace != null)
                {
                    workspace.SetDesiredHeightAndWidthInDockedMode(alarmViewer, alarmViewer.Height, alarmViewer.Width);
                    alarmViewer.ClearValue(FrameworkElement.WidthProperty);
                    alarmViewer.ClearValue(FrameworkElement.HeightProperty);

                    BitmapImage bm = alarmViewer.GetControlImage();

                    workspace.AddDockingChildren(alarmViewer, GetTitle(endpointDescriptionViewModel),
                                                 UFInterfaces.DockState.Dock, UFInterfaces.DockSide.Bottom,
                                                 itemID: String.Format("{0}_{1}", nameof(OPCUAAlarmViewer), GetTitle(endpointDescriptionViewModel)));
                    workspace.SetDockedElementIcon(alarmViewer, new ImageBrush(bm));
                    workspace.ActivateDockedElement(alarmViewer);

                    AddRecentEndpoint(endpointDescriptionViewModel);
                }
            }
        }

        public void ViewAuditEvents(EndpointDescriptionViewModel endpointDescriptionViewModel)
        {
            using (new WaitCursor(this))
            {
                SessionViewModel sessionViewModel = SessionViewModel.FindOrCreate(Properties.Resources.SessionBrowseName, endpointDescriptionViewModel);
                OPCUAAuditEventViewer auditEventViewer = new OPCUAAuditEventViewer(sessionViewModel);

                if (workspace != null)
                {
                    workspace.SetDesiredHeightAndWidthInDockedMode(auditEventViewer, auditEventViewer.Height, auditEventViewer.Width);
                    auditEventViewer.ClearValue(FrameworkElement.WidthProperty);
                    auditEventViewer.ClearValue(FrameworkElement.HeightProperty);

                    BitmapImage bm = auditEventViewer.GetControlImage();

                    workspace.AddDockingChildren(auditEventViewer, GetTitle(endpointDescriptionViewModel),
                                                 UFInterfaces.DockState.Dock, UFInterfaces.DockSide.Bottom,
                                                 itemID: String.Format("{0}_{1}", nameof(OPCUAAuditEventViewer), GetTitle(endpointDescriptionViewModel)));
                    workspace.SetDockedElementIcon(auditEventViewer, new ImageBrush(bm));
                    workspace.ActivateDockedElement(auditEventViewer);

                    AddRecentEndpoint(endpointDescriptionViewModel);
                }
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is OPCUABrowseServer))
                return;

            OPCUABrowseServer serverbrowser = e.TargetItem as OPCUABrowseServer;
            if (mapOpenBrowseServer.ContainsValue(serverbrowser))
                mapOpenBrowseServer.Remove(GetTitle(serverbrowser.endpointDescriptionViewModel));
            if (mapOpenAreaBrowseServer.ContainsValue(serverbrowser))
                mapOpenAreaBrowseServer.Remove(GetTitle(serverbrowser.endpointDescriptionViewModel));
            
            workspace.RemoveDockingChildren(serverbrowser);
        }

        bool bProcessingKeyPressed;
        private void treeViewDiscovering_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5 || bProcessingKeyPressed)
                return;

            bProcessingKeyPressed = true;

            try
            {
                e.Handled = false;
                using (new WaitCursor(this))
                {

                    
                    TreeListNode selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
                    if (selectedItem != null && selectedItem.Tag is WorkstationViewModel)
                    {
                        WorkstationViewModel wvm = selectedItem.Tag as WorkstationViewModel;
                        selectedItem.IsExpanded = false;
                        selectedItem.Nodes.Clear();
                        RemoveTreeItem(wvm);
                        DiscoveryViewModel.RemoveHostName(wvm.Title);
                        TreeListNode parent = null;
                        mapModelItem.TryGetValue(wvm.Parent, out parent);
                        AddTreeItem(wvm, parent);
                    }
                    else if (selectedItem != null && selectedItem.Tag is DomainViewModel)
                    {
                        DomainViewModel dvm = selectedItem.Tag as DomainViewModel;
                        selectedItem.IsExpanded = false;
                        selectedItem.Nodes.Clear();
                        RemoveTreeItem(dvm);
                        DiscoveryViewModel.RemoveHostName(dvm.Title);
                        TreeListNode parent = null;
                        mapModelItem.TryGetValue(dvm.Parent, out parent);
                        AddTreeItem(dvm, parent);
                    }
                }
            }
            finally
            {
                bProcessingKeyPressed = false;
            }
        }

        private void treeViewDiscovering_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeListNode selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (selectedItem == null)
                return;
            if (selectedItem != null && selectedItem.Tag is TreeViewItemViewModel)
            {
                TreeViewItemViewModel vmb = selectedItem.Tag as TreeViewItemViewModel;
                ContextMenu menu = null;
                if (vmb is IEntityReference)
                    menu = (vmb as IEntityReference).contextMenu;
                while (menu == null && vmb.Parent != null)
                {
                    vmb = vmb.Parent;
                    if (vmb is IEntityReference)
                        menu = (vmb as IEntityReference).contextMenu;
                }

                if (menu != null)
                {
                    menu.DataContext = vmb;
                    if (CustomFontHelper.CanApplyCustomFont())
                        (from object item in menu.Items where item is Control select item as Control).ToList().ForEach((item) =>
                        {
                            item.FontSize = CustomFontHelper.GetCustomFontSize();
                            item.FontFamily = CustomFontHelper.GetCustomFontFamily();
                        });
                    treeListControl.ContextMenu = menu;
                    e.Handled = true;
                }
            }
        }

        void LoginHandler(object sender, RoutedEventArgs e)
        {
            TreeListNode selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (selectedItem != null && selectedItem.Tag is DomainViewModel)
            {
                var dvm = selectedItem.Tag as DomainViewModel;
                ShowCredentialOptions options = new ShowCredentialOptions();
                options.WindowTitle = dvm.Title;
                options.MainInstruction = Properties.Resources.ShowCredentialMainInstruction;
                options.Content = Properties.Resources.ShowCredentialContent;
                options.SavedCredentialsBucket = dvm.Title;
                options.ShowSaveCheckBox = true;
                options.ShowUIForSavedCredentials = true;
                options.SavedCredentialsBucket = Properties.Resources.SessionBrowseName;

                if (component.UIInterface != null)
                {
                    ShowCredentialResults res = component.UIInterface.ShowCredentialDialog(options);
                    if (res.result == CustomDialogResults.OK && !String.IsNullOrWhiteSpace(res.UserName))
                    {
                        ProgressDialog dlg = new ProgressDialog()
                        {
                            AutoShowDelay = 0,
                            Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                            ProgressBarIndeterminate = true,
                            DialogText = Properties.Resources.ShowCredentialValidating,
                            IsCancellingEnabled = false
                        };

                        bool userValidated = false;
                        dlg.RunWorkerThread(null, (o, ev) =>
                        {
                            var userName = res.UserName;
                            var password = res.Password;

                            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                            {
                                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, dvm.Title))
                                {
                                    userValidated = context.ValidateCredentials(userName, password);
                                }
                            }
                        });

                        if (!userValidated)
                        {
                            component.UIInterface.ShowInformation(String.Format(Properties.Resources.UserAuthenticationFailed, res.UserName));
                            return;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(res.UserName))
                    {

                        selectedItem.IsExpanded = false;
                        selectedItem.Nodes.Clear();
                        RemoveTreeItem(dvm);
                        TreeListNode parent = null;
                        mapModelItem.TryGetValue(dvm.Parent, out parent);
                        AddTreeItem(dvm, parent);
                        dvm.RestartDiscovery(res.UserName, res.Password);
                        selectedItem.IsExpanded = true;
                    }
                }
            }
        }

        private void addHost_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoveryViewModel == null)
                return;

            WorkstationViewModel vm = new WorkstationViewModel(Dispatcher.CurrentDispatcher, String.Empty, DiscoveryViewModel.NetworkViewModel);
            FrameworkElement fm = vm.Tooltip as FrameworkElement;
            if (fm != null)
                fm.DataContext = vm;

            //var wnd = new ChildWindow 
            //{ 
            //    HasCloseButton = true,
            //    Content = fm
            //};
            
            //wnd.Closed += (o, ev) =>
            //    {
            //        if (wnd.DialogResult != true)
            //            return;

            //        AddTreeItem(vm, null);
            //        DiscoveryViewModel.AddHostName(vm.Title);

            //        IsBusyGrid.Visibility = Visibility.Collapsed;
            //        treeListControl.Visibility = Visibility.Visible;
            //    };
            //wnd.Show();

            GeneralDialogContent addHostDialog = new GeneralDialogContent(fm)
            {
                Title = Properties.Resources.AddNewHostNameTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "AddHost"
            };
            if (addHostDialog.ShowDialog() != true)
                return;
            AddTreeItem(vm, null);
            DiscoveryViewModel.AddHostName(vm.Title);

            IsBusyGrid.Visibility = Visibility.Collapsed;
            treeListControl.Visibility = Visibility.Visible;
        }

        private void addEndpoint_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoveryViewModel == null)
                return;

            EndpointDescriptionViewModel vm = new EndpointDescriptionViewModel(DiscoveryViewModel);
            FrameworkElement fm = vm.Tooltip as FrameworkElement;
            if (fm != null)
                fm.DataContext = vm;
            //var panel = new StackPanel();
            //panel.Children.Add(fm);
            //var btnOk = new Button() { Content = "OK", IsDefault = true };
            //panel.Children.Add(btnOk);

            //var wnd = new ChildWindow
            //{
            //    HasCloseButton = true,
            //    Content = panel
            //};

            //btnOk.Click += (o, ev) =>
            //    {
            //        wnd.DialogResult = true;
            //        wnd.Close();
            //    };
            //wnd.Closed += (o, ev) =>
            //{
            //    if (wnd.DialogResult != true)
            //        return;

            //    try
            //    {
            //        vm.UpdateEndpointDescription(vm.SecurityMode != MessageSecurityMode.None);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Trace.TraceError(ex.ToString());
            //        System.Windows.MessageBox.Show(ex.ToString());
            //        return;
            //    }

            //    AddTreeItem(vm, null);

            //    IsBusyGrid.Visibility = Visibility.Collapsed;
            //    treeListControl.Visibility = Visibility.Visible;
            //};
            //wnd.Show();

            GeneralDialogContent addHostDialog = new GeneralDialogContent(fm)
            {
                Title = Properties.Resources.AddNewEndpointTitle,
                Owner = this.FindParent<Window>(),
                HelpLink = "AddEndpoint"
            };
            if (addHostDialog.ShowDialog() != true)
                return;

            try
            {
                vm.UpdateEndpointDescription(vm.UseSecurity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
                if (component.UIInterface != null)
                    component.UIInterface.ShowError(ex.Message);
                // return;
            }

            AddTreeItem(vm, null);

            IsBusyGrid.Visibility = Visibility.Collapsed;
            treeListControl.Visibility = Visibility.Visible;
        }

        public bool BrowseEndpoint(String endpoint)
        {
            EndpointDescriptionViewModel vm = new EndpointDescriptionViewModel(DiscoveryViewModel);
            try
            {
                vm.EndpointUrl = endpoint;
                vm.UpdateEndpointDescription(vm.UseSecurity);
            }
            catch (Exception ex)
            {
                return false;
                //System.Diagnostics.Trace.TraceError(ex.ToString());
                //System.Windows.MessageBox.Show(ex.ToString());
                // return;
            }

            AddTreeItem(vm, null);
            Browse(vm);

            return true;
        }

        #region Save Load Recents

        readonly String StoreFileName = String.Format("{0}.dat", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));

        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            { }

            return null;
        }

        void SaveRecentEndpoints()
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName))
                return;
            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                try
                {
                    Mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

                try
                {
                    EndpointDescriptionCollection eplist = new EndpointDescriptionCollection();
                    foreach (var v in listRecentEndpoints)
                        eplist.Add(v.endpointDescription);

                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.Create, isoStorage))
                    {
                        XmlWriterSettings settings = new XmlWriterSettings
                        {
                            Indent = true,
                            OmitXmlDeclaration = false,
                            Encoding = Encoding.UTF8
                        };

                        using (XmlWriter writer = XmlWriter.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(EndpointDescriptionCollection));
                                serializer.WriteObject(writer, eplist);
                            }
                            catch (Exception ex)
                            {
                                writer.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        void LoadRecentEndpoints()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(StoreFileName) ||
                isoStorage.GetFileNames(StoreFileName).Length <= 0)
                return;

            var name = Assembly.GetExecutingAssembly().GetName().Name;
            using (var Mutex = new Mutex(false, name))
            {
                Mutex.WaitOne();

                try
                {
                    using (Stream stream = new IsolatedStorageFileStream(StoreFileName, FileMode.OpenOrCreate, isoStorage))
                    {
                        XmlReaderSettings settings = new XmlReaderSettings
                        {
                            ConformanceLevel = ConformanceLevel.Document,
                            CloseInput = true
                        };

                        using (XmlReader reader = XmlReader.Create(stream, settings))
                        {
                            try
                            {
                                DataContractSerializer serializer = new DataContractSerializer(typeof(EndpointDescriptionCollection));
                                EndpointDescriptionCollection eplist = serializer.ReadObject(reader) as EndpointDescriptionCollection;

                                listRecentEndpoints.Clear();
                                eplist.ForEach(e =>
                                {
                                    var ep = new EndpointDescriptionViewModel(e, new ApplicationDescriptionViewModel(e.Server, null, null, null));
                                    AddRecentEndpoint(ep, false);
                                });

                                foreach (var v in listRecentEndpoints)
                                    AddTreeRecent(v);
                            }
                            catch (Exception ex)
                            {
                                listRecentEndpoints.Clear();
                                reader.Close();
                            }
                        }
                    }
                }
                catch
                {

                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
        }

        void AddRecentEndpoint(EndpointDescriptionViewModel e, bool bAddTree = true)
        {
            foreach(var v in listRecentEndpoints)
            {
                if (v.Title == e.Title && v.DisplayTitle == e.DisplayTitle || v.EndpointUrl == e.EndpointUrl)
                {
                    if (listRecentEndpoints.Count == 1)
                        return;

                    if (bAddTree)
                        RemoveTreeRecent(v);
                    listRecentEndpoints.Remove(v);
                    break;
                }
            }

            listRecentEndpoints.Add(e);
            if (listRecentEndpoints.Count > 20)
            {
                if (bAddTree)
                    RemoveTreeRecent(listRecentEndpoints[0]);
                listRecentEndpoints.RemoveAt(0);
            }

            if (bAddTree)
            {
                SaveRecentEndpoints();
                AddTreeRecent(e);
            }
        }

        void AddTreeRecent(EndpointDescriptionViewModel dvm)
        {
            if (dvm == null)
                return;
            object tag = dvm;
            TreeListNode subitem = new TreeListNode();
            object header = tag;

            //lock (staticTreeLock)
            {
                treeListRecentControl.BeginDataUpdate();
                try
                {
                    var ic = new TreeItemControl(header);
                    ConnectControl treeitem = new ConnectControl(this) { DataContext = dvm };
                    treeitem.AppDescTitle = dvm.Parent.Title;
                    ic.InnerControl = treeitem;
                    var newitem = treeListRecentControl.AddNode(ic, null, tag);

                    UpdateFolderIcon(newitem, false);
                }
                finally
                {
                    treeListRecentControl.EndDataUpdate();
                }
            }
        }

        void RemoveTreeRecent(EndpointDescriptionViewModel e)
        {
            TreeListNode eNode = (from n in treeListViewRecent.Nodes where n.Tag == e select n).FirstOrDefault() as TreeListNode;
            if(eNode != null)
                treeListViewRecent.Nodes.Remove(eNode);
        }

        public void RemoveRecentEndpoint(EndpointDescriptionViewModel e)
        {
            RemoveTreeRecent(e);
            listRecentEndpoints.Remove(e);
            SaveRecentEndpoints();
        }
        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            DiscoveryViewModel.NetworkViewModel.Children.CollectionChanged -= CollectionChanged;
            if (workspace != null)
            {
                // workspace.ContentRendered -= workspace_ContentRendered;
                workspace.CloseButtonClick -= workspace_CloseButtonClick;
            }
            if (innerDockingManager != null)
                innerDockingManager.DockItemClosed -= innerDockingManager_CloseButtonClick;

            DiscoveryViewModel.Dispose();
        }

        private void innerDockingManager_CloseButtonClick(object sender, DockItemClosedEventArgs e)
        {
            if (!(e.OriginalSource is OPCUABrowseServer))
                return;

            OPCUABrowseServer serverbrowser = e.OriginalSource as OPCUABrowseServer;
            if (mapOpenBrowseServer.ContainsValue(serverbrowser))
                mapOpenBrowseServer.Remove(GetTitle(serverbrowser.endpointDescriptionViewModel));
            if (mapOpenAreaBrowseServer.ContainsValue(serverbrowser))
                mapOpenAreaBrowseServer.Remove(GetTitle(serverbrowser.endpointDescriptionViewModel));
        }
        #endregion

        OPCUAEntityReference opcuaEntityReferenceCurrent;
        internal void SetCurrentReference(OPCUAEntityReference opcuaEntityReference)
        {
            opcuaEntityReferenceCurrent = opcuaEntityReference;
            if (!opcuaEntityReference.IsValid)
                return;

            EndpointDescriptionViewModel endpointDescriptionViewModel = EndpointDescriptionViewModel.SelectEndpoint(opcuaEntityReference.HostName, opcuaEntityReference.AppName, false);
            if (endpointDescriptionViewModel == null)
                return;

            if (opcuaEntityReference.ResolvedNodeId != null)
                Browse(endpointDescriptionViewModel/*, (NodeId)opcuaEntityReference.ResolvedNodeId*/);
        }
    }
}
