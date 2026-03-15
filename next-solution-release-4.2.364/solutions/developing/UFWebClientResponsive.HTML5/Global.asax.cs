using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using DevExpress.Web;
using log4net;
using Utilities;
using WPFUtilities;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using DevExpress.DashboardWeb.Designer;
using DocumentManager.ComponentService;
using DevExpress.DashboardWeb;
using UFWebClientResponsive.HTML5;
using System.Configuration;
using log4net.Config;
using Mindscape.Raygun4Net;
using DevExpress.XtraReports.Security;

namespace UFWebClient.HTML5
{
    public class Global : System.Web.HttpApplication
    {
        static internal int DemoModeCountDown = 5;
        static internal int DemoModeClickCounter = 10;

        static public Uri defaultUri { get; protected set; }
        static public String ClientSessionName { get; protected set; }
        static public int RefreshPollingTime { get; protected set; }
        static public int RefreshPollingTimeCount { get; protected set; }
        static public int DelayBroadcaster { get; protected set; }
        static public int SessionTimeout { get; protected set; }
        static public int ConcurrentRenderingPipeline { get; protected set; }
        static public String Theme { get; protected set; }
        static public bool LowResolution { get; protected set; }
        static public bool DisablePopupScreen { get; protected set; }
        static public bool DisableAlertOnCommandExecution { get; protected set; }
        static public bool DisableStaticOptimization { get; protected set; }
        static public bool AllowReportScriptExecution { get; protected set; }
        static public bool AllowReportCodeFormat { get; protected set; }
        static public bool ShowProjectTitle { get; protected set; }

        static public bool DisableCreateNewDashboard { get; protected set; }

        static public bool DisableExecutingCustomSqlDashboard { get; protected set; }
        
        static public bool SwitchToViewerDashboard { get; protected set; }

        static public bool ShowHeader { get; protected set; }
        static public String ProjectTitle { get; protected set; }
        static public bool ShowScreenNavigator { get; protected set; }
        static public bool ShowPopupCloseBtn { get; protected set; }
        static public String LogoUrl { get; protected set; }

        static public bool HideUserRegister { get; protected set; }
        
        static public bool HideDashboardMenuItem { get; protected set; }
        static public bool HideAlarmMenuItem { get; protected set; }
        static public bool HideDataAnalisysMenuItem { get; protected set; }
        static public bool HideDataGridMenuItem { get; protected set; }
        static public bool HideReportMenuItem { get; protected set; }

        static public int MaxAlarmsStatisticData { get; protected set; }

        static public Uri currentPage { get; set; }
        static public UFProjectManager.UFProjectDocument projectDocument { get; protected set; }
        static public List<TileInfo> listTiles = new List<TileInfo>();
        static public List<TileInfo> listTilesFlat = new List<TileInfo>();
        static public Dictionary<String, IList<TileInfo>> mapTiles = new Dictionary<String, IList<TileInfo>>();
        static List<String> temporaryFiles = new List<String>();

        static ComponentHost componentHost = new ComponentHost();
        static PluginServices Plugins = new PluginServices();

        static Dictionary<String, ScreenSinkServiceSession> mapCurrentSessions = new Dictionary<String, ScreenSinkServiceSession>();
        static public int AddSession(String id, ScreenSinkServiceSession session)
        {
            RemoveSession(id);
            lock (mapCurrentSessions)
            {
                mapCurrentSessions.Add(id, session);
                log.Info(String.Format(Properties.Resources.NewSession, id, mapCurrentSessions.Count));
                return mapCurrentSessions.Count;
            }
        }
        static public int RemoveSession(String id)
        {
            ScreenSinkServiceSession session = null;
            lock (mapCurrentSessions)
            {
                if (mapCurrentSessions.ContainsKey(id))
                {
                    session = mapCurrentSessions[id];
                    mapCurrentSessions.Remove(id);
                    log.Info(String.Format(Properties.Resources.SessionEnd, id, mapCurrentSessions.Count));
                }

                return mapCurrentSessions.Count;
            }

            if (session != null)
                session.Dispose();
        }
        static public ScreenSinkServiceSession GetSession(String id)
        {
            lock (mapCurrentSessions)
            {
                if (mapCurrentSessions.ContainsKey(id))
                    return mapCurrentSessions[id];
                return null;
            }
        }
        static void RemoveAllSessions()
        {
            var sessions = new List<ScreenSinkServiceSession>();
            lock (mapCurrentSessions)
            {
                sessions = mapCurrentSessions.Values.ToList();
                mapCurrentSessions.Clear();
            }

            // first release all license sessions.
            foreach (var session in sessions)
                session.ScreenSink.ReleaseLicense();

            // next dispose all sessions.
            foreach (var session in sessions)
                session.Dispose();
        }

        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.WebClientHTML5Log);

        static ScreenManager.ComponentService.ScreenManagerComponent screenComponent = new ScreenManager.ComponentService.ScreenManagerComponent();
        static public ScreenManager.ComponentService.ScreenManagerComponent ScreenComponent
        {
            get
            {
                return screenComponent;
            }
        }

        static UFRecipeEditor.ComponentService.RecipeEditorManagerComponent recipeComponent = new UFRecipeEditor.ComponentService.RecipeEditorManagerComponent();
        static public UFRecipeEditor.ComponentService.RecipeEditorManagerComponent RecipeComponent
        {
            get
            {
                return recipeComponent;
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

        static UFUAEditor.ComponentService.UFUAEditorManagerComponent ufuaEditorComponent = new UFUAEditor.ComponentService.UFUAEditorManagerComponent();
        static public UFUAEditor.ComponentService.UFUAEditorManagerComponent UFUAEditorComponent
        {
            get
            {
                return ufuaEditorComponent;
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

        static UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent crossReferenceEditorManagerComponent = new UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent();
        static public UFCrossReferenceEditor.ComponentService.CrossReferenceEditorManagerComponent CrossReferenceEditorManagerComponent
        {
            get
            {
                return crossReferenceEditorManagerComponent;
            }
        }

        static OPCUAClientStatus.ComponentService.OPCUABrowserComponent oPCUABrowserComponent = new OPCUAClientStatus.ComponentService.OPCUABrowserComponent();
        static public OPCUAClientStatus.ComponentService.OPCUABrowserComponent OPCUABrowserComponent
        {
            get
            {
                return oPCUABrowserComponent;
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

        static UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent unitConverterEditorComponent = new UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent();
        static public UnitConverterManager.ComponentService.UnitConverterEditorManagerComponent UnitConverterEditorComponent
        {
            get
            {
                return unitConverterEditorComponent;
            }
        }

        static ScriptManager.ComponentService.ScriptManagerComponent scriptManagerComponent = new ScriptManager.ComponentService.ScriptManagerComponent();
        static public ScriptManager.ComponentService.ScriptManagerComponent ScriptManagerComponent
        {
            get
            {
                return scriptManagerComponent;
            }
        }

        static MSEditor.ComponentService.SchedulerEditorManagerComponent schedulerEditorComponent = new MSEditor.ComponentService.SchedulerEditorManagerComponent();
        static public MSEditor.ComponentService.SchedulerEditorManagerComponent SchedulerEditorComponent
        {
            get
            {
                return schedulerEditorComponent;
            }
        }

        public static Dictionary<String, String> historiansettings = new Dictionary<String, String>();
        public static Dictionary<String, String> eventsettings = new Dictionary<String, String>();
        public static Dictionary<String, String> serverentitysettings = new Dictionary<String, String>();
        public static Dictionary<String, String> schedulersettings = new Dictionary<String, String>();

        static T GetSettingValue<T>(KeyValueConfigurationCollection settings, String key, T defaultValue)
        {
            var value = settings[key];
            if (value != null)
            {
                try
                {
                    defaultValue = (T)Convert.ChangeType(value.Value, typeof(T));
                }
                catch (Exception ex)
                {

                }
            }

            return defaultValue;
        }

        static bool bLogExceptions = true;
        static Global()
        {
            DevExpress.Xpf.Core.ApplicationThemeHelper.UseLegacyDefaultTheme = true;
            RegistryKeysHelper.ReadLogException(ref bLogExceptions);
        }

        void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();

            DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;

            // Code that runs on application startup
            var rootWebConfig =
                            System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var settings = rootWebConfig.AppSettings.Settings;
            
            var uri = settings["Uri"];
            if (uri != null)
                defaultUri = new Uri(uri.Value, UriKind.RelativeOrAbsolute);
            var clientSessionName = settings["ClientSessionName"];
            if (clientSessionName != null)
                ClientSessionName = clientSessionName.Value;
            var theme = settings["Theme"];
            if (theme != null)
                Theme = theme.Value;

            RefreshPollingTime = GetSettingValue(settings, "RefreshPollingTime", 1000);
            RefreshPollingTimeCount = GetSettingValue(settings, "RefreshPollingTimeCount", RefreshPollingTimeCount);
            DelayBroadcaster = GetSettingValue(settings, "DelayBroadcaster", 100);
            SessionTimeout = GetSettingValue(settings, "SessionTimeout", SessionTimeout);
            ConcurrentRenderingPipeline = GetSettingValue(settings, "ConcurrentRenderingPipeline", ConcurrentRenderingPipeline);
            var softwareRendering = GetSettingValue(settings, "SoftwareRendering", false);
            if (softwareRendering)
                ScreenManager.ComponentService.ScreenManagerComponent.EnableSoftwareRendering();
            LowResolution = GetSettingValue(settings, "LowResolution", LowResolution);
            DisablePopupScreen = GetSettingValue(settings, "DisablePopupScreen", DisablePopupScreen);
            DisableAlertOnCommandExecution = GetSettingValue(settings, "DisableAlertOnCommandExecution", DisableAlertOnCommandExecution);
            DisableStaticOptimization = GetSettingValue(settings, "DisableStaticOptimization", DisableStaticOptimization);
            AllowReportScriptExecution = GetSettingValue(settings, "AllowReportScriptExecution", true);
            AllowReportCodeFormat = GetSettingValue(settings, "AllowReportCodeFormat", false);
            ShowProjectTitle = GetSettingValue(settings, "ShowProjectTitle", true);
            DisableCreateNewDashboard = GetSettingValue(settings, "DisableCreateNewDashboard", DisableCreateNewDashboard);
            DisableExecutingCustomSqlDashboard = GetSettingValue(settings, "DisableExecutingCustomSqlDashboard", DisableExecutingCustomSqlDashboard);
            SwitchToViewerDashboard = GetSettingValue(settings, "SwitchToViewerDashboard", SwitchToViewerDashboard);
            ProjectTitle = GetSettingValue(settings, "ProjectTitle", ProjectTitle);
            ShowScreenNavigator = GetSettingValue(settings, "ShowScreenNavigator", true);
            ShowPopupCloseBtn = GetSettingValue(settings, "ShowPopupCloseBtn", true);
            ShowHeader = GetSettingValue(settings, "ShowHeader", true);
            LogoUrl = GetSettingValue(settings, "LogoUrl", LogoUrl);
            HideUserRegister = GetSettingValue(settings, "HideUserRegister", false);

            HideDashboardMenuItem = GetSettingValue(settings, "HideDashboardMenuItem", false);
            HideAlarmMenuItem = GetSettingValue(settings, "HideAlarmMenuItem", false);
            HideDataAnalisysMenuItem = GetSettingValue(settings, "HideDataAnalisysMenuItem", false);
            HideDataGridMenuItem = GetSettingValue(settings, "HideDataGridMenuItem", false);
            HideReportMenuItem = GetSettingValue(settings, "HideReportMenuItem", false);

            MaxAlarmsStatisticData = GetSettingValue(settings, "MaxAlarmsStatisticData", 0);

            projectDocument = UFProjectManager.UFProjectDocument.FromFile(Global.defaultUri.OriginalString, UFProjectManagerComponent);
            if (projectDocument == null)
                throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

            projectDocument.PreSubscribeServerSession(UFUAEditorComponent);

            if (AllowReportScriptExecution)
                ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(DevExpress.XtraReports.Security.ExecutionMode.Unrestricted);
            if (AllowReportCodeFormat)
            {
                DevExpress.Security.Resources.AccessSettings.ReportingSpecificResources.SetRules(
                    DevExpress.XtraReports.Security.SerializationFormatRule.Allow(
                   DevExpress.XtraReports.UI.SerializationFormat.Code,
                   DevExpress.XtraReports.UI.SerializationFormat.Xml));
            }

            projectDocument.SetCurrentLogFileName();

#if !DEBUG
            WinWrap.Basic.Util.DllDir = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "bin\\Libraries");
#endif

            log.Info(Properties.Resources.AppStarting);

            componentHost.Components.Add(UFProjectManagerComponent);
            componentHost.Components.Add(ClientEditorManagerComponent);
            componentHost.Components.Add(UFUAEditorComponent);
            componentHost.Components.Add(ScreenComponent);
            componentHost.Components.Add(RecipeComponent);
            componentHost.Components.Add(ReportComponent);
            componentHost.Components.Add(UFUserEditorComponent);
            componentHost.Components.Add(StringEditorComponent);
            componentHost.Components.Add(UnitConverterEditorComponent);
            componentHost.Components.Add(CrossReferenceEditorManagerComponent);
            componentHost.Components.Add(OPCUABrowserComponent);
            componentHost.Components.Add(ScriptManagerComponent);

            UpdateProjectSettings(projectDocument);

            Plugins.FindPlugins(String.Format("{0}\\bin\\DataSinks\\", AppDomain.CurrentDomain.BaseDirectory));

            foreach (var pluginOn in Plugins.AvailablePlugins)
            {
                try
                {
                    ((UFInterfaces.Types.AvailablePlugin)pluginOn).Instance.Initialize();
                }
                catch (Exception ex)
                {
                    log.Error(Properties.Resources.FailedToInitializePlugin, ex);
                }
            }

            //OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            //OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();
            OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
#if !DEBUG
            MSZ.MSZView.KeyChangeEvent += (ob, ev) =>
            {
                OPCUAViewModel.OPCUAEntityReference.UpdateLicenseSerialNumber();
            };
#endif

            ScreenComponent.LoadRequiredAssemblies();
            StringEditorComponent.LoadRuntimeStrings(projectDocument);
            if (projectDocument.GetWholeDocumentLists((IDocumentManager)RecipeComponent).Count > 0)
                RecipeComponent.PreSubscribeServerSession(projectDocument);

            InitProjectTilesMap();

            currentPage = projectDocument.GetStartupScreen(Global.ScreenComponent);
            if (currentPage == null)
                throw new ArgumentNullException("Missing default uri to load !");

            //var dashboardStorage = new DashboardFileStorage(@"~/App_Data");
            //DashboardConfigurator.Default.SetDashboardStorage(dashboardStorage);
        }

        static void UpdateProjectSettings(UFProjectManager.UFProjectDocument doc, UFProjectManager.UFProjectDocument parent = null)
        {
            doc.Parent = parent;
            doc.UpdateSessionSettings();

            historiansettings[doc.Title] = UFUAEditorComponent.GetHistorianDefaultConnection(doc);
            eventsettings[doc.Title] = UFUAEditorComponent.GetEventDefaultConnection(doc);
            serverentitysettings[doc.Title] = UFUAEditorComponent.GetServerEntityReference(doc);
            schedulersettings[doc.Title] = SchedulerEditorComponent.GetServerEntityReference(doc);

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

        void Application_End(object sender, EventArgs e)
        {
            log.Info(Properties.Resources.AppEnd);

            //  Code that runs on application shutdown
            if (projectDocument != null)
            {
                RecipeComponent.UnsubscribeServerSession(projectDocument);
                projectDocument.UnsubscribeServerSession();
                projectDocument.Dispose();
                projectDocument = null;
            }

            try
            {
                RemoveAllSessions();
            }
            catch
            { }

            TryDeleteTemporaryFile();
        }

        void Application_Error(object sender, EventArgs e)
        {
            RemoveAllSessions();

            var exception = Server.GetLastError();
            HttpException httpException = exception as HttpException;
            if (httpException != null)
                log.Error(Properties.Resources.AppError, exception);
            else
            {
                log.Fatal(Properties.Resources.AppError, exception);
#if !DEBUG
                if (bLogExceptions)
                    new RaygunClient().Send(exception);
#endif
                TryDeleteTemporaryFile();
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            //log.Info(Properties.Resources.NewSession);
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            //log.Info(Properties.Resources.SessionEnd);
        }

        static TileInfo AddTileInfo(IScreenController controller, Uri uri, List<TileInfo> parenttileInfo)
        {
            var tileInfo = new TileInfo() { Section = controller.GetTitle() };
            uri = Global.projectDocument.MakeRelativeUri(uri, Global.ScreenComponent);
            if (Global.projectDocument.IsVisible(uri))
            {
                // tileInfo.Name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                tileInfo.Name = System.IO.Path.ChangeExtension(uri.GetPathString(), null);
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}/", Global.ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}\\", Global.ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace('/', ' ');
                tileInfo.Name = tileInfo.Name.Replace('\\', ' ');

                var color = controller.GetIdentityColor(uri);
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
                var uri2 = Global.projectDocument.MakeAbosoluteUri(image, Global.ScreenComponent);
                if (uri2 != null && uri2.IsAbsoluteUri)
                {
#if !DEBUG
                                    uri2 = Global.projectDocument.MakeRelativeUri(uri2, AppDomain.CurrentDomain.BaseDirectory);
                                    if (uri2 != null)
#endif
                    tileInfo.ImageUrl = uri2.ToString();
                }
                else
                {
                    uri2 = Global.projectDocument.ExportRelativeUriToTemporaryFile(uri2, "png");
                    if (uri2 != null)
                    {
                        tileInfo.ImageUrl = uri2.ToString();
                        temporaryFiles.Add(uri2.GetPathString());
                    }
                }
                tileInfo.Section = controller.GetTitle();
                //tileInfo.Section = tileInfo.Name;
                //tileInfo.Section = tileInfo.Section.Replace('/', '_');
                //tileInfo.Section = tileInfo.Section.Replace('\\', '_');

                tileInfo.HasGeoCoordinates = controller.GetHasGeoCoordinates(uri);
                tileInfo.Latitude = controller.GetLatitude(uri);
                tileInfo.Longitude = controller.GetLongitude(uri);

                tileInfo.UsersVisibility = controller.GetUsersVisibility(uri);
                tileInfo.RolesVisibility = controller.GetRolesVisibility(uri);

                parenttileInfo.Add(tileInfo);
                listTilesFlat.Add(tileInfo);
            }

            return tileInfo;
        }

        static void AddTileInfos(IScreenController controller, List<TileInfo> list)
        {
            controller.GetScreenLists(Global.ScreenComponent).ForEach(uri =>
            {
                AddTileInfo(controller, uri, list);
            });

            controller.GetScreenControllers(Global.ScreenComponent).ForEach(c =>
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
            Global.projectDocument.GetScreenControllers(Global.ScreenComponent).ForEach(controller =>
            {
                AddTileInfos(controller);

                var tileinfo = new TileInfo() { Section = controller.GetTitle() };
                listTiles.Add(tileinfo);
                AddTileInfos(controller, tileinfo.childs);
            });
        }

        static void AddTileInfo(IScreenController controller, Uri uri)
        {
            var tileInfo = new TileInfo();
            uri = Global.projectDocument.MakeRelativeUri(uri, Global.ScreenComponent);
            if (Global.projectDocument.IsVisible(uri))
            {
                // tileInfo.Name = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
                tileInfo.Name = System.IO.Path.ChangeExtension(uri.GetPathString(), null);
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}/", Global.ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace(String.Format("{0}\\", Global.ScreenComponent.TypeLabel), "");
                tileInfo.Name = tileInfo.Name.Replace('/', ' ');
                tileInfo.Name = tileInfo.Name.Replace('\\', ' ');

                var color = controller.GetIdentityColor(uri);
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
                var uri2 = Global.projectDocument.MakeAbosoluteUri(image, Global.ScreenComponent);
                if (uri2 != null && uri2.IsAbsoluteUri)
                {
#if !DEBUG
                                    uri2 = Global.projectDocument.MakeRelativeUri(uri2, AppDomain.CurrentDomain.BaseDirectory);
                                    if (uri2 != null)
#endif
                    tileInfo.ImageUrl = uri2.ToString();
                }
                else
                {
                    uri2 = Global.projectDocument.ExportRelativeUriToTemporaryFile(uri2, "png");
                    if (uri2 != null)
                    {
                        tileInfo.ImageUrl = uri2.ToString();
                        temporaryFiles.Add(uri2.GetPathString());
                    }
                }
                // tileInfo.Section = controller.GetTitle();
                tileInfo.Section = tileInfo.Name;
                tileInfo.Section = tileInfo.Section.Replace('/', '_');
                tileInfo.Section = tileInfo.Section.Replace('\\', '_');

                tileInfo.HasGeoCoordinates = controller.GetHasGeoCoordinates(uri);
                tileInfo.Latitude = controller.GetLatitude(uri);
                tileInfo.Longitude = controller.GetLongitude(uri);

                tileInfo.UsersVisibility = controller.GetUsersVisibility(uri);
                tileInfo.RolesVisibility = controller.GetRolesVisibility(uri);

                if (!mapTiles.ContainsKey(tileInfo.Section))
                    mapTiles.Add(tileInfo.Section, new List<TileInfo>());
                mapTiles[tileInfo.Section].Add(tileInfo);
            }
        }

        static void AddTileInfos(IScreenController controller)
        {
            controller.GetScreenLists(Global.ScreenComponent).ForEach(uri =>
            {
                AddTileInfo(controller, uri);
            });

            controller.GetScreenControllers(Global.ScreenComponent).ForEach(c =>
            {
                if (c != controller)
                    AddTileInfos(c);
            });
        }

        void TryDeleteTemporaryFile()
        {
            temporaryFiles.ForEach((file) =>
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch { }
            });
        }
    }
}