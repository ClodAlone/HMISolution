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
    /// Interaction logic for OPCUAAlarmViewer.xaml
    /// </summary>
    public partial class OPCUAAlarmViewer : UserControl, IDisposable
    {
        #region Declarations
        public SessionViewModel sessionViewModel { get; private set; }
        public SubscriptionViewModel Subscription { get; private set; }
        public NodeId AreaId { get; private set; }

        SafeObservableCollection<ConditionStateViewModel> conditionStateList;
        bool bSubscribed;

        DispatcherTimer timer;
        #endregion

        public OPCUAAlarmViewer()
        {
            InitializeComponent();
            IntResources();
        }

        void IntResources()
        {
            var bDesignmode = DesignerProperties.GetIsInDesignMode(this);
            ThemeImageHelper.LoadBitmapImageResourceDictionary(this.Resources, this, !bDesignmode);
            OPUAEVAccount.Glyph = TryFindResource("OPUAEVAccount") as ImageSource;
            OPUAEVCash.Glyph = TryFindResource("OPUAEVCash") as ImageSource;
            OPUAEVCategorySmall.Glyph = TryFindResource("OPUAEVCategorySmall") as ImageSource;
            OPUAEVChecking.Glyph = TryFindResource("OPUAEVChecking") as ImageSource;
            OPUAEVLedger.Glyph = TryFindResource("OPUAEVLedger") as ImageSource;
            OPUAEVWireTransfer.Glyph = TryFindResource("OPUAEVWireTransfer") as ImageSource;
            OPUAEVprint.Glyph = TryFindResource("OPUAEVprint") as ImageSource;
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
        public OPCUAAlarmViewer(SessionViewModel s, NodeId area)
        {
            sessionViewModel = s;
            sessionViewModel.PropertyChanged += sessionViewModel_PropertyChanged;

            if (area == null)
                AreaId = ObjectIds.Server;
            else
                AreaId = area;

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

        public BitmapImage GetControlImage()
        {
            BitmapImage bm = TryFindResource("OPUAEVCash") as BitmapImage;
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
            if (conditionStateList != null && bSubscribed)
            {
                conditionStateList.CollectionChanged -= conditionStateList_CollectionChanged;
                bSubscribed = false;
            }

            if (sessionViewModel.CanCreateSubscription && Subscription == null)
                Subscription = sessionViewModel.CreateSubscription(Properties.Resources.AlarmSubscriptionName);

            if (Subscription != null)
            {
                ExpandedNodeIdCollection list = new ExpandedNodeIdCollection();
                list.Add(AreaId);

                var map = Subscription.AddAlarmEventMonitoredItem(list, null, null);
                if (map.Count > 0)
                {
                    conditionStateList = map[AreaId].ConditionStateList;
                    gridAlarmList.Dispatcher.InvokeIfRequired(() =>
                        {
                            gridAlarmList.ItemsSource = conditionStateList;
                            gridAlarmList.DataContext = conditionStateList;
                        });

                    conditionStateList.CollectionChanged += conditionStateList_CollectionChanged;
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

                conditionStateList.CollectionChanged -= conditionStateList_CollectionChanged;
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
            if ((e.Column != null || e.Column.FieldName == "AnimationElement") && conditionStateList != null)
            {
                e.Value = conditionStateList[e.ListSourceRowIndex];
            }
        }


        private void GridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (GridView == null)
                return;

            if (GridView.FocusedRowData.RowHandle.Value >= 0 && conditionStateList != null &&
                GridView.FocusedRowData.RowHandle.Value < conditionStateList.Count)
            {
                var mi = conditionStateList[GridView.FocusedRowData.RowHandle.Value];

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

            if (conditionStateList != null && bSubscribed)
            {
                conditionStateList.CollectionChanged -= conditionStateList_CollectionChanged;
                bSubscribed = false;
            }
            conditionStateList = null;

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
