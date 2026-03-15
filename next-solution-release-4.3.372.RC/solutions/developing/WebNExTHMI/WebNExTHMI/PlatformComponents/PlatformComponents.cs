using DataLoggerModel.Helpers;
using Newtonsoft.Json;
using DataReader.Helpers;
using DataReader.SchemaInfo;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DocumentManager.ComponentService;
using HistoricalEventControl;
using Newtonsoft.Json.Linq;
using OPCUAViewModel;
using ReportManager.ReportService;
using ReportSettings.Documents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using UFUAHistorianModel;
using Utilities;
using WPFPenHelpers;
using WPFUtilities;
using UFUAEditor.ComponentService;
using DevExpress.XtraPrinting;
using SmtpSender;
using System.Threading;
using MjpegProcessor;
using System.Xml;
using System.Xml.Linq;
using LinqStatistics;
using WPFUtilities.HistoricalHelpers;
using UFProjectManager;
using ExternalAuthentication.Model;
using ExternalAuthentication.Services;


namespace WebNExTHMI.PlatformComponents
{
    public class AuditDataItemModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public double? dValue { get; set; }
        public string ValueBefore { get; set; }
        public double? dValueBefore { get; set; }
        public string RecordDateTime { get; set; }
        public string RecordDateTimeMilliseconds { get; set; }
        public string SourceTimeStamp { get; set; }
        public string SourceTimeStampMilliseconds { get; set; }
        public ushort SourcePicoseconds { get; set; }
        public string ServerTimeStamp { get; set; }
        public string ServerTimeStampMilliseconds { get; set; }
        public ushort ServerPicoseconds { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
        public string Reason { get; set; }
        public Tuple<int, int> DateTimeTicks { get; set; }
    }

    public class ServerAuditDataItemModel
    {
        public List<AuditDataItemModel> Items { get; set; } = new List<AuditDataItemModel>();
        public long StartDateTimeTicks { get; set; }
        public long EndDateTimeTicks { get; set; }

        public ServerAuditDataItemModel() { }
    }

    public class ServerAuditLogItem
    {
        public List<AuditLogItem> Items { get; set; } = new List<AuditLogItem>();
        public long StartDateTimeTicks { get; set; }
        public long EndDateTimeTicks { get; set; }

        public ServerAuditLogItem() { }
    }

    public class LogItem
    {
        public int Item;
        public string Logger;
        public long TimeStamp;
        public string Level;
        public string Thread;
        public string Message;
        public string NDC;
        public string Identity;
        public string Details;
        public string MachineName;
        public string HostName;
        public string UserName;
        public string App;
        public LogItem()
        { }
    }

    public class LogItems
    {
        public bool SingleLineStructure { get; set; } = false;
        public List<LogItem> Items { get; set; } = new List<LogItem>();
        public List<string> Loggers { get; set; } = new List<string>();
        public string Schema { get; set; } = "log4j";
        public LogItems() { }
    }
    public class DecoderData
    {
        IPCameraFrame latestFrame;
        long maxKeepAliveMS;
        long latestRequestTimestamp { get; set; }
        public MjpegDecoder Decoder { get; set; }
        public string UriID { get; private set; }
        public Uri Uri { get; private set; }
        public string User { get; private set; }
        public string Password { get; private set; }
        public int CloseTimeout { get; private set; }
        public string LatestError { get; set; }
        public IPCameraFrame LatestFrame {
            get {
                return latestFrame;
            }
            set {
                latestFrame = value;
            }
        }
        public DecoderData(MjpegDecoder decoder, string uriID, Uri uri, string user, string password, int closeTimeout, long maxKeepAliveMS = 0)
        {
            Decoder = decoder;
            this.maxKeepAliveMS = maxKeepAliveMS;
            UriID = uriID;
            Uri = uri;
            User = user;
            Password = password;
            CloseTimeout = closeTimeout;
            UpdateLatestRequestTimestamp();
        }
        public void UpdateLatestRequestTimestamp()
        {
            latestRequestTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        public bool IsKeepAliveExpired()
        {
            if (maxKeepAliveMS <= 0)
                return false;

            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - latestRequestTimestamp > maxKeepAliveMS;
        }
    }
    public class IPCameraFrame
    {
        public string FrameData { get; set; }
        public string FrameHash { get; set; }
        public IPCameraFrame()
        {

        }
        public IPCameraFrame(string frameData, string frameHash)
        {
            FrameData = frameData;
            FrameHash = frameHash;
        }
        public bool HasDifferentHash(string hash)
        {
            return hash != FrameHash;
        }
    }

    static public class PlatformComponents
    {
        static string tileDetailsFileName = "Tile.details";
        static ComponentHost componentHost = new ComponentHost();
        static PluginServices Plugins = new PluginServices();

        static UFProjectManager.UFProjectDocument projectDocument;

        public static Dictionary<String, String> historiansettings = new Dictionary<String, String>();
        public static Dictionary<String, String> eventsettings = new Dictionary<String, String>();
        public static Dictionary<String, String> serverentitysettings = new Dictionary<String, String>();
        public static Dictionary<String, String> schedulersettings = new Dictionary<String, String>();
        public static string logsDir = "Log";
        public static List<string> logSupportedExtensions = new List<string>() { ".xml", ".log" };
        static Dictionary<String, DecoderData> mapDecoders = new Dictionary<string, DecoderData>();
        static Dictionary<String, int> activeMjpegDecoders = new Dictionary<string, int>();
        private static System.Timers.Timer IPCameraKeepAliveTimer;

        #region Historical Data Declarations
        static Dictionary<String, IDataLayer> MapToHistoricalDataLayer = new Dictionary<String, IDataLayer>();
        static Dictionary<String, IDataLayer> MapNodeIdsToHistoricalDataLayer = new Dictionary<String, IDataLayer>();
        static Dictionary<String, IDataLayer> MapToDataLoggerDataLayer = new Dictionary<String, IDataLayer>();
        static IDictionary<String, String> MapToDatalogerConnectsions;
        static IDictionary<String, String> MapToHistoricalConnectsions;
        static IDictionary<String, String> MapNodeIdsToHistoricalConnectsions = new Dictionary<String, String>();
        static Dictionary<string, Dictionary<String, DataLoggerSettings>> MapDlrSettings = new Dictionary<string, Dictionary<string, DataLoggerSettings>>();
        static object lockObject = new object();
        static readonly int keepAliveTimerTick = 2000;
        #endregion

        static public void Init()
        {
            componentHost.Components.Add(UFProjectManagerComponent);
            componentHost.Components.Add(ClientEditorManagerComponent);
            componentHost.Components.Add(UFUserEditorComponent);
            componentHost.Components.Add(StringEditorComponent);
            componentHost.Components.Add(UFUAEditorComponent);
            componentHost.Components.Add(ScreenComponent);
            componentHost.Components.Add(EventEditorComponent);
            componentHost.Components.Add(RecipeEditorComponent);

            SysVariables.SysVariables.GetSysVariables().Initialize();
            TempVariablesManager.TempVariables.GetTempVariables().Initialize();

            //Plugins.FindPlugins(String.Format("{0}\\bin\\DataSinks\\", AppDomain.CurrentDomain.BaseDirectory));

            //foreach (var pluginOn in Plugins.AvailablePlugins)
            //{
            //    try
            //    {
            //        ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
            //    }
            //    catch (Exception ex)
            //    {
            //        // log.Error(Properties.Resources.FailedToInitializePlugin, ex);
            //    }
            //}
        }

        static public bool OpenProject(String filename)
        {
            projectDocument = UFProjectManager.UFProjectDocument.FromFile(filename, UFProjectManagerComponent);
            if (projectDocument == null)
                throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

            UpdateProjectSettings(projectDocument);

            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
#if !DEBUG
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
            };
#endif

            ScreenComponent.LoadRequiredAssemblies();
            StringEditorComponent.LoadRuntimeStrings(projectDocument);
            if (projectDocument.GetWholeDocumentLists((IDocumentManager)RecipeEditorComponent).Count > 0)
                RecipeEditorComponent.PreSubscribeServerSession(projectDocument);

            // if (projectDocument.StartType == StartType.TilePage)
            InitProjectTilesMap();

            EventEditorComponent.Execute(new Uri(projectDocument.ProjectFolder, UriKind.RelativeOrAbsolute), projectDocument, ExecutionMode.Synchro, projectDocument);

            return true;
        }

        static public bool OpenARMappingProject(String filename)
        {
            return true;
        }

        static String RemoveExtension(String file)
        {
            var ext = System.IO.Path.GetExtension(file);
            return String.IsNullOrEmpty(ext) ? file : file.Replace(ext, "");
        }

        static public String PrependScreenTypeLabel(String name)
        {
            if (!name.StartsWith(String.Format("{0}/", ScreenComponent.TypeLabel)))
                name = String.Format("{0}/{1}", ScreenComponent.TypeLabel, name);
            return name;
        }

        static public String CompleteWithExtensionScreen(String file)
        {
            if (file.IndexOf(ScreenComponent.FileType) != -1)
                return file;
            return String.Format("{0}{1}", file, ScreenComponent.FileType);
        }

        static public String CompleteWithExtensionReport(String file)
        {
            if (file.IndexOf(ReportComponent.FileType) != -1)
                return file;
            return String.Format("{0}{1}", file, ReportComponent.FileType);
        }

        static public Tuple<String, IDocument> MakeAbsolute(String name, String callerScreenName = null)
        {
            int found = name.IndexOfAny(new char[] { '\\', '/' });
            if (found == -1)
                name = String.Format("{0}{1}{2}", ScreenComponent.TypeLabel, Path.DirectorySeparatorChar, name);
            UFProjectManager.UFProjectDocument project = null;
            if (callerScreenName != null)
                project = projectDocument.UpdateParentFromUri(new Uri(callerScreenName, UriKind.RelativeOrAbsolute), ScreenComponent) as UFProjectManager.UFProjectDocument;
            if (project == null)
                project = projectDocument;
            var currentPage = project.MakeAbosoluteUri(new Uri(ConvertToMobile(name), UriKind.RelativeOrAbsolute));
            var parent = project.UpdateParentFromUri(currentPage, ScreenComponent);

            return new Tuple<String, IDocument>(RemoveExtension(currentPage.GetPathString()), parent);
        }

        static String ConvertToMobile(String uri)
        {
            if (!IsMobile || projectDocument == null)
                return uri;

            var name = System.IO.Path.GetFileNameWithoutExtension(uri);
            var newName = String.Format("{0}\\{1}{2}{3}", System.IO.Path.GetDirectoryName(uri), name, "Mobile",
                System.IO.Path.GetExtension(uri));
            var uriNew = projectDocument.MakeAbosoluteUri(new Uri(newName, UriKind.RelativeOrAbsolute), ScreenComponent);
            if (System.IO.File.Exists(uriNew.OriginalString))
                return RemoveExtension(uriNew.OriginalString);
            return RemoveExtension(uri);
        }

        static void UpdateProjectSettings(UFProjectManager.UFProjectDocument doc, UFProjectManager.UFProjectDocument parent = null)
        {
            doc.Parent = parent;
            doc.UpdateSessionSettings();

            historiansettings[doc.Title] = UFUAEditorComponent.GetHistorianDefaultConnection(doc);
            eventsettings[doc.Title] = UFUAEditorComponent.GetEventDefaultConnection(doc);
            serverentitysettings[doc.Title] = UFUAEditorComponent.GetServerEntityReference(doc);
            schedulersettings[doc.Title] = SchedulerComponent.GetServerEntityReference(doc);

            MapToDataLoggerDataLayer.Clear();
            MapToHistoricalDataLayer.Clear();
            MapNodeIdsToHistoricalConnectsions.Clear();
            MapNodeIdsToHistoricalDataLayer.Clear();

            doc.ListChildProjectPaths.ForEach(project =>
            {
                var abs = doc.MakeAbosoluteUri(project, UFProjectManagerComponent);
                if (abs != null)
                {
                    using (var childProject = UFProjectManager.UFProjectDocument.FromFile(abs.GetPathString(), UFProjectManagerComponent))
                    {
                        if (childProject != null)
                            UpdateProjectSettings(childProject, doc);
                    }
                }
            });
        }

        static public bool LogingRequired
        {
            get; set;
        }

        static public bool ShowDebugWindow
        {
            get; set;
        }

        static public bool ForceMainPageAR
        {
            get; set;
        }

        static public bool ARScanQRCodeOnly
        {
            get; set;
        }
        static public bool ARScanQRCode
        {
            get; set;
        }
        static public int ARScanQRToleranceMS
        {
            get; set;
        }
        static public float ARQRCodeScaleFactor
        {
            get; set;
        }
        static public string ActiveSessionCountVariableName
        {
            get; set;
        }

        static public int ScreenDelayUnloadMSecs
        {
            get; set;
        }

        static public int ScreenAliveTimeoutSecs
        {
            get; set;
        }

        static public String MapProvider
        {
            get; set;
        }

        static public String MapProviderKey
        {
            get; set;
        }

        static public String MapLayers
        {
            get; set;
        }

        static public int MapZoomFactor
        {
            get; set;
        }

        static public int MapMaxZoomFactor
        {
            get; set;
        }

        static public string MapBounds
        {
            get; set;
        }
        static public bool LegacyMapView
        {
            get; set;
        }
        static public string MapViewTilesURL
        {
            get; set;
        }
        static public string MapViewTilesOptions
        {
            get; set;
        }

        static public String TensorFlowConnectTo
        {
            get; set;
        }

        static public String TensorFlowBindTo
        {
            get; set;
        }

        static public int TensorFlowTimeoutSec
        {
            get; set;
        }

        static public bool IsMobile
        {
            get; set;
        }

        static public int CacheDelay
        {
            get; set;
        }

        static public bool StaticToolboxLoading
        {
            get; set;
        }
        static public int IPCameraKeepAliveTimeout
        {
            get; set;
        }

        static public int ServerInvokeMinFreqOnDrag
        {
            get; set;
        }

        static public String GetProjectTitle()
        {
            return projectDocument.GetTitle();
        }

        static public String GetProjectTheme()
        {
            return projectDocument.GetTheme().ToString();
        }

        static public IDocument GetProjectDocument()
        {
            return projectDocument;
        }

        static public String GetSessionString()
        {
            return projectDocument.Title;
        }


        static public int GetConfigurationStartType()
        {
            var startType = (int)projectDocument.StartType;
#if !DEBUG
            if (projectDocument.StartType == StartType.GeoPage)
            {
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
                if (!state)
                {
                    return (int)StartType.TilePage;
                }
            }
#endif
            return startType;
        }

        static public String GetConfigurationDetectedScreen(String className)
        {
            if (className == null)
                return null;

            var uri = new Uri(className, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                return projectDocument.MakeRelativeUri(uri).GetPathString();
            else
                return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public String GetConfigurationStartupScreen()
        {
            var uri = projectDocument.GetStartupScreen(ScreenComponent);
            if (uri == null)
                return null;

            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public String GetConfigurationTopScreen()
        {
            var uri = projectDocument.GetTopScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public double GetConfigurationTopScreenHeight()
        {
            var uri = projectDocument.GetTopScreen(ScreenComponent);
            if (uri == null)
                return 0;

            using (var screenDocument = ScreenSettings.ScreenDocument.FromFile(uri.GetPathString(), projectDocument))
            {
                return screenDocument.Height;
            }
        }

        static public String GetConfigurationBottomScreen()
        {
            var uri = projectDocument.GetBottomScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public double GetConfigurationBottomScreenHeight()
        {
            var uri = projectDocument.GetBottomScreen(ScreenComponent);
            if (uri == null)
                return 0;

            using (var screenDocument = ScreenSettings.ScreenDocument.FromFile(uri.GetPathString(), projectDocument))
            {
                return screenDocument.Height;
            }
        }

        static public String GetConfigurationLeftScreen()
        {
            var uri = projectDocument.GetLeftScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public double GetConfigurationLeftScreenWidth()
        {
            var uri = projectDocument.GetLeftScreen(ScreenComponent);
            if (uri == null)
                return 0;

            using (var screenDocument = ScreenSettings.ScreenDocument.FromFile(uri.GetPathString(), projectDocument))
            {
                return screenDocument.Width;
            }
        }

        static public String GetConfigurationRightScreen()
        {
            var uri = projectDocument.GetRightScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public double GetConfigurationRightScreenWidth()
        {
            var uri = projectDocument.GetRightScreen(ScreenComponent);
            if (uri == null)
                return 0;

            using (var screenDocument = ScreenSettings.ScreenDocument.FromFile(uri.GetPathString(), projectDocument))
            {
                return screenDocument.Width;
            }
        }

        static public bool GetRestoreLastOpenScreenAndZoom()
        {
            return projectDocument.RestoreLastOpenScreenAndZoom;
        }

        static public String GetConfigurationAppBarTopScreen()
        {
            var uri = projectDocument.GetAppBarTopScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }
        static public String GetConfigurationAppBarBottomScreen()
        {
            var uri = projectDocument.GetAppBarBottomScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }
        static public String GetConfigurationAppBarLeftScreen()
        {
            var uri = projectDocument.GetAppBarLeftScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }
        static public String GetConfigurationAppBarRightScreen()
        {
            var uri = projectDocument.GetAppBarRightScreen(ScreenComponent);
            if (uri == null)
                return null;
            return projectDocument.MakeRelativeUri(uri, ScreenComponent).GetPathString();
        }

        static public String GetResetStatisticsNodeId()
        {
            var opcString = UFUAEditorComponent.GetNodeIdEntityReference(projectDocument,
                UFUAServerInfo.BrowserNames.ResetStatistics,
                UFUAServerInfo.Guids.RootTagsGuid.ToString());
            return opcString;
        }

        static public String GetToggleAlarmSoundNodeId()
        {
            var opcString = UFUAEditorComponent.GetNodeIdEntityReference(projectDocument,
            String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName,
            UFUAServerInfo.BrowserNames.AlarmsSoundStateName),
            UFUAServerInfo.Guids.SystemTagsGuid.ToString());
            return opcString;
        }

        static public String GetAlarmsSoundBuzzingNameNodeId()
        {
            var opcString = UFUAEditorComponent.GetNodeIdEntityReference(projectDocument,
            String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName,
            UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingName),
            UFUAServerInfo.Guids.SystemTagsGuid.ToString());
            return opcString;
        }

        static public String GetActiveSessionsWebHMINameNodeId()
        {
            var opcString = UFUAEditorComponent.GetNodeIdEntityReference(projectDocument,
            UFUAServerInfo.BrowserNames.ActiveSessionsWebHMIName,
            UFUAServerInfo.Guids.SystemTagsGuid.ToString());
            return opcString;
        }

        static public bool IsRemoteServer()
        {
            var applicationName = UFUAEditorComponent.GetDefApplicationName(projectDocument);
            return RealTimeConnectionManagerViewModel.IsSessionServerRemote(projectDocument.Title, applicationName);
        }

        static public int GetConfigurationAutoLogoutSeconds(String user = null)
        {
            return UFUserEditorComponent.GetAutoLogoutSeconds(projectDocument, user);
        }

        static public int GetRoleAutoLogoutSeconds(String role = null)
        {
            return UFUserEditorComponent.GetRoleAutoLogoutSeconds(projectDocument, role);
        }

        static public int GetDaysLeftPasswordExpires(String user)
        {
            return UFUserEditorComponent.GetDaysLeftPasswordExpires(projectDocument, user);
        }

        static public bool GetConfigurationEnableUserManager()
        {
            return UFUserEditorComponent.GetEnableUserManager(projectDocument);
        }

        static public int GetConfigurationMinRequiredPasswordLength()
        {
            return UFUserEditorComponent.GetMinRequiredPasswordLength(projectDocument);
        }

        static public IEnumerable<String> GetAvailableLanguages()
        {
            return StringEditorComponent.GetListAvailableCultures(projectDocument);
        }

        static public String GetProjectCulture()
        {
            return projectDocument.GetProjectCulture();
        }

        static public bool GetForceStartupCultureName()
        {
            return projectDocument.ForceStartupCultureName;
        }

        static public IDictionary<String, String> GetListStringForCulture(String culture)
        {
            return StringEditorComponent.GetListStringForCulture(projectDocument, culture);
        }

        static public int ValidateUser(String user, String password)
        {
            return UFUserEditorComponent.VerifyUser(projectDocument, user, password);
        }

        static public String GetUserRole(String user)
        {
            return UFUserEditorComponent.GetUserRole(projectDocument, user);
        }

        static public int GetUserAccessLevel(String user)
        {
            return UFUserEditorComponent.GetUserAccessLevel(projectDocument, user);
        }

        static public int GetRoleAccessLevel(String role)
        {
            return UFUserEditorComponent.GetRoleAccessLevel(projectDocument, role);
        }

        static public String GetUserCultureName(String user)
        {
            return UFUserEditorComponent.GetUserCultureName(projectDocument, user);
        }

        static public String GetRoleCultureName(String role)
        {
            return UFUserEditorComponent.GetRoleCultureName(projectDocument, role);
        }

        static public int GetUserAccessMask(String user)
        {
            return UFUserEditorComponent.GetUserAccessMask(projectDocument, user);
        }

        static public int GetRoleAccessMask(String role)
        {
            return UFUserEditorComponent.GetRoleAccessMask(projectDocument, role);
        }

        static public String GetUserElectronicSignature(String user)
        {
            return UFUserEditorComponent.GetUserElectronicSignature(projectDocument, user);
        }

        static public bool UpdateUser(String user, String oldpassword, String newpassword)
        {
            return UFUserEditorComponent.UpdateUser(projectDocument, user, oldpassword, newpassword);
        }

        static public bool IsOldPassword(String user, String newpassword)
        {
            return UFUserEditorComponent.GetIsOldPassword(projectDocument, user, newpassword);
        }

        static public List<String> PreloadedToolboxComponents
        {
            get;
            set;
        } = new List<string>();

        static public ExternalIdPUserSettings GetExternalAuthenticationConfiguration()
        {
            var externalIdPSettingsDictionary = UFUserEditorComponent.GetExternalAuthenticationSettings(projectDocument);
            var externalIdPUserSettings = new ExternalIdPUserSettings();

            if (externalIdPSettingsDictionary != null && externalIdPSettingsDictionary.Count > 0)
            {
                var convertGeneralSettingsToExternalIdPUserSettings = new ConvertGeneralSettingsToExternalIdPUserSettings();
                externalIdPUserSettings =
                        convertGeneralSettingsToExternalIdPUserSettings.Execute(externalIdPSettingsDictionary);
            }
            
            return externalIdPUserSettings;
        }

        static public string GetExternalAuthenticationUserRole(String role)
        {
            return UFUserEditorComponent.GetExternalAuthenticationUserRole(projectDocument, role);
        }

        #region Components
        static UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        static public UFProjectManager.ComponentService.UFProjectManagerComponent UFProjectManagerComponent
        {
            get
            {
                return ufProjectManagerComponent;
            }
        }

        static ClientEditor.ComponentService.ClientEditorManagerComponent clientEditorManagerComponent = new ClientEditor.ComponentService.ClientEditorManagerComponent();
        static public ClientEditor.ComponentService.ClientEditorManagerComponent ClientEditorManagerComponent
        {
            get
            {
                return clientEditorManagerComponent;
            }
        }

        static UFUserEditor.ComponentService.UFUserEditorManagerComponent ufUserEditorComponent = new UFUserEditor.ComponentService.UFUserEditorManagerComponent();
        static public UFUserEditor.ComponentService.UFUserEditorManagerComponent UFUserEditorComponent
        {
            get
            {
                return ufUserEditorComponent;
            }
        }

        static StringManager.ComponentService.StringEditorManagerComponent stringEditorComponent = new StringManager.ComponentService.StringEditorManagerComponent();
        static public StringManager.ComponentService.StringEditorManagerComponent StringEditorComponent
        {
            get
            {
                return stringEditorComponent;
            }
        }

        static UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        static public UFUAEditor.ComponentService.UFUAEditorManagerComponent UFUAEditorComponent
        {
            get
            {
                return ufuaEditorComponent;
            }
        }

        static UFEventEditor.ComponentService.EventEditorManagerComponent ufEventEditorComponent = new UFEventEditor.ComponentService.EventEditorManagerComponent();
        static public UFEventEditor.ComponentService.EventEditorManagerComponent EventEditorComponent
        {
            get
            {
                return ufEventEditorComponent;
            }
        }

        static MSEditor.ComponentService.SchedulerEditorManagerComponent schedulerComponent = new MSEditor.ComponentService.SchedulerEditorManagerComponent();
        static public MSEditor.ComponentService.SchedulerEditorManagerComponent SchedulerComponent
        {
            get
            {
                return schedulerComponent;
            }
        }

        static ScreenManager.ComponentService.ScreenManagerComponent screenComponent = new ScreenManager.ComponentService.ScreenManagerComponent();
        static public ScreenManager.ComponentService.ScreenManagerComponent ScreenComponent
        {
            get
            {
                return screenComponent;
            }
        }

        static ReportManager.ComponentService.ReportManagerComponent reportComponent = new ReportManager.ComponentService.ReportManagerComponent();
        static public ReportManager.ComponentService.ReportManagerComponent ReportComponent
        {
            get
            {
                return reportComponent;
            }
        }

        static UFRecipeEditor.ComponentService.RecipeEditorManagerComponent recipeEditorComponent = new UFRecipeEditor.ComponentService.RecipeEditorManagerComponent();
        static public UFRecipeEditor.ComponentService.RecipeEditorManagerComponent RecipeEditorComponent
        {
            get
            {
                return recipeEditorComponent;
            }
        }
        #endregion

        #region Tiles

        static public List<TileInfo> listTiles = new List<TileInfo>();
        static public List<TileInfo> listTilesFlat = new List<TileInfo>();
        static public List<TileInfo> listMapPinsFlat = new List<TileInfo>();
        static public Dictionary<String, IList<TileInfo>> mapTiles = new Dictionary<String, IList<TileInfo>>();
        static public string tileViewBackground;
        static List<Tuple<string, string>> screensInfo;

        static TileInfo AddTileInfo(IScreenController controller, Uri uri, List<TileInfo> parenttileInfo, JToken tileDetails)
        {
            var tileInfo = new TileInfo() { Section = controller.GetTitle() };
            uri = projectDocument.MakeRelativeUri(uri, ScreenComponent);
            if (projectDocument.IsVisible(uri))
            {
                // tileInfo.Name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                tileInfo.Name = System.IO.Path.ChangeExtension(uri.GetPathString(), null);
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace('/', ' ');
                tileInfo.Name = tileInfo.Name.Replace('\\', ' ');
                if (tileDetails != null)
                {
                    try
                    {
                        tileViewBackground = (string)tileDetails["Color"];
                        tileInfo.Color = (from tile in tileDetails["ScreenObjects"] where (string)tile["Path"] == uri.ToString() select (string)tile["Color"]).FirstOrDefault();
                    }
                    catch { }
                }

                //var color = controller.GetIdentityColor(uri);

                /*
                tileInfo.ColorValue = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

                var colorName = ColorHelpers.GetColorName(color);
                if (colorName == null)
                    tileInfo.Color = "bg-color-darken";
                else if (colorName.Contains("Blue"))
                {
                    if (colorName.Contains("Dark"))
                        tileInfo.Color = "bg-color-blueDark";
                    else
                        tileInfo.Color = "bg-color-blue";
                }
                else if (colorName.Contains("Green"))
                {
                    if (colorName.Contains("Dark"))
                        tileInfo.Color = "bg-color-greenDark";
                    else
                        tileInfo.Color = "bg-color-green";
                }
                else if (colorName.Contains("Red"))
                {
                    tileInfo.Color = "bg-color-red";
                }
                else if (colorName.Contains("Yellow"))
                {
                    tileInfo.Color = "bg-color-yellow";
                }
                else if (colorName.Contains("Orange"))
                {
                    tileInfo.Color = "bg-color-orange";
                }
                else if (colorName.Contains("Pink"))
                {
                    tileInfo.Color = "bg-color-pink";
                }
                else if (colorName.Contains("Purple"))
                {
                    tileInfo.Color = "bg-color-purple";
                }
                else if (colorName.Contains("White"))
                {
                    tileInfo.Color = "bg-color-white";
                }
                else
                    tileInfo.Color = "bg-color-darken";
                */
                tileInfo.Url = uri.GetPathString();

                switch (controller.TileSize(uri))
                {
                    case DocumentManager.ComponentService.TileSize.ExtraSmall:
                        tileInfo.IsExtraSmall = true;
                        break;
                    case DocumentManager.ComponentService.TileSize.Large:
                        tileInfo.IsLarge = true;
                        break;
                    case DocumentManager.ComponentService.TileSize.ExtraLarge:
                        tileInfo.IsExtraLarge = true;
                        break;
                }

                var image = new Uri(System.IO.Path.ChangeExtension(uri.GetPathString(), "png"), UriKind.RelativeOrAbsolute);
                var uri2 = projectDocument.MakeAbosoluteUri(image, ScreenComponent);
                if (uri2 != null && uri2.IsAbsoluteUri)
                {
#if !DEBUG
                    uri2 = projectDocument.MakeRelativeUri(uri2, AppDomain.CurrentDomain.BaseDirectory);
                    if (uri2 != null)
#endif
                    tileInfo.ImageUrl = uri2.ToString();
                }
                /*
                else
                {
                    uri2 = projectDocument.ExportRelativeUriToTemporaryFile(uri2, "png");
                    if (uri2 != null)
                    {
                        tileInfo.ImageUrl = uri2.ToString();
                        temporaryFiles.Add(uri2.GetPathString());
                    }
                }
                */
                tileInfo.Section = controller.GetTitle();
                //tileInfo.Section = tileInfo.Name;
                //tileInfo.Section = tileInfo.Section.Replace('/', '_');
                //tileInfo.Section = tileInfo.Section.Replace('\\', '_');

                tileInfo.HasGeoCoordinates = controller.GetHasGeoCoordinates(uri);
                tileInfo.Latitude = controller.GetLatitude(uri);
                tileInfo.Longitude = controller.GetLongitude(uri);

                tileInfo.UsersVisibility = controller.GetUsersVisibility(uri);
                tileInfo.RolesVisibility = controller.GetRolesVisibility(uri);

                tileInfo.MinZoomLevel = controller.GetMapMinZoomLevelVisibility(uri);
                tileInfo.MaxZoomLevel = controller.GetMapMaxZoomLevelVisibility(uri);

                tileInfo.LatTag = controller.GetLatitudeTag(uri);
                tileInfo.LonTag = controller.GetLongitudeTag(uri);

                parenttileInfo.Add(tileInfo);
                listTilesFlat.Add(tileInfo);

                
                if (tileInfo.HasGeoCoordinates)
                {
#if !DEBUG
                    var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
                    if (state == true)
                        listMapPinsFlat.Add(tileInfo);
#else
                        listMapPinsFlat.Add(tileInfo);
#endif
                }
            }

            return tileInfo;
        }

        static void AddTileInfos(IScreenController controller, List<TileInfo> list)
        {
            var tileDetailsPath = String.Format("{0}{1}{2}{1}{3}", Path.GetDirectoryName(projectDocument.ProjectPath), Path.DirectorySeparatorChar,  SpecialFolders.Documents.ToString(), tileDetailsFileName);
            JToken tileDetails = null;
            try
            {
                tileDetails = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(tileDetailsPath));
            }
            catch (Exception ex)
            {

            }
            controller.GetScreenLists(ScreenComponent).ForEach(uri =>
            {
                AddTileInfo(controller, uri, list, tileDetails);
            });

            controller.GetScreenControllers(ScreenComponent).ForEach(c =>
            {
                if (c != controller)
                {
                    var ti = new TileInfo() { Section = c.GetTitle() };
                    list.Add(ti);
                    AddTileInfos(c, ti.childs);
                }
            });
        }

        static void InitProjectTilesMap()
        {
            projectDocument.GetScreenControllers(ScreenComponent).ForEach(controller =>
            {
                AddTileInfos(controller);

                var tileinfo = new TileInfo() { Section = controller.GetTitle() };
                listTiles.Add(tileinfo);
                AddTileInfos(controller, tileinfo.childs);
            });
            listTilesFlat = listTilesFlat.OrderBy(tile => tile.Url).ToList();
        }

        static internal List<Tuple<string, string>> GetProjectScreensList()
        {
            if (screensInfo != null)
                return screensInfo;

            screensInfo = new List<Tuple<string, string>>();
            projectDocument.GetScreenControllers(ScreenComponent).ForEach(controller =>
            {
                AddScreenInfos(controller);
            });
            return screensInfo;
        }

        static void AddScreenInfos(IScreenController controller)
        {
            controller.GetScreenLists(ScreenComponent).ForEach(uri =>
            {
                uri = projectDocument.MakeRelativeUri(uri, ScreenComponent);
                if (projectDocument.IsVisible(uri))
                {
                    string screenName;
                    screenName = Path.ChangeExtension(uri.GetPathString(), null);
                    screenName = screenName.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), "");
                    screenName = screenName.Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), "");
                    screensInfo.Add(new Tuple<string, string>(screenName, uri.ToString()));
                }
            });

            controller.GetScreenControllers(ScreenComponent).ForEach(c =>
            {
                if (c != controller)
                    AddScreenInfos(c);
            });
        }

        static void AddTileInfo(IScreenController controller, Uri uri)
        {
            var tileInfo = new TileInfo();
            uri = projectDocument.MakeRelativeUri(uri, ScreenComponent);
            if (projectDocument.IsVisible(uri))
            {
                // tileInfo.Name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                tileInfo.Name = System.IO.Path.ChangeExtension(uri.GetPathString(), null);
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}/", ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}\\", ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace('/', ' ');
                tileInfo.Name = tileInfo.Name.Replace('\\', ' ');

                var color = controller.GetIdentityColor(uri);
                /*
                var colorName = ColorHelpers.GetColorName(color);
                if (colorName == null)
                    tileInfo.Color = "bg-color-darken";
                else if (colorName.Contains("Blue"))
                {
                    if (colorName.Contains("Dark"))
                        tileInfo.Color = "bg-color-blueDark";
                    else
                        tileInfo.Color = "bg-color-blue";
                }
                else if (colorName.Contains("Green"))
                {
                    if (colorName.Contains("Dark"))
                        tileInfo.Color = "bg-color-greenDark";
                    else
                        tileInfo.Color = "bg-color-green";
                }
                else if (colorName.Contains("Red"))
                {
                    tileInfo.Color = "bg-color-red";
                }
                else if (colorName.Contains("Yellow"))
                {
                    tileInfo.Color = "bg-color-yellow";
                }
                else if (colorName.Contains("Orange"))
                {
                    tileInfo.Color = "bg-color-orange";
                }
                else if (colorName.Contains("Pink"))
                {
                    tileInfo.Color = "bg-color-pink";
                }
                else if (colorName.Contains("Purple"))
                {
                    tileInfo.Color = "bg-color-purple";
                }
                else if (colorName.Contains("White"))
                {
                    tileInfo.Color = "bg-color-white";
                }
                else
                    tileInfo.Color = "bg-color-darken";
                */

                tileInfo.Url = uri.GetPathString();

                switch (controller.TileSize(uri))
                {
                    case DocumentManager.ComponentService.TileSize.ExtraSmall:
                        tileInfo.IsExtraSmall = true;
                        break;
                    case DocumentManager.ComponentService.TileSize.Large:
                        tileInfo.IsLarge = true;
                        break;
                    case DocumentManager.ComponentService.TileSize.ExtraLarge:
                        tileInfo.IsExtraLarge = true;
                        break;
                }

                var image = new Uri(System.IO.Path.ChangeExtension(uri.GetPathString(), "png"), UriKind.RelativeOrAbsolute);
                var uri2 = projectDocument.MakeAbosoluteUri(image, ScreenComponent);
                if (uri2 != null && uri2.IsAbsoluteUri)
                {
#if !DEBUG
                    uri2 = projectDocument.MakeRelativeUri(uri2, AppDomain.CurrentDomain.BaseDirectory);
                    if (uri2 != null)
#endif
                    tileInfo.ImageUrl = uri2.ToString();
                }
                /*
                else
                {
                    uri2 = projectDocument.ExportRelativeUriToTemporaryFile(uri2, "png");
                    if (uri2 != null)
                    {
                        tileInfo.ImageUrl = uri2.ToString();
                        temporaryFiles.Add(uri2.GetPathString());
                    }
                }
                */
                // tileInfo.Section = controller.GetTitle();
                tileInfo.Section = tileInfo.Name;
                tileInfo.Section = tileInfo.Section.Replace('/', '_');
                tileInfo.Section = tileInfo.Section.Replace('\\', '_');

                tileInfo.HasGeoCoordinates = controller.GetHasGeoCoordinates(uri);
                tileInfo.Latitude = controller.GetLatitude(uri);
                tileInfo.Longitude = controller.GetLongitude(uri);

                tileInfo.UsersVisibility = controller.GetUsersVisibility(uri);
                tileInfo.RolesVisibility = controller.GetRolesVisibility(uri);

                tileInfo.MinZoomLevel = controller.GetMapMinZoomLevelVisibility(uri);
                tileInfo.MaxZoomLevel = controller.GetMapMinZoomLevelVisibility(uri);

                tileInfo.LatTag = controller.GetLatitudeTag(uri);
                tileInfo.LonTag = controller.GetLongitudeTag(uri);

                if (!mapTiles.ContainsKey(tileInfo.Section))
                    mapTiles.Add(tileInfo.Section, new List<TileInfo>());
                mapTiles[tileInfo.Section].Add(tileInfo);
            }
        }

        static void AddTileInfos(IScreenController controller)
        {
            controller.GetScreenLists(ScreenComponent).ForEach(uri =>
            {
                AddTileInfo(controller, uri);
            });

            controller.GetScreenControllers(ScreenComponent).ForEach(c =>
            {
                if (c != controller)
                    AddTileInfos(c);
            });
        }
        #endregion

        #region HistoricalData

        class DataLoggerSettings
        {
            public string Name { get; set; }
            public string TableName { get; set; }
            public string UtcTimeColumnName { get; set; }
            public string LocalTimeColumnName { get; set; }
            public string MillisecondsColumnName { get; set; }
            public string UserColumnName { get; set; }
            public string ReasonColumnName { get; set; }
            public DataLoggerColumns Columns { get; set; }
        }

        class DataLoggerColumns : List<DataLoggerColumn>
        {
            public DataLoggerColumns()
            {
            }
        }

        class DataLoggerColumn
        {
            public string Name { get; set; }
            public string ColumnTagName { get; set; }
        }

        static public JObject GetDataSetJObject(DataSet dataSet)
        {
            return dataSet == null ? null : JObject.Parse(JsonConvert.SerializeObject(dataSet, new Newtonsoft.Json.Converters.DataSetConverter()));
        }

        static public string LoadDataloggerViewerData(string connectionString, List<string> historicalList, string selecteditem, int maxRows, int commandTimeout, DateSpan rangeType, long? startTimestamp, long? endTimestamp, string sessionString, CancellationToken ct)
        {
            var ret = String.Empty;
            var gridDataSet = new DataSet();
            var UFUAEditor = projectDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor == null || ct.IsCancellationRequested)
                return ret;

            //CreateDataSource()
            //InitValues()
            DataLoggerSettings currentSettings = null;
            List<string> qualityColumnNames = new List<string>();
            string lcurrentSettingsXml = UFUAEditor.GetDataLoggerDataTable(projectDocument, selecteditem, inExecution: true);
            if (lcurrentSettingsXml != null)
            {
                var dlt = lcurrentSettingsXml.FromXml<DataLoggerTable>();
                if (dlt != null)
                {
                    qualityColumnNames = dlt.StatusCodeColumnNamesList;
                    currentSettings = new DataLoggerSettings()
                    {
                        Name = dlt.Name,
                        TableName = dlt.TableName,
                        UtcTimeColumnName = dlt.UtcTimeColumnName,
                        LocalTimeColumnName = dlt.LocalTimeColumnName,
                        MillisecondsColumnName = dlt.MillisecondsColumnName,
                        UserColumnName = dlt.UserColumnName,
                        ReasonColumnName = dlt.ReasonColumnName
                    };
                    currentSettings.Columns = new DataLoggerColumns();
                    var columns = UFUAEditor.GetDataLoggerColumnList(projectDocument, selecteditem) as List<String>;
                    if (columns != null)
                        columns.ForEach(x => currentSettings.Columns.Add(new DataLoggerColumn() { Name = x }));
                }
            }
            else if (currentSettings != null)
            {
                currentSettings.Columns.Clear();
                currentSettings = null;
            }
            //InitValues()
            //InitConnection()
            string defaultDataProvider = null;
            string defaultConnectionString = null;
            var settings = UFUAEditor.GetDataLoggerConnection(projectDocument, selecteditem);
            if (String.IsNullOrEmpty(settings))
                settings = XpoHelpers.XpoHelper.NormalizeConnectionString(connectionString, projectDocument?.rootBase);
            if (String.IsNullOrEmpty(settings))
                settings = UFUAEditor.GetHistorianDefaultConnection(projectDocument);

            if (!String.IsNullOrEmpty(settings))
            {
                settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(settings, sessionString);
                var helper = new ConnectionStringParser(settings);
                var providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType))
                {
                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                }
                else
                {
                    defaultDataProvider = helper.GetPartByName("DataProvider");
                    helper.RemovePartByName("DataProvider");
                    defaultConnectionString = helper.GetConnectionString();
                }
            }
            //InitConnection()
            if (ct.IsCancellationRequested || currentSettings == null || string.IsNullOrEmpty(defaultConnectionString) || string.IsNullOrEmpty(defaultDataProvider))
                return ret;

            var maxr = maxRows;
            var _defaultDataProvider = defaultDataProvider;
            var _defaultConnectionString = defaultConnectionString;

            //if (param != null)
            //{
            //    SetTimeSpan((DateSpan)Enum.Parse(typeof(DateSpan), param.ToString(), true));
            //}

            DateTime DateTimeStart;
            DateTime DateTimeEnd;

            if (startTimestamp != null && endTimestamp != null)
            {
                DateTimeStart = DateTimeOffset.FromUnixTimeSeconds((long)startTimestamp).DateTime;
                DateTimeEnd = DateTimeOffset.FromUnixTimeSeconds((long)endTimestamp).DateTime;
            }
            else
            {
                var dt = SetTimeSpan(rangeType);
                DateTimeStart = dt.Item1;
                DateTimeEnd = dt.Item2;
            }

            var utcStart = DateTimeStart.ToUniversalTime();
            var utcEnd = DateTimeEnd.ToUniversalTime();

            if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                utcStart = (DateTime)SqlDateTime.MinValue;
            if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                utcEnd = (DateTime)SqlDateTime.MaxValue;

            string _tablename = currentSettings.TableName;
            string _utccolumnname = currentSettings.UtcTimeColumnName;
            string _localcolumnname = currentSettings.LocalTimeColumnName;

            using (var connection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
            {
                DataTable retTable = null;
                try
                {
                    connection.Open();
                    if (ct.IsCancellationRequested)
                        return ret;
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);
                    DbSchemaInfo dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandTimeout = commandTimeout;

                    StringBuilder commantText = new StringBuilder("SELECT ");
                    if (maxr > 0 && dbSchemaInfo.IsSupportedTopKeyword)
                        commantText.AppendFormat(" TOP {0} ", maxr);

                    commantText.AppendFormat(" * FROM {0}", dbSchemaInfo.WrapObjectName(_tablename));

                    var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                    datestart.DbType = System.Data.DbType.DateTime;
                    datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                    datestart.Value = utcStart;
                    dbdapater.SelectCommand.Parameters.Add(datestart);

                    var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                    dateend.DbType = System.Data.DbType.DateTime;
                    dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                    dateend.Value = utcEnd;
                    dbdapater.SelectCommand.Parameters.Add(dateend);

                    commantText.AppendFormat(" WHERE {0} >= {1} AND {2} <= {3}  ORDER BY {4} DESC",
                        dbSchemaInfo.WrapObjectName(_utccolumnname),
                        datestart.ParameterName,
                        dbSchemaInfo.WrapObjectName(_utccolumnname),
                        dateend.ParameterName,
                        dbSchemaInfo.WrapObjectName(_utccolumnname));

                    dbdapater.SelectCommand.CommandText = commantText.ToString();
                    gridDataSet.Clear();
                    dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                    //if (!bOnlySchema)
                        //dbdapater.Fill(gridDataSet, _tablename);
                    if (dbSchemaInfo.IsSupportedTopKeyword)
                    {
                        //if (!bOnlySchema)
                        dbdapater.Fill(gridDataSet, _tablename);
                        retTable = gridDataSet.Tables[0];
                    }
                    else //if (!bOnlySchema)
                        retTable = DataReader.DataReader.DataTableFromDataSet(gridDataSet, dbdapater, maxr);
                    commantText = null;
                    //var dataView = gridDataSet.Tables[_tablename].Copy().AsDataView();
                }
                finally
                {
                    connection.Close();
                }

                if (ct.IsCancellationRequested)
                    return ret;

                var jRows = new JArray();
                if (retTable != null)
                    jRows = JArray.Parse(JsonConvert.SerializeObject(retTable));
                var jObject = new JObject(
                    new JProperty("DataRows", jRows),
                    new JProperty("StartDateTimeTicks", JToken.Parse(DateTimeStart.Ticks.ToString())),
                    new JProperty("EndDateTimeTicks", JToken.Parse(DateTimeEnd.Ticks.ToString()))
                );
                jObject.Add("UtcTimeColName", _utccolumnname);
                jObject.Add("LocalTimeColName", _localcolumnname);

                return jObject.ToString(Newtonsoft.Json.Formatting.None);
            }
        }

        static IDataLayer CreateHistoricalDataLayer(String settings, int commandTimeout)
        {
            return UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditDataItem>(settings, commandTimeout);
        }

        static IDataLayer CreateHistoricalEventDataLayer(String settings, int commandTimeout)
        {
            return UFUAHistorianModel.Helpers.HistorianHelper.CreateDataLayer<UFUAAuditLogItem>(settings, commandTimeout);
        }

        static IDataLayer GetDataLoggerConnectionStringDataLayer(String name, String sessionString, int commandTimeout)
        {
            lock (MapToDataLoggerDataLayer)
            {
                if (MapToDataLoggerDataLayer.ContainsKey(name))
                    return MapToDataLoggerDataLayer[name];

                var conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(UFUAEditorComponent.GetDataLoggerConnection(projectDocument, name), sessionString);
                if (string.IsNullOrEmpty(conn))
                    return null;

                var _dl = CreateHistoricalDataLayer(conn, commandTimeout);
                if (_dl != null)
                {
                    MapToDataLoggerDataLayer.Add(name, _dl);
                    return _dl;
                }
            }

            return null;
        }
        
        static IDataLayer GetHistoricalConnectionStringDataLayerByNodeid(UFProjectDocument document, string nodeId, string appName, string historicalName, string connStr, int commandTimeout, string sessionString)
        {
            lock (MapNodeIdsToHistoricalDataLayer)
            {
                if (MapNodeIdsToHistoricalDataLayer.ContainsKey(nodeId))
                    return MapNodeIdsToHistoricalDataLayer[nodeId];

                var conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connStr, sessionString);
                if (string.IsNullOrEmpty(conn))
                    return null;

                var _dl = CreateHistoricalDataLayer(conn, commandTimeout);
                if (_dl != null)
                {
                    MapNodeIdsToHistoricalDataLayer.Add(nodeId, _dl);
                    return _dl;
                }
            }

            return null;
        }

        static IDataLayer GetHistoricalConnectionStringDataLayer(String name, String sessionString, int commandTimeout)
        {
            lock (MapToHistoricalDataLayer)
            {
                if (MapToHistoricalDataLayer.ContainsKey(name))
                    return MapToHistoricalDataLayer[name];

                var conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(UFUAEditorComponent.GetHistorianConnection(projectDocument, name), sessionString);
                if (string.IsNullOrEmpty(conn))
                    return null;

                var _dl = CreateHistoricalDataLayer(conn, commandTimeout);
                if (_dl != null)
                {
                    MapToHistoricalDataLayer.Add(name, _dl);
                    return _dl;
                }
            }

            return null;
        }

        static IDataLayer GetHistoricalEventConnectionStringDataLayer(String name, String sessionString, int commandTimeout)
        {
            lock (MapToHistoricalDataLayer)
            {
                if (MapToHistoricalDataLayer.ContainsKey(name))
                    return MapToHistoricalDataLayer[name];

                var conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(UFUAEditorComponent.GetHistorianConnection(projectDocument, name), sessionString);
                if (string.IsNullOrEmpty(conn))
                    return null;

                var _dl = CreateHistoricalEventDataLayer(conn, commandTimeout);
                if (_dl != null)
                {
                    MapToHistoricalDataLayer.Add(name, _dl);
                    return _dl;
                }
            }

            return null;
        }

        static public List<string> InitDataloggerValues(string sessionString)
        {
            var list = new List<string>() { String.Empty };
            var UFUAEditor = projectDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor != null)
                list = UFUAEditor.GetDataLoggerSettingsNameList(projectDocument, true, true).ToList();
            return list;
        }

        static public List<string> InitHistoricalValues(bool showAuditTrace, string sessionString)
        {
            var list = new List<string>() { String.Empty };
            var UFUAEditor = projectDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor != null)
            {
                if (showAuditTrace)
                {
                    var list2 = UFUAEditor.GetAuditTraceTagNameList(projectDocument, bReloadDocument: false, inExecution: true);
                    if (list2 != null)
                        list.AddRange(list2.OrderBy(x => x));
                }
                else
                {
                    var list2 = UFUAEditor.GetHistoricalSettingsNameList(projectDocument, bReloadDocument: false, inExecution: true);
                    if (list2 != null)
                        list.AddRange(list2.OrderBy(x => x));
                }
            }
            return list;
        }

        public static Tuple<DateTime, DateTime> SetTimeSpan(DateSpan span)
        {
            DateTime date1 = DateTime.Now;
            DateTime date2 = DateTime.Now;
            switch (span)
            {
                case DateSpan.All:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.All);
                    break;
                case DateSpan.Minute:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Minute);
                    break;
                case DateSpan.Hour:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Hour);
                    break;
                case DateSpan.Day:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Day);
                    break;
                case DateSpan.Week:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Week);
                    break;
                case DateSpan.Month:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Month);
                    break;
                case DateSpan.Year:
                    DateRangeHelpers.SetDateRange(out date1, out date2, DateTime.Now, DateSpan.Year);
                    break;
                default:
                    break;
            }
            return new Tuple<DateTime, DateTime>(date1, date2);
        }

        static public ServerAuditDataItemModel LoadHistoricalViewerData(string connection, bool showAuditTrace, List<string> historicalList, string selecteditem, int maxRows, int commandTimeout, DateSpan rangeType, long? startTimestamp, long? endTimestamp, string sessionString, CancellationToken ct)
        {
            var ret = new ServerAuditDataItemModel();
            var UFUAEditor = projectDocument.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
            if (UFUAEditor == null)
                return ret;

            string connectionStringAuditTrace = null;
            foreach (var x in historicalList)
            {
                if (ct.IsCancellationRequested)
                    return ret;
                if (!String.IsNullOrEmpty(x)/* && !MapToHistoricalConnectsions.ContainsKey(x)*/)
                {
                    if (showAuditTrace)
                    {
                        connectionStringAuditTrace = UFUAEditor.GetAuditTraceDefaultConnection(projectDocument);
                        connectionStringAuditTrace = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(connectionStringAuditTrace, sessionString);
                    }
                    else
                        InitHistorianConnections(sessionString, null);
                }
            };

            //CreateDataSource method body
            DateTime DateTimeStart;
            DateTime DateTimeEnd;

            if (startTimestamp != null && endTimestamp != null)
            {
                DateTimeStart = DateTimeOffset.FromUnixTimeSeconds((long)startTimestamp).DateTime;
                DateTimeEnd = DateTimeOffset.FromUnixTimeSeconds((long)endTimestamp).DateTime;
            }
            else
            {
                var dt = SetTimeSpan(rangeType);
                DateTimeStart = dt.Item1;
                DateTimeEnd = dt.Item2;
            }

            ret.StartDateTimeTicks = DateTimeStart.Ticks;
            ret.EndDateTimeTicks = DateTimeEnd.Ticks;

            var projectConnectionString = PlatformComponents.historiansettings[PlatformComponents.GetProjectTitle()];
            var connectionString = (showAuditTrace ? connectionStringAuditTrace : XpoHelpers.XpoHelper.NormalizeConnectionString(connection, projectDocument?.rootBase)) ?? projectConnectionString;
            if (String.IsNullOrEmpty(connectionString))// || !dInit || bRefreshing) // || ufw == null)
                return ret;

            List<AuditDataItemModel> list = new List<AuditDataItemModel>();
            List<UFUAAuditDataItem> _list = new List<UFUAAuditDataItem>();

            var utcStart = DateTimeStart.ToUniversalTime();
            var utcEnd = DateTimeEnd.ToUniversalTime();

            if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                utcStart = (DateTime)SqlDateTime.MinValue;
            if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                utcEnd = (DateTime)SqlDateTime.MaxValue;

            IDataLayer _dl = GetHistoricalConnectionStringDataLayer(selecteditem, sessionString, commandTimeout);
            if (_dl == null)
                _dl = CreateHistoricalDataLayer(connectionString, commandTimeout);

            if (ct.IsCancellationRequested)
                return ret;

            Dictionary<int, string> mapDescriptions = null;
            using (UnitOfWork _ufw = new UnitOfWork(_dl))
            {
                if (string.IsNullOrEmpty(selecteditem))
                {
                    if (showAuditTrace)
                    {
                        var auditTraceSuffix = String.Format("#{0}", UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix());
                        mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                           where entry.HistoricalName.EndsWith(auditTraceSuffix)
                                           select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                    }
                    else
                    {
                        mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                           select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                    }

                    if (utcStart == utcEnd)
                        _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                 where !showAuditTrace || mapDescriptions.Keys.Contains(entry.DataLogRef)
                                 orderby entry.RecordDateTimeUtc descending
                                 select entry).Take((int)maxRows).ToList();
                    else
                        _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                 where (!showAuditTrace || mapDescriptions.Keys.Contains(entry.DataLogRef)) &&
                                 (entry.RecordDateTimeUtc >= utcStart) && (entry.RecordDateTimeUtc <= utcEnd)
                                 orderby entry.RecordDateTimeUtc descending
                                 select entry).Take((int)maxRows).ToList();
                }
                else
                {
                    if (showAuditTrace)
                    {
                        var auditTraceSuffix = String.Format("#{0}", UFUAServerInfo.UFUAServerInfo.GetAuditTraceSuffix());
                        var auditNameSuffix = String.Format(".{0}", selecteditem.Replace('/', '.'));
                        mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                           where entry.HistoricalName.EndsWith(auditTraceSuffix) &&
                                               entry.Name.EndsWith(auditNameSuffix)
                                           select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                    }
                    else
                    {
                        mapDescriptions = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)/*.AsParallel()*/
                                           where entry.HistoricalName == selecteditem
                                           select entry).ToDictionary(entry => entry.Oid, entry => entry.Description);
                    }

                    if (utcStart == utcEnd)
                        _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                 where mapDescriptions.Keys.Contains(entry.DataLogRef)
                                 orderby entry.RecordDateTimeUtc descending
                                 select entry).Take((int)maxRows).ToList();
                    else
                    {
                        _list = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)//.AsParallel()
                                 where mapDescriptions.Keys.Contains(entry.DataLogRef) &&
                                 entry.RecordDateTimeUtc >= utcStart && entry.RecordDateTimeUtc <= utcEnd
                                 orderby entry.RecordDateTimeUtc descending
                                 select entry).Take((int)maxRows).ToList();
                    }
                }
            }
            foreach (var entry in _list)
            {
                if (ct.IsCancellationRequested)
                    return ret;
                var _entry = new AuditDataItemModel()
                {
                    Name = entry.Name,
                    Description = mapDescriptions.ContainsKey(entry.DataLogRef) ? mapDescriptions[entry.DataLogRef] : String.Empty,
                    Value = entry.Value,
                    dValue = entry.dValue.HasValue ? entry.dValue.Value : double.NaN,
                    ValueBefore = entry.ValueBefore,
                    dValueBefore = entry.dValueBefore.HasValue ? entry.dValueBefore.Value : double.NaN,
                    RecordDateTime = entry.RecordDateTimeUtc.ToString("G"),
                    RecordDateTimeMilliseconds = entry.RecordDateTimeUtc.Millisecond.ToString().PadLeft(3, '0'),
                    SourceTimeStamp = entry.SourceTimeStamp.ToLocalTime().ToString("G"),
                    SourceTimeStampMilliseconds = entry.SourceTimeStamp.Millisecond.ToString().PadLeft(3, '0'),
                    SourcePicoseconds = entry.SourcePicoseconds,
                    ServerTimeStamp = entry.ServerTimeStamp.ToLocalTime().ToString("G"),
                    ServerTimeStampMilliseconds = entry.ServerTimeStamp.Millisecond.ToString().PadLeft(3, '0'),
                    ServerPicoseconds = entry.ServerPicoseconds,
                    Status = entry.Status,
                    UserName = entry.UserName,
                    Reason = entry.Reason
                };
                list.Add(_entry);
            };
            ret.Items = list;
            return ret;
        }

        static void InitHistorianConnections(string sessionString, string customConnString)
        {
            lock (lockObject)
            {
                if (MapToHistoricalConnectsions == null)
                {
                    MapToHistoricalConnectsions = new Dictionary<String, String>();
                    List<string> list = new List<string>();
                    var list3 = ufuaEditorComponent.GetHistoricalSettingsNameList(projectDocument, bReloadDocument: false, inExecution: true);
                    if (list3 != null)
                        list.AddRange(list3);
                    list.ForEach(x =>
                    {
                        if (!MapToHistoricalConnectsions.ContainsKey(x))
                        {
                            var conn = ufuaEditorComponent.GetHistorianConnection(projectDocument, x);
                            MapToHistoricalConnectsions.Add(x, string.IsNullOrEmpty(conn) ? string.Empty : RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, sessionString));
                        }
                    });
                }
            }
        }

        static void InitDataloggerConnections(string sessionString, string customConnString = null)
        {
            lock (lockObject)
            {
                if (MapToDatalogerConnectsions == null)
                {
                    MapToDatalogerConnectsions = new Dictionary<string, string>();
                    List<string> dlist = new List<string>();
                    var list2 = ufuaEditorComponent.GetDataLoggerSettingsNameList(projectDocument, bReloadDocument: false, inExecution: true);
                    if (list2 != null)
                        dlist.AddRange(list2);
                    dlist.ForEach(x =>
                    {
                        if (!MapToDatalogerConnectsions.ContainsKey(x))
                        {
                            var conn = ufuaEditorComponent.GetDataLoggerConnection(projectDocument, x);
                            if (String.IsNullOrEmpty(conn))
                                conn = customConnString;
                            if (String.IsNullOrEmpty(conn))
                                conn = ufuaEditorComponent.GetHistorianDefaultConnection(projectDocument);

                            if (!String.IsNullOrEmpty(conn))
                                conn = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(conn, sessionString);

                            MapToDatalogerConnectsions.Add(x, conn);
                        }
                    });
                }
            }
        }

        static public string InitChartHistoricalData(string historicalname, string sessionString)
        {
            InitHistorianConnections(sessionString, null);
            string conn;
            lock (MapToHistoricalConnectsions)
            {
                conn = (MapToHistoricalConnectsions != null && !string.IsNullOrEmpty(historicalname) && MapToHistoricalConnectsions.ContainsKey(historicalname) && !string.IsNullOrEmpty(MapToHistoricalConnectsions[historicalname])) ? MapToHistoricalConnectsions[historicalname] : null;
            }
            return conn ?? PlatformComponents.historiansettings[PlatformComponents.GetProjectTitle()];
        }

        static public StatesChartConfigData InitStatesChartHistoricalData(string connString, string historicalname, bool bDlrSource, string sessionString, string nodeID, string appName)
        {
            //InitServerDocument
            var settings = ufuaEditorComponent.GetDataLoggerSettings(projectDocument);
            var dlrSettings = new Dictionary<string, DataLoggerColumnListControl.DataLoggerSettings>();
            if (settings != null)
            {
                foreach (List<String> d in settings)
                {
                    if (!dlrSettings.ContainsKey(d[0]))
                    {
                        DataLoggerColumnListControl.DataLoggerSettings ds = new DataLoggerColumnListControl.DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                        dlrSettings.Add(ds.Name, ds);
                    }
                }
            }
            //InitServerDocument

            //string _keyname = nodeID;
            //mapKeySeries[_keyname] = new GenericSeries(PenReferenceList[key], key);//the first pen is the contestual tag

            string UtcTimeColumnName = "UtcTimeCol";
            var scData = new StatesChartConfigData()
            {
                connectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connString, projectDocument?.rootBase),
                tablename = historicalname,
                utccolumnname = UtcTimeColumnName
            };

            if (bDlrSource)
            {
                InitDataloggerConnections(sessionString, connString);
                var historical = historicalname;
                if (!string.IsNullOrEmpty(historical))
                {
                    lock (MapToDatalogerConnectsions)
                    {
                        if (MapToDatalogerConnectsions != null && MapToDatalogerConnectsions.ContainsKey(historical) && !string.IsNullOrEmpty(MapToDatalogerConnectsions[historical]))
                        {
                            scData.connectionString = MapToDatalogerConnectsions[historicalname];
                        }
                    }
                    if (dlrSettings != null && dlrSettings.ContainsKey(historical))
                    {
                        scData.tablename = string.IsNullOrEmpty(dlrSettings[historical].TableName) ? historical : dlrSettings[historical].TableName;
                        scData.utccolumnname = string.IsNullOrEmpty(dlrSettings[historical].UtcTimeColumnName) ? UtcTimeColumnName : dlrSettings[historical].UtcTimeColumnName;
                    }
                }
            }
            else
            {
                scData.connectionString = GetConnectionStringForNodeId(nodeID, appName, historicalname, connString);
            }
            if (scData.connectionString == null)
                scData.connectionString = PlatformComponents.historiansettings[PlatformComponents.GetProjectTitle()];
            return scData;
        }
        
        static string GetConnectionStringForNodeId(string nodeId, string appName, string historicalName, string connStr)
        {
            lock (MapNodeIdsToHistoricalConnectsions)
            {
                if (MapNodeIdsToHistoricalConnectsions.ContainsKey(nodeId))
                    return MapNodeIdsToHistoricalConnectsions[nodeId];
            }

            lock (lockObject)
            {
                var document = ufuaEditorComponent.GetProjectDocument(projectDocument, appName, true);
                if (document != null)
                {
                    if (!string.IsNullOrEmpty(historicalName))
                    {
                        connStr = ufuaEditorComponent.GetHistorianConnection(document, historicalName);
                        if (string.IsNullOrEmpty(connStr))
                            connStr = ufuaEditorComponent.GetHistorianDefaultConnection(document);
                    }
                }
            }

            lock (MapNodeIdsToHistoricalConnectsions)
            {
                MapNodeIdsToHistoricalConnectsions[nodeId] = connStr;
            }
            return connStr;
        }

        public class StatesChartConfigData {
            public string connectionString;
            public string tablename;
            public string utccolumnname;
            public StatesChartConfigData() { }
        }

        static public AggregatedValues LoadSparklineHistoricalData(string screenId, string connection, string dataprovider, int commandTimeout, double invalidPointsValue, string _valuecolumnname, string _groupby, int _maxrecord, string _select, string _orderby, string _tablename, string _timecolumnname, string _whereCondition, string connectionstring, string sessionString, CancellationToken ct)
        {
            string defaultDataProvider = null;
            string defaultConnectionString = null;

            if (!string.IsNullOrEmpty(connectionstring))
            {
                defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connectionstring);
                defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(connectionstring);
            }
            else
            {
                if (ufuaEditorComponent != null)
                {
                    string settings = RealTimeConnectionManagerViewModel.ReplaceServerRenamedOnDataSource(ufuaEditorComponent.GetHistorianDefaultConnection(projectDocument), sessionString);
                    if (!string.IsNullOrEmpty(settings))
                    {
                        defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(settings);
                        defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(settings);
                    }
                }
            }

            if(!string.IsNullOrEmpty(dataprovider) && !string.IsNullOrEmpty(connection))
            {
                defaultDataProvider = dataprovider;
                defaultConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(connection, projectDocument?.rootBase);
            }

            if (string.IsNullOrEmpty(defaultDataProvider) || string.IsNullOrEmpty(defaultConnectionString))
                throw new Exception("Empty data provider or connection string");

            var _data = new AggregatedValues();
            if (ct.IsCancellationRequested)
                return _data;

            DateTime startdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime enddate = startdate.AddHours(24);

            try
            {
                using (var dbconnection = DataReader.DataReader.CreateDbConnection(defaultDataProvider, defaultConnectionString))
                {

                    dbconnection.Open();
                    if (ct.IsCancellationRequested)
                        return _data;
                    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(defaultDataProvider);

                    DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(defaultDataProvider, defaultConnectionString);
                    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(defaultDataProvider);
                    dbdapater.SelectCommand.Connection = dbconnection;

                    StringBuilder commantText = new StringBuilder();
                    DataSet gridDataSet = new DataSet();

                    if (string.IsNullOrEmpty(_select))
                    {
                        commantText.AppendFormat("SELECT");
                        if (dbSchemaInfo.IsSupportedTopKeyword && _maxrecord > 0.0)
                            commantText.AppendFormat(" TOP {0} ", _maxrecord);
                        commantText.AppendFormat(" {0}", dbSchemaInfo.WrapObjectName(_timecolumnname));
                        commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(_valuecolumnname));

                        commantText.AppendFormat(" FROM {0}",
                            dbSchemaInfo.WrapObjectName(_tablename));


                    }
                    else
                        commantText.AppendFormat("{0}", _select);

                    //commantText.AppendFormat(" WHERE {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(_valuecolumnname));

                    var datestart = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                    datestart.DbType = System.Data.DbType.DateTime;
                    datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                    datestart.Value = startdate;
                    dbdapater.SelectCommand.Parameters.Add(datestart);

                    var dateend = DataReader.DataReader.CreateDbParameter(defaultDataProvider);
                    dateend.DbType = System.Data.DbType.DateTime;
                    dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                    dateend.Value = enddate;
                    dbdapater.SelectCommand.Parameters.Add(dateend);

                    commantText.AppendFormat(" WHERE {0} >= {1} AND {0} < {2} AND {0} IS NOT NULL",
                        dbSchemaInfo.WrapObjectName(_timecolumnname),
                        datestart.ParameterName,
                        dateend.ParameterName);

                    if (!string.IsNullOrEmpty(_whereCondition))
                        commantText.AppendFormat(" AND {0}", _whereCondition);

                    commantText.AppendFormat(" ORDER BY ");

                    if (!string.IsNullOrEmpty(_orderby))
                    {
                        if (!_orderby.Contains(_timecolumnname))
                            commantText.AppendFormat("{0} ASC, ", dbSchemaInfo.WrapObjectName(_timecolumnname));
                        commantText.AppendFormat("{0}", _orderby);
                    }
                    else
                        commantText.AppendFormat("{0} ASC", dbSchemaInfo.WrapObjectName(_timecolumnname));

                    if (!string.IsNullOrEmpty(_groupby))
                        commantText.AppendFormat(" GROUP BY {0}", _groupby);

                    dbdapater.SelectCommand.CommandText = commantText.ToString();
                    dbdapater.SelectCommand.CommandTimeout = (int)commandTimeout;

                    dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                    dbdapater.Fill(gridDataSet, _tablename);

                    using (DataView dataView = new DataView(gridDataSet.Tables[0]))
                    {
                        foreach (DataRowView rowView in dataView)
                        {
                            if (ct.IsCancellationRequested)
                                return _data;
                            DateTime date = (DateTime)rowView[_timecolumnname];
                            _data.Values.Add(new MyDataValue()
                            {
                                SourceTimestamp = date,
                                Value = rowView[_valuecolumnname] is DBNull ? invalidPointsValue : rowView[_valuecolumnname],
                                dValue = rowView[_valuecolumnname] is DBNull ? invalidPointsValue : System.Convert.ToDouble(rowView[_valuecolumnname]),
                                //FilteringColumn = 1
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsBlocking(ex))
                    _data.bBlockingException = true;
            }
            return _data;
        }

        static bool IsBlocking(Exception ex)
        {
            if ((ex as System.Data.SqlClient.SqlException)?.Number == -2 || (ex as InvalidOperationException)?.HResult == -2146233079) //CommandTimeout exception - Connection timeout expired
                return false;
            return ex.InnerException == null || IsBlocking(ex.InnerException);
        }

        static public Tuple<string, string> UpdateTagFromMap(string tagName, Dictionary<string, Tuple<string, string>> parametersMap)
        {
            foreach (var tag in parametersMap.Keys)
            {
                var rel = TagPathHelper.GetTagPath(null, tag, false);
                if (rel == tagName)
                    return new Tuple<string, string>(TagPathHelper.GetTagPath(null, parametersMap[tag].Item1, false), parametersMap[tag].Item2);
            }
            return null;
        }

        static Dictionary<String, DataLoggerSettings> UpdateDlrSettings(string historicalName)
        {
            bool bSettingsExist;
            lock (MapDlrSettings)
            {
                bSettingsExist = MapDlrSettings.ContainsKey(historicalName);
                if (bSettingsExist)
                    return MapDlrSettings[historicalName];
            }
            
            Dictionary<String, DataLoggerSettings> dlrSettings = new Dictionary<String, DataLoggerSettings>();
            var _list = ufuaEditorComponent.GetDataLoggerSettings(projectDocument).ToList();
            if (_list != null)
            {
                (from d in _list where historicalName == (d[0]) select d).ToList()./*AsParallel().ForAll*/ForEach(d =>
                {
                    DataLoggerSettings ds = new DataLoggerSettings() { Name = d[0], TableName = d[1], UtcTimeColumnName = d[2] };
                    ds.Columns = new DataLoggerColumns();
                    var columns = ufuaEditorComponent.GetDataLoggerColumnSettingList(projectDocument, ds.Name) as List<List<String>>;
                    if (columns != null)
                    {
                        columns.ForEach(s =>
                        {
                            ds.Columns.Add(new DataLoggerColumn() { Name = s[0], ColumnTagName = s[3] });
                        });
                        dlrSettings.Add(ds.Name, ds);
                    }
                });
            }
            lock (MapDlrSettings)
            {
                if (!MapDlrSettings.ContainsKey(historicalName))
                    MapDlrSettings.Add(historicalName, dlrSettings);
            }
            return dlrSettings;
        }

        static public AggregatedValues LoadHistoricalData(string connString, int commandTimeout, long? startTimestamp, long? endTimestamp, bool usesourcetimestamp, int maxRecord, bool aggregate, int maxaggregationfactor, int maxDeadLockRetry, bool localize, TimeSpan viewTimeFrame, bool minAggregation, bool maxAggregation, bool avgAggregation, string tagName, string columnTagName, Dictionary<string, Tuple<string, string>> parametersMap, string serieTitle, string nodeID, string appName, string historicalname, bool bDlrSource, string sessionString, string conditionalString, bool bNeedsMedian, bool bNeedsVariance, bool bNeedsStdDev, System.Threading.CancellationToken ct)
        {
            AggregatedValues ret = new AggregatedValues();
            
            var tagData = UpdateTagFromMap(bDlrSource ? columnTagName : tagName, parametersMap);
            if (tagData != null)
            {
                tagName = tagData.Item1;
                nodeID = tagData.Item2;
            }

            var name = tagName;

            var serie = name;
            if (!String.IsNullOrEmpty(serieTitle))
                serie = serieTitle;

            var maxDateTimeValue = (DateTime)SqlDateTime.MaxValue;
            var minDateTimeValue = (DateTime)SqlDateTime.MinValue;
            var diff = new TimeSpan();

            DateTime utcStart = minDateTimeValue;
            DateTime utcEnd = maxDateTimeValue;

            if (startTimestamp != null)
                utcStart = DateTimeOffset.FromUnixTimeSeconds((long)startTimestamp).DateTime;

            if (endTimestamp != null)
                utcEnd = DateTimeOffset.FromUnixTimeSeconds((long)endTimestamp).DateTime;

            if (utcStart == utcEnd)
            {
                utcStart = minDateTimeValue;
                utcEnd = maxDateTimeValue;
            }

            if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                utcStart = (DateTime)SqlDateTime.MinValue;
            if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                utcEnd = (DateTime)SqlDateTime.MaxValue;

            var settingStorage = new SettingsStorage()
            {
                mapSeries = new Dictionary<string, SerieSettings>()
                    {
                        { serie, new SerieSettings()
                            {
                                NodeID = nodeID,
                                MinAggregation = minAggregation,
                                MaxAggregation = maxAggregation,
                                AvgAggregation = avgAggregation
                            }
                        }
                    }
            };

            var seriesettings = settingStorage.mapSeries[serie];

            int maxrecord = aggregate ? maxRecord * maxaggregationfactor : maxRecord;

            var connStr = String.IsNullOrEmpty(connString) ? PlatformComponents.historiansettings[PlatformComponents.GetProjectTitle()] : XpoHelpers.XpoHelper.NormalizeConnectionString(connString, projectDocument?.rootBase);

            if (String.IsNullOrEmpty(connStr))
                return null;

            if (!bDlrSource)
            {
                UFProjectDocument projectDoc;
                lock (lockObject)
                {
                    projectDoc = ufuaEditorComponent.GetProjectDocument(projectDocument, appName, true) as UFProjectDocument;
                }

                if (projectDoc != null)
                {
                    connStr = String.IsNullOrEmpty(connString) ? PlatformComponents.historiansettings[projectDoc?.GetTitle()] : XpoHelpers.XpoHelper.NormalizeConnectionString(connString, projectDoc?.rootBase);
                    connStr = GetConnectionStringForNodeId(nodeID, appName, historicalname, connStr);

                    var _dl = GetHistoricalConnectionStringDataLayerByNodeid(projectDoc, nodeID, appName, historicalname, connStr, commandTimeout, sessionString);
                    if (_dl == null)
                        _dl = CreateHistoricalDataLayer(connStr, commandTimeout);

                    #region Historicals
                    string realTagname = name.Replace("//", "/");
                    realTagname = realTagname.Replace("/", ".");
                    if (realTagname.StartsWith("."))
                        realTagname = realTagname.Substring(1);
                    if (!realTagname.StartsWith("Tags."))
                        realTagname = $"Tags.{realTagname}";

                    bool bUseOid = false;

                    if (ct.IsCancellationRequested)
                        return ret;

                    using (UnitOfWork _ufw = new UnitOfWork(_dl))
                    {
                        try
                        {
                            UFUAAuditDataLog _uFUAAuditDataLog = (from entry in new XPQuery<UFUAAuditDataLog>(_ufw)// .AsParallel()
                                                                  where /*entry.Name == realTagname && entry.HistoricalName == historicalname &&*/ entry.NodeId == seriesettings.NodeID
                                                                  select entry).FirstOrDefault();

                            if (_uFUAAuditDataLog == null)
                                return ret;

                            if (!string.IsNullOrEmpty(seriesettings.NodeID) && _uFUAAuditDataLog != null)
                                bUseOid = true;
                            bool useDateTime = !(utcStart == minDateTimeValue && utcEnd == maxDateTimeValue);

                            if (!usesourcetimestamp)
                            {
                                int retry = 0;
                                while (true)
                                {
                                    try
                                    {
                                        ret.Values = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                      where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                                      (!useDateTime || (useDateTime && (entry.RecordDateTimeUtc >= utcStart) && (entry.RecordDateTimeUtc <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                      orderby entry.RecordDateTimeUtc descending
                                                      select new MyDataValue(
                                                          entry.RecordDateTimeUtc.ToLocalTime(),
                                                          entry.dValue
                                                        )
                                                      ).Take(maxrecord).ToList();
                                        //if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null)) /*&& (useDateTime || ret.Values.Count == 1)*/
                                        //{
                                        //    var pointBeforeVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                        //                                  where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                        //                                  entry.RecordDateTimeUtc < utcStart
                                        //                                  orderby entry.RecordDateTimeUtc descending
                                        //                                  select new MyDataValue(
                                        //                                      entry.RecordDateTimeUtc.ToLocalTime(),
                                        //                                      entry.dValue
                                        //                                    )
                                        //                  ).Take(1).ToList();
                                        //    var pointAfterVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                        //                                 where entry.Name == realTagname && entry.RecordDateTimeUtc != null &&
                                        //                                 entry.RecordDateTimeUtc > utcEnd
                                        //                                 orderby entry.RecordDateTimeUtc descending
                                        //                                 select new MyDataValue(
                                        //                                 entry.RecordDateTimeUtc.ToLocalTime(),
                                        //                                 entry.dValue
                                        //                               )
                                        //                  ).Take(1).ToList();
                                        //    InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame);
                                        //}
                                        break;
                                    }
                                    catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                    {
                                        if (++retry >= maxDeadLockRetry)
                                            throw ex;
                                    }
                                }
                            }
                            else
                            {
                                int retry = 0;
                                while (true)
                                {
                                    try
                                    {
                                        ret.Values = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                      where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                      (!useDateTime || (useDateTime && (entry.SourceTimeStamp >= utcStart) && (entry.SourceTimeStamp <= utcEnd))) && (!bUseOid || entry.DataLogRef == _uFUAAuditDataLog.Oid && bUseOid)
                                                      orderby entry.SourceTimeStamp descending
                                                      select new MyDataValue(
                                                          localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                          entry.dValue
                                                        )
                                                      ).Take(maxrecord).ToList();
                                        //if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null)) /*&& useDateTime*/
                                        //{
                                        //    var pointBeforeVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                        //                                  where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                        //                                  entry.SourceTimeStamp < utcStart
                                        //                                  orderby entry.SourceTimeStamp descending
                                        //                                  select new MyDataValue(
                                        //                                      localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                        //                                      entry.dValue
                                        //                                    )
                                        //                  ).Take(1).ToList();
                                        //    var pointAfterVisualRange = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                        //                                 where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                        //                                 entry.SourceTimeStamp > utcEnd
                                        //                                 orderby entry.SourceTimeStamp descending
                                        //                                 select new MyDataValue(
                                        //                                     localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                        //                                     entry.dValue
                                        //                                   )
                                        //                  ).Take(1).ToList();
                                        //    InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame);
                                        //}
                                        break;
                                    }
                                    catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                    {
                                        if (++retry >= maxDeadLockRetry)
                                            throw ex;
                                    }
                                }
                            }

                            settingStorage.mapSeries[serie].numPoints = ret.NumPoints = ret.Values.Count;
                            settingStorage.mapSeries[serie].numCompressRation = ret.NumCompressRation = 1;
                            if (ret.Values.Count > maxRecord)
                            {
                                int div = ret.Values.Count / maxRecord;

                                if (useDateTime && !bUseOid)
                                {
                                    List<MyDataValue> missingBefore;
                                    List<MyDataValue> missingAfter;
                                    int retry = 0;
                                    while (true)
                                    {
                                        try
                                        {
                                            missingBefore = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                             where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                             entry.SourceTimeStamp < utcStart
                                                             orderby entry.SourceTimeStamp descending
                                                             select new MyDataValue(
                                                                 localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                                 entry.dValue
                                                               )
                                                       ).Take(div).ToList();
                                            missingAfter = (from entry in new XPQuery<UFUAAuditDataItem>(_ufw)// .AsParallel()
                                                            where entry.Name == realTagname && entry.SourceTimeStamp != null &&
                                                            entry.SourceTimeStamp > utcEnd
                                                            orderby entry.SourceTimeStamp ascending
                                                            select new MyDataValue(
                                                                localize ? entry.SourceTimeStamp.ToLocalTime() : entry.SourceTimeStamp,
                                                                entry.dValue
                                                              )
                                                       ).Take(div).ToList();
                                            break;
                                        }
                                        catch (DevExpress.Xpo.DB.Exceptions.SqlExecutionErrorException ex)
                                        {
                                            if (++retry >= maxDeadLockRetry)
                                                throw ex;
                                        }
                                    }
                                    if (missingBefore.Count > 0)
                                        ret.Values.InsertRange(0, missingBefore);
                                    if (missingAfter.Count > 0)
                                        ret.Values.AddRange(missingAfter);
                                }

                                AggregatedValues retaggregated = new AggregatedValues();
                                retaggregated.Values = Aggregate(ret.Values, div);
                                settingStorage.mapSeries[serie].numCompressRation = ret.NumCompressRation = div;
                                settingStorage.mapSeries[serie].numCompressPoint = ret.NumCompressPoint = retaggregated.Values.Count;

                                return retaggregated;
                            }
                        }
                        catch (Exception ex)
                        {
                            //ShowError(ex);
                        }
                    }
                    #endregion
                }
            }
            else
            {
                string defaultDataProvider = null;
                string defaultConnectionString = null;

                var helper = new ConnectionStringParser(connStr);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType))
                {
                    defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(connStr);
                    defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(connStr);
                }
                else
                {
                    defaultDataProvider = helper.GetPartByName("DataProvider");
                    helper.RemovePartByName("DataProvider");
                    defaultConnectionString = helper.GetConnectionString();
                }

                #region Datalogger
                var minAgg = settingStorage.mapSeries[serie].MinAggregation;
                var maxAgg = settingStorage.mapSeries[serie].MaxAggregation;
                var avgAgg = settingStorage.mapSeries[serie].AvgAggregation;

                var useAggregatedTables = false; //NeedToAggregate(utcStart, utcEnd);
                if (useAggregatedTables && !minAgg && !maxAgg && !avgAgg)
                    return ret;
                //string postfix = aggTablePostFiss;
                string dlrname = historicalname;

                String _defaultDataProvider = defaultDataProvider;
                String _defaultConnectionString = defaultConnectionString;

                InitDataloggerConnections(sessionString);

                if (tagData != null)
                {
                    var dlrSettings = UpdateDlrSettings(dlrname);
                    name = (from c in dlrSettings[dlrname].Columns where TagPathHelper.GetTagPath(null, c.ColumnTagName, false) == tagName select c.Name).FirstOrDefault();
                    if (string.IsNullOrEmpty(name))
                        return ret;
                }

                string realTagname = name.Replace("/", ".");

                string dlConn = null;

                if (ct.IsCancellationRequested)
                    return ret;

                lock (MapToDatalogerConnectsions)
                {
                    if (MapToDatalogerConnectsions != null && MapToDatalogerConnectsions.ContainsKey(dlrname) && !string.IsNullOrEmpty(MapToDatalogerConnectsions[dlrname]))
                        dlConn = MapToDatalogerConnectsions[dlrname];
                }
                if (dlConn != null)
                {
                    try
                    {
                        var hlp = new ConnectionStringParser(dlConn);
                        string prvType = hlp.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                        if (!String.IsNullOrEmpty(prvType))
                        {
                            _defaultDataProvider = XpoConversionHelper.GetDataProviderFromXpoConnection(dlConn);
                            _defaultConnectionString = XpoConversionHelper.GetConnectionStringFromXpoConnection(dlConn);
                        }
                        else
                        {
                            _defaultDataProvider = hlp.GetPartByName("DataProvider");
                            hlp.RemovePartByName("DataProvider");
                            _defaultConnectionString = hlp.GetConnectionString();
                        }
                    }
                    catch (Exception)
                    {
                        _defaultDataProvider = defaultDataProvider;
                        _defaultConnectionString = defaultConnectionString;
                    }
                }

                if (string.IsNullOrEmpty(dlrname) || string.IsNullOrEmpty(realTagname) || string.IsNullOrEmpty(_defaultConnectionString) || string.IsNullOrEmpty(_defaultDataProvider))
                    return ret;

                DataLoggerColumnListControl.DataLoggerSettings ds = null;
                var setting = (from List<string> s in ufuaEditorComponent.GetDataLoggerSettings(projectDocument).ToList() where s[0] == historicalname select s).FirstOrDefault();
                if (setting != null)
                {
                    ds = new DataLoggerColumnListControl.DataLoggerSettings() { Name = setting[0], TableName = setting[1], UtcTimeColumnName = setting[2] };
                    ds.Columns = new DataLoggerColumnListControl.DataLoggerColumns();
                    var cols = ufuaEditorComponent.GetDataLoggerColumnSettingList(projectDocument, ds.Name) as List<List<String>>;
                    if (cols != null)
                    {
                        cols.ForEach(s =>
                        {
                            ds.Columns.Add(new DataLoggerColumnListControl.DataLoggerColumn() { Name = s[0], SourceTimeStampColumnName = s[1], AddSourceTimeStampColumn = bool.Parse(s[2]), ColumnTagName = s[3], ColumnTagGuid = s[4] });
                        });
                    }
                }

                DataLoggerColumnListControl.DataLoggerColumns columns;
                string _utccolumnname;
                string _tablename;
                
                if (ds == null)
                    return ret;

                _tablename = string.IsNullOrEmpty(ds.TableName) ? dlrname : ds.TableName;
                //if (useAggregatedTables)
                //    _tablename = $"{ _tablename}{postfix}";
                _utccolumnname = seriesettings.UseTableAggregation || string.IsNullOrEmpty(ds.UtcTimeColumnName) ? "UtcTimeCol" : ds.UtcTimeColumnName;
                columns = ds.Columns;

                var column = (from c in columns where c.Name == realTagname select c).FirstOrDefault();
                string _timeColumnName = !seriesettings.UseTableAggregation && usesourcetimestamp && column != null && column.AddSourceTimeStampColumn ? !string.IsNullOrEmpty(column.SourceTimeStampColumnName) ? $"{realTagname}_{column.SourceTimeStampColumnName}" : $"{realTagname}_SourceTimeStamp" : _utccolumnname;
                var minrealTagname = $"{realTagname}_MIN";
                var maxrealTagname = $"{realTagname}_MAX";
                var avgrealTagname = $"{realTagname}_AVG";

                var connection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString);
                using (var dbconnection = DataReader.DataReader.CreateDbConnection(_defaultDataProvider, _defaultConnectionString))
                {
                    try
                    {
                        dbconnection.Open();
                        var dbdapater = DataReader.DataReader.CreateDbDataAdapter(_defaultDataProvider);

                        DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(_defaultDataProvider, _defaultConnectionString);
                        dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(_defaultDataProvider);
                        dbdapater.SelectCommand.Connection = dbconnection;
                        dbdapater.SelectCommand.CommandTimeout = commandTimeout;

                        StringBuilder commantText = new StringBuilder("SELECT ");
                        DataSet gridDataSet = new DataSet();

                        if (dbSchemaInfo.IsSupportedTopKeyword)
                            commantText.AppendFormat("TOP {0} ", maxrecord);
                        commantText.AppendFormat("{0}", dbSchemaInfo.WrapObjectName(_timeColumnName));

                        if (useAggregatedTables)
                        {
                            if (minAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(minrealTagname));
                            if (maxAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(maxrealTagname));
                            if (avgAgg)
                                commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(avgrealTagname));
                        }
                        else
                            commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(realTagname));


                        commantText.AppendFormat(" FROM {0} WHERE {1} IS NOT NULL",
                            dbSchemaInfo.WrapObjectName(_tablename),
                            dbSchemaInfo.WrapObjectName(_timeColumnName));

                        if (!String.IsNullOrEmpty(conditionalString.Trim()))
                            commantText.AppendFormat(" AND {0}", conditionalString);

                        if (useAggregatedTables)
                        {
                            if (minAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(minrealTagname));
                            if (maxAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(maxrealTagname));
                            if (avgAgg)
                                commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(avgrealTagname));
                        }
                        else
                            commantText.AppendFormat(" AND {0} IS NOT NULL", dbSchemaInfo.WrapObjectName(realTagname));


                        if (!(utcStart == minDateTimeValue && utcEnd == maxDateTimeValue))
                        {
                            var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                            datestart.DbType = System.Data.DbType.DateTime;
                            datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                            datestart.Value = utcStart;
                            dbdapater.SelectCommand.Parameters.Add(datestart);

                            var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                            dateend.DbType = System.Data.DbType.DateTime;
                            dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                            dateend.Value = utcEnd;
                            dbdapater.SelectCommand.Parameters.Add(dateend);

                            commantText.AppendFormat(" AND {0} >= {1} AND {0} <= {2} AND {0} IS NOT NULL",
                                dbSchemaInfo.WrapObjectName(_timeColumnName),
                                datestart.ParameterName,
                                dateend.ParameterName);

                        }

                        commantText.AppendFormat(" ORDER BY {0} DESC",
                            dbSchemaInfo.WrapObjectName(_timeColumnName));

                        dbdapater.SelectCommand.CommandText = commantText.ToString();
                        dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);
                        dbdapater.Fill(gridDataSet, _tablename);


                        using (DataView dataView = new DataView(gridDataSet.Tables[0]))
                        {
                            dataView.Sort = string.Format("{0} DESC", _timeColumnName);

                            foreach (DataRowView rowView in dataView)
                            {
                                if (ct.IsCancellationRequested)
                                    break;
                                DateTime date = (DateTime)rowView[_timeColumnName];
                                var sourcetimestamp = (usesourcetimestamp && localize) || !usesourcetimestamp ? date.ToLocalTime() + diff : date + diff;
                                if (useAggregatedTables)
                                {
                                    if (minAgg && gridDataSet.Tables[0].Columns.Contains(minrealTagname))
                                        ret.MinValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[minrealTagname])));
                                    if (maxAgg && gridDataSet.Tables[0].Columns.Contains(maxrealTagname))
                                        ret.MaxValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[maxrealTagname])));
                                    if (avgAgg && gridDataSet.Tables[0].Columns.Contains(avgrealTagname))
                                        ret.AvgValues.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[avgrealTagname])));
                                }
                                else
                                    ret.Values.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[realTagname])));
                            }

                            if (ct.IsCancellationRequested)
                                return ret;

                            settingStorage.mapSeries[serie].numPoints = ret.NumPoints = dataView.Count;
                            settingStorage.mapSeries[serie].numCompressRation = ret.NumCompressRation = 1;
                            if (dataView.Count > maxRecord)
                            {
                                int div = dataView.Count / maxRecord;
                                AggregatedValues retaggregated = new AggregatedValues();

                                if (useAggregatedTables)
                                {
                                    if (ret.MinValues.Count > 0)
                                        retaggregated.MinValues = Aggregate(ret.MinValues, div/*, ct*/);
                                    if (ret.MaxValues.Count > 0)
                                        retaggregated.MaxValues = Aggregate(ret.MaxValues, div/*, ct*/);
                                    if (ret.AvgValues.Count > 0)
                                        retaggregated.AvgValues = Aggregate(ret.AvgValues, div/*, ct*/);
                                    settingStorage.mapSeries[serie].numCompressPoint = Math.Max(Math.Max(ret.MaxValues.Count, ret.MinValues.Count), ret.AvgValues.Count);
                                }
                                else
                                {
                                    if (ret.Values.Count > 0)
                                    {
                                        List<MyDataValue> missingBefore;
                                        List<MyDataValue> missingAfter;
                                        missingBefore = FillMissingValues(utcStart, utcEnd, dbdapater, dbSchemaInfo, _defaultDataProvider, div, _tablename, _timeColumnName, realTagname, diff, usesourcetimestamp, localize, true, ct);
                                        missingAfter = FillMissingValues(utcStart, utcEnd, dbdapater, dbSchemaInfo, _defaultDataProvider, div, _tablename, _timeColumnName, realTagname, diff, usesourcetimestamp, localize, false, ct);
                                        if (missingBefore.Count > 0)
                                            ret.Values.AddRange(missingBefore);
                                        if (missingAfter.Count > 0)
                                            ret.Values.InsertRange(0, missingAfter);
                                        retaggregated.Values = Aggregate(ret.Values, div/*, ct*/);
                                    }
                                    settingStorage.mapSeries[serie].numCompressPoint = ret.Values.Count;
                                }

                                settingStorage.mapSeries[serie].numCompressRation = div;

                                return retaggregated;
                            }
                            //else if (bAddVirtualPoints && (bAggregationChecked || visualRangeBeforeAggregationToggle == null))
                            //{
                            //    var pointBeforeVisualRange = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, 1, _tablename, _timeColumnName, realTagname, diff, usesourcetimestamp, localize, true);
                            //    var pointAfterVisualRange = FillMissingValues(dbdapater, dbSchemaInfo, _defaultDataProvider, 1, _tablename, _timeColumnName, realTagname, diff, usesourcetimestamp, localize, false);
                            //    InsertOuterPoints(ret.Values, pointBeforeVisualRange, pointAfterVisualRange, viewTimeFrame);
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        //ShowError(ex);
                    }
                }
                #endregion
            }
            var validValues = (from dv in ret.Values where dv.dValue != null select (double)dv.dValue);
            try
            {
                ret.minValue = validValues.Min();
                ret.maxValue = validValues.Max();

                if (bNeedsMedian)
                    ret.median = validValues.Median();
                if (bNeedsVariance)
                    ret.variance = validValues.Variance();
                if (bNeedsStdDev)
                    ret.standardDeviation = validValues.StandardDeviation();
            }
            catch { }
            return ret;
        }

        static double? GetNullOrValidDouble(object item)
        {
            double? val = null;
            if (item is DBNull)
                return val;
            try
            {
                val = Convert.ToDouble(item, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch { }
            return val;
        }

        static List<MyDataValue> FillMissingValues(DateTime utcStart, DateTime utcEnd, System.Data.Common.DbDataAdapter dbdapater, DbSchemaInfo dbSchemaInfo, string _defaultDataProvider, int div, string _tablename, string _timeColumnName, string realTagname, TimeSpan diff, bool usesourcetimestamp, bool localize, bool bBefore, System.Threading.CancellationToken ct)
        {
            if (utcStart == (DateTime)SqlDateTime.MinValue && utcEnd == (DateTime)SqlDateTime.MaxValue)
                return new List<MyDataValue>();

            List<MyDataValue> ret = new List<MyDataValue>();
            DataTable retTable = null;

            StringBuilder commantText = new StringBuilder("SELECT ");
            DataSet gridDataSet = new DataSet();

            if (dbSchemaInfo.IsSupportedTopKeyword)
                commantText.AppendFormat("TOP {0} ", div);

            commantText.AppendFormat("{0}", dbSchemaInfo.WrapObjectName(_timeColumnName));

            commantText.AppendFormat(", {0}", dbSchemaInfo.WrapObjectName(realTagname));

            commantText.AppendFormat(" FROM {0} WHERE {1} IS NOT NULL",
            dbSchemaInfo.WrapObjectName(_tablename),
            dbSchemaInfo.WrapObjectName(_timeColumnName));

            dbdapater.SelectCommand.Parameters.Clear();

            if (bBefore)
            {
                var datestart = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                datestart.DbType = System.Data.DbType.DateTime;
                datestart.ParameterName = dbSchemaInfo.FormatParameterName("DateStart");
                datestart.Value = utcStart;
                dbdapater.SelectCommand.Parameters.Add(datestart);
                commantText.AppendFormat(" AND {0} < {1} AND {0} IS NOT NULL",
                dbSchemaInfo.WrapObjectName(_timeColumnName),
                datestart.ParameterName);
            }
            else
            {
                var dateend = DataReader.DataReader.CreateDbParameter(_defaultDataProvider);
                dateend.DbType = System.Data.DbType.DateTime;
                dateend.ParameterName = dbSchemaInfo.FormatParameterName("DateEnd");
                dateend.Value = utcEnd;
                dbdapater.SelectCommand.Parameters.Add(dateend);

                commantText.AppendFormat(" AND {0} > {1} AND {0} IS NOT NULL",
                dbSchemaInfo.WrapObjectName(_timeColumnName),
                dateend.ParameterName);
            }

            commantText.AppendFormat(" ORDER BY {0} DESC",
                dbSchemaInfo.WrapObjectName(_timeColumnName));

            dbdapater.SelectCommand.CommandText = commantText.ToString();
            dbdapater.FillSchema(gridDataSet, SchemaType.Source, _tablename);

            if (dbSchemaInfo.IsSupportedTopKeyword)
            {
                dbdapater.Fill(gridDataSet, _tablename);
                retTable = gridDataSet.Tables[0];
            }
            else
                retTable = DataReader.DataReader.DataTableFromDataSet(gridDataSet, dbdapater, div);

            if (retTable != null)
            {
                using (DataView dataView = new DataView(retTable))
                {
                    dataView.Sort = string.Format("{0} DESC", _timeColumnName);

                    foreach (DataRowView rowView in dataView)
                    {
                        if (ct.IsCancellationRequested)
                            break;
                        DateTime date = (DateTime)rowView[_timeColumnName];
                        var sourcetimestamp = (usesourcetimestamp && localize) || !usesourcetimestamp ? date.ToLocalTime() + diff : date + diff;
                        ret.Add(new MyDataValue(sourcetimestamp, GetNullOrValidDouble(rowView[realTagname])));
                    }
                }
            }
            return ret;
        }

        private static bool NeedToAggregate(DateTime utcStart, DateTime utcEnd, SerieSettings serieSettings)
        {
            return CanUseAggregatedTables(serieSettings) &&
                (utcStart == utcEnd || (utcEnd - utcStart).TotalMinutes > 10) && (serieSettings.MinAggregation || serieSettings.MaxAggregation || serieSettings.AvgAggregation);
        }

        static bool CanUseAggregatedTables(SerieSettings serieSettings)
        {
            return !string.IsNullOrEmpty(serieSettings.HistoricalName) && serieSettings.UseTableAggregation
                && serieSettings.DLRSorce && UFUAEditorComponent.UsesAggreagatedTables(projectDocument, serieSettings.HistoricalName);
        }

        static List<MyDataValue> Aggregate(List<MyDataValue> values, int div)
        {
            var aggregated = new List<MyDataValue>();
            int i = 0;
            while (i * div < values.Count)
            {
                var list = values.Skip(i * div).Take(div).ToList();
                var average = list.Aggregate((acc, cur) => acc + cur) / list.Count;
                var timespan = list[list.Count - 1].SourceTimestamp - list[0].SourceTimestamp;
                var timespanaverage = new TimeSpan(0, 0, (int)timespan.TotalSeconds / 2);
                var time = list[0].SourceTimestamp + timespanaverage;
                aggregated.Add(new MyDataValue() { SourceTimestamp = time, dValue = average.dValue, bCompressed = true });
                i++;
            }
            return aggregated;

        }

        static public ServerAuditLogItem LoadHistoricalEventData(string connString, string sessionString, int filterEventType,
                                    long? startTimestamp, long? endTimestamp, DateSpan? rangeType, DateSpan defaultRangeType, int maxRows, int clientTimezoneOffset, CancellationToken ct)
        {
            var commandTimeout = 30;
            var ret = new ServerAuditLogItem();

            var types = FilterTypes.FilterType[(Filters)filterEventType];
            DateTime startTime;
            DateTime endTime;

            if (startTimestamp != null && endTimestamp != null)
            {
                startTime = DateTimeOffset.FromUnixTimeSeconds((long)startTimestamp).DateTime;
                endTime = DateTimeOffset.FromUnixTimeSeconds((long)endTimestamp).DateTime;
            }
            else
            {
                var dt = SetTimeSpan(rangeType != null ? (DateSpan)rangeType : defaultRangeType);
                startTime = dt.Item1;
                endTime = dt.Item2;
            }

            ret.StartDateTimeTicks = startTime.Ticks;
            ret.EndDateTimeTicks = endTime.Ticks;

            var connStr = String.IsNullOrEmpty(connString) ? PlatformComponents.eventsettings[PlatformComponents.GetProjectTitle()] : XpoHelpers.XpoHelper.NormalizeConnectionString(connString, projectDocument?.rootBase);
            if (String.IsNullOrEmpty(connStr))
                return ret;

            var _dl = GetHistoricalEventConnectionStringDataLayer(connStr, sessionString, commandTimeout);
            if (_dl == null)
                _dl = CreateHistoricalEventDataLayer(connStr, commandTimeout);

            if (ct.IsCancellationRequested)
                return ret;

            using (UnitOfWork _ufw = new UnitOfWork(_dl))
            {
                var utcStart = startTime.ToUniversalTime();
                var utcEnd = endTime.ToUniversalTime();
                if (utcStart < (DateTime)SqlDateTime.MinValue || utcStart > (DateTime)SqlDateTime.MaxValue)
                    utcStart = (DateTime)SqlDateTime.MinValue;
                if (utcEnd < (DateTime)SqlDateTime.MinValue || utcEnd > (DateTime)SqlDateTime.MaxValue)
                    utcEnd = (DateTime)SqlDateTime.MaxValue;

                if (utcStart == utcEnd)
                    ret.Items = (from entry in new XPQuery<UFUAAuditLogItem>(_ufw)//.AsParallel()
                            where types.Contains(entry.EventType)
                            orderby entry.EventDateTimeUtc descending
                            select new AuditLogItem(
                                  entry.EventDateTimeUtc,
                                  entry.EventType,
                                  entry.SourceName,
                                  entry.SourceNode,
                                  entry.UserName,
                                  entry.UtcRecordingTime,
                                  entry.Severity,
                                  entry.EventDateTimeUtc.AddMinutes(-clientTimezoneOffset), //entry.EventDateTime
                                  entry.EventComment,
                                  entry.EventDetails,
                                  entry.EventDuration,
                                  entry.EventMessage,
                                  entry.EventOccurence,
                                  entry.EventSequence,
                                  entry.EventState
                            )).Take(maxRows).ToList();
                else
                {
                    ret.Items = (from entry in new XPQuery<UFUAAuditLogItem>(_ufw)//.AsParallel()
                            where types.Contains(entry.EventType) &&
                                 ((entry.EventDateTimeUtc >= utcStart) && (entry.EventDateTimeUtc <= utcEnd))
                            orderby entry.EventDateTimeUtc descending
                            select new AuditLogItem(
                                  entry.EventDateTimeUtc,
                                  entry.EventType,
                                  entry.SourceName,
                                  entry.SourceNode,
                                  entry.UserName,
                                  entry.UtcRecordingTime,
                                  entry.Severity,
                                  entry.EventDateTimeUtc.AddMinutes(-clientTimezoneOffset), //entry.EventDateTime
                                  entry.EventComment,
                                  entry.EventDetails,
                                  entry.EventDuration,
                                  entry.EventMessage,
                                  entry.EventOccurence,
                                  entry.EventSequence,
                                  entry.EventState
                            )).Take(maxRows).ToList();
                }
            }
            return ret;
        }

        #endregion

        #region Report

        static public string SavePrintSendReport(String jsonoptions)
        {
            using (var report = PlatformComponents.CreateReportDocument(jsonoptions))
            {
                var detectedObject = JObject.Parse(jsonoptions);
                ExportFileType filetype = 0;
                if (detectedObject["filetype"] != null)
                    filetype = (ExportFileType)(int)detectedObject["filetype"];
                ReportCommandType type = 0;
                if (detectedObject["commandType"] != null)
                    type = (ReportCommandType)(int)detectedObject["commandType"];
                var folder = (string)detectedObject["folderpath"];
                var filename = (string)detectedObject["filename"];
                switch (type)
                {
                    case ReportCommandType.Save:

                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        switch (filetype)
                        {
                            case ExportFileType.Csv:
                                filename = String.Format("{0}.Csv", filename);
                                report.ExportToCsv(Path.Combine(folder, filename));
                                break;
                            case ExportFileType.Html:
                                filename = String.Format("{0}.Html", filename);
                                report.ExportToHtml(Path.Combine(folder, filename));
                                break;
                            case ExportFileType.Xls:
                                filename = String.Format("{0}.Xls", filename);
                                report.ExportToXls(Path.Combine(folder, filename));
                                break;
                            case ExportFileType.Pdf:
                                filename = String.Format("{0}.Pdf", filename);
                                report.ExportToPdf(Path.Combine(folder, filename));
                                break;
                            default:
                                throw new Exception(Properties.Resources.NoReportFileType);
                        }
                        break;
                    case ReportCommandType.Send:

                        var recipients = (string)detectedObject["recipient"];
                        var recipient = new Dictionary<String, String>();

                        var smtpsettings = UFUserEditorComponent.GetSMTPSettings(projectDocument);

                        if (!string.IsNullOrEmpty(recipients) && recipients.Split(':').Count() > 1)
                            recipient = UFUserEditorComponent.GetUsersEmail(projectDocument, recipients.Split(':')[1]);

                        if (recipient == null || recipient.Count() == 0 || smtpsettings == null || smtpsettings.Count != 7)
                        {
                            throw new Exception(Properties.Resources.MissingParameterExecutingReportCommand);
                        }

                        var from = (string)detectedObject["from"];
                        var fromalias = (string)detectedObject["fromalias"];
                        var subject = (string)detectedObject["mailsubject"];
                        var mobject = (string)detectedObject["mailobject"];
                        if (!IsValidEmailAddress(from))
                            from = smtpsettings[0];
                        if (String.IsNullOrEmpty(fromalias))
                            fromalias = Properties.Resources.FromAlias;

                        using (MemoryStream mem = new MemoryStream())
                        {
                            ServerSettings serverSettings = new ServerSettings();
                            MailSettings mail = new MailSettings();
                            switch (filetype)
                            {
                                case ExportFileType.Csv:
                                    report.ExportToCsv(mem);
                                    mem.Seek(0, System.IO.SeekOrigin.Begin);
                                    filename = String.Format("{0}.Csv", filename);
                                    mail.AddAttachment(mem, filename, "application/csv");
                                    break;
                                case ExportFileType.Html:
                                    report.ExportToHtml(mem);
                                    mem.Seek(0, System.IO.SeekOrigin.Begin);
                                    filename = String.Format("{0}.Html", filename);
                                    mail.AddAttachment(mem, filename, "application/html");
                                    break;
                                case ExportFileType.Xls:
                                    report.ExportToXls(mem);
                                    mem.Seek(0, System.IO.SeekOrigin.Begin);
                                    filename = String.Format("{0}.Xls", filename);
                                    mail.AddAttachment(mem, filename, "application/xls");
                                    break;
                                case ExportFileType.Pdf:
                                    report.ExportToPdf(mem);
                                    mem.Seek(0, System.IO.SeekOrigin.Begin);
                                    filename = String.Format("{0}.Pdf", filename);
                                    mail.AddAttachment(mem, filename, "application/pdf");
                                    break;
                                default:
                                    throw new Exception(Properties.Resources.NoReportFileType);
                            }
                            //smtpsettings[0].StaticFromAddress);
                            //smtpsettings[1].ServerAddress);
                            //smtpsettings[2].PortNumber.ToString());
                            //smtpsettings[3].EnAutentication.ToString());
                            //smtpsettings[4].UserName);
                            //smtpsettings[5].Password);

                            serverSettings.ServerAddress = smtpsettings[1];
                            serverSettings.ServerPort = int.Parse(smtpsettings[2]);
                            if (bool.Parse(smtpsettings[3]))
                            {
                                serverSettings.Password = smtpsettings[5];
                                serverSettings.User = smtpsettings[4];
                            }
                            if (fromalias.Length > 0)
                                mail.From = string.Format("{0} <{1}>", fromalias, from);
                            else
                                mail.From = from;

                            foreach (var key in recipient.Keys)
                            {
                                if (IsValidEmailAddress(recipient[key]))
                                    mail.Address.Add(recipient[key]);
                            }
                            mail.Subject = subject;
                            mail.Message = mobject;
                            mail.Name = report.DisplayName;
                            using (var smtp = new SmtpClientModule())
                            {
                                smtp.Init(serverSettings);
                                var result = smtp.SendMail(mail);
                                if (result != SmtpErrors.NoError)
                                    return smtp.ErrorMessage;
                            }
                        }
                        break;
                    case ReportCommandType.Print:
                        new PrintToolBase(report.PrintingSystem).Print();
                        break;

                    default:
                        throw new Exception(Properties.Resources.NoUnsupportedCommandType);
                }

                return null/*true*/;
            }
        }

        static public long timestampToTicks(ulong timestamp)
        {
            return (long)(timestamp * 10000 + 621355968000000000);
        }

        static bool IsValidEmailAddress(string s)
        {
            var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            return regex.IsMatch(s);
        }

        static public XtraReport CreateReportDocument(String jsonoptions)
        {
            var sourceType = DefaultSourceType.Undefined;

            var detectedObject = JObject.Parse(jsonoptions);
            var reportAlarmDoc = (string)detectedObject["ReportAlarmDoc"];
            if (reportAlarmDoc != null && !String.IsNullOrEmpty(reportAlarmDoc))
            {
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx/IoqCaj13T3tqZw/KR6gYQ=="/* STA */);
                if (state == false)
                {
                    throw new Exception(Properties.Resources.NoStatisticsLicense);
                }
#endif
                sourceType = DefaultSourceType.EventLog;
            }
            else
            {
#if !DEBUG
                var state = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNxcPspvRaFavRuuz2KyDjJ8w=="/* REP */);
                if (state == false)
                {
                    throw new Exception(Properties.Resources.NoReportLicense);
                }
#endif
            }

            var reportUrl = (string)detectedObject["reportUrl"];
            if (String.IsNullOrEmpty(reportUrl))
                reportUrl = (string)detectedObject["reportName"];

            var parameters = new List<ReportParameters.Parameter>();
            var parameterlist = detectedObject["parameters"];
            if (parameterlist != null)
            {
                foreach (var o in parameterlist)
                {
                    var parameter = new ReportParameters.Parameter();
                    parameters.Add(parameter);
                    parameter.Name = (String)o["Name"];
                    parameter.StringRef = (String)o["StringRef"];
                    parameter.TagRefXml = (String)o["TagRefXml"];
                    parameter.Type = (ReportParameters.ParameterType)(int)o["Type"];
                    parameter.Value = (String)o["Value"];
                }
            }

            ReportDocument reportDoc = null;
            int maxTake = 0;
            int commandTimeout = 0;
            if (reportAlarmDoc != null && !String.IsNullOrEmpty(reportAlarmDoc))
            {
                var periodType = ReportPeriodType.Today;
                try
                {
                    periodType = (ReportPeriodType)(int)detectedObject["PeriodType"];
                }
                catch { }

                DateTime dateFrom = DateTime.MinValue;
                DateTime dateTo = DateTime.MinValue;
                switch (periodType)
                {
                    case ReportPeriodType.Today:
                        dateFrom = DateTime.Now.AddDays(-1);
                        dateTo = DateTime.Now;
                        break;
                    case ReportPeriodType.Yesterday:
                        dateFrom = DateTime.Now.AddDays(-2);
                        dateTo = DateTime.Now;
                        break;
                    case ReportPeriodType.LastWeek:
                        dateFrom = DateTime.Now.AddDays(-7);
                        dateTo = DateTime.Now;
                        break;
                    case ReportPeriodType.LastMonth:
                        dateFrom = DateTime.Now.AddMonths(-1);
                        dateTo = DateTime.Now;
                        break;
                    case ReportPeriodType.LastYear:
                        dateFrom = DateTime.Now.AddYears(-1);
                        dateTo = DateTime.Now;
                        break;
                    default:
                        try
                        {
                            dateFrom = (DateTime)detectedObject["StartDate"];
                            dateTo = (DateTime)detectedObject["EndDate"];
                        }
                        catch { }
                        break;
                }

                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.DateFrom, Type = ReportParameters.ParameterType.DateTime, Value = dateFrom });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.DateTo, Type = ReportParameters.ParameterType.DateTime, Value = dateTo });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.Period, Type = ReportParameters.ParameterType.Integer, Value = (int)periodType });
                parameters.Add(new ReportParameters.Parameter() { Name = ReportAlarms.ParameterNames.Source, Type = ReportParameters.ParameterType.String, Value = detectedObject["AlarmsSource"] });

                try
                {
                    maxTake = (int)detectedObject["MaxTake"];
                }
                catch { }

                try
                {
                    commandTimeout = (int)detectedObject["CommandTimeout"];
                }
                catch { }

                var reportType = AlarmReportType.OrderByDateTime;
                try
                {
                    reportType = (AlarmReportType)(int)detectedObject["ReportType"];
                }
                catch { }

                var reportalarmtype = new ReportAlarms.ReportAlarms(reportType, projectDocument);
                reportDoc = reportalarmtype.GetReportDocument();
            }
            else
            {
                var tuple = PlatformComponents.MakeAbsolute(reportUrl);
                reportUrl = PlatformComponents.CompleteWithExtensionReport(tuple.Item1);

                reportDoc = ReportDocument.FromFile(reportUrl, PlatformComponents.GetProjectDocument());
            }

            reportDoc.Parent = PlatformComponents.GetProjectDocument();
            var reportService = new ReportServiceHelper(reportDoc, PlatformComponents.UFUAEditorComponent, sourceType);
            if (maxTake > 0)
                reportService.MaxTake = maxTake;
            if (commandTimeout > 0)
                reportService.CommandTimeout = commandTimeout;
            var report = reportService.PrepareXtraReportDocument(parameters);
            report.Extensions[SerializationService.Guid] = CustomUntypedDataSetSerializer.Name;
            report.CreateDocument();
            return report;
        }

        #endregion

        #region Log Viewer
        static public void LoadXMLLogFile(LogItems ret, string logFilePath)
        {
            var nt = new NameTable();
            var mgr = new XmlNamespaceManager(nt);
            var log4j = "http://example.com/log4j";
            mgr.AddNamespace("log4j", log4j);
            var log4net = "http://example.com/log4net";
            mgr.AddNamespace("log4net", log4net);
            var pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);
            var settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };

            //var content = File.ReadAllText(logFilePath);
            //doc.LoadXml(content);
            XNamespace log4jNs = log4j;
            XNamespace log4netNs = log4net;
            var currentItemIndex = 1;
            using (XmlReader xr = XmlReader.Create(logFilePath, settings, pc))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.LocalName == "event")
                    {
                        using (XmlReader eventReader = xr.ReadSubtree())
                        {
                            eventReader.Read();
                            XElement eventEl = XNode.ReadFrom(eventReader) as XElement;
                            var logger = (string)eventEl.Attribute("logger");
                            if (!ret.Loggers.Contains(logger))
                                ret.Loggers.Add(logger);
                            long timestamp; 
                            try
                            {
                                timestamp = (long)eventEl.Attribute("timestamp");
                            }
                            catch (FormatException)
                            {
                                timestamp = ((DateTimeOffset)DateTime.Parse(eventEl.Attribute("timestamp").Value)).ToUnixTimeSeconds();
                            }
                            var propElement = eventEl.Element(log4jNs + "properties");
                            IEnumerable<XElement> propData;
                            if (propElement == null)
                            {
                                ret.Schema = "log4net";
                                propData = eventEl.Element(log4netNs + "properties").Elements(log4netNs + "data");
                            }
                            else
                                propData = propElement.Elements(log4jNs + "data");
                            var logItem = new LogItem()
                            {
                                Item = currentItemIndex,
                                Logger = logger,
                                TimeStamp = timestamp,
                                Level = (string)eventEl.Attribute("level"),
                                Thread = (string)eventEl.Attribute("thread"),
                                Message = ret.Schema == "log4net" ? (string)eventEl.Element(log4netNs + "message") : (string)eventEl.Element(log4jNs + "message")
                            };
                            if (ret.Schema == "log4net")
                                logItem.App = (string)eventEl.Attribute("domain");
                            
                            string details = String.Empty;
                            if (ret.Schema == "log4net")
                                details = (string)eventEl.Element(log4netNs + "exception");
                            else
                                details = (string)eventEl.Element(log4j + "throwable");
                            logItem.Details = details;

                            foreach (var data in propData)
                            {
                                switch ((string)data.Attribute("name"))
                                {
                                    case "log4japp":
                                        logItem.App = (string)data.Attribute("value");
                                        break;
                                    case "log4net:UserName":
                                        logItem.UserName = (string)data.Attribute("value");
                                        break;
                                    case "log4jmachinename":
                                        logItem.MachineName = (string)data.Attribute("value");
                                        break;
                                    case "log4net:HostName":
                                        logItem.HostName = (string)data.Attribute("value");
                                        if (ret.Schema == "log4net")
                                            logItem.MachineName = (string)data.Attribute("value");
                                        break;
                                    case "log4net:Identity":
                                        logItem.Identity = (string)data.Attribute("value");
                                        break;
                                    case "NDC":
                                        logItem.NDC = (string)data.Attribute("value");
                                        break;
                                }
                            }
                            ret.Items.Add(logItem);
                            eventReader.Close();
                        }
                        currentItemIndex++;
                    }
                }
                xr.Close();
            }
        }
        #endregion
        static public DecoderData GetOrCreateMjpegDecoder(string uriId, Uri uri, string user, string password, int closeTimeout)
        {
            MjpegDecoder newDecoder = null;
            lock (mapDecoders)
            {
                if (!mapDecoders.ContainsKey(uriId))
                {
                    SetIPCameraTimer();
                    newDecoder = new MjpegDecoder(TimeSpan.FromMilliseconds(closeTimeout), uriId);
                    mapDecoders[uriId] = new DecoderData(newDecoder, uriId, uri, user, password, closeTimeout, IPCameraKeepAliveTimeout);
                    activeMjpegDecoders[uriId] = 0;
                }
                activeMjpegDecoders[uriId]++;
                mapDecoders[uriId].UpdateLatestRequestTimestamp();
            }
            newDecoder?.ParseStream(uri, user, password);
            return mapDecoders[uriId];
        }
        static void SetIPCameraTimer()
        {
            if (IPCameraKeepAliveTimer == null)
            {
                IPCameraKeepAliveTimer = new System.Timers.Timer(keepAliveTimerTick);
                IPCameraKeepAliveTimer.Elapsed += (o, e) =>
                {
                    lock (mapDecoders)
                    {
                        if (mapDecoders.Count == 0 && IPCameraKeepAliveTimer.Enabled)
                            IPCameraKeepAliveTimer.Enabled = false;
                        else
                        {
                            foreach (var decoderData in mapDecoders.Values)
                                if (decoderData.IsKeepAliveExpired())
                                    DisposeMjpegDecoder(decoderData.UriID);

                        }
                    }
                };
                IPCameraKeepAliveTimer.AutoReset = true;
            }
            if (!IPCameraKeepAliveTimer.Enabled)
                IPCameraKeepAliveTimer.Enabled = true;
        }
        static public DecoderData GetMjpegDecoder(string uriId)
        {
            DecoderData ret = null;
            lock (mapDecoders)
            {
                if (mapDecoders.ContainsKey(uriId))
                    ret = mapDecoders[uriId];
            }
            return ret;
        }

        static public void CheckDisposeMjpegDecoder(string uriId, bool bForcedStreamDispose = false)
        {
            lock (mapDecoders)
            {
                if (mapDecoders.ContainsKey(uriId))
                {
                    activeMjpegDecoders[uriId] = bForcedStreamDispose ? 0 : activeMjpegDecoders[uriId] - 1;
                    if (activeMjpegDecoders[uriId] == 0)
                        DisposeMjpegDecoder(uriId);
                }
            }
        }

        static void DisposeMjpegDecoder(string uriId)
        {
            MjpegDecoder decoder = null;
            lock (mapDecoders)
            {
                if (!mapDecoders.ContainsKey(uriId))
                    return;

                activeMjpegDecoders.Remove(uriId);
                decoder = mapDecoders[uriId].Decoder;
                mapDecoders.Remove(uriId);
            }
            decoder?.Dispose();
        }
    }
}
