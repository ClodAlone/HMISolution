using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Configuration;
using Mindscape.Raygun4Net;
using log4net.Config;
using log4net;
using Utilities;
using WPFUtilities;
using UFInterfaces.CoreHostComponents;
using UFInterfaces;
using DevExpress.DashboardWeb.Designer;
using DocumentManager.ComponentService;
using DevExpress.DashboardWeb;

namespace UFWebClient.HTML5
{
    public class Global : System.Web.HttpApplication
    {
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
        static public bool DisableStaticOptimization { get; protected set; }

        static public bool DisableCreateNewDashboard { get; protected set; }
        static public bool SwitchToViewerDashboard { get; protected set; }

        static public Uri currentPage { get; set; }
        static public UFProjectManager.UFProjectDocument projectDocument { get; protected set; }
        static public Dictionary<String, IList<TileInfo>> mapTiles = new Dictionary<String, IList<TileInfo>>();
        static List<String> temporaryFiles = new List<String>();

        static ComponentHost componentHost = new ComponentHost();
        static PluginServices Plugins = new PluginServices();

        static Dictionary<String, ScreenSinkServiceSession> mapCurrentSessions = new Dictionary<String, ScreenSinkServiceSession>();
        static public void AddSession(String id, ScreenSinkServiceSession session)
        {
            RemoveSession(id);
            lock (mapCurrentSessions)
            {
                mapCurrentSessions.Add(id, session);
            }
        }
        static public void RemoveSession(String id)
        {
            lock (mapCurrentSessions)
            {
                if (mapCurrentSessions.ContainsKey(id))
                {
                    mapCurrentSessions[id].Dispose();
                    mapCurrentSessions.Remove(id);
                }
            }
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

        //static MSEditor.ComponentService.SchedulerEditorManagerComponent schedulerEditorComponent = new MSEditor.ComponentService.SchedulerEditorManagerComponent();
        //static public MSEditor.ComponentService.SchedulerEditorManagerComponent SchedulerEditorComponent
        //{
        //    get
        //    {
        //        return schedulerEditorComponent;
        //    }
        //}

        public static String historiansettings;
        public static String eventsettings;
        public static String serverentitysettings;
        public static String schedulersettings;

        void Application_Start(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();

            DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;

            // Code that runs on application startup
            Configuration rootWebConfig =
                            System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");

            KeyValueConfigurationCollection settings = rootWebConfig.AppSettings.Settings;
            var uri = settings["Uri"];
            if (uri != null)
                defaultUri = new Uri(uri.Value, UriKind.RelativeOrAbsolute);
            var clientSessionName = settings["ClientSessionName"];
            if (clientSessionName != null)
                ClientSessionName = clientSessionName.Value;
            var theme = settings["Theme"];
            if (theme != null)
                Theme = theme.Value;
            RefreshPollingTime = 1000;
            var refreshPollingTime = settings["RefreshPollingTime"];
            if (refreshPollingTime != null)
            {
                try
                {
                    RefreshPollingTime = Convert.ToInt32(refreshPollingTime.Value);
                }
                catch (Exception ex)
                {
                    
                }
            }
            var refreshPollingTimeCount = settings["RefreshPollingTimeCount"];
            if (refreshPollingTimeCount != null)
            {
                try
                {
                    RefreshPollingTimeCount = Convert.ToInt32(refreshPollingTimeCount.Value);
                }
                catch (Exception ex)
                {

                }
            }
            DelayBroadcaster = 100;
            var delayBroadcaster = settings["DelayBroadcaster"];
            if (delayBroadcaster != null)
            {
                try
                {
                    DelayBroadcaster = Convert.ToInt32(delayBroadcaster.Value);
                }
                catch (Exception ex)
                {

                }
            }

            var sessionTimeout = settings["SessionTimeout"];
            if (sessionTimeout != null)
            {
                try
                {
                    SessionTimeout = Convert.ToInt32(sessionTimeout.Value);
                }
                catch (Exception ex)
                {

                }
            }
            var concurrentRenderingPipeline = settings["ConcurrentRenderingPipeline"];
            if (concurrentRenderingPipeline != null)
            {
                try
                {
                    ConcurrentRenderingPipeline = Convert.ToInt32(concurrentRenderingPipeline.Value);
                }
                catch (Exception ex)
                {

                }
            }            
            var softwareRendering = settings["SoftwareRendering"];
            if (softwareRendering != null)
            {
                try
                {
                    bool bSet = Convert.ToBoolean(softwareRendering.Value);
                    if (bSet)
                        ScreenManager.ComponentService.ScreenManagerComponent.EnableSoftwareRendering();
                }
                catch (Exception ex)
                {

                }
            }
            var lowResolution = settings["LowResolution"];
            if (lowResolution != null)
            {
                try
                {
                    LowResolution = Convert.ToBoolean(lowResolution.Value);
                }
                catch (Exception ex)
                {

                }
            }
            var disablePopupScreen = settings["DisablePopupScreen"];
            if (disablePopupScreen != null)
            {
                try
                {
                    DisablePopupScreen = Convert.ToBoolean(disablePopupScreen.Value);
                }
                catch (Exception ex)
                {

                }
            }
            var disableStaticOptimization = settings["DisableStaticOptimization"];
            if (disableStaticOptimization != null)
            {
                try
                {
                    DisableStaticOptimization = Convert.ToBoolean(disableStaticOptimization.Value);
                }
                catch (Exception ex)
                {

                }
            }

            var disableCreateNewDashboard = settings["DisableCreateNewDashboard"];
            if (disableCreateNewDashboard != null)
            {
                try
                {
                    DisableCreateNewDashboard = Convert.ToBoolean(disableCreateNewDashboard.Value);
                }
                catch (Exception ex)
                {

                }
            }

            var switchToViewerDashboard = settings["SwitchToViewerDashboard"];
            if (switchToViewerDashboard != null)
            {
                try
                {
                    SwitchToViewerDashboard = Convert.ToBoolean(switchToViewerDashboard.Value);
                }
                catch (Exception ex)
                {

                }
            }
            

            projectDocument = UFProjectManager.UFProjectDocument.FromFile(Global.defaultUri.OriginalString, UFProjectManagerComponent);
            if (projectDocument == null)
                throw new ArgumentNullException("Missing project to load or project cannot be loaded!");

            projectDocument.SetCurrentLogFileName();

#if !DEBUG
            WinWrap.Basic.Util.DllDir = String.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "bin\\Libraries");
#endif

            log.Info(Properties.Resources.AppStarting);

            componentHost.Components.Add(UFProjectManagerComponent);
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

            projectDocument.UpdateSessionSettings();

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

            OPCUAViewModel.OPCUAEntityReference.SetDocumentParent(projectDocument);
            OPCUAViewModel.OPCUAEntityReference.StartDataSinkInterfaces();

            StringEditorComponent.LoadRuntimeStrings(projectDocument);

            InitProjectTilesMap();
            InitProjectRecipes();

            currentPage = projectDocument.GetStartupScreen(Global.ScreenComponent);
            if (currentPage == null)
                throw new ArgumentNullException("Missing default uri to load !");

            historiansettings = UFUAEditorComponent.GetHistorianDefaultConnection(projectDocument);
            eventsettings = UFUAEditorComponent.GetEventDefaultConnection(projectDocument);
            serverentitysettings = UFUAEditorComponent.GetServerEntityReference(projectDocument);
            //schedulersettings = SchedulerEditorComponent.GetServerEntityReference(projectDocument);

            var dashboardStorage = new DashboardFileStorage(@"~/App_Data");
            DashboardConfigurator.Default.SetDashboardStorage(dashboardStorage);
        }

        void Application_End(object sender, EventArgs e)
        {
            log.Info(Properties.Resources.AppEnd);

            //  Code that runs on application shutdown
            if (projectDocument != null)
            {
                projectDocument.Dispose();
                projectDocument = null;
            }

            TryDeleteTemporaryFile();
        }

        void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            log.Fatal(Properties.Resources.AppError, exception);
#if !DEBUG
            new RaygunClient().Send(exception);
#endif

            TryDeleteTemporaryFile();
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            log.Info(Properties.Resources.NewSession);
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            log.Info(Properties.Resources.SessionEnd);
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

        static void InitProjectTilesMap()
        {
            Global.projectDocument.GetScreenControllers(Global.ScreenComponent).ForEach(controller =>
                {
                    AddTileInfos(controller);
                });
        }

        static void InitProjectRecipes()
        {
            RecipeComponent.Execute(null, projectDocument, DocumentManager.ComponentService.ExecutionMode.Normal, projectDocument.ConfigurationId);
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
