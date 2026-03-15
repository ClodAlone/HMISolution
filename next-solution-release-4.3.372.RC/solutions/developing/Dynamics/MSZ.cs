using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.IO.Compression;
using System.Xml;
using System.IO;
using System.Text;
using Utilities;
using System.ServiceModel;
using MSZServiceCMS;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Newtonsoft.Json.Linq;
using System.Xml.XPath;
using MSZ.Interfaces;
using MSZ.Services;

namespace MSZ
{
    public sealed class MSZView
    {
        #region Declarations
        private static string _server { get { return WPFUtilities.CryptString.CryptString.DecryptString("aWWmz7U/Nodh7mSA9L8oAQ=="); } }//("SVR")
        private static string _runtime { get { return WPFUtilities.CryptString.CryptString.DecryptString("ABmVqlvmqLNT3RbPaCt1Pg=="); } }//("RT")
        private static string _developer { get { return WPFUtilities.CryptString.CryptString.DecryptString("2/gZ3e0cSqiFpdGw57W2ZA=="); } }//("DEV")
        private static string _client { get { return WPFUtilities.CryptString.CryptString.DecryptString("ZNsToNGejkH59OOzi7bWvQ=="); } }//("CMD")
        private static string _datalogger { get { return WPFUtilities.CryptString.CryptString.DecryptString("QfHPNIlk1a6nPnEyRK9clw=="); } }//("DLR")
        private static string _recipe { get { return WPFUtilities.CryptString.CryptString.DecryptString("nCEgMHYEH5RLOaN03LCbxw=="); } }//("RCP")
        private static string _vbnet { get { return WPFUtilities.CryptString.CryptString.DecryptString("/e40Vrioka3qo/RSOgI2xw=="); } }//("VB")
        private static string _scheduler { get { return WPFUtilities.CryptString.CryptString.DecryptString("TfIcH8BA94C2MbB6w+u4Mw=="); } }//("SCD")
        private static string _networking { get { return WPFUtilities.CryptString.CryptString.DecryptString("qr7ETdFumhQnxIJbEbhorQ=="); } }//("NTW")
        private static string _redundancy { get { return WPFUtilities.CryptString.CryptString.DecryptString("+TUE/Sg1qoePVZu7g+dR+w=="); } }//("RED")
        private static string _debug { get { return WPFUtilities.CryptString.CryptString.DecryptString("fug/a/XB+OiTnd/2ZdhCXA=="); } }//("DBG")
        private static string _modbus { get { return WPFUtilities.CryptString.CryptString.DecryptString("8td5jGWD4tzp6zylCEPuww=="); } }//("MDB")
        private static string _automation { get { return WPFUtilities.CryptString.CryptString.DecryptString("vSwyN4V9dnvKnDFD3OYujA=="); } }//("AUT")
        private static string _telemetry { get { return WPFUtilities.CryptString.CryptString.DecryptString("0PQBKWp51zv/Ir+CTa8jTg=="); } }//("TLM")
        private static string _facilities { get { return WPFUtilities.CryptString.CryptString.DecryptString("Yp4rzMtuSxNanPXI6JTivA=="); } }//("FCS")
        private static string _iot { get { return WPFUtilities.CryptString.CryptString.DecryptString("SSaQsA8U3VuElRmBmDg8GQ=="); } }//("IOT")
        private static string _linux { get { return WPFUtilities.CryptString.CryptString.DecryptString("qm0Yrk8EOrSn8UWbFEq19Q=="); } }//("LNX")
        private static string _geolocal { get { return WPFUtilities.CryptString.CryptString.DecryptString("xh0e5EYFBkZtn0unXtNJfA=="); } }//("GEO")
        private static string _3D { get { return WPFUtilities.CryptString.CryptString.DecryptString("NEcoOsNxAg4tjat/8T5zoQ=="); } }//("G3D")
        private static string _ARealty { get { return WPFUtilities.CryptString.CryptString.DecryptString("QIySLm54AfStlYVTfb9iWg=="); } }//("ART")
        private static string _array1 { get { return WPFUtilities.CryptString.CryptString.DecryptString("ukPkcOTPxNMjSedSM+K1TA=="); } }//("AR1")
        private static string _report { get { return WPFUtilities.CryptString.CryptString.DecryptString("lohgq2F1Shi/lvnweCixaA=="); } }//("REP")
        private static string _dispatcher { get { return WPFUtilities.CryptString.CryptString.DecryptString("iFUE/lJmjCniaH2/8irz5w=="); } }//("DIS")
        private static string _alarmstat { get { return WPFUtilities.CryptString.CryptString.DecryptString("eDJgTyZ8URP38n06+AK8ug=="); } }//("STA")
        private static string _opcuaserver { get { return WPFUtilities.CryptString.CryptString.DecryptString("2LEJER2ToEwTJrOoBYYDLg=="); } }//("OUAS")
        private static string _deploy { get { return WPFUtilities.CryptString.CryptString.DecryptString("F8EDBcvXLdyv/OpSrhoqcg=="); } }//("WDEP")
        private static string _webclient5 { get { return WPFUtilities.CryptString.CryptString.DecryptString("p6OOEyokimxEBwJapBYnaQ=="); } }//("WCL5")
        private static string _webclient { get { return WPFUtilities.CryptString.CryptString.DecryptString("GWRgSK00mEYcSGJIfVzStQ=="); } }//("WCL")
        private static string _servertag { get { return WPFUtilities.CryptString.CryptString.DecryptString("w89JD4DHxzgkFpU10NWGLA=="); } }//("STG")
        private static string _clienttag { get { return WPFUtilities.CryptString.CryptString.DecryptString("1uZeuk41g5tEN8PkGiCwLw=="); } }//("CTG")
        private static string _drivers { get { return WPFUtilities.CryptString.CryptString.DecryptString("PuXfSP24fhC4JL6M7LAQzA=="); } }//("DRV")
        private static string _childs { get { return WPFUtilities.CryptString.CryptString.DecryptString("FmzaxK+3RVr/LUPMmVHeCQ=="); } }//("CHLD")
        private static string _screens { get { return WPFUtilities.CryptString.CryptString.DecryptString("/H16Mdag/oIEZx3eDz8BEg=="); } }//("SCR")
        private static string _alarms { get { return WPFUtilities.CryptString.CryptString.DecryptString("I+TwLvoQ7TUbaQ+YyVetAw=="); } }//("ALR")
        private static string _net { get { return WPFUtilities.CryptString.CryptString.DecryptString("zxVCYC4E/39MftYBrBiV3Q=="); } }//("NET")
        private static string _proenergy { get { return WPFUtilities.CryptString.CryptString.DecryptString("gg9YQvhvx70kafcB0yaFmA=="); } }//("PEN")
        private static string _prolean { get { return WPFUtilities.CryptString.CryptString.DecryptString("b2eHWGOY6pOyUuDOqvHGtw=="); } }//("PLN")
        private static string _sinstance { get { return WPFUtilities.CryptString.CryptString.DecryptString("ojGsNaTsIoN+EtaYLi40Jw=="); } }//("SIN")
        private static string _little { get { return WPFUtilities.CryptString.CryptString.DecryptString("wstSI/P41LJeGb6158MEww=="); } }//("99")
        private static string _big { get { return WPFUtilities.CryptString.CryptString.DecryptString("mxPFclWvubfF/qDzqyi2zg=="); } }//("9999999")

        private static string _sn { get { return "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNGBzfZpWLG9DRUXidtfCTQ=="; } }///* ("SN") */
        private static string _sitecode { get { return "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxdwuNxi+4QGNeS1AygM9QffcVLoXjExXn7Gng7dCSU/c="; } }///* ("SiteCode") */
        private static string _expiringdate { get { return "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxmr1U5lcG+U7v4O5kYz+BNPv3GmMzYnscltzgUahUBm8="; } }///* ("ExpiringDate") */

        private static string pgrCode { get { return "aqyFuVmb6LP+jg3oTfy4Dhz2ys8ZxGGjtEDqhdp/mf8="; } } // ("MovNextPGRcodeXX")
        private static string _regRootCU { get { return "0YT4GVf3luebAUshVyhBDvJ0x4KRI/hkTny5jHlWRr0="; } } // ("HKEY_CURRENT_USER")
        private static string _regRootCR { get { return "cUp97j7nWfteed1IGu9UTXVbZDfMmS74VSRtALteW0g="; } } // ("HKEY_CLASSES_ROOT")
        private static string _regCU { get { return "0YT4GVf3luebAUshVyhBDv2qfAxXi4rDqd0Sc2ezoyDBEHy5KKGntPJ7AcYySYLeGZIHL5E84NR+5Wi7PhlWJroFnQLw0ha4eK2T4MOBUw5K/jAO7AJocQmgICARZKy+"; } } // ("HKEY_CURRENT_USER\SOFTWARE\Classes\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID")
        private static string _regCR { get { return "cUp97j7nWfteed1IGu9UTcwU6pn9EVapVLFUFOkVLuJE/OY7jWbsXZH2SnpgD26yWCL36R/GopiTbAypSfZyoRu19dAHAZNZz6mhTKpbPh0="; } } // ("HKEY_CLASSES_ROOT\CLSID\{305C71A1-B8B9-45E4-BA09-283C34F3F69E}\ProgID")
        private static string _modifier { get { return WPFUtilities.CryptString.CryptString.DecryptString("owvdR9QMHVimhOu883vokA=="); } } // ("removed: ")
        private static string _mshobj { get { return WPFUtilities.CryptString.CryptString.DecryptString("lwDzRg8f9fl/oC3D8JOghj/Eano0mIqWWs0UfphJMxM="); } } // ("memory sharedmap")

        private static SecurityMode netTcpSecurityMode = SecurityMode.None;

        private static string InternalCache { get; set; }
        private static string ServerNetName { get; set; }
        private static bool bSuspended { get; set; }
        private static bool bForciblySuspended { get; set; }
        private static bool bEnableRemoteRequest { get; set; }
        private static string RemovedLicenses { get; set; }

        public static string ServerNetPort { get; private set; }

        private static readonly Dictionary<string, string> cacheKeyValues = new Dictionary<string, string>();

        private static IMSZServiceCMS clientTcp;
        private static IMSZWServiceCMS clientWTcp;

        private const int DemoMaxIntValue = 100000;

        private static Object lockObject = new Object();
        private static Object locktcpObject = new Object();
        private static Object lockwtcpObject = new Object();

        private static TransReqType srqtype = TransReqType.Hardware;

        private static Thread WorkerThread;
        private static Thread WorkerThreadNetworkServer;

        private static bool bExitMode;

        private static System.Diagnostics.Process currentProcess;
        private static AutoResetEvent NewForceCheckLicense;
        private static AutoResetEvent NewForceReading;
        private static ManualResetEvent LicenseRead;
        
        private static String xamlsku;
        private static bool isDeamon;

        #endregion

        #region Properties

        private static SecurityMode NetTcpSecurityMode
        {
            get
            {
                return netTcpSecurityMode;
            }
            set
            {
                netTcpSecurityMode = value;
            }
        }
        private static long maxReceivedMessageSize = 524288; // 512 KB
        private static long MaxReceivedMessageSize
        {
            get
            {
                return maxReceivedMessageSize;
            }
        }
        private static long maxReceivedWMessageSize = 524288; // 512 KB
        private static long MaxReceivedWMessageSize
        {
            get
            {
                return maxReceivedWMessageSize;
            }
        }

        private static String netTcpServiceAddress = "net.tcp://{0}:{1}/MSZService";
        private static String NetTcpServiceAddress
        {
            get
            {
                return netTcpServiceAddress;
            }
        }

        private static String netTcpWServiceAddress = "http://{0}:{1}/MSZWService";
        private static String NetTcpWServiceAddress
        {
            get
            {
                return netTcpWServiceAddress;
            }
        }

#if !DEBUG
        private static String wHostName = "collect.progea.com";
#else
        private static String wHostName = "localhost";
#endif
        private static String WHostName
        {
            get
            {
                return wHostName;
            }
        }

        private static String wServerPort = "63917";
        private static String WServerPort
        {
            get
            {
                return wServerPort;
            }
        }
        private static String requestID = "{0}@{1}@{2}@{3}";
        private static String RequestID
        {
            get
            {
                return WPFUtilities.CryptString.CryptString.EncryptString(string.Format(requestID,
                System.Environment.MachineName,
                currentProcess.ProcessName,
                currentProcess.Id,
                currentProcess.SessionId));
            }
        }
        private static String requestID2 = "{0}@{1}";
        private static String RequestID2
        {
            get
            {
                try
                {
                    string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
#if !NET_STANDARD
                    if (MSZUtils.IsGoGlobalServerSession && currentProcess.SessionId == 0)
                        userName = String.Format(requestID2, userName, currentProcess.Id);
#endif
                    return WPFUtilities.CryptString.CryptString.EncryptString(string.Format(requestID2,
                    MSZ.MSZUtils.GetPrevious(),
                    userName));

                }
                catch (Exception)
                {
                    return WPFUtilities.CryptString.CryptString.EncryptString(MSZ.MSZUtils.GetPrevious());
                }
            }
        }

        private static TimeSpan timeout = TimeSpan.FromSeconds(5);
        private static TimeSpan Timeout
        {
            get
            {
                return timeout;
            }
        }

        private static TimeSpan wTimeout = TimeSpan.FromSeconds(15);
        private static TimeSpan WTimeout
        {
            get
            {
                return wTimeout;
            }
        }

        public static bool IsRemoved { get; private set; }
        
        static Tuple<string, string> customCompanyNameAndApplicationFolder;
#endregion

#region Constructors
        static MSZView()
        {
            // default settings
            _ScheduleTime = 60000;
            _WaitWTime = 600000;
            LicenseKeyService = new LicenseKeyService(_modifier);
            
            MSZDelRead.ProductId = (uint)MSZDelRead.ApplicationType.apMovicon;

            LoadXmlSkuFile();

            try
            {
                ServiceSettings settings = ReadSettings();
                if (settings.ServerName.Length > 0 && settings.PortNumber.Length > 0 && Convert.ToInt64(settings.PortNumber) > 0)
                {
                    ServerNetName = settings.ServerName;
                    ServerNetPort = settings.PortNumber;
                }
                else
                {
                    ServerNetName = Properties.Settings.Default.HostName;
                    ServerNetPort = Properties.Settings.Default.ServerPort;
                }
            }
            catch (Exception)
            {
                ServerNetName = Properties.Settings.Default.HostName;
                ServerNetPort = Properties.Settings.Default.ServerPort;
            }

            Init();

            AppDomain.CurrentDomain.ProcessExit += (o, e) =>
            {
                Terminate();
            };
        }
#endregion

#region Private Runtime Properties
#if NET_STANDARD
        static string filePath;
#endif
        public static string FilePath
        {
            get {
#if !NET_STANDARD
                return String.Format("{0}{4}{1}{4}{2}{4}{3}",
                                 Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                 customCompanyNameAndApplicationFolder?.Item1 ?? Properties.Settings.Default.CompanyName,
                                 customCompanyNameAndApplicationFolder?.Item2 ?? Properties.Settings.Default.CommonApplicationFolder,
                                 Properties.Settings.Default.LicenseDefaultFileName,
                                 Path.DirectorySeparatorChar);
#else
                if (filePath == null)
                {
                    var path = String.Format("{0}{3}{1}{3}{2}{3}",
                                 Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                 customCompanyNameAndApplicationFolder?.Item1 ?? Properties.Settings.Default.CompanyName,
                                 customCompanyNameAndApplicationFolder?.Item2 ?? Properties.Settings.Default.CommonApplicationFolder,
                                 Path.DirectorySeparatorChar);

                    var desktopLicenseFile = Path.Combine(path, Properties.Settings.Default.LicenseDefaultFileName);
                    var defaultLicenseFile = Path.Combine(path, Properties.Settings.Default.LicenseNetCoreFileName);
                    if (File.Exists(defaultLicenseFile))
                        filePath = defaultLicenseFile;
                    else if (System.IO.Directory.Exists(path))
                    {
                        try
                        {
                            var files = System.IO.Directory.GetFiles(path, String.Format("*{0}", Properties.Settings.Default.LicenseNetCoreExtension));
                            if (files.Length > 0)
                                filePath = files[0];
                            else if (File.Exists(desktopLicenseFile))
                                filePath = desktopLicenseFile;
                            else
                                return defaultLicenseFile;
                        }
                        catch
                        {
                            return defaultLicenseFile;
                        }
                    }
                    else
                        return defaultLicenseFile;
                }

                return filePath;
#endif
            }
        }

        private static string[] modulesNotAvailableInDemoMode = 
        { 
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ+LlYKBrULWLfRbHkuX7FA=="/* RT */, 
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxWuNSo2Dv1fX3PHz66oYOgA=="/* DEV */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxbsDOBmBOlIRVAC/8OSgHDQ=="/* NET */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNGBzfZpWLG9DRUXidtfCTQ=="/* SN */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxSRSfdPty4M/h8yKtCnKfpw=="/* CMD */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxY+APFmQPtOWSUaMJjYrwAg=="/* DBG */,
            "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx3ItKbxy9EMJoUOWvtWCzGQ=="/* AR1 */
        };

        private static string[] modulesTagNotAvailableInDemoMode = 
        {
            "ukPkcOTPxNMjSedSM+K1TA==",/* AR1 */
            //"gg9YQvhvx70kafcB0yaFmA==",/* PEN */
            //"b2eHWGOY6pOyUuDOqvHGtw=="/* PLN */
            //"ABmVqlvmqLNT3RbPaCt1Pg=="/* RT */, 
            //"2/gZ3e0cSqiFpdGw57W2ZA=="/* DEV */
        };

        private static Dictionary<string, string> modulesValueInDemoMode = new Dictionary<string, string>()
        {
            { "PuXfSP24fhC4JL6M7LAQzA==", "32" },//("DRV")
            {"KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx8EE1k87k7s2L4ud471OrRg==", "32" },//("DRV")
            { "FmzaxK+3RVr/LUPMmVHeCQ==", "16" },//("CHLD")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx47GuMgQbSRsJkASLqdvNn+GZ5/UKoeW6jV/sPhaKu1I=", "16" },//("CHLD")
            { "zxVCYC4E/39MftYBrBiV3Q==", "2" },//("NET")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxbsDOBmBOlIRVAC/8OSgHDQ==", "2" },//("NET")
            { "ojGsNaTsIoN+EtaYLi40Jw==", "2" },//("SIN")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxasqQE7XJtxBA3WdmRKB/PA==", "2" },//("SIN")
            { "/H16Mdag/oIEZx3eDz8BEg==", "8192" },//("SCR")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxJeSCTSIQynLFbrlTy2hpfA==", "8192" },//("SCR")
            { "I+TwLvoQ7TUbaQ+YyVetAw==", "16000"},//("ALR")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxNLOWKl78+jQgKGUlcim5yQ==", "16000"},//("ALR")
            { "gg9YQvhvx70kafcB0yaFmA==", "5" },//("PEN")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxDXOvujXgK6tvUKtbGQzR9A==", "5" },//("PEN")
            { "b2eHWGOY6pOyUuDOqvHGtw==", "5" },//("PLN")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxzKojW/Qlqpgmbof/sU8ezA==", "5" },//("PLN")
            { "p6OOEyokimxEBwJapBYnaQ==", "5" },//("WCL5")
            { "KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxQ0HNh4VycfQXLY5Z6lNe+QBGr0uKoS0SZW/4j3jqDSg=", "5" }
        };

        private static string NoDate = "NullDate";
        private static int MaxExpiringDays = 365;

        private static string _NetState;
        private static string NetState
        {
            get { return _NetState; }
        }

        private static uint _Serial;
        private static uint Serial
        {
            get { return _Serial; }
        }

        private static readonly int _ScheduleTime;
        private static int ScheduleTime
        {
            get { return _ScheduleTime; }
        }

        private static bool _Krytp;
        private static bool Krytp
        {
            get { return _Krytp; }
        }

        private static readonly int _WaitTime;
        private static int WaitTime
        {
            get { return _WaitTime; }
        }

        private static readonly int _WaitWTime;
        private static int WaitWTime
        {
            get { return _WaitWTime; }
        }

        private static string modulesTag
        {
            get { return WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="); }
        }
#endregion

#region Private Methods
        static ServiceSettings ReadSettings()
        {
            var commonFolder = String.Format("{0}{3}{1}{3}{2}",
                                     Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                     Properties.Settings.Default.CompanyName,
                                     Properties.Settings.Default.CommonApplicationFolder,
                                     Path.DirectorySeparatorChar);
            var settingsfile = System.IO.Path.Combine(commonFolder, "msz_netcontrols.config");

            ServiceSettings ss = new ServiceSettings();
            using (XmlReader reader = XmlReader.Create(settingsfile))
            {
                try
                {
                    var serializer = new DataContractSerializer(typeof(ServiceSettings));
                    ss = (serializer.ReadObject(reader) as string).FromXml<ServiceSettings>();
                }
                catch (Exception ex)
                {
                    reader.Close();
                }
            }

            return ss;
        }

        static void MSZNetClose()
        {
            try
            {
                lock (locktcpObject)
                {
                    if (clientTcp == null)
#if !NET_STANDARD
                        clientTcp = ChannelFactory<IMSZServiceCMS>.CreateChannel(
                            new NetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize },
                            new EndpointAddress(String.Format(NetTcpServiceAddress, ServerNetName, ServerNetPort)));
#else
                        clientTcp = new ChannelFactory<IMSZServiceCMS>(
                            new NetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize },
                            new EndpointAddress(String.Format(NetTcpServiceAddress, ServerNetName, ServerNetPort))).CreateChannel();
#endif
                }
                ((IContextChannel)clientTcp).OperationTimeout = Timeout;
                int mode;
                if (clientTcp != null)
                    clientTcp.Request(new MSZRequest() { RequestID = RequestID, RequestID2 = RequestID2, RequestType = "GggXJhcbIDBw8PDMdtXGPw==" });
            }
            catch (Exception ex)
            {
            }
            finally
            {
              CleanClientTcp(true);
            }
        }

        static string LocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return localIP;
        }

#if !NET_STANDARD
        static string GetPublicIP()
        {
            string a4 = string.Empty;
            try
            {
                string url = "http://checkip.dyndns.org";
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream()))
                    {
                        string response = sr.ReadToEnd().Trim();
                        string[] a = response.Split(':');
                        string a2 = a[1].Substring(1);
                        string[] a3 = a2.Split('<');
                        a4 = a3[0];
                    }
                }
            }
            catch
            { }

            return a4;
        }
#endif

        static string MSZNetRequest()
        {
            string ret = string.Empty;
            try
            {
                lock (locktcpObject)
                {
                    if (clientTcp == null)
#if !NET_STANDARD
                        clientTcp = ChannelFactory<IMSZServiceCMS>.CreateChannel(
                            new NetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize },
                            new EndpointAddress(String.Format(NetTcpServiceAddress, ServerNetName, ServerNetPort)));
#else
                        clientTcp = new ChannelFactory<IMSZServiceCMS>(
                            new NetTcpBinding(NetTcpSecurityMode) { MaxReceivedMessageSize = MaxReceivedMessageSize },
                            new EndpointAddress(String.Format(NetTcpServiceAddress, ServerNetName, ServerNetPort))).CreateChannel();
#endif
                }

                ((IContextChannel)clientTcp).OperationTimeout = Timeout;

            //string ret = string.Empty;
            //try
            //{
                if (clientTcp != null)
                    ret = clientTcp.Request(new MSZRequest() { RequestID = RequestID, RequestID2 = RequestID2, RequestType = "IiFxly+MSH6loa1Jm9a/pQ==" });
                if (!string.IsNullOrEmpty(ret))
                {
                    string _RequestID = ret.Substring(0,RequestID.Length);
                    if (_RequestID != null && _RequestID == RequestID)
                    {
                        ret = ret.Substring(RequestID.Length);

                        try
                        {
                            var netstate = clientTcp.Request(new MSZRequest() { RequestID = RequestID, RequestID2 = RequestID2, RequestType = "iZEsGneVntCcoR66tw80cQ==" });
                            if (netstate != null && netstate.Length > RequestID.Length && netstate.Contains(RequestID))
                                _NetState = netstate.Substring(RequestID.Length);

                            var serial = clientTcp.Request(new MSZRequest() { RequestID = RequestID, RequestID2 = RequestID2, RequestType = "iQeRnwG8TKuTsKMG7cEmTw==" });
                            if (serial != null && serial.Length > RequestID.Length && serial.Contains(RequestID))
                                _Serial = uint.Parse(WPFUtilities.CryptString.CryptString.DecryptString(serial.Substring(RequestID.Length)));
                        }
                        catch (Exception)
                        {
                            _Serial = 0;
                            srqtype = TransReqType.Hardware;
                            ret = string.Empty;
                        }
                    }
                    else
                    {
                        srqtype = TransReqType.Hardware;
                        ret = string.Empty;
                    }
                }
                else
                {
                    srqtype = TransReqType.Hardware;
                    ret = string.Empty;
                }
            }
            catch (Exception ex)
            {
                srqtype = TransReqType.Hardware;
                ret = string.Empty;
            }

            if (string.IsNullOrEmpty(ret))
            {
                srqtype = TransReqType.Hardware;
                CleanClientTcp();
            }

            return ret;
        }

        static string MSZWLFakeRequest()
        {
            string ret = null;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lock (lockwtcpObject)
                {
                    if (clientWTcp == null)
                        try
                        {
#if !NET_STANDARD
                            clientWTcp = ChannelFactory<IMSZWServiceCMS>.CreateChannel(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort)));
#else
                            clientWTcp = new ChannelFactory<IMSZWServiceCMS>(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort))).CreateChannel();
#endif
                        }
                        catch
                        { }
                }
                ((IContextChannel)clientWTcp).OperationTimeout = WTimeout;

                try
                {
                    var serial = GetSerial();
                    if(serial == "0")
                    {
                        CleanClientWTcp();
                        return null;
                    }

                    if (clientWTcp != null)
                        ret = clientWTcp.Request(new MSZWRequest()
                        {
                            RequestID = RequestID,
                            Request = WPFUtilities.CryptString.CryptString.EncryptString(serial),
                            RequestID2 = MSZ.MSZUtils.GetPrevious(),
                            RequestType = "aoFJf9sqeQYwONvVAvEfHg=="
                        });
                    if (!string.IsNullOrEmpty(ret))
                    {
                        string _RequestID = ret.Substring(0, RequestID.Length);
                        if (_RequestID != null && _RequestID == RequestID)
                        {
                            ret = ret.Substring(RequestID.Length);
                        }
                        else
                        {
                            ret = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CleanClientWTcp();
                    return null;
                }
            }
            return ret;
        }

        static string MSZWLRequest()
        {
            string ret = null;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lock (lockwtcpObject)
                {
                    if (clientWTcp == null)
                        try
                        {
#if !NET_STANDARD
                            clientWTcp = ChannelFactory<IMSZWServiceCMS>.CreateChannel(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort)));
#else
                            clientWTcp = new ChannelFactory<IMSZWServiceCMS>(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort))).CreateChannel();
#endif
                        }
                        catch
                        { }
                }
                ((IContextChannel)clientWTcp).OperationTimeout = WTimeout;

                string host = string.Empty;
                try
                {
                    host = WPFUtilities.CryptString.CryptString.EncryptString("local IP " + LocalIPAddress());
                }
                catch (Exception ex)
                {
                }

                try
                {
                    if (clientWTcp != null)
                        ret = clientWTcp.Request(new MSZWRequest() { RequestID = RequestID, Request = host,
                                                                     RequestID2 = MSZ.MSZUtils.GetPrevious(),
                                                                     RequestType = "CdQqW2FK/EEX0tL1T8Qfnw==" });
                    if (!string.IsNullOrEmpty(ret))
                    {
                        string _RequestID = ret.Substring(0, RequestID.Length);
                        if (_RequestID != null && _RequestID == RequestID)
                        {
                            ret = ret.Substring(RequestID.Length);
                        }
                        else
                        {
                            ret = string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CleanClientWTcp();
                    return null;
                }
            }
            return ret;
        }

        static void CleanClientTcp(bool forceclosing = false)
        {
            lock (locktcpObject)
            {
                if (clientTcp != null)
                {
                    var proxy = clientTcp as ICommunicationObject;

                    //Done with the service, let's close it.
                    try
                    {
                        if (forceclosing)
                        {
                            proxy.Abort();
                        }
                        else if (proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close();
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                    }

                    try
                    {
                        if (clientTcp is IDisposable)
                            (clientTcp as IDisposable).Dispose();
                    }
                    catch (Exception ex)
                    {
                    }
                    clientTcp = null;
                }
            }
        }

        static void CleanClientWTcp(bool forceclosing = false)
        {
            lock (lockwtcpObject)
            {
                if (clientWTcp != null)
                {
                    var proxy = clientWTcp as ICommunicationObject;

                    //Done with the service, let's close it.
                    try
                    {
                        if (forceclosing)
                        {
                            proxy.Abort();
                        }
                        else if (proxy.State != CommunicationState.Faulted)
                        {
                            proxy.Close();
                        }
                        else
                        {
                            proxy.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        proxy.Abort();
                    }

                    try
                    {
                        if (clientWTcp is IDisposable)
                            (clientWTcp as IDisposable).Dispose();
                    }
                    catch (Exception ex)
                    {
                    }
                    clientWTcp = null;
                }
            }
        }

        static void MSZFLNetClose()
        {
            string ret = string.Empty;
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lock (lockwtcpObject)
                {
                    if (clientWTcp == null)
                        try
                        {
#if !NET_STANDARD
                            clientWTcp = ChannelFactory<IMSZWServiceCMS>.CreateChannel(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort)));
#else
                            clientWTcp = new ChannelFactory<IMSZWServiceCMS>(
                                new BasicHttpBinding() { MaxReceivedMessageSize = MaxReceivedWMessageSize },
                                new EndpointAddress(String.Format(NetTcpWServiceAddress, WHostName, wServerPort))).CreateChannel();
#endif

                        }
                        catch
                        { }
                }
                ((IContextChannel)clientWTcp).OperationTimeout = WTimeout;

                try
                {
                    if (clientWTcp != null)
                        ret = clientWTcp.Request(new MSZWRequest()
                        {
                            RequestID = RequestID,
                            Request = WPFUtilities.CryptString.CryptString.EncryptString(GetSerial()),
                            RequestID2 = MSZ.MSZUtils.GetPrevious(),
                            RequestType = "GggXJhcbIDBw8PDMdtXGPw=="
                        });
                }
                catch (Exception ex)
                {
                    ret = ex.Message;
                }
                finally
                {
                    CleanClientWTcp(true);
                }
            }
        }

        static string ReadSGKey()
        {
            uint[] Data = new uint[MSZDelRead.MReg];
            byte[] _dataPair = new byte[MSZDelRead.MBReg];

            Data = MSZDelRead.ReadData();
            if (Data == null)
                return null;

            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", "Modules", "");
            xDoc.AppendChild(xRoot);

            //SiteCode
            XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", "SiteCode"));
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
            if (!_pgrtemp.Equals(pgrCode))
                return string.Empty;

            xElemL0.InnerText = sc.Trim('\0');
            xRoot.AppendChild(xElemL0);

            //ExpiringDate
            XmlElement xElemL10 = xDoc.CreateElement(string.Format("{0}", "ExpiringDate"));
            DateTime aDay = DateTime.UtcNow;
            //TimeSpan aDelta = new System.TimeSpan(int.Parse(ExpiringDelta), 0, 0, 0);
            //DateTime aDate = aDay.Add(aDelta);
            k = -1;
            btemp[++k] = (byte)Data[4];
            btemp[++k] = (byte)(Data[4] >> 8);
            btemp[++k] = (byte)(Data[4] >> 16);
            btemp[++k] = (byte)(Data[4] >> 24);
            btemp[++k] = (byte)Data[5];
            btemp[++k] = (byte)(Data[5] >> 8);
            btemp[++k] = (byte)(Data[5] >> 16);
            btemp[++k] = (byte)(Data[5] >> 24);
            if (btemp[2] == 0 &&
                   btemp[1] == 0 &&
                   btemp[0] == 0 &&
                   btemp[3] == 0 &&
                   btemp[4] == 0 &&
                   btemp[5] == 0)
            {
                xElemL10.InnerText = NoDate;
            }
            else
            {
                try
                {
                    xElemL10.InnerText = (new DateTime(btemp[2] + 2000, btemp[1], btemp[0], btemp[3], btemp[4], btemp[5])).ToString();
                }
                catch
                {
                }
            }

            xRoot.AppendChild(xElemL10);
            //ActivationDate
            //XmlElement xElemL10 = xDoc.CreateElement(string.Format("{0}", ActivationMarkUp));
            //xRoot.AppendChild(xElemL10);

            //boolean options
            k = -1;
            btemp[++k] = (byte)Data[8];
            btemp[++k] = (byte)(Data[8] >> 8);
            btemp[++k] = (byte)(Data[8] >> 16);
            btemp[++k] = (byte)(Data[8] >> 24);
            btemp[++k] = (byte)Data[9];
            btemp[++k] = (byte)(Data[9] >> 8);
            btemp[++k] = (byte)(Data[9] >> 16);
            btemp[++k] = (byte)(Data[9] >> 24);
            if (((btemp[0] & 1) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _developer));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 2) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _runtime));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 4) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _server));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 8) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _datalogger));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 16) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _client));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 32) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _recipe));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 64) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _vbnet));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[0] & 128) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _scheduler));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 1) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _deploy));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 2) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _report));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 4) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _opcuaserver));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 8) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _3D));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 16) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarmstat));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 32) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _geolocal));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 64) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _dispatcher));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[1] & 128) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _networking));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 1) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _redundancy));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[2] & 2) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _debug));
                xRoot.AppendChild(xElemL10);
            }
            if (((btemp[2] & 4) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _modbus));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 8) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _automation));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 16) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _telemetry));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 32) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _facilities));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 64) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _iot));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[2] & 128) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _linux));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[3] & 1) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _ARealty));
                xRoot.AppendChild(xElemL10);
            }

            if (((btemp[3] & 2) > 0))
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _array1));
                xRoot.AppendChild(xElemL10);
            }

            //numeric options
            if (Data[10] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _servertag)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[10]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[11] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _clienttag)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[11]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[12] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _drivers)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[12]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[13] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _childs)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[13]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[14] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient5)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[14]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[15] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _webclient)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[15]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[16] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _alarms)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[16]).ToString();
                xRoot.AppendChild(xElemL10);
            }
            if (Data[17] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _screens)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[17]).ToString();
                xRoot.AppendChild(xElemL10);
            }

            if (Data[18] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _net)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[18]).ToString();
                xRoot.AppendChild(xElemL10);
            }

            if (Data[19] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _proenergy)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[19]).ToString();
                xRoot.AppendChild(xElemL10);
            }

            if (Data[20] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _prolean)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[20]).ToString();
                xRoot.AppendChild(xElemL10);
            }

            if (Data[21] > 0)
            {
                xElemL10 = xDoc.CreateElement(string.Format("{0}", _sinstance)); // int
                xElemL10.InnerText = Convert.ToUInt32(Data[21]).ToString();
                xRoot.AppendChild(xElemL10);
            }

            return xDoc.InnerXml;
        }

        static long GetInternalRegister(string module, string xamlkey)
        {
            try
            {
                if (!String.IsNullOrEmpty(xamlkey))
                {
                    var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xamlkey, modulesTag, true, false);
                    if (keyexpandolist.Count() != 0)
                    {
                        var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                        foreach (var element in keyexpandolist)
                        {
                            var dictionary = element as IDictionary<string, object>;
                            if (dictionary.ContainsKey(xamlModule))
                                return long.Parse(string.Format("{0}", dictionary[xamlModule]));
                        }
                    }
                }

            }
            catch
            { }

            return -1;
        }

        static bool GetInternalOption(string module, string xamlkey)
        {
            try
            {
                if (!String.IsNullOrEmpty(xamlkey))
                {
                    var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xamlkey, modulesTag, true, false);
                    if (keyexpandolist.Count() != 0)
                    {
                        var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                        foreach (var element in keyexpandolist)
                        {
                            var dictionary = element as IDictionary<string, object>;
                            if (dictionary.ContainsKey(xamlModule))
                                return true;
                        }
                    }
                }

            }
            catch
            { }

            return false;
        }

        static List<string> GetKeyCodes()
        {
            var ret = new List<string>();

            try
            {
                if (!string.IsNullOrWhiteSpace(RemovedLicenses))
                    ret.AddRange(RemovedLicenses.Split('@'));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    ret = LicenseKeyService.ReadAll(ret);
                }
                else
                {
                    using (var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                    {
                        string path = WPFUtilities.CryptString.CryptString.DecryptString(_regCR);
                        string root = WPFUtilities.CryptString.CryptString.DecryptString(_regRootCR);
                        path = path.Substring(root.Length + 1); // remove "HKEY_CLASSES_ROOT\" part.
                        using (var key = hklm.OpenSubKey(path))
                        {
                            if (key != null)
                            {
                                key.GetValueNames().ToList().ForEach((value) =>
                                {
                                    var serial = WPFUtilities.CryptString.CryptString.DecryptString(value);
                                    if (serial.StartsWith(_modifier))
                                        serial = serial.Substring(_modifier.Length);
                                    if (!ret.Contains(serial))
                                        ret.Add(serial);
                                });
                            }
                        }
                    }

                    using (var hklm = Microsoft.Win32.RegistryKey.OpenBaseKey(RegistryHive.Users, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
                    {
                        string path = WPFUtilities.CryptString.CryptString.DecryptString(_regCU);
                        string root = WPFUtilities.CryptString.CryptString.DecryptString(_regRootCU);
                        path = path.Substring(root.Length + 1); // remove "HKEY_CURRENT_USER\" part.
                        using (var key = hklm.OpenSubKey(path))
                        {
                            if (key != null)
                            {
                                key.GetValueNames().ToList().ForEach((value) =>
                                {
                                    var serial = WPFUtilities.CryptString.CryptString.DecryptString(value);
                                    if (serial.StartsWith(_modifier))
                                        serial = serial.Substring(_modifier.Length);
                                    if (!ret.Contains(serial))
                                        ret.Add(serial);
                                });
                            }
                        }
                    }
                }
            }
            catch
            { }

            return ret;
        }


        static void UpdateRemoved(string code)
        {
            try
            {
                var value = WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}{1}", _modifier, code));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    LicenseKeyService.Write(code);
                }
                else
                {
                    String path = "RegistryWriter.exe";
                    Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                var arguments = String.Format("/E /C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1, WPFUtilities.CryptString.CryptString.DecryptString(_regCU), value, "",
                        Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

                    using (var winProcess = new System.Diagnostics.Process())
                    {
                        winProcess.StartInfo.FileName = path;
                        winProcess.StartInfo.Arguments = WPFUtilities.CryptString.CryptString.EncryptString(arguments);
                        winProcess.StartInfo.UseShellExecute = false;
                        winProcess.StartInfo.CreateNoWindow = true;
                        winProcess.Start();
                    }
                }
            }
            catch
            { }
        }

        static void CleanRemoved()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    LicenseKeyService.CleanAll();
                }
                else
                {
                    String path = "RegistryWriter.exe";
                    Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                var arguments = String.Format("/E /C\"{0}\" /K\"{1}\"", 4, WPFUtilities.CryptString.CryptString.DecryptString(_regCU));

                    using (var winProcess = new System.Diagnostics.Process())
                    {
                        winProcess.StartInfo.FileName = path;
                        winProcess.StartInfo.Arguments = WPFUtilities.CryptString.CryptString.EncryptString(arguments);
                        winProcess.StartInfo.UseShellExecute = false;
                        winProcess.StartInfo.CreateNoWindow = true;
                        winProcess.Start();
                    }
                }
            }
            catch
            { }
        }

        static bool Startup()
        {
            lock (lockThreadObject)
            {
                if (bExitMode)
                    return false;

                if (NewForceCheckLicense == null)
                    NewForceCheckLicense = new AutoResetEvent(false);

                if (LicenseRead == null)
                    LicenseRead = new ManualResetEvent(false);
                else
                    LicenseRead.Reset();

                if (NewForceReading == null)
                    NewForceReading = new AutoResetEvent(false);

                if (WorkerThread == null)
                {
                    WorkerThread = new Thread(WorkingThread) { IsBackground = true };
                    WorkerThread.Start(0);
                }

                return WorkerThread != null;
            }
        }

        static bool Init(bool krytp = false)
        {
            currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            _Krytp = krytp;
            return Startup();
        }

        static void Terminate()
        {
            var threads = new List<Thread>();
            lock (lockThreadObject)
            {
                bExitMode = true;

                if (WorkerThreadNetworkServer != null)
                    threads.Add(WorkerThreadNetworkServer);
                if (WorkerThread != null)
                    threads.Add(WorkerThread);

                if (NewForceCheckLicense != null)
                    NewForceCheckLicense.Set();

                if (NewForceReading != null)
                    NewForceReading.Set();
            }

            foreach (var thread in threads)
                thread.Join();

            lock (lockThreadObject)
            {
                if (NewForceCheckLicense != null)
                    NewForceCheckLicense.Dispose();

                if (NewForceReading != null)
                    NewForceReading.Dispose();

                if (LicenseRead != null)
                    LicenseRead.Dispose();
            }

            CloseTcp(true);
        }
#endregion

#region Public Methods
        public static bool ThreadRunning()
        {
            return WorkerThread != null;
        }

        public static void Read()
        {
            lock (lockThreadObject)
            {
                if (NewForceReading != null)
                    NewForceReading.Set();
            }
        }

        public static void Remove(out string code)
        {
            try
            {
                code = string.Empty;
                var _code = GetModule(_sn);
                var value = WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}{1}", _modifier, _code));

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    code = LicenseKeyService.Remove(value);
                }
                else
                {
                    String path = "RegistryWriter.exe";
                    Assembly callingMainAssembly = Assembly.GetEntryAssembly();
                    if (callingMainAssembly != null)
                    path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(callingMainAssembly.Location), path);

                var arguments = String.Format("/E /C\"{0}\" /K\"{1}\" /N\"{2}\" /V\"{3}\" /T\"{4}\"", 1, WPFUtilities.CryptString.CryptString.DecryptString(_regCR), value, "", 
                        Enum.GetName(typeof(RegistryValueKind), RegistryValueKind.String));

                    RunElevated.Run(path, WPFUtilities.CryptString.CryptString.EncryptString(arguments));
                    code = WPFUtilities.CryptString.CryptString.EncryptString(string.Format("{0}", _code));
                }
            }
            catch
            {
                code = null;
            }
        }

        public static void SetLicensePath(Tuple<string, string> licenseFolders)
        {
            if (licenseFolders != null)
                customCompanyNameAndApplicationFolder = new Tuple<string, string>(licenseFolders.Item1, licenseFolders.Item2);
            else
                customCompanyNameAndApplicationFolder = null;
            _Serial = 0;
            isDeamon = false;
            srqtype = TransReqType.Hardware;
#if NET_STANDARD
            filePath = null;
#endif
            LoadXmlSkuFile();
        }

        public static void InitRemoteRequest(int i/* don't remove */)
        {
            bEnableRemoteRequest = true;

#if !DEBUG
            UFSolutionNext.ArrayIndex.processed += (obo, eve) =>
            {
                if (obo == null)
                {
                    System.Environment.Exit(-10);
                }
            };
            UFSolutionNext.ArrayIndex.VerifyArrayIndex(0x10);
            if (xamlsku != null)
                UFSolutionNext.ArrayIndex.VerifyArrayIndex(0x20);
#endif

            lock (lockThreadObject)
            {
                if (NewForceCheckLicense != null)
                    NewForceCheckLicense.Set();
            }
        }

        public static bool CheckState(bool force = false, bool sync = false)
        {
            //if (force || !StateLicense.HasValue)
            if (bSuspended)
                return true;

            lock (lockThreadObject)
            {
                if (force && LicenseRead != null && NewForceReading != null)
                {
                    LicenseRead.Reset();
                    NewForceReading.Set();
                }

                if (LicenseRead != null)
                { 
                    var read = !sync ? LicenseRead.WaitOne(10000) : LicenseRead.WaitOne();
                    if (!read)
                        LicenseRead.Set();
                }
            }

            if (StateLicense != null && StateLicense.HasValue)
                return StateLicense.Value;
            else
                return true;
        }

        public static int GetDemoMaxIntValue() { return DemoMaxIntValue; }

        /// <summary>
        /// Get numeric module value.
        /// </summary>
        /// <param name="module">encryptyed module name in xml format.</param>
        /// <param name="xamlkey">optional encryptyed key data where search the module name.</param>
        /// <returns>The module value inside the key data (-1 if missing).</returns>
        public static long GetModule(string module, string xamlkey = null)
        {
            if (!string.IsNullOrEmpty(xamlkey))
                return GetInternalRegister(module, WPFUtilities.CryptString.CryptString.DecryptString(xamlkey));

            var bNotFoundLicense = CheckState();
            if (bNotFoundLicense && !modulesNotAvailableInDemoMode.Contains(module))
            {
                if (modulesValueInDemoMode.ContainsKey(module))
                    return long.Parse(modulesValueInDemoMode[module]);
                else
                    return DemoMaxIntValue;
            }
            else if (bNotFoundLicense)
                return 0;

            lock (lockObject)
            {
                if (cacheKeyValues.ContainsKey(module))
                    return long.Parse(cacheKeyValues[module]);
                else
                    return -1;
            }
        }

        /// <summary>
        /// Get boolean module value.
        /// </summary>
        /// <param name="module">encryptyed module name in xml format.</param>
        /// <param name="xamlkey">optional encryptyed key data where search the module name.</param>
        /// <returns>True if the module name is inside the key data (false if missing).</returns>
        public static bool GetModules(string module, string xamlkey = null)
        {
            if (!string.IsNullOrEmpty(xamlkey))
                return GetInternalOption(module, WPFUtilities.CryptString.CryptString.DecryptString(xamlkey));

            var bNotFoundLicense = CheckState();
            if (bNotFoundLicense && !modulesNotAvailableInDemoMode.Contains(module))
                return true;
            else if (bNotFoundLicense)
                return false;

            lock (lockObject)
            {
                return cacheKeyValues.ContainsKey(module);
            }
        }

        /// <summary>
        /// check modules value
        /// </summary>
        /// <param name="modules">modules required</param>
        /// <returns>list of true/false values if the requested modules were read or not</returns>
        public static bool[] GetMultiModules(string modules)
        {
            var ret = new List<bool>();
            var xamlKey = string.Empty;

            if (CheckState())
            {
                try
                {
                    var xamlModules = WPFUtilities.CryptString.CryptString.DecryptString(modules);
                    var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModules, modulesTag, true, false);
                    if (modulesexpandoList.Count() != 0)
                    {
                        modulesexpandoList.ToList().ForEach(element =>
                        {
                            var dictionary = element as IDictionary<string, object>;
                            dictionary.ToList().ForEach(d =>
                            {
                                if (!modulesTagNotAvailableInDemoMode.Contains(WPFUtilities.CryptString.CryptString.EncryptString(d.Key)))
                                    ret.Add(true);
                                else if (CheckState())
                                    ret.Add(false);
                            });

                        });
                    }
                }
                catch
                { }
            }
            else
            {
                try
                {
                    string internalCache = null;
                    lock (lockObject)
                    {
                        internalCache = InternalCache;
                    }

                    if (!String.IsNullOrEmpty(internalCache))
                    {
                        var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(internalCache, modulesTag, true, false);
                        if (keyexpandolist.Count() != 0)
                        {
                            var xamlModules = WPFUtilities.CryptString.CryptString.DecryptString(modules);
                            var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModules, modulesTag, true, false);
                            if (modulesexpandoList.Count() != 0)
                            {
                                modulesexpandoList.ToList().ForEach(element =>
                                {
                                    var dictionary = element as IDictionary<string, object>;
                                    dictionary.ToList().ForEach(d =>
                                    {
                                        keyexpandolist.ToList().ForEach(e =>
                                        {
                                            var regkeydictionary = e as IDictionary<string, object>;
                                            if (regkeydictionary.Contains(new KeyValuePair<string, object>(d.Key, d.Value)))
                                                ret.Add(true);
                                            else
                                                ret.Add(false);
                                        });
                                    });

                                });
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var xamlModules = WPFUtilities.CryptString.CryptString.DecryptString(modules);
                            var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModules, modulesTag, true, false);
                            if (modulesexpandoList.Count() != 0)
                            {
                                modulesexpandoList.ToList().ForEach(element =>
                                {
                                    var dictionary = element as IDictionary<string, object>;
                                    dictionary.ToList().ForEach(d =>
                                    {
                                        if (!modulesTagNotAvailableInDemoMode.Contains(WPFUtilities.CryptString.CryptString.EncryptString(d.Key)))
                                            ret.Add(true);
                                        else if (CheckState())
                                            ret.Add(false);
                                    });

                                });
                            }
                            //CheckState(true);
                        }
                        catch
                        { }
                    }
                }
                catch
                { }
            }


            return ret.ToArray();
        }

        /// <summary>
        /// get module value
        /// </summary>
        /// <param name="module">module required</param>
        /// <returns>list of module values</returns>
        public static string[] GetMultiModule(string module)
        {
            var ret = new List<string>();
            var xamlKey = string.Empty;
            var value = string.Empty;

            if (CheckState())
            {
                try
                {
                    var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                    var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModule, modulesTag, true, false);
                    if (modulesexpandoList.Count() != 0)
                    {
                        modulesexpandoList.ToList().ForEach(element =>
                        {
                            var dictionary = element as IDictionary<string, object>;
                            dictionary.ToList().ForEach(d =>
                            {
                                var key = WPFUtilities.CryptString.CryptString.EncryptString(d.Key);
                                if (!modulesTagNotAvailableInDemoMode.Contains(key))
                                {
                                    if (modulesValueInDemoMode.ContainsKey(key))
                                        ret.Add(modulesValueInDemoMode[key]);
                                    else
                                        ret.Add(DemoMaxIntValue.ToString());
                                }
                                else
                                    ret.Add("0");
                            });

                        });
                    }
                }
                catch
                { }
            }
            else
            {
                try
                {
                    string internalCache = null;
                    lock (lockObject)
                    {
                        internalCache = InternalCache;
                    }

                    if (!String.IsNullOrEmpty(internalCache))
                    {
                        var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(internalCache, modulesTag, true, false);
                        if (keyexpandolist.Count() != 0)
                        {
                            var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                            var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModule, modulesTag, true, false);
                            if (modulesexpandoList.Count() != 0)
                            {
                                modulesexpandoList.ToList().ForEach(element =>
                                {
                                    var dictionary = element as IDictionary<string, object>;
                                    dictionary.ToList().ForEach(d =>
                                    {
                                        keyexpandolist.ToList().ForEach(e =>
                                        {
                                            var regkeydictionary = e as IDictionary<string, object>;
                                            if (regkeydictionary.ContainsKey(d.Key))
                                                ret.Add((string)regkeydictionary[d.Key]);
                                            else
                                                ret.Add("0");
                                        });
                                    });

                                });
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                            var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModule, modulesTag, true, false);
                            if (modulesexpandoList.Count() != 0)
                            {
                                modulesexpandoList.ToList().ForEach(element =>
                                {
                                    var dictionary = element as IDictionary<string, object>;
                                    dictionary.ToList().ForEach(d =>
                                    {
                                        var key = WPFUtilities.CryptString.CryptString.EncryptString(d.Key);
                                        if (!modulesTagNotAvailableInDemoMode.Contains(key))
                                        {
                                            if (modulesValueInDemoMode.ContainsKey(key))
                                                ret.Add(modulesValueInDemoMode[key]);
                                            else
                                                ret.Add(DemoMaxIntValue.ToString());
                                        }
                                        else
                                            ret.Add("0");
                                    });

                                });
                            }
                        }
                        catch
                        { }
                    }

                }
                catch
                { }
            }

            return ret.ToArray();
        }

        public static string GetNetState()
        {
            if (!string.IsNullOrEmpty(NetState))
                return NetState;
            
            return string.Empty;
        }

        public static string GetKeyData()
        {
            lock (lockObject)
            {
                return WPFUtilities.CryptString.CryptString.EncryptString(InternalCache);
            }
        }

        public static string GetSiteKey()
        {
            return ReadModule(_sitecode);
        }

        public static string GetSerial()
        {

            if (srqtype == TransReqType.Hardware)
            {
                return String.Format("{0}", Serial);
            }
            else
            {
                var serial = ReadModule(_sn);
                if (!String.IsNullOrEmpty(serial))
                    return serial;
            }

            return "0";
        }
        
        public static string GetSerialInfo(bool force = false, bool shortVersion = false)
        {
            string result;
            var isInFault = CheckState(force);
            if (isInFault)
            {
                if(shortVersion)
                {
                    if (bForciblySuspended)
                        result = Properties.Settings.Default.LicenseForciblySuspended;
                    else if (bSuspended)
                        result = Properties.Settings.Default.LicenseSuspended;
                    else if (IsRemoved)
                        result = Properties.Settings.Default.LicenseNotValid; 
                    else
                        result = Properties.Settings.Default.LicenseNotPresent; 
                }
                else
                {
                    if (bForciblySuspended)
                        result = Properties.Resources.DongleForciblySuspendedInfo;
                    else if (bSuspended)
                        result = Properties.Resources.DongleSuspendedInfo;
                    else if (IsRemoved)
                        result = Properties.Resources.DongleRemoved;
                    else
                        result = Properties.Resources.DongleNotFound;
                }
            }
            else
            {
                var serial = GetSerial();
                var netstate = WPFUtilities.CryptString.CryptString.DecryptString(MSZ.MSZView.GetNetState());
                if (!string.IsNullOrEmpty(serial) && !serial.Equals("0"))
                {
                    result = serial;

                    if (!shortVersion)
                    {
                        if (!string.IsNullOrEmpty(netstate))
                            result = Properties.Resources.DongleNetFound;
                        else
                        {
                            string type = srqtype == TransReqType.Hardware ? Properties.Resources.HWDongle : Properties.Resources.SWDongle;
                            result = String.Format(Properties.Resources.DongleFound, type);
                        }

                        var _serial = String.Format(Properties.Resources.DongleSerialNumber, serial);
                        var expiringDate = GetExpiringDate();
                        result = $"{result}: {_serial}";
                        if (expiringDate != DateTime.MinValue)
                        {
                            var _date = String.Format(Properties.Resources.DongleExpiringDate, expiringDate.ToShortDateString());
                            result = $"{result}: {_serial} {_date}";
                        }
                    }
                }
                else
                {
                    if (!shortVersion)
                    {
                        if (!string.IsNullOrEmpty(netstate))
                            result = Properties.Resources.DongleNetFoundNoSerial;
                        else
                            result = Properties.Resources.DongleFoundNoSerial;

                        var expiringDate = GetExpiringDate();
                        if (expiringDate != DateTime.MinValue)
                        {
                            var _date = String.Format(Properties.Resources.DongleExpiringDate, expiringDate.ToShortDateString());
                            result = $"{result}: {_date}";
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(netstate))
                            result = $"{Properties.Settings.Default.LicenseNETTemporary}";
                        else
                            result = $"{Properties.Settings.Default.LicenseTemporary}"; 
                    }
                }
            }

            return result;
        }

        public static DateTime GetExpiringDate()
        {
            var expiringDate = DateTime.MinValue;
            var date = MSZ.MSZView.ReadModule(_expiringdate);
            if (date != null && !date.Equals(NoDate) && 
                DateTime.TryParse(date, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out expiringDate))
                return expiringDate;

            return DateTime.MinValue;
        }

        public static bool CheckLocal(bool force = false)
        {
            return !CheckState(force) && File.Exists(FilePath) && srqtype == TransReqType.Software;
        }

        public static void Suspend()
        {
            bSuspended = true;
        }
        public static bool IsSuspended()
        {
            return bSuspended;
        }
        public static bool IsForciblySuspended()
        {
            return bForciblySuspended;
        }
#endregion

#region Public Events
        public static event EventHandler<MSZEventArgs> KeyChangeEvent;
        private static void OnKeyChangingEvent(MSZEventArgs e)
        {
            EventHandler<MSZEventArgs> temp = KeyChangeEvent;
            if (temp != null)
                temp(null, e);
        }
#endregion

#region WorkerThread

        static object lockThreadObject = new object();
        static DateTime LastScheduleTime = new DateTime();
        static DateTime FirstScheduleTime = new DateTime();
        static bool? StateLicense;
        static void WorkingThread(object data)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            FirstScheduleTime = DateTime.UtcNow;
            ForceReading();
            StartOrStopRemoteSoftwareLicenseCheck();

            while (!bExitMode)
            {
                NewForceReading.WaitOne(ScheduleTime);
                if (bExitMode)
                    break;

                LastScheduleTime = DateTime.UtcNow;
                ForceReading();
                StartOrStopRemoteSoftwareLicenseCheck();
            }
        }

        static void StartOrStopRemoteSoftwareLicenseCheck()
        {
            Thread thread = null;
            lock (lockThreadObject)
            {
                if (bExitMode)
                    return;

                if (IsRemoteSoftwareLicenseCheckNeeded())
                {
                    if (WorkerThreadNetworkServer == null)
                    {
                        WorkerThreadNetworkServer = new Thread(WorkingThreadNetworkServer) { IsBackground = true };
                        WorkerThreadNetworkServer.Start();
                        if (NewForceCheckLicense != null)
                            NewForceCheckLicense.Set();
                    }
                }
                else if (WorkerThreadNetworkServer != null)
                {
                    if (NewForceCheckLicense != null)
                        NewForceCheckLicense.Set();
                    thread = WorkerThreadNetworkServer;
                    WorkerThreadNetworkServer = null;
                }
            }

            if (thread != null)
                thread.Join();
        }

        static bool IsRemoteSoftwareLicenseCheckNeeded()
        {
            return (StateLicense.HasValue && !StateLicense.Value || IsRemoved && !bSuspended) && srqtype == TransReqType.Software;
        }

        static void WorkingThreadNetworkServer(object data)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (!bExitMode)
            {
                NewForceCheckLicense.WaitOne(WaitWTime);
                if (bExitMode || !IsRemoteSoftwareLicenseCheckNeeded())
                    break;

                CheckNetworkServer();
                CheckFNetworkServer();
            }

            CloseNetworkServerTcp(true);
            if (checkFNetworkServerMutex != null)
            {
                checkFNetworkServerMutex.Dispose();
                checkFNetworkServerMutex = null;
            }
        }

        static void CloseTcp(bool forceclosing = false)
        {
            try
            {
                CleanClientTcp(forceclosing);
                if (srqtype == TransReqType.Net)
                    MSZNetClose();
            }
            catch
            { }
        }

        static void CloseNetworkServerTcp(bool forceclosing = false)
        {
            try
            {
                CleanClientWTcp(forceclosing);
                if (checkFNetworkServerMutex != null)
                    MSZFLNetClose();
            }
            catch
            { }
        }

        static void CheckNetworkServer()
        {
            try
            {
                var result = MSZWLRequest();
                if (result != null)
                {
                    string ret = WPFUtilities.CryptString.CryptString.DecryptString(result);
                    if (RemovedLicenses == null)
                        CleanRemoved();
                    if (RemovedLicenses != ret)
                    {
                        RemovedLicenses = ret;
                        ret.Split('@').ToList().ForEach(x =>
                        {
                            UpdateRemoved(x);
                        });
                    }
                }
            }
            catch
            { }
        }

        static Mutex checkFNetworkServerMutex = null;
        private static readonly ILicenseKeyService LicenseKeyService;

        static void CheckFNetworkServer()
        {
            if (bForciblySuspended || !bEnableRemoteRequest)
                return;

            if (checkFNetworkServerMutex == null)
            {
                string mutexName = $"{_mshobj}{currentProcess.ProcessName}";
                try
                {
                    using (var mutex = Mutex.OpenExisting(mutexName))
                    { }
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    //string user = Environment.UserDomainName + "\\"
                    //                    + Environment.UserName;
                    //MutexSecurity mSec = new MutexSecurity();

                    //MutexAccessRule rule = new MutexAccessRule(user,
                    //    MutexRights.Synchronize | MutexRights.Modify,
                    //    AccessControlType.Deny);
                    //mSec.AddAccessRule(rule);

                    //rule = new MutexAccessRule(user,
                    //    MutexRights.FullControl,
                    //    AccessControlType.Allow);
                    //mSec.AddAccessRule(rule);

                    bool createdNew = false;
                    try
                    {
#if !NET_STANDARD
                        var mutexSecurity = new MutexSecurity();
                        mutexSecurity.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                            MutexRights.FullControl, System.Security.AccessControl.AccessControlType.Allow));
#endif
                        checkFNetworkServerMutex = new Mutex(false, mutexName, out createdNew
#if !NET_STANDARD
                            , mutexSecurity
#endif
                            );
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Unable to create the mutex for server license check! ({0})", ex.Message));
                    }

                    if (!createdNew && checkFNetworkServerMutex != null)
                    {
                        checkFNetworkServerMutex.Dispose();
                        checkFNetworkServerMutex = null;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(String.Format("Unable to open mutex for server license check! ({0})", exception.Message));
                }
            }

            if (checkFNetworkServerMutex != null)
            {
                try
                {
                    string ret = MSZWLFakeRequest();
                    if (!string.IsNullOrEmpty(ret) && ret == "DTAIoMRyRAx0s8DI42iwQw==")
                    {
                        string serial = ReadModule(_sn);
                        if (serial != null)
                            UpdateRemoved(serial);
                        bSuspended = true;
                        bForciblySuspended = true;
                        ForceReading();
                    }
                }
                catch { }
            }
        }


        static bool ForceReading(bool postEvent = true)
        {
            bool bNotFoundLicense = false;
            string internalCache = null;

            if (bSuspended)
            {
                bNotFoundLicense = true;
            }
            else
            {
                internalCache = GetKey();
                if (Serial == 0)
                {
                    if (string.IsNullOrEmpty(internalCache))
                    {
                        bNotFoundLicense = true;
                        if (srqtype == TransReqType.Software)
                        {
                            srqtype = TransReqType.Net;
                            return ForceReading(postEvent);
                        }
                    }
                    else
                    {
                        string ret = ReadModule(_sitecode, internalCache);
                        string ret2 = ReadModule(_sn, internalCache);
                        if (string.IsNullOrEmpty(ret)
#if NET_STANDARD
                            && string.IsNullOrEmpty(ret2)
#endif
                            )
                        {
                            bNotFoundLicense = true;
                            if (srqtype == TransReqType.Software)
                            {
                                srqtype = TransReqType.Net;
                                return ForceReading(postEvent);
                            }
                        }
                        else if ((from c in GetKeyCodes() where c == ret2 select c).FirstOrDefault() != null)
                        {
                            IsRemoved = true;
                            bNotFoundLicense = true;
                        }
                        else if (!string.IsNullOrEmpty(ret) &&
                            MSZUtils.CheckPrevious(ret) != true
                            && srqtype != TransReqType.Net
                            )
                        {
                            bNotFoundLicense = true;
                            if (srqtype == TransReqType.Software)
                            {
                                srqtype = TransReqType.Net;
                                return ForceReading(postEvent);
                            }
                        }
                        else
                        {
                            string ret1 = ReadModule(_expiringdate, internalCache);
                            if (string.IsNullOrEmpty(ret1))
                            {
                                bNotFoundLicense = true;
                                if (srqtype == TransReqType.Software)
                                {
                                    srqtype = TransReqType.Net;
                                    return ForceReading(postEvent);
                                }
                            }
                            else
                            {
                                if (!ret1.Equals(NoDate))
                                {
                                    try
                                    {
                                        DateTime lDT = DateTime.Parse(ret1, System.Globalization.CultureInfo.InvariantCulture);
                                        if (DateTime.Compare(lDT, DateTime.UtcNow) < 0 || (lDT - DateTime.UtcNow) > TimeSpan.FromDays(MaxExpiringDays))
                                        {
                                            bNotFoundLicense = true;
                                            if (srqtype == TransReqType.Software)
                                            {
                                                srqtype = TransReqType.Net;
                                                return ForceReading(postEvent);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        bNotFoundLicense = true;
                                        if (srqtype == TransReqType.Software)
                                        {
                                            srqtype = TransReqType.Net;
                                            return ForceReading(postEvent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(internalCache))
                    {
                        bNotFoundLicense = true;
                        if (srqtype == TransReqType.Hardware)
                        {
                            srqtype = TransReqType.Software;
                            return ForceReading(postEvent);
                        }
                    }
                    else if (MSZDelRead.UConfigData[0] == 1)
                    {
                        var ret = ReadModule(_sitecode, internalCache);
                        if (string.IsNullOrEmpty(ret))
                        {
                            bNotFoundLicense = true;
                            if (srqtype == TransReqType.Hardware)
                            {
                                srqtype = TransReqType.Software;
                                return ForceReading(postEvent);
                            }
                        }
                    }
                }
            }

            bool statechanged = StateLicense != bNotFoundLicense;
            bool internalCacheChanged = internalCache != InternalCache;
            
            if (StateLicense != bNotFoundLicense)
                StateLicense = bNotFoundLicense;

            if (statechanged || internalCacheChanged)
            {
                lock (lockObject)
                {
                    InternalCache = internalCache;
                    cacheKeyValues.Clear();
                    UpdateCache();
                }
            }

            LicenseRead.Set();

            if (postEvent)
            {
                MSZEventArgs m = new MSZEventArgs(bNotFoundLicense, bNotFoundLicense ? (LastScheduleTime - FirstScheduleTime).TotalSeconds : 0, statechanged);
                OnKeyChangingEvent(m);
            }

            return bNotFoundLicense;
        }

        static void UpdateCache()
        {
            try
            {
                if (!String.IsNullOrEmpty(InternalCache))
                {
                    var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(InternalCache, modulesTag, true, false);
                    if (keyexpandolist.Count() != 0)
                    {
                        keyexpandolist.ToList().ForEach(element =>
                        {
                            var dictionary = element as IDictionary<string, object>;
                            dictionary.ToList().ForEach(d =>
                            {
                                var key = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><Modules><{0}/></Modules>", d.Key);
                                key = WPFUtilities.CryptString.CryptString.EncryptString(key);
                                if (!cacheKeyValues.ContainsKey(key))
                                {
                                    cacheKeyValues[key] = String.Format("{0}", d.Value);
                                }
                            });
                        });
                    }
                }

            }
            catch
            { }
        }

        static string ReadModule(string module, string xamlkey = null)
        {
            string value = null;

            try
            {
                if (string.IsNullOrEmpty(xamlkey))
                {
                    lock (lockObject)
                    {
                        xamlkey = InternalCache;
                    }
                }

                if (!String.IsNullOrEmpty(xamlkey))
                {
                    var keyexpandolist = Utilities.XmlHelper.GetExpandoFromXml(xamlkey, modulesTag, true, false);
                    if (keyexpandolist.Count() != 0)
                    {
                        var xamlModule = WPFUtilities.CryptString.CryptString.DecryptString(module);
                        var modulesexpandoList = Utilities.XmlHelper.GetExpandoFromXml(xamlModule, modulesTag, true, false);
                        if (modulesexpandoList.Count() != 0)
                        {
                            modulesexpandoList.ToList().ForEach(element =>
                            {
                                var dictionary = element as IDictionary<string, object>;
                                dictionary.ToList().ForEach(d =>
                                {
                                    keyexpandolist.ToList().ForEach(e =>
                                    {
                                        var regkeydictionary = e as IDictionary<string, object>;
                                        if (regkeydictionary.ContainsKey(d.Key))
                                            value = string.Format("{0}", regkeydictionary[d.Key]);
                                    });
                                });
                            });
                        }
                    }
                }

            }
            catch
            { }

            return value;
        }

        static string GetKey()
        {
            if (bSuspended)
                return string.Empty;
#if !NET_STANDARD
            if (System.Windows.Forms.SystemInformation.TerminalServerSession && !_Krytp)
            {
                var proclist = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);
                proclist.ToList().ForEach(x =>
                {
                    if ((from p in proclist where p.SessionId != x.SessionId select p).FirstOrDefault() != null)
                    {
                        srqtype = TransReqType.Net;
                    }
                });
                proclist.ToList().ForEach(x => x.Dispose());
            }
            else if (MSZUtils.IsGoGlobalServerSession && !_Krytp)
            {
                var proclist = System.Diagnostics.Process.GetProcessesByName(currentProcess.ProcessName);
                var GoGlobalProcess = (from c in proclist where c.SessionId == 0 select c).ToList();
                if (GoGlobalProcess.Count > 1)
                    srqtype = TransReqType.Net;
                proclist.ToList().ForEach(x => x.Dispose());
            }
#endif
            try
            {
                if (srqtype == TransReqType.Hardware)
                {
                    if (isDeamon)
                    {
                        var keyDeamon = ReadDaemon();
                        if (!String.IsNullOrEmpty(keyDeamon))
                            return keyDeamon;
                    }

                    string _sgKey;
                    int retry = 0;

                    while (true)
                    {
                        _sgKey = ReadSGKey();

                        if (_sgKey != null || ++retry >= 3)
                            break;

                        Thread.Sleep(100);
                    }

                    if (String.IsNullOrEmpty(_sgKey))
                        srqtype = TransReqType.Software;
                    else
                    {
                        //_Serial = MSZDelRead.SerialNumber[0];
                        _Serial = MSZDelRead.ReadMovSerialNumber();
                        return _sgKey;
                    }

                }
            }
            catch
            {
                srqtype = TransReqType.Software;
            }

            if (srqtype == TransReqType.Software)
            {
                _Serial = 0;
                if (File.Exists(FilePath))
                {
                    try
                    {
                        return WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(FilePath));
                    }
                    catch (Exception)
                    {
                        return string.Empty;
                    }
                }
                else if (!Krytp)
                    srqtype = TransReqType.Net;
                else
                {
                    var keyDeamon = ReadDaemon();
                    if (!String.IsNullOrEmpty(keyDeamon))
                        return keyDeamon;

                    return string.Empty;
                }
            }

            if (srqtype == TransReqType.Net)
            {
                if (_Krytp)
                {
                    var keyDeamon = ReadDaemon();
                    if (!String.IsNullOrEmpty(keyDeamon))
                        return keyDeamon;

                    _Serial = 0;
                    srqtype = TransReqType.Hardware;
                    return string.Empty;
                }

                _Serial = 0;
                var ret = WPFUtilities.CryptString.CryptString.DecryptString(MSZNetRequest());
                if (!String.IsNullOrEmpty(ret))
                {
                    return ret;
                }
                else
                {
                    var keyDeamon = ReadDaemon();
                    if (!String.IsNullOrEmpty(keyDeamon))
                        return keyDeamon;
                }
            }

            return string.Empty;
        }

        static void LoadXmlSkuFile()
        {
            try
            {
                var filepath = String.Format("{0}{4}{1}{4}{2}{4}{3}",
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    customCompanyNameAndApplicationFolder?.Item1 ?? Properties.Settings.Default.CompanyName,
                    customCompanyNameAndApplicationFolder?.Item2 ?? Properties.Settings.Default.CommonApplicationFolder,
                    Properties.Settings.Default.LicenseSKUFileName,
                    Path.DirectorySeparatorChar);
                xamlsku = WPFUtilities.CryptString.CryptString.DecryptString(System.IO.File.ReadAllText(filepath));
            }
            catch
            { }
        }

        static string ReadDaemon()
        {
            try
            {
                bool bSuccess = false;
                uint lResult = 0;
                uint jsonBufferSize = MSZDll.IPLD_DEFAULT_JSON_BUFFSIZE;
                var json_buffer = new StringBuilder((int)jsonBufferSize);

                bSuccess = MSZDll.ipldGetLicStatus(Utilities.AssemblyInfo.FileFormatVersion, json_buffer, ref jsonBufferSize, ref lResult);
                if (!bSuccess && lResult == (UInt32)MSZDll.ipldResult.IPLD_ERROR_NOT_ENOUGH_MEMORY)
                {
                    json_buffer = new StringBuilder((int)jsonBufferSize);
                    bSuccess = MSZDll.ipldGetLicStatus(Utilities.AssemblyInfo.FileFormatVersion, json_buffer, ref jsonBufferSize, ref lResult);
                }

                if (bSuccess && lResult == (UInt32)MSZDll.ipldResult.IPLD_NO_ERROR)
                {
                    var licenseInfo = JObject.Parse(json_buffer.ToString());
                    var skuType = licenseInfo["sku1"];
                    var doc = System.Xml.Linq.XDocument.Parse(xamlsku);
                    var expandoObject = new System.Dynamic.ExpandoObject();
                    var element = doc.Root.XPathSelectElement(String.Format("//sku{0}", skuType));
                    if (element != null)
                    {
                        XmlDocument xDoc = new XmlDocument();
                        XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
                        xDoc.AppendChild(xDecl);

                        XmlElement xRoot = xDoc.CreateElement(modulesTag);
                        xDoc.AppendChild(xRoot);

                        //SiteCode
                        //XmlElement xElemL0 = xDoc.CreateElement(string.Format("{0}", "SiteCode"));
                        //xElemL0.InnerText = licenseInfo["mac"].Value<String>();
                        //xRoot.AppendChild(xElemL0);
                        foreach (var child in element.Descendants())
                        {
                            var xElem = xDoc.CreateElement(child.Name.LocalName);
                            xElem.InnerText = child.Value;
                            xRoot.AppendChild(xElem);
                        }

                        _Serial = skuType.Value<uint>();
                        srqtype = TransReqType.Hardware;
                        isDeamon = true;
                        return xDoc.InnerXml;
                    }
                }
            }
            catch (Exception ex)
            { }

            _Serial = 0;
            isDeamon = false;
            return string.Empty;
        }
#endregion
    }
}
