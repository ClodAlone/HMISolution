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
using System.Windows.Threading;
using Opc.Ua;
using System.ComponentModel;
using Utilities;
using System.Collections.Specialized;
using CommonControls;
using System.Reflection;
using System.Windows.Markup;
using Utilities.WPF;
using DevExpress.Xpf.Printing;


namespace OPCUAEventViewers
{
    /// <summary>
    /// Interaction logic for OPCUAAuditEventViewer.xaml
    /// </summary>
    public partial class OPCUAAuditEventViewer : UserControl, IDisposable
    {
        #region Declarations
        public SessionViewModel sessionViewModel { get; private set; }
        public SubscriptionViewModel Subscription { get; private set; }
        public NodeId AreaId { get; private set; }

        SafeObservableCollection<AuditEventStateViewModel> auditEventStateList;
        bool bSubscribed;

        DispatcherTimer timer;
        #endregion

        public OPCUAAuditEventViewer()
        {
            InitializeComponent();
            IntResources();
        }

        void IntResources()
        {
            var bDesignmode = DesignerProperties.GetIsInDesignMode(this);

            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, this, !bDesignmode);
            OPUAEVPrint.Glyph = TryFindResource("OPUAEVPrint") as ImageSource;
            OPUAEVCancl.Glyph = TryFindResource("OPUAEVCancl") as ImageSource;
            SetBusy(false);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is MonitoredItemViewModel)
            {
                SetBusy(true);

                var mivm = DataContext as MonitoredItemViewModel;

                if (mivm.monitoredItem.ResolvedNodeId == null)
                    AreaId = ObjectIds.Server;
                else
                    AreaId = mivm.monitoredItem.ResolvedNodeId;

                sessionViewModel = mivm.monitoredItem.Subscription.Session.Handle as SessionViewModel;
                if (sessionViewModel != null)
                {
                    if (!sessionViewModel.Connected)
                    {
                        sessionViewModel.AutoConnect = true;
                        sessionViewModel.PromoteIdleExecution(DispatcherPriority.Invalid);
                    }
                    else
                        AddSubscription();

                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(3);
                        timer.Tick += timer_Tick;
                        timer.Start();
                    }
                    else
                    {
                        timer.Stop();
                        timer.Start();
                    }
                }
            }
        }

        public OPCUAAuditEventViewer(SessionViewModel s)
        {
            sessionViewModel = s;
            sessionViewModel.PropertyChanged += sessionViewModel_PropertyChanged;

            AreaId = ObjectIds.Server;

            InitializeComponent();
            IntResources();

            SetBusy(true);

            if (!sessionViewModel.Connected)
            {
                sessionViewModel.AutoConnect = true;
                sessionViewModel.PromoteIdleExecution(DispatcherPriority.Invalid);
            }
            else
                AddSubscription();

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            else
            {
                timer.Stop();
                timer.Start();
            }
        }

        void SetBusy(bool bBusy)
        {
            if (bBusy)
            {
                gridAlarmList.Visibility = Visibility.Collapsed;

                busyContent.Visibility = Visibility.Visible;
                busyControl.Visibility = Visibility.Visible;
                IsBusyGrid.Visibility = Visibility.Visible;
            }
            else
            {
                gridAlarmList.Visibility = Visibility.Visible;

                busyContent.Visibility = Visibility.Collapsed;
                busyControl.Visibility = Visibility.Collapsed;
                IsBusyGrid.Visibility = Visibility.Collapsed;
            }
        }

        public BitmapImage GetControlImage()
        {
            BitmapImage bm = TryFindResource("OPUAEVLedger") as BitmapImage;
            return bm;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;
            }

            SetBusy(false);
        }

        private void sessionViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Connected" && sessionViewModel.Connected == true)
                AddSubscription();
        }

        private void AddSubscription()
        {
            if (auditEventStateList != null && bSubscribed)
            {
                auditEventStateList.CollectionChanged -= conditionStateList_CollectionChanged;
                bSubscribed = false;
            }

            if (sessionViewModel.CanCreateSubscription && Subscription == null)
                Subscription = sessionViewModel.CreateSubscription(Properties.Resources.AuditEventSubscriptionName);

            if (Subscription != null)
            {
                ExpandedNodeIdCollection list = new ExpandedNodeIdCollection();
                list.Add(AreaId);

                var map = Subscription.AddAuditingEventMonitoredItem(list);
                if (map.Count > 0)
                {
                    auditEventStateList = map[AreaId].AuditEventStateList;
                    gridAlarmList.Dispatcher.InvokeIfRequired(() =>
                        {
                            gridAlarmList.ItemsSource = auditEventStateList;
                            gridAlarmList.DataContext = auditEventStateList;
                        });
                    auditEventStateList.CollectionChanged += conditionStateList_CollectionChanged;
                    bSubscribed = true;
                }

                Subscription.ApplyChanges();
            }
        }

        void conditionStateList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Tick -= timer_Tick;
                    timer = null;
                }

                auditEventStateList.CollectionChanged -= conditionStateList_CollectionChanged;
                bSubscribed = false;

                gridAlarmList.Dispatcher.BeginInvokeInBackgroundIfRequired(() =>
                    {
                        SetBusy(false);
                    });
            }
        }

        /*
        private void grid_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e)
        {
            if ((e.Column != null || e.Column.FieldName == "AnimationElement") && auditEventStateList != null)
            {
                e.Value = auditEventStateList[e.ListSourceRowIndex];
            }
        }


        private void GridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (GridView.FocusedRowData.RowHandle.Value >= 0 && auditEventStateList != null &&
                GridView.FocusedRowData.RowHandle.Value < auditEventStateList.Count)
            {
                var mi = auditEventStateList[GridView.FocusedRowData.RowHandle.Value];

                GridView.ContextMenu = mi.contextMenu;
                if (GridView.ContextMenu != null)
                    GridView.ContextMenu.DataContext = mi;

                toolbar.DataContext = mi;
                toolbar.Visibility = Visibility.Visible;
            }
            else
            {
                toolbar.DataContext = null;
                toolbar.Visibility = Visibility.Collapsed;
                if (GridView.ContextMenu != null)
                    GridView.ContextMenu.DataContext = null;
                GridView.ContextMenu = null;
            }
        }
        */
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            UtilitiesPrintHelper.PrintControl(this.FindParent<Window>(), (IPrintableControl)gridAlarmList.View, Properties.Resources.AuditEventSubscriptionName, Properties.Resources.AuditEventSubscriptionName, false, System.Drawing.Printing.PaperKind.A4);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;
                timer = null;
            }

            if (sessionViewModel == null)
                return;

            if (Subscription != null)
            {
                sessionViewModel.RemoveSubscription(Subscription);
                Subscription = null;
            }

            if (auditEventStateList != null && bSubscribed)
            {
                auditEventStateList.CollectionChanged -= conditionStateList_CollectionChanged;
                bSubscribed = false;
            }
            auditEventStateList = null;

            sessionViewModel.PropertyChanged -= sessionViewModel_PropertyChanged;
            // sessionViewModel.Dispose();
            sessionViewModel = null;

            // gridAlarmList.Model.Dispose();
            try
            {
                gridAlarmList.Dispose();
            }
            catch (Exception ex)
            {
                
            }
        }

        #endregion
    }
}
