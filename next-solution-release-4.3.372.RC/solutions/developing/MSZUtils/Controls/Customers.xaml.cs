using DevExpress.Xpo;
using System.IO.IsolatedStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using Utilities;
using UFInterfaces.PropertyControl;
using CommonControls.PropertyDataTemplate;
using System.Data.SqlTypes;
using StringManager.ComponentService;
using System.Globalization;
using DocumentManager.ComponentService;
using Converters;
using System.Windows.Data;
using WPFUtilities.PropertyDataTemplate;
using Utilities.WPF;
using WPFUtilities;
using GridLayout;
using UIMsgBoxAlertService.ComponentService;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Grid;
using System.Data;
using DevExpress.Xpf.Core;
using MSZUtils.Helpers;
using System.Diagnostics;
using MSZUtilsServiceHelper;
using System.Windows.Markup;
using System.Threading;

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for HistoricalEvents.xaml
    /// </summary>
    public partial class Customers : DXWindow, IDisposable
    {
        #region Contructor
        bool bLoaded;
        List<Customer> customers;
        WebRequestManager webRequestManager;
        CancellationTokenSource tokenSource;
        CancellationToken ct;
        TaskScheduler sc = null;
        List<Task> pendingTask = new List<Task>();
        string currentStyle;
        List<AreaGeoID> areaGeoId;
        internal Customers(WebRequestManager webRequestManager, List<AreaGeoID> areaGeoIds, List<Customer> customers, string currentStyle)
        {
            InitializeComponent();
            areaGeoId = areaGeoIds;
            this.webRequestManager = webRequestManager;
            this.customers = customers;
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    this.currentStyle = currentStyle;
                    ThemeHelper.SetTheme(this, currentStyle);
                    gridControl.ItemsSource = customers;
                    gridControl.GroupBy("Editable");
                }
            };
        }
        #endregion
        #region Methods
        void InitToken()
        {
            if (sc == null)
                sc = TaskScheduler.FromCurrentSynchronizationContext();
            if (tokenSource == null)
            {
                tokenSource = new CancellationTokenSource();
                ct = tokenSource.Token;
            }
        }
        string AppLogPath = MainWindow.AppLogPath;

        void DoAction(Action action1, Action action2)
        {
            InitToken();

            try
            {
                SetBusy(true);
                var task1 = Task.Factory.StartNew(delegate
                {
                    if (ct.IsCancellationRequested)
                        return;
                    else
                        action1();
                }, tokenSource.Token);
                pendingTask.Add(task1);
                var task2 = task1.ContinueWith(ret =>
                {
                    if (pendingTask.Contains(task1))
                        pendingTask.Remove(task1);

                    if (ret.IsFaulted)
                    {
                        Debug.WriteLine("I have observed a {0}",
                        ret.Exception.GetType().Name);
                        File.AppendAllText(AppLogPath, string.Format("{0}{1}", ret.Exception.ToString(), Environment.NewLine));
                    }

                    if (pendingTask.Count == 0)
                        SetBusy(false);

                    if (ct.IsCancellationRequested)
                        return;
                    else
                        action2();
                }, sc);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
            }
        }

        public void ShowDetailsList()
        {
            ShowDialog();
        }

        private void NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = new Customer();
            InsertOrUpdateCustomer(customer);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void tableView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Customer customer = gridControl.SelectedItem as Customer;
            if (customer == null)
                return;
            if (!customer.Editable)
            {
                MessageBox.Show(Properties.Resources.CustomerNotEditable);
                return;
            }

            InsertOrUpdateCustomer(customer);
        }
        void SetBusy(bool bBusy)
        {
            Dispatcher.InvokeIfRequired(() =>
            {
                if (bBusy)
                {
                    busyContent.Text = Properties.Resources.WaitText;
                    busyControl.Visibility = Visibility.Visible;
                    busyContent.Visibility = Visibility.Visible;
                }
                else
                {
                    busyControl.Visibility = Visibility.Collapsed;
                    busyContent.Visibility = Visibility.Collapsed;
                }
            });
        }
        #endregion


        private void InsertOrUpdateCustomer(Customer customer)
        {
            NewCustomer newCustomer = new NewCustomer(customer, areaGeoId) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            ThemeHelper.SetTheme(newCustomer, currentStyle);
            newCustomer.ShowDetailsList();

                if (!(bool)newCustomer.DialogResult)
                return;

            SetBusy(true);
            bool ret = false;
            Action action1 = () =>
            {
                ret = webRequestManager.InsertOrUpdateCustomer(customer);
                if (!ret)
                {
                    customers.Clear();
                    customers = webRequestManager.GetCustomerList();
                }
            };
            Action action2 = () =>
            {
                gridControl.ItemsSource = null;
                gridControl.ItemsSource = customers;
            };

            DoAction(action1, action2);
        }
        public void Dispose()
        {
            if (tokenSource != null)
                tokenSource.Cancel();

            if (pendingTask.Count > 0)
            {
                pendingTask.ForEach(l =>
                {
                    Task.WaitAll(l);
                });
            }

            if (tokenSource != null)
                tokenSource.Dispose();
        }
    }
}
