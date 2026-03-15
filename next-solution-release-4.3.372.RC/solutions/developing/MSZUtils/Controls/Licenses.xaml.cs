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
using System.Windows.Markup;
using System.Xml;
using System.Runtime.Serialization;
using DevExpress.Data.Filtering;
using Ookii.Dialogs.Wpf;
using DevExpress.Export;
using ViewModelLib;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using WPFPenHelpers;

namespace MSZUtils.Controls
{
    /// <summary>
    /// Interaction logic for HistoricalEvents.xaml
    /// </summary>
    public partial class Licenses : DXWindow, IDisposable
    {
        #region Declarations
        bool bInit;
        bool bLoaded;
        internal DataSet gridDataSet = new DataSet();
        List<LicenceInfo> licenceList = new List<LicenceInfo>();
        WebRequestManager webRequestManager;
        internal List<BoolOption> retBOptions = new List<BoolOption>();
        internal List<IntOption> retIOptions = new List<IntOption>();
        List<BoolOption> retDefBOptions = new List<BoolOption>();
        List<IntOption> retDefIOptions = new List<IntOption>();
        List<LogLicenceInfo> retLogInfos = new List<LogLicenceInfo>();
        CustomFilters FiltersList = new CustomFilters();

        public event EventHandler<ErrorEventArgs> Error;
        protected void OnError(string error)
        {
            Error?.Invoke(this, new ErrorEventArgs() { ErrorMessage = error });
        }
        const string title = "MSZUtilsLicenses";
        internal LicenceInfo selectedLicence;

        string actualConfig = "All";
        internal string ActualConfig
        {
            // IMPORTANT: To maintain parity between setting a property in XAML and procedural code, do not touch the getter and setter inside this dependency property!
            get
            {
                return actualConfig;
            }
            set
            {
                if (actualConfig.Equals(value))
                    return;
                filtersCombo.Text = actualConfig = value;
            }
        }
        #endregion

        #region Contructor
        internal Licenses(WebRequestManager webRequestManager, List<BoolOption> retDefBOptions, List<IntOption> retDefIOptions, string currentStyle)
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    //cmbExportType.ItemsSource = Enum.GetValues(typeof(ExportFileType)).Cast<ExportFileType>();
                    //cmbExportType.SelectedIndex = 0;
                    ThemeHelper.SetTheme(this, currentStyle);
                    //this.licenceList.AddRange(list);
                    this.webRequestManager = webRequestManager;
                    this.retDefBOptions.AddRange(retDefBOptions);
                    this.retDefIOptions.AddRange(retDefIOptions);
                    this.retBOptions.AddRange(this.retDefBOptions);
                    this.retIOptions.AddRange(this.retDefIOptions);
                    ReloadAndInitValues();
                    //gridControl.ItemsSource = list;
                }
            };
        }
        #endregion

        #region Methods


        public void Dispose()
        {
            SaveDesignGridLayout();

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

        private void expandAll_Click(object sender, RoutedEventArgs e)
        {
            gridControl.ExpandAllGroups();
        }

        private void collapseAll_Click(object sender, RoutedEventArgs e)
        {
            gridControl.CollapseAllGroups();
        }

        private void bestFitColumns_Click(object sender, RoutedEventArgs e)
        {
            tableView.BestFitColumns();
        }

        private void clearFilters_Click(object sender, RoutedEventArgs e)
        {
            gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
        }

        private void clearGroups_Click(object sender, RoutedEventArgs e)
        {
            gridControl.ClearGrouping();
        }

        private void clearSort_Click(object sender, RoutedEventArgs e)
        {
            gridControl.ClearSorting();
        }

        private void expandAllLog_Click(object sender, RoutedEventArgs e)
        {
            historicals.ExpandAllGroups();
        }

        private void collapseAllLog_Click(object sender, RoutedEventArgs e)
        {
            historicals.CollapseAllGroups();
        }

        private void bestFitColumnsLog_Click(object sender, RoutedEventArgs e)
        {
            tableLogView.BestFitColumns();
        }

        private void clearFiltersLog_Click(object sender, RoutedEventArgs e)
        {
            historicals.FilterCriteria = CriteriaOperator.Parse(string.Empty);
        }

        private void clearGroupsLog_Click(object sender, RoutedEventArgs e)
        {
            historicals.ClearGrouping();
        }

        private void clearSortLog_Click(object sender, RoutedEventArgs e)
        {
            historicals.ClearSorting();
        }
        internal void PreviewReport()
        {
            // Create a report and print it.
            XtraReport report = CreateReport();
            report.ShowPreviewDialog();
        }
        private XtraReport CreateReport()
        {
            XtraReport report = new XtraReport();

            CreateReportHeader(report);
            CreateReportFooter(report);

            return report;
        }
        private void CreateReportFooter(XtraReport report)
        {
            PageHeaderBand headerBand = new PageHeaderBand();
            XRTable headerTable = new XRTable();
            //headerTable.Size = new System.Drawing.Size(300, 100);

            // Start table initialization.
            headerTable.BeginInit();

            // Enable table borders to see its boundaries.
            //headerTable.BorderWidth = 2;
            headerTable.Borders = DevExpress.XtraPrinting.BorderSide.All;

            // Create a table row.
            XRTableRow hrow = new XRTableRow();

            // Create two table cells.
            XRTableCell hcell0 = new XRTableCell();
            XRTableCell hcell1 = new XRTableCell();

            // Construct the table.
            hrow.Cells.Add(hcell0);
            hrow.Cells.Add(hcell1);
            headerTable.Rows.Add(hrow);

            // Finish table initialization.
            headerTable.EndInit();

            report.Bands.Add(headerBand);
            report.Bands[BandKind.PageHeader].Controls.Add(headerTable);

            DetailBand detail = new DetailBand();
            XRTable table = new XRTable();
            //table.Size = new System.Drawing.Size(300, 100);

            // Start table initialization.
            table.BeginInit();

            // Enable table borders to see its boundaries.
            //table.BorderWidth = 2;
            table.Borders = DevExpress.XtraPrinting.BorderSide.All;

            // Create a table row.
            XRTableRow row = new XRTableRow();

            // Create two table cells.
            XRTableCell cell0 = new XRTableCell();
            XRTableCell cell1 = new XRTableCell();

            // Construct the table.
            row.Cells.Add(cell0);
            row.Cells.Add(cell1);
            table.Rows.Add(row);

            // Finish table initialization.
            table.EndInit();
            // Create a Detail band and add the bar code to it.
            report.Bands.Add(detail);
            report.Bands[BandKind.Detail].Controls.Add(table);
        }


        private void CreateReportHeader(XtraReportBase report)
        {
            //Creating a Report header
            ReportHeaderBand header = new ReportHeaderBand();
            report.Bands.Add(header);
            header.HeightF = 0;

            XRLabel label = new XRLabel();
            header.Controls.Add(label);

            label.BackColor = System.Drawing.Color.White;
            label.ForeColor = System.Drawing.Color.DarkBlue;
            label.Font = new System.Drawing.Font("Segoe UI", 24, System.Drawing.FontStyle.Bold);
            label.Text = "Licenses report";
            label.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            label.LocationF = new System.Drawing.PointF(10, 0);

            XtraReport rep = report.RootReport;
            label.WidthF = rep.PageWidth - rep.Margins.Right - rep.Margins.Left;

            XRTable table = new XRTable();
            header.Controls.Add(table);
        }
        void ExportData(bool details = false)
        {
            List<TemplatedLink> links = new List<TemplatedLink>();
            if(details)
            {
                PreviewReport();
            }
            else
            {
                links.Add(new PrintableControlLink(tableView));
                links.Add(new PrintableControlLink(tableLogView));
                CompositeLink compositeLink = new CompositeLink(links);
                compositeLink.PageBreaks.Add(new PageBreakInfo(links[0]) { PageBreakAfter = true });
                compositeLink.Landscape = true;
                compositeLink.PaperKind = System.Drawing.Printing.PaperKind.A4;
                PrintHelper.ShowRibbonPrintPreview(this, compositeLink);
            }
        }
        private void ReloadAndInitValues()
        {
            List<LicenceInfo> list = new List<LicenceInfo>();
            SetBusy(true);
            var task1 = Task.Factory.StartNew(delegate
            {
                list = webRequestManager.GetSerialListInfo();
                return list;
            });
            pendingTask.Add(task1);
            var task2 = task1.ContinueWith(ret =>
            {
                if (pendingTask.Contains(task1))
                    pendingTask.Remove(task1);
                try
                {
                    if (ret.IsFaulted)
                    {
                        Debug.WriteLine("I have observed a {0}",
                        ret.Exception.GetType().Name);
                        File.AppendAllText(AppLogPath, string.Format("{0}{1}", ret.Exception.ToString(), Environment.NewLine));
                    }
                    else
                    {
                        licenceList = ret.Result;
                        gridControl.ItemsSource = list;
                    }
                    if (licenceList.Count > 0)
                        gridControl.SelectedItem = licenceList[0];
                    if (!bInit && !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) || (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.V))
                        LoadDesignGridLayout();
                    else
                    {
                        RestoreDefaultFilters();
                        gridControl.GroupBy("CustomerDescription");
                    }

                    tableView.BestFitColumns();

                    bInit = true;
                    ManageSelection();
                }
                catch (Exception)
                {
                }
                finally
                {
                    bInit = true;
                    SetBusy(false);
                }
            }, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        string AppLogPath = MainWindow.AppLogPath;
        
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
        public void ShowDetailsList()
        {
            Show();
        }
        internal virtual void SaveDesignGridLayout()
        {
            var storage = SerializationHelper.GetStorage();
            try
            {
                var fileName = SerializationHelper.GetStoreFileNameDocking(title, "Results");
                if (storage.FileExists(fileName))
                    storage.DeleteFile(fileName);
                using (var fileStream = storage.OpenFile(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    gridControl.SaveLayoutToStream(fileStream);
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                var fileName = SerializationHelper.GetStoreFileNameDocking(title, "History");
                if (storage.FileExists(fileName))
                    storage.DeleteFile(fileName);
                using (var fileStream = storage.OpenFile(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    historicals.SaveLayoutToStream(fileStream);
                }
            }
            catch (Exception ex)
            {
            }

            SaveFilters();
        }

        private void SaveFilters()
        {
            var storage = SerializationHelper.GetStorage();
            try
            {
                var fileName = SerializationHelper.GetStoreFileNameDocking(title, "ActualConfig");
                if (storage.FileExists(fileName))
                    storage.DeleteFile(fileName);
                using (var fileStream = storage.OpenFile(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    SerializationHelper.WriteProjectDataStream<string>(fileStream, ActualConfig);
                }
            }
            catch (Exception ex)
            {
            }

            try
            {
                var fileName = SerializationHelper.GetStoreFileNameDocking(title, "Filters");
                if (storage.FileExists(fileName))
                    storage.DeleteFile(fileName);
                using (var fileStream = storage.OpenFile(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite))
                {
                    SerializationHelper.WriteProjectDataStream<CustomFilters>(fileStream, FiltersList);
                }
            }
            catch (Exception)
            {
            }
        }

        internal virtual void LoadDesignGridLayout()
        {
            try
            {
                using (var fileStream = SerializationHelper.GetStorage().OpenFile(SerializationHelper.GetStoreFileNameDocking(title,"Results"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    gridControl.RestoreLayoutFromStream(fileStream);
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                using (var fileStream = SerializationHelper.GetStorage().OpenFile(SerializationHelper.GetStoreFileNameDocking(title, "History"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    historicals.RestoreLayoutFromStream(fileStream);
                }
            }
            catch (Exception ex)
            {
            }

            LoadFilters();

        }
        bool isLoadingFilters;
        private void LoadFilters()
        {
            isLoadingFilters = true;
            try
            {
                using (var fileStream = SerializationHelper.GetStorage().OpenFile(SerializationHelper.GetStoreFileNameDocking(title, "ActualConfig"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    ActualConfig = SerializationHelper.ReadProjectDataFromStream<string>(fileStream);
                }
            }
            catch (Exception ex)
            {
            }
            try
            {
                using (var fileStream = SerializationHelper.GetStorage().OpenFile(SerializationHelper.GetStoreFileNameDocking(title, "Filters"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    FiltersList = SerializationHelper.ReadProjectDataFromStream<CustomFilters>(fileStream);
                }
            }
            catch (Exception)
            {
                RestoreDefaultFilters();
            }

            filtersCombo.ItemsSource = FiltersList.OrderBy(x => x.FilterString);

            if (FiltersList.Count > 0)
            {
                try
                {
                    var setting = (from m in FiltersList where m.Name.Equals(ActualConfig) select m).FirstOrDefault();
                    if (setting != null && !string.IsNullOrEmpty(setting.FilterString))
                    {
                        gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
                        gridControl.FilterCriteria = CriteriaOperator.Parse(setting.FilterString);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    isLoadingFilters = false;
                }
            }


            if (ActualConfig != filtersCombo.Text && FiltersList.Count > 0)
                ChangeSetting(FiltersList[0]);
        }
        Dictionary<String, String> defFilters = new Dictionary<string, string>()
        {
            {"All"," "},
            {"Germany","[AreaGeoDescr] = 'Germany'"},
            {"International","[AreaGeoDescr] = 'International'"},
            {"Italy","[AreaGeoDescr] = 'Italy'"},
            {"USA","[AreaGeoDescr] = 'USA'"},
            {"USA and International","[AreaGeoDescr] = 'USA&International'"},
            {"Temporary","[SerialNumber] < 0"},
            {"Temporary Germany","[SerialNumber] < 0 And [AreaGeoDescr] = 'Germany'"},
            {"Temporary International","[SerialNumber] < 0 And [AreaGeoDescr] = 'International'"},
            {"Temporary Italy","[SerialNumber] < 0 And [AreaGeoDescr] = 'Italy'"},
            {"Temporary USA","[SerialNumber] < 0 And [AreaGeoDescr] = 'USA'"},
            {"Temporary USA and International","[SerialNumber] < 0 And [AreaGeoDescr] = 'USA&International'"},
            {"Unlimited","[SerialNumber] > 0"},
            {"Unlimited Germany","[SerialNumber] > 0 And [AreaGeoDescr] = 'Germany'"},
            {"Unlimited International","[SerialNumber] > 0 And [AreaGeoDescr] = 'International'"},
            {"Unlimited Italy","[SerialNumber] > 0 And [AreaGeoDescr] = 'Italy'"},
            {"Unlimited USA","[SerialNumber] > 0 And [AreaGeoDescr] = 'USA'"},
            {"Unlimited USA and International","[SerialNumber] > 0 And [AreaGeoDescr] = 'USA&International'"},
        };
        private void RestoreDefaultFilters()
        {
            defFilters.Keys.ToList().ForEach(x =>
            {
                Filter setting = (from m in FiltersList where m.Name.Equals(x) select m).FirstOrDefault();
                if (setting != null)
                    setting.FilterString = defFilters[x];
                else
                    FiltersList.Add(new Filter() { Name = x, FilterString = defFilters[x] });
            });

            try
            {
                isLoadingFilters = true;
                filtersCombo.ItemsSource = FiltersList.OrderBy(x => x.FilterString);
            }
            catch (Exception)
            {
            }
            finally
            {
                isLoadingFilters = false;
                filtersCombo.SelectedIndex = 0;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void gridControl_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            ManageSelection();
        }

        private void ManageSelection()
        {
            if (bInit && (gridControl.SelectedItem as LicenceInfo) != null)
            {
                //OnSelectionChanged((gridControl.SelectedItem as LicenceInfo).SerialNumber);
                var serial = (gridControl.SelectedItem as LicenceInfo).SerialNumber;
                ReadDBLicense(serial);
            }
        }

        private void ReadDBLicense(int serial)
        {
            bool ret = false;
            Action action1 = () =>
            {
                LicenceInfo _licenseInfo = null;
                string serialNumber = serial.ToString();
                retLogInfos.Clear();

                if (!ct.IsCancellationRequested)
                    _licenseInfo = webRequestManager.GetSerialInfo(serialNumber);

                if (!ct.IsCancellationRequested && _licenseInfo != null)
                {
                    _licenseInfo.SerialNumber = serial;
                    ret = webRequestManager.GetSerialOptions(_licenseInfo.ID.ToString(), retBOptions, retIOptions);
                    webRequestManager.GetSerialHistoryLogOptions(serialNumber, retLogInfos);
                }

                selectedLicence = _licenseInfo;
            };
            Action action2 = () =>
            {
                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                historicals.ItemsSource = null;

                if (!ret)
                {
                    retBOptions.Clear();
                    retIOptions.Clear();
                    retBOptions.AddRange(retDefBOptions);
                    retIOptions.AddRange(retDefIOptions);

                    //ClearValues();
                    MessageBox.Show(Properties.Resources.NoOptionsFoundInDB, Properties.Resources.ErrorCaption);
                }
                boolOptionList.ItemsSource = retBOptions;
                intOptionList.ItemsSource = retIOptions;
                historicals.ItemsSource = retLogInfos;
                tableLogView.BestFitColumns();
            };

            DoAction(action1, action2);
        }

        public event EventHandler<SelectionEventArgs> SelectionChanged;
        protected void OnSelectionChanged(int index)
        {
            SelectionChanged?.Invoke(this, new SelectionEventArgs() { ID = index });
        }
        CancellationTokenSource tokenSource;
        CancellationToken ct;
        TaskScheduler sc = null;
        List<Task> pendingTask = new List<Task>();
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

                    if (ct.IsCancellationRequested)
                    {
                        SetBusy(false);
                        return;
                    }
                    else
                        try
                        {
                            action2();
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            SetBusy(false);
                        }
                }, sc);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
            }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            if ((gridControl.SelectedItem as LicenceInfo) != null)
            {
                OnSelectionChanged((gridControl.SelectedItem as LicenceInfo).SerialNumber);
                Close();
            }
            else
                MessageBox.Show(Properties.Resources.SelectLicWarning, Properties.Resources.ErrorCaption);
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            if ((gridControl.SelectedItem as LicenceInfo) != null)
                OnSelectionChanged((gridControl.SelectedItem as LicenceInfo).SerialNumber);

            else
                MessageBox.Show(Properties.Resources.SelectLicWarning, Properties.Resources.ErrorCaption);
        }

        private void ReloadAndInitValues(object sender, RoutedEventArgs e)
        {
            ReloadAndInitValues();
        }
        #endregion

        private void Button9_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exportData_Click(object sender, RoutedEventArgs e)
        {
            ExportData();
        }

        private void historicals_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName != "NewValue") return;
            var newValue = (e.Value).ToString().Substring(1).Replace("; ",Environment.NewLine);
            e.DisplayText = newValue;
        }

        private void exportDetails_Click(object sender, RoutedEventArgs e)
        {
            ExportData(true);
        }

        private void saveFilter_Click(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(filtersCombo.Text))
            {
                ActualConfig = filtersCombo.Text;
                var setting = (from m in FiltersList where m.Name.Equals(filtersCombo.Text) select m).FirstOrDefault();
                if (setting == null)
                {
                    FiltersList.Add(new Filter() { Name = filtersCombo.Text, FilterString = gridControl.FilterString });
                }
                else
                {
                    setting.FilterString = gridControl.FilterString;
                }
                SaveFilters();
            }
        }

        private void filtersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isLoadingFilters)
                return;
            ChangeSetting((sender as ComboBox).SelectedItem as Filter);
            gridControl.CollapseAllGroups();
        }

        void ChangeSetting(Filter setting)
        {
            using (var cursor = new WaitCursor())
            {
                try
                {
                    if (setting != null && !string.IsNullOrEmpty(setting.FilterString) && !string.IsNullOrEmpty(setting.Name))
                    {
                        ActualConfig = setting.Name;
                        gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
                        gridControl.FilterCriteria = CriteriaOperator.Parse(setting.FilterString);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void deleteFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filtersCombo.Text))
            {
                isLoadingFilters = true;
                filtersCombo.ItemsSource = null;
                var setting = (from m in FiltersList where m.Name.Equals(filtersCombo.Text) select m).FirstOrDefault();
                if (setting != null)
                {
                    FiltersList.Remove(setting);
                }
                if (FiltersList.Count > 0)
                {
                    filtersCombo.ItemsSource = FiltersList.OrderBy(x => x.FilterString);
                    ActualConfig = FiltersList[0].Name;
                    filtersCombo.Text = ActualConfig;
                }
                else
                {
                    ActualConfig = string.Empty;
                }

                SaveFilters();
                isLoadingFilters = false;

                gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
                gridControl.FilterCriteria = CriteriaOperator.Parse(ActualConfig);
            }
        }

        private void loadFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(filtersCombo.Text))
            {
                isLoadingFilters = true;
                var setting = (from m in FiltersList where m.Name.Equals(filtersCombo.Text) select m).FirstOrDefault();
                if (setting != null && !string.IsNullOrEmpty(setting.FilterString))
                {
                    gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
                    gridControl.FilterCriteria = CriteriaOperator.Parse(setting.FilterString);
                    ActualConfig = setting.Name;
                }

                SaveFilters();
                isLoadingFilters = false;
            }
        }

        DateSpan lastFilter = DateSpan.All;
        TimeRangeDates dates;
        private void dateFilter_Click(object sender, RoutedEventArgs e)
        {
            lastFilter = (DateSpan)Enum.Parse(typeof(DateSpan), (sender as Button).Tag.ToString());
            DateTime date1;
            DateTime date2;
            DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, lastFilter);
            String filter = gridControl.FilterString;

            dates.start = date1;
            dates.end = date2;

            SetDates(date1, date2);
        }

        private void SetDates(DateTime date1, DateTime date2)
        {
            startDate.Text = date1.ToString();
            endDate.Text = date2.ToString();
            try
            {
                String filter = gridControl.FilterString;
                String columnFilter = gridControl.GetColumnFilterString("LastChangesDate");
                String newColumnFilter = $"[LastChangesDate] Between(#{date1.Year.ToString("0000")}-{date1.Month.ToString("00")}-{date1.Day.ToString("00")}#,#{date2.Year.ToString("0000")}-{date2.Month.ToString("00")}-{date2.Day.ToString("00")}#)";
                if (lastFilter == DateSpan.All)
                    newColumnFilter = string.Empty;

                if (!string.IsNullOrEmpty(columnFilter))
                    filter = filter.Replace(columnFilter, newColumnFilter);
                else
                {
                    if (string.IsNullOrEmpty(filter))
                        filter = newColumnFilter;
                    else
                        filter = $"{filter} And {newColumnFilter}";
                }
                try
                {
                    gridControl.FilterCriteria = CriteriaOperator.Parse(string.Empty);
                    gridControl.FilterCriteria = CriteriaOperator.Parse(filter);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }
        }

        private void addDateFilter_Click(object sender, RoutedEventArgs e)
        {
            if (lastFilter == DateSpan.All)
                return;

            if (dates.start == null)
                dates.start = DateTime.Now.AddDays(-7);
            if (dates.end == null)
                dates.end = DateTime.Now;

            TimeRangeHelper.GetSelectedTimeRangeCombo(dates.start, dates.end, lastFilter, true, (sender as Button).Tag as string == "0", out dates);

            DateTime date1 = dates.start;
            DateTime date2 = dates.end;
            startDate.Text = date1.ToString();
            endDate.Text = date2.ToString();

            SetDates(date1, date2);
        }

        private void restoreDefaultFilters_Click(object sender, RoutedEventArgs e)
        {
            RestoreDefaultFilters();
        }
    }


    public class Converter : MarkupExtension, IMultiValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
                return 0;
            var val1 = (int)values[0];
            var val2 = (int)values[1];
            return ((double)val1 / (double)val2) * 100;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class ExpiringDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                if (dateTime.CompareTo(DateTime.MinValue) == 0)
                    return Properties.Resources.UnlimitedOption;
                else
                    return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GroupSummaryItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var data = item as GridGroupSummaryData;
            if (data != null && data.SummaryItem != null && data.SummaryItem.FieldName == "spread")
            {
                return (container as FrameworkElement).FindResource("SummaryTemplate") as DataTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}
