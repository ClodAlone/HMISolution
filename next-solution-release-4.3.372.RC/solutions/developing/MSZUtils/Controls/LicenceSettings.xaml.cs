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
using System.Threading;

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for HistoricalEvents.xaml
    /// </summary>
    public partial class LicenceSettings : DXWindow
    {
        #region Contructor
        bool bInit;
        bool bLoaded;
        internal DataSet gridDataSet = new DataSet();
        List<LicenceType> licTypeList = new List<LicenceType>();
        WebRequestManager webRequestManager;
        internal List<BoolOption> retBOptions = new List<BoolOption>();
        internal List<IntOption> retIOptions = new List<IntOption>();
        Dictionary<int, List<BoolOption>> boolOptionMap = new Dictionary<int, List<BoolOption>>();
        Dictionary<int, List<IntOption>> intOptionMap = new Dictionary<int, List<IntOption>>();
        List<BoolOption> retDefBOptions = new List<BoolOption>();
        List<IntOption> retDefIOptions = new List<IntOption>();
        internal LicenceInfo selectedLicence;
        bool bDirty;
        public bool Dirty { get { return bDirty; } }
        string AppLogPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSZUtils.log");
        internal LicenceSettings(WebRequestManager webRequestManager, List<BoolOption> retDefBOptions, List<IntOption> retDefIOptions, string currentStyle)
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;

                    ThemeHelper.SetTheme(this, currentStyle);
                    this.webRequestManager = webRequestManager;
                    this.retDefBOptions.AddRange(retDefBOptions);
                    this.retDefIOptions.AddRange(retDefIOptions);
                    this.retBOptions.AddRange(this.retDefBOptions);
                    this.retIOptions.AddRange(this.retDefIOptions);
                    ReloadAndInitValues();
                }
            };
        }
        #endregion
        #region Methods
        private void ReloadAndInitValues()
        {
            List<LicenceType> list = new List<LicenceType>();
            SetBusy(true);
            var task1 = Task.Factory.StartNew(delegate
            {
                boolOptionMap.Clear();
                intOptionMap.Clear();
                retBOptions.Clear();
                retIOptions.Clear();
                licTypeList.Clear();
                retBOptions.AddRange(retDefBOptions);
                retIOptions.AddRange(retDefIOptions);
                list = webRequestManager.GetLicTypeList(true);
                return list;
            });
            var task2 = task1.ContinueWith(ret =>
            {
                if (ret.IsFaulted)
                {
                    Debug.WriteLine("I have observed a {0}",
                    ret.Exception.GetType().Name);
                    File.AppendAllText(AppLogPath, string.Format("{0}{1}", ret.Exception.ToString(), Environment.NewLine));
                    licencetype.ItemsSource = null;
                }
                else
                {
                    licTypeList.AddRange(ret.Result);
                    licencetype.ItemsSource = licTypeList;
                }
                if (licTypeList.Count > 0)
                    licencetype.SelectedItem = licTypeList[0];
                bInit = true;
                ManageSelection();
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void ManageSelection()
        {
            if (bInit && (licencetype.SelectedItem as LicenceType) != null)
            {
                var licType = (licencetype.SelectedItem as LicenceType);
                ReadDBOptions(licType.ID);
                boolOptionList.IsHitTestVisible = !licType.Hidden;
                intOptionList.IsHitTestVisible = !licType.Hidden;
            }
            else
                SetBusy(false);
        }
        List<Task> pendingTask = new List<Task>();
        CancellationTokenSource tokenSource;
        CancellationToken ct;
        TaskScheduler sc = null;
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
        private void ReadDBOptions(int licTypeID)
        {
            bool ret = false;
            List<BoolOption> _BoolOptions = new List<BoolOption>();
            List<IntOption> _IntOptions = new List<IntOption>();
            Action action1 = () =>
            {
                if (!ct.IsCancellationRequested)
                {
                    if (boolOptionMap.ContainsKey(licTypeID))
                        _BoolOptions = boolOptionMap[licTypeID];
                    else
                    {
                        var boolOptions = webRequestManager.GetBoolOptions(licTypeID);
                        retDefBOptions.ForEach(opt =>
                        {
                            var bOption = boolOptions.Find(x => x.ID == opt.ID);
                            if (bOption != null)
                                opt.Enabled = bOption.Enabled;
                            else
                                bOption = opt.Clone();

                            _BoolOptions.Add(bOption);
                        });
                        boolOptionMap.Add(licTypeID, _BoolOptions);
                    }

                    if (intOptionMap.ContainsKey(licTypeID))
                        _IntOptions = intOptionMap[licTypeID];
                    else
                    {
                        var intOptions = webRequestManager.GetNumericOptions(licTypeID);
                        retDefIOptions.ForEach(opt =>
                        {
                            var bOption = intOptions.Find(x => x.ID == opt.ID);
                            if (bOption != null)
                                opt.Enabled = bOption.Enabled;
                            else
                                bOption = opt.Clone();

                            _IntOptions.Add(bOption);
                        });
                        intOptionMap.Add(licTypeID, _IntOptions);
                    }

                    ret = true;
                }
            };
            Action action2 = () =>
            {
                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                retBOptions.Clear();
                retIOptions.Clear();
                if (!ret)
                {
                    retBOptions.AddRange(retDefBOptions);
                    retIOptions.AddRange(retDefIOptions);
                    //ClearValues();
                    MessageBox.Show(Properties.Resources.NoOptionsFoundInDB, Properties.Resources.ErrorCaption);
                }
                else
                {
                    retBOptions.AddRange(_BoolOptions);
                    retIOptions.AddRange(_IntOptions);
                }
                boolOptionList.ItemsSource = retBOptions;
                intOptionList.ItemsSource = retIOptions;
            };

            DoAction(action1, action2);
        }

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
        public void ShowDetailsList()
        {
            ShowDialog();

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
        private void Exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReloadAndInitValues(object sender, RoutedEventArgs e)
        {
            ReloadAndInitValues();
            bInit = true;
            ManageSelection();
        }

        private void SaveOptionsToDB(object sender, RoutedEventArgs e)
        {
            SaveOptionsToDB();
        }

        private void SaveOptionsToDBAndExit(object sender, RoutedEventArgs e)
        {
            SaveOptionsToDB();
            Close();
        }

        private void SaveOptionsToDB()
        {
            StringBuilder errorInfo = new StringBuilder(Properties.Resources.ErrorSavingOptions);
            List<int> keyList = boolOptionMap.Keys.Intersect(intOptionMap.Keys).ToList();
            keyList.ForEach(k =>
            {
                LicenceType licType = (from l in licTypeList where l.ID == k select l).FirstOrDefault();
                if (licType != null)
                {
                    var retLicType = webRequestManager.UpdateLicTypeOptions(licType, boolOptionMap[k], intOptionMap[k]);
                    if (retLicType == null)
                        errorInfo.Append(licType.Name);
                    else
                    {
                        licType.ID = retLicType.ID;
                        licType.Description = retLicType.Description;
                        licType.Name = retLicType.Name;
                        licType.Hidden = retLicType.Hidden;
                    }
                }
            });

            bDirty = true;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string newLicName = (from l in licTypeList where l.Name.Equals(Properties.Resources.NewLicenceTypeName) select l.Name).FirstOrDefault();
            int i = 0;
            while (newLicName != null)
            {
                i++;
                newLicName = (from l in licTypeList where l.Name.Equals(Properties.Resources.NewLicenceTypeName + i) select l.Name).FirstOrDefault();
            }

            if (i > 0)
                newLicName = Properties.Resources.NewLicenceTypeName + i;
            else
                newLicName = Properties.Resources.NewLicenceTypeName;


            LicenceType licType = new LicenceType()
            {
                ID = (from l in licTypeList orderby l.ID descending select l).FirstOrDefault().ID + 1,
                Name = newLicName,
                Description = Properties.Resources.NewLicTypeDescription,
                Hidden = false
            };

            bInit = false;
            boolOptionList.ItemsSource = null;
            intOptionList.ItemsSource = null;
            licencetype.ItemsSource = null;
            licTypeList.Add(licType);
            retBOptions.Clear();
            retIOptions.Clear();
            retBOptions.AddRange(retDefBOptions);
            retIOptions.AddRange(retDefIOptions);

            List<BoolOption> _BoolOptions = new List<BoolOption>();
            List<IntOption> _IntOptions = new List<IntOption>();
            _BoolOptions.AddRange(retDefBOptions);
            _IntOptions.AddRange(retDefIOptions);

            if (!boolOptionMap.ContainsKey(licType.ID))
                boolOptionMap.Add(licType.ID, _BoolOptions);
            if (!intOptionMap.ContainsKey(licType.ID))
                intOptionMap.Add(licType.ID, _IntOptions);

            boolOptionList.ItemsSource = retBOptions;
            intOptionList.ItemsSource = retIOptions;
            licencetype.ItemsSource = licTypeList;
            licencetype.SelectedItem = licType;
            bInit = true;
        }

        private void licencetype_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            ManageSelection();
        }

        private void EditValue_GotFocus(object sender, RoutedEventArgs e)
        {
            LicenceType selected = (MSZUtilsServiceHelper.LicenceType)((System.Windows.FrameworkElement)sender)?.DataContext;
            if(selected != null)
                licencetype.SelectedItem = selected;
        }

        private void hiddenField_Checked(object sender, RoutedEventArgs e)
        {
            LicenceType selected = (MSZUtilsServiceHelper.LicenceType)((System.Windows.FrameworkElement)sender)?.DataContext;
            boolOptionList.IsHitTestVisible = !selected.Hidden;
            intOptionList.IsHitTestVisible = !selected.Hidden;
        }
    }
}
