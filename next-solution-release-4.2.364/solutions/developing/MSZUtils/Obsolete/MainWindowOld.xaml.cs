using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using DevExpress.Xpo;
using Microsoft.Win32;
using MSZ;
using MSZFactory;
using Utilities;
using DataReader.Helpers;
using MSZUtilsServiceHelper;

namespace MSZUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowOld : Window, IDisposable
    {
        #region declarations
        bool bLoaded;
        //DB
        //string ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zp/0Q8zC5u6IVmzT7u2x4R8R7U7H19YF7ux3uviV7lBzYbKl9qnbx9aCeNzfqyxWpZEuNoLrFaIOY1BtJqCe5iWX+TqGr5ir0BukZOwxnRepkjlU2TQZoNKQg2ISFSJ1sWMJb6fWmSmDrqADHzjMFM4");
        //DB_Test
        string ConnectionString = WPFUtilities.CryptString.CryptString.DecryptString("JaiqPjwg5IdMVDQ9MIqGwpByA/vyEJ7Hau72harj5zp/0Q8zC5u6IVmzT7u2x4R8R7U7H19YF7ux3uviV7lBzYbKl9qnbx9aCeNzfqyxWpZEuNoLrFaIOY1BtJqCe5iWS+5IXJBmJKHc4+o+0zGJT+66l7TY3200GUiqDc7fB75jRFUaRhKRrNq92KSCGjZ3");
        Dictionary<int, List<BoolOption>> MaptoLicTypeDefBoolValues = new Dictionary<int, List<BoolOption>>();
        Dictionary<int, List<IntOption>> MaptoLicTypeDefIntValues = new Dictionary<int, List<IntOption>>();
        Dictionary<string, string> MapToMarkUp = new Dictionary<string, string>();
        Dictionary<string, string> MapToMszPar = new Dictionary<string, string>();


        List<LicenceType> retLicenceType = new List<LicenceType>();
        List<Customer> retCustomers = new List<Customer>();
        List<BoolOption> retBOptions = new List<BoolOption>();
        List<IntOption> retIOptions = new List<IntOption>();

        String _defaultDataProvider = string.Empty;
        String _defaultConnectionString = string.Empty;
        int IDProdotto { get; set; }
        int RangeStartNr { get; set; }

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
                //return String.Format("{0}\\{1}",
                //          Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                //          WPFUtilities.CryptString.CryptString.DecryptString("Uzw5RtLX40qWDZmLrPI6Ku25WBMOgAg+8pJS8UdlRlw="));
            }
        }
        private static string AppLogPath
        {
            get
            {
                return System.IO.Path.Combine(GetAssemblyPath(), "MSZUtils.log");
            }
        }

        static string GetAssemblyPath()
        {
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            return basedir;
        }
        private static string PrinterPath
        {
            get
            {
                return System.IO.Path.Combine(GetAssemblyPath(), WPFUtilities.CryptString.CryptString.DecryptString("x4X18P4WcJZHvouVH3HgxQ=="));
                //return String.Format("{0}\\{1}",
                //          Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                //          WPFUtilities.CryptString.CryptString.DecryptString("x4X18P4WcJZHvouVH3HgxQ=="));
            }
        }


        private static string NoDate = "rg51dnJNzvrnhxjPoi4zmw==";
        private static string pgrCode = "aqyFuVmb6LP+jg3oTfy4Dhz2ys8ZxGGjtEDqhdp/mf8=";
        private string PrevMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("BTEKkPCMPZn/v6d2OM7ekQ=="); } }//SiteCode
        private string DateMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("HOa44ZEOiZwUy5LheAVQUw=="); } }//ExpiringDate
        private string ActivationMarkUp { get { return WPFUtilities.CryptString.CryptString.DecryptString("t+V7RhAwpnVWTiyYcYIy+w=="); } }//ActivationDate
        private string ExpiringDelta { get { return WPFUtilities.CryptString.CryptString.DecryptString("prv/e1GpOfRUtjX+HrMM2A=="); } }//30
        private string AppMarkup { get { return WPFUtilities.CryptString.CryptString.DecryptString(""); } }//AppType

        private string _server { get { return WPFUtilities.CryptString.CryptString.DecryptString("aWWmz7U/Nodh7mSA9L8oAQ=="); } }//("SVR");
        private string _runtime { get { return WPFUtilities.CryptString.CryptString.DecryptString("ABmVqlvmqLNT3RbPaCt1Pg=="); } }//("RT");
        private string _developer { get { return WPFUtilities.CryptString.CryptString.DecryptString("2/gZ3e0cSqiFpdGw57W2ZA=="); } }//("DEV");
        private string _client { get { return WPFUtilities.CryptString.CryptString.DecryptString("ZNsToNGejkH59OOzi7bWvQ=="); } }//("CMD");
        private string _datalogger { get { return WPFUtilities.CryptString.CryptString.DecryptString("QfHPNIlk1a6nPnEyRK9clw=="); } }//("DLR");
        private string _recipe { get { return WPFUtilities.CryptString.CryptString.DecryptString("nCEgMHYEH5RLOaN03LCbxw=="); } }//("RCP");
        private string _vbnet { get { return WPFUtilities.CryptString.CryptString.DecryptString("/e40Vrioka3qo/RSOgI2xw=="); } }//("VB");
        private string _scheduler { get { return WPFUtilities.CryptString.CryptString.DecryptString("TfIcH8BA94C2MbB6w+u4Mw=="); } }//("SCD");
        private string _networking { get { return WPFUtilities.CryptString.CryptString.DecryptString("qr7ETdFumhQnxIJbEbhorQ=="); } }//("NTW");
        private string _redundancy { get { return WPFUtilities.CryptString.CryptString.DecryptString("+TUE/Sg1qoePVZu7g+dR+w=="); } }//("RED");
        private string _geolocal { get { return WPFUtilities.CryptString.CryptString.DecryptString("xh0e5EYFBkZtn0unXtNJfA=="); } }//("GEO");
        private string _3D { get { return WPFUtilities.CryptString.CryptString.DecryptString("NEcoOsNxAg4tjat/8T5zoQ=="); } }//("G3D");
        private string _report { get { return WPFUtilities.CryptString.CryptString.DecryptString("lohgq2F1Shi/lvnweCixaA=="); } }//("REP");
        private string _dispatcher { get { return WPFUtilities.CryptString.CryptString.DecryptString("iFUE/lJmjCniaH2/8irz5w=="); } }//("DIS");
        private string _alarmstat { get { return WPFUtilities.CryptString.CryptString.DecryptString("eDJgTyZ8URP38n06+AK8ug=="); } }//("STA");
        private string _opcuaserver { get { return WPFUtilities.CryptString.CryptString.DecryptString("2LEJER2ToEwTJrOoBYYDLg=="); } }//("OUAS");
        private string _deploy { get { return WPFUtilities.CryptString.CryptString.DecryptString("F8EDBcvXLdyv/OpSrhoqcg=="); } }//("WDEP");
        private string _webclient5 { get { return WPFUtilities.CryptString.CryptString.DecryptString("p6OOEyokimxEBwJapBYnaQ=="); } }//("WCL5");
        private string _webclient { get { return WPFUtilities.CryptString.CryptString.DecryptString("GWRgSK00mEYcSGJIfVzStQ=="); } }//("WCL");
        private string _servertag { get { return WPFUtilities.CryptString.CryptString.DecryptString("w89JD4DHxzgkFpU10NWGLA=="); } }//("STG");
        private string _clienttag { get { return WPFUtilities.CryptString.CryptString.DecryptString("1uZeuk41g5tEN8PkGiCwLw=="); } }//("CTG");
        private string _drivers { get { return WPFUtilities.CryptString.CryptString.DecryptString("PuXfSP24fhC4JL6M7LAQzA=="); } }//("DRV");
        private string _childs { get { return WPFUtilities.CryptString.CryptString.DecryptString("FmzaxK+3RVr/LUPMmVHeCQ=="); } }//("CHLD");
        private string _screens { get { return WPFUtilities.CryptString.CryptString.DecryptString("/H16Mdag/oIEZx3eDz8BEg=="); } }//("SCR");
        private string _alarms { get { return WPFUtilities.CryptString.CryptString.DecryptString("I+TwLvoQ7TUbaQ+YyVetAw=="); } }//("ALR");
        private string _little { get { return WPFUtilities.CryptString.CryptString.DecryptString("wstSI/P41LJeGb6158MEww=="); } }//("99");
        private string _big { get { return WPFUtilities.CryptString.CryptString.DecryptString("mxPFclWvubfF/qDzqyi2zg=="); } }//("9999999");
        private string _serial { get { return WPFUtilities.CryptString.CryptString.DecryptString("/2WysmBwr00nKx3ylLf4Pg=="); } }//("SN");
        private string _net { get { return WPFUtilities.CryptString.CryptString.DecryptString("zxVCYC4E/39MftYBrBiV3Q=="); } }//("NET");
        private string _proenergy { get { return WPFUtilities.CryptString.CryptString.DecryptString("gg9YQvhvx70kafcB0yaFmA=="); } }//("PEN")
        private string _prolean { get { return WPFUtilities.CryptString.CryptString.DecryptString("b2eHWGOY6pOyUuDOqvHGtw=="); } }//("PLN")
        private string _debug { get { return WPFUtilities.CryptString.CryptString.DecryptString("fug/a/XB+OiTnd/2ZdhCXA=="); } }//("DBG")

        private string _modbus { get { return WPFUtilities.CryptString.CryptString.DecryptString("8td5jGWD4tzp6zylCEPuww=="); } }//("MDB")
        private string _automation { get { return WPFUtilities.CryptString.CryptString.DecryptString("vSwyN4V9dnvKnDFD3OYujA=="); } }//("AUT")
        private string _telemetry { get { return WPFUtilities.CryptString.CryptString.DecryptString("0PQBKWp51zv/Ir+CTa8jTg=="); } }//("TLM")
        private string _facilities { get { return WPFUtilities.CryptString.CryptString.DecryptString("Yp4rzMtuSxNanPXI6JTivA=="); } }//("FCS")
        private string _iot { get { return WPFUtilities.CryptString.CryptString.DecryptString("SSaQsA8U3VuElRmBmDg8GQ=="); } }//("IOT")




        private int _disabled = 0;
        private int _small = 99;
        private int _huge = 9999999;
        private static string xCode = string.Empty;
        private const int REGISTER_MAX_BYTECOUNT = 188;
        private const int REGISTER_MAX_COUNT = 47;
        private string InputFile { get; set; }
        private uint mMovicon = 0;
        #endregion
        public MainWindowOld()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                if (!bLoaded)
                {
                    InitValues();
                    dateTimePicker1.DateTime = DateTime.Now;
                    sglockCheck.Visibility = Visibility.Collapsed;

                    var args = Environment.GetCommandLineArgs();
                    foreach (string s in args)
                    {
                        InputFile = s;
                    }
                    if (!string.IsNullOrEmpty(InputFile) && File.Exists(InputFile))
                        try
                        {
                            ReadFile(InputFile);
                        }
                        catch (Exception ex)
                        {
                        }
                }
            };
        }

        private void InitValues()
        {
            try
            {
                _defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(ConnectionString);
                _defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(ConnectionString);

                numRemoved.Text = string.Empty;
                licencetype.ItemsSource = null;
                customers.ItemsSource = null;
                boolOptionList.ItemsSource = null;
                intOptionList.ItemsSource = null;
                retLicenceType.Clear();
                retCustomers.Clear();
                retBOptions.Clear();
                retIOptions.Clear();
                MaptoLicTypeDefBoolValues.Clear();
                MaptoLicTypeDefIntValues.Clear();
                MapToMarkUp.Clear();
                MapToMszPar.Clear();

                MapToMarkUp.Add("DV", _developer);
                MapToMarkUp.Add("RT", _runtime);
                MapToMarkUp.Add("SR", _server);
                MapToMarkUp.Add("DL", _datalogger);
                MapToMarkUp.Add("CL", _client);
                MapToMarkUp.Add("RC", _recipe);
                MapToMarkUp.Add("VB", _vbnet);
                MapToMarkUp.Add("SC", _scheduler);
                MapToMarkUp.Add("WD", _deploy);
                MapToMarkUp.Add("DA", _report);
                MapToMarkUp.Add("OS", _opcuaserver);
                MapToMarkUp.Add("TD", _3D);
                MapToMarkUp.Add("AS", _alarmstat);
                MapToMarkUp.Add("GL", _geolocal);
                MapToMarkUp.Add("DR", _dispatcher);
                MapToMarkUp.Add("NW", _networking);
                MapToMarkUp.Add("RD", _redundancy);
                MapToMarkUp.Add("DB", _debug);

                MapToMarkUp.Add("MDO", _modbus);
                MapToMarkUp.Add("AUO", _automation);
                MapToMarkUp.Add("TLO", _telemetry);
                MapToMarkUp.Add("FCO", _facilities);
                MapToMarkUp.Add("IOO", _iot);



                MapToMarkUp.Add("NVS", _servertag);
                MapToMarkUp.Add("NVC", _clienttag);
                MapToMarkUp.Add("NDR", _drivers);
                MapToMarkUp.Add("NCH", _childs);
                MapToMarkUp.Add("NW5", _webclient5);
                MapToMarkUp.Add("NWC", _webclient);
                MapToMarkUp.Add("NAL", _alarms);
                MapToMarkUp.Add("NSC", _screens);
                MapToMarkUp.Add("NSR", _serial);
                MapToMarkUp.Add("NTL", _net);
                MapToMarkUp.Add("NPE", _proenergy);
                MapToMarkUp.Add("NPL", _prolean);
                
                MapToMszPar.Add(_developer, "DV");
                MapToMszPar.Add(_runtime, "RT");
                MapToMszPar.Add(_server, "SR");
                MapToMszPar.Add(_datalogger, "DL");
                MapToMszPar.Add(_client, "CL");
                MapToMszPar.Add(_recipe, "RC");
                MapToMszPar.Add(_vbnet, "VB");
                MapToMszPar.Add(_scheduler, "SC");
                MapToMszPar.Add(_deploy, "WD");
                MapToMszPar.Add(_report, "DA");
                MapToMszPar.Add(_opcuaserver, "OS");
                MapToMszPar.Add(_3D, "TD");
                MapToMszPar.Add(_alarmstat, "AS");
                MapToMszPar.Add(_geolocal, "GL");
                MapToMszPar.Add(_dispatcher, "DR");
                MapToMszPar.Add(_networking, "NW");
                MapToMszPar.Add(_redundancy, "RD");
                MapToMszPar.Add(_debug, "DB");

                MapToMszPar.Add(_modbus, "MDO");
                MapToMszPar.Add(_automation, "AUO");
                MapToMszPar.Add(_telemetry, "TLO");
                MapToMszPar.Add(_facilities, "FCO");
                MapToMszPar.Add(_iot, "IOO");


                MapToMszPar.Add(_servertag, "NVS");
                MapToMszPar.Add(_clienttag, "NVC");
                MapToMszPar.Add(_drivers, "NDR");
                MapToMszPar.Add(_childs, "NCH");
                MapToMszPar.Add(_webclient5, "NW5");
                MapToMszPar.Add(_webclient, "NWC");
                MapToMszPar.Add(_alarms, "NAL");
                MapToMszPar.Add(_screens, "NSC");
                MapToMszPar.Add(_serial, "NSR");
                MapToMszPar.Add(_net, "NTL");
                MapToMszPar.Add(_proenergy, "NPE");
                MapToMszPar.Add(_prolean, "NPL");
                
                if (string.IsNullOrEmpty(_defaultConnectionString) || string.IsNullOrEmpty(_defaultDataProvider))
                    return;
                try
                {
                    //ID Prodotto
                    DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbProdotti]",
                                                    "[NomeProdotto] Like '%Platform NExT%'", null, 
                                                    "[IdProdotto]"));
                    foreach (DataRowView rowView in dataView)
                    {
                        IDProdotto = (int)rowView["IdProdotto"];
                        RangeStartNr = (int)rowView["RangeInizio"];
                        break;
                    }

                    //SerialNumber
                    InitSerialNumber();

                    //Tipi licenza
                    dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbTipiLicenza]",
                                                    _IDProdotto, null, 
                                                    "[IdTipoLicenza]"));

                    foreach (DataRowView rowView in dataView)
                    {
                        retLicenceType.Add(new LicenceType()
                        {
                            ID = (int)rowView["IdTipoLicenza"],
                            Name = rowView["NomeTipoLicenza"].ToString(),
                            Description = rowView["Descrizione"].ToString(),
                        });

                        //Bool Def Options
                        StringBuilder _select = new StringBuilder("SELECT [IdTipoLicenza], ");
                        _select.Append("[tbOpzioni].[IdOpzione], [tbOpzioni].[Writable], [NomeOpzione], [ParametroMsz], ");
                        _select.Append("[Valore] ");
                        _select.Append("FROM [tbOpzioni] INNER JOIN [tbValOpzioniTipiLicenza] ON [tbOpzioni].[IdOpzione] = [tbValOpzioniTipiLicenza].[IdOpzione]");

                        DataView dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        _select.ToString(),
                                                        string.Format("[Abilitata] = 1 AND [TipoOpzione] = 'B' AND [IdTipoLicenza] = {0}", rowView["IdTipoLicenza"]), null, 
                                                        "[IdTipoLicenza]"));
                        List<BoolOption> retBDefOptions = new List<BoolOption>();

                        foreach (DataRowView rowdView in dataDefView)
                        {
                            retBDefOptions.Add(new BoolOption()
                            {
                                ID = (int)rowdView["IdOpzione"],
                                Name = rowdView["NomeOpzione"].ToString(),
                                MszParameter = rowdView["ParametroMsz"].ToString(),
                                Value = bool.Parse(rowdView["Valore"].ToString()),
                                Enabled = bool.Parse(rowdView["Writable"].ToString())
                            });
                        }

                        MaptoLicTypeDefBoolValues.Add((int)rowView["IdTipoLicenza"], retBDefOptions);

                        //Int Def Options
                        _select = new StringBuilder("SELECT [IdTipoLicenza], ");
                        _select.Append("[tbOpzioni].[IdOpzione], [tbOpzioni].[Writable], [NomeOpzione], [ParametroMsz], ");
                        _select.Append("[Valore] ");
                        _select.Append("FROM [tbOpzioni] INNER JOIN [tbValOpzioniTipiLicenza] ON [tbOpzioni].[IdOpzione] = [tbValOpzioniTipiLicenza].[IdOpzione]");

                        dataDefView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        _select.ToString(),
                                                        string.Format("[Abilitata] = 1 AND [TipoOpzione] = 'N' AND [IdTipoLicenza] = {0}", rowView["IdTipoLicenza"]), null, 
                                                        "[IdTipoLicenza]"));
                        List<IntOption> retIDefOptions = new List<IntOption>();

                        foreach (DataRowView rowdView in dataDefView)
                        {
                            retIDefOptions.Add(new IntOption()
                            {
                                ID = (int)rowdView["IdOpzione"],
                                Name = rowdView["NomeOpzione"].ToString(),
                                MszParameter = rowdView["ParametroMsz"].ToString(),
                                Value = uint.Parse(rowdView["Valore"].ToString()),
                                Enabled = bool.Parse(rowdView["Writable"].ToString())
                            });
                        }

                        MaptoLicTypeDefIntValues.Add((int)rowView["IdTipoLicenza"], retIDefOptions);
                    }

                    licencetype.ItemsSource = retLicenceType;

                    //Clienti
                    dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbAnagrafe]",
                                                    null, null, 
                                                    "[descrizion]"));

                    foreach (DataRowView rowView in dataView)
                    {
                        retCustomers.Add(new Customer()
                        {
                            ID = (int)rowView["AnagrafeID"],
                            Code = rowView["codice"].ToString(),
                            Name = string.Format("{0} ({1}) - {2}", rowView["descrizion"].ToString(), rowView["prov"].ToString(), rowView["codice"].ToString())
                        });
                    }
                    customers.ItemsSource = retCustomers;


                    //Boolean Options
                    dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbOpzioni]",
                                                    string.Format("[TipoOpzione] = 'B' AND [Abilitata] = 1 AND [IdOpzione] IN (SELECT [IdOpzione] FROM [tbOpzioniProdotti] WHERE {0})", _IDProdotto), null,
                                                    "[OrderNum]"));

                    foreach (DataRowView rowView in dataView)
                    {
                        retBOptions.Add(new BoolOption()
                        {
                            ID = (int)rowView["IdOpzione"],
                            Name = rowView["NomeOpzione"].ToString(),
                            MszParameter = rowView["ParametroMsz"].ToString(),
                            Enabled = (bool)rowView["Writable"],
                            Value = !(bool)rowView["Writable"]
                        });
                    }
                    boolOptionList.ItemsSource = retBOptions;

                    //Analog Options
                    dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbOpzioni]",
                                                    string.Format("[TipoOpzione] = 'N' AND [Abilitata] = 1 AND [IdOpzione] IN (SELECT [IdOpzione] FROM [tbOpzioniProdotti] WHERE {0})", _IDProdotto), null, 
                                                    "[IdOpzione]"));

                    foreach (DataRowView rowView in dataView)
                    {
                        retIOptions.Add(new IntOption()
                        {
                            ID = (int)rowView["IdOpzione"],
                            Name = rowView["NomeOpzione"].ToString(),
                            MszParameter = rowView["ParametroMsz"].ToString(),
                            Enabled = (bool)rowView["Writable"],
                            Value = 0
                        });
                    }
                    intOptionList.ItemsSource = retIOptions;


                }
                catch (Exception ex)
                {
                    File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
            }

             
        }

        private void InitSerialNumber()
        {
            numSerial.Value = RangeStartNr;

            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            "SELECT TOP 1 * FROM [tbLicenze]",
                                            string.Format("[NumeroLicenza] >= {0}",RangeStartNr), null, 
                                            "[NumeroLicenza] DESC"));
            foreach (DataRowView rowView in dataView)
            {
                numSerial.Value = (int)rowView["NumeroLicenza"] + 1;
                break;
            }
        }
        private void licencetype_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (licencetype.SelectedIndex == -1)
                return;
            try 
	        {
	           int _selectedIndex = (licencetype.SelectedValue as LicenceType).ID;
               List<BoolOption> _booloptions = MaptoLicTypeDefBoolValues[_selectedIndex];
               List<IntOption> _intoptions = MaptoLicTypeDefIntValues[_selectedIndex];
               boolOptionList.ItemsSource = null;
               intOptionList.ItemsSource = null;
               //retBOptions.Clear();
               //retIOptions.Clear();
               //retBOptions.AddRange(_booloptions); 
               //retIOptions.AddRange(_intoptions); 
               retBOptions.ForEach(x => {
                   BoolOption _opt = _booloptions.Find(opt => opt.ID == x.ID);
                   if (_opt != null)
                   {
                       x.Value = _opt.Value;
                       x.Enabled = _opt.Enabled;
                   }
                   else
                   {
                       x.Value = false;
                       x.Enabled = false;
                   }
               });

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


               boolOptionList.ItemsSource = retBOptions;
               intOptionList.ItemsSource = retIOptions;
	        }
	        catch (Exception ex)
	        {
                File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
	        }
        }


        private void ClearValue()
        {
            siteCode.Text = string.Empty;
            logNote.Text = string.Empty;
            Order.Text = string.Empty;
            Bill.Text = string.Empty;
            boolOptionList.ItemsSource = null;
            intOptionList.ItemsSource = null;
            retBOptions.ForEach(x => { x.Value = false; x.Enabled = true; });
            retIOptions.ForEach(x => { x.Value = 0; x.Enabled = true; });
            boolOptionList.ItemsSource = retBOptions;
            intOptionList.ItemsSource = retIOptions;
            customers.SelectedIndex = -1;
            licencetype.SelectedIndex = -1;
        }
        private byte[] CreateSWLic(bool writefile = true, bool opendialog = true)
        {
            sglockCheck.Visibility = Visibility.Collapsed;

            if (numSerial.Value == 0 && (bool)Unlimited.IsChecked)
            {
                MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                return null;
            }
            if (siteCode.Text.Length == 0)
            {
                MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                return null;
            }

            try
            {
                WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                return null;
            }

            string pt = FilePath;

            StringBuilder param = new StringBuilder();
            //ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            DateTime aDay = dateTimePicker1.DateTime;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);
            param.Append(string.Format("ED={0};", aDate.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            //UN Unlimited -> 0,1
            param.Append(string.Format("UN={0};", (bool)Unlimited.IsChecked ? 1 : 0));

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
            param.Append(string.Format("NSR={0}", (bool)Unlimited.IsChecked ? numSerial.Value : 0));

            var craddle = new Craddle();
            //string lic = pippo.Generate(
            //    (uint)(rMoviconBA.Checked ?
            //        MSZ.MSZDelRead.ApplicationType.apMoviconBA :
            //            (rMovTrace.Checked ? MSZ.MSZDelRead.ApplicationType.apMovTrace : MSZ.MSZDelRead.ApplicationType.apMovicon)),

            //                siteCode.Text,
            //                    param.ToString());
            string lic = craddle.Generate((uint)(MSZ.MSZDelRead.ApplicationType.apMoviconBA),
                                siteCode.Text,
                                param.ToString());

            //byte[] data = Encoding.ASCII.GetBytes(lic);

            if (writefile)
                WriteFile(pt, lic, param.ToString(), opendialog);

            //return lic;
            return Encoding.ASCII.GetBytes(lic);
        }

        private void WriteFile(string pt, string lic, string param, bool opendialog = true)
        {
            File.WriteAllText(pt, lic);

            if (!(bool)Unlimited.IsChecked)
            {
                string _pt = LogPath;
                string customer = customers.SelectedIndex != -1 && customers.SelectedValue != null ? (customers.SelectedValue as Customer).Name : "Cliente Generico";
                string licence = licencetype.SelectedIndex != -1 && licencetype.SelectedValue != null ? (licencetype.SelectedValue as LicenceType).Name : "Licenza Generica";

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

            if (opendialog)
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
            System.IO.StreamReader sr = new
                System.IO.StreamReader(fileName);
            var value = string.Empty;
            string ss = WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ==");
            string xKey = WPFUtilities.CryptString.CryptString.DecryptString(sr.ReadToEnd());
            sr.Close();
            var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xKey, ss, true, false);
            if (keyexpandolist.Count() != 0)
            {
                //var pippo = keyexpandolist.ToList();
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
                        dateTimePicker1.IsEnabled = false;
                        Unlimited.IsChecked = true;
                    }
                    else
                    {
                        dateTimePicker1.IsEnabled = true;
                        Unlimited.IsChecked = false;
                        dateTimePicker1.DateTime = DateTime.Parse((string)modules[DateMarkUp], System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                //{boolean}
                foreach (var item in retBOptions)
                {
                    try
                    {
                        item.Value = modules.Keys.Contains(MapToMarkUp[item.MszParameter]) ? true : false;
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
                        item.Value = modules.Keys.Contains(MapToMarkUp[item.MszParameter]) ? Convert.ToUInt32((string)modules[MapToMarkUp[item.MszParameter]]) : 0;
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
        private bool checkAppType()
        {
            MSZDelRead.ProductId = uint.MaxValue;

            //read serial number
            uint serial = MSZDelRead.ReadMovSerialNumber();
            uint[] Data = new uint[REGISTER_MAX_COUNT];

            Data = MSZDelRead.ReadData();
            if (Data == null)
            {
                return true;
            }

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
            }

            return true;
        }
        private bool WriteSGLock()
        {
            try
            {
                sglockCheck.Visibility = Visibility.Collapsed;

                if (numSerial.Value == 0)
                {
                    MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                    return true;
                }


                if (!checkAppType())
                {
                    if (MessageBox.Show(Properties.Resources.ApplicationError, Properties.Resources.ApplicationTitle, MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        return true;
                    }
                }

                MovApp.IsChecked = true;
                //unlimited value is mandatory for SGLock 
                Unlimited.IsChecked = true;
                dateTimePicker1.IsEnabled = false;

                uint serial = (uint)numSerial.Value;
                MSZDelRead.WriteMovSerialNumber(serial);

                uint[] Data = new uint[REGISTER_MAX_COUNT];

                siteCode.Text = string.Empty;

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

                //if (Unlimited.Checked)
                //{

                //MovNext programming Code
                btemp[0] = Convert.ToByte(0);
                btemp[1] = Convert.ToByte(0);
                btemp[2] = Convert.ToByte(0);
                btemp[3] = Convert.ToByte(0);
                btemp[4] = Convert.ToByte(0);
                btemp[5] = Convert.ToByte(0);
                //}
                //else 
                //{
                //    btemp[0] = Convert.ToByte(dateTimePicker1.Value.Day);
                //    btemp[1] = Convert.ToByte(dateTimePicker1.Value.Month);
                //    btemp[2] = Convert.ToByte(dateTimePicker1.Value.Year - 2000);
                //    //btemp[3] = Convert.ToByte(dateTimePicker1.Value.Hour);
                //    //btemp[4] = Convert.ToByte(dateTimePicker1.Value.Minute);
                //    //btemp[5] = Convert.ToByte(dateTimePicker1.Value.Second);
                //    btemp[3] = Convert.ToByte(23);
                //    btemp[4] = Convert.ToByte(59);
                //    btemp[5] = Convert.ToByte(59);
                //}


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

                BoolOption _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_developer]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 1;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_runtime]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 2;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_server]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 4;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_datalogger]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 8;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_client]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 16;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_recipe]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 32;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_vbnet]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 64;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_scheduler]);
                if (_bret != null && _bret.Value)
                    btemp[0] |= 128;

                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_deploy]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 1;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_report]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 2;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_opcuaserver]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 4;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_3D]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 8;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_alarmstat]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 16;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_geolocal]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 32;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_dispatcher]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 64;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_networking]);
                if (_bret != null && _bret.Value)
                    btemp[1] |= 128;

                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_redundancy]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 1;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_debug]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 2;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_modbus]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 4;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_automation]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 8;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_telemetry]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 16;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_facilities]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 32;
                _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_iot]);
                if (_bret != null && _bret.Value)
                    btemp[2] |= 64;

                Data[++idx] = BitConverter.ToUInt32(btemp, 0);
                Data[++idx] = BitConverter.ToUInt32(btemp, 4);
                //int options 64 bytes

                IntOption _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_servertag]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_clienttag]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_drivers]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_childs]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_webclient5]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                
                //_iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_webclient]); //no more used
                //if (_iret != null && _iret.Value >= 0)
                    //Data[++idx] = Convert.ToUInt32(_iret.Value);
                ++idx;
                Data[idx] = Convert.ToUInt32(_iret.Value);
                
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_alarms]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_screens]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_net]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_proenergy]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);
                _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_prolean]);
                ++idx;
                if (_iret != null)// && _iret.Value >= 0)
                    Data[idx] = Convert.ToUInt32(_iret.Value);



                uint aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovicon;
                if ((bool)MovTrace.IsChecked)
                    aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMovTrace;
                else if ((bool)MovBa.IsChecked)
                    aptype = (uint)MSZ.MSZDelRead.ApplicationType.apMoviconBA;

                try
                {
                    MSZDelRead.WriteData(Data, aptype);
                    sglockCheck.Visibility = Visibility.Collapsed;
                }
                catch(Exception ex)
                {
                    sglockCheck.Visibility = Visibility.Visible;
                    MessageBox.Show(string.Format("Errore in fase di scrittura: {0}",ex.Message), Properties.Resources.ErrorCaption);
                    return true;
                }

                return false;

            }
            catch (Exception e)
            {

                MessageBox.Show(string.Format("Errore in fase di scrittura: {0}", e.Message), Properties.Resources.ErrorCaption);
                return true;
            }
        }
        private void ReadDB()
        {
            //Licenze
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            "SELECT * FROM [tbLicenze]",
                                            string.Format("[NumeroLicenza] = {0}", numSerial.Value), null, 
                                            "[IdLicenza]"));
            if(dataView.Count > 0)
                foreach (DataRowView rowView in dataView)
                {
                    int idLicenceType = (int)rowView["IdTipoLicenza"];
                    int idLicence = (int)rowView["IdLicenza"];
                    int idAnagrafe = (int)rowView["AnagrafeID"];
                    string code = rowView["CodiceCliente"].ToString();

                    numRemoved.Text = rowView["RemovedCode"].ToString();
                    Order.Text = rowView["Ordine"].ToString();
                    siteCode.Text = rowView["SiteCode"].ToString() == WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==") ? string.Empty : rowView["SiteCode"].ToString();
                    Bill.Text = rowView["Fattura"].ToString();
                    logNote.Text =rowView["Note"].ToString();

                    foreach (LicenceType item in licencetype.Items)
                    {
                        if (item.ID == idLicenceType)
                        {
                            licencetype.SelectedItem = item;
                            break;
                        }
                    }

                    dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbLicenzeDettagli]",
                                                    string.Format("[IdLicenza] = {0}", idLicence), null, 
                                                    "[IdOpzione]"));

                    foreach (DataRowView rowOView in dataView)
                    {
                        try
                        {
                            //{boolean}
                            retBOptions.Find(x => x.ID == (int)rowOView["IdOpzione"]).Value = bool.Parse(rowOView["Valore"].ToString());
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                //{numeric}
                                retIOptions.Find(x => x.ID == (int)rowOView["IdOpzione"]).Value = uint.Parse(rowOView["Valore"].ToString());
                            }
                            catch (Exception ex1)
                            {
                                File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex1.ToString(), Environment.NewLine));
                            }
                        }
                    }


                    boolOptionList.ItemsSource = null;
                    intOptionList.ItemsSource = null;
                    boolOptionList.ItemsSource = retBOptions;
                    intOptionList.ItemsSource = retIOptions;
                    foreach (Customer item in customers.Items)
                    {
                        if (item.ID == idAnagrafe && item.Code == code)
                        {
                            customers.SelectedItem = item;
                            break;
                        }
                    }

                    break;
                }
            else
            {
                InitValues();
                MessageBox.Show(string.Format("Nessuna licenza trovata nel database!"), Properties.Resources.ErrorCaption);
            }
        }
        void PrintFile()
        {
            try
            {
                StringBuilder strOpt = new StringBuilder(numSerial.Text);
                strOpt.Append(";");
                strOpt.Append((licencetype.SelectedValue as LicenceType).Name);
                strOpt.Append(";");

                //{boolean}
                foreach (var item in retBOptions)
                {
                    strOpt.Append(item.Value ? string.Format("{0}-", item.MszParameter) : string.Empty);
                }

                string _strOpt = strOpt.ToString().Substring(0, strOpt.ToString().Length - 1);

                File.WriteAllText(PrinterPath, _strOpt);
                this.FileCreated += (o, e) =>
                {
                    Process.Start(System.IO.Path.Combine(GetAssemblyPath(), "PrintLabel.exe"));
                };

                CheckFileExists();


                //try
                //{
                //    Process.Start(System.IO.Path.Combine(GetAssemblyPath(), "PrintLabel.exe"));
                //}
                //catch (Exception ex)
                //{
                //    File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                //}
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Errore in fase di stampa: {0}", e.Message), Properties.Resources.ErrorCaption);
            }
        }
        
        public event EventHandler FileCreated;
        public void CheckFileExists()
        {
            while (!File.Exists(PrinterPath))
            {
                Thread.Sleep(1000);
            }
            FileCreated(this, new EventArgs());
        }

        public void PrintLabels(object sender, EventArgs e)
        {
          MessageBox.Show("File Created!");
        }

        static IDataLayer CreateLicDataLayer(String settings)
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(settings, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(tbLicenze));
            return new ThreadSafeDataLayer(dict, store);
        }

        static IDataLayer CreateLicStDataLayer(String settings)
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(settings, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(tbLicenzeStorico));
            return new ThreadSafeDataLayer(dict, store);
        }

        static IDataLayer CreateLicDtDataLayer(String settings)
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(settings, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(tbLicenzeDettagli));
            return new ThreadSafeDataLayer(dict, store);
        }

        static IDataLayer CreateLicDtStDataLayer(String settings)
        {
            var dict = new DevExpress.Xpo.Metadata.ReflectionDictionary();
            var store = XpoDefault.GetConnectionProvider(settings, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            dict.GetDataStoreSchema(typeof(tbLicenzeDettagliStorico));
            return new ThreadSafeDataLayer(dict, store);
        }

        private void AddHistory()
        {
            bool bSwLic;
            //if (numSerial.Value < RangeStartNr)
            if(numSerial.Value == 0)
            {
                MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                return;
            }

            if(customers.SelectedIndex == -1)
            {
                MessageBox.Show("Selezionare il cliente per cui inserire la licenza!", Properties.Resources.ErrorCaption);
                return;
            }

            if (licencetype.SelectedIndex == -1)
            {
                MessageBox.Show("Selezionare una licenza valida!", Properties.Resources.ErrorCaption);
                return;
            }

            if (siteCode.Text.Length == 0)
            {
                if (MessageBox.Show("Questa licenza verrà inserita come licenza HW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                    bSwLic = false;
                else if (MessageBox.Show("Questa licenza verrà dunque inserita come licenza SW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                    bSwLic = true;
                else
                    return;
            }
            else
            {
                if (MessageBox.Show("Questa licenza verrà inserita come licenza SW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                    return;
                bSwLic = true;
            }

            if (siteCode.Text.Length > 0)
            {
                try
                {
                    WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                    return;
                }
            }
            //else
            //    siteCode.Text = WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==");



            //Licenze
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            "SELECT * FROM [tbLicenze]",
                                            string.Format("[NumeroLicenza] = {0}", numSerial.Value), null, 
                                            "[IdLicenza]"));

            bool ret;
            bool bCreateLCode = false;
            if (dataView.Count > 0)
            {
                foreach (DataRowView rowcView in dataView)
                {
                    if ((int)rowcView["AnagrafeID"] != (customers.SelectedValue as Customer).ID)
                    {
                        if (MessageBox.Show("Licenza già inserita per un ALTRO cliente. Sei sicuro di voler modificare la licenza e aggiungere una riga allo storico DB?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                        return;
                    }
                    else if (MessageBox.Show("Sei sicuro di voler modificare la licenza e aggiungere una riga allo storico DB?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                    return;

                    if (bSwLic) //&& rowcView["SoftKey"] != null && rowcView["SoftKey"] != DBNull.Value)
                        bCreateLCode = true;
                    else
                        bCreateLCode = false;

                    break;
                }

                if(MoveLicenceToLicenceHistory(dataView))
                    MessageBox.Show("ATTENZIONE: Errore in fase di storicizzazione!", Properties.Resources.ErrorCaption);
            }
            else if (bSwLic)
                bCreateLCode = true;
            else
                bCreateLCode = false;

            if (InsertValueToDB(bCreateLCode, bSwLic))
                MessageBox.Show("ATTENZIONE: Errore in fase di inserimento!", Properties.Resources.ErrorCaption);
        }
        DataView CopyDataView(DataView dataView, bool allRecords = false)
        {
            DataView copyDataView = null;
            try
            {
                uint maxTransactions = MaxTransactionsBeforeCommit;
                if (maxTransactions > 0)
                {
                    copyDataView = new DataView(dataView.Table.Clone());
                    while (dataView.Count > 0)
                    {
                        if (!allRecords && copyDataView.Count >= maxTransactions)
                            break;
                        copyDataView.Table.ImportRow(dataView[0].Row);
                        dataView.Delete(0);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return copyDataView;
        }
        private bool InsertValueToDB(bool bCreateLCode, bool bSwLic)
        {
            try 
            {
                using (var idlDel = CreateLicDataLayer(ConnectionString))
                {
                    using (var ufw = new UnitOfWork(idlDel))
                    {
                        if (ufw == null)
                            return true;

                        ufw.LockingOption = LockingOption.None;

                        ufw.BeginTransaction();

                        try
                        {
                            tbLicenze _det = new tbLicenze(ufw)
                            {
                                AnagrafeID = (customers.SelectedValue as Customer).ID,
                                CodiceCliente = (customers.SelectedValue as Customer).Code,
                                NumeroLicenza = (int)numSerial.Value,
                                RemovedCode = numRemoved.Text,
                                SiteCode = siteCode.Text.Length == 0 && !bSwLic ? WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==") : siteCode.Text,
                                //SoftKey = bCreateLCode && siteCode.Text.Length > 0 ? CreateSWLic(false) : null,
                                FileKey = bCreateLCode && siteCode.Text.Length > 0 ? CreateSWLic(false) : null,
                                DataGenerazioneSoftKey = DateTime.Now,
                                Ordine = Order.Text,
                                Fattura = Bill.Text,
                                Note = logNote.Text,
                                IdProdotto = IDProdotto,
                                IdTipoLicenza = (licencetype.SelectedValue as LicenceType).ID
                            };
                            ufw.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            ufw.RollbackTransaction();
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                            return true;
                        }

                        ufw.DropIdentityMap();

                        var licenceToInsert = (from entry in new XPQuery<tbLicenze>(ufw)/*.AsParallel()*/
                                                where entry.NumeroLicenza == numSerial.Value
                                                select entry).Take(1).ToList();

                        if (licenceToInsert.Count > 0)
                        {
                            if(UpdateLicenceOptions(licenceToInsert[0].IdLicenza))
                                return true;
                        }
                    }
#if DEBUG
                    Debug.WriteLine(String.Format("Licence Logger - Deletion of records for Tag Name {0}", numSerial.Value));
#endif
                }
            }
            catch (Exception ex)
            {
                WriteLogMessage("Falied inserting entries", ConnectionString, numSerial.Value, ex.Message);
                return true;
            }
            return false;
        }

        private bool InsertDetailOptions(int idLicenza)
        {
            using (var idlDel = CreateLicDtDataLayer(ConnectionString))
            {
                using (var ufw = new UnitOfWork(idlDel))
                {
                    if (ufw == null)
                        return true;

                    ufw.LockingOption = LockingOption.None;
                    //insert tbLicenzeDettagli
                    //{boolean}
                    foreach (var item in retBOptions)
                    {
                        ufw.BeginTransaction();

                        try
                        {
                            tbLicenzeDettagli _det = new tbLicenzeDettagli(ufw) { IdLicenza = idLicenza, IdOpzione = item.ID, Valore = item.Value.ToString() };
                            ufw.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            ufw.RollbackTransaction();
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                            return true;
                        }

                        ufw.DropIdentityMap();
                    }
                    //{numeric}
                    foreach (var item in retIOptions)
                    {
                        ufw.BeginTransaction();

                        try
                        {
                            tbLicenzeDettagli _det = new tbLicenzeDettagli(ufw) { IdLicenza = idLicenza, IdOpzione = item.ID, Valore = item.Value.ToString() };
                            ufw.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            ufw.RollbackTransaction();
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                            return true;
                        }

                        ufw.DropIdentityMap();
                    }
                }
            }
            return false;
        }

        bool bError;
        bool MoveLicenceToLicenceHistory(DataView dataView)
        {
            try
            {


                using (var idl = CreateLicDataLayer(ConnectionString))
                {
                    using (var ufw = new UnitOfWork(idl))
                    {
                        if (ufw == null)
                            return true;

                        ufw.LockingOption = LockingOption.None;

                        var licenceToDelete = (from entry in new XPQuery<tbLicenze>(ufw)/*.AsParallel()*/
                                                where entry.NumeroLicenza == numSerial.Value
                                                select entry).Take(1).ToList();

                        if (licenceToDelete.Count > 0)
                        {
                            /*Inser row to tbLicenzeStorico*/
                            if (InsertToLicenceHistory(licenceToDelete[0]))
                                return true;

                            /*Inser row to tbLicenzeDettagliStorico and Delete row from tbLicenzeDettagli*/
                            if (MoveLicenceOptions(licenceToDelete[0]))
                                return true;
                        }
                    }
                    using (var writer = new DataWriter.DataSetWriter(_defaultDataProvider, _defaultConnectionString, MaxTransactionsBeforeCommit))
                    {
                        try
                        {
                            if (dataView.Count > 0)
                            {
                                dataView.Table.TableName = "tbLicenze";
                                writer.DeleteRows(dataView);
                                writer.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            writer.TryRollback();
                            return true;
                        }
                    }
#if DEBUG
                    Debug.WriteLine(String.Format("Licence Logger - Deletion of records for Tag Name {0}", numSerial.Value));
#endif
                }
            }
            catch (Exception ex)
            {
                WriteLogMessage("Falied deleting enities", ConnectionString, numSerial.Value, ex.Message);
                return true;
            }
            return false;
        }

        private bool InsertToLicenceHistory(tbLicenze tbLicenze)
        {
            using (var idl = CreateLicStDataLayer(ConnectionString))
            {
                using (var ufw = new UnitOfWork(idl))
                {
                    if (ufw == null)
                        return true;

                    ufw.LockingOption = LockingOption.None;
                    //insert tbLicenzeStorico
                    ufw.BeginTransaction();

                    try
                    {
                        tbLicenzeStorico _det = new tbLicenzeStorico(ufw) {
                            IdLicenza = tbLicenze.IdLicenza,
                            AnagrafeID=tbLicenze.AnagrafeID,
                            NumeroLicenza=tbLicenze.NumeroLicenza,
                            RemovedCode = tbLicenze.RemovedCode,
                            SiteCode=tbLicenze.SiteCode,
                            SoftKey=tbLicenze.SoftKey,
                            FileKey = tbLicenze.FileKey,
                            UserNameGenerazioneSoftKey = tbLicenze.UserNameGenerazioneSoftKey,
                            DataGenerazioneSoftKey=tbLicenze.DataGenerazioneSoftKey,
                            Ordine=tbLicenze.Ordine,
                            Fattura=tbLicenze.Fattura,
                            Note=tbLicenze.Note,
                            IdProdotto=tbLicenze.IdProdotto,
                            IdTipoLicenza=tbLicenze.IdTipoLicenza,
                            DataOraRichiestaPulizia=tbLicenze.DataOraRichiestaPulizia
                        };
                        ufw.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                        bError = true;
                        ufw.RollbackTransaction();
                        File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                        return true;
                    }

                    ufw.DropIdentityMap();
                }
            }
            return false;

        }

        private bool MoveLicenceOptions(tbLicenze tbLicenze)
        {
            using (var idl = CreateLicDtDataLayer(ConnectionString))
            {
                using (var ufw = new UnitOfWork(idl))
                {
                    if (ufw == null)
                        return true;
                    ufw.LockingOption = LockingOption.None;

                    var entriesToDelete = (from entry in new XPQuery<tbLicenzeDettagli>(ufw)/*.AsParallel()*/
                                           where entry.IdLicenza == tbLicenze.IdLicenza
                                           select entry).ToList();

                    if (entriesToDelete.Count > 0)
                    {
                        /*Inser row to tbLicenzeDettagliStorico*/
                        if (InsertHistoryDetails(entriesToDelete))
                            return true;

                        /*Delete row from tbLicenzeDettagli*/
                        try
                        {
                            using (var writer = new DataWriter.DataSetWriter(_defaultDataProvider, _defaultConnectionString, MaxTransactionsBeforeCommit))
                            {
                                try
                                {
                                    DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                                    _defaultConnectionString,
                                                                    "SELECT * FROM [tbLicenzeDettagli]",
                                                                    string.Format("[IdLicenza] = {0}", tbLicenze.IdLicenza), null,
                                                                    "[IdLicenza]"));
                                    if (dataView.Count > 0)
                                    {
                                        dataView.Table.TableName = "tbLicenzeDettagli";
                                        writer.DeleteRows(dataView);
                                        writer.Commit();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    writer.TryRollback();
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool InsertHistoryDetails(List<tbLicenzeDettagli> entriesToDelete)
        {
            using (var idl = CreateLicDtStDataLayer(ConnectionString))
            {
                using (var ufw = new UnitOfWork(idl))
                {
                    if(ufw == null)
                        return true;

                    ufw.LockingOption = LockingOption.None;
                    //insert tbLicenzeDettagliStorico
                    foreach (var item in entriesToDelete)
                    {
                        ufw.BeginTransaction();

                        try
                        {
                            tbLicenzeDettagliStorico _det = new tbLicenzeDettagliStorico(ufw) { IdLicenza = item.IdLicenza, IdOpzione = item.IdOpzione, Valore = item.Valore };
                            ufw.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            ufw.RollbackTransaction();
                            File.AppendAllText(AppLogPath, string.Format("{0}{1}", ex.ToString(), Environment.NewLine));
                            return true;
                        }

                        ufw.DropIdentityMap();
                    }

                 }
            }
            return false;
        }


        internal static string WriteLogMessage(String format, params object[] args)
        {
            var logMessage = String.Format("{0} - {1}", DateTime.Now, String.Format(format, args));
            Console.WriteLine(logMessage);
            return logMessage;
        }
        private void UpdateDB()
        {
            bool bSwLic;
            if (numSerial.Value == 0)
            {
                MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                return;
            }

            if (customers.SelectedIndex == -1)
            {
                MessageBox.Show("Selezionare il cliente per cui inserire la licenza!", Properties.Resources.ErrorCaption);
                return;
            }

            if (licencetype.SelectedIndex == -1)
            {
                MessageBox.Show("Selezionare una licenza valida!", Properties.Resources.ErrorCaption);
                return;
            }

            if (siteCode.Text.Length == 0)
            {
                if (MessageBox.Show("Questa licenza verrà inserita come licenza HW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                    bSwLic = false;
                else if (MessageBox.Show("Questa licenza verrà quindi inserita come licenza SW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                    bSwLic = true;
                else
                    return;
            }
            else
            {
                if (MessageBox.Show("Questa licenza verrà inserita come licenza SW. Procedere?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                return;
                bSwLic = true;
            }

            if (siteCode.Text.Length > 0)
            {
                try
                {
                    WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                    return;
                }
            }
            //else
            //    siteCode.Text = WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==");

            //Licenze
            DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                            _defaultConnectionString,
                                            "SELECT * FROM [tbLicenze]",
                                            string.Format("[NumeroLicenza] = {0}", numSerial.Value), null, 
                                            "[IdLicenza]"));
            bool ret = true;
            bool bCreateLCode = false;
            if (dataView.Count > 0)
            {
                foreach (DataRowView rowView in dataView)
                {
                    if ((int)rowView["AnagrafeID"] != (customers.SelectedValue as Customer).ID)
                    {
                        if (MessageBox.Show("Licenza già inserita per un ALTRO cliente. Sei sicuro di voler sovrascrivere la licenza esistente?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                            return;
                    }
                    else if (MessageBox.Show("Sei sicuro di voler sovrascrivere la licenza esistente?", Properties.Resources.ErrorCaption, MessageBoxButton.YesNo).Equals(MessageBoxResult.No))
                        return;

                    if (bSwLic) //&& rowView["SoftKey"] != null && rowView["SoftKey"] != DBNull.Value)
                        bCreateLCode = true;
                    else
                        bCreateLCode = false;

                    ret = UpdateDBValues(bCreateLCode, bSwLic);
                    break;
                }
            }
            else
            {
                if (bSwLic)
                    bCreateLCode = true;
                else
                    bCreateLCode = false;

                if (InsertValueToDB(bCreateLCode, bSwLic))
                    MessageBox.Show("ATTENZIONE: Errore in fase di aggiornamento!", Properties.Resources.ErrorCaption);
            }
        }
        private bool UpdateDBValues(bool bCreateLCode,bool bSwLic)
        {
            try 
            {
                using (var writer = new DataWriter.DataSetWriter(_defaultDataProvider, _defaultConnectionString, MaxTransactionsBeforeCommit))
                {
                    try
                    {
                        DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                        _defaultConnectionString,
                                                        "SELECT * FROM [tbLicenze]",
                                                        string.Format("[NumeroLicenza] = {0}", numSerial.Value), null,
                                                        "[IdLicenza]"));
                        if (dataView.Count > 0)
                        {
                            dataView.Table.TableName = "tbLicenze";
                            int IdLicenza = -1;
                            foreach (DataRowView rowView in dataView)
                            {
                                IdLicenza = (int)rowView["IdLicenza"];
                                rowView["AnagrafeID"] = (customers.SelectedValue as Customer).ID;
                                rowView["CodiceCliente"] = (customers.SelectedValue as Customer).Code;
                                rowView["NumeroLicenza"] = (int)numSerial.Value;
                                rowView["RemovedCode"] = numRemoved.Text;
                                rowView["SiteCode"] = siteCode.Text.Length == 0 && !bSwLic ? WPFUtilities.CryptString.CryptString.DecryptString("XpjpRD1hK2eMZvV4M+6H2A==") : siteCode.Text;
                                if (bCreateLCode && siteCode.Text.Length > 0)
                                    //rowView["SoftKey"] = CreateSWLic(false);
                                    rowView["FileKey"] = CreateSWLic(false);
                                rowView["DataGenerazioneSoftKey"] = DateTime.Now;
                                rowView["Ordine"] = Order.Text;
                                rowView["Fattura"] = Bill.Text;
                                rowView["Note"] = logNote.Text;
                                rowView["IdProdotto"] = IDProdotto;
                                rowView["IdTipoLicenza"] = (licencetype.SelectedValue as LicenceType).ID;
                                rowView["FileKey"] = null;
                                break;
                            }

                            var listColumns = new List<string>() {"IdLicenza" };

                            writer.UpdateRows(dataView,listColumns,true);
                            writer.Commit();

                            if (IdLicenza == -1 || UpdateLicenceOptions(IdLicenza))
                                return true;
                        }
                        else
                        {
                            if (InsertValueToDB(bCreateLCode, bSwLic))
                                return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        writer.TryRollback();
                        return true;
                    }
#if DEBUG
                    Debug.WriteLine(String.Format("Licence Logger - Deletion of records for Tag Name {0}", numSerial.Value));
#endif
                }
            }
            catch (Exception ex)
            {
                WriteLogMessage("Falied updating enities", ConnectionString, numSerial.Value, ex.Message);
                return true;
            }
            return false;
        }

        private bool UpdateLicenceOptions(int idLicenze)
        {
            using (var writer = new DataWriter.DataSetWriter(_defaultDataProvider, _defaultConnectionString, MaxTransactionsBeforeCommit))
            {
                try
                {
                    DataView dataView = new DataView(DataReader.DataReader.GetDataSetSqlData(_defaultDataProvider,
                                                    _defaultConnectionString,
                                                    "SELECT * FROM [tbLicenzeDettagli]",
                                                    string.Format("[IdLicenza] = {0}", idLicenze), null,
                                                    "[IdLicenza]"));
                    if (dataView.Count > 0)
                    {
                        dataView.Table.TableName = "tbLicenzeDettagli";

                        foreach (DataRowView rowView in dataView)
                        {
                            var idLicenza = (int)rowView["IdLicenza"];
                            var idOpzione = (int)rowView["IdOpzione"];
                            BoolOption bOption = (BoolOption)(from item in retBOptions where item.ID == idOpzione select item).FirstOrDefault();
                            IntOption iOption = (IntOption)(from item in retIOptions where item.ID == idOpzione select item).FirstOrDefault();
                            rowView["Valore"] = bOption != null ? bOption.Value : iOption!= null ? iOption.Value : rowView["Valore"];
                        }

                        var listColumns = (from c in dataView.Table.Columns.OfType<DataColumn>()
                                           where c.AutoIncrement == true
                                           select c.ColumnName).ToList();

                        writer.UpdateRows(dataView);
                        writer.Commit();
                    }
                    else
                    {
                        if (InsertDetailOptions(idLicenze))
                            return true;
                    }
                }
                catch (Exception ex)
                {
                    writer.TryRollback();
                    return true;
                }
            }
            return false;
        }

        private void ReadSGLock()
        {
            MSZDelRead.ProductId = uint.MaxValue;

            //read serial number
            uint serial = MSZDelRead.ReadMovSerialNumber();
            numSerial.Value = serial;

            uint[] Data = new uint[REGISTER_MAX_COUNT];
            Data = MSZDelRead.ReadData();
            if (Data == null)
                return;

            //Application type
            switch ((MSZ.MSZDelRead.ApplicationType)MSZDelRead.ProductId)
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
            var _pgrtemp = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));

            if (_pgrtemp.Equals(pgrCode))
                sglockCheck.Visibility = Visibility.Collapsed;
            else
            {
                sglockCheck.Visibility = Visibility.Visible;
                return;
            }

            //siteCode.Text = WPFUtilities.CryptString.CryptString.EncryptString(sc.Trim('\0'));
            siteCode.Text = string.Empty;

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
                    Unlimited.IsChecked = true;
                    dateTimePicker1.IsEnabled = false;
                }
                else
                {
                    Unlimited.IsChecked = false;
                    dateTimePicker1.IsEnabled = true;
                    dateTimePicker1.DateTime = new DateTime(btemp[2] + 2000, btemp[1], btemp[0], btemp[3], btemp[4], btemp[5]);
                }
            }
            catch(Exception ex)
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

            BoolOption _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_developer]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 1) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_runtime]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 2) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_server]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 4) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_datalogger]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 8) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_client]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 16) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_recipe]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 32) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_vbnet]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 64) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_scheduler]);
            if (_bret != null)
                _bret.Value = ((btemp[0] & 128) > 0);

            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_deploy]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 1) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_report]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 2) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_opcuaserver]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 4) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_3D]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 8) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_alarmstat]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 16) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_geolocal]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 32) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_dispatcher]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 64) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_networking]);
            if (_bret != null)
                _bret.Value = ((btemp[1] & 128) > 0);

            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_redundancy]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 1) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_debug]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 2) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_modbus]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 4) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_automation]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 8) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_telemetry]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 16) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_facilities]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 32) > 0);
            _bret = retBOptions.Find(opt => opt.MszParameter == MapToMszPar[_iot]);
            if (_bret != null)
                _bret.Value = ((btemp[2] & 64) > 0);

            //int options
            IntOption _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_servertag]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[10]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_clienttag]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[11]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_drivers]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[12]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_childs]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[13]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_webclient5]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[14]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_webclient]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[15]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_alarms]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[16]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_screens]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[17]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_net]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[18]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_proenergy]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[19]);
            _iret = retIOptions.Find(opt => opt.MszParameter == MapToMszPar[_prolean]);
            if (_iret != null && _iret.Value >= 0)
                _iret.Value = Convert.ToUInt32(Data[20]);

            boolOptionList.ItemsSource = null;
            intOptionList.ItemsSource = null;
            boolOptionList.ItemsSource = retBOptions;
            intOptionList.ItemsSource = retIOptions;
            customers.SelectedIndex = -1;
            licencetype.SelectedIndex = -1;
        }
        private void CreateHWUpdateFile()
        {
            sglockCheck.Visibility = Visibility.Collapsed;
            MovApp.IsChecked = true;
            //unlimited value is mandatory for SGLock 
            Unlimited.IsChecked = true;
            dateTimePicker1.IsEnabled = false;

            string pt = string.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), string.Format("{0}.key", numSerial.Value));

            if (numSerial.Value == 0)
            {
                MessageBox.Show("Inserisci un numero di serie valido!", Properties.Resources.ErrorCaption);
                return;
            }
            //if (siteCode.Text.Length == 0)
            //{
            //    MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
            //    return;
            //}

            try
            {
                WPFUtilities.CryptString.CryptString.DecryptString(siteCode.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Inserisci un Site Code valido!", Properties.Resources.ErrorCaption);
                return;
            }

            StringBuilder param = new StringBuilder();
            //ED ExpiringDate -> dd/mm/yyyy if Unlimited=NoDate
            DateTime aDay = dateTimePicker1.DateTime;
            DateTime aDate = new DateTime(aDay.Year, aDay.Month, aDay.Day, 23, 59, 59);
            param.Append(string.Format("ED={0};", aDate.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            //UN Unlimited -> 0,1
            param.Append(string.Format("UN={0};", (bool)Unlimited.IsChecked ? 1 : 0));

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
            param.Append(string.Format("NSR={0}", numSerial.Value));

            var pippo = new Craddle();
            string lic = pippo.Generate(
                (uint)((bool)MovBa.IsChecked ?
                    MSZ.MSZDelRead.ApplicationType.apMoviconBA :
                        ((bool)MovTrace.IsChecked ? MSZ.MSZDelRead.ApplicationType.apMovTrace : MSZ.MSZDelRead.ApplicationType.apMovicon)),

                            siteCode.Text,
                                param.ToString());
            //string lic = pippo.Generate((uint)(MSZ.MSZDelRead.ApplicationType.apMoviconBA),
            //                    siteCode.Text,
            //                    param.ToString());

            byte[] data = Encoding.ASCII.GetBytes(lic);

            File.WriteAllText(pt, lic);
            Process.Start("explorer.exe", string.Format("/select,{0}", pt));

        }

        private void Unlimited_Click(object sender, RoutedEventArgs e)
        {
            logNote.IsEnabled = dateTimePicker1.IsEnabled = !(bool)(sender as CheckBox).IsChecked;
            numSerial.Value = dateTimePicker1.IsEnabled ? 0 : numSerial.Value;
        }
        private void btn2_Click(object sender, RoutedEventArgs e)
        {
            ReadSWLic();
        }
        private void ReloadAndInitValues(object sender, RoutedEventArgs e)
        {
            InitValues();
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearValue();
            InitSerialNumber();
        }
        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            CreateSWLic();
        }

        private void btn3_Click(object sender, RoutedEventArgs e)
        {
            WriteSGLock();
        }
        private void btn4_Click(object sender, RoutedEventArgs e)
        {
            ReadSGLock();
        }
        private void btn5_Click(object sender, RoutedEventArgs e)
        {
            CreateHWUpdateFile();
        }

        private void ReadDB_Click(object sender, RoutedEventArgs e)
        {
            ReadDB();
        }
        private void btn11_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", string.Format("/open,{0}", LogPath));
        }
        private void AddToDB_Click(object sender, RoutedEventArgs e)
        {
            AddHistory();
        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintFile();
        }
        private void OverrideDB_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }

        private void btn13_Click_1(object sender, RoutedEventArgs e)
        {
            InitSerialNumber();
        }

        bool bDisposed;
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
        }

        private void btn16_Click(object sender, RoutedEventArgs e)
        {
            if(!WriteSGLock())
            {
                PrintFile();
                AddHistory();
            }
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (numRemoved.Text == WPFUtilities.CryptString.CryptString.EncryptString(numSerial.Value.ToString()))
                    MessageBox.Show("Code is Valid!!","Licence Manager",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                else
                    MessageBox.Show("Code is NOT Valid for the serial number inserted", "Licence Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
            }
        }

        private void scriviaggiorna_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(CreateSWLic(false) != null)
                {
                    AddHistory();
                    Process.Start("explorer.exe", string.Format("/select,{0}", FilePath));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex.ToString()));
            }
        }
    }
}
