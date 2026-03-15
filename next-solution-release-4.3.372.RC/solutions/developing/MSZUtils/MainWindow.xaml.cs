using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpo;
using Microsoft.Win32;
using MSZ;
using MSZFactory;
using Utilities;
using MSZUtilsServiceHelper;
using UIMsgBoxAlertService.ComponentService;
using WPFUtilities;
using System.Windows.Threading;
using MSZUtils.Controls;
using MSZUtils.Helpers;
using DevExpress.Xpf.Editors;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace MSZUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Declarations
        public static readonly RoutedCommand ShowLicences = new RoutedCommand();
        public static readonly RoutedCommand ShowCustomers = new RoutedCommand();
        public static readonly RoutedCommand ReadLicense = new RoutedCommand();

        bool bLoaded;
#if !DEBUG
        //DB
        string ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zp/0Q8zC5u6IVmzT7u2x4R8R7U7H19YF7ux3uviV7lBzYbKl9qnbx9aCeNzfqyxWpZEuNoLrFaIOY1BtJqCe5iWX+TqGr5ir0BukZOwxnRepkjlU2TQZoNKQg2ISFSJ1sWMJb6fWmSmDrqADHzjMFM4");
        //DB_Test
        //string ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zp/0Q8zC5u6IVmzT7u2x4R8R7U7H19YF7ux3uviV7lBzYbKl9qnbx9aCeNzfqyxWpZEuNoLrFaIOY1BtJqCe5iWS+5IXJBmJKHc4+o+0zGJT+66l7TY3200GUiqDc7fB75jRFUaRhKRrNq92KSCGjZ3");
#else
        //DB Debug
        string ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zp/0Q8zC5u6IVmzT7u2x4R8R7U7H19YF7ux3uviV7lBzYbKl9qnbx9aCeNzfqyxWpZEuNoLrFaIOY1BtJqCe5iWVaN0PfBlid4dxfqI2c1pm42hT1JsR5mBOEENVYAo3emt+82Zfebpb21m+3CNxwji");
#endif
        Dictionary<int, List<BoolOption>> MaptoLicTypeDefBoolValues = new Dictionary<int, List<BoolOption>>();
        Dictionary<int, List<IntOption>> MaptoLicTypeDefIntValues = new Dictionary<int, List<IntOption>>();
        Dictionary<string, string> MapToMarkUp = new Dictionary<string, string>();

        string multiInstance = WPFUtilities.CryptString.CryptString.DecryptString("hEE5MgekJrgW5iJ7+fKcIg==");
        string hWInstance = WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==");
        List <LicenceType> retLicenceType = new List<LicenceType>();
        List<Customer> retCustomers = new List<Customer>();
        List<AreaGeoID> retAreaGeoIds = new List<AreaGeoID>();
        List<BoolOption> retBOptions = new List<BoolOption>();
        List<IntOption> retIOptions = new List<IntOption>();

        List<BoolOption> retDefBOptions = new List<BoolOption>();
        List<IntOption> retDefIOptions = new List<IntOption>();

        String _defaultDataProvider = string.Empty;
        String _defaultConnectionString = string.Empty;
        uint MaxTransactionsBeforeCommit = 10;
        string _IDProdotto 
        {
            get { return string.Format("[IdProdotto] = {0}", IDProdotto); }
        }
        private static string FilePath
        {
            get
            {
                return String.Format("{0}\\{1}",
                          Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                          WPFUtilities.CryptString.CryptString.DecryptString("qeDRnBYSvVODSl4TSUUVjZi5mXWW1erIHSd4xxNxLTQ="));
            }
        }
        private static string LogPath
        {
            get
            {
                return System.IO.Path.Combine(GetAssemblyPath(), WPFUtilities.CryptString.CryptString.DecryptString("Uzw5RtLX40qWDZmLrPI6Ku25WBMOgAg+8pJS8UdlRlw="));
            }
        }
        internal static string AppLogPath
        {
            get
            {
                return System.IO.Path.Combine(GetAssemblyPath(), "MSZUtils.log");
            }
        }
        private static string PrinterPath
        {
            get
            {
                return System.IO.Path.Combine(GetAssemblyPath(), WPFUtilities.CryptString.CryptString.DecryptString("x4X18P4WcJZHvouVH3HgxQ=="));
            }
        }
        static string GetAssemblyPath()
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            return basedir;
        }
        string currentStyle;

        private static string NoDate = "rg51dnJNzvrnhxjPoi4zmw==";
        private static string pgrCode = "aqyFuVmb6LP+jg3oTfy4Dhz2ys8ZxGGjtEDqhdp/mf8=";
        private static string xCode = string.Empty;
        private const int REGISTER_MAX_BYTECOUNT = 188;
        private const int REGISTER_MAX_COUNT = 47;
        private string InputFile { get; set; }
        IUIMsgBoxAlertService uIInterface;
        List<Task> pendingTask = new List<Task>();
        CommandLineOptions args = null;
        #endregion

        #region  Properties
        private string PrevMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("BTEKkPCMPZn/v6d2OM7ekQ=="); } }//SiteCode
        private string DateMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("HOa44ZEOiZwUy5LheAVQUw=="); } }//ExpiringDate
        private string ActivationMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("t+V7RhAwpnVWTiyYcYIy+w=="); } }//ActivationDate
        private string ExpiringDelta { get { return WPFUtilities.CryptString.CryptString.DecryptString("prv/e1GpOfRUtjX+HrMM2A=="); } }//30
        private string AppMarkup { get { return WPFUtilities.CryptString.CryptString.DecryptString(""); } }//AppType

        private string ExpD { get { return WPFUtilities.CryptString.CryptString.DecryptString("xBRvCvaiIyolXzyKfTBYdA=="); } }
        private string UnTd { get { return WPFUtilities.CryptString.CryptString.DecryptString("87/5JHLZro50L6530fKBIg=="); } }
        private string NSrl { get { return WPFUtilities.CryptString.CryptString.DecryptString("WNDataAVWdeTCdxwIqfSYw=="); } }
        private string _serial { get { return WPFUtilities.CryptString.CryptString.DecryptString("/2WysmBwr00nKx3ylLf4Pg=="); } }//("SN");

        public string FileFormatVersion { get; private set; }

        int IDProdotto { get; set; }
        int RangeStartNr { get; set; }
        WebRequestManager webRequestManager;
        #endregion

        #region ctor
        public MainWindow()
        {
            InitializeComponent();
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            mainBuild.Text = $"{fileVersion.ProductMajorPart}.{fileVersion.ProductMinorPart}.{fileVersion.ProductBuildPart}";
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    bLoaded = true;
                    InitTheme();

                    SetInputShortcut(btnOpenLicenses,Properties.Resources.OpenLicenseTable);
                    SetInputShortcut(btnCustomerTable, Properties.Resources.OpenCustomerTable);
                    SetInputShortcut(btnReadLicenses, Properties.Resources.ReadLicenseFromDB);

                    uIInterface = new UIMsgBoxAlertService.ComponentService.UIMsgBoxAlertServiceComponent();
                    currentStyle = Properties.Settings.Default.ThemeName;
                    SetTheme(currentStyle);
                    if (Properties.Settings.Default.ThemeName == "Blend")
                        Background = new SolidColorBrush(Color.FromArgb(255, 45, 45, 45));
                    removingnumberImage.Stretch = Stretch.Fill;
                    getfirstserialImage.Stretch = Stretch.Fill;
                    reloadImage.Stretch = Stretch.Fill;
                    addnewImage.Stretch = Stretch.Fill;

                    //InitKeyValues();
                    dateTimePicker1.DateTime = DateTime.Now;
                    ClearKeyError();
                    webRequestManager = new WebRequestManager();
                    webRequestManager.InactivityElapsed += (obj, ea) =>
                    {
                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            inactivityClosure.Visibility = Visibility.Visible;
                            ShowError(Properties.Resources.ExpiredLogin, false);
                            Application.Current.Shutdown(-20);
                        });
                    };
                    webRequestManager.Error += (obj, ea) =>
                    {
                        Dispatcher.BeginInvokeIfRequired(() =>
                        {
                            ShowError(ea.ErrorMessage, false);
                            //Application.Current.Shutdown(-20);
                        });
                    };

                    Action action = () =>
                    {
                        SetBusy(false);

                        if (tokenSource != null)
                            tokenSource.Cancel();

                        uIInterface = null;
                        if(webRequestManager != null)
                        {
                            webRequestManager.LogOffUser();
                            webRequestManager.CloseNetworkServerTcp(true);
                        }

                        if (pendingTask.Count > 0)
                            Task.WaitAll(pendingTask.ToArray());

                        if (tokenSource != null)
                            tokenSource.Dispose();
                    };

                    AppDomain.CurrentDomain.ProcessExit += (obj, ea) =>
                    {
                        action();
                    };

                    UserLogOn();
                }
            };
        }

        private void SetInputShortcut(TextBlock btn, string label)
        {
            try
            {
                string _label = label;
                string firstLabel = _label.Substring(0, 1);
                string lastLabel = _label.Substring(1, label.Length - 1);
                btn.Inlines.Add(new Underline(new Run(firstLabel)));
                btn.Inlines.Add(new Run(lastLabel));
            }
            catch (Exception)
            {
                btn.Text = label;
            }
        }
        #endregion

        #region methods
        private void UserLogOn()
        {
            var loginWindow = new LoginControl(null, currentStyle) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            ClientAutenticationCredentials ret = loginWindow.GetCredentials();
            if (ret == null)
            {
                Application.Current.Shutdown();
            }
            else
            {
                SetBusy(true);

                Action action1 = () =>
                {
                    webRequestManager.ValidateUser(ret);
                };
                Action action2 = () =>
                {
                    if (ret.UserID == -1)
                    {
                        UserLogOn();
                    }
                    else if (ret == null)
                    {
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        SetBusy(true);
                        ClearValues();
                        if (webRequestManager.LoginInfo.UserType > 0)
                        {
                            btnAdvancedSettings.Visibility = Visibility.Visible;
                            btnAdvancedSettings.IsEnabled = true;

                            //btnCustomerTable.Visibility = Visibility.Visible;
                            //btnCustomerTable.IsEnabled = true;

                            //btnReportViewer.Visibility = Visibility.Visible;
                            //btnReportViewer.IsEnabled = true;

                        }
                    }
                };

                DoAction(action1, action2);
            }
        }

        private void SetTheme(string currentStyle)
        {
            ThemeHelper.SetTheme(this, currentStyle);
            switch (currentStyle)
            {
                case "Default": Background = Brushes.DarkGray; break;
                case "Blend": Background = new SolidColorBrush(Color.FromArgb(255, 51,51,51)); break;
                case "VS2010": Background = Brushes.LightGray; break;
                case "Office2007Black": Background = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)); break;
                case "Office2007Silver": Background = Brushes.LightGray; break;
                case "Office2007Blue": Background = new SolidColorBrush(Color.FromArgb(255, 176, 196, 222)); break;
                case "Office2010Black": Background = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)); break;
                case "Office2010Silver": Background = Brushes.LightGray; break;
                case "Office2010Blue": Background = new SolidColorBrush(Color.FromArgb(255, 176, 196, 222)); break;
                default: break;
            }
        }
        private void InitTheme()
        {
            themeKeeper.ItemsSource = new List<string>()
            {"Default",
            "Blend",
            "VS2010",
            "Office2007Black",
            "Office2007Silver",
            "Office2007Blue",
            "Office2010Black",
            "Office2010Silver",
            "Office2010Blue"};

        }
        private void MergeOptionOnPanel(object sender, RoutedEventArgs e)
        {
            LoadOptions(licencetype.SelectedItem as LicenceType, true);
        }

        private void ClearValues()
        {
            try
            {
                CombosInitialized = false;
                numRemoved.Text = string.Empty;
                licencetype.ItemsSource = null;
                licencetype.SelectedItem = null;
                customers.ItemsSource = null;
                customers.SelectedItem = null;
                siteCode.Text = string.Empty;
                instanceNumber.Value = 1;

                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                retLicenceType.Clear();
                retCustomers.Clear();
                retAreaGeoIds.Clear();
                retBOptions.Clear();
                retIOptions.Clear();
                retDefBOptions.Clear();
                retDefIOptions.Clear();
                MaptoLicTypeDefBoolValues.Clear();
                MaptoLicTypeDefIntValues.Clear();
                MapToMarkUp.Clear();

                Action action1 = () =>
                {
                    if (!ct.IsCancellationRequested)
                        //SerialNumber
                       // GetNewSerialAvailable();

                    if (!ct.IsCancellationRequested)
                        //Boolean Options
                        InitBoolOptions();

                    if (!ct.IsCancellationRequested)
                        //Analog Options
                        InitNumericOptions();

                    if (!ct.IsCancellationRequested)
                        FillCombos();
                };
                Action action2 = () =>
                {
                    numSerial.Value = 0;
                    boolOptionList.ItemsSource = retBOptions;
                    intOptionList.ItemsSource = retIOptions;

                    customers.ItemsSource = retCustomers;
                    licencetype.ItemsSource = retLicenceType;
                    CombosInitialized = true;

                    if (args != null && !string.IsNullOrEmpty(args.Filename) && File.Exists(args.Filename))
                        try
                        {
                            ReadFile(args.Filename);
                        }
                        catch (Exception ex)
                        {
                            ShowError(string.Format(Properties.Resources.ErrorMessage, ex.ToString()), true);
                        }

                };

                DoAction(action1, action2);
            }
            catch(Exception ex)
            {
                ShowError(string.Format("Error: {0}", ex.ToString()), true);
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
                    if (ret.IsFaulted)
                    {
                        Debug.WriteLine("I have observed a {0}",
                        ret.Exception.GetType().Name);
                        File.AppendAllText(AppLogPath, string.Format("{0}{1}", ret.Exception.ToString(), Environment.NewLine));
                    }
                    if (pendingTask.Contains(task1))
                        pendingTask.Remove(task1);

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
        private void InitBoolOptions()
        {
            retBOptions = webRequestManager.InitBoolOptions();
            retDefBOptions.AddRange(retBOptions);
            retBOptions.ForEach(x => MapToMarkUp.Add(x.MszParameter, x.AddInfo));
        }

        private void InitNumericOptions()
        {
            retIOptions = webRequestManager.InitNumericOptions();
            retDefIOptions.AddRange(retIOptions);
            retIOptions.ForEach(x => MapToMarkUp.Add(x.MszParameter, x.AddInfo));
        }

        private void InitCustomerList()
        {
            retCustomers = webRequestManager.GetCustomerList();
        }

        private List<IntOption> GetNumericOptions(int idType)
        {
            return webRequestManager.GetNumericOptions(idType);
        }

        private List<BoolOption> GetBoolOptions(int idType)
        {
            return webRequestManager.GetBoolOptions(idType);
        }

        private void InitLicTypeList()
        {
            retLicenceType = webRequestManager.GetLicTypeList();
        }

        private void ShowError(string message, bool isError)
        {
            if(isError)
                ShowMessage(message, Properties.Resources.ErrorCaption, MessageType.Error);
            else
                ShowMessage(message, Properties.Resources.ApplicationTitle, MessageType.Warning);
        }

        private void GetNewSerialAvailable()
        {
            RangeStartNr = webRequestManager.GetNextSerialAvailable(); 
        }

        private void ClearValue()
        {
            siteCode.Text = string.Empty;
            instanceNumber.Value = 1;
            logNote.Text = string.Empty;
            Order.Text = string.Empty;
            Bill.Text = string.Empty;
            FinalCustomer.Text = string.Empty;
            Euro.Text = string.Empty;
            boolOptionList.ItemsSource = null;
            intOptionList.ItemsSource = null;
            retBOptions.ForEach(x => { x.Value = false;});
            retIOptions.ForEach(x => { x.Value = 0;});
            boolOptionList.ItemsSource = retBOptions;
            intOptionList.ItemsSource = retIOptions;
            customers.SelectedIndex = -1;
            licencetype.SelectedIndex = -1;
        }

        private void WriteFile(string pt, string lic, string param)
        {
            File.WriteAllText(pt, lic);

            if (!(bool)Unlimited.IsChecked)
            {
                string _pt = LogPath;
                string customer = customers.SelectedIndex != -1 && customers.SelectedItemValue != null ? (customers.SelectedItemValue as Customer).Name : "Cliente Generico";
                string licence = licencetype.SelectedIndex != -1 && licencetype.SelectedItemValue != null ? (licencetype.SelectedItemValue as LicenceType).Name : "Licenza Generica";

                FileInfo txtfile = new FileInfo(LogPath);
                if (File.Exists(LogPath) && txtfile.Length > (1024 * 1024))       // ## NOTE: 1MB max file size
                {
                    File.Move(LogPath, System.IO.Path.Combine(System.IO.Path.GetDirectoryName(LogPath), string.Format("{0}{1}.txt", System.IO.Path.GetFileNameWithoutExtension(LogPath), string.Format("_{0}{1}{2}_{3}{4}{5}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second))));
                    File.Delete(LogPath);
                }

                if (logNote.Text.Length > 0)
                    File.AppendAllText(LogPath, string.Format("{5}{0} - generata licenza per:{5}{1} di tipo {2}{5}{3}{5}{4}", DateTime.Now, customer, licence, param, logNote.Text, Environment.NewLine));
                else
                    File.AppendAllText(LogPath, string.Format("{4}{0} - generata licenza per:{4}{1} di tipo {2}{4}{3}", DateTime.Now, customer, licence, param, Environment.NewLine));
            }

            Process.Start("explorer.exe", string.Format("/select,{0}", pt));
        }

        private void ReadSWLic()
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            Nullable<bool> result = op.ShowDialog();
            if (result == true)
            {
                ReadFile(op.FileName);
            }
        }

        private void ReadFile(string fileName)
        {
            SetBusy(true);
            try
            {
                System.IO.StreamReader sr = new
                    System.IO.StreamReader(fileName);
                var value = string.Empty;
                string ss = WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ==");
                string xKey = WPFUtilities.CryptString.CryptString.DecryptString(sr.ReadToEnd());
                sr.Close();
                var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xKey, ss, true, false);
                if (keyexpandolist.Count() != 0)
                {
                    var modules = keyexpandolist.ToList()[0] as IDictionary<string, object>;
                    //Serial
                    if (modules.Keys.Contains(_serial))
                        numSerial.Value = Convert.ToInt32((string)modules[_serial]);
                    //SiteCode
                    if (modules.Keys.Contains(PrevMarkUp))
                    {
                        siteCode.Text = WPFUtilities.CryptString.CryptString.EncryptString((string)modules[PrevMarkUp]);
                    }
                    //ExpiringDate
                    if (modules.Keys.Contains(DateMarkUp))
                    {
                        if (((string)modules[DateMarkUp]).Equals(WPFUtilities.CryptString.CryptString.DecryptString(NoDate)))
                        {
                            //dateTimePicker1.IsEnabled = false;
                            Unlimited.IsChecked = true;
                        }
                        else
                        {
                            //.IsEnabled = true;
                            Unlimited.IsChecked = false;
                            dateTimePicker1.DateTime = DateTime.Parse((string)modules[DateMarkUp], System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }

                    //{boolean}
                    foreach (var item in retBOptions)
                    {
                        try
                        {
                            string par = WPFUtilities.CryptString.CryptString.DecryptString(MapToMarkUp[item.MszParameter]);
                            item.Value = modules.Keys.Contains(par) ? true : false;
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                        }
                    }
                    //{numeric}
                    foreach (var item in retIOptions)
                    {
                        try
                        {
                            string par = WPFUtilities.CryptString.CryptString.DecryptString(MapToMarkUp[item.MszParameter]);
                            item.Value = modules.Keys.Contains(par) ? Convert.ToUInt32((string)modules[par]) : 0;
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                        }
                    }

                    boolOptionList.ItemsSource = null;
                    intOptionList.ItemsSource = null;
                    boolOptionList.ItemsSource = retBOptions;
                    intOptionList.ItemsSource = retIOptions;
                    customers.SelectedIndex = -1;
                    licencetype.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                ShowMessage(Properties.Resources.IllegalFileError, Properties.Resources.ErrorCaption, MessageType.Error);
            }
            SetBusy(false);
        }

        private bool checkAppType(out uint productId)
        {
            MSZDelRead.ProductId = uint.MaxValue;
            productId = 0;

            //read serial number
            uint serial = MSZDelRead.ReadMovSerialNumber();
            uint[] Data = new uint[REGISTER_MAX_COUNT];

            Data = MSZDelRead.ReadData();
            if (Data == null)
            {
                return true;
            }

            productId = MSZDelRead.ProductId;
            //Application type
            switch ((MSZ.MSZDelRead.ApplicationType)MSZDelRead.ProductId)
            {
                case MSZ.MSZDelRead.ApplicationType.apMovicon:
                    return true;
                    break;
                case MSZ.MSZDelRead.ApplicationType.apMovTrace:
                case MSZ.MSZDelRead.ApplicationType.apMoviconBA:
                    return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        bool GetLicType(string text, string title, MessageType type, bool setHW, out bool swLic)
        {
            MessageContent message = new MessageContent(text, type);
            Grid grid = new Grid();
            StackPanel selection = new StackPanel() 
            {   
                HorizontalAlignment = HorizontalAlignment.Center, 
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical
            };
            RadioButton radioButton = new RadioButton() { FontSize = 22, GroupName = "licType", Content = Properties.Resources.HWLicType, IsChecked = setHW };
            RadioButton radioButton1 = new RadioButton() { FontSize = 22, GroupName = "licType", Content = Properties.Resources.SWLicType, IsChecked = !setHW };
            selection.Children.Add(radioButton);
            selection.Children.Add(radioButton1);
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Star) });
            grid.Children.Add(message);
            grid.Children.Add(selection);
            Grid.SetRow(message, 0);
            Grid.SetRow(selection, 1);


            GeneralDialogButtons buttons = GeneralDialogButtons.OkCancelButtons;
            GeneralDialogContent wnd = new GeneralDialogContent(grid, buttons)
            {
                Owner = this,
                Title = title
            };
            ThemeHelper.SetTheme(grid, currentStyle);
            var res = wnd.ShowDialog();
            swLic = !(bool)radioButton.IsChecked;
            return res == true;
        }

        bool ShowMessage(string text, string title, MessageType type, bool forceOkCancel = false)
        {
            MessageContent message = new MessageContent(text, type);
            GeneralDialogButtons buttons = GeneralDialogButtons.OkCancelButtons;
            if (!forceOkCancel && (type == MessageType.Error || type==MessageType.Warning || type == MessageType.Info))
                buttons = GeneralDialogButtons.OkButton;
            GeneralDialogContent wnd = new GeneralDialogContent(message, buttons)
            {
                Owner = this,
                Title = title
            };
            ThemeHelper.SetTheme(message, currentStyle);
            return wnd.ShowDialog() == true;
        }

        private void WriteSGLock()
        {
            try
            {
                ClearKeyError();

                if (numSerial.Value == 0)
                {
                    ShowMessage(Properties.Resources.InsertValidSerialWarning, Properties.Resources.ErrorCaption, MessageType.Warning);
                    return;
                }
                uint productId;
                if (!checkAppType(out productId))
                {
                    if(!ShowMessage(string.Format(Properties.Resources.WriteKeyWarning, Enum.GetName(typeof(MSZ.MSZDelRead.ApplicationType), productId)), 
                        Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                    {
                        return;
                    }
                }

                MovApp.IsChecked = true;
                //unlimited value is mandatory for SGLock 
                Unlimited.IsChecked = true;
                //dateTimePicker1.IsEnabled = false;

                uint serial = (uint)numSerial.Value;
                siteCode.Text = string.Empty;

                uint aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovicon;
                if ((bool)MovTrace.IsChecked)
                    aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovTrace;
                else if ((bool)MovBa.IsChecked)
                    aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMoviconBA;
                string err = string.Empty;
                Action action1 = () =>
                {
                    MSZDelRead.WriteMovSerialNumber(serial);

                    uint[] Data = new uint[REGISTER_MAX_COUNT];


                    //Sitecode 12-16 bytes
                    //string sc = WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                    //byte[] btemp = System.Text.Encoding.ASCII.GetBytes(sc); //new byte[16];
                    byte[] btemp = System.Text.Encoding.ASCII.GetBytes(WPFUtilities.CryptString.CryptString.DecryptString(pgrCode)); //new byte[16];
                    for (int i = 0; i < btemp.Length / 4; i++)
                    {
                        Data[i] = BitConverter.ToUInt32(btemp, i * 4);
                    }
                    ////ExpiringDate 6 bytes
                    //btemp = new byte[8];

                    btemp = new byte[8];

                    //MovNext programming Code
                    btemp[0] = Convert.ToByte(0);
                    btemp[1] = Convert.ToByte(0);
                    btemp[2] = Convert.ToByte(0);
                    btemp[3] = Convert.ToByte(0);
                    btemp[4] = Convert.ToByte(0);
                    btemp[5] = Convert.ToByte(0);


                    Data[4] = BitConverter.ToUInt32(btemp, 0);
                    Data[5] = BitConverter.ToUInt32(btemp, 4);
                    //ActivationDate 6 bytes
                    btemp[0] = 0;// Convert.ToByte(dateTimePicker1.Value.Day);
                    btemp[1] = 0;//Convert.ToByte(dateTimePicker1.Value.Month);
                    btemp[2] = 0;//Convert.ToByte(dateTimePicker1.Value.Year);
                    btemp[3] = 0;//Convert.ToByte(dateTimePicker1.Value.Hour);
                    btemp[4] = 0;//Convert.ToByte(dateTimePicker1.Value.Minute);
                    btemp[5] = 0;//Convert.ToByte(dateTimePicker1.Value.Second);
                    int idx = 5;
                    Data[++idx] = BitConverter.ToUInt32(btemp, 0);
                    Data[++idx] = BitConverter.ToUInt32(btemp, 4);
                    //bool options 8 bytes
                    btemp[0] = 0;
                    btemp[1] = 0;
                    btemp[2] = 0;
                    btemp[3] = 0;
                    btemp[4] = 0;
                    btemp[5] = 0;
                    btemp[6] = 0;
                    btemp[7] = 0;


                    //bool options
                    BoolOption _bret = null;
                    var hwBOptions = retBOptions.OrderBy(x => x.OrderInfo).ToList();

                    for (int i = 0; i < hwBOptions.Count; i++)
                    {
                        _bret = hwBOptions[i];
                        var index = (_bret.OrderInfo - 1) / 8;
                        var power = (_bret.OrderInfo - 1) - (index * 8);
                        if (_bret != null && _bret.Value)
                        {
                            btemp[index] |= (byte)Math.Pow(2, power);
                        }
                    }

                    Data[++idx] = BitConverter.ToUInt32(btemp, 0);
                    Data[++idx] = BitConverter.ToUInt32(btemp, 4);

                    //int options
                    IntOption _iret = null;
                    var hwIOptions = retIOptions.OrderBy(x => x.OrderInfo).ToList();
                    for (int i = 0; i < hwIOptions.Count; i++)
                    {
                        _iret = hwIOptions[i];
                        if (_iret != null && _iret.Value >= 0)
                        {
                            int index = i >= 5 ? 11 + i : 10 + i;
                            Data[index] = Convert.ToUInt32(_iret.Value);
                        }
                    }

                    try
                    {
                        MSZDelRead.WriteData(Data, aptype);
                    }
                    catch (Exception ex)
                    {
                        err = ex.Message;
                        return;
                    }

                    return;

                };
                Action action2 = () =>
                {
                    if(err.Length > 0)
                    {
                        ShowMessage(string.Format(Properties.Resources.HwWritingError, err), Properties.Resources.ErrorCaption, MessageType.Error);
                    }
                    else
                    {
                        CheckSGLock();
                    }
                };
                DoAction(action1, action2);


            }
            catch (Exception e)
            {

                ShowMessage(string.Format(Properties.Resources.HwWritingError, e.Message), Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }
        }

        private void CheckSGLock()
        {
            MSZ.MSZDelRead.ApplicationType proID = MSZ.MSZDelRead.ApplicationType.apMovicon;
            string _pgrtemp = string.Empty;
            Action action1 = () =>
            {
                MSZDelRead.ProductId = uint.MaxValue;

                uint[] Data = new uint[REGISTER_MAX_COUNT];
                Data = MSZDelRead.ReadData();
                if (Data == null)
                    return;

                //sitecode
                byte[] btemp = new byte[16];
                int k = -1;
                for (int i = 0; i < 4; i++)
                {
                    btemp[++k] = (byte)Data[i];
                    btemp[++k] = (byte)(Data[i] >> 8);
                    btemp[++k] = (byte)(Data[i] >> 16);
                    btemp[++k] = (byte)(Data[i] >> 24);
                }

                string sc = System.Text.Encoding.ASCII.GetString(btemp);
                _pgrtemp = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));
            };

            Action action2 = () =>
            {
                if (_pgrtemp.Equals(pgrCode))
                    ShowMessage(Properties.Resources.OperationCompleted, Properties.Resources.ApplicationTitle, MessageType.Info);
                else
                {
                    ShowKeyError();
                    ShowMessage(string.Format(Properties.Resources.WritingSGLockWarning, Properties.Resources.KeyEmptyWarning), 
                        Properties.Resources.ApplicationTitle, MessageType.Error);
                }
            };

            DoAction(action1, action2);

        }

        private void ReadDBLicense(int serial = -1)
        {
            string _serial = serial == -1 ? string.Empty : serial.ToString();
            int idLicenceType = -1;
            int idLicence = -1;
            int idAnagrafe = -1;
            int _instanceNumber = -1;
            string code = string.Empty;
            string _removedCode = string.Empty;
            string _order = string.Empty;
            string _siteCode = string.Empty;
            string _bill = string.Empty;
            string _price = string.Empty;
            string _finalCustomer = string.Empty;
            string _note = string.Empty;
            string _desc = string.Empty;
            string _numSerial = _serial == string.Empty ? numSerial.Value.ToString() : _serial;
            byte[] _fileKey = null;
            bool ret = false;
            DateTime? _sKExpiredDate = DateTime.MinValue;
            LicType _licType = LicType.HW;
            Action action1 = () =>
            {
                LicenceInfo _licenseInfo = null;
                if (!ct.IsCancellationRequested)
                    _licenseInfo = webRequestManager.GetSerialInfo(_numSerial);

                if (!ct.IsCancellationRequested && _licenseInfo != null)
                {
                    idLicence = _licenseInfo.ID;
                    idLicenceType = _licenseInfo.IDType;
                    idAnagrafe = _licenseInfo.CustomerID;
                    code = _licenseInfo.CustomerCode.ToString();

                    _instanceNumber = _licenseInfo.InstanceNumber;

                    _removedCode = _licenseInfo.RemovedCode;
                    _order = _licenseInfo.Order;
                    _siteCode = _licenseInfo.SiteCode;
                    _bill = _licenseInfo.Bill;
                    _price = _licenseInfo.Price;
                    _finalCustomer = _licenseInfo.FinalCustomer;
                    _note = _licenseInfo.Note;
                    _desc = _licenseInfo.LicDescr;
                    _fileKey = _licenseInfo.FileKey;
#if DEBUG
                    try
                    {
                        var lidec = WPFUtilities.CryptString.CryptString.DecryptString(Encoding.ASCII.GetString(_fileKey));
                    }
                    catch (Exception)
                    {
                    }
#endif
                    _sKExpiredDate = _licenseInfo.SKExpiredDate;
                    _licType = _licenseInfo.IDLicType;
                    ret = webRequestManager.GetSerialOptions(_licenseInfo.ID.ToString(), retBOptions, retIOptions);
                }
            };

            Action action2 = () =>
            {
                if(ret)
                {
                    boolOptionList.ItemsSource = null;
                    intOptionList.ItemsSource = null;
                    boolOptionList.ItemsSource = retBOptions;
                    intOptionList.ItemsSource = retIOptions;
                    if (!string.IsNullOrEmpty(_removedCode))
                    {
                        numRemoved.Text = _removedCode;
                        numRemoved.IsEnabled = false;
                        btnCheck.IsEnabled = false;
                    }
                    else
                    {
                        numRemoved.Text = string.Empty;
                        numRemoved.IsEnabled = true;
                        btnCheck.IsEnabled = true;
                    }

                    numSerial.Text = _numSerial;
                    instanceNumber.Value = _instanceNumber;
                    description.Text = _desc; ;
                    Order.Text = _order;
                    siteCode.Text = _siteCode == hWInstance || _siteCode == multiInstance ? string.Empty : _siteCode;
                    Bill.Text = _bill;
                    FinalCustomer.Text = _finalCustomer;
                    Euro.Text = _price;

                    logNote.Text = _note;
                    var license = (from l in (licencetype.ItemsSource as List<LicenceType>) where l.ID == idLicenceType select l).FirstOrDefault();
                    licencetype.SelectedItem = license;
                    var customer = (from c in (customers.ItemsSource as List<Customer>) where c.ID == idAnagrafe && c.Code == code select c).FirstOrDefault();
                    customers.SelectedItem = customer;

                    if (_siteCode == multiInstance /*|| (string.IsNullOrEmpty(_siteCode) && _fileKey != null && _fileKey.Count() > 0)*/)
                    {
                        btnNetCore_Click(null, null);
                    }
                    else
                    {
                        btnWindows_Click(null, null);
                    }

                    if (!string.IsNullOrEmpty(_removedCode))
                    {
                        btnWindows.IsEnabled = false;
                        btnNetCore.IsEnabled = false;
                        siteCode.IsEnabled = false;
                        numRemoved.Text = _removedCode;
                        numRemoved.IsEnabled = false;
                        btnCheck.IsEnabled = false;

                        btn7.IsEnabled = false;
                        btnSaveOptionRange.IsEnabled = false;
                        btn1.IsEnabled = false;
                        btn2.IsEnabled = false;
                        btn3.IsEnabled = false;
                        btn23.IsEnabled = false;
                        btn5.IsEnabled = false;
                        btn4.IsEnabled = false;
                    }
                    else
                    {
                        numRemoved.Text = string.Empty;
                        numRemoved.IsEnabled = true;
                        btnCheck.IsEnabled = true;

                        btn7.IsEnabled = true;
                        btnSaveOptionRange.IsEnabled = true;
                        btn1.IsEnabled = true;
                        btn2.IsEnabled = true;
                        btn3.IsEnabled = true;
                        btn23.IsEnabled = true;
                        btn5.IsEnabled = true;
                        btn4.IsEnabled = true;
                    }


                    if (_siteCode == hWInstance)
                        dbLicType.Text = Properties.Resources.HWLicType;
                    else if(_licType == LicType.UnlimitedSW /*_sKExpiredDate.Equals(DateTime.MinValue)*/)
                    {
                        dbLicType.Text = Properties.Resources.UnlimitedSWLicType;
                        dateTimePicker1.DateTime = DateTime.Now;
                        Unlimited.IsChecked = true;
                    }
                    else
                    {
                        dbLicType.Text = Properties.Resources.SWLicType;
                        dateTimePicker1.DateTime = _sKExpiredDate.Value;
                        Unlimited.IsChecked = false;
                    }

                    dbLicType.Visibility = Visibility.Visible;
                    Storyboard storyboard = dbLicType.TryFindResource("blink") as Storyboard;
                    if(storyboard != null)
                    {
                        storyboard.Completed += (o, e) =>
                        {
                            dbLicType.Visibility = Visibility.Collapsed;
                        };
                        storyboard.Begin(dbLicType);
                    }

                }
                else
                {
                    //ClearValues();
                    ShowMessage(Properties.Resources.NoLicenseFoundError, Properties.Resources.ErrorCaption, MessageType.Error);
                }
            };

            DoAction(action1, action2);
        }
        bool useNetCore = false;
        private LicenceInfo AddHistory(bool forceCreation = false, string serial = null, bool bSilentMode = false, bool swLic = false, bool skipChoice = false)
        {
            LicenceInfo _licenseInfo = null;
            string _numSerial = string.IsNullOrEmpty(serial) ? numSerial.Value.ToString() : serial;

            if (!bSilentMode)
            {
                SetBusy(true);
                _licenseInfo = webRequestManager.GetSerialInfo(_numSerial);
                SetBusy(false);
                if (_licenseInfo != null)
                {
                    if (_licenseInfo.CustomerID != (customers.SelectedItemValue as Customer).ID)
                    {
                        if (!ShowMessage(Properties.Resources.LicenseDifferentCustomerWarning, Properties.Resources.ErrorCaption, MessageType.Error, true))
                            return null;
                    }
                    else if (!ShowMessage(Properties.Resources.LicenseEditingWarning, Properties.Resources.ErrorCaption, MessageType.Error, true))
                        return null;
                }
            }

            bool bSwLic = false;
            //if (numSerial.Value < RangeStartNr)
            if (!bSilentMode)
            {
                if (numSerial.Value == 0 && siteCode.Text.Length == 0 && !useNetCore)
                {
                    ShowMessage(Properties.Resources.SiteCodeTemporaryLicenseWarning, Properties.Resources.ErrorCaption, MessageType.Warning);
                    return null;
                }

                if (customers.SelectedIndex == -1)
                {
                    ShowMessage(Properties.Resources.SelectCustomerWarning, Properties.Resources.ErrorCaption, MessageType.Warning);
                    customers.Focus();
                    return null;
                }

                if (licencetype.SelectedIndex == -1)
                {
                    ShowMessage(Properties.Resources.SelectLicTypeWarning, Properties.Resources.ErrorCaption, MessageType.Warning);
                    licencetype.Focus();
                    return null;
                }
                if (siteCode.Text.Length == 0)
                {
                    if (useNetCore)
                    {
                        if (ShowMessage(Properties.Resources.InseringMultiInstanceSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                            bSwLic = true;
                        else
                            return null;
                    }
                    else if (!skipChoice)
                    {
                        if (!GetLicType(Properties.Resources.SelectLicenseType, Properties.Resources.ApplicationTitle, MessageType.Warning, false, out bSwLic))
                            return null;
                        else if (!(bool)Unlimited.IsChecked && numSerial.Value < Properties.Settings.Default.MinSerialPerLimitedLicenses && bSwLic)
                        {
                            ShowMessage(Properties.Resources.LimitedLicensesWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                            return null;
                        }
                        else if (!bSwLic)
                            Unlimited.IsChecked = true;
                    }
                    else
                        bSwLic = swLic;
                    //else if (ShowMessage(Properties.Resources.InseringHWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                    //    bSwLic = false;
                    //else if (ShowMessage(Properties.Resources.InseringSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                    //    bSwLic = true;
                    //else
                    //    return null;
                }
                else
                {
                    //if (!ShowMessage(Properties.Resources.InseringSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                    //    return null;
                    //bSwLic = true;
                    if (!skipChoice)
                    {
                        if (!GetLicType(Properties.Resources.SelectLicenseType, Properties.Resources.ApplicationTitle, MessageType.Warning, false, out bSwLic))
                            return null;
                        else if (!(bool)Unlimited.IsChecked && numSerial.Value < Properties.Settings.Default.MinSerialPerLimitedLicenses && bSwLic)
                        {
                            ShowMessage(Properties.Resources.LimitedLicensesWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                            return null;
                        }
                        else if (!bSwLic)
                            Unlimited.IsChecked = true;

                    }
                    else if (skipChoice)
                        bSwLic = swLic;
                    if (!bSwLic)
                        siteCode.Text = string.Empty;
                }
                if (siteCode.Text.Length > 0)
                {
                    try
                    {
                        WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                    }
                    catch (Exception)
                    {
                        ShowMessage(Properties.Resources.SiteCodeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                        return null;
                    }
                }
                //else
                //    siteCode.Text = WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==");
            }
            else
                bSwLic = swLic;

            bool bCreateLCode = false;
            try
            {
                if (!bSilentMode)
                {
                    SetBusy(true);
                    _licenseInfo = webRequestManager.GetSerialInfo(_numSerial);
                    SetBusy(false);
                    if (_licenseInfo != null)
                    {
                        //if (_licenseInfo.CustomerID != (customers.SelectedItemValue as Customer).ID)
                        //{
                        //    if (!ShowMessage(Properties.Resources.LicenseDifferentCustomerWarning, Properties.Resources.ErrorCaption, MessageType.Error, true))
                        //        return null;
                        //}
                        //else if (!ShowMessage(Properties.Resources.LicenseEditingWarning, Properties.Resources.ErrorCaption, MessageType.Error, true))
                        //    return null;

                        if ((bSwLic && siteCode.Text.Length > 0) || forceCreation) 
                            bCreateLCode = true;
                        else
                            bCreateLCode = false;

                        SetBusy(true);
                        _licenseInfo = UpdateLicense(bCreateLCode, bSwLic, _licenseInfo);
                        if (_licenseInfo == null)
                        {
                            ShowMessage(Properties.Resources.LicenseUpdatingError, Properties.Resources.ErrorCaption, MessageType.Error);
                            SetBusy(false);
                            return null;
                        }
                        else
                        {
                            SetBusy(false);
                            return _licenseInfo;
                        }
                    }
                    else if (bSwLic)
                        bCreateLCode = true;
                    else
                        bCreateLCode = false;

                }
                SetBusy(true);
                _licenseInfo = UpdateLicense(bCreateLCode, bSwLic, bSilentMode: bSilentMode, serial: serial);
                if (_licenseInfo == null)
                {
                    ShowMessage(Properties.Resources.LicenseUpdatingError, Properties.Resources.ErrorCaption, MessageType.Error);
                    SetBusy(false);
                    return null;
                }

                SetBusy(false);
                return _licenseInfo;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, Properties.Resources.ErrorCaption, MessageType.Error);
                SetBusy(false);
            }
            finally
            {
                SetBusy(false);
            }
            return null;
        }

        private LicenceInfo UpdateLicense(bool bCreateLCode, bool bSwLic, LicenceInfo infos = null, bool bSilentMode = false, string serial = null)
        {
            LicenceInfo _licenseInfo = new LicenceInfo();

            if (customers.SelectedIndex != -1)
            {
                _licenseInfo.CustomerID = (customers.SelectedItemValue as Customer).ID;
                _licenseInfo.CustomerCode = (customers.SelectedItemValue as Customer).Code;
            }
            if(!bSilentMode)
            {
                _licenseInfo.SerialNumber = (int)numSerial.Value;
                _licenseInfo.RemovedCode = numRemoved.Text;
                _licenseInfo.SiteCode = siteCode.Text.Length == 0 && !bSwLic ? hWInstance : siteCode.Text;
            }
            else
                _licenseInfo.SerialNumber = int.Parse(serial);

            _licenseInfo.IDLicType = bSwLic ? (bool)Unlimited.IsChecked ? LicType.UnlimitedSW : LicType.TemporarySW : LicType.HW;
            if (!bSilentMode || useNetCore)
            {
                if (string.IsNullOrEmpty(_licenseInfo.SiteCode) && useNetCore)
                    _licenseInfo.SiteCode = multiInstance;
                if (siteCode.Text.Length > 0 || useNetCore || bSwLic)
                    _licenseInfo.IDLicType = (bool)Unlimited.IsChecked ? LicType.UnlimitedSW : LicType.TemporarySW;
                else
                    _licenseInfo.IDLicType = LicType.HW;
                //_licenseInfo.GenerateSWCode = true;
                _licenseInfo.SWCodeOptions = GetSWOptions();
                //_licenseInfo.FileKey = null;
                if(bCreateLCode)
                {
                    var craddle = new Craddle();
                    string lic = craddle.Generate(0,
                                            _licenseInfo.SiteCode,
                                            _licenseInfo.SWCodeOptions);
                    _licenseInfo.FileKey = Encoding.ASCII.GetBytes(lic);
                }
                else if(string.IsNullOrEmpty(_licenseInfo.SiteCode) &&
                       ((_licenseInfo.IDLicType == LicType.TemporarySW && _licenseInfo.SerialNumber > Properties.Settings.Default.MinSerialPerLimitedLicenses) ||
                        (_licenseInfo.IDLicType == LicType.TemporarySW && _licenseInfo.SerialNumber == 0) ||
                        _licenseInfo.IDLicType == LicType.UnlimitedSW))
                {
                    _licenseInfo.FileKey = null;
                }


                if (infos == null || (infos != null && (infos.SiteCode != _licenseInfo.SiteCode || string.IsNullOrEmpty(_licenseInfo.SiteCode))))
                {
                    _licenseInfo.SKUserName = webRequestManager.LoginInfo.UserName;
                    _licenseInfo.SKGenerationDate = DateTime.UtcNow;
                }
            }
            _licenseInfo.LicDescr = description.Text;
            _licenseInfo.Order = Order.Text;
            _licenseInfo.Bill = Bill.Text;
            _licenseInfo.Price = Euro.Text;
            _licenseInfo.FinalCustomer = FinalCustomer.Text;
            _licenseInfo.Note = logNote.Text;
            _licenseInfo.ProductID = IDProdotto;
            _licenseInfo.Description = description.Text;
            _licenseInfo.InstanceNumber = (int)instanceNumber.Value;
            if (licencetype.SelectedIndex != -1)
                _licenseInfo.IDType = (licencetype.SelectedItemValue as LicenceType).ID;

            DateTime aDay = dateTimePicker1.DateTime;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 00, 00, 00);

            if (dateTimePicker1.IsEnabled && !(bool)Unlimited.IsChecked)
                _licenseInfo.SKExpiredDate = aDate;
            else
                _licenseInfo.SKExpiredDate = DateTime.MinValue;

           return webRequestManager.UpdateSerialOptions(_licenseInfo, retBOptions, retIOptions);
        }

        private string GetSWOptions()
        {
            StringBuilder param = new StringBuilder();
            //ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            DateTime aDay = dateTimePicker1.DateTime;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 00, 00, 00);
            param.Append(string.Format("{1}={0};", aDate.ToString(System.Globalization.CultureInfo.InvariantCulture), ExpD));
            //UN Unlimited -> 0,1
            param.Append(string.Format("{1}={0};", (bool)Unlimited.IsChecked ? 1 : 0, UnTd));

            //{boolean}
            foreach (var item in retBOptions)
            {
                param.Append(string.Format("{0}={1};", item.MszParameter, item.Value ? 1 : 0));
            }
            //{numeric}
            foreach (var item in retIOptions)
            {
                param.Append(string.Format("{0}={1};", item.MszParameter, item.Value));
            }
            //NSR numSerial -> UINT32
            param.Append(string.Format("{1}={0}", (bool)Unlimited.IsChecked ? numSerial.Value : numSerial.Value < Properties.Settings.Default.MinSerialPerLimitedLicenses ? 0 : numSerial.Value, NSrl));

            return param.ToString();
        }

        internal static string WriteLogMessage(String format, params object[] args)
        {
            var logMessage = String.Format("{0} - {1}", DateTime.Now, String.Format(format, args));
            Console.WriteLine(logMessage);
            return logMessage;
        }
   
        private void ReadSGLock()
        {
            MSZ.MSZDelRead.ApplicationType proID = MSZ.MSZDelRead.ApplicationType.apMovicon;
            uint serial = 0;
            string _pgrtemp = string.Empty;
            bool checkUnlimited = false;
            DateTime date = new DateTime();
            Action action1 = () =>
            {
                MSZDelRead.ProductId = uint.MaxValue;

                //read serial number
                serial = MSZDelRead.ReadMovSerialNumber();
                
                uint[] Data = new uint[REGISTER_MAX_COUNT];
                Data = MSZDelRead.ReadData();
                if (Data == null)
                    return;

                //Application type
                proID = (MSZ.MSZDelRead.ApplicationType)MSZDelRead.ProductId;

                //sitecode
                byte[] btemp = new byte[16];
                int k = -1;
                for (int i = 0; i < 4; i++)
                {
                    btemp[++k] = (byte)Data[i];
                    btemp[++k] = (byte)(Data[i] >> 8);
                    btemp[++k] = (byte)(Data[i] >> 16);
                    btemp[++k] = (byte)(Data[i] >> 24);
                }

                string sc = System.Text.Encoding.ASCII.GetString(btemp);
                _pgrtemp = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));
                if (!_pgrtemp.Equals(pgrCode))
                    return;

                //ExpiringDate
                k = -1;
                btemp[++k] = (byte)Data[4];
                btemp[++k] = (byte)(Data[4] >> 8);
                btemp[++k] = (byte)(Data[4] >> 16);
                btemp[++k] = (byte)(Data[4] >> 24);
                btemp[++k] = (byte)Data[5];
                btemp[++k] = (byte)(Data[5] >> 8);
                btemp[++k] = (byte)(Data[5] >> 16);
                btemp[++k] = (byte)(Data[5] >> 24);
                try
                {
                    if (btemp[2] == 0 &&
                       btemp[1] == 0 &&
                       btemp[0] == 0 &&
                       btemp[3] == 0 &&
                       btemp[4] == 0 &&
                       btemp[5] == 0)
                    {
                        checkUnlimited = true;
                    }
                    else
                    {
                        checkUnlimited = false;
                        date = new DateTime(btemp[2] + 2000, btemp[1], btemp[0], btemp[3], btemp[4], btemp[5]);
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                }
                //Activation date

                //bool option
                k = -1;
                btemp[++k] = (byte)Data[8];
                btemp[++k] = (byte)(Data[8] >> 8);
                btemp[++k] = (byte)(Data[8] >> 16);
                btemp[++k] = (byte)(Data[8] >> 24);
                btemp[++k] = (byte)Data[9];
                btemp[++k] = (byte)(Data[9] >> 8);
                btemp[++k] = (byte)(Data[9] >> 16);
                btemp[++k] = (byte)(Data[9] >> 24);


                //bool options
                BoolOption _bret = null;
                var hwBOptions = retBOptions.OrderBy(x => x.OrderInfo).ToList();

                for (int i = 0; i < hwBOptions.Count; i++)
                {
                    _bret = hwBOptions[i];
                    var index = (_bret.OrderInfo - 1) / 8;
                    var power = (_bret.OrderInfo - 1) - (index * 8);
                    if (_bret != null)
                    {
                        _bret.Value = (btemp[index] & (byte)Math.Pow(2, power)) > 0;
                    }
                }


                //int options
                IntOption _iret = null;
                var hwIOptions = retIOptions.OrderBy(x => x.OrderInfo).ToList();
                for (int i = 0; i < hwIOptions.Count; i++)
                {
                    _iret = hwIOptions[i];
                    if (_iret != null && _iret.Value >= 0)
                    {
                        int index = i >= 5 ? 11 + i : 10 + i;
                        _iret.Value = Convert.ToUInt32(Data[index]);
                    }
                }
            };

            Action action2 = () =>
            {
                switch (proID)
                {
                    case MSZ.MSZDelRead.ApplicationType.apMovicon:
                        MovApp.IsChecked = true;
                        break;
                    case MSZ.MSZDelRead.ApplicationType.apMovTrace:
                        MovTrace.IsChecked = true;
                        break;
                    case MSZ.MSZDelRead.ApplicationType.apMoviconBA:
                        MovBa.IsChecked = true;
                        break;
                }

                if (_pgrtemp.Equals(pgrCode))
                    ClearKeyError();
                else
                {
                    ShowKeyError();
                    return;
                }

                if (checkUnlimited)
                {
                    Unlimited.IsChecked = true;
                    //dateTimePicker1.IsEnabled = false;
                }
                else
                {
                    Unlimited.IsChecked = false;
                    //dateTimePicker1.IsEnabled = true;
                    dateTimePicker1.DateTime = date;
                }

                siteCode.Text = string.Empty;
                numSerial.Value = serial;
                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                boolOptionList.ItemsSource = retBOptions;
                intOptionList.ItemsSource = retIOptions;
                customers.SelectedIndex = -1;
                licencetype.SelectedIndex = -1;
            };

            DoAction(action1, action2);
            
        }

        private void ShowKeyError()
        {
            sglockCheck.Visibility = Visibility.Visible;
        }

        private void ClearKeyError()
        {

            sglockCheck.Visibility = Visibility.Collapsed;
        }

        private void CreateHWUpdateFile(string lic)
        {
            string pt = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), string.Format("{0}.key", numSerial.Value));
            
            if (string.IsNullOrEmpty(lic))
                ShowError(Properties.Resources.LicenseUpdatingError, true);
            else
            {
                File.WriteAllText(pt, lic);
                Process.Start("explorer.exe", string.Format("/select,{0}", pt));
            }
        }

        private void Unlimited_Click(object sender, RoutedEventArgs e)
        {
            if(!(bool)Unlimited.IsChecked)
                numSerial.Value = 0;
            dateTimePicker1.DateTime = DateTime.Now;
        }
        private void ReadSWLic(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            ReadSWLic();
        }
        private void ReloadAndInitValues(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            ClearValues();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            ClearValue();
            //GetNewSerialAvailable();
            description.Text = null;
            numSerial.Value = 0;

            numRemoved.Text = string.Empty;
            numRemoved.IsEnabled = true;
            siteCode.IsEnabled = true;
            btnCheck.IsEnabled = true;

            btn7.IsEnabled = true;
            btnSaveOptionRange.IsEnabled = true;
            btn1.IsEnabled = true;
            btn2.IsEnabled = true;
            btn3.IsEnabled = true;
            btn23.IsEnabled = true;
            btn5.IsEnabled = true;
            btn4.IsEnabled = true;
            btnWindows_Click(null, null);
        }
        private void CreateSWLic(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            if (siteCode.Text.Length == 0 && !useNetCore)
            {
                ShowMessage(Properties.Resources.SiteCodeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }
            else if (siteCode.Text.Length > 0)
            {
                try
                {
                    WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                }
                catch (Exception)
                {
                    ShowMessage(Properties.Resources.SiteCodeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                    return;
                }
            }

            if (numSerial.Value == 0)
            {
                ShowMessage(Properties.Resources.InsertValidSerialWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }
            else if(!(bool)Unlimited.IsChecked && numSerial.Value < Properties.Settings.Default.MinSerialPerLimitedLicenses)
            {
                ShowMessage(Properties.Resources.LimitedLicensesWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }

            LicenceInfo lic = AddHistory(skipChoice: true, swLic: true);
            if (lic != null)
                CreateSWLic(Encoding.ASCII.GetString(lic.FileKey));

            ReadDBLicense();
        }

        private void CreateSWLic(string lic)
        {
            if (string.IsNullOrEmpty(lic))
                ShowError(Properties.Resources.LicenseUpdatingError, true);
            else
            {
                string pt = FilePath;
                File.WriteAllText(pt, lic);
                Process.Start("explorer.exe", string.Format("/select,{0}", FilePath));
            }
        }

        private void ReadHWKey(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            ReadSGLock();
        }

        private void CreateHWUpdateFile(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            MovApp.IsChecked = true;
            //unlimited value is mandatory for SGLock 
            Unlimited.IsChecked = true;

            if (numSerial.Value == 0)
            {
                ShowMessage(Properties.Resources.InsertValidSerialWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }
            //if (DateTime.Compare(dateTimePicker1.DateTime, DateTime.Now) < 0)
            //    dateTimePicker1.DateTime = DateTime.Now;

            LicenceInfo lic = AddHistory(forceCreation:true, skipChoice: true, swLic: false);
            if (lic != null)
                CreateHWUpdateFile(Encoding.ASCII.GetString(lic.FileKey));
        }

        private void ReadDBLicense_Click(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            ReadDBLicense();
        }

        private void OpenLogFile(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/open,{0}", LogPath));
        }

        private void AddToDB_Click(object sender, RoutedEventArgs e)
        {
            AddToDB();
        }

        private void AddToDB()
        {
            ClearKeyError();
            LicenceInfo lic = AddHistory();
            if (lic != null) 
                ShowMessage(Properties.Resources.OperationCompleted, Properties.Resources.ApplicationTitle, MessageType.Info);
            
            ReadDBLicense();
        }

        private void ManageWarning(string res)
        {
            if (webRequestManager.ResIsInError(res))
            {
                ShowError(res.Substring(RequestType.Error.ToString().Length - 1), true);
            }
            else if (webRequestManager.ResIsIllegal(res))
            {
                ShowError(res.Substring(RequestType.Illegal.ToString().Length - 1),true);
            }

        }

        private void UpdateAndWriteHWKey(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            LicenceInfo lic = AddHistory(skipChoice: true, swLic: false);
            if (lic != null)
                WriteSGLock();
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (numRemoved.Text == WPFUtilities.CryptString.CryptString.EncryptString(numSerial.Value.ToString()))
                    ShowError(Properties.Resources.ValidRemovingCode, false);
                else
                    ShowError(Properties.Resources.InvalidRemovingCode,true);
            }
            catch (Exception ex)
            {
                ShowError(string.Format("Error: {0}", ex.ToString()), true);
            }
        }

        private void btnDisable_Click(object sender, RoutedEventArgs e)
        {
            instanceNumber.Value = -1;
        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            instanceNumber.Value = 1;
        }

        private void GetNewSerialAvailable(object sender, RoutedEventArgs e)
        {
            //GetNewSerialAvailable();
            numSerial.Value = 0;
        }

        CancellationTokenSource tokenSource;
        CancellationToken ct;
        TaskScheduler sc = null;
        bool CombosInitialized;
        void FillCombos()
        {
            if (CombosInitialized)
                return;
            //Clienti
            InitCustomerList();
            //Tipi licenza
            InitLicTypeList();
            //AreeGeoID
            InitAreas();
        }

        private void InitAreas()
        {
            retAreaGeoIds = webRequestManager.GetAreeGeoIDList();
        }

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

        private void LoadOptions(LicenceType selectedItem, bool bMergeOptions = false)
        {
            if (selectedItem == null)
                return;
            int idType = selectedItem.ID;

            SetBusy(true);

            Action action1 = () =>
            {
                if (!ct.IsCancellationRequested && !MaptoLicTypeDefBoolValues.ContainsKey(idType))
                    MaptoLicTypeDefBoolValues.Add(idType, GetBoolOptions(idType));
                if (!ct.IsCancellationRequested && !MaptoLicTypeDefIntValues.ContainsKey(idType))
                    MaptoLicTypeDefIntValues.Add(idType, GetNumericOptions(idType));

                List<BoolOption> _booloptions = MaptoLicTypeDefBoolValues[idType];
                List<IntOption> _intoptions = MaptoLicTypeDefIntValues[idType];
                if (ct.IsCancellationRequested)
                    return;

                retBOptions.ForEach(x =>
                {
                    BoolOption _opt = _booloptions.Find(opt => opt.ID == x.ID);
                    if (_opt != null)
                    {
                        x.Value = bMergeOptions ? _opt.Value ? true : x.Value : _opt.Value;
                    }
                });
                if(!bMergeOptions)
                    retIOptions.ForEach(x =>
                    {
                        IntOption _opt = _intoptions.Find(opt => opt.ID == x.ID);
                        if (_opt != null)
                        {
                            x.Value = _opt.Value;
                            x.Enabled = _opt.Enabled;
                        }
                        else
                        {
                            x.Value = 0;
                            x.Enabled = false;
                        }
                    });
            };

            Action action2 = () =>
            {
                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                boolOptionList.ItemsSource = retBOptions;
                intOptionList.ItemsSource = retIOptions;
            };

            DoAction(action1, action2);
        }

        private void LoadOptionsOnPanel(object sender, RoutedEventArgs e)
        {
            LoadOptions(licencetype.SelectedItem as LicenceType);
        }
        private void LogOffUser(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            LogOffUser();
        }

        void LogOffUser()
        {
            CombosInitialized = false;
            numRemoved.Text = string.Empty;
            licencetype.ItemsSource = null;
            licencetype.SelectedItem = null;
            customers.ItemsSource = null;
            customers.SelectedItem = null;
            numSerial.Value = 0;
            Order.Text = string.Empty;
            Bill.Text = string.Empty;
            FinalCustomer.Text = string.Empty;
            Euro.Text = string.Empty;
            logNote.Text = string.Empty;
            boolOptionList.ItemsSource = null;
            intOptionList.ItemsSource = null;
            retLicenceType.Clear();
            retCustomers.Clear();
            retBOptions.Clear();
            retIOptions.Clear();
            MaptoLicTypeDefBoolValues.Clear();
            MaptoLicTypeDefIntValues.Clear();
            MapToMarkUp.Clear();
            webRequestManager.LogOffUser();
            btnAdvancedSettings.Visibility = Visibility.Collapsed;
            btnAdvancedSettings.IsEnabled = false;
            //btnCustomerTable.Visibility = Visibility.Collapsed;
            //btnCustomerTable.IsEnabled = false;
            //btnReportViewer.Visibility = Visibility.Collapsed;
            //btnReportViewer.IsEnabled = false;
            UserLogOn();
        }

        private void CreateTemporarySWLic(object sender, RoutedEventArgs e)
        {
            if (numSerial.Value > 0 && !ShowMessage(Properties.Resources.TemporaryLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
            {
                return;
            }

            if (siteCode.Text.Length == 0 && !useNetCore)
            {
                ShowMessage(Properties.Resources.SiteCodeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                return;
            }
            else if (siteCode.Text.Length > 0)
            {
                try
                {
                    WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                }
                catch (Exception)
                {
                    ShowMessage(Properties.Resources.SiteCodeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                    return;
                }
            }

            ClearKeyError();

            Unlimited.IsChecked = false;
            //if (sender == btn60DaysSWKey)
            //    dateTimePicker1.DateTime = DateTime.Now.AddDays(60);
            //if (sender == btn1YearSWKey)
            //    dateTimePicker1.DateTime = DateTime.Now.AddDays(365);
            if (dateTimePicker1.DateTime.CompareTo(dateTimePicker1.MaxValue) == 1)
                dateTimePicker1.DateTime = dateTimePicker1.MaxValue.Value.AddDays(-1);

            numSerial.Value = 0;

            if (DateTime.Compare(dateTimePicker1.DateTime, DateTime.Now) < 0)
                dateTimePicker1.DateTime = DateTime.Now;
            LicenceInfo lic = AddHistory(skipChoice: true, swLic: true);
            if (lic != null)
                CreateSWLic(Encoding.ASCII.GetString(lic.FileKey));
        }

        private void OpenLicenseTable_Click(object sender, RoutedEventArgs e)
        {
            LoadLicenseDetails();
        }

        private void LoadLicenseDetails()
        {
            try
            {
                Licenses licenses = new Licenses(webRequestManager, retDefBOptions, retDefIOptions, currentStyle) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                ThemeHelper.SetTheme(licenses, currentStyle);
                licenses.ShowDetailsList();
                licenses.Error += (o, e) =>
                {
                    Dispatcher.BeginInvokeIfRequired(() =>
                    {
                        ShowError(e.ErrorMessage, true);
                    });
                };
                licenses.SelectionChanged += (o, e) =>
                {
                    if (licenses.selectedLicence != null && e.ID != -1)
                    {
                        //ReadDBLicense(e.ID);
                        licenses.retBOptions.AsParallel().ForAll(x =>
                        {
                            BoolOption _opt = retBOptions.Find(opt => opt.ID == x.ID);
                            if (_opt != null)
                            {
                                _opt.Value = x.Value;
                            }
                        });
                        licenses.retIOptions.AsParallel().ForAll(x =>
                        {
                            IntOption _opt = retIOptions.Find(opt => opt.ID == x.ID);
                            if (_opt != null)
                            {
                                _opt.Value = x.Value;
                            }
                        });

                        boolOptionList.ItemsSource = null;
                        intOptionList.ItemsSource = null;
                        boolOptionList.ItemsSource = retBOptions;
                        intOptionList.ItemsSource = retIOptions;

                        numRemoved.Text = licenses.selectedLicence.RemovedCode;
                        instanceNumber.Value = licenses.selectedLicence.InstanceNumber;
                        numSerial.Text = licenses.selectedLicence.SerialNumber.ToString();
                        description.Text = licenses.selectedLicence.LicDescr;
                        Order.Text = licenses.selectedLicence.Order;
                        var _siteCode = licenses.selectedLicence.SiteCode;
                        var _fileKey = licenses.selectedLicence.FileKey;
                        siteCode.Text = _siteCode == hWInstance || _siteCode == multiInstance ? string.Empty : _siteCode;
                        instanceNumber.Value = licenses.selectedLicence.InstanceNumber;
                        Bill.Text = licenses.selectedLicence.Bill;
                        FinalCustomer.Text = licenses.selectedLicence.FinalCustomer;
                        Euro.Text = licenses.selectedLicence.Price; 
                        logNote.Text = licenses.selectedLicence.Note;
                        var license = (from l in (licencetype.ItemsSource as List<LicenceType>) where l.ID == (int)licenses.selectedLicence.IDType select l).FirstOrDefault();
                        licencetype.SelectedItem = license;
                        var customer = (from c in (customers.ItemsSource as List<Customer>) where c.ID == (int)licenses.selectedLicence.CustomerID && c.Code == licenses.selectedLicence.CustomerCode select c).FirstOrDefault();
                        customers.SelectedItem = customer;

                        if (_siteCode == multiInstance || (string.IsNullOrEmpty(_siteCode) && _fileKey != null && _fileKey.Count() > 0))
                            btnNetCore_Click(null, null);
                        else
                            btnWindows_Click(null, null);
                    }
                };
                licenses.Closing += (o, e) =>
                {
                    licenses.Dispose();
                };

            }
            catch (Exception ex)
            {
                SetBusy(false);
                File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
            }
        }
        private void OpenCustomerTable_Click(object sender, RoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            Customers customers = new Customers(webRequestManager, retAreaGeoIds, retCustomers, currentStyle) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            ThemeHelper.SetTheme(customers, currentStyle);
            customers.ShowDetailsList();
        }

        private void OpenAdvanceInsertion_Click(object sender, RoutedEventArgs e)
        {
            ClearKeyError();
            bool bError = false;
            bool bWarning = false;
            if (customers.SelectedIndex == -1)
            {
                ShowMessage(Properties.Resources.SelectCustomerWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                customers.Focus();
                return;
            }

            if (licencetype.SelectedIndex == -1)
            {
                ShowMessage(Properties.Resources.SelectLicTypeWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                licencetype.Focus();
                return;
            }


            bool bSWLic = false;

            if (useNetCore)
            {
                if (ShowMessage(Properties.Resources.InseringMultiInstanceSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
                    bSWLic = true;
                else
                    return;
            }
            else
            {
                if (!GetLicType(Properties.Resources.SelectLicenseType, Properties.Resources.ApplicationTitle, MessageType.Warning, false, out bSWLic))
                    return;
                else if (!bSWLic)
                    Unlimited.IsChecked = true;

                if (bSWLic)
                    btnWindows_Click(null, null);
            }

            //if (!ShowMessage(Properties.Resources.InseringHWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
            //{
            //    if (useNetCore && ShowMessage(Properties.Resources.InseringMultiInstanceSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
            //        bSWLic = true;
            //    else
            //    {
            //        if (!ShowMessage(Properties.Resources.InseringSWLicenseWarning, Properties.Resources.ApplicationTitle, MessageType.Warning, true))
            //            return;
            //        else
            //        {
            //            btnWindows_Click(null, null);
            //            bSWLic = true;
            //        }
            //    }
            //}
            //else
            //    bSWLic = false;

            int serial;
            int.TryParse(numSerial.Text, out serial);
            var customerID = (customers.SelectedItem as Customer).ID;
            AdvancedControl advancedControl = new AdvancedControl(serial, serial + 1) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            if (advancedControl.ShowDetailsList())
            {
                List<int> list = new List<int>();
                if (advancedControl.Selection == Options.Range)
                {
                    int from = advancedControl.FromSerial;
                    int to = advancedControl.ToSerial;
                    if (to < from)
                    {
                        from = advancedControl.ToSerial;
                        to = advancedControl.FromSerial;
                    }
                    list.AddRange(Enumerable.Range(from, to - from + 1));
                }
                else
                {
                    var numbers = advancedControl.SerialNumbers?.Split(';');
                    if (numbers != null)
                    {
                        numbers.ToList().ForEach(x =>
                        {
                            int number;
                            if (int.TryParse(x, out number) && !list.Contains(number))
                                list.Add(number);
                        });
                    }
                }
                bool unlimitedWarning = false;
                StringBuilder stringBuilder = new StringBuilder();
                StringBuilder wstringBuilder = new StringBuilder();
                list.ForEach(serialNum =>
                {
                    try
                    {
                        if (!(bool)Unlimited.IsChecked && serialNum < Properties.Settings.Default.MinSerialPerLimitedLicenses && bSWLic)
                        {
                            unlimitedWarning = true;
                            stringBuilder.Append(string.Format(Properties.Resources.LicenseNotInsertedWarning, serialNum));
                            stringBuilder.Append(Environment.NewLine);
                            bError = true;
                            return;
                        }

                        var _licenseInfo = webRequestManager.GetSerialInfo(serialNum.ToString());
                        LicenceInfo lic = AddHistory(serial: serialNum.ToString(), bSilentMode: true, swLic: bSWLic);
                        if (lic == null)
                        {
                            stringBuilder.Append(string.Format(Properties.Resources.LicenseNotInsertedWarning, serialNum));
                            stringBuilder.Append(Environment.NewLine);
                            bError = true;
                            return;
                        }

                        if (_licenseInfo.CustomerID != (customers.SelectedItemValue as Customer).ID)
                        {
                            wstringBuilder.Append(string.Format(Properties.Resources.LicenseDifferentCustomerWarning1, serialNum, (customers.SelectedItemValue as Customer).Name));
                            wstringBuilder.Append(Environment.NewLine);
                            bWarning = true;
                        }
                    }
                    catch (Exception)
                    {
                        stringBuilder.Append(string.Format(Properties.Resources.LicenseNotInsertedWarning, serialNum));
                        stringBuilder.Append(Environment.NewLine);
                        bError = true;
                    }
                });

                if (bWarning)
                    ShowMessage(wstringBuilder.ToString(), Properties.Resources.ApplicationTitle, MessageType.Warning);

                if (!bError)
                {
                    if (!bWarning)
                        ShowMessage(Properties.Resources.OperationCompleted, Properties.Resources.ApplicationTitle, MessageType.Info);
                }
                else
                {
                    if (unlimitedWarning)
                        ShowMessage(Properties.Resources.LimitedLicensesWarning, Properties.Resources.ErrorCaption, MessageType.Error);
                    ShowMessage(stringBuilder.ToString(), Properties.Resources.ErrorCaption, MessageType.Error);
                }
            }
        }
        private void themeKeeper_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                currentStyle = (string)themeKeeper.SelectedItem;
                SetTheme(currentStyle);
            }
            catch (Exception)
            {
            }
        }

        private void OpenAdvanceSettings_Click(object sender, RoutedEventArgs e)
        {
            LicenceSettings licenceSettings = new LicenceSettings(webRequestManager, retDefBOptions, retDefIOptions, currentStyle) { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            ThemeHelper.SetTheme(licenceSettings, currentStyle);
            licenceSettings.ShowDetailsList();
            if(licenceSettings.Dirty)
                ClearValues();
        }
        private void OnShowLicences(object sender, ExecutedRoutedEventArgs e)
        {
            LoadLicenseDetails();
        }
        #endregion

        private void OnShowCustomers(object sender, ExecutedRoutedEventArgs e)
        {
            LoadCustomers();
        }

        private void OnSaveToDB(object sender, ExecutedRoutedEventArgs e)
        {
            AddToDB();
        }

        private void OnReadLicense(object sender, ExecutedRoutedEventArgs e)
        {
            ClearKeyError();
            ReadDBLicense();
        }

        private void instanceNumber_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (instanceNumber.Value == 0)
                instanceNumber.Value = 1;

            if (instanceNumber.Value == -1)
            {
                btnDisable.Visibility = Visibility.Collapsed;
                btnEnable.Visibility = Visibility.Visible;
                suspendedWarning.Visibility = Visibility.Visible;
                instanceNumber.IsEnabled = false;
                instanceNumber.Background = Brushes.Red;
            }
            else
            {
                btnDisable.Visibility = Visibility.Visible;
                btnEnable.Visibility = Visibility.Collapsed;
                suspendedWarning.Visibility = Visibility.Collapsed;
                instanceNumber.IsEnabled = true;
                instanceNumber.Background = Brushes.White;
            }
        }

        private void btnNetCore_Click(object sender, RoutedEventArgs e)
        {
            netcoreWarning.Visibility = Visibility.Visible;
            btnWindows.Visibility = Visibility.Visible;
            btnNetCore.Visibility = Visibility.Collapsed;
            siteCode.IsEnabled = false;
            siteCode.Text = string.Empty;
            siteCode.Background = Brushes.Red;
            useNetCore = true;
        }

        private void btnWindows_Click(object sender, RoutedEventArgs e)
        {
            netcoreWarning.Visibility = Visibility.Collapsed;
            btnWindows.Visibility = Visibility.Collapsed;
            btnNetCore.Visibility = Visibility.Visible;
            siteCode.IsEnabled = true;
            siteCode.Background = Brushes.White;
            useNetCore = false;
        }

        private void SetCalendar(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int monthes;
            Unlimited.IsChecked = false;
            if (int.TryParse(button?.Tag.ToString(), out monthes))
                dateTimePicker1.DateTime = DateTime.Now.AddMonths(monthes);
        }
    }
}
