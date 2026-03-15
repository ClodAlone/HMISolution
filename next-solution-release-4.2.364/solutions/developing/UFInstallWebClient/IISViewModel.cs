using DataGridElementSettings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.WPF;

namespace UFInstallWebClient
{
    internal class IISViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Public Properties
        private WebClientType _WebClientType = WebClientType.Html5;
        public WebClientType WebClientType
        {
            get { return _WebClientType; }
            set
            {
                if (_WebClientType == value)
                    return;

                _WebClientType = value;
                OnPropertyChanged("WebClientType");
                OnPropertyChanged("IsValid");
            }
        }

        private int _AutoLogoutSeconds = 0;
        public int AutoLogoutSeconds
        {
            get { return _AutoLogoutSeconds; }
            set
            {
                if (_AutoLogoutSeconds == value)
                    return;

                _AutoLogoutSeconds = value;
                OnPropertyChanged("AutoLogoutSeconds");
            }
        }

        private bool _EnableUserManager = false;
        public bool EnableUserManager
        {
            get { return _EnableUserManager; }
            set
            {
                if (_EnableUserManager == value)
                    return;

                _EnableUserManager = value;
                OnPropertyChanged("EnableUserManager");
            }
        }

        private int _MaxInvalidPasswordAttempts;
        public int MaxInvalidPasswordAttempts
        {
            get { return _MaxInvalidPasswordAttempts; }
            set
            {
                if (_MaxInvalidPasswordAttempts == value)
                    return;

                _MaxInvalidPasswordAttempts = value;
                OnPropertyChanged("MaxInvalidPasswordAttempts");
            }
        }

        private int _CallingProcessId;
        public int CallingProcessId
        {
            get { return _CallingProcessId; }
            set
            {
                _CallingProcessId = value;
                OnPropertyChanged("CallingProcessId");
            }
        }

        public string CurrentSkin
        {
            get;
            set;
        } = "Blend";

        private string _WebSite = Properties.Settings.Default.DefaultWebSite;
        public string WebSite
        {
            get { return _WebSite; }
            set
            {
                if (_WebSite == value)
                    return;

                _WebSite = value;
                OnPropertyChanged("WebSite");
                OnPropertyChanged("IsValid");
            }
        }

        private string _ApplicationName = string.Empty;
        public string ApplicationName
        {
            get { return _ApplicationName; }
            internal set
            {
                if (_ApplicationName == value)
                    return;

                _ApplicationName = value;
                OnPropertyChanged("ApplicationName");
            }
        }

        private string _ApplicationTheme = Properties.Settings.Default.DefaultTheme;
        public string ApplicationTheme
        {
            get { return _ApplicationTheme; }
            internal set
            {
                if (_ApplicationTheme == value)
                    return;

                _ApplicationTheme = value;
                OnPropertyChanged("ApplicationTheme");
            }
        }

        private string _AliasName = string.Empty;
        public string AliasName
        {
            get { return _AliasName; }
            set
            {
                var normalizedValue = NormalizeAliasName(value);
                if (_AliasName == normalizedValue)
                    return;

                _AliasName = normalizedValue;
                OnPropertyChanged("AliasName");
                OnPropertyChanged("IISUrl");
                OnPropertyChanged("IsValid");
            }
        }

        private string _DeployPath = string.Empty;
        public string DeployPath
        {
            get 
            {
                while (_DeployPath.EndsWith("\\"))
                    _DeployPath = _DeployPath.Substring(0, _DeployPath.Length - 1);

                return _DeployPath; 
            }
            set
            {
                if (_DeployPath == value)
                    return;

                _DeployPath = value;
                OnPropertyChanged("DeployPath");
                OnPropertyChanged("IsValid");
            }
        }

        private string _UriProjectPath = string.Empty;
        public string UriProjectPath
        {
            get { return _UriProjectPath; }
            set
            {
                if (_UriProjectPath == value)
                    return;

                _UriProjectPath = value;
                OnPropertyChanged("UriProjectPath");
                OnPropertyChanged("IsValid");
            }
        }

        private string[] _UriSpecialFolders;
        public string[] UriSpecialFolders
        {
            get { return _UriSpecialFolders; }
            set
            {
                if (_UriSpecialFolders == value)
                    return;

                _UriSpecialFolders = value;
                OnPropertyChanged("UriSpecialFolders");
            }
        }

        private string[] _UriChildProjects;
        public string[] UriChildProjects
        {
            get { return _UriChildProjects; }
            set
            {
                if (_UriChildProjects == value)
                    return;

                _UriChildProjects = value;
                OnPropertyChanged("UriChildProjects");
            }
        }

        public string ProjectName
        {
            get 
            {
                if (!String.IsNullOrEmpty(UriProjectPath))
                {
                    if (IsDataSourceProjectBase)
                        return NormalizeAliasName(XpoHelpers.XpoHelper.GetDataSourceTitle(UriProjectPath));
                    else
                        return System.IO.Path.GetFileNameWithoutExtension(UriProjectPath);
                }

                return String.Empty;
            }
        }

        private string _ApplicationPoolName = Properties.Settings.Default.DefaultAppPool;
        public string ApplicationPoolName
        {
            get { return _ApplicationPoolName; }
            set
            {
                if (_ApplicationPoolName == value)
                    return;

                if (value == null)
                    return;

                _ApplicationPoolName = value;
                OnPropertyChanged("ApplicationPoolName");
                OnPropertyChanged("IsValid");
            }
        }

        private ObservableCollection<string> _ApplicationPoolList;
        public ObservableCollection<string> ApplicationPoolList
        {
            get
            {
                if (_ApplicationPoolList == null)
                {
                    _ApplicationPoolList = new ObservableCollection<string>();
                    var appPools = IIS7Manager.IISAppPool.GetAppPools();
                    appPools.ForEach((name) => { _ApplicationPoolList.Add(name); });
                }

                return _ApplicationPoolList;
            }
        }

        private string _IndentityName = Properties.Settings.Default.DefaultIdentity;
        public string IndentityName
        {
            get 
            {
                if (IndentityValue == IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.SpecificUser)
                    return _IndentityName; 
                else
                    return Enum.GetName(typeof(IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType), IndentityValue);
            }
            set
            {
                if (_IndentityName == value)
                    return;

                _IndentityName = value;
                OnPropertyChanged("IndentityName");
                OnPropertyChanged("IsValid");
            }
        }

        private string _IndentityPassword;
        public string IndentityPassword
        {
            get
            {
                if (IndentityValue == IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.SpecificUser)
                    return _IndentityPassword;
                else
                    return String.Empty;
            }
            set
            {
                if (_IndentityPassword == value)
                    return;

                _IndentityPassword = value;
                OnPropertyChanged("IndentityPassword");
                OnPropertyChanged("IsValid");
            }
        }

        private IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType _IndentityValue = IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType.ApplicationPoolIdentity;
        public IIS7Manager.IISAppPool.IISAppPoolProcessModelIdentityType IndentityValue
        {
            get { return _IndentityValue; }
            set
            {
                if (_IndentityValue == value)
                    return;

                _IndentityValue = value;
                OnPropertyChanged("IndentityValue");
                OnPropertyChanged("IsValid");
            }
        }

        public string IISUrl
        {
            get
            {
                return String.Format(Properties.Resources.IISHttpUrl, AliasName);
            }
        }

        #region Advanced Settings

        int _RefreshPollingTime = Properties.Settings.Default.RefreshPollingTimeDefaultValue;
        public int RefreshPollingTime
        {
            get
            {
                return _RefreshPollingTime;
            }
            set
            {
                if (_RefreshPollingTime == value)
                    return;

                _RefreshPollingTime = value;
                OnPropertyChanged("RefreshPollingTime");
            }
        }

        int _RefreshPollingTimeCount = Properties.Settings.Default.RefreshPollingTimeCountDefaultValue;
        public int RefreshPollingTimeCount
        {
            get
            {
                return _RefreshPollingTimeCount;
            }
            set
            {
                if (_RefreshPollingTimeCount == value)
                    return;

                _RefreshPollingTimeCount = value;
                OnPropertyChanged("RefreshPollingTimeCount");
            }
        }

        int _DelayBroadcaster = Properties.Settings.Default.DelayBroadcasterDefaultValue;
        public int DelayBroadcaster
        {
            get
            {
                return _DelayBroadcaster;
            }
            set
            {
                if (_DelayBroadcaster == value)
                    return;

                _DelayBroadcaster = value;
                OnPropertyChanged("DelayBroadcaster");
            }
        }

        int _SessionTimeout = Properties.Settings.Default.SessionTimeoutDefaultValue;
        public int SessionTimeout
        {
            get
            {
                return _SessionTimeout;
            }
            set
            {
                if (_SessionTimeout == value)
                    return;

                _SessionTimeout = value;
                OnPropertyChanged("SessionTimeout");
            }
        }

        int _ConcurrentRenderingPipeline = Properties.Settings.Default.ConcurrentRenderingPipelineValue;
        public int ConcurrentRenderingPipeline
        {
            get
            {
                return _ConcurrentRenderingPipeline;
            }
            set
            {
                if (_ConcurrentRenderingPipeline == value)
                    return;

                _ConcurrentRenderingPipeline = value;
                OnPropertyChanged("ConcurrentRenderingPipeline");
            }
        }

        bool _StaticOptimization;
        public bool StaticOptimization
        {
            get
            {
                return _StaticOptimization;
            }
            set
            {
                if (_StaticOptimization == value)
                    return;

                _StaticOptimization = value;
                OnPropertyChanged("StaticOptimization");
            }
        }

        bool _ShowProjectTitle = true;
        public bool ShowProjectTitle
        {
            get
            {
                return _ShowProjectTitle;
            }
            set
            {
                if (_ShowProjectTitle == value)
                    return;

                _ShowProjectTitle = value;
                OnPropertyChanged("ShowProjectTitle");
            }
        }

        bool _DisableCreateNewDashboard;
        public bool DisableCreateNewDashboard
        {
            get
            {
                return _DisableCreateNewDashboard;
            }
            set
            {
                if (_DisableCreateNewDashboard == value)
                    return;

                _DisableCreateNewDashboard = value;
                OnPropertyChanged("DisableCreateNewDashboard");
            }
        }

        bool _SwitchToViewerDashboard;
        public bool SwitchToViewerDashboard
        {
            get
            {
                return _SwitchToViewerDashboard;
            }
            set
            {
                if (_SwitchToViewerDashboard == value)
                    return;

                _SwitchToViewerDashboard = value;
                OnPropertyChanged("SwitchToViewerDashboard");
            }
        }

        bool _ShowHeader = true;
        public bool ShowHeader
        {
            get
            {
                return _ShowHeader;
            }
            set
            {
                if (_ShowHeader == value)
                    return;

                _ShowHeader = value;
                OnPropertyChanged("ShowHeader");
            }
        }

        bool _ShowScreenNavigator = true;
        public bool ShowScreenNavigator
        {
            get
            {
                return _ShowScreenNavigator;
            }
            set
            {
                if (_ShowScreenNavigator == value)
                    return;

                _ShowScreenNavigator = value;
                OnPropertyChanged("ShowScreenNavigator");
            }
        }

        bool _ShowUserRegisterLink = true;
        public bool ShowUserRegisterLink
        {
            get
            {
                return _ShowUserRegisterLink;
            }
            set
            {
                if (_ShowUserRegisterLink == value)
                    return;

                _ShowUserRegisterLink = value;
                OnPropertyChanged("ShowUserRegisterLink");
            }
        }

        bool _LowResolution = Properties.Settings.Default.LowResolutionDefaultValue;
        public bool LowResolution
        {
            get
            {
                return _LowResolution;
            }
            set
            {
                if (_LowResolution == value)
                    return;

                _LowResolution = value;
                OnPropertyChanged("LowResolution");
            }
        }

        bool _SoftwareRendering = Properties.Settings.Default.SoftwareRenderingDefaultValue;
        public bool SoftwareRendering
        {
            get
            {
                return _SoftwareRendering;
            }
            set
            {
                if (_SoftwareRendering == value)
                    return;

                _SoftwareRendering = value;
                OnPropertyChanged("SoftwareRendering");
            }
        }

        bool _DisablePopupScreen;
        public bool DisablePopupScreen
        {
            get
            {
                return _DisablePopupScreen;
            }
            set
            {
                if (_DisablePopupScreen == value)
                    return;

                _DisablePopupScreen = value;
                OnPropertyChanged("DisablePopupScreen");
            }
        }

        string _ProjectTitle;
        public string ProjectTitle
        {
            get
            {
                return _ProjectTitle;
            }
            set
            {
                if (_ProjectTitle == value)
                    return;

                _ProjectTitle = value;
                OnPropertyChanged("ProjectTitle");
            }
        }

        string _LogoUrl;
        public string LogoUrl
        {
            get
            {
                return _LogoUrl;
            }
            set
            {
                if (_LogoUrl == value)
                    return;

                _LogoUrl = value;
                OnPropertyChanged("LogoUrl");
            }
        }

        #endregion

        public bool IsValid
        {
            get
            {
                if (owner != null && !owner.ValidateBindings())
                    return false;

                switch (WebClientType)
                {
                    case WebClientType.Html5:
                        return UriProjectPath.Length > 0 && WebSite.Length > 0 && AliasName.Length > 0 && DeployPath.Length > 0 && ApplicationPoolName.Length > 0 && IndentityName.Length > 0 && DeployPath.Length > 0;
                    default:
                        return WebSite.Length > 0 && AliasName.Length > 0 && DeployPath.Length > 0 && ApplicationPoolName.Length > 0 && IndentityName.Length > 0 && DeployPath.Length > 0;
                }
            }
        }

        bool _IsPublished = false;
        public bool IsPublished
        {
            get
            {
                return _IsPublished;
            }
            set
            {
                if (_IsPublished == value)
                    return;
                _IsPublished = value;
                OnPropertyChanged("IsPublished");
            }
        }

        DataGridElement[] _DataGridSources;
        public DataGridElement[] DataGridSources
        {
            get
            {
                if (_DataGridSources == null)
                    _DataGridSources = new DataGridElement[0];

                return _DataGridSources;
            }
            set
            {
                if (_DataGridSources == value)
                    return;

                _DataGridSources = value;
                OnPropertyChanged("DataGridSources");
            }
        }

        DataGridElement[] _DataAnalysisSources;
        public DataGridElement[] DataAnalysisSources
        {
            get
            {
                if (_DataAnalysisSources == null)
                    _DataAnalysisSources = new DataGridElement[0];

                return _DataAnalysisSources;
            }
            set
            {
                if (_DataAnalysisSources == value)
                    return;

                _DataAnalysisSources = value;
                OnPropertyChanged("DataAnalysisSources");
            }
        }

        DataGridElement[] _DashBoardSources;
        public DataGridElement[] DashBoardSources
        {
            get
            {
                if (_DashBoardSources == null)
                    _DashBoardSources = new DataGridElement[0];

                return _DashBoardSources;
            }
            set
            {
                if (_DashBoardSources == value)
                    return;

                _DashBoardSources = value;
                OnPropertyChanged("DashBoardSources");
            }
        }

        public bool IsDataSourceProjectBase
        {
            get 
            {
                return XpoHelpers.XpoHelper.IsDataSource(UriProjectPath);
            }
        }
        #endregion

        #region Internal Members
        internal System.Windows.Window owner = null; 
        #endregion

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        #region IDataErrorInfo
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
        #endregion

        #region Private or Protected Methods
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "AliasName")
            {
                if (AliasName == null || String.IsNullOrEmpty(AliasName.Trim()))
                    return Properties.Resources.MissingValue;
            }
            else if (propertyName == "DeployPath")
            {
                if (!DirectoryHelper.IsValidDirectory(DeployPath, false))
                    return Properties.Resources.InvalidDeployPath;
                else if (!IsDataSourceProjectBase)
                {
                    string destPath = string.Format("{0}\\{1}", DeployPath, ProjectName);
                    if (destPath.StartsWith(System.IO.Path.GetDirectoryName(UriProjectPath), StringComparison.InvariantCultureIgnoreCase))
                        return Properties.Resources.RecursiveDeployPath;
                }
            }
            else if (propertyName == "ApplicationPoolName")
            {
                if (ApplicationPoolName == null || String.IsNullOrEmpty(ApplicationPoolName.Trim()))
                    return Properties.Resources.MissingValue;
            }
            else if (propertyName == "IndentityName")
            {
                if (IndentityName == null || String.IsNullOrEmpty(IndentityName.Trim()))
                    return Properties.Resources.MissingValue;
            }

            return null;
        }

        static String NormalizeAliasName(string aliasName)
        {
            var normalizedAliasName = aliasName;
            var index = normalizedAliasName.LastIndexOf(' ');
            if (index != -1)
                normalizedAliasName = aliasName.Substring(index + 1);
            else
                normalizedAliasName = aliasName;

            string regex = string.Format("[{0};]", System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars())));
            normalizedAliasName = System.Text.RegularExpressions.Regex.Replace(normalizedAliasName, regex, "_");
            
            return normalizedAliasName;
        }
        #endregion
    }
}
