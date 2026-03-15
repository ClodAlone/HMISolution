using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Opc.Ua;
using OPCUABrowser.ComponentService;
using OPCUAEventViewers;
using OPCUAViewModel;
using Tracing.ComponentService;
using UFInterfaces;
using UIMsgBoxAlertService.ComponentService;
using Utilities;
using Utilities.Animations;
using Utilities.ProgressDialog;
using Utilities.WPF;
using ViewModelLib;
using System.Diagnostics;
using System.Windows.Data;
using WPFUtilities;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Core;
using IWorkspace = UFInterfaces.IWorkspace;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors;
using DevExpress.Data.TreeList;

namespace OPCUABrowser
{
    /// <summary>
    /// Interaction logic for OPCUABrowseServer.xaml
    /// </summary>
    public partial class OPCUABrowseServer : UserControl, IDisposable
    {
        readonly static object DummyNode = new Object();
        public SessionViewModel sessionViewModel { get; private set; }
        public BrowserViewModel Browser { get; private set; }
        public SubscriptionViewModel Subscription { get; private set; }
        public EndpointDescriptionViewModel endpointDescriptionViewModel { get; private set; }
        Dictionary<TreeItemControl, TreeListNode> contentToNodeMap = new Dictionary<TreeItemControl, TreeListNode>();

        readonly Dictionary<Type, Object> mapTypeTooltips = new Dictionary<Type, Object>();
        readonly SafeObservableCollection<MonitoredItemViewModel> listMonitoredItems = new SafeObservableCollection<MonitoredItemViewModel>();

        readonly OPCUABrowserComponent component;
        readonly IWorkspace workspace;
        readonly ISimpleLogging simpleLogging;

        readonly NodeId refereId;
        readonly ExpandedNodeId browseForTypeId;
        readonly bool bBrowseAlarmArea;
        readonly List<UIElement> transitioners = new List<UIElement>();

        public OPCUABrowseServer(EndpointDescriptionViewModel edvm,
                                 OPCUABrowserComponent c, NodeId nodeId, ExpandedNodeId browseforTypeId, 
                                 bool bAlarmArea, bool bIsEmbedded = false, bool allowMultiSelect = false)
        {
            component = c;
            if (!bIsEmbedded)
                workspace = c.Workspace;
            simpleLogging = c.SimpleLogging;
            refereId = nodeId;
            browseForTypeId = browseforTypeId;
            bBrowseAlarmArea = bAlarmArea;

            endpointDescriptionViewModel = edvm;
            CommonConstruct();

            if (bIsEmbedded)
                columnLive.Width = new GridLength(0);

            var prevSplitterWidth = columnLive.Width.Value;
            splitter.DragCompleted += (s, e) =>
            {
                if (prevSplitterWidth == 0.0 && columnLive.Width.Value > 0.0)
                    subitemSelected();
                else if (prevSplitterWidth > 0.0 && columnLive.Width.Value == 0.0)
                    subitemUnselected();
                prevSplitterWidth = columnLive.Width.Value;
            };

            if (allowMultiSelect)
                treeListControl.SelectionMode = MultiSelectMode.Row;
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

        void UpdateFolderIcon(TreeListNode node, bool isOpen)
        {
            if (node != null && node.Tag is IEntityReference)
            {
                var collapsedImage = (node.Tag as IEntityReference).CollapsedImageSource;
                var expandedImage = (node.Tag as IEntityReference).ExpandedImageSource;
                (node.Content as TreeItemControl).ResourceIcon = isOpen ? expandedImage == null ? collapsedImage : expandedImage : collapsedImage;
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

        void OnTreeNodeExpanding(object sender, TreeListNodeAllowEventArgs e)
        {
            if (e == null)
                return;

            TreeListNode item = e.Node;

            if (item == null || TreeListControlHelper.WasExpanded(item) || !(item.Tag is TreeViewItemViewModel) ||
                !TreeListControlHelper.CanBeExpanded(item)) //Node's subtree already populated
                return;

            e.Handled = true;
            ExpandedItem(item);
        }

        void ClearDummyNode(TreeListNode node)
        {
            List<TreeListNode> dummyNodes = (from n in node.Nodes where n.Tag == TreeListControlHelper.DummyNode select n).ToList();
            dummyNodes.ForEach(dnode => { node.Nodes.Remove(dnode); });
        }


        bool bLoaded;
        bool bFirstTimeLoaded;
        private void CommonConstruct()
        {
            sessionViewModel = SessionViewModel.FindOrCreate(Properties.Resources.SessionBrowseName, endpointDescriptionViewModel);
            DataContext = sessionViewModel;
            sessionViewModel.PropertyChanged += sessionViewModel_PropertyChanged;
            InitializeComponent();

            gridControl.SelectionChanged += CurrencyManager_CurrentRecordSelectionChanged;

            if (!sessionViewModel.Connected)
            {
                treeListView.Nodes.Clear();
                contentToNodeMap.Clear();
                treeListControl.BeginDataUpdate();
                try
                {
                    var ic = new TreeItemControl(Properties.Resources.ConnectingMessage);
                    var newitem = treeListControl.AddNode(ic);
                    contentToNodeMap.Add(ic, newitem);
                }
                finally
                {
                    treeListControl.EndDataUpdate();
                }

                sessionViewModel.AutoConnect = true;
                sessionViewModel.PromoteIdleExecution(DispatcherPriority.Invalid);
            }
            else
            {
                BrowseServer();
                treeListView.IsEnabled = true;
            }

            Loaded += (o, e) =>
                {
                    if (!bLoaded)
                    {
                        bLoaded = true;
                        foreach (UIElement uie in ContainerContents.Children)
                            transitioners.Add(uie);
                        ContainerContents.Children.Clear();

                        _transContainer.control = transitioners[1];

                        _transContainerTree.control = transitioners[2];
                    }
                };

            cbView.EditValueChanged += (o, e) =>
            {
                ViewDescription view = null;

                var editsettings = cbView.EditSettings as ComboBoxEditSettings;
                var selectedItem = (from ComboBoxEditItem item in editsettings.Items where item.Content == cbView.EditValue select item).FirstOrDefault();

                ReferenceDescription reference = selectedItem.Tag as ReferenceDescription;

                if (reference != null && !NodeId.IsNull(reference.NodeId))
                {
                    view = new ViewDescription();
                    view.ViewId = ExpandedNodeId.ToNodeId(reference.NodeId, sessionViewModel.NamespaceUris);
                    view.ViewVersion = 0;
                    view.Timestamp = DateTime.MinValue;
                }

                if (Browser != null)
                {
                    Browser.ViewDescription = view;
                    BrowseExecution();
                }
            };
        }

        private void BrowseView()
        {
            var references = sessionViewModel.ViewDescriptions;

            var editsettings = cbView.EditSettings as ComboBoxEditSettings;
            editsettings.Items.Clear();
            editsettings.Items.Add(
                new ComboBoxEditItem()
                {
                    Content = Properties.Resources.DisplayNoneOption,
                    Tag = new ReferenceDescription() { NodeId = ExpandedNodeId.Null, DisplayName = Properties.Resources.DisplayNoneOption }
                });

            if (references != null)
            {
                for (int ii = 0; ii < references.Count; ii++)
                {
                    editsettings.Items.Add(new ComboBoxEditItem()
                    {
                        Content = references[ii].DisplayName,
                        Tag = references[ii]
                    });
                }
            }

            cbView.EditValue = editsettings.Items.Count > 0 ? (editsettings.Items[0] as ComboBoxEditItem).Content : null;
        }

        private void BrowseServer()
        {
            try
            {
                using (new WaitCursor())
                {
                    if (Browser == null)
                    {
                        if (bBrowseAlarmArea)
                            Browser = sessionViewModel.CreateAreaBrowser(refereId);
                        else
                            Browser = sessionViewModel.CreateBrowser(refereId, browseForTypeId != null);
                        Browser.PropertyChanged += sessionViewModel_PropertyChanged;
                        BrowseTypes.DataContext =
                        BrowseEventTypes.DataContext =
                        BrowseViews.DataContext =
                        BrowseObjectTypes.DataContext =
                        BrowseDataTypes.DataContext =
                        BrowseVariableType.DataContext = Browser;
                    }

                    ProgressDialog dlg = new ProgressDialog();
                    // dlg.Owner = this.FindParent<Window>() ?? Application.Current.MainWindow;
                    dlg.ProgressBarIndeterminate = true;
                    dlg.DialogText = "Loading ...";
                    dlg.IsCancellingEnabled = false;

                    //start processing and submit the start value
                    if (dlg.RunWorkerThread(null, (o, ev) =>
                    {
                        try
                        {
                            if (browseForTypeId != null)
                            {
                                var list = Browser.BrowseForTypeDefinition(browseForTypeId);

                                Dispatcher.Invoke(
                                (Action)delegate
                                {
                                    foreach (var advm in list)
                                        AddTreeItem(advm, null);
                                }, DispatcherPriority.Send);
                            }
                            else
                            {
                                Browser.IsExpanded = true;

                                var list = Browser.Children;

                                Dispatcher.Invoke(
                                (Action)delegate
                                {
                                    foreach (var advm in list)
                                        AddTreeItem(advm, null);
                                }, DispatcherPriority.Send);
                            }
                        }
                        catch (Exception ex)
                        {
                            simpleLogging?.AddItem(String.Format(Properties.Resources.LogTitle, sessionViewModel.AppTitle), sessionViewModel.LastMessage);                            
                        }
                    }))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                simpleLogging?.AddItem(String.Format(Properties.Resources.LogTitle, sessionViewModel.AppTitle), sessionViewModel.LastMessage);
            }
        }

        void BrowseExecution()
        {
            treeListView.Nodes.Clear();
            contentToNodeMap.Clear();
            if (Browser != null)
            {
                Browser.Children.Clear();
                Browser.IsExpanded = false;
            }
            BrowseServer();
        }

        DispatcherOperation BrowsePending;
        void sessionViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Contains("BrowseFor"))
            {
                if (BrowsePending == null)
                {
                    Action action = () => BrowseExecution();
                    BrowsePending = Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
                    BrowsePending.Completed += (S, E) => BrowsePending = null;
                }
            }
            else if (String.Compare(e.PropertyName, "LastMessage", false) == 0)
            {
                if (workspace != null)
                {
                    Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            if (sessionViewModel != null && sessionViewModel.Connected == false)
                            {
                                if (StatusText.Visibility != Visibility.Visible)
                                    StatusText.Blink(3000, Double.NaN, Double.NaN, new SineEase() { EasingMode = EasingMode.EaseIn });
                                StatusTextBlock.Text = sessionViewModel.LastMessage;
                                StatusText.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                StatusText.Blink(-1, Double.NaN, Double.NaN, new BackEase() { EasingMode = EasingMode.EaseOut });
                                StatusText.Visibility = Visibility.Collapsed;
                            }

                            //if (sessionViewModel != null && sessionViewModel.Connected == false)
                            //{
                            //    TreeListNode subitem = new TreeListNode
                            //    {
                            //        Header = sessionViewModel.LastMessage
                            //    };
                            //    treeListView.Items.Clear();
                            //    treeListView.Items.Add(subitem);
                            //}

                            if (sessionViewModel != null && simpleLogging != null)
                                simpleLogging.AddItem(String.Format(Properties.Resources.LogTitle, sessionViewModel.AppTitle), sessionViewModel.LastMessage);
                        });
                }
            }
            else if (String.Compare(e.PropertyName, "Connected", false) == 0)
            {
                if (!CheckAccess())
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        new Action<object, PropertyChangedEventArgs>(sessionViewModel_PropertyChanged),
                        sender,
                        new object[] { e }
                    );
                    return;
                }

                if (sessionViewModel == null)
                    return;

                if (sessionViewModel.Connected == false)
                {
                    if (workspace != null)
                        workspace.ShowTaskBarTooltip("Server Disconnected", sessionViewModel.Title, 5000);

                    //treeListView.Items.Clear();
                    //TreeListNode subitem = new TreeListNode { Header = "Disconnected" };
                    //treeListView.Items.Add(subitem);
                    treeListView.IsEnabled = false;

                    sessionViewModel.AutoConnect = true;
                    sessionViewModel.PromoteIdleExecution(DispatcherPriority.Invalid);

                    //if (bGraphBrowser)
                    //{
                    //    ShowGraphBrowser_Click(null, null);
                    //    if (GraphicBrowser != null)
                    //    {
                    //        GraphicBrowser.Dispose();
                    //        GraphicBrowser = null;
                    //    }
                    //}
                }
                else
                {
                    RebrowseOnConnected();
                }
            }
        }

        private void RebrowseOnConnected()
        {
            try
            {
                if (!bFirstTimeLoaded)
                {
                    bFirstTimeLoaded = true;
                    treeListView.Nodes.Clear();
                    contentToNodeMap.Clear();
                    BrowseView();
                    BrowseServer();
                }
                else if (Browser != null)
                    Browser.browser.Session = sessionViewModel.Session;

                treeListView.IsEnabled = true;

                if (StatusText.Visibility != Visibility.Collapsed)
                {
                    StatusText.Blink(-1, Double.NaN, Double.NaN, new BackEase() { EasingMode = EasingMode.EaseOut });
                    StatusText.Visibility = Visibility.Collapsed;
                }

                if (workspace != null)
                    workspace.ShowTaskBarTooltip("Server Connected", sessionViewModel.Title, 5000);
            }
            catch (Exception ex)
            {
                if (BrowsePending == null)
                {
                    Action action = () => RebrowseOnConnected();
                    BrowsePending = Dispatcher.BeginInvoke(action, DispatcherPriority.ApplicationIdle);
                    BrowsePending.Completed += (S, E) => BrowsePending = null;
                }                
            }
        }

        private Object GetTooltip(TreeViewItemViewModel dvm)
        {
            Object ret = null;
            if (mapTypeTooltips.TryGetValue(dvm.GetType(), out ret))
                return ret;

            if (dvm is IEntityReference)
                ret = (dvm as IEntityReference).Tooltip;
            if (ret != null)
                mapTypeTooltips.Add(dvm.GetType(), ret);

            return ret;
        }

        Dictionary<TreeViewItemViewModel, TreeListNodeCollection> mapModelList = new Dictionary<TreeViewItemViewModel, TreeListNodeCollection>();
        Dictionary<TreeViewItemViewModel, TreeListNode> mapModelItem = new Dictionary<TreeViewItemViewModel, TreeListNode>();
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

            string header = GetItemTitle(dvm);
            treeListControl.BeginDataUpdate();
            try
            {
                var ic = new TreeItemControl(tag,header);
                var newitem = treeListControl.AddNode(ic, parent, tag);
                treeListControl.AddNode(null, newitem, TreeListControlHelper.DummyNode);

                mapModelList.Add(dvm, newitem.Nodes);
                mapModelItem.Add(dvm, newitem);

                UpdateFolderIcon(newitem, false);
            }
            finally
            {
                treeListControl.EndDataUpdate();
            }
        }

        bool bDobuleClick;
        private void Tree_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            bDobuleClick = true;
        }

        private void Tree_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvokeAsynchronouslyInBackground(() =>
            {
                treeListView.Focusable = true;
                treeListView.Focus();
            });

            e.Handled = false;
            if (!bDobuleClick)
                return;
            bDobuleClick = false;

            e.Handled = true;
            var selected = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (selected != null && workspace == null)
            {
                var wnd = this.FindParent<Window>();
                if (wnd != null)
                {
                    wnd.DialogResult = true;
                    wnd.Close();
                }
            }
        }

        private String GetItemTitle(TreeViewItemViewModel dvm)
        {
            String ret;
            if (browseForTypeId != null)
            {
                ReferenceDescriptionViewModel rdvm = dvm as ReferenceDescriptionViewModel;
                if (String.IsNullOrEmpty(rdvm.RelativePath))
                    ret = dvm.Title;
                else
                    ret = String.Format("{0}/{1}", rdvm.RelativePath, dvm.Title);
            }
            else
                ret = dvm.Title;

            return ret;
        }

        void subitemUnselected()
        {
            if (Browser != null && Browser.BrowseForDataTypes)
                return;

            AttributesViewControl.ItemsSource = null;
            if (workspace != null)
                workspace.ContextObject = null;
            if (Subscription == null)
                return;

            listMonitoredItems.Clear();
            Subscription.RemoveMonitoredItems();
            Subscription.ApplyChanges();

            var listSelected = treeListControl.GetSelectedNodes();
            if (listSelected.Count() > 1)
            {
                if (workspace != null)
                    workspace.ContextObject = null;
                return;
            }

            TooltipContent.Content = null;
        }

        void LoginHandler(object sender, RoutedEventArgs e)
        {
            ShowCredentialOptions options = new ShowCredentialOptions();
            options.WindowTitle = sessionViewModel.Title;
            options.MainInstruction = Properties.Resources.ShowCredentialMainInstruction;
            options.Content = Properties.Resources.ShowCredentialContent;
            options.SavedCredentialsBucket = sessionViewModel.Title;
            options.ShowSaveCheckBox = true;
            options.ShowUIForSavedCredentials = true;
            options.SavedCredentialsBucket = Properties.Resources.SessionBrowseName;

            if (component.UIInterface != null)
            {
                ShowCredentialResults res = component.UIInterface.ShowCredentialDialog(options);
                if (res.result == CustomDialogResults.OK && !String.IsNullOrEmpty(res.UserName))
                {
                    try
                    {
                        sessionViewModel.RenewUserIdentity(new UserIdentity(res.UserName, res.Password), new StringCollection());
                    }
                    catch (Exception ex)
                    {
                        component.UIInterface.ShowError(String.Format(Properties.Resources.UserAuthenticationFailed, ex.Message));
                    }
                }
            }
        }

        void treeListControl_SelectionChanged(object sender, TreeListSelectionChangedEventArgs e)
        {
            e.Handled = true;

            subitemUnselected();
            subitemSelected();
        }

        DispatcherTimer delay;
        void subitemSelected()
        {
            if (columnLive.Width.Value == 0.0)
                return;
            else if (Browser != null && Browser.BrowseForDataTypes)
                return;
            var listSelected = treeListControl.GetSelectedNodes();
            if (listSelected.Count() > 1)
            {
                if (workspace != null)
                    workspace.ContextObject = null;
                return;
            }
            
            if (delay == null)
            {
                delay = new DispatcherTimer();
                delay.Interval = TimeSpan.FromMilliseconds(400);
                delay.Tick += delay_Tick;
                delay.Start();
            }
            else
            {
                delay.Stop();
                delay.Start();
            }
        }
        void delay_Tick(object sender, EventArgs e)
        {
            delay.Stop();
            TreeListNode subitem = treeListControl.GetSelectedNodes().FirstOrDefault();
            if (subitem == null)
                return;
            ReferenceDescriptionViewModel dvm = subitem.Tag as ReferenceDescriptionViewModel;

            ProgressDialog dlg = new ProgressDialog();
            // dlg.Owner = this.FindParent<Window>() ?? Application.Current.MainWindow;
            dlg.ProgressBarIndeterminate = true;
            dlg.DialogText = "Reading Attributes...";
            dlg.IsCancellingEnabled = false;

            //start processing and submit the start value
            if (dlg.RunWorkerThread(dvm, (o, ev) =>
            {
                var list = dvm.ReadableAttributesList;

                Dispatcher.Invoke(
                (Action)delegate
                {
                    AttributesViewControl.ItemsSource = list;
                    bool bTypeDef = dvm.TypeDefinition != null; // just use to fetch type definition
                }, DispatcherPriority.Send);
            }))
            {
            }

            /*
            if (treeListView.SelectedItems.Count > 1)
            {
                BrowseSelectedTypeDef.IsEnabled = BrowseAllItemsSelectedTypeDef.IsEnabled = false;
                List<Object> list = new List<Object>();
                foreach (TreeListNode tv in treeListView.SelectedItems)
                {
                    if (tv.Tag != null)
                        list.Add(tv.Tag);
                }
                if (workspace != null)
                    workspace.ContextObjects = list;
            }
            else if (treeListView.SelectedItem != null && (treeListView.SelectedItem is TreeListNode))
            {
                BrowseSelectedTypeDef.IsEnabled = BrowseAllItemsSelectedTypeDef.IsEnabled = dvm.TypeDefinition != null;
                if (workspace != null)
                    workspace.ContextObject = subitem.Tag;
            }
            */

            var obj = GetTooltip(dvm) as FrameworkElement;
            TooltipContent.Content = obj;
            obj.DataContext = subitem.Tag;

            if (ShowLiveData.IsChecked == false || dvm == null)
                return;

            subitem.IsExpanded = true;

            if (dvm.ChildrenVariables.Count == 0)
                return;

            // using (new WaitCursor())

            ProgressDialog dlgItms = new ProgressDialog();
            // dlgItms.Owner = this.FindParent<Window>() ?? Application.Current.MainWindow;
            dlgItms.ProgressBarIndeterminate = true;
            dlgItms.DialogText = "Subscribing to items...";
            dlgItms.IsCancellingEnabled = false;

            //start processing and submit the start value
            if (dlgItms.RunWorkerThread(dvm, (o, ev) =>
            {
                if (sessionViewModel.CanCreateSubscription && Subscription == null)
                    Subscription = sessionViewModel.CreateSubscription(Properties.Resources.BrowseSubscriptionName);

                if (Subscription != null)
                {
                    Subscription.AddMonitoredItem(dvm.ChildrenVariables);
                    Subscription.ApplyChanges();
                }

                Dispatcher.Invoke(
                (Action)delegate
                {
                    listMonitoredItems.Clear();
                    using (var updater = new CollectionUpdater(listMonitoredItems))
                    {
                        foreach (TreeViewItemViewModel tvi in Subscription.Children)
                            listMonitoredItems.Add(tvi as MonitoredItemViewModel);
                    }

                    gridControl.BeginDataUpdate();
                    gridControl.ItemsSource = listMonitoredItems;
                    gridControl.EndDataUpdate();
                }, DispatcherPriority.Send);

                //ColumnValues.Width = new GridLength(3, GridUnitType.Star);
                //ColumnSplitter.Width = new GridLength(5, GridUnitType.Pixel);
            }))
            {
            }

            /*
#if DEBUG
            var mapToCheck = new Dictionary<String, ExpandedNodeId>();
            List<String> arrayToCheck = new List<String>();
            foreach(var refer in dvm.ChildrenVariables)
            {
                ReferenceDescriptionViewModel rdvm = refer as ReferenceDescriptionViewModel;
                String rdvmCompletePath = rdvm.CompletePath;
                if (String.IsNullOrEmpty(rdvmCompletePath))
                    continue;

                arrayToCheck.Add(rdvmCompletePath);
                mapToCheck.Add(rdvm.CompletePath, rdvm.NodeId);
            }

            try
            {
                var ret = sessionViewModel.GetNodeIds(refereId, sessionViewModel.NamespaceUris, arrayToCheck.ToArray());
                arrayToCheck.ForEach(nodeid =>
                    {
                        if (ret.ContainsKey(nodeid))
                        {
                            if (ret[nodeid] != mapToCheck[nodeid])
                                if (workspace != null)
                                    MessageBox.Show(String.Format(Properties.Resources.CheckNodeIdsDifferent,
                                                nodeid, ret[nodeid], mapToCheck[nodeid]));
                        }
                        else if (workspace != null)
                            MessageBox.Show(String.Format(Properties.Resources.CheckNodeIdsNotFound, nodeid));
                    });
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());

                Debug.Fail(ex.ToString());
            }
#endif
             */
        }

        /*
        private void colValue_Validate(object sender, GridCellValidationEventArgs e)
        {
            if (GridView.FocusedRowData.RowHandle.Value >= 0 && GridView.FocusedRowData.RowHandle.Value < listMonitoredItems.Count)
            {
                MonitoredItemViewModel mi = listMonitoredItems[GridView.FocusedRowData.RowHandle.Value];
                if (mi.referenceItem.IsUserWritable)
                    mi.WriteValue(e.Value);
            }
        }
        */

        private void treeViewDiscovering_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5)
                return;

            e.Handled = true;
            using (new WaitCursor(this))
            {
                var selectedItem = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
                if (selectedItem == null)
                    return;
                var dvm = selectedItem.Tag as TreeViewItemViewModel;
                if (dvm == null)
                    return;
                // selectedItem.IsExpanded = false;
                // selectedItem.Items.Clear();
                // selectedItem.Items.Add(DummyNode);
                dvm.Children.Clear();

                var dvmReference = selectedItem.Tag as ReferenceDescriptionViewModel;
                if (dvmReference != null)
                    dvmReference.ChildrenVariables.Clear();
                
                dvm.IsExpanded = false;
                // selectedItem.IsExpanded = true;
                ExpandedItem(selectedItem);
            }
        }

        void ExpandedItem(TreeListNode item)
        {
            if (item == null || TreeListControlHelper.WasExpanded(item)) //Node's subtree already populated
                return;

            if (!TreeListControlHelper.CanBeExpanded(item))
            {
                if (item.Tag is IEntityReference)
                    UpdateFolderIcon(item, true);
                return;
            }

            (item.Content as TreeItemControl).IsNodeExpanding = true;
            treeListControl.BeginDataUpdate();
            try
            {
                ClearNodes(item);
                TreeViewItemViewModel dvm = item.Tag as TreeViewItemViewModel;

                ProgressDialog dlg = new ProgressDialog();
                // dlg.Owner = this.FindParent<Window>() ?? Application.Current.MainWindow;
                dlg.ProgressBarIndeterminate = true;
                dlg.DialogText = "Loading...";
                dlg.IsCancellingEnabled = false;

                //start processing and submit the start value
                if (dlg.RunWorkerThread(item, (o, ev) =>
                {
                    //the sender property is a reference to the dialog's BackgroundWorker
                    //component
                    BackgroundWorker worker = (BackgroundWorker)o;

                    //the start value was submitted as an argument
                    Dispatcher.Invoke(
                    (Action)delegate
                    {
                        dvm = (TreeViewItemViewModel)item.Tag;
                        dvm.IsExpanded = true;
                    }, DispatcherPriority.Send);

                    SortedDictionary<String, TreeViewItemViewModel> sortedMap = new SortedDictionary<String, TreeViewItemViewModel>();
                    foreach (var advm in dvm.Children)
                    {
                        var title = GetItemTitle(advm);
                        while (sortedMap.ContainsKey(title))
                            title = string.Format("{0} ", title);
                        sortedMap.Add(title, advm);
                    }

                    int i = 0;
                    foreach (var advm in sortedMap.Values)
                    //    AddTreeItem(advm, item.Items);

                    //foreach (var advm in dvm.Children)
                    {
                        //increment the value
                        //if the user cancelled, break
                        if (worker.CancellationPending)
                            break;

                        // dlg.UpdateProgress(i);
                        worker.ReportProgress(int.MinValue, advm.Title);
                        Dispatcher.Invoke(
                        (Action)delegate
                        {
                            AddTreeItem(advm, item);
                        }, DispatcherPriority.Send);
                    }
                }))
                {
                }
            }
            finally
            {
                treeListControl.EndDataUpdate();
                (item.Content as TreeItemControl).IsNodeExpanding = false;
            }
        }

        void ClearNodes(TreeListNode node)
        {
            node.Nodes.Clear();
        }

        void ClearObjectMapNode(TreeListNode node)
        {
            if (node.Content is TreeItemControl && contentToNodeMap.ContainsKey(node.Content as TreeItemControl))
            {
                var content = node.Content as TreeItemControl;
                contentToNodeMap.Remove(content);
            }

            foreach (var child in node.Nodes)
                ClearObjectMapNode(child);
        }

        void OnTreeNodeChanged(object sender, TreeListNodeChangedEventArgs e)
        {
            if (e.ChangeType == NodeChangeType.Remove && e.Node.Tag != null && e.Node.Tag != TreeListControlHelper.DummyNode)
                ClearObjectMapNode(e.Node);
        }

        void CurrencyManager_CurrentRecordSelectionChanged(object sender, DevExpress.Xpf.Grid.GridSelectionChangedEventArgs e)
        {
            MonitoredItemViewModel mi = gridControl.SelectedItem as MonitoredItemViewModel;
            if (mi == null)
                return;
            if (CustomFontHelper.CanApplyCustomFont())
                (from object item in mi.contextMenu.Items where item is Control select item as Control).ToList().ForEach((item) =>
                {
                    item.FontSize = CustomFontHelper.GetCustomFontSize();
                    item.FontFamily = CustomFontHelper.GetCustomFontFamily();
                });
            gridControl.ContextMenu = mi.contextMenu;
            //GridRangeInfo range = e.Range.ExpandRange(0, 0, gridControl.Model.RowCount, gridControl.Model.ColumnCount);
            //int row, col;
            //if (range.GetFirstCell(out row, out col))
            //{
            //    MonitoredItemViewModel mi = listMonitoredItems[row];
            //    gridControl.ContextMenu = mi.contextMenu;

            //if (workspace != null)
            //    workspace.ContextObject = mi;

            //    //gridControl.Model.Columns["Value"].AllowEditing = mi.referenceItem.IsUserWritable ?
            //    //    DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
            //}
        }

        /*
        private void GridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (GridView.FocusedRowData.RowHandle.Value >= 0 && GridView.FocusedRowData.RowHandle.Value < listMonitoredItems.Count)
            {
                MonitoredItemViewModel mi = listMonitoredItems[GridView.FocusedRowData.RowHandle.Value];
                gridControl.ContextMenu = mi.contextMenu;
                if (GridView.ContextMenu != null)
                    GridView.ContextMenu.DataContext = mi;

                if (workspace != null)
                    workspace.ContextObject = mi;

                gridControl.Columns["Value"].AllowEditing = mi.referenceItem.IsUserWritable ?
                    DevExpress.Utils.DefaultBoolean.True : DevExpress.Utils.DefaultBoolean.False;
            }
        }

        private void GridView_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
        }
        */

        //bool bGraphBrowser;
        //OPCUAGraphicBrowser GraphicBrowser;
        //UIElement previouscontent;
        //private void ShowGraphBrowser_Click(object sender, RoutedEventArgs e)
        //{
        //    if (e != null)
        //        e.Handled = true;
        //    bGraphBrowser = !bGraphBrowser;
        //    ShowGraph.IsChecked = bGraphBrowser;
        //    if (!bGraphBrowser)
        //    {
        //        _transContainerTree.control = transitioners[2];
        //    }
        //    else
        //    {
        //        if (GraphicBrowser == null)
        //            GraphicBrowser = new OPCUAGraphicBrowser(sessionViewModel.CreateBrowser(refereId, browseForTypeId != null), component);
        //        _transContainerTree.control = GraphicBrowser;
        //    }
        //}

        bool bLiveData = true;
        private void ShowLiveData_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            bLiveData = !bLiveData;
            ShowLiveData.IsChecked = bLiveData;

            var prev = _transContainer.control as UIElement;
            if (prev == transitioners[0])
                _transContainer.control = transitioners[1];
            else
                _transContainer.control = transitioners[0];
            e.Handled = true;
            subitemUnselected();
            subitemSelected();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            UtilitiesPrintHelper.PrintControl(this.FindParent<Window>(), (IPrintableControl)gridControl.View, Properties.Resources.PrintDialogTitle, Properties.Resources.PrintDialogWndTitle, false, System.Drawing.Printing.PaperKind.A4);
            //VisualDataNodeLink link = new VisualDataNodeLink((IRootDataNodeSource)GridView, "Grid Document");
            //PrintPreviewWindow window = new PrintPreviewWindow(LayoutHelper.FindParentObject<FrameworkElement>(this), link.PrintingSystem)
            //{
            //    Caption = "Grid Preview"
            //};
            //link.CreateDocument(true);
            //window.Show();
        }

        public OPCUAEntityReference GetSelectedEntityReference()
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (item == null)
                return null;

            var dvm = item.Tag as ReferenceDescriptionViewModel;

            var bAllow = IsDraggable(dvm);
            if (!bAllow)
                return null;

            return dvm.CreateEntityReference(null);
        }

        public OPCUAEntityReferenceList GetSelectedEntityReferenceList()
        {
            OPCUAEntityReferenceList opcuaEntityReferences = null;
            foreach (var selectedItem in treeListControl.GetSelectedNodes())
            {
                if (!(selectedItem is TreeListNode))
                    continue;

                var item = selectedItem as TreeListNode;
                var dvm = item.Tag as ReferenceDescriptionViewModel;

                var bAllow = IsDraggable(dvm);
                if (!bAllow)
                    continue;

                var entityReference = dvm.CreateEntityReference(null);
                if (entityReference == null)
                    continue;

                if (opcuaEntityReferences == null)
                    opcuaEntityReferences = new OPCUAEntityReferenceList();

                opcuaEntityReferences.Add(entityReference);
            }

            return opcuaEntityReferences;
        }

        private void BrowseSelectedTypeDef_Click(object sender, RoutedEventArgs e)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (workspace == null || item == null)
                return;

            ReferenceDescriptionViewModel dvm = item.Tag as ReferenceDescriptionViewModel;

            NodeId idToBrowse = (NodeId)dvm.NodeId;
            ExpandedNodeId browsefortypedefId = null;
            String title = dvm.Title;
            idToBrowse = (NodeId)dvm.TypeDefinition;

            title = String.Format(Properties.Resources.TypeDefinitionFor,
                (dvm.NodeReferences[0] as ReferenceDescriptionViewModel).DisplayName.ToString());

            OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                    component, idToBrowse, browsefortypedefId, false);

            GeneralDialogContent dlg = new GeneralDialogContent(serverbrowser)
            {
                // Owner = this.FindParent<Window>() ?? Application.Current.MainWindow,
                Title = title,
                HelpLink = "BrowseSelectedType"
            };
            if (dlg.ShowDialog() != true)
                return;

            serverbrowser.Dispose();

            //workspace.SetDesiredHeightAndWidthInDockedMode(serverbrowser, serverbrowser.Height, serverbrowser.Width);
            //serverbrowser.ClearValue(FrameworkElement.WidthProperty);
            //serverbrowser.ClearValue(FrameworkElement.HeightProperty);

            //BitmapImage bm = OPCUABrowseServer.GetControlImage();
            //workspace.SetDockedElementIcon(serverbrowser, new ImageBrush(bm));

            //workspace.AddDockingChildren(serverbrowser, Title,
            //                                DockState.Document, DockSide.Tabbed);
            //workspace.ActivateDockedElement(serverbrowser);
        }

        private void BrowseAllItemsSelectedTypeDef_Click(object sender, RoutedEventArgs e)
        {
            var item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (workspace == null || item == null)
                return;

            ReferenceDescriptionViewModel dvm = item.Tag as ReferenceDescriptionViewModel;

            NodeId idToBrowse = (NodeId)dvm.NodeId;
            ExpandedNodeId browsefortypedefId = null;
            String Title = dvm.Title;
            browsefortypedefId = dvm.TypeDefinition;
            idToBrowse = null;

            Title = String.Format(Properties.Resources.AllObjectTypesFor,
                (dvm.NodeReferences[0] as ReferenceDescriptionViewModel).DisplayName.ToString());

            OPCUABrowseServer serverbrowser = new OPCUABrowseServer(endpointDescriptionViewModel,
                                                                    component, idToBrowse, browsefortypedefId, false);
            workspace.SetDesiredHeightAndWidthInDockedMode(serverbrowser, serverbrowser.Height, serverbrowser.Width);
            serverbrowser.ClearValue(FrameworkElement.WidthProperty);
            serverbrowser.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = OPCUABrowserComponent.GetControlImage("OPCBOPCClient");

            workspace.AddDockingChildren(serverbrowser, Title,
                                            UFInterfaces.DockState.Document, UFInterfaces.DockSide.Tabbed,
                                            itemID: String.Format("{0}_{1}", nameof(OPCUABrowseServer), (dvm.NodeReferences[0] as ReferenceDescriptionViewModel).DisplayName.ToString()));
            workspace.SetDockedElementIcon(serverbrowser, new ImageBrush(bm));
            workspace.ActivateDockedElement(serverbrowser);
        }

        private void treeView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TreeListNode item = treeListControl.GetSelectedNodes().FirstOrDefault() as TreeListNode;
            if (item == null)
                return;
            if (workspace == null || !bBrowseAlarmArea || e.ClickCount <= 1)
                return;

            e.Handled = true;

            ReferenceDescriptionViewModel dvm = item.Tag as ReferenceDescriptionViewModel;
            SessionViewModel sessionViewModel = SessionViewModel.FindOrCreate(Properties.Resources.SessionBrowseName, endpointDescriptionViewModel);
            OPCUAAlarmViewer alarmViewer = new OPCUAAlarmViewer(sessionViewModel, (NodeId)dvm.NodeId);

            workspace.SetDesiredHeightAndWidthInDockedMode(alarmViewer, alarmViewer.Height, alarmViewer.Width);
            alarmViewer.ClearValue(FrameworkElement.WidthProperty);
            alarmViewer.ClearValue(FrameworkElement.HeightProperty);

            BitmapImage bm = alarmViewer.GetControlImage();

            workspace.AddDockingChildren(alarmViewer, endpointDescriptionViewModel.endpointDescription.Server.ApplicationName.ToString(),
                                            UFInterfaces.DockState.Document, UFInterfaces.DockSide.Tabbed,
                                            itemID: String.Format("{0}_{1}", nameof(OPCUAAlarmViewer), endpointDescriptionViewModel.endpointDescription.Server.ApplicationName.ToString()));
            workspace.SetDockedElementIcon(alarmViewer, new ImageBrush(bm));
            workspace.ActivateDockedElement(alarmViewer);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (delay != null)
            {
                delay.Stop();
                delay.Tick -= delay_Tick;
                delay = null;
            }

            if (sessionViewModel == null)
                return;

            if (Browser != null)
            {
                Browser.PropertyChanged -= sessionViewModel_PropertyChanged;
                Browser.Dispose();
                Browser = null;
            }

            //if (GraphicBrowser != null)
            //{
            //    GraphicBrowser.Dispose();
            //    GraphicBrowser = null;
            //}

            if (Subscription != null)
            {
                sessionViewModel.RemoveSubscription(Subscription);
                Subscription = null;
            }

            sessionViewModel.PropertyChanged -= sessionViewModel_PropertyChanged;
            sessionViewModel.Dispose();
            sessionViewModel = null;

            //gridControl.Model.Dispose();
            try
            {
                gridControl.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion

        internal void Browsefor(NodeId node)
        {
            if (Browser == null)
                return;

            Browser.rootId = node;
        }

        #region Drag & Drop

        private void TreeListControl_DragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            return;
        }

        List<TreeListNode> draggedNodes = new List<TreeListNode>();
        private void TreeListControl_DragStart(object sender, StartRecordDragEventArgs e)
        {
            draggedNodes.Clear();

            if (e.Records == null || e.Records.Count() == 0)
            {
                e.AllowDrag = false;
                e.Handled = true;
                return;
            }

            foreach (var c in e.Records)
            {
                if (c as TreeItemControl != null && contentToNodeMap.ContainsKey(c as TreeItemControl))
                    draggedNodes.Add(contentToNodeMap[c as TreeItemControl]);
            }

            bool bAllow = true;
            foreach (var dragItem in draggedNodes)
            {
                if (dragItem.Tag == null || dragItem.Tag is string || dragItem.Tag is ReferenceDescriptionViewModel)
                {
                    var model = dragItem.Tag as ReferenceDescriptionViewModel;
                    bAllow = IsDraggable(model);
                }
            }

            if (!bAllow)
            {
                e.AllowDrag = false;
                e.Handled = true;
                draggedNodes.Clear();
                return;
            }
        }

        private void TreeListControl_DragEnd(object sender, DropRecordEventArgs e)
        {
            e.Handled = true;
        }

        void TreeListControl_CompletedDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        bool IsDraggable(ReferenceDescriptionViewModel model)
        {
            try
            {
                var entity = model.CreateEntityReference(null);

                var path = RelativePath.Parse(
                    entity.RelativePath,
                    sessionViewModel.Session.TypeTree,
                    sessionViewModel.NamespaceUris,
                    sessionViewModel.NamespaceUris);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}