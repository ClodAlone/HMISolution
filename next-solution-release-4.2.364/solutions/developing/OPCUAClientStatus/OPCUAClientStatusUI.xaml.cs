using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCUAViewModel;
using System.Collections.Specialized;
using UFInterfaces;
using Tracing.ComponentService;
using ViewModelLib;
using Utilities.Animations;
using DevExpress.Xpf.Charts;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Opc.Ua;
using Utilities;
using System.Windows.Threading;
using StringManager.ComponentService;
using TranslationHelpers;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using WPFUtilities;
using WPFUtilities.Extensions;

namespace OPCUAClientStatus
{
    /// <summary>
    /// Interaction logic for OPCUAClientStatusUI.xaml
    /// </summary>
    public partial class OPCUAClientStatusUI : UserControl, IDisposable
    {
        #region OverrideBaseProperties
        private void OverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(OPCUAClientStatusUI));
            dpd.AddValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(OPCUAClientStatusUI));
            dpd.AddValueChangedSafe(this, OnBackgroundChanged);
        }
        private void DetachOverrideBaseProperties()
        {
            DependencyPropertyDescriptor dpd;

            dpd = DependencyPropertyDescriptor.FromProperty(ForegroundProperty, typeof(OPCUAClientStatusUI));
            dpd.RemoveValueChangedSafe(this, OnForegroundChanged);
            dpd = DependencyPropertyDescriptor.FromProperty(BackgroundProperty, typeof(OPCUAClientStatusUI));
            dpd.RemoveValueChangedSafe(this, OnBackgroundChanged);
        }
        private void OnForegroundChanged(object sender, EventArgs e)
        {
            var control = sender as OPCUAClientStatusUI;
            if (control != null)
            {
                control.OnForegroundChanged();
            }
        }
        protected virtual void OnForegroundChanged()
        {
            if (bInit)
                UpdateControlLayout();
        }
        private void OnBackgroundChanged(object sender, EventArgs e)
        {
            var control = sender as OPCUAClientStatusUI;
            if (control != null)
            {
                control.OnBackgroundChanged();
            }
        }
        protected virtual void OnBackgroundChanged()
        {
            // TODO: Add your property changed side-effects. Descendants can override as well.
            if(bInit && !IsManipulationEnabled)
                UpdateControlLayout();
        }


        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;
            if (workspace != null)
                workspace.ContextObject = null;
        }

        private void UpdateControlLayout()
        {
            if (this.ReadLocalValue(BackgroundProperty) != DependencyProperty.UnsetValue)
            {
                secondGrid.Background = secondGrid.Background = treeListView.Background = mainGrid.Background = Background;
            }

            if (this.ReadLocalValue(ForegroundProperty) != DependencyProperty.UnsetValue)
            {
            }
        }
        #endregion
        readonly IWorkspace workspace;
        readonly ISimpleLogging simpleLogging;
        readonly IStringEditorManager stringManager;
        readonly Dictionary<TreeViewItemViewModel, TreeListNode> mapModelItem = new Dictionary<TreeViewItemViewModel, TreeListNode>();
        readonly Dictionary<TreeViewItemViewModel, FrameworkElement> mapModelPopup = new Dictionary<TreeViewItemViewModel, FrameworkElement>();
        
        List<NotifyCollectionChangedEventArgs> notifyCollectionChangesList;
        DispatcherOperation dpCollectionChanged;

        bool bLazyUI = false;
        bool bIsPopup = false;
        bool bInit;
        bool bLoaded;
        static Object staticTreeLock = new Object();
        static Object staticLock = new Object();
        static bool timerAquired = false;
        DispatcherTimer timer;
        public OPCUAClientStatusUI(IWorkspace w, ISimpleLogging l, IStringEditorManager s)
        {
            InitializeComponent();

            workspace = w;
            simpleLogging = l;
            stringManager = s;

            OverrideBaseProperties();
            if (workspace != null)
            {
                workspace.CloseButtonClick += workspace_CloseButtonClick;
                workspace.DockStateChanged += workspace_DockStateChanged;
                UpdateControlLayout();
                bInit = true;
            }
            lock (staticLock)
            {
                if (!timerAquired)
                {
                    timerAquired = true;
                    textStatus.DataContext = ViewModelBase.StatisticData;
                    timer = new DispatcherTimer();
                    timer.Interval = ViewModelBase.StatisticDataTimeSpan;
                    timer.Tick += (o, e) =>
                        {
                            ViewModelBase.RefreshStatisticData();
                        };
                    timer.Start();
                }
                else
                    textStatus.Visibility = System.Windows.Visibility.Collapsed;
            }

            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    if (stringManager != null)
                    {
                        StringManager_CultureChanged(null, null);
                        stringManager.CultureChanged += StringManager_CultureChanged;
                    }
                }
            };
        }
        internal string stringPlaceolder = "OPCUAClientStatus";
        private void StringManager_CultureChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
            {
                pendingCount.Text = Properties.Resources.OPCUAClientStatusPendingCount;
                clientStatusThreads.Text = Properties.Resources.OPCUAClientStatusThreads;
                clientStatusRefreshRate.Text = Properties.Resources.OPCUAClientStatusUIRefreshRate;
                clientStatusAtSeconds.Text = Properties.Resources.OPCUAClientStatusAtSeconds;
                clientStatusServerUpdate.Text = Properties.Resources.OPCUAClientStatusServerUpdate;
                clientStatusAtStatus.Text = Properties.Resources.OPCUAClientStatusAtSeconds;
                clientStatusCPU.Text = Properties.Resources.OPCUAClientStatusCPU;
                uiPercent.Text = Properties.Resources.OPCUAClientStatusPercent;
                uiLatency.Text = Properties.Resources.OPCUAClientStatusUILatency;
                miliSeconds.Text = Properties.Resources.OPCUAClientStatusMiliSeconds;

                UpdateSessionLabel();
                UpdateSubscriptionLabel();
                UpdateMonitoredLabel();
            });
        }
        public void EnableUIMng(bool isPopup = false)
        {
            bIsPopup = isPopup;
            if (bLazyUI)
                return;
            bLazyUI = true;

            if (notifyCollectionChangesList == null)
                notifyCollectionChangesList = new List<NotifyCollectionChangedEventArgs>();

            if (RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers.Count > 0)
            {
                var list = new List<RealTimeConnectionManagerViewModel>(RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers.Count);
                list.AddRange(RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers);
                list_CollectionChanged(RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
            }

            if (SessionViewModel.listActiveSessions.Count > 0)
            {
                var list = new List<SessionViewModel>(SessionViewModel.listActiveSessions.Count);
                list.AddRange(SessionViewModel.listActiveSessions);
                list_CollectionChanged(SessionViewModel.listActiveSessions,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
            }

            if (SubscriptionViewModel.listActiveSubscriptions.Count > 0)
            {
                var list = new List<SubscriptionViewModel>(SubscriptionViewModel.listActiveSubscriptions.Count);
                list.AddRange(SubscriptionViewModel.listActiveSubscriptions);
                list_CollectionChanged(SubscriptionViewModel.listActiveSubscriptions,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
            }

            if (MonitoredItemViewModel.listActiveMonitoredItems.Count > 0)
            {
                var list = new List<MonitoredItemViewModel>(MonitoredItemViewModel.listActiveMonitoredItems.Count);
                list.AddRange(MonitoredItemViewModel.listActiveMonitoredItems);
                list_CollectionChanged(MonitoredItemViewModel.listActiveMonitoredItems,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
            }

            SessionViewModel.listActiveSessions.CollectionChanged += list_CollectionChanged;
            SubscriptionViewModel.listActiveSubscriptions.CollectionChanged += list_CollectionChanged;
            MonitoredItemViewModel.listActiveMonitoredItems.CollectionChanged += list_CollectionChanged;
            RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers.CollectionChanged += list_CollectionChanged;
        }

        void list_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChanged(e);
        }

        void NotifyCollectionChanged(NotifyCollectionChangedEventArgs items)
        {
            bool bNewDispatcherOperation = false;
            lock (notifyCollectionChangesList)
            {
                bNewDispatcherOperation = notifyCollectionChangesList.Count == 0;
                notifyCollectionChangesList.Add(items);
            }

            if (bNewDispatcherOperation || dpCollectionChanged == null ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Completed ||
                dpCollectionChanged.Status == DispatcherOperationStatus.Aborted)
            {
                dpCollectionChanged = Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
                {
                    var notifyList = new List<NotifyCollectionChangedEventArgs>();
                    lock (notifyCollectionChangesList)
                    {
                        notifyList.AddRange(notifyCollectionChangesList);
                        notifyCollectionChangesList.Clear();
                    }

                    try
                    {
                        treeListControl.BeginDataUpdate();
                        notifyList.ForEach((e) => 
                        {
                            List<TreeViewItemViewModel> newItems = null;
                            List<TreeViewItemViewModel> oldItems = null;

                            if (e.NewItems != null && e.NewItems.Count != 0)
                            {
                                int processed = 0;
                                foreach (TreeViewItemViewModel dvm in e.NewItems)
                                {
                                   if (!(dvm is RealTimeConnectionManagerViewModel) ||
                                        (dvm as RealTimeConnectionManagerViewModel).SessionViewModel == null)
                                    {
                                        if (++processed > Properties.Settings.Default.MaxCollectionChangeItemsProcessed)
                                        {
                                            if (newItems == null)
                                                newItems = new List<TreeViewItemViewModel>();
                                            newItems.Add(dvm);
                                        }
                                        else 
                                            AddTreeItem(dvm);
                                    }

                                    
                                }
                            }

                            if (e.OldItems != null && e.OldItems.Count != 0)
                            {
                                int processed = 0;
                                foreach (TreeViewItemViewModel dvm in e.OldItems)
                                {
                                    if (++processed > Properties.Settings.Default.MaxCollectionChangeItemsProcessed)
                                    {
                                        if (oldItems == null)
                                            oldItems = new List<TreeViewItemViewModel>();
                                        oldItems.Add(dvm);
                                    }
                                    else
                                        RemoveTreeItem(dvm);
                                }
                            }

                            if (newItems != null && newItems.Count > 0)
                                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems));
                            if (oldItems != null && oldItems.Count > 0)
                                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
                        });

                        UpdateSessionLabel();
                        UpdateSubscriptionLabel();
                        UpdateMonitoredLabel();
                    }
                    finally
                    {
                        treeListControl.EndDataUpdate();
                    }
                });
            }
        }

        private void UpdateSessionLabel()
        {
            sessionSeries.Argument = Properties.Resources.SessionsChartTooltip;
            sessionSeries.Value = SessionViewModel.listActiveSessions.Count;
            LabelSessions.Content = String.Format(Properties.Resources.SessionsNumber, SessionViewModel.listActiveSessions.Count);
        }

        private void UpdateSubscriptionLabel()
        {
            subscriptionSeries.Argument = Properties.Resources.SubscriptionsChartTooltip;
            subscriptionSeries.Value = SubscriptionViewModel.listActiveSubscriptions.Count;
            LabelSubscriptions.Content = String.Format(Properties.Resources.SubscriptionsNumber, SubscriptionViewModel.listActiveSubscriptions.Count);
        }

        private void UpdateMonitoredLabel()
        {
            monitoredItemSeries.Argument = Properties.Resources.MonitoredItemsChartTooltip;
            monitoredItemSeries.Value = MonitoredItemViewModel.listActiveMonitoredItems.Count;
            LabelMonitoredItems.Content = String.Format(Properties.Resources.MonitoredItemsNumber, MonitoredItemViewModel.listActiveMonitoredItems.Count);
        }

        void SetBindingOnProp(TreeListNode node, object bindingSource, string propName, DependencyProperty targetDP = null)
        {
            if (targetDP == null)
                targetDP = TreeItemControl.ItemHeaderProperty;
            var myBinding = new Binding(propName);
            myBinding.Source = bindingSource;
            myBinding.Mode = BindingMode.OneWay;
            myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //myBinding.Converter = new TreeItemHeaderConverter(treeListControl, node);
            BindingOperations.SetBinding(node.Content as HeaderedItemsControl, targetDP, myBinding);
        }

        private void AddTreeItem(TreeViewItemViewModel dvm)
        {
            if (dvm == null || mapModelItem.ContainsKey(dvm))
                return;
            object tag = dvm;
            TreeListNode parent = null;
            if (dvm.Parent != null)
                mapModelItem.TryGetValue(dvm.Parent, out parent);

            if (parent != null && !parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding)
            {
                ClearDummyNode(parent);
                parent.IsExpanded = true;
                var list = (from p in parent.Nodes where p.Tag == tag select p).ToList();
                if (list.Count > 0)
                    return;
            }

            object header = tag;

            if (dvm is SessionViewModel)
            {
                dvm.PropertyChanged += dvmSession_PropertyChanged;
                dvmSession_PropertyChanged(dvm, new PropertyChangedEventArgs("CurrentState"));
                dvmSession_PropertyChanged(dvm, new PropertyChangedEventArgs("Connected"));
            }
            else if (dvm is RealTimeConnectionManagerViewModel)
            {
                dvm.PropertyChanged += dvmRealTimeConnection_PropertyChanged;
            }

            //lock(staticTreeLock)
            {
                var ic = new TreeItemControl(header);
                var newitem = treeListControl.AddNode(ic, parent, tag);
                if (dvm is SessionViewModel)
                {
                    SessionItemControl treeitem = new SessionItemControl(null) { DataContext = dvm };
                    ic.InnerControl = treeitem;
                }
                else if (dvm is RealTimeConnectionManagerViewModel)
                {
                    PendingItemControl treeitem = new PendingItemControl(){ DataContext = dvm };
                    ic.InnerControl = treeitem;
                }
                else
                {
                    TreeItemCustomControl treeitem = new TreeItemCustomControl() { DataContext = dvm };
                    ic.InnerControl = treeitem;
                }

                if (NeedToBeExpanded(newitem))
                    treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

                mapModelItem.Add(dvm, newitem);

                UpdateFolderIcon(newitem, false);
            }
        }

        void OnTreeNodeCollapsing(object sender, TreeListNodeAllowEventArgs e)
        {
            if (!(e.Node.Content as TreeItemControl).IsNodeExpanding)
                UpdateFolderIcon(e.Node, false);
        }

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node.Tag is IEntityReference)
            {
                var collapsedImage = (node.Tag as IEntityReference).CollapsedImageSource;
                var expandedImage = (node.Tag as IEntityReference).ExpandedImageSource;
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? expandedImage == null ? collapsedImage : expandedImage : collapsedImage;
            }
        }

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            if (e == null)
                return;

            TreeListNode item = e.Node;

            if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                return;

            if (!TreeListControlHelper.CanBeExpanded(item))
            {
                if (item.Tag is IEntityReference)
                    UpdateFolderIcon(item, true);
                return;
            }

            //lock (staticTreeLock)
            {
                (item.Content as TreeItemControl).IsNodeExpanding = true;
                treeListControl.BeginDataUpdate();

                try
                {
                    ClearDummyNode(item);

                    if (item.Tag is IEntityReference)
                        UpdateFolderIcon(item, true);
                    e.Handled = true;
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                    (item.Content as TreeItemControl).IsNodeExpanding = false;
                }
            }
        }

        void ClearDummyNode(TreeListNode node)
        {
            List<TreeListNode> dummyNodes = (from n in node.Nodes where n.Tag == TreeListControlHelper.DummyNode select n).ToList();
            treeListControl.BeginDataUpdate();
            try
            {
                dummyNodes.ForEach(dnode => { node.Nodes.Remove(dnode); });
            }
            finally
            {
                treeListControl.EndDataUpdate();
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

        void dvmRealTimeConnection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SessionViewModel")
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    RemoveTreeItem(sender as TreeViewItemViewModel);
                });
            }
        }

        void dvmSession_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentState")
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    SessionViewModel svm = sender as SessionViewModel;
                    TreeListNode subitem;
                    if (mapModelItem.TryGetValue(svm, out subitem))
                    {
                        (treeListView.GetRowElementByRowHandle(subitem.RowHandle))?.Blink(svm.CurrentState == ServerState.Running ? -1 : 500, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
                    }
                });
            }
            else if (e.PropertyName == "Connected")
            {
                Dispatcher.BeginInvokeIfRequired(() =>
                {
                    SessionViewModel svm = sender as SessionViewModel;
                    TreeListNode subitem;
                    if (mapModelItem.TryGetValue(svm, out subitem))
                    {
                        if (svm.Connected)
                        {
                            //(treeListView.GetRowElementByRowHandle(subitem.RowHandle))?.Background = new SolidColorBrush(Colors.Transparent);
                            (treeListView.GetRowElementByRowHandle(subitem.RowHandle))?.Blink(svm.CurrentState == ServerState.Running ? -1 : 500, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });
                        }
                        else
                        {
                            //(treeListView.GetRowElementByRowHandle(subitem.RowHandle))?.Background = new SolidColorBrush(Colors.Red);
                            (treeListView.GetRowElementByRowHandle(subitem.RowHandle))?.Blink(svm.CurrentState == ServerState.Running ? -1 : 500, 0.5, 1, new BackEase() { EasingMode = EasingMode.EaseOut });

                            // workspace.FlashDockedElement(this);
                        }
                    }
                });
            }
        }

        private void RemoveTreeItem(TreeViewItemViewModel dvm)
        {
            if (dvm == null)
                return;

            if (dvm is SessionViewModel)
                dvm.PropertyChanged -= dvmSession_PropertyChanged;
            else if (dvm is RealTimeConnectionManagerViewModel)
                dvm.PropertyChanged -= dvmRealTimeConnection_PropertyChanged;

            TreeListNode subitem;
            if (mapModelItem.TryGetValue(dvm, out subitem))
            {
                mapModelItem.Remove(dvm);

                TreeListNode parent = null;
                if (dvm.Parent != null)
                    mapModelItem.TryGetValue(dvm.Parent, out parent);
                if (parent != null)
                {
                    parent.Nodes.Remove(subitem);
                    if (parent.Nodes.Count == 0 && !parent.IsExpanded && !(parent.Content as TreeItemControl).IsNodeExpanding && NeedToBeExpanded(parent))
                        treeListControl.AddNode(null, parent, TreeListControlHelper.DummyNode);
                }
                else
                    treeListView.Nodes.Remove(subitem);

                if (!bIsPopup && subitem.Tag is IDisposable)
                    (subitem.Tag as IDisposable).Dispose();
            }

            if (workspace != null)
            {
                FrameworkElement popup;
                if (mapModelPopup.TryGetValue(dvm, out popup))
                {
                    workspace.RemoveDockingChildren(popup);
                    mapModelPopup.Remove(dvm);
                }
            }
        }

        private static Object GetTooltip(TreeViewItemViewModel dvm)
        {
            Object ret;
            if (dvm is IEntityReference)
                ret = (dvm as IEntityReference).Tooltip;
            else
                ret = null;
            return ret;
        }

        private void treeViewStatus_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeListNode selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (selectedItem == null)
                return;
            if (selectedItem.Tag is TreeViewItemViewModel)
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
                    treeListControl.ContextMenu = menu;
                    e.Handled = true;
                }
            }
        }

        private void treeViewStatus_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount <= 1)
                return;
            TreeListNode itemToselected = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (itemToselected == null)
                return;
            e.Handled = true;
            ShowPopup(itemToselected);
        }

        void ShowPopup(TreeListNode subitem)
        {
            if (workspace != null)
            {
                TreeViewItemViewModel dvm = subitem.Tag as TreeViewItemViewModel;

                FrameworkElement popup;
                if (mapModelPopup.TryGetValue(dvm, out popup))
                    workspace.FlashDockedElement(popup);
                else
                {
                    Object o = GetTooltip(dvm);
                    FrameworkElement tooltip = o as FrameworkElement;
                    if (tooltip == null)
                        return;
                    tooltip.DataContext = dvm;
                    tooltip.Name = dvm.GetType().Name;

                    //Window wnd = new Window 
                    //{ 
                    //    //ShowInTaskbar = false, 
                    //    Content = tooltip, 
                    //    Title = dvm.Title, 
                    //    SizeToContent = SizeToContent.WidthAndHeight, 
                    //    ResizeMode = ResizeMode.NoResize, 
                    //    Tag = subitem.Tag,
                    //    WindowStyle = WindowStyle.ToolWindow,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Style = ApplicationPropertiesHelper.GetProperty<String>("CurrentSkin")
                    //};

                    //wnd.Closed += wnd_Closed;
                    //wnd.Show();

                    double width = tooltip.Width;
                    double height = tooltip.Height;
                    if (width == 0.0 || Double.IsNaN(width))
                        width = 400;
                    if (height == 0.0 || Double.IsNaN(height))
                        height = 500;
                    if (workspace != null)
                        workspace.SetDesiredHeightAndWidthInDockedMode(tooltip, height, width);
                    tooltip.ClearValue(FrameworkElement.WidthProperty);
                    tooltip.ClearValue(FrameworkElement.HeightProperty);

                    // workspace.SetDesiredSideMode(tooltip, Properties.Resources.OPCUAClientStatus_Title);
                    workspace.AddDockingChildren(tooltip, dvm.Title, UFInterfaces.DockState.Dock, UFInterfaces.DockSide.Left);
                    if (dvm is IEntityReference)
                    {
                        ImageSource ico = (dvm as IEntityReference).CollapsedImageSource;
                        workspace.SetDockedElementIcon(tooltip, new ImageBrush(ico));
                    }
                    workspace.ActivateDockedElement(tooltip);

                    mapModelPopup.Add(dvm, tooltip);
                }
            }
        }

        void workspace_CloseButtonClick(object sender, UFInterfaces.CloseButtonEventArgs e)
        {
            if (!(e.TargetItem is FrameworkElement))
                return;

            FrameworkElement view = e.TargetItem as FrameworkElement;
            TreeViewItemViewModel dvm = view.DataContext as TreeViewItemViewModel;
            if (dvm == null)
                return;

            FrameworkElement popup;
            if (mapModelPopup.TryGetValue(dvm, out popup))
                mapModelPopup.Remove(dvm);
        }

        void workspace_DockStateChanged(FrameworkElement sender, UFInterfaces.DockStateEventArgs e)
        {
            if (!(sender is FrameworkElement) || workspace == null || e.NewState != UFInterfaces.DockState.Hidden)
                return;

            FrameworkElement view = sender as FrameworkElement;
            TreeViewItemViewModel dvm = view.DataContext as TreeViewItemViewModel;
            if (dvm == null)
                return;

            FrameworkElement popup;
            if (mapModelPopup.TryGetValue(dvm, out popup))
            {
                mapModelPopup.Remove(dvm);
                workspace.RemoveDockingChildren(view);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            DetachOverrideBaseProperties();
            if (stringManager != null)
                stringManager.CultureChanged -= StringManager_CultureChanged;

            lock (staticLock)
            {
                if (timer != null)
                {
                    timerAquired = false;
                    timer.Stop();
                }
            }

            if (bLazyUI)
            {
                bLazyUI = false;

                SessionViewModel.listActiveSessions.CollectionChanged -= list_CollectionChanged;
                SubscriptionViewModel.listActiveSubscriptions.CollectionChanged -= list_CollectionChanged;
                MonitoredItemViewModel.listActiveMonitoredItems.CollectionChanged -= list_CollectionChanged;
                RealTimeConnectionManagerViewModel.listActiveRealTimeConnectionManagers.CollectionChanged -= list_CollectionChanged;

                if (workspace != null)
                {
                    workspace.CloseButtonClick -= workspace_CloseButtonClick;
                    workspace.DockStateChanged -= workspace_DockStateChanged;

                    foreach (var v in mapModelPopup.Values)
                        workspace.RemoveDockingChildren(v);
                }

                if (mapModelItem.Count > 0)
                {
                    var list = new List<TreeViewItemViewModel>();
                    list.AddRange(mapModelItem.Keys.ToList());
                    foreach (var dvm in list)
                        RemoveTreeItem(dvm);
                }

                mapModelItem.Clear();
                mapModelPopup.Clear();

                if (dpCollectionChanged != null &&
                    dpCollectionChanged.Status != DispatcherOperationStatus.Aborted &&
                    dpCollectionChanged.Status != DispatcherOperationStatus.Completed)
                    dpCollectionChanged.Abort();
            }

            if (!bIsPopup)
            {
                MonitoredItemViewModel[] ma = new MonitoredItemViewModel[MonitoredItemViewModel.listActiveMonitoredItems.Count];
                MonitoredItemViewModel.listActiveMonitoredItems.CopyTo(ma, 0);
                foreach (var m in ma)
                    m.Dispose();

                SubscriptionViewModel[] sa = new SubscriptionViewModel[SubscriptionViewModel.listActiveSubscriptions.Count];
                SubscriptionViewModel.listActiveSubscriptions.CopyTo(sa, 0);
                foreach (var s in sa)
                    s.Dispose();

                SessionViewModel[] va = new SessionViewModel[SessionViewModel.listActiveSessions.Count];
                SessionViewModel.listActiveSessions.CopyTo(va, 0);
                foreach (var v in va)
                    v.Dispose();
            }
        }

        #endregion
    }
}
