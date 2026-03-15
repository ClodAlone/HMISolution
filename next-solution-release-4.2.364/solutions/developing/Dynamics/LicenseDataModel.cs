using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MSZ
{
    public class LicenseDataModel : INotifyPropertyChanged
    {
        #region Declarations
        public event PropertyChangedEventHandler PropertyChanged;
        private const int BOptions = 24;
        private const int IOptions = 11;
        public List<string> BOptionMap = new List<string>();
        public List<string> IOptionMap = new List<string>();
        #endregion
        #region Public props
        string siteCode;
        public string SiteCode
        {
            get
            {
                return siteCode;
            }
            set
            {
                if (siteCode != value)
                {
                    siteCode = value;
                    OnPropertyChanged("SiteCode");
                }
            }
        }
        string licenseType;
        public string LicenseType
        {
            get
            {
                return licenseType;
            }
            set
            {
                if (licenseType != value)
                {
                    licenseType = value;
                    OnPropertyChanged("LicenseType");
                }
            }
        }
        string expiringDate;
        public string ExpiringDate
        {
            get
            {
                return expiringDate;
            }
            set
            {
                if (value != expiringDate)
                {
                    expiringDate = value;
                    OnPropertyChanged("ExpiringDate");
                }
            }
        }
        string serialNumberText;
        public string SerialNumberText
        {
            get
            {
                return serialNumberText;
            }
            set
            {
                if (value != serialNumberText)
                {
                    serialNumberText = value;
                    OnPropertyChanged("SerialNumberText");
                }
            }
        }
        string serialNumber;
        public string SerialNumber
        {
            get
            {
                return serialNumber;
            }
            set
            {
                if (value != serialNumber)
                {
                    serialNumber = value;
                    OnPropertyChanged("SerialNumber");
                }
            }
        }
        string netState;
        public string NetState
        {
            get
            {
                return netState;
            }
            set
            {
                if (value != netState)
                {
                    netState = value;
                    OnPropertyChanged("NetState");
                }
            }
        }
        string ioTagsNumber;
        public string IOTagsNumber
        {
            get
            {
                return ioTagsNumber;
            }
            set
            {
                if (value != ioTagsNumber)
                {
                    ioTagsNumber = value;
                    OnPropertyChanged("IOTagsNumber");
                }
            }
        }
        string driversEnabled;
        public string DriversEnabled
        {
            get
            {
                return driversEnabled;
            }
            set
            {
                if (value != driversEnabled)
                {
                    driversEnabled = value;
                    OnPropertyChanged("DriversEnabled");
                }
            }
        }
        string childProjectsNo;
        public string ChildProjectsNo
        {
            get
            {
                return childProjectsNo;
            }
            set
            {
                if (value != childProjectsNo)
                {
                    childProjectsNo = value;
                    OnPropertyChanged("ChildProjectsNo");
                }
            }
        }
        string numberOfScreens;
        public string NumberOfScreens
        {
            get
            {
                return numberOfScreens;
            }
            set
            {
                if (value != numberOfScreens)
                {
                    numberOfScreens = value;
                    OnPropertyChanged("NumberOfScreens");
                }
            }
        }
        string numberOfAlarms;
        public string NumberOfAlarms
        {
            get
            {
                return numberOfAlarms;
            }
            set
            {
                if (value != numberOfAlarms)
                {
                    numberOfAlarms = value;
                    OnPropertyChanged("NumberOfAlarms");
                }
            }
        }
        string webClientUsers;
        public string WebClientUsers
        {
            get
            {
                return webClientUsers;
            }
            set
            {
                if (value != webClientUsers)
                {
                    webClientUsers = value;
                    OnPropertyChanged("WebClientUsers");
                }
            }
        }
        string concurrentIODataServers;
        public string ConcurrentIODataServers
        {
            get
            {
                return concurrentIODataServers;
            }
            set
            {
                if (value != concurrentIODataServers)
                {
                    concurrentIODataServers = value;
                    OnPropertyChanged("ConcurrentIODataServers");
                }
            }
        }
        string netLicenseUsers;
        public string NETLicenseUsers
        {
            get
            {
                return netLicenseUsers;
            }
            set
            {
                if (value != netLicenseUsers)
                {
                    netLicenseUsers = value;
                    OnPropertyChanged("NETLicenseUsers");
                }
            }
        }
        string proEnergyPoints;
        public string ProEnergyPoints
        {
            get
            {
                return proEnergyPoints;
            }
            set
            {
                if (value != proEnergyPoints)
                {
                    proEnergyPoints = value;
                    OnPropertyChanged("ProEnergyPoints");
                }
            }
        }
        string proLeanOEE;
        public string ProLeanOEE
        {
            get
            {
                return proLeanOEE;
            }
            set
            {
                if (value != proLeanOEE)
                {
                    proLeanOEE = value;
                    OnPropertyChanged("ProLeanOEE");
                }
            }
        }
        bool driverBasic;
        public bool DriverBasic
        {
            get
            {
                return driverBasic;
            }
            set
            {
                if (value != driverBasic)
                {
                    driverBasic = value;
                    OnPropertyChanged("DriverBasic");
                }
            }
        }
        bool driverAutomation;
        public bool DriverAutomation
        {
            get
            {
                return driverAutomation;
            }
            set
            {
                if (value != driverAutomation)
                {
                    driverAutomation = value;
                    OnPropertyChanged("DriverAutomation");
                }
            }
        }
        bool driverTelemetry;
        public bool DriverTelemetry
        {
            get
            {
                return driverTelemetry;
            }
            set
            {
                if (value != driverTelemetry)
                {
                    driverTelemetry = value;
                    OnPropertyChanged("DriverTelemetry");
                }
            }
        }
        bool driverFacilities;
        public bool DriverFacilities
        {
            get
            {
                return driverFacilities;
            }
            set
            {
                if (value != driverFacilities)
                {
                    driverFacilities = value;
                    OnPropertyChanged("DriverFacilities");
                }
            }
        }
        bool driverIoT;
        public bool DriverIoT
        {
            get
            {
                return driverIoT;
            }
            set
            {
                if (value != driverIoT)
                {
                    driverIoT = value;
                    OnPropertyChanged("DriverIoT");
                }
            }
        }
        bool licenseNetCore;
        public bool LicenseNetCore
        {
            get
            {
                return licenseNetCore;
            }
            set
            {
                if (value != licenseNetCore)
                {
                    licenseNetCore = value;
                    OnPropertyChanged("LicenseNetCore");
                }
            }
        }
        bool licenseEditor;
        public bool LicenseEditor
        {
            get
            {
                return licenseEditor;
            }
            set
            {
                if (value != licenseEditor)
                {
                    licenseEditor = value;
                    OnPropertyChanged("LicenseEditor");
                }
            }
        }
        bool licenseServerRT;
        public bool LicenseServerRT
        {
            get
            {
                return licenseServerRT;
            }
            set
            {
                if (value != licenseServerRT)
                {
                    licenseServerRT = value;
                    OnPropertyChanged("LicenseServerRT");
                }
            }
        }
        bool licenseClientRT;
        public bool LicenseClientRT
        {
            get
            {
                return licenseClientRT;
            }
            set
            {
                if (value != licenseClientRT)
                {
                    licenseClientRT = value;
                    OnPropertyChanged("LicenseClientRT");
                }
            }
        }
        bool licenseHistorianDatalogger;
        public bool LicenseHistorianDatalogger
        {
            get
            {
                return licenseHistorianDatalogger;
            }
            set
            {
                if (value != licenseHistorianDatalogger)
                {
                    licenseHistorianDatalogger = value;
                    OnPropertyChanged("LicenseHistorianDatalogger");
                }
            }
        }
        bool licenseRecipes;
        public bool LicenseRecipes
        {
            get
            {
                return licenseRecipes;
            }
            set
            {
                if (value != licenseRecipes)
                {
                    licenseRecipes = value;
                    OnPropertyChanged("LicenseRecipes");
                }
            }
        }
        bool licenseVBNET;
        public bool LicenseVBNET
        {
            get
            {
                return licenseVBNET;
            }
            set
            {
                if (value != licenseVBNET)
                {
                    licenseVBNET = value;
                    OnPropertyChanged("LicenseVBNET");
                }
            }
        }
        bool licenseScheduler;
        public bool LicenseScheduler
        {
            get
            {
                return licenseScheduler;
            }
            set
            {
                if (value != licenseScheduler)
                {
                    licenseScheduler = value;
                    OnPropertyChanged("LicenseScheduler");
                }
            }
        }
        bool licenseNetworking;
        public bool LicenseNetworking
        {
            get
            {
                return licenseNetworking;
            }
            set
            {
                if (value != licenseNetworking)
                {
                    licenseNetworking = value;
                    OnPropertyChanged("LicenseNetworking");
                }
            }
        }
        bool licenseReports;
        public bool LicenseReports
        {
            get
            {
                return licenseReports;
            }
            set
            {
                if (value != licenseReports)
                {
                    licenseReports = value;
                    OnPropertyChanged("LicenseReports");
                }
            }
        }
        bool licenseAlarmDispatcher;
        public bool LicenseAlarmDispatcher
        {
            get
            {
                return licenseAlarmDispatcher;
            }
            set
            {
                if (value != licenseAlarmDispatcher)
                {
                    licenseAlarmDispatcher = value;
                    OnPropertyChanged("LicenseAlarmDispatcher");
                }
            }
        }
        bool licenseDowntimeAnalyzer;
        public bool LicenseDowntimeAnalyzer
        {
            get
            {
                return licenseDowntimeAnalyzer;
            }
            set
            {
                if (value != licenseDowntimeAnalyzer)
                {
                    licenseDowntimeAnalyzer = value;
                    OnPropertyChanged("LicenseDowntimeAnalyzer");
                }
            }
        }
        bool licenseOPCUAServer;
        public bool LicenseOPCUAServer
        {
            get
            {
                return licenseOPCUAServer;
            }
            set
            {
                if (value != licenseOPCUAServer)
                {
                    licenseOPCUAServer = value;
                    OnPropertyChanged("LicenseOPCUAServer");
                }
            }
        }
        bool licenseRedundancy;
        public bool LicenseRedundancy
        {
            get
            {
                return licenseRedundancy;
            }
            set
            {
                if (value != licenseRedundancy)
                {
                    licenseRedundancy = value;
                    OnPropertyChanged("LicenseRedundancy");
                }
            }
        }
        bool licenseGeoScada;
        public bool LicenseGeoScada
        {
            get
            {
                return licenseGeoScada;
            }
            set
            {
                if (value != licenseGeoScada)
                {
                    licenseGeoScada = value;
                    OnPropertyChanged("LicenseGeoScada");
                }
            }
        }
        bool license3D;
        public bool License3D
        {
            get
            {
                return license3D;
            }
            set
            {
                if (value != license3D)
                {
                    license3D = value;
                    OnPropertyChanged("License3D");
                }
            }
        }
        bool licenseAR;
        public bool LicenseAR
        {
            get
            {
                return licenseAR;
            }
            set
            {
                if (value != licenseAR)
                {
                    licenseAR = value;
                    OnPropertyChanged("LicenseAR");
                }
            }
        }
        bool licenseArray1;
        public bool LicenseArray1
        {
            get
            {
                return licenseArray1;
            }
            set
            {
                if (value != licenseArray1)
                {
                    licenseArray1 = value;
                    OnPropertyChanged("LicenseArray1");
                }
            }
        }
        bool clientOptions;
        public bool ClientOptions
        {
            get
            {
                return clientOptions;
            }
            set
            {
                if (value != clientOptions)
                {
                    clientOptions = value;
                    OnPropertyChanged("ClientOptions");
                }
            }
        }
        bool mSZMSZViewCheckState;
        public bool MSZMSZViewCheckState
        {
            get
            {
                return mSZMSZViewCheckState;
            }
            set
            {
                if (value != mSZMSZViewCheckState)
                {
                    mSZMSZViewCheckState = value;
                    OnPropertyChanged("MSZMSZViewCheckState");
                }
            }
        }
        public Dictionary<string, string> OptionsTags { get; set; }
        #endregion
        #region Ctors
        public LicenseDataModel() {
            ClientOptions = true;
        }
        public LicenseDataModel(bool bOnlyServerOptions)
        {
            ClientOptions = !bOnlyServerOptions;
        }
        #endregion
        #region Methods
        public async Task LoadDataAsync()
        {
            await Task.Run(delegate
            {
                LoadData();
            });
        }

        public void LoadData()
        {
            LoadMainData();
            LoadBOptions();
            LoadIOptions();
        }

        void LoadMainData()
        {
            MSZMSZViewCheckState = MSZ.MSZView.CheckState(true);
            if (MSZMSZViewCheckState)
                LicenseType = Properties.Resources.LicenseTypeDemo;
            else
                LicenseType = String.IsNullOrEmpty(MSZ.MSZView.GetSiteKey()) ? Properties.Resources.LicenseTypeEmbedded : Properties.Resources.LicenseTypeFull;
            SiteCode = MSZ.MSZUtils.GetPrevious();
            var expiringDate = MSZ.MSZView.GetExpiringDate();
            ExpiringDate = expiringDate != DateTime.MinValue ? expiringDate.ToShortDateString() : null;
            SerialNumber = MSZ.MSZView.GetSerial();
            NetState = WPFUtilities.CryptString.CryptString.DecryptString(MSZ.MSZView.GetNetState());
            if (!MSZMSZViewCheckState && !string.IsNullOrEmpty(NetState))
                NETLicenseUsers = NetState;
        }
        void LoadBOptions()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="), "");
            xDoc.AppendChild(xRoot);

            for (var i = 1; i <= BOptions; i++)
            {
                var tname = string.Format("BOption{0}", i);
                BOptionMap.Add(tname);
                xRoot.AppendChild(xDoc.CreateElement(string.Format("{0}", OptionsTags[string.Format("Tag{0}", tname)])));
            }
            var ret = MSZ.MSZView.GetMultiModules(WPFUtilities.CryptString.CryptString.EncryptString(xDoc.InnerXml));
            try
            {
                LicenseEditor = ret[1];
                LicenseServerRT = ret[2];
                LicenseClientRT = ret[3];
                LicenseHistorianDatalogger = ret[4];
                LicenseRecipes = ret[5];
                LicenseVBNET = ret[6];
                LicenseScheduler = ret[7];
                LicenseNetworking = ret[8];
                LicenseReports = ret[11];
                LicenseAlarmDispatcher = ret[12];
                LicenseDowntimeAnalyzer = ret[13];
                LicenseOPCUAServer = ret[14];
                LicenseRedundancy = ret[15];

                LicenseGeoScada = ret[9];
                License3D = ret[10];

                DriverBasic = ret[16];
                DriverAutomation = ret[17];
                DriverTelemetry = ret[18];
                DriverFacilities = ret[19];
                DriverIoT = ret[20];

                LicenseAR = ret[21];
                LicenseNetCore = ret[22];
                LicenseArray1 = ret[23];
            }
            catch { }
        }
        void LoadIOptions()
        {
            XmlDocument xDoc = new XmlDocument();
            XmlDeclaration xDecl = xDoc.CreateXmlDeclaration("1.0", System.Text.Encoding.UTF8.WebName, null);
            xDoc.AppendChild(xDecl);

            XmlElement xRoot = xDoc.CreateElement("", WPFUtilities.CryptString.CryptString.DecryptString("ulzPfvth+VjJrz5nlKNkWQ=="), "");
            xDoc.AppendChild(xRoot);

            for (var i = 1; i <= IOptions; i++)
            {
                var tname = string.Format("IOption{0}", i);
                IOptionMap.Add(tname);
                xRoot.AppendChild(xDoc.CreateElement(string.Format("{0}", OptionsTags[string.Format("Tag{0}", tname)])));
            }
            var ret = MSZ.MSZView.GetMultiModule(WPFUtilities.CryptString.CryptString.EncryptString(xDoc.InnerXml));
            try
            {
                IOTagsNumber = ret[0];
                DriversEnabled = ret[2];
                ChildProjectsNo = ret[3];
                NumberOfScreens = ret[4];
                NumberOfAlarms = ret[5];
                WebClientUsers = ret[6];
                NETLicenseUsers = ret[7];
                ProEnergyPoints = ret[8];
                ProLeanOEE = ret[9];
                ConcurrentIODataServers = ret[10];
            }
            catch { }
        }
        void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }
}
