using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using UFInterfaces.Constants;
using System.Xml;
using DocumentManager.ComponentService;
#if !WINDOWS_UWP
using System.Windows.Media;
#if !NETSTANDARD
using System.Windows.Controls;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using VFS;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.DB;
using UFProjectManager.PropertyDataTemplate;
using UFProjectManager.ScriptHelpers;
using ScriptServiceCMS;
using UIMsgBoxAlertService.ComponentService;
#else
using System.Xml.Serialization;
#endif
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Appender;
#else
using Windows.UI;
using Windows.UI.Xaml.Controls;
#endif
using ViewModelLib;
using Utilities;
using UriResolver.ComponentService;
using UFProjectManager.ComponentService;
using System.Windows.Input;
using System.ComponentModel;
using OPCUAViewModel;
using System.Windows;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using WPFUtilities;
using UFInterfaces;
using UFProjectManager.Document;
using System.IO.IsolatedStorage;
#if !NET_STANDARD
using UFUAEditor.ComponentService;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFInterfaces.PropertyControl;
using MSSchedulerSettings.ComponentService;
using DevExpress.Xpf.Core.Native;
using System.Threading;
#endif
using ClientEditor.ComponentService;
using System.Runtime.InteropServices;
using StringManager.ComponentService;
using System.Text.RegularExpressions;
using UFProjectManager.Properties;

namespace UFProjectManager
{
    [DataContract(Name = "ProjectData", Namespace = Namespaces.UriProgea)]
    public partial class UFProjectDocument : ViewModelBase,
#if !WINDOWS_UWP && !NET_STANDARD
        ICloneable, INotifyPropertyVisibilityChanged,
#endif
        IDocument, IScreenController, IEntityReference
    {
#region Declarations

        ProjectStatus projectStatus;

        UFProjectManagerComponent projectManagerComponent;
//#if !NET_STANDARD
//        int maxInvalidPasswordAttempts;
//#endif

        Dictionary<String, ResourceFolderWatcher> mapTypeResources;
        Dictionary<String, IDocumentManager> mapTypeDocumentManagers;

        bool temporaryDisableProtection;

#if !WINDOWS_UWP
#if !NET_STANDARD
        FileSystemProviderBase fileSystemProvider;
        private static readonly ILog log = LogManager.GetLogger(Properties.Resources.GeneralLog);
        private static readonly ILog logUsers = LogManager.GetLogger(Properties.Resources.UsersManager);
#else
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.GeneralLog);
        private static readonly ILog logUsers = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.UsersManager);
#endif
#if !DEBUG
        bool geolocal = false;
#endif
#endif

        #endregion

        #region ICloneable Members

#if !WINDOWS_UWP && !NET_STANDARD
        UFProjectDocument()
        {
            CommonConstruct();
        }

        UFProjectDocument(UFProjectDocument template)
        {
            CommonConstruct();

            if (template == null)
                return;

            throw new NotImplementedException();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Clone()
        {
            return new UFProjectDocument(this);
        }
#endif
#endregion

#region Persistance

        [DataMember]
        List<Uri> listChildProjectPaths;
#if !WINDOWS_UWP && !NET_STANDARD
        [DataMember]
        List<Uri> listStartupScripts;
        [DataMember]
        List<StartupScript> listStartupScriptsEx;
#endif
        [DataMember]
        List<StartupLogic> listStartupLogicsEx;
#if !NET_STANDARD
        [DataMember]
        List<AutoloadScreen> autoloadScreenListEx;
#endif
        [DataMember]
        StartType startType = StartType.TilePage;
        //[DataMember]
        //TouchType touchType = TouchType.None;
        [DataMember]
        String speechCulture;
        [DataMember]
        String cultureName;
        [DataMember]
        bool forceStartupCultureName;
        [DataMember]
        String converterName;
        [DataMember]
        List<String> defaultSpeechCommands;
        [DataMember]
        ThemeType theme =  ThemeType.Blend;
        [DataMember]
        double latitude = Double.NaN;
        [DataMember]
        double longitude = Double.NaN;
        [DataMember]
        BingMapKind bingMapKind = BingMapKind.Hybrid;
        [DataMember]
        bool visible = true;
        [DataMember]
        DocumentManager.ComponentService.TileSize tileSize = DocumentManager.ComponentService.TileSize.ExtraLarge;
#if !NET_STANDARD
        [DataMember]
#else
        [XmlIgnore]
#endif
        Color identityColor = Colors.Transparent;
        [DataMember]
        String description;
        [DataMember]
        Dictionary<Uri, UFControllerData> mapUriControllerData;
        [DataMember]
        Dictionary<Uri, UFChildProjectData> mapUriChildProjectData;
        [DataMember]
        Guid projectId;
        [DataMember]
        String passwordProject;
        [DataMember]
        int removeDisabledItemAfterSecs = 30;
        [DataMember]
        int maxCleanCount = 2;
        [DataMember]
        bool useAlwaysSecureConnections = false;
        [DataMember]
        int slowSamplingInterval = 30000;
        [DataMember]
        bool disableWhenNotUsed = true;
        [DataMember]
        int publishingInterval = 250;
        [DataMember]
        int fastSamplingInterval = 250;
        [DataMember]
        bool connectItemsAtStartup = false;
        [DataMember]
        bool awaitConnectedItemsAtStartup = false;
        [DataMember]
        [Browsable(false)]
        Dictionary<String, AppNameSettings> mapAppNameSettings = new Dictionary<String, AppNameSettings>();
        //[DataMember]
        //Dictionary<String, AppNameSettings> mapAppNameChildProjectSettings = new Dictionary<String, AppNameSettings>();     
        [DataMember]
        bool enableBackup = true;
        [DataMember]
        int maxBackupCount = 10;
        [DataMember]
        bool speechEnabled = false;
        [DataMember]
        double speechConfidenceLevel = 0.7;
        [DataMember]
        String version;
        [DataMember(Name = "versionOriginal")]
        String originalVersion;
        [DataMember]
        Uri mainScreen;
        [DataMember]
        Dictionary<String, String> mapRenamedFolders;
        [DataMember]
        Dictionary<String, String> mapRenamedResources;
        [DataMember]
        double mapZoomLevelVisibility = 1;
        [DataMember]
        double mapMaxZoomLevelVisibility = 20;
        [DataMember]
        String projectType;
        [DataMember]
        bool layoutScreenOrder = false;
        [DataMember]
        bool restoreLastOpenScreenAndZoom = false;
        [DataMember]
        bool enablePageChangeGesture = true;
        [DataMember]
        bool disablePageTransitions = true;
        [OnDeserializing]
        private void PreInitialize(StreamingContext context)
        {
            CommonConstruct();
        }

        void CommonConstruct()
        {
            latitude = Double.NaN;
            longitude = Double.NaN;
            identityColor = Colors.Transparent;
            startType = StartType.TilePage;
            //touchType = TouchType.None;
            speechCulture = "en-us";
            cultureName = "en-us";
            forceStartupCultureName = false;
            converterName = string.Empty;
            defaultSpeechCommands = new List<String>()
            {
                "next",
                "back",
                "home",
                "reset",
                "swipe up",
                "swipe down",
                "zoom in",
                "zoom out",
                "swipe left",
                "swipe right"
            };

            theme = ThemeType.Blend;
            visible = true;
            tileSize = DocumentManager.ComponentService.TileSize.ExtraLarge;
            bingMapKind = BingMapKind.Hybrid;
            mapZoomLevelVisibility = 1;
            mapMaxZoomLevelVisibility = 20;
            layoutScreenOrder = false;
            restoreLastOpenScreenAndZoom = false;

            removeDisabledItemAfterSecs = 30;
            maxCleanCount = 2;
            useAlwaysSecureConnections = false;
            slowSamplingInterval = 30000;
            disableWhenNotUsed = true;
            publishingInterval = 250;
            fastSamplingInterval = 250;
            connectItemsAtStartup = false;
            awaitConnectedItemsAtStartup = false;
            enableBackup = true;
            maxBackupCount = 10;

            speechEnabled = false;
            speechConfidenceLevel = 0.7;
            enablePageChangeGesture = true;
            disablePageTransitions = true;
        }
        #endregion

        #region Constructor

        public UFProjectDocument(UFProjectManagerComponent p)
        {
            projectManagerComponent = p;
            CommonConstruct();
            Initialize();
        }

        void Initialize()
        {
            if (projectId == Guid.Empty)
                projectId = Guid.NewGuid();
            if (originalVersion == null)
                originalVersion = version;
            projectStatus = new ProjectStatus(this);
            mapTypeResources = new Dictionary<String, ResourceFolderWatcher>();
            mapTypeDocumentManagers = new Dictionary<String, IDocumentManager>();
            if (mapUriControllerData == null)
                mapUriControllerData = new Dictionary<Uri, UFControllerData>();
            if (mapUriChildProjectData == null)
                mapUriChildProjectData = new Dictionary<Uri, UFChildProjectData>();
            if (mapAppNameSettings == null)
                mapAppNameSettings = new Dictionary<String, AppNameSettings>();
            //if (mapAppNameChildProjectSettings == null)
            //    mapAppNameChildProjectSettings = new Dictionary<String, AppNameSettings>();
#if !WINDOWS_UWP
#if !DEBUG
            geolocal = MSZ.MSZView.GetModules("KXS7y64HI+TAGrwcJgBbSp8rwADghe5thN77IIPXmuqG+ab5543ZRBCCsCBJbBNx+myJYPxeGnrgbhGui/FFig=="/* GEO */);
#endif
#endif
        }
#endregion
        
#region Methods
        [OnDeserialized]
        private void PostInitialize(StreamingContext context)
        {
            Initialize();
        }

        private static UFProjectDocument ReadProjectDataFromStream(Stream stream)
        {
            DataContractSerializer formatter = new DataContractSerializer(typeof(UFProjectDocument));
            UFProjectDocument ret = formatter.ReadObject(stream) as UFProjectDocument;
            ret.NormalizeUri();
            return ret;
        }

        void NormalizeUri()
        {
            lock (mapUriChildProjectData)
            {
                mapUriControllerData.Keys.ToList().ForEach(key =>
                    {
                        var uri = key.GetUrlDecodedUri();
                        if (!mapUriControllerData.ContainsKey(uri))
                        {
                            var value = mapUriControllerData[key];
                            mapUriControllerData.Remove(key);
                            mapUriControllerData.Add(uri, value);
                        }
                    });

                mapUriChildProjectData.Keys.ToList().ForEach(key =>
                {
                    var uri = key.GetUrlDecodedUri();
                    if (!mapUriChildProjectData.ContainsKey(uri))
                    {
                        var value = mapUriChildProjectData[key];
                        mapUriChildProjectData.Remove(key);
                        mapUriChildProjectData.Add(uri, value);
                    }
                });
            }

            if (listChildProjectPaths != null)
            {
                listChildProjectPaths.ToList().ForEach((item) =>
                {
                    var uri = item.GetUrlDecodedUri();
                    if (!listChildProjectPaths.Contains(uri))
                    {
                        listChildProjectPaths.Remove(item);
                        listChildProjectPaths.Add(uri);
                    }
                });
            }

#if !WINDOWS_UWP && !NET_STANDARD
            if (listStartupScripts != null)
            {
                listStartupScripts.ToList().ForEach((item) =>
                {
                    var uri = item.GetUrlDecodedUri();
                    if (!listStartupScripts.Contains(uri))
                    {
                        listStartupScripts.Remove(item);
                        listStartupScripts.Add(uri);
                    }
                });

                ConvertStartupScript();
            }
#endif
        }

        internal void NormalizeRenamedResources()
        {
            if (mapRenamedResources != null)
            {
                mapRenamedResources.Keys.ToList().ForEach(key =>
                {
                    var uri = MakeRelativeUri(new Uri(key, UriKind.RelativeOrAbsolute)).GetPathString();
                    if (!mapRenamedResources.ContainsKey(uri))
                    {
                        var value = new Uri(mapRenamedResources[key], UriKind.RelativeOrAbsolute);
                        value = MakeRelativeUri(value);
                        mapRenamedResources.Remove(key);
                        mapRenamedResources.Add(uri, value.GetPathString());
                    }
                });
            }

            if (mapRenamedFolders != null)
            {
                mapRenamedFolders.Keys.ToList().ForEach(key =>
                {
                    var originalUri = new Uri(key, UriKind.RelativeOrAbsolute);
                    if (originalUri.IsAbsoluteUri)
                    {
                        var path = (from c in mapTypeResources.Values.AsParallel()
                                    where new Uri(String.Format("{0}/", c.Path), UriKind.RelativeOrAbsolute).IsBaseOf(originalUri)
                                    select c.Path).FirstOrDefault();
                        if (path != null)
                        {
                            var uri = MakeRelativeUri(originalUri, path).GetPathString();
                            if (!mapRenamedFolders.ContainsKey(uri))
                            {
                                var value = new Uri(mapRenamedFolders[key], UriKind.RelativeOrAbsolute);
                                value = MakeRelativeUri(value, path);
                                mapRenamedFolders.Remove(key);
                                mapRenamedFolders.Add(uri, value.GetPathString());
                            }
                        }
                    }
                });
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal void RenameReferences(UFInterfaces.Editors.CrossReferenceModel model)
        {
            if (model.ApplyNewNames)
            {
                if (MainScreen != null)
                {
                    var newUri = MakeRelativeUri(MakeAbosoluteUri(MainScreen));
                    if (MainScreen != newUri)
                        MainScreen = newUri;
                }

                AutoloadScreenList.ForEach(screen =>
                {
                    if (screen.Uri != null)
                    {
                        var newUri = MakeRelativeUri(MakeAbosoluteUri(screen.Uri));
                        if (screen.Uri != newUri)
                        {
                            screen.Uri = newUri;
                            NeedsSave = true;
                        }
                    }
                });

                ListStartupLogics.ForEach(logic =>
                {
                    if (logic.Uri != null)
                    {
                        var newUri = MakeRelativeUri(MakeAbosoluteUri(logic.Uri));
                        if (logic.Uri != newUri)
                        {
                            logic.Uri = newUri;
                            NeedsSave = true;
                        }
                    }
                });

                ListStartupScripts.ForEach(script =>
                {
                    if (script.Uri != null)
                    {
                        var newUri = MakeRelativeUri(MakeAbosoluteUri(script.Uri));
                        if (script.Uri != newUri)
                        {
                            script.Uri = newUri;
                            NeedsSave = true;
                        }
                    }
                });
            }
        }

        void ConvertStartupScript()
        {
            if (listStartupScripts == null)
                return;
            if (listStartupScriptsEx == null)
                listStartupScriptsEx = new List<StartupScript>();
            listStartupScripts.ForEach(uri =>
                {
                    listStartupScriptsEx.Add(new StartupScript()
                    {
                        Uri = uri,
                        ExecutionMode = ExecutionMode.Normal
                    });
                });
            listStartupScripts.Clear();
            listStartupScripts = null;
        }

        internal void StopServiceScript()
        {
            using (var scriptService = new ScriptServiceCMS.ScriptServiceCSMHelpers(ConfigurationId.ToString(), log))
            {
                if (scriptService.IsServerStartedManually)
                {
                    if (!scriptService.IsServerRunningAsService)
                        scriptService.StopServer();
                }
            }
        }

        public void StartupServiceScripts(IDocumentManager managerscript = null)
        {
            var listStartupNoService = (from c in ListStartupScripts where c.ExecuteAsService == true select c).ToList();
            if (managerscript == null)
            {
                if (listStartupNoService.Count > 0)
                {
                    //var startupControl = new Controls.StartupControl();
                    //startupControl.txtTitle.Text = Properties.Resources.StartupScriptTitle;
                    //var wnd = new DevExpress.Xpf.Core.DXWindow()
                    //{
                    //    BorderEffect = DevExpress.Xpf.Core.BorderEffect.Default,
                    //    ShowInTaskbar = false,
                    //    ResizeMode = System.Windows.ResizeMode.NoResize,
                    //    WindowStyle = WindowStyle.None,
                    //    WindowState = WindowState.Normal,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Content = startupControl,
                    //    SizeToContent = SizeToContent.WidthAndHeight
                    //};

                    //ThemeHelper.SetTheme(wnd);

                    //var canClose = false;
                    //wnd.Closing += (o, e) =>
                    //{
                    //    e.Cancel = !canClose;
                    //};

                    //wnd.Show();
                    using (var serviceHelper = new ScriptServiceCMS.ScriptServiceCSMHelpers(ConfigurationId.ToString(), log))
                    {
                        if (!serviceHelper.IsServerRunning)
                        {
                            try
                            {
                                if (serviceHelper.StartServer(/*startupControl.txtevent, startupControl.Scroll, */false, false, ProjectPath))
                                {
                                    var dateTime = DateTime.Now.AddSeconds(60);
                                    while (!serviceHelper.IsServerRunning && dateTime > DateTime.Now)
                                        WaitForPriority.DoEventsSync();
                                    while (!serviceHelper.IsServerStarted && dateTime > DateTime.Now && serviceHelper.IsServerStartedManually)
                                        WaitForPriority.DoEventsSync();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (projectManagerComponent.UIInterface != null)
                                {
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerScriptFailed.Replace("'newline'", Environment.NewLine),
                                        serviceHelper.ServerName, ex.Message));
                                }
                            }
                        }
                    }
                    //canClose = true;
                    //wnd.Close();
                }
            }
            else
            {
                listStartupNoService.ForEach(script =>
                {
                    managerscript.Execute(script.Uri, this, script.ExecutionMode, this);
                });
            }
        }
        
        internal void StopServiceLogic()
        {
            using (var logicService = new LogicServiceCMS.LogicServiceCSMHelpers(ConfigurationId.ToString(), log))
            {
                if (logicService.IsServerStartedManually)
                {
                    if (!logicService.IsServerRunningAsService)
                        logicService.StopServer();
                }
            }
        }

        public void StartupServiceLogics(IDocumentManager managerlogic = null)
        {
            var listStartupNoService = (from c in ListStartupLogics where c.ExecuteAsService == true select c).ToList();
            if (managerlogic == null)
            {
                if (listStartupNoService.Count > 0)
                {
                    //var startupControl = new Controls.StartupControl();
                    //startupControl.txtTitle.Text = Properties.Resources.StartupLogicTitle;
                    //var wnd = new DevExpress.Xpf.Core.DXWindow()
                    //{
                    //    BorderEffect = DevExpress.Xpf.Core.BorderEffect.Default,
                    //    ShowInTaskbar = false,
                    //    ResizeMode = System.Windows.ResizeMode.NoResize,
                    //    WindowStyle = WindowStyle.None,
                    //    WindowState = WindowState.Normal,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Content = startupControl,
                    //    SizeToContent = SizeToContent.WidthAndHeight
                    //};

                    //ThemeHelper.SetTheme(wnd);

                    //var canClose = false;
                    //wnd.Closing += (o, e) =>
                    //{
                    //    e.Cancel = !canClose;
                    //};

                    //wnd.Show();
                    using (var serviceHelper = new LogicServiceCMS.LogicServiceCSMHelpers(ConfigurationId.ToString(), log))
                    {
                        if (!serviceHelper.IsServerRunning)
                        {
                            try
                            {
                                if (serviceHelper.StartServer(/*startupControl.txtevent, startupControl.Scroll, */false, false, ProjectPath))
                                {
                                    var dateTime = DateTime.Now.AddSeconds(60);
                                    while (!serviceHelper.IsServerRunning && dateTime > DateTime.Now)
                                        WaitForPriority.DoEventsSync();
                                    while (!serviceHelper.IsServerStarted && dateTime > DateTime.Now && serviceHelper.IsServerStartedManually)
                                        WaitForPriority.DoEventsSync();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (projectManagerComponent.UIInterface != null)
                                {
                                    projectManagerComponent.UIInterface.ShowError(String.Format(Properties.Resources.StartServerLogicFailed.Replace("'newline'", Environment.NewLine),
                                        serviceHelper.ServerName, ex.Message));
                                }
                            }
                        }
                    }
                    //canClose = true;
                    //wnd.Close();
                }
            }
            else
            {
                listStartupNoService.ForEach(script =>
                {
                    managerlogic.Execute(script.Uri, this, script.ExecutionMode, this);
                });
            }
        }
#endif
       

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsPasswordProtected()
        {
            return !String.IsNullOrEmpty(passwordProject);
        }
#if !WINDOWS_UWP
#if !NET_STANDARD
        internal bool IsComeFromLowerVersion()
        {
            Version projectVersion;
            if (Version.TryParse(version, out projectVersion))
            {
                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
                    return projectVersion.Major < versionInfo.ProductMajorPart || 
                        (projectVersion.Major == versionInfo.ProductMajorPart && 
                        projectVersion.Minor < versionInfo.ProductMinorPart);
                }
                catch
                { }
            }

            return true;
        }
        internal bool NeedToUpdateProject()
        {
            Version projectVersion;
            if (Version.TryParse(version, out projectVersion))
            {
                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
                    return projectVersion.Major < Properties.Settings.Default.UpdateProjectFromMajorVersion ||
                        (projectVersion.Major == Properties.Settings.Default.UpdateProjectFromMajorVersion &&
                        projectVersion.Minor < Properties.Settings.Default.UpdateProjectFromMinorVersion);
                }
                catch
                { }
            }

            return true;
        }
#endif
        internal bool IsComeFromHigherVersion()
        {
            Version projectVersion;
            if (Version.TryParse(version, out projectVersion))
            {
                try
                {
                    var versionInfo = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
                    return projectVersion.Major > versionInfo.ProductMajorPart ||
                        (projectVersion.Major == versionInfo.ProductMajorPart &&
                        projectVersion.Minor > versionInfo.ProductMinorPart);
                }
                catch
                { }
            }

            return false;
        }

        static int pswCnt = 0;
        public bool CheckPassword(String psw)
        {
            if(++pswCnt > 3)
            {
                int sleeptime = int.MaxValue;
                if (pswCnt < 1000)
                    sleeptime = 1000 * pswCnt * pswCnt;
                System.Threading.Thread.Sleep(sleeptime);
            }
            if (MatchPassword(psw))
                return true;

            return false;
        }
        internal bool MatchPassword(String psw)
        {
            if (!IsPasswordProtected())
                return true;
            return String.Compare(WPFUtilities.CryptString.CryptString.DecryptString(passwordProject), psw, false) == 0;
        }
#endif
        List<String> AddResources(ResourceFolderWatcher folder)
        {
            var list = (from c in folder.ListResources
                        select MakeRelativeUri(c).GetPathString()).ToList();

            folder.ListFolders.ToList().ForEach(f =>
            {
                var ret = AddResources(f);
                list.AddRange(ret);
            });
            return list;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddPassword(string pwd)
        {
            if (!string.IsNullOrEmpty(pwd) && !IsPasswordProtected())
                SetProjectPassword(pwd, silent: true);
        }

        internal void SetProjectPassword(String psw, bool silent = false)
        {
            if (fileSystemProvider == null && 
                !silent && projectManagerComponent.UIInterface != null && projectManagerComponent.UIInterface.ShowYesNo(Properties.Resources.PropagateChangesToAllResources, CustomDialogIcons.Question) != CustomDialogResults.Yes)
                return;

            bool encryptFile = !String.IsNullOrEmpty(psw);
            if (String.IsNullOrEmpty(psw))
                passwordProject = null;
            else
                passwordProject = WPFUtilities.CryptString.CryptString.EncryptString(psw);
            NeedsSave = true;

            if (fileSystemProvider != null)
                return;

            if(projectManagerComponent.Workspace != null)
                projectManagerComponent.Workspace.IsBusy = true;
            bool bError = false;
            try
            {
                temporaryDisableProtection = true;
                using (var cursor = new WaitCursor())
                {
                    var dataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();

                    dataSinkInterfaces.ForEach(ds =>
                    {
                        var dsInterface = OPCUAEntityReference.GetDataSinkInterface(ds);
                        if(projectManagerComponent.Workspace != null)
                            projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, ds);
                        try
                        {
                            var ret = dsInterface.Save(this, encryptFile);
                            if (!ret)
                                bError = true;
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            log.Error(Properties.Resources.ErrorSavingDocument, ex);
                        }
                    });

                    (from item in mapTypeDocumentManagers.Values
                     where item.isMultipleResource
                     select item).ToList().ForEach(x =>
                    {
                        try
                        {
                            var folder = GetResourceFolderWatcher(x.TypeScheme);
                            var resourcelist = AddResources(folder);

                            resourcelist.ForEach(uri =>
                                {
                                    if (projectManagerComponent.Workspace != null)
                                        projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SavingResource, uri);
                                    try
                                    {
                                        var ret = x.SaveDocument(this, MakeAbosoluteUri(new Uri(uri, UriKind.RelativeOrAbsolute)), encryptFile);
                                        if (!ret)
                                            bError = true;
                                    }
                                    catch(Exception ex)
                                    {
                                        bError = true;
                                        log.Error(Properties.Resources.ErrorSavingDocument, ex);
                                    }
                                });
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            log.Error(Properties.Resources.ErrorSavingDocument, ex);
                        }
                     });

                    (from item in mapTypeDocumentManagers.Values
                     where (!item.isMultipleResource || item.isServiceResource) && !(item is UFProjectManagerComponent)
                     select item).ToList().ForEach(x =>
                    {
                        try
                        {
                            if (projectManagerComponent.Workspace != null)
                                projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SavingResource, x.TypeLabel);
                            try
                            {
                                var ret = x.SaveDocument(this, MakeAbosoluteUri(new Uri(ProjectFolder, UriKind.RelativeOrAbsolute)), encryptFile);
                                if (!ret)
                                    bError = true;
                            }
                            catch (Exception ex)
                            {
                                bError = true;
                                log.Error(Properties.Resources.ErrorSavingDocument, ex);
                            }
                        }
                        catch (Exception ex)
                        {
                            bError = true;
                            log.Error(Properties.Resources.ErrorSavingDocument, ex);
                        }
                    });
                }
            }
            finally
            {
                temporaryDisableProtection = false;
                if (projectManagerComponent.Workspace != null)
                    projectManagerComponent.Workspace.IsBusy = false;
            }
            if (!silent && bError && projectManagerComponent.UIInterface != null)
                projectManagerComponent.UIInterface.ShowError(Properties.Resources.ErrorOccuredPropagatingChanges);
        }
#endif
        const String projectDataTag = "ProjectData";
        public static UFProjectDocument FromFile(String path, UFProjectManagerComponent p)
        {
            try
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (XpoHelpers.XpoHelper.IsDataSource(path))
                {
                    using (var fileSystemProvider = new DataSourceFileSystemProvider("") { ConnectionString = path })
                    {
                        var data = fileSystemProvider.ReadFile(new FileManagerFile(fileSystemProvider, projectDataTag));
                        using (var memoryStream = new MemoryStream(data))
                        {
                            UFProjectDocument ret = ReadProjectDataFromStream(memoryStream);
                            ret.projectManagerComponent = p;
                            ret.ProjectPath = path;

                            // ret.opcuaClientSettings = OPCUAClientSettings.Open(path);
                            ret.CreateProjectFolders();
                            ret.NormalizeRenamedResources();
                            return ret;
                        }
                    }
                }
#endif
                if (String.IsNullOrEmpty(path))
                    return null;

#if !NET_STANDARD
                if (!File.Exists(path))
                    return null;
#endif

#if !WINDOWS_UWP

                if (!Utilities.IO.FileSystem.IsXmlFile(path))
                {
                    var data = WPFUtilities.CryptString.CryptString.DecryptString(File.ReadAllText(path));
                    using (var reader = new MemoryStream(Convert.FromBase64String(data)))
                    {
                        UFProjectDocument ret = ReadProjectDataFromStream(reader);
                        ret.projectManagerComponent = p;
                        ret.ProjectPath = path;

                        // ret.opcuaClientSettings = OPCUAClientSettings.Open(path);

                        ret.CreateProjectFolders();
                        ret.NormalizeRenamedResources();
                        return ret;
                    }
                }
                else
#endif
                    return LoadProjectFileStream(path, p);
            }
            catch (Exception e)
            {
#if NET_STANDARD
                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && e is System.IO.FileNotFoundException)
                    {
                        try
                        {
                            string directory = Path.GetDirectoryName(path);
                            string pattern = Path.GetFileName(path).ToLower();
                            
                            IEnumerable<string> foundFiles = Directory.EnumerateFiles(directory)
                                .Where(file => file.Substring(directory.Length + 1).ToLower() == pattern)
                                .ToList();

                            if (foundFiles.Any())
                                path = foundFiles.First();
                        
                            return LoadProjectFileStream(path, p);
                        }
                        catch {
                            return null;
                        }
                    }
#endif
                return null;
            }
        }

        static UFProjectDocument LoadProjectFileStream(string path, UFProjectManagerComponent p)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                UFProjectDocument ret = ReadProjectDataFromStream(fileStream);
                ret.projectManagerComponent = p;
                ret.ProjectPath = path;

                // ret.opcuaClientSettings = OPCUAClientSettings.Open(path);

                ret.CreateProjectFolders();
                ret.NormalizeRenamedResources();
                return ret;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        private bool WriteProjectDataStream(Stream ostrm)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                CloseOutput = true
            };

            using (XmlWriter writer = XmlDictionaryWriter.Create(ostrm, settings))
            {
                bool bRet = false;
                try
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(UFProjectDocument));

                    Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    version = versionInfo.FileVersion;
                    if (originalVersion == null)
                        originalVersion = version;
                    lock (mapUriControllerData)
                    {
                        serializer.WriteObject(writer, this);
                    }
                    bRet = true;
                }
                finally
                {
                    writer.Close();
                }

                return bRet;
            }
        }

        internal void SaveBackup(bool bForce = false)
        {
            if (!bForce && !EnableBackup)
                return;

            if (fileSystemProviderBase != null)
            {
                if (fileSystemProviderBase is DataSourceFileSystemProvider)
                {
                    var datamodel = new DataReader.DataReaderModel(Title, (fileSystemProviderBase as DataSourceFileSystemProvider).ConnectionString);
                    var writer = new DataWriter.DataSetWriter(datamodel.DataProvider, datamodel.Connection);

                    try
                    {
                        writer.CreateBackup();
                    }
                    catch(Exception ex)
                    {
                        log.Error(Properties.Resources.ErrorSavingBackup, ex);
                    }
                }

                return;
            }

            var backupFile = System.IO.Path.ChangeExtension(ProjectPath, String.Format("{0}.backup.zip", DateTime.Now.ToString("yyyyMMddHHmmss")));
            while(File.Exists(backupFile))
            {
                backupFile = System.IO.Path.ChangeExtension(ProjectPath, String.Format("{0}.backup.zip", DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            var sourceDir = System.IO.Path.GetDirectoryName(ProjectPath);
            var tempDir = System.IO.Path.GetTempPath();

            string[] files = null;
            try
            {
                files = Directory.GetFiles(sourceDir, "*.backup.zip");
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorSavingBackup, ex);
                return;
            }
            if (files == null)
                return;

            var list = (from c in files
                        orderby File.GetLastWriteTime(c) ascending
                        select c).ToList();
            list.ForEach(c =>
                {
                    try
                    {
                        var dest = tempDir + System.IO.Path.GetFileName(c);
                        File.Move(c, dest);
                    }
                    catch (Exception ex)
                    {
                     
                    }
                });
            try
            {
                var tempFile = System.IO.Path.Combine(tempDir, System.IO.Path.GetRandomFileName());
                List<string> exceptions = new List<string>() 
                { 
                    ".dynjobs", 
                    ".flusheddata", 
                    ".alrsettings", 
                    ".tagsettings", 
                    ".log"
                };
                ZipFileHelper.CreateArchive(sourceDir, tempFile, exceptions);
                File.Move(tempFile, backupFile);
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorSavingBackup, ex);
            }

            list.ForEach(c =>
            {
                try
                {
                    var source = tempDir + System.IO.Path.GetFileName(c);
                    File.Move(source, c);
                }
                catch (Exception ex)
                {

                }
            });

            while (MaxBackupCount > 0 && list.Count >= MaxBackupCount)
            {
                var toRemove = list[0];
                list.RemoveAt(0);
                try
                {
                    File.Delete(toRemove);
                }
                catch (Exception ex)
                {
                        
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool SaveToFile(bool forceEncryption = false)
        {            
            var filepathMutex = ProjectPath.Replace("/", ".").Replace("\\", ".");
            using (var mutex = new Mutex(false, filepathMutex))
            {
                if (projectManagerComponent.Workspace != null)
                {
                    projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, Title);
                    projectManagerComponent.Workspace.IsBusy = true;
                }

                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    log.Warn("AbandonedMutexException", ex);
                }
                
                try
                {
                    bool bRet = true;
                    var folderRuntimeData = GetSpecialFolder(SpecialFolders.RuntimeData);
                    try
                    {
                        Directory.Delete(folderRuntimeData.OriginalString, true);
                    }
                    catch { }

                    var dataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
                    dataSinkInterfaces.ForEach(ds =>
                    {
                        var dsInterface = OPCUAEntityReference.GetDataSinkInterface(ds);
                        var restorebusy = String.Empty;
                        if (projectManagerComponent.Workspace != null)
                        {
                            restorebusy = projectManagerComponent.Workspace.BusyContent;
                            projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, ds);
                        }
                        try
                        {
                            var ret = dsInterface.Save(this, forceEncryption);
                            if (!ret)
                                bRet = false;
                        }
                        catch (Exception ex)
                        {
                            bRet = false;
                            log.Error(Properties.Resources.ErrorSavingDocument, ex);
                        }
                        if (projectManagerComponent.Workspace != null)
                            projectManagerComponent.Workspace.BusyContent = restorebusy;
                    });

                    if (fileSystemProviderBase != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            if (!WriteProjectDataStream(memoryStream))
                                return false;

                            fileSystemProviderBase.UploadFile(null, projectDataTag, memoryStream.ToArray());

                            return bRet;
                        }
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(ProjectPath));
                    if (forceEncryption || IsPasswordProtected())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            if (!WriteProjectDataStream(memoryStream))
                                return false;

                            var str = Convert.ToBase64String(memoryStream.ToArray());
                            var toWrite = WPFUtilities.CryptString.CryptString.EncryptString(str);
                            File.WriteAllText(ProjectPath, toWrite);
                            return bRet;
                        }
                    }
                    else
                    {
                        using (var ostrm = File.Open(ProjectPath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            if (!WriteProjectDataStream(ostrm))
                                return false;
                            return bRet;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format(Properties.Resources.ErrorSavingDocument, ex.Message),
                                            Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                finally
                {
                    if (projectManagerComponent.Workspace != null)
                        projectManagerComponent.Workspace.IsBusy = false;

                    mutex.ReleaseMutex();
                }
            }
        }

        private void SaveFoldersAndFilesToTarget(String pathTo, 
                                                bool bTargetDataSource, 
                                                UFProjectDocument newDoc, 
                                                ResourceFolderWatcher folder, 
                                                String type, bool bUploading = false)
        {
            foreach (var resource in folder.ListResources)
            {
                try
                {
                    Uri relative = null;
                    if (resource.IsAbsoluteUri)
                        relative = (new Uri(String.Format("{0}\\", mapTypeResources[type]/*folder*/.Path), UriKind.RelativeOrAbsolute)).MakeRelativeUri(resource);
                    else
                    {
                        var resourceString = resource.GetPathString();
                        resourceString = resourceString.Replace(mapTypeResources[type]/*folder*/.Path, "");
                        relative = new Uri(resourceString, UriKind.RelativeOrAbsolute);
                    }
                    projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, relative.GetPathString());
                    if (bTargetDataSource)
                        mapTypeDocumentManagers[type].Copy(resource, pathTo, true, this, bUploading);
                    else
                    {
                        var absolute = String.Format("{0}\\{1}", newDoc.mapTypeResources[type].Path, relative.GetPathString());
                        var path = Path.GetDirectoryName(absolute);
                        Directory.CreateDirectory(path);
                        mapTypeDocumentManagers[type].Copy(resource, absolute, true, this, bUploading);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            foreach (var f in folder.ListFolders)
            {
                SaveFoldersAndFilesToTarget(pathTo, bTargetDataSource, newDoc, f, type, bUploading);
            }
        }

        internal bool UploadProject(String pathTo)
        {
            bool bRet = true;
            projectManagerComponent.Workspace.IsBusy = true;
            try
            {
                projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, Title);
                if (fileSystemProvider != null)
                {
                    var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, projectDataTag);
                    if (fileSystemProvider.Exists(fileManagerFileSettings))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFileSettings);
                            File.WriteAllBytes(pathTo, data);
                    }
                }
                else
                {
                    if (File.Exists(ProjectPath))
                        File.Copy(ProjectPath, pathTo, true);
                }
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorSaveAs, ex);
                projectManagerComponent.Workspace.IsBusy = false;
                return false;
            }

            using (var newDoc = UFProjectDocument.FromFile(pathTo, projectManagerComponent))
            {
                foreach (var type in mapTypeDocumentManagers.Keys)
                {
                    if (mapTypeDocumentManagers[type].TypeScheme == projectManagerComponent.TypeScheme)
                        continue;

                    if (mapTypeDocumentManagers[type].isMultipleResource)
                    {
                        SaveFoldersAndFilesToTarget(pathTo, false, newDoc, mapTypeResources[type], type, true);
                    }
                    if (!mapTypeDocumentManagers[type].isMultipleResource || mapTypeDocumentManagers[type].isServiceResource)
                    {
                        try
                        {
                            Uri uriSource = null;
                            try
                            {
                                uriSource = new Uri(String.Format("{0}:{1}", mapTypeDocumentManagers[type].TypeScheme, ProjectFolder));
                            }
                            catch
                            {
                                uriSource = new Uri(String.Format("{0}", ProjectFolder));
                            }

                            Uri uriTarget = null;
                            try
                            {
                                uriTarget = new Uri(String.Format("{0}:{1}", mapTypeDocumentManagers[type].TypeScheme, newDoc.ProjectFolder));
                            }
                            catch
                            {
                                uriTarget = new Uri(String.Format("{0}", newDoc.ProjectFolder));
                            }

                            projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, mapTypeDocumentManagers[type].TypeLabel);
                            mapTypeDocumentManagers[type].Copy(uriSource, uriTarget.GetPathString(), true, this, true);
                        }
                        catch (Exception ex)
                        {
                            bRet = false;
                            log.Error(Properties.Resources.ErrorSaveAs, ex);
                        }
                    }
                }

                var dataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();

                dataSinkInterfaces.ForEach(ds =>
                {
                    try
                    {
                        Uri uriSource = null;
                        try
                        {
                            uriSource = new Uri(String.Format("{0}:{1}", ds, ProjectFolder, UriKind.RelativeOrAbsolute));
                        }
                        catch
                        {
                            uriSource = new Uri(String.Format("{0}", ProjectFolder));
                        }

                        Uri uriTarget = null;
                        try
                        {
                            uriTarget = new Uri(String.Format("{0}:{1}", ds, newDoc.ProjectFolder, UriKind.RelativeOrAbsolute));
                        }
                        catch
                        {
                            uriTarget = new Uri(String.Format("{0}", newDoc.ProjectFolder));
                        }

                        projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, ds);
                        OPCUAEntityReference.GetDataSinkInterface(ds).Copy(uriSource, uriTarget.GetPathString(), true, this);
                    }
                    catch (Exception ex)
                    {
                        bRet = false;
                        log.Error(Properties.Resources.ErrorSaveAs, ex);
                    }
                });


                if (fileSystemProvider != null)
                {
                    var folders = fileSystemProvider.GetFolders(new FileManagerFolder(fileSystemProvider, ProjectFolder));
                    var sourceDirfolders = fileSystemProvider.GetFolders(null);
                    var listFolders = folders.ToList();
                    listFolders.AddRange(sourceDirfolders);
                    listFolders.ForEach(folder =>
                    {
                        if (bRet && folder.FullName != ProjectFolder)
                        {
                            bool bCopy = true;
                            foreach (var type in mapTypeDocumentManagers.Keys)
                            {
                                if (mapTypeResources[type].Path == folder.FullName)
                                {
                                    bCopy = false;
                                    break;
                                }
                            }

                            if (bCopy)
                            {
                                var folderTypeScheme = folder.FullName;
                                if (folder.FullName.StartsWith(projectDataTag))
                                    folderTypeScheme = folder.FullName.Substring(projectDataTag.Length + 1);
                                foreach (var ds in dataSinkInterfaces)
                                {
                                    if (OPCUAEntityReference.GetDataSinkInterface(ds).TypeScheme == folderTypeScheme)
                                    {
                                        bCopy = false;
                                        break;
                                    }
                                }
                            }

                            if (bCopy)
                            {
                                try
                                {
                                    var pathToName = Path.GetFileNameWithoutExtension(pathTo);
                                    var dirTo = Path.GetDirectoryName(pathTo);
                                    var name = System.IO.Path.GetFileName(folder.FullName);
                                    var dirToName = String.Format("{0}\\{1}\\{2}", dirTo, pathToName, name);
                                    if (sourceDirfolders.Contains(folder))
                                        dirToName = String.Format("{0}\\{1}", dirTo, name);
                                    if (!Directory.Exists(dirToName))
                                        Directory.CreateDirectory(dirToName);
                                    projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, folder.FullName);
                                    CopyAll(folder.FullName, dirToName, null);
                                }
                                catch (Exception ex)
                                {
                                    bRet = false;
                                    log.Error(Properties.Resources.ErrorSaveAs, ex);
                                }
                            }
                        }
                    });
                }
                else
                {
                    var folders = Directory.GetDirectories(ProjectFolder);

                    var sourceDir = System.IO.Path.GetDirectoryName(ProjectPath);
                    var sourceDirfolders = Directory.GetDirectories(sourceDir);
                    var listFolders = folders.ToList();
                    listFolders.AddRange(sourceDirfolders);
                    listFolders.ForEach(folder =>
                    {
                        if (bRet && folder != ProjectFolder)
                        {
                            bool bCopy = true;
                            foreach (var type in mapTypeDocumentManagers.Keys)
                            {
                                if (mapTypeResources[type].Path == folder)
                                {
                                    bCopy = false;
                                    break;
                                }
                            }

                            if (bCopy)
                            {
                                try
                                {
                                    var pathToName = Path.GetFileNameWithoutExtension(pathTo);
                                    var dirTo = Path.GetDirectoryName(pathTo);
                                    var name = System.IO.Path.GetFileName(folder);
                                    var dirToName = String.Format("{0}\\{1}\\{2}", dirTo, pathToName, name);
                                    if (sourceDirfolders.Contains(folder))
                                        dirToName = String.Format("{0}\\{1}", dirTo, name);
                                    if (!Directory.Exists(dirToName))
                                        Directory.CreateDirectory(dirToName);
                                    projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, folder);
                                    CopyAll(folder, dirToName, null);
                                }
                                catch (Exception ex)
                                {
                                    bRet = false;
                                    log.Error(Properties.Resources.ErrorSaveAs, ex);
                                }
                            }
                        }
                    });
                }
            }

            try
            {
                var commonFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var certStore = @"OPC Foundation\CertificateStores\MachineDefault";
                var pathSourceCert = String.Format(@"{0}\{1}", commonFolder, certStore);
                var dirTo = Path.GetDirectoryName(pathTo);
                var destFolder = String.Format("{0}\\{1}", dirTo, certStore);
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, pathSourceCert);
                CopyAll(pathSourceCert, destFolder, null);
            }
            catch (Exception ex)
            {
                bRet = false;
                log.Error(Properties.Resources.ErrorSaveAs, ex);
            }

            

            projectManagerComponent.Workspace.BusyContent = null;
            projectManagerComponent.Workspace.IsBusy = false;
            return bRet;
        }

        internal bool SaveAs(String pathTo)
        {
            bool bRet = true;
            DataSourceFileSystemProvider targetVFS = null;
            bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(pathTo);
            if (bTargetDataSource)
            {
                targetVFS = new DataSourceFileSystemProvider("")
                {
                    ConnectionString = pathTo
                };
            }
            else
            {
                var ext = System.IO.Path.GetExtension(pathTo);
                if (String.IsNullOrEmpty(ext))
                    pathTo = pathTo + Properties.Settings.Default.DefaultFileExt;
            }

            try
            {
                projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, Title);
                projectManagerComponent.Workspace.IsBusy = true;
                if (fileSystemProvider != null)
                {
                    var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, projectDataTag);
                    if (fileSystemProvider.Exists(fileManagerFileSettings))
                    {
                        var data = fileSystemProvider.ReadFile(fileManagerFileSettings);
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, projectDataTag, data);
                        else
                            File.WriteAllBytes(pathTo, data);
                    }
                }
                else
                {
                    if (File.Exists(ProjectPath))
                    {
                        if (targetVFS != null)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                if (!WriteProjectDataStream(memoryStream))
                                    return false;

                                targetVFS.UploadFile(null, projectDataTag, memoryStream.ToArray());
                            }
                        }
                        else
                            File.Copy(ProjectPath, pathTo, true);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Properties.Resources.ErrorSaveAs, ex);
                projectManagerComponent.Workspace.IsBusy = false;
                return false;
            }
            finally
            {
                if (targetVFS != null)
                    targetVFS.Dispose();
            }

            using (var newDoc = UFProjectDocument.FromFile(pathTo, projectManagerComponent))
            {
                foreach (var type in mapTypeDocumentManagers.Keys)
                {
                    if (mapTypeDocumentManagers[type].TypeScheme == projectManagerComponent.TypeScheme)
                        continue;

                    if (mapTypeDocumentManagers[type].isMultipleResource)
                    {
                        SaveFoldersAndFilesToTarget(pathTo, bTargetDataSource, newDoc, mapTypeResources[type], type);
                    }
                    if (!mapTypeDocumentManagers[type].isMultipleResource || mapTypeDocumentManagers[type].isServiceResource)
                    {
                        try
                        {
                            Uri uriSource = null;
                            try
                            {
                                uriSource = new Uri(String.Format("{0}:{1}", mapTypeDocumentManagers[type].TypeScheme, ProjectFolder));
                            }
                            catch
                            {
                                uriSource = new Uri(String.Format("{0}", ProjectFolder));
                            }

                            Uri uriTarget = null;
                            try
                            {
                                uriTarget = new Uri(String.Format("{0}:{1}", mapTypeDocumentManagers[type].TypeScheme, newDoc.ProjectFolder));
                            }
                            catch
                            {
                                uriTarget = new Uri(String.Format("{0}", newDoc.ProjectFolder));
                            }

                            projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, mapTypeDocumentManagers[type].TypeLabel);
                            if (bTargetDataSource)
                                mapTypeDocumentManagers[type].Copy(uriSource, pathTo, true, this, false);
                            else
                                mapTypeDocumentManagers[type].Copy(uriSource, uriTarget.GetPathString(), true, this, false);
                        }
                        catch (Exception ex)
                        {
                            bRet = false;
                            log.Error(Properties.Resources.ErrorSaveAs, ex);
                        }
                    }
                }

                var dataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();

                dataSinkInterfaces.ForEach(ds =>
                    {
                        try
                        {
                            Uri uriSource = null;
                            try
                            {
                                uriSource = new Uri(String.Format("{0}:{1}", ds, ProjectFolder, UriKind.RelativeOrAbsolute));
                            }
                            catch
                            {
                                uriSource = new Uri(String.Format("{0}", ProjectFolder));
                            }

                            Uri uriTarget = null;
                            try
                            {
                                uriTarget = new Uri(String.Format("{0}:{1}", ds, newDoc.ProjectFolder, UriKind.RelativeOrAbsolute));
                            }
                            catch
                            {
                                uriTarget = new Uri(String.Format("{0}", newDoc.ProjectFolder));
                            }

                            projectManagerComponent.Workspace.BusyContent = String.Format(Properties.Resources.SaveAsDoc, ds);
                            if (bTargetDataSource)
                                OPCUAEntityReference.GetDataSinkInterface(ds).Copy(uriSource, pathTo, true, this);
                            else
                                OPCUAEntityReference.GetDataSinkInterface(ds).Copy(uriSource, uriTarget.GetPathString(), true, this);
                        }
                        catch (Exception ex)
                        {
                            bRet = false;
                            log.Error(Properties.Resources.ErrorSaveAs, ex);
                        }
                    });


                if (fileSystemProvider != null)
                {
                    try
                    {
                        var folders = fileSystemProvider.GetFolders(new FileManagerFolder(fileSystemProvider, ProjectFolder));
                        var sourceDirfolders = fileSystemProvider.GetFolders(null);
                        var listFolders = folders.ToList();
                        listFolders.AddRange(sourceDirfolders);
                        listFolders.ForEach(folder =>
                        {
                            if (bRet && folder.FullName != ProjectFolder)
                            {
                                bool bCopy = true;
                                List<String> fileExtensions = null;
                                foreach (var type in mapTypeDocumentManagers.Keys)
                                {
                                    if (mapTypeResources[type].Path == folder.FullName)
                                    {
                                        var saveAsFileExtensions = mapTypeDocumentManagers[type].SaveAsFileExtensions;
                                        if (saveAsFileExtensions != null && saveAsFileExtensions.Length > 0)
                                            fileExtensions = new List<String>(saveAsFileExtensions);
                                        else
                                            bCopy = false;
                                        break;
                                    }
                                }

                                if (bCopy)
                                {
                                    var folderTypeScheme = folder.FullName;
                                    if (folder.FullName.StartsWith(projectDataTag))
                                        folderTypeScheme = folder.FullName.Substring(projectDataTag.Length + 1);
                                    foreach (var ds in dataSinkInterfaces)
                                    {
                                        if (OPCUAEntityReference.GetDataSinkInterface(ds).TypeScheme == folderTypeScheme)
                                        {
                                            bCopy = false;
                                            break;
                                        }
                                    }
                                }

                                if (bCopy)
                                {
                                    try
                                    {
                                        var pathToName = Path.GetFileNameWithoutExtension(pathTo);
                                        var dirTo = Path.GetDirectoryName(pathTo);
                                        var name = System.IO.Path.GetFileName(folder.FullName);
                                        var dirToName = String.Format("{0}\\{1}\\{2}", dirTo, pathToName, name);
                                        if (sourceDirfolders.Contains(folder))
                                            dirToName = String.Format("{0}\\{1}", dirTo, name);
                                        if (targetVFS == null)
                                        {
                                            if (!Directory.Exists(dirToName))
                                                Directory.CreateDirectory(dirToName);
                                        }
                                        CopyAll(folder.FullName, dirToName, targetVFS, fileExtensions);
                                    }
                                    catch (Exception ex)
                                    {
                                        bRet = false;
                                        log.Error(Properties.Resources.ErrorSaveAs, ex);
                                    }
                                }
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        bRet = false;
                        log.Error(Properties.Resources.ErrorSaveAs, ex);
                    }
                }
                else
                {
                    try
                    {
                        var folders = Directory.GetDirectories(ProjectFolder);

                        var sourceDir = System.IO.Path.GetDirectoryName(ProjectPath);
                        var sourceDirfolders = Directory.GetDirectories(sourceDir);
                        var listFolders = folders.ToList();
                        listFolders.AddRange(sourceDirfolders);
                        listFolders.ForEach(folder =>
                            {
                                if (bRet && folder != ProjectFolder)
                                {
                                    bool bCopy = true;
                                    List<String> fileExtensions = null;
                                    foreach (var type in mapTypeDocumentManagers.Keys)
                                    {
                                        if (mapTypeResources[type].Path == folder)
                                        {
                                            var saveAsFileExtensions = mapTypeDocumentManagers[type].SaveAsFileExtensions;
                                            if (saveAsFileExtensions != null && saveAsFileExtensions.Length > 0)
                                                fileExtensions = new List<String>(saveAsFileExtensions);
                                            else
                                                bCopy = false;
                                            break;
                                        }
                                    }

                                    if (bCopy)
                                    {
                                        try
                                        {
                                            var pathToName = Path.GetFileNameWithoutExtension(pathTo);
                                            var dirTo = Path.GetDirectoryName(pathTo);
                                            var name = System.IO.Path.GetFileName(folder);
                                            var dirToName = String.Format("{0}\\{1}\\{2}", dirTo, pathToName, name);
                                            if (sourceDirfolders.Contains(folder))
                                                dirToName = String.Format("{0}\\{1}", dirTo, name);
                                            if (targetVFS == null)
                                            {
                                                if (!Directory.Exists(dirToName))
                                                    Directory.CreateDirectory(dirToName);
                                            }
                                            CopyAll(folder, dirToName, targetVFS, fileExtensions);
                                        }
                                        catch (Exception ex)
                                        {
                                            bRet = false;
                                            log.Error(Properties.Resources.ErrorSaveAs, ex);
                                        }
                                    }
                                }
                            });
                    }
                    catch (Exception ex)
                    {
                        bRet = false;
                        log.Error(Properties.Resources.ErrorSaveAs, ex);
                    }
                }
            }

            if (!bTargetDataSource)
            {
                try
                {
                    using (var newDoc = UFProjectDocument.FromFile(pathTo, projectManagerComponent))
                    {
                        if (IsPasswordProtected())
                            newDoc.SetProjectPassword(null, silent: true);
                        newDoc.projectId = Guid.NewGuid();
                        newDoc.SaveToFile();
                    }
                }
                catch (Exception ex)
                {
                    bRet = false;
                    log.Error(Properties.Resources.ErrorSaveAs, ex);
                }
            }

            projectManagerComponent.Workspace.BusyContent = null;
            projectManagerComponent.Workspace.IsBusy = false;
            return bRet;
        }

        internal Dictionary<string, string> GetTileDescriptions()
        {
            return mapUriControllerData.Where(k => !string.IsNullOrEmpty(k.Value.Description)).ToDictionary(k => k.Key.GetPathString(), k => k.Value.Description);
        }

        void CopyAll(String source, String dest, DataSourceFileSystemProvider targetVFS, List<String> filterExtensions = null)
        {
            if (fileSystemProvider != null)
            {
                var folders = fileSystemProvider.GetFolders(new FileManagerFolder(fileSystemProvider, source));
                folders.ToList().ForEach(folder =>
                {
                    var name = System.IO.Path.GetFileName(folder.FullName);
                    var destFolder = System.IO.Path.Combine(dest, name);
                    if (targetVFS == null)
                    {
                        if (!Directory.Exists(destFolder))
                            Directory.CreateDirectory(destFolder);
                    }
                    else
                    {
                    }
                    CopyAll(folder.FullName, destFolder, targetVFS, filterExtensions);
                });

                var files = fileSystemProvider.GetFiles(new FileManagerFolder(fileSystemProvider, source));
                files.ToList().ForEach(file =>
                {
                    if (filterExtensions == null || filterExtensions.Contains(file.Extension.ToLower()))
                    {
                        var data = fileSystemProvider.ReadFile(file);
                        if (targetVFS != null)
                            targetVFS.UploadFile(null, file.FullName, data);
                        else
                        {
                            var name = System.IO.Path.GetFileName(file.FullName);
                            var destFile = System.IO.Path.Combine(dest, name);

                            File.WriteAllBytes(destFile, data);
                        }
                    }
                });
            }
            else
            {
                var folders = Directory.GetDirectories(source);
                folders.ToList().ForEach(folder =>
                {
                    var name = System.IO.Path.GetFileName(folder);
                    var destFolder = System.IO.Path.Combine(dest, name);
                    if (targetVFS == null)
                    {
                        if (!Directory.Exists(destFolder))
                            Directory.CreateDirectory(destFolder);
                    }
                    else
                    {
                        string target;
                        if (folder.StartsWith(rootBase))
                            target = String.Format("{0}{1}", projectDataTag, folder.Replace(rootBase, ""));
                        else
                            target = folder.Replace(System.IO.Path.GetDirectoryName(ProjectPath), "");

                        targetVFS.CreateFolder(null, target);
                    }
                    CopyAll(folder, destFolder, targetVFS, filterExtensions);
                });

                var files = Directory.GetFiles(source);
                files.ToList().ForEach(file =>
                {
                    if (filterExtensions == null || filterExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                    {
                        var name = System.IO.Path.GetFileName(file);
                        var destFile = System.IO.Path.Combine(dest, name);
                        if (targetVFS == null)
                        {
                            try
                            {
                                File.Copy(file, destFile, true);
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            string target;
                            if (file.StartsWith(rootBase))
                                target = String.Format("{0}{1}", projectDataTag, file.Replace(rootBase, ""));
                            else
                                target = file.Replace(System.IO.Path.GetDirectoryName(ProjectPath), "");

                            byte[] data = File.ReadAllBytes(file);
                            //var data = Encoding.Unicode.GetBytes(File.ReadAllText(file));
                            targetVFS.UploadFile(null, target, data);
                        }
                    }
                });
            }
        }
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void CreateProjectFolders()
        {
            if (projectManagerComponent == null || projectManagerComponent.UriRisolver == null)
                return;

            var listDocs = projectManagerComponent.UriRisolver.GetListInstalledDocumentManagers();
            listDocs.ForEach(idoc =>
            {
                var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
                if (String.IsNullOrEmpty(ProjectFolder))
                    folderName = String.Format("{0}", idoc.TypeLabel);
#if !WINDOWS_UWP && !NET_STANDARD
                if (fileSystemProviderBase != null)
                {
                    fileSystemProviderBase.CreateFolder(null, folderName);
                }
                else
#endif
                {
                    Directory.CreateDirectory(folderName);
                }

                mapTypeResources.Add(idoc.TypeScheme, new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD

                    fileSystemProviderBase, 
#endif
                    idoc.TypeTitle, bWatch: idoc.isMultipleResource));
                mapTypeDocumentManagers.Add(idoc.TypeScheme, idoc);
            });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetResourcePath(String scheme, String subfolder = null)
        {
            if (mapTypeResources.ContainsKey(scheme))
                return new Uri(subfolder == null ? String.Format("{0}{1}", mapTypeResources[scheme].Path, Path.DirectorySeparatorChar) : String.Format("{0}{2}{1}{2}", mapTypeResources[scheme].Path, subfolder, Path.DirectorySeparatorChar),
                    UriKind.RelativeOrAbsolute);
            return null;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetNewDocumentUri(string name, string scheme, string subfolder = null)
        {
            var relativeUri = GetResourcePath(scheme, subfolder);
            return projectManagerComponent.UriRisolver.CreateNewDocumentFromUriAndScheme(relativeUri, scheme, this, IsPasswordProtected());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri CreateNewResource(String scheme, String relativePath = null)
        {
            if (mapTypeResources.ContainsKey(scheme))
            {
                try
                {
                    temporaryDisableProtection = true;

                    var relativeUri = new Uri(relativePath ?? String.Format("{0}\\", mapTypeResources[scheme].Path), 
                    UriKind.RelativeOrAbsolute);

                    Uri uri = projectManagerComponent.UriRisolver.CreateNewDocumentFromUriAndScheme(relativeUri, scheme, this, IsPasswordProtected());
                    if (uri == null)
                        return null;
                    using (new WaitCursor())
                    {
                        IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                        manager.Edit(uri, this);
                        // UFProjectExplorerUI.AddListDocumentManager(manager);
                    }

                    return uri;
                }
                finally
                {
                    temporaryDisableProtection = false;
                }
            }
            else
                throw new ArgumentException("Missing the document scheme mapping");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri CreateNewTemplatedScreen(String scheme, String relativePath = null)
        {
            if (mapTypeResources.ContainsKey(scheme))
            {
                try
                {
                    temporaryDisableProtection = true;

                    var relativeUri = new Uri(relativePath ?? String.Format("{0}\\", mapTypeResources[scheme].Path),
                    UriKind.RelativeOrAbsolute);


                    Uri uri = projectManagerComponent.ScreenManager.CreateNewTemplatedDocument(relativeUri, this, IsPasswordProtected());
                    if (uri == null)
                        return null;
                    using (new WaitCursor())
                    {
                        (projectManagerComponent.ScreenManager as IDocumentManager).Edit(uri, this);
                    }

                    return uri;
                }
                finally
                {
                    temporaryDisableProtection = false;
                }
            }
            else
                throw new ArgumentException("Missing the document scheme mapping");
        }
#endif
        internal List<String> GetControllerDataStrings()
        {
            var list = new List<String>();
            lock (mapUriControllerData)
            {
                foreach (var key in mapUriControllerData.Keys)
                {
                    var id = System.IO.Path.GetFileNameWithoutExtension(key.OriginalString);
                    if (!String.IsNullOrEmpty(id))
                        list.Add(id);
                    if (!String.IsNullOrEmpty(mapUriControllerData[key].Description))
                        list.Add(mapUriControllerData[key].Description);
                }
            }
            return list;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal bool GetFriendObjects(UFControllerData data, UFInterfaces.GetFriendObjectsEventArgs e)
        {
            lock (mapUriControllerData)
            {
                var found = (from c in mapUriControllerData where c.Value == data select c.Key).ToList();
                if (found.Count > 0)
                {
                    var uri = MakeAbosoluteUri(found[0]);
                    var docManager = GetResourceDocumentManager(uri);
                    if (docManager != null)
                    {
                        var ret = docManager.GetChildDocumentManagers(uri, this);
                        var doc = docManager.GetDocument(uri);
                        if (doc != null)
                        {
                            if (e.friendList == null)
                                e.friendList = new List<Object>();
                            e.friendList.Add(doc);
                            return true;
                        }
                    }
                }

                return false;
            }
        }
#endif
        public bool ControllerDataExist(Uri uri)
        {
            lock (mapUriControllerData)
            {
                uri = uri.GetUrlDecodedUri();
                var relative = GetControllerDataRelativeUri(uri).GetUrlDecodedUri();
                return mapUriControllerData.ContainsKey(relative);
            }
        }
        public UFControllerData GetControllerData(Uri uri, bool bCheckEvents = false)
        {
            lock (mapUriControllerData)
            {
                uri = uri.GetUrlDecodedUri();
                var relative = GetControllerDataRelativeUri(uri).GetUrlDecodedUri();
                if (!mapUriControllerData.ContainsKey(relative))
                {
#if !WINDOWS_UWP && !NET_STANDARD
                    bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(ProjectPath);
                    if (bTargetDataSource)
                    {
                        var path = uri.GetPathString();
                        if (path.StartsWith(projectDataTag))
                        {
                            path = path.Replace(String.Format("{0}\\", projectDataTag), "");
                            path = path.Replace(String.Format("{0}/", projectDataTag), "");
                            path = path.Replace("\\", "/");

                            try
                            {
                                uri = new Uri(path, UriKind.RelativeOrAbsolute).GetUrlDecodedUri();
                                relative = GetControllerDataRelativeUri(uri).GetUrlDecodedUri();
                            }
                            catch
                            {

                            }
                        }

                        if (!mapUriControllerData.ContainsKey(relative))
                        {
                            var controllerData = new UFControllerData();
                            mapUriControllerData.Add(relative, controllerData);
                        }
                    }
                    else
#endif
                    {
                        var controllerData = new UFControllerData();
                        mapUriControllerData.Add(relative, controllerData);
                    }
                }

#if !WINDOWS_UWP && !NET_STANDARD
                if (bCheckEvents)
                {
                    mapUriControllerData[relative].PropertyChanged += UFProjectDocument_PropertyChanged;
                }

                var docManager = GetResourceDocumentManager(uri);

                if (docManager != null)
                    mapUriControllerData[relative].IsGeoScadaSupported = (docManager is UFProjectManagerComponent ? false : true);
#endif
                return mapUriControllerData[relative];
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal void RemoveControllerData(Uri uri, bool isFolder = false)
        {
            lock (mapUriControllerData)
            {
                uri = uri.GetUrlDecodedUri();
                if (mapUriControllerData.ContainsKey(uri))
                {
                    mapUriControllerData.Remove(uri);
                    NeedsSave = true;
                }

                var relative = GetControllerDataRelativeUri(uri).GetUrlDecodedUri();
                if (mapUriControllerData.ContainsKey(relative))
                {
                    mapUriControllerData.Remove(relative);
                    NeedsSave = true;
                }

                if (isFolder)
                {
                    var searchKey = relative.GetPathString();
                    var foundUris = (from c in mapUriControllerData.Keys
                                     where c.GetPathString() != searchKey && c.GetPathString().StartsWith(searchKey + "/")
                                     select c).ToList();

                    foreach (var uriOld in foundUris)
                    {
                        var oldPath = String.Format("{0}{1}", uriOld.GetPathString(), uriOld.GetPathString().Remove(0, searchKey.Length));
                        RemoveControllerData(uriOld);
                    }
                }
            }
        }

        internal void CopyControllerData(Uri uriOld, Uri uriNew, UFProjectDocument sourceProject, bool isFolder = false, bool isMovedItem = false)
        {
            lock (mapUriControllerData)
            {
                uriOld = uriOld.GetUrlDecodedUri();
                var relativeOld = sourceProject.GetControllerDataRelativeUri(uriOld).GetUrlDecodedUri();
                Uri relativeNew = null;
                if (sourceProject.mapUriControllerData.ContainsKey(relativeOld))
                {
                    uriNew = uriNew.GetUrlDecodedUri();
                    relativeNew = GetControllerDataRelativeUri(uriNew).GetUrlDecodedUri();
                    if (mapUriControllerData.ContainsKey(relativeNew))
                        mapUriControllerData.Remove(relativeNew);

                    var controllerData = new UFControllerData(sourceProject.mapUriControllerData[relativeOld]);
                    mapUriControllerData.Add(relativeNew, controllerData);
                    NeedsSave = true;
                }

                if (isFolder)
                {
                    var searchKey = relativeOld.GetPathString();
                    var foundUris = (from c in sourceProject.mapUriControllerData.Keys
                                     where c.GetPathString() != searchKey && c.GetPathString().StartsWith(searchKey)
                                     select c).ToList();

                    foreach (var uri in foundUris)
                    {
                        var oldPath = String.Format("{0}{1}", uriOld.GetPathString(), uri.GetPathString().Remove(0, searchKey.Length));
                        var newPath = String.Format("{0}{1}", uriNew.GetPathString(), uri.GetPathString().Remove(0, searchKey.Length));
                        CopyControllerData(new Uri(oldPath, UriKind.RelativeOrAbsolute), new Uri(newPath, UriKind.RelativeOrAbsolute), sourceProject);
                    }
                }
            }
        }
#endif
        internal UFChildProjectData GetChildProjectData(Uri uri, bool bCheckEvents = false)
        {
            lock (mapUriChildProjectData)
            {
                uri = uri.GetUrlDecodedUri();
                var relative = MakeRelativeUri(uri).GetUrlDecodedUri();
                if (!mapUriChildProjectData.ContainsKey(relative))
                {
#if !WINDOWS_UWP && !NET_STANDARD
                    bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(ProjectPath);
                    if (bTargetDataSource)
                    {
                        var path = uri.GetPathString();
                        if (path.StartsWith(projectDataTag))
                        {
                            path = path.Replace(String.Format("{0}\\", projectDataTag), "");
                            path = path.Replace(String.Format("{0}/", projectDataTag), "");
                            path = path.Replace("\\", "/");

                            try
                            {
                                uri = new Uri(path, UriKind.RelativeOrAbsolute).GetUrlDecodedUri();
                                relative = MakeRelativeUri(uri).GetUrlDecodedUri();
                            }
                            catch
                            {

                            }
                        }

                        if (!mapUriChildProjectData.ContainsKey(relative))
                        {
                            var controllerData = new UFChildProjectData();
                            mapUriChildProjectData.Add(relative, controllerData);
                        }
                    }
                    else
#endif
                    {
                        var controllerData = new UFChildProjectData();
                        mapUriChildProjectData.Add(relative, controllerData);
                    }
                }

#if !WINDOWS_UWP && !NET_STANDARD
                if (bCheckEvents)
                {
                    mapUriChildProjectData[relative].PropertyChanged += UFProjectDocument_PropertyChanged;
                }
#endif
                return mapUriChildProjectData[relative];
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal void RemoveChildProjectData(Uri uri)
        {
            lock (mapUriChildProjectData)
            {
                uri = uri.GetUrlDecodedUri();
                if (mapUriChildProjectData.ContainsKey(uri))
                {
                    mapUriChildProjectData.Remove(uri);
                    NeedsSave = true;
                }

                var relative = MakeRelativeUri(uri).GetUrlDecodedUri();
                if (mapUriChildProjectData.ContainsKey(relative))
                {
                    mapUriChildProjectData.Remove(relative);
                    NeedsSave = true;
                }
            }
        }

        internal void CopyChildProjectData(Uri uriOld, Uri uriNew)
        {
            lock (mapUriChildProjectData)
            {
                uriOld = uriOld.GetUrlDecodedUri();
                var relativeOld = MakeRelativeUri(uriOld).GetUrlDecodedUri();
                if (mapUriChildProjectData.ContainsKey(relativeOld))
                {
                    uriNew = uriNew.GetUrlDecodedUri();
                    var relativeNew = MakeRelativeUri(uriNew).GetUrlDecodedUri();
                    if (mapUriChildProjectData.ContainsKey(relativeNew))
                        mapUriChildProjectData.Remove(relativeNew);

                    var controllerData = new UFChildProjectData(mapUriChildProjectData[relativeOld]);
                    mapUriChildProjectData.Add(relativeNew, controllerData);
                    NeedsSave = true;
                }
            }
        }

        void UFProjectDocument_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NeedsSave = true;
        }
#endif
        internal ResourceFolderWatcher GetResourceFolderWatcher(String type)
        {
            return mapTypeResources[type];
        }
        internal Dictionary<String, String> GetRenamedResources()
        {
            return mapRenamedResources;
        }

        internal IDocumentManager GetResourceDocumentManager(String type)
        {
            return mapTypeDocumentManagers[type];
        }

        internal IDocumentManager GetResourceDocumentManager(Uri uri)
        {
            if (projectManagerComponent.UriRisolver != null)
            {
                var ret = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                if (ret != null)
                    return ret;
            }
            return projectManagerComponent;
        }

#if !WINDOWS_UWP
#if !NET_STANDARD
        internal IDocumentManager EditResource(Uri uri)
        {
            using (new WaitCursor())
            {
                if (fileSystemProvider == null && !uri.IsAbsoluteUri)
                    uri = new Uri(new Uri(ProjectFolder), uri);
                IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                manager.Edit(uri, this);
                return manager;
            }
        }
        
        internal void DeleteResource(Uri uri)
        {
            using (new WaitCursor())
            {
                if (fileSystemProvider == null && !uri.IsAbsoluteUri)
                    uri = new Uri(new Uri(ProjectFolder), uri);
                IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                manager.Delete(uri, this);
                
                RemoveControllerData(uri);
            }
        }

        internal bool RenameResource(Uri uri, String oldName, String newName)
        {
            try
            {
                IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
                manager.Rename(uri, oldName, newName, this);
            }
            catch
            {
                return false;
            }

            return true;
        }

        internal bool CopyResource(Uri uri, String newPath, bool bCopy)
        {
            IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
            manager.Copy(uri, newPath, bCopy, this, false);
            return true;
        }

        internal System.Windows.Media.ImageSource GetResourceImage(Uri uri)
        {
            IDocumentManager manager = projectManagerComponent.UriRisolver.ResolveUri(uri) as IDocumentManager;
            if (manager != null)
                return manager.TypeIcon;
            return projectManagerComponent.TypeIcon;
        }

        OPCUAEntityReference serverRedundancySubscriber;
        OPCUAEntityReference serverAlarmsSoundBuzzing;
        PropertyObserver<OPCUAEntityReference> observerAlarmsSoundBuzzing;
        PropertyObserver<MonitoredItemViewModel> observerAlarmsSoundBuzzingMonitoredModel;
        PropertyObserver<MonitoredItemViewModel> observerAlarmsSoundActiveMonitoredModel;
        List<OPCUAEntityReference> serverItems;
        Thread connectItemsThread;
        ManualResetEvent waitEvent;
        IBusyComponent busyComponent;
        int waitingItemsCount;

        public void PreSubscribeServerSession(IUFUAEditorManager ufuaeditormanager = null)
        {
            if (ufuaeditormanager == null)
                ufuaeditormanager = projectManagerComponent.UFUAEditor;
            else
                awaitConnectedItemsAtStartup = false;

            if (ufuaeditormanager == null)
                return;

            if (projectManagerComponent.UFUAEditor != null)
            {
                {
                    var opcString = ufuaeditormanager.GetNodeIdEntityReference(this,
                            String.Format("{0}/{1}", UFUAServerInfo.BrowserNames.AlarmsName,
                            UFUAServerInfo.BrowserNames.AlarmsSoundBuzzingName),
                            UFUAServerInfo.Guids.SystemTagsGuid.ToString());

                    try
                    {
                        serverAlarmsSoundBuzzing = opcString.FromXml<OPCUAEntityReference>();
                    }
                    catch
                    { }

                    if (serverAlarmsSoundBuzzing != null)
                    {
                        observerAlarmsSoundBuzzing = new PropertyObserver<OPCUAEntityReference>(serverAlarmsSoundBuzzing);
                        observerAlarmsSoundBuzzing.RegisterHandler(n => n.MonitoredItemViewModel, n =>
                        {
                            if (observerAlarmsSoundBuzzingMonitoredModel != null)
                                observerAlarmsSoundBuzzingMonitoredModel.Dispose();

                            observerAlarmsSoundBuzzingMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(n.MonitoredItemViewModel);
                            observerAlarmsSoundBuzzingMonitoredModel.RegisterHandler(m => m.Value, m =>
                            {
                                bool isBuzzing;
                                if (bool.TryParse(m.Value, out isBuzzing))
                                {
                                    Beeper.BeepState.ChangePlayState(isBuzzing);
                                }
                            });
                        });

                        serverAlarmsSoundBuzzing.SetInUse(this, true);
                        serverAlarmsSoundBuzzing.Resolve(Title);
                    }
                }

                var applicationName = ufuaeditormanager.GetDefApplicationName(this);
                if (RealTimeConnectionManagerViewModel.IsSessionServerRemote(Title, applicationName))
                {
                    var soundActive = SysVariables.SysVariables.GetSysVariables(this).GetVariable(SysVariables.SysNames.AlarmSoundActiveOnClient);
                    if (soundActive != null)
                    {
                        var invariantCultureValue = LoadStorageValue(SysVariables.SysNames.AlarmSoundActiveOnClient);
                        observerAlarmsSoundActiveMonitoredModel = new PropertyObserver<MonitoredItemViewModel>(soundActive);
                        observerAlarmsSoundActiveMonitoredModel.RegisterHandler(m => m.InvariantCultureValue, m =>
                        {
                            if (invariantCultureValue != m.InvariantCultureValue)
                            {
                                invariantCultureValue = m.InvariantCultureValue;
                                SaveStorageValue(SysVariables.SysNames.AlarmSoundActiveOnClient, m.InvariantCultureValue);
                            }

                            bool isActive;
                            if (bool.TryParse(m.InvariantCultureValue, out isActive))
                            {
                                Beeper.BeepState.ChangeShelveState(!isActive);
                            }
                        });

                        if (invariantCultureValue != null)
                        {
                            bool isSoundActive;
                            if (bool.TryParse(invariantCultureValue, out isSoundActive))
                            {
                                SysVariables.SysVariables.GetSysVariables(this).UpdateSysVariable(SysVariables.SysNames.AlarmSoundActiveOnClient, isSoundActive);
                            }
                        }
                    }
                }
            }

            if (ConnectItemsAtStartup && connectItemsThread == null)
            {
                waitEvent = new System.Threading.ManualResetEvent(false);
                if (AwaitConnectedItemsAtStartup)
                    busyComponent = GetService(typeof(IBusyComponent)) as IBusyComponent;
                var busyText = busyComponent?.BusyContent;
                connectItemsThread = new System.Threading.Thread((o) => 
                {
                    if (!AwaitConnectedItemsAtStartup)
                    {
                        System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
                        waitEvent.Set();
                    }

                    serverItems = new List<OPCUAEntityReference>();
                    var list = ufuaeditormanager.GetFlatListTags(this, inExecution: true);
                    if (list != null)
                    {
                        foreach (var tagName in list)
                        {
                            if (ProjectStatus.Terminating)
                                return;

                            var item = ufuaeditormanager.GetTagEntityReference(this, tagName, null, inExecution: true);
                            if (item != null)
                                serverItems.Add(item.FromXml<OPCUAEntityReference>());
                        }
                    }

                    var mapDefinitions = ufuaeditormanager.GetFlatListPrototypes(this, inExecution: true);
                    var mapPrototypes = ufuaeditormanager.GetFlatListPrototypeInstances(this, inExecution: true);
                    if (mapDefinitions != null && mapPrototypes != null)
                    {
                        foreach (var instance in mapPrototypes.Keys)
                        {
                            if (ProjectStatus.Terminating)
                                return;

                            if (mapPrototypes[instance] == null || !mapDefinitions.ContainsKey(mapPrototypes[instance]))
                                continue;

                            foreach (var member in mapDefinitions[mapPrototypes[instance]])
                            {
                                if (ProjectStatus.Terminating)
                                    return;

                                var item = ufuaeditormanager.GetTagEntityReference(this, member, instance, inExecution: true);
                                if (item != null)
                                    serverItems.Add(item.FromXml<OPCUAEntityReference>());
                            }
                        }
                    }

                    if (serverItems.Count > 0)
                    {
                        int pendingConnections = 0;
                        waitingItemsCount = serverItems.Count;
                        if (AwaitConnectedItemsAtStartup && busyComponent != null)
                        {
                            busyComponent.IsBusy = true;
                            busyComponent.BusyContent = Properties.Resources.ConnectItemsAtStartup;
                        }
                        foreach (var item in serverItems)
                        {
                            if (ProjectStatus.Terminating)
                                return;

                            item.PropertyChanged += Item_PropertyChanged;
                            item.SetInUse(this, true);
                            item.Resolve(Title);

                            if (!AwaitConnectedItemsAtStartup)
                            {
                                if (++pendingConnections > Properties.Settings.Default.ConnectItemsAtStartupPacketSize)
                                {
                                    pendingConnections = 0;
                                    if (Properties.Settings.Default.ConnectItemsAtStartupSleepTime > 0)
                                        System.Threading.Thread.Sleep(Properties.Settings.Default.ConnectItemsAtStartupSleepTime);
                                }
                            }
                        }
                    }
                });
                connectItemsThread.IsBackground = true;
                connectItemsThread.Start();

                if (AwaitConnectedItemsAtStartup && busyComponent != null)
                    busyComponent.Closed += BusyComponent_Closed;
                
                var pendingItems = waitingItemsCount;
                while (true)
                {
                    var signaled = waitEvent.WaitOne(Properties.Settings.Default.ConnectItemsAtStartupTimeout);
                    if (signaled)
                        break;
                    else if (pendingItems == waitingItemsCount)
                    {
                        log.Info(Properties.Resources.TimeoutConnectingItemsAtStartup);
                        break;
                    }
                    pendingItems = waitingItemsCount;
                }
                if (AwaitConnectedItemsAtStartup && busyComponent != null)
                {
                    awaitConnectedItemsAtStartup = false;
                    busyComponent.Closed -= BusyComponent_Closed;
                    busyComponent.IsBusy = false;
                    busyComponent.BusyContent = busyText;
                    busyComponent.ResetProgress();
                }
                waitEvent.Dispose();
            }
        }

        void BusyComponent_Closed(object sender, EventArgs e)
        {
            busyComponent.Closed -= BusyComponent_Closed;
            waitEvent.Set();
        }

        void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = (OPCUAEntityReference)sender;
            if (e.PropertyName == "MonitoredItemViewModel")
            {
                item.PropertyChanged -= Item_PropertyChanged;
                item.SetInUse(this, false);

                var currentWaitingItemsCount = System.Threading.Interlocked.Decrement(ref waitingItemsCount);
                if (currentWaitingItemsCount >= 0)
                {
                    var percentRate = ((double)(serverItems.Count - currentWaitingItemsCount) / (double)serverItems.Count) * 100.0;
                    var sysVariables = OPCUAEntityReference.GetDataSinkInterface(SysVariables.SysNames.dataSynkName);
                    sysVariables?.UpdateVariable(SysVariables.SysNames.PercentageOfConnectedStartupTags, new Opc.Ua.DataValue(new Opc.Ua.Variant(percentRate), Opc.Ua.StatusCodes.Good));
                    if (AwaitConnectedItemsAtStartup)
                    {
                        busyComponent?.SetProgress(percentRate);
                        if (currentWaitingItemsCount == 0)
                            waitEvent.Set();
                    }
                }
            }
        }

        public void UnsubscribeServerSession()
        {
            if (eventLogCommand != null)
            {
                eventLogCommand.Dispose();
                eventLogCommand = null;
            }

            if (serverRedundancySubscriber != null)
            {
                serverRedundancySubscriber.SetInUse(this, false);
                serverRedundancySubscriber = null;
            }

            if (serverAlarmsSoundBuzzing != null)
            {
                serverAlarmsSoundBuzzing.SetInUse(this, false);
                serverAlarmsSoundBuzzing = null;

                if (observerAlarmsSoundBuzzingMonitoredModel != null)
                {
                    observerAlarmsSoundBuzzingMonitoredModel.UnregisterHandler(m => m.DataValue);
                    observerAlarmsSoundBuzzingMonitoredModel.Dispose();
                    observerAlarmsSoundBuzzingMonitoredModel = null;
                }

                if (observerAlarmsSoundBuzzing != null)
                {
                    observerAlarmsSoundBuzzing.UnregisterHandler(p => p.MonitoredItemViewModel);
                    observerAlarmsSoundBuzzing.Dispose();
                    observerAlarmsSoundBuzzing = null;
                }

                if (observerAlarmsSoundActiveMonitoredModel != null)
                {
                    observerAlarmsSoundActiveMonitoredModel.UnregisterHandler(m => m.DataValue);
                    observerAlarmsSoundActiveMonitoredModel.Dispose();
                    observerAlarmsSoundActiveMonitoredModel = null;
                }

                Beeper.BeepState.ShutDown();
            }

            if (connectItemsThread != null)
            {
                connectItemsThread.Join();
                connectItemsThread = null;
            }

            if (serverItems != null)
            {
                foreach (var item in serverItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                    if (item.MonitoredItemViewModel == null)
                        item.SetInUse(this, false);
                }
                serverItems.Clear();
                serverItems = null;
            }
        }

        bool bSubscribePromptLogin;
        Uri loginScreen;
        bool bSubscribePromptVerifyLogin;
        Uri verifyPasswordScreen;
        internal void SubscribeAuthenticationEvents()
        {
            if (projectManagerComponent.UserEditor != null && Parent == null)
            {
                verifyPasswordScreen = GetVerifyPasswordScreen();
                if (verifyPasswordScreen != null)
                {
                    bSubscribePromptVerifyLogin = true;
                    projectManagerComponent.UserEditor.PromptVerifyPassword += UserEditor_PromptVerifyPassword;
                }
            }

            if (projectManagerComponent.UFUAEditor != null)
            {
                var server = projectManagerComponent.UFUAEditor.GetServerEntityReference(this, bCheckEmpty: true);
                if (server == null)
                    return;

                if (eventLogCommand == null)
                {
                    eventLogCommand = new EventLogCommand();
                    eventLogCommand.Init(this, this, Title);
                }

                try
                {
                    serverRedundancySubscriber = server.FromXml<OPCUAEntityReference>();

                    serverRedundancySubscriber.SetInUse(this, true);
                    serverRedundancySubscriber.Resolve(Title);
                }
                catch
                {
                }
            }

            if (projectManagerComponent.AuthenticationCredentialsProvider == null)
                return;

            //if (projectManagerComponent.UserEditor != null)
            //    maxInvalidPasswordAttempts = projectManagerComponent.UserEditor.GetMaxInvalidPasswordAttempts(this);
            projectManagerComponent.AuthenticationCredentialsProvider.FailedLogin += AuthenticationCredentialsProvider_FailedLogin;
            projectManagerComponent.AuthenticationCredentialsProvider.UserUnlocked += AuthenticationCredentialsProvider_UserUnlocked;
            projectManagerComponent.AuthenticationCredentialsProvider.FaultedProvider += AuthenticationCredentialsProvider_FaultedProvider;
            projectManagerComponent.AuthenticationCredentialsProvider.UserOnline += AuthenticationCredentialsProvider_UserOnline;

            if (Parent == null)
            {
                loginScreen = GetLoginScreen();
                if (loginScreen != null)
                {
                    bSubscribePromptLogin = true;
                    projectManagerComponent.AuthenticationCredentialsProvider.PromptUserLogin += AuthenticationCredentialsProvider_PromptUserLogin;
                }
            }
        }

        private void UserEditor_PromptVerifyPassword(object sender, UFUserEditor.ComponentService.VerifyPasswordEventArgs e)
        {
            var managerscreen = GetScreenDocumentManager();
            if (managerscreen == null)
                return;

            DataSinkInterface dataSinkInterface = null;
            var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            foreach (var dataSink in listDataSinkInterfaces)
            {
                var ds = OPCUAEntityReference.GetDataSinkInterface(dataSink);
                if (ds.GetVariable(_Login_User_Variable, this) != null &&
                    ds.GetVariable(_Login_VerifyPassword_Variable, this) != null &&
                    ds.GetVariable(_Login_OldPassword_Variable, this) != null &&
                    ds.GetVariable(_Login_Password_Variable, this) != null)
                {
                    dataSinkInterface = ds;
                    break;
                }
            }

            if (dataSinkInterface == null)
                return;

            var temp = dataSinkInterface.GetVariable(_Login_User_Variable, this);
            if (temp != null)
                temp.WriteValue(e.User);
            temp = dataSinkInterface.GetVariable(_Login_Password_Variable, this);
            if (temp != null)
                temp.WriteValue(String.Empty);
            temp = dataSinkInterface.GetVariable(_Login_OldPassword_Variable, this);
            if (temp != null)
                temp.WriteValue(String.Empty);
            temp = dataSinkInterface.GetVariable(_Login_VerifyPassword_Variable, this);
            if (temp != null)
                temp.WriteValue(String.Empty);
            temp = dataSinkInterface.GetVariable(_Login_ChangePassword_Force, this);
            if (temp != null)
                temp.WriteValue(e.bForce);

            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null)
                temp.WriteValue("0");

            var map = new Dictionary<string, object>();
            managerscreen.Execute(verifyPasswordScreen, this, ExecutionMode.Synchro, map);

            e.requestingVerifyPassword = true;
            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null && temp.DataValue != null)
            {
                var okRet = String.Format("{0}", temp.DataValue.Value);
                var bok = false;
                try
                {
                    bok = Convert.ToBoolean(okRet);
                }
                catch
                { }

                e.dialogResult = bok;
                if (bok)
                {
                    temp = dataSinkInterface.GetVariable(_Login_VerifyPassword_Variable, this);
                    if (temp != null && temp.DataValue != null)
                        e.VerifyPassword = temp.DataValue.Value as String;
                    temp = dataSinkInterface.GetVariable(_Login_Password_Variable, this);
                    if (temp != null && temp.DataValue != null)
                        e.Password = temp.DataValue.Value as String;
                    temp = dataSinkInterface.GetVariable(_Login_OldPassword_Variable, this);
                    if (temp != null && temp.DataValue != null)
                        e.OldPassword = temp.DataValue.Value as String;
                }
            }
        }

        bool bSubscribePromptPads;
        Uri numericPadScreen;
        Uri alphanumericPadScreen;
        Uri numericPasswordPadScreen;
        Uri alphanumericPasswordPadScreen;
        internal void SubscribePadsEvents()
        {
            if (Parent != null)
                return;
            numericPadScreen = GetNumericPadScreen();
            alphanumericPadScreen = GetAlphanumericPadScreen();
            numericPasswordPadScreen = GetNumericPasswordPadScreen();
            alphanumericPasswordPadScreen = GetAlphanumericPasswordPadScreen();

            if (numericPadScreen != null ||
                alphanumericPadScreen != null ||
                numericPasswordPadScreen != null ||
                alphanumericPasswordPadScreen != null)
            {
                bSubscribePromptPads = true;
                Pads.Pads.PromptPads += Pads_PromptPads;
            }
        }

        internal void UnsubscribeAuthenticationEvents()
        {
            if (projectManagerComponent.UserEditor != null)
            {
                if (bSubscribePromptVerifyLogin)
                {
                    bSubscribePromptVerifyLogin = false;
                    projectManagerComponent.UserEditor.PromptVerifyPassword -= UserEditor_PromptVerifyPassword;
                }
            }

            if (projectManagerComponent.AuthenticationCredentialsProvider == null)
                return;

            projectManagerComponent.AuthenticationCredentialsProvider.FailedLogin -= AuthenticationCredentialsProvider_FailedLogin;
            projectManagerComponent.AuthenticationCredentialsProvider.UserUnlocked -= AuthenticationCredentialsProvider_UserUnlocked;
            projectManagerComponent.AuthenticationCredentialsProvider.FaultedProvider -= AuthenticationCredentialsProvider_FaultedProvider;
            projectManagerComponent.AuthenticationCredentialsProvider.UserOnline -= AuthenticationCredentialsProvider_UserOnline;
            if (bSubscribePromptLogin)
            {
                bSubscribePromptLogin = false;
                projectManagerComponent.AuthenticationCredentialsProvider.PromptUserLogin -= AuthenticationCredentialsProvider_PromptUserLogin;
            }
        }

        internal void UnsubscribePadsEvents()
        {
            if (!bSubscribePromptPads)
                return;
            bSubscribePromptPads = false;
            Pads.Pads.PromptPads -= Pads_PromptPads;
        }

        static readonly String _Pad_Value_Variable = "PadValueVariable";
        static readonly String _Pad_MinValue_Variable = "PadMinValueVariable";
        static readonly String _Pad_MaxValue_Variable = "PadMaxValueVariable";
        static readonly String _Pad_OK = "PadOK";
        static readonly String _Last_Error = "LastError";
        static readonly String _Pad_Title = "PadTitle";
        private const string PadTagDescription = "PadTagDescription";
        private const string PadShowTagDescription = "PadShowTagDescription";

        static readonly String _AlphaNumericPadMaxLength = "AlphaNumericPadMaxLength";
        static readonly String _NumericPadNotValid = "NumericPadNotValid";
        static readonly String _NumericPadNotInRange = "NumericPadNotInRange";

        private void Pads_PromptPads(object sender, Pads.PadEventArgs e)
        {
            var managerscreen = GetScreenDocumentManager();
            if (managerscreen == null)
                return;

            Uri screen = null;
            if (e.isAlphaNumeric)
            {
                if (e.isPassword)
                    screen = alphanumericPasswordPadScreen;
                else
                    screen = alphanumericPadScreen;
            }
            else
            {
                if (e.isPassword)
                    screen = numericPasswordPadScreen;
                else
                    screen = numericPadScreen;
            }
            if (screen == null)
                return;

            DataSinkInterface dataSinkInterface = null;
            var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            foreach (var dataSink in listDataSinkInterfaces)
            {
                var ds = OPCUAEntityReference.GetDataSinkInterface(dataSink);
                if (ds.GetVariable(_Pad_Value_Variable, this) != null)
                {
                    dataSinkInterface = ds;
                    break;
                }
            }

            if (dataSinkInterface == null)
                return;

            var temp = dataSinkInterface.GetVariable(_Pad_Value_Variable, this);
            if (temp != null)
                temp.WriteValue(e.value);
            temp = dataSinkInterface.GetVariable(_Pad_MinValue_Variable, this);
            if (temp != null)
            {
                if (e.min.HasValue)
                    temp.WriteValue(e.min.Value);
                else
                    temp.WriteValue(0);
            }
            temp = dataSinkInterface.GetVariable(_Pad_MaxValue_Variable, this);
            if (temp != null)
            {
                if (e.max.HasValue)
                    temp.WriteValue(e.max.Value);
                else
                    temp.WriteValue(0);
            }
            temp = dataSinkInterface.GetVariable(_Pad_Title, this);
            if (temp != null)
            {
                if (String.IsNullOrEmpty(e.title))
                    temp.WriteValue(String.Empty);
                else
                    temp.WriteValue(e.title);
            }

            temp = dataSinkInterface.GetVariable(PadShowTagDescription, this);
            if (temp != null)
                temp.WriteValue(e.ShowTagDescription);

            if (e.ShowTagDescription)
            {
                temp = dataSinkInterface.GetVariable(PadTagDescription, this);
                if (temp != null)
                {
                    var value = GetLocalizedText(e.TagDescription);
                    temp.WriteValue(value);
                }
            }

            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null)
                temp.WriteValue("0");

            var map = new Dictionary<string, object>();
            map.Add("X", e.x == -1 ? Double.NaN : e.x);
            map.Add("Y", e.y == -1 ? Double.NaN : e.y);
            map.Add("IsRelative", e.isRelative);
            map.Add("IsPromptPadRequest", true);

        requestpad:
            managerscreen.Execute(screen, this, ExecutionMode.Synchro, map);

            e.requestingPads = true;
            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null && temp.DataValue != null)
            {
                var okRet = String.Format("{0}", temp.DataValue.Value);
                var bok = false;
                try
                {
                    bok = Convert.ToBoolean(okRet);
                }
                catch
                { }

                if (bok)
                {
                    temp = dataSinkInterface.GetVariable(_Pad_Value_Variable, this);
                    if (temp != null && temp.DataValue != null)
                    {
                        var strvalue = temp.DataValue.Value as String;
                        if (e.isAlphaNumeric && e.max.HasValue)
                        {
                            if (strvalue.Length > e.max.Value)
                            {
                                temp = dataSinkInterface.GetVariable(_Last_Error, this);
                                if (temp != null)
                                {
                                    var error = _AlphaNumericPadMaxLength;
                                    if (Strings != null)
                                    {
                                        var id = Strings.GetStringFromID(_AlphaNumericPadMaxLength);
                                        if (!String.IsNullOrEmpty(id))
                                            error = id;
                                    }
                                    temp.WriteValue(error);
                                }
                                goto requestpad;
                            }
                        }
                        if (!e.isAlphaNumeric && (e.min.HasValue || e.max.HasValue))
                        {
                            double value;
                            if (!Double.TryParse(strvalue, out value))
                            {
                                temp = dataSinkInterface.GetVariable(_Last_Error, this);
                                if (temp != null)
                                {
                                    var error = _NumericPadNotValid;
                                    if (Strings != null)
                                    {
                                        var id = Strings.GetStringFromID(_NumericPadNotValid);
                                        if (!String.IsNullOrEmpty(id))
                                            error = id;
                                    }
                                    temp.WriteValue(error);
                                }
                                goto requestpad;
                            }

                            if (e.min.HasValue && value < e.min.Value ||
                                e.max.HasValue && value > e.max.Value)
                            {
                                temp = dataSinkInterface.GetVariable(_Last_Error, this);
                                if (temp != null)
                                {
                                    var error = _NumericPadNotInRange;
                                    if (Strings != null)
                                    {
                                        var id = Strings.GetStringFromID(_NumericPadNotInRange);
                                        if (!String.IsNullOrEmpty(id))
                                            error = id;
                                    }
                                    temp.WriteValue(error);
                                }
                                goto requestpad;
                            }
                        }

                        e.value = strvalue;
                    }
                }
            }
        }

        private string GetLocalizedText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            if (GetService(typeof(IStringEditorManager)) is IStringEditorManager stringEditorManager)
            {
                var activeCulture = stringEditorManager.GetActiveCulture(this);
                var mapValues = stringEditorManager.GetListStringForCulture(this, activeCulture);
                if (mapValues.ContainsKey(text))
                    return mapValues[text];
            }

            return text;
        }

        static readonly String _Login_User_Variable = "LoginUserVariable";
        static readonly String _Login_Password_Variable = "LoginPasswordVariable";
        static readonly String _Login_OldPassword_Variable = "LoginOldPasswordVariable";
        static readonly String _Login_VerifyPassword_Variable = "LoginVerifyPasswordVariable";
        static readonly String _Login_ChangePassword_Force = "LoginChangePasswordForce";
        static readonly String _Login_RequestRole_Variable = "LoginRequestRoleVariable";
        static readonly String _Login_RequestUser_Variable = "LoginRequestUserVariable";
        static readonly String _Login_RequestLevel_Variable = "LoginRequestLevelVariable";

        private void AuthenticationCredentialsProvider_PromptUserLogin(object sender, LoginInfoEventArgs e)
        {
            var managerscreen = GetScreenDocumentManager();
            if (managerscreen == null)
                return;

            DataSinkInterface dataSinkInterface = null;
            var listDataSinkInterfaces = OPCUAEntityReference.GetDataSinkInterfaces();
            foreach(var dataSink in listDataSinkInterfaces)
            {
                var ds = OPCUAEntityReference.GetDataSinkInterface(dataSink);
                if (ds.GetVariable(_Login_User_Variable, this) != null &&
                    ds.GetVariable(_Login_Password_Variable, this) != null)
                {
                    dataSinkInterface = ds;
                    break;
                }
            }

            if (dataSinkInterface == null)
                return;

            var temp = dataSinkInterface.GetVariable(_Login_User_Variable, this);
            if (temp != null)
                temp.WriteValue(String.Empty);
            temp = dataSinkInterface.GetVariable(_Login_Password_Variable, this);
            if (temp != null)
                temp.WriteValue(String.Empty);

            temp = dataSinkInterface.GetVariable(_Login_RequestRole_Variable, this);
            if (temp != null)
                temp.WriteValue(e.requestedRole);
            temp = dataSinkInterface.GetVariable(_Login_RequestUser_Variable, this);
            if (temp != null)
                temp.WriteValue(e.requestedUser);
            temp = dataSinkInterface.GetVariable(_Login_RequestLevel_Variable, this);
            if (temp != null)
                temp.WriteValue(e.requestedLevel);

            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null)
                temp.WriteValue("0");

            var map = new Dictionary<string, object>();
            managerscreen.Execute(loginScreen, this, ExecutionMode.Synchro, map);

            e.requestingUserAuthentication = true;
            temp = dataSinkInterface.GetVariable(_Pad_OK, this);
            if (temp != null && temp.DataValue != null)
            {
                var okRet = String.Format("{0}", temp.DataValue.Value);
                var bok = false;
                try
                {
                    bok = Convert.ToBoolean(okRet);
                }
                catch
                { }

                e.dialogResult = bok;
                if (bok)
                {
                    temp = dataSinkInterface.GetVariable(_Login_User_Variable, this);
                    if (temp != null && temp.DataValue != null)
                        e.User = temp.DataValue.Value as String;
                    temp = dataSinkInterface.GetVariable(_Login_Password_Variable, this);
                    if (temp != null && temp.DataValue != null)
                        e.Password = temp.DataValue.Value as String;
                }
            }
        }

        int nCounterLoginFailed = 0;
        void AuthenticationCredentialsProvider_FailedLogin(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            if (ev.ApplicationName == Title)
            {
                if (ev.IsLocked)
                {
                    var message = String.Format(Properties.Resources.UserLockedFailedLogIn, ev.User);
                    logUsers.Info(message);
                    AddLogEntity(System.Net.Dns.GetHostName(), DateTime.UtcNow, message, System.Diagnostics.EventLogEntryType.Information);
                }
                else // if (++nCounterLoginFailed >= maxInvalidPasswordAttempts)
                {
                    var message = String.Format(Properties.Resources.UserFailedLogIn, ev.User);
                    logUsers.Info(message);
                    AddLogEntity(System.Net.Dns.GetHostName(), DateTime.UtcNow, message, System.Diagnostics.EventLogEntryType.Information);
                }
            }
        }

        void AuthenticationCredentialsProvider_UserUnlocked(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            if (ev.ApplicationName == Title)
            {
                var message = String.Format(Properties.Resources.UserUnlocked, ev.User);
                logUsers.Info(message);
                AddLogEntity(System.Net.Dns.GetHostName(), DateTime.UtcNow, message, null, null, lastUser, System.Diagnostics.EventLogEntryType.Information);
            }
        }

        void AuthenticationCredentialsProvider_FaultedProvider(object sender, LoginInfoEventArgs ev)
        {
            if (ev.ApplicationName == Title && string.IsNullOrEmpty(ev.ErrorInfo) == false)
            {
                var message = String.Format(Resources.ExternalAuthenticationFailed, ev.ErrorInfo);

                logUsers.Warn(message);
                AddLogEntity(System.Net.Dns.GetHostName(), DateTime.UtcNow, message, EventLogEntryType.Warning);   
            }
        }

        string lastUser;
        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs ev)
        {
            nCounterLoginFailed = 0;
            if (ev.IsRefreshing || lastUser == ev.User)
                return;
            lastUser = ev.User;

            if (serverRedundancySubscriber == null)
                return;
            
            System.Threading.ThreadPool.QueueUserWorkItem((o) => 
            {
                if (bDisposed)
                    return;

                var oldPriority = System.Threading.Thread.CurrentThread.Priority;
                System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;

                Opc.Ua.IUserIdentity identity = null;
                if (!String.IsNullOrEmpty(ev.User))
                    identity = new Opc.Ua.UserIdentity(ev.User, ev.Password);

                try
                {
                    RealTimeConnectionManagerViewModel.SetUserIdentity(Title, identity, new Opc.Ua.StringCollection());
                }
                catch (Exception ex)
                {
                    if (projectManagerComponent.Workspace != null)
                        projectManagerComponent.Workspace.ShowTaskBarTooltip(Title,
                            String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message), 5000);
                    logUsers.Warn(String.Format(Properties.Resources.FailedToActivateUserOnServer, ex.Message));
                }
                finally
                {
                    System.Threading.Thread.CurrentThread.Priority = oldPriority;
                }
            });
        }
#endif

        String oldLogFileName;
        public void SetCurrentLogFileName()
        {
#if !NET_STANDARD
            if (fileSystemProviderBase != null)
                return;
#endif
            if (!String.IsNullOrEmpty(oldLogFileName))
                return;

            var rootAppender = ((Hierarchy)LogManager.GetRepository(
#if NET_STANDARD
                System.Reflection.Assembly.GetEntryAssembly()
#endif
                )).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            if (rootAppender == null)
                return;

            oldLogFileName = rootAppender.File;
            var filename = Path.GetFileNameWithoutExtension(oldLogFileName);

            var sourceDir = System.IO.Path.GetDirectoryName(ProjectPath);
            var logPath =
#if NET_STANDARD
                Properties.Settings.Default.NetCoreLogPath.Replace('\\', Path.DirectorySeparatorChar);
#else
                Properties.Settings.Default.LogPath;
#endif

            var logPathFilename = Path.GetFileNameWithoutExtension(logPath);
            logPath = logPath.Replace(logPathFilename, filename);

            sourceDir = String.Format("{0}{2}{1}", sourceDir, logPath, Path.DirectorySeparatorChar);
            rootAppender.File = sourceDir;
            rootAppender.ActivateOptions();
        }

        internal void RestoreCurrentLogFileName()
        {
            if (String.IsNullOrEmpty(oldLogFileName))
                return;

            var rootAppender = ((Hierarchy)LogManager.GetRepository(
#if NET_STANDARD
                System.Reflection.Assembly.GetEntryAssembly()
#endif
                )).Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            if (rootAppender == null)
                return;
            rootAppender.File = oldLogFileName;
            rootAppender.ActivateOptions();
            oldLogFileName = null;
        }
#endif
        public void UpdateSessionSettings()
        {
            var map = new Dictionary<String, AppNameSettings>();
            IClientEditorManager clientEditorManager = GetService(typeof(IClientEditorManager)) as IClientEditorManager;
            if (clientEditorManager != null)
            {
                var _map = clientEditorManager.GetAppNameSettings(this);
                if (_map != null)
                {
                    foreach (var entry in _map.Keys)
                        map.Add(entry, _map[entry] as AppNameSettings);
                }
            }

            foreach (var entry in MapAppNameSettings.Keys)
            {
                if (!map.ContainsKey(entry))
                    map.Add(entry, MapAppNameSettings[entry]);
            }

            String[] serverUriArray = null;
#if !WINDOWS_UWP && !NET_STANDARD
            if (projectManagerComponent.UFUAEditor != null)
                serverUriArray = projectManagerComponent.UFUAEditor.GetServerUriArray(this);
#endif

            String parentTitle = null;
            if (Parent is UFProjectDocument &&
                serverUriArray == null)
            {
                //var projectName = System.IO.Path.GetFileNameWithoutExtension(ProjectPath);
                var parentDoc = Parent as UFProjectDocument;
                //if (parentDoc.MapAppNameChildProjectSettings.ContainsKey(projectName))
                //{
                //    if (map.ContainsKey(projectName))
                //        map.Remove(projectName);
                //    map.Add(projectName, parentDoc.MapAppNameChildProjectSettings[projectName]);
                //}
                parentTitle = parentDoc.Title;
            }

            RealTimeConnectionManagerViewModel.AddSessionSettings(Title, new SessionSettings()
                                                        {
                                                            ParentTitle = parentTitle,
                                                            ConnectItemsAtStartup = this.ConnectItemsAtStartup,
                                                            RemoveDisabledItemAfterSecs = this.RemoveDisabledItemAfterSecs,
                                                            MaxCleanCount = this.MaxCleanCount,
                                                            UseAlwaysSecureConnections = this.UseAlwaysSecureConnections,
                                                            SlowSamplingInterval = this.SlowSamplingInterval,
                                                            DisableWhenNotUsed = this.DisableWhenNotUsed,
                                                            PublishingInterval = this.PublishingInterval,
                                                            FastSamplingInterval = this.FastSamplingInterval,
                                                            ServerArray = serverUriArray,
                                                            MapAppNameSettings = map
                                                        });
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void enableProtection()
        {
            temporaryDisableProtection = false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void disableProtection()
        {
            temporaryDisableProtection = true;
        }
#endregion

#region Commands

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool CanSave
        {
            get { return IsValid; }
        }

        RelayCommand _newResourceCommand;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand NewResourceCommand
        {
            get
            {
                if (_newResourceCommand == null)
                {
                    _newResourceCommand = new RelayCommand(
                        param =>
                        {
                            CreateNewResource(param as String);

                            // Dirty the commands registered with CommandManager,
                            // such as our Save command, so that they are queried
                            // to see if they can execute now.
                            CommandManager.InvalidateRequerySuggested();
                        },
                        param => CanNewResourceCommand
                        );
                }
                return _newResourceCommand;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool CanNewResourceCommand
        {
            get { return IsValid; }
        }

        private void UpdateFileSystemProvider()
        {
            try
            {
                var helper = new ConnectionStringParser(ProjectPath);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (!String.IsNullOrEmpty(providerType))
                {
                    fileSystemProvider = new DataSourceFileSystemProvider("")
                                        {
                                            ConnectionString = ProjectPath
                                        };
                }
            }
            catch (Exception ex)
            {
             
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddChildProject(Uri uri)
        {
            var urlUri = uri.GetUrlDecodedUri();
            if (ListChildProjectPaths.Contains(urlUri))
                return;
            ListChildProjectPaths.Add(urlUri);
            OnPropertyChanged("ListChildProjectPaths");
            NeedsSave = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RemoveChildProject(Uri uri)
        {
            var urlUri = uri.GetUrlDecodedUri();
            if (!ListChildProjectPaths.Contains(urlUri))
                return;
            ListChildProjectPaths.Remove(urlUri);
            OnPropertyChanged("ListChildProjectPaths");
            NeedsSave = true;
        }

        public Dictionary<string, IDocument> GetAllProjectsDocuments()
        {
            return GetAllProjectsDocuments(this); ;
        }

        private Dictionary<string, IDocument> GetAllProjectsDocuments(IDocument doc)
        {
            var map = new Dictionary<string, IDocument>();
            if (projectManagerComponent.UFUAEditor != null)
            {
                var appName = projectManagerComponent.UFUAEditor.GetAplicationName(doc);
                if(appName != null)
                {
                    if(!map.ContainsKey(appName))
                        map.Add(appName, doc);
                }                   
            }

            var children = doc.Childs ?? doc.Parent?.Childs ?? new List<IDocument>();
            foreach (var child in children)
            {
                var childMap = GetAllProjectsDocuments(child);
                foreach(var c in childMap)
                { 
                    if(!map.ContainsKey(c.Key))
                    {
                        map.Add(c.Key, c.Value);
                    }
                    else
                    {
                        log.Warn(string.Format(Resources.ErrorDuplicateProjectName, c.Key));
                    }
                }                
            }
            return map;
        }


        public void AddStartupScript(StartupScript sc)
        {
            if (ListStartupScripts.Contains(sc))
                return;
            ListStartupScripts.Add(sc);
            OnPropertyChanged("ListStartupScripts");
            NeedsSave = true;
        }

        public void RemoveStartupScript(StartupScript sc)
        {
            if (!ListStartupScripts.Contains(sc))
                return;
            ListStartupScripts.Remove(sc);
            OnPropertyChanged("ListStartupScripts");
            NeedsSave = true;
        }

        public void ClearStartupScripts()
        {
            ListStartupScripts.Clear();
            OnPropertyChanged("ListStartupScripts");
            NeedsSave = true;
        }

        internal void AddStartupLogic(StartupLogic sc)
        {
            if (ListStartupLogics.Contains(sc))
                return;
            ListStartupLogics.Add(sc);
            OnPropertyChanged("ListStartupLogics");
            NeedsSave = true;
        }

        internal void RemoveStartupLogic(StartupLogic sc)
        {
            if (!ListStartupLogics.Contains(sc))
                return;
            ListStartupLogics.Remove(sc);
            OnPropertyChanged("ListStartupLogics");
            NeedsSave = true;
        }

        internal void ClearStartupLogics()
        {
            ListStartupLogics.Clear();
            OnPropertyChanged("ListStartupLogics");
            NeedsSave = true;
        }

        internal void AddAutoloadScreen(AutoloadScreen sc)
        {
            if (AutoloadScreenList.Contains(sc))
                return;
            AutoloadScreenList.Add(sc);
            OnPropertyChanged("AutoloadScreenList");
            NeedsSave = true;
        }

        internal void RemoveAutoloadScreen(AutoloadScreen sc)
        {
            if (!AutoloadScreenList.Contains(sc))
                return;
            AutoloadScreenList.Remove(sc);
            OnPropertyChanged("AutoloadScreenList");
            NeedsSave = true;
        }

        internal void ClearAutoloadScreenList()
        {
            AutoloadScreenList.Clear();
            OnPropertyChanged("AutoloadScreenList");
            NeedsSave = true;
        }
#endif
#endregion

#region Properties

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public Guid ConfigurationId
        {
            get
            {
                return projectId;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String OriginalVersion
        {
            get
            {
                return originalVersion;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public String ProjectVersion
        {
            get
            {
                return version;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool IsValid
        {
            get
            {
                return !String.IsNullOrEmpty(ProjectPath);
            }
        }

        String projectPath;
        public String ProjectPath
        {
            get
            {
                return projectPath;
            }
            internal set
            {
                if (projectPath == value)
                    return;

                projectPath = value;

#if !WINDOWS_UWP && !NET_STANDARD
                UpdateFileSystemProvider();
#endif
            }
        }

        public String ProjectFolder
        {
            get
            {
#if !WINDOWS_UWP && !NET_STANDARD
                bool bTargetDataSource = XpoHelpers.XpoHelper.IsDataSource(ProjectPath);
                if (bTargetDataSource)
                    return projectDataTag;
#endif
                String ret = String.Format("{0}{2}{1}", Path.GetDirectoryName(ProjectPath), Path.GetFileNameWithoutExtension(ProjectPath), Path.DirectorySeparatorChar);
                return ret;
            }
            set
            {
                return;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public ProjectStatus ProjectStatus
        {
            get
            {
                return projectStatus;
            }
        }

#if !WINDOWS_UWP && !NET_STANDARD
        private bool _NeedsSave = false;
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool NeedsSave
        {
            get { return _NeedsSave; }
            set
            {
                if (_NeedsSave == value)
                    return;

                _NeedsSave = value;
                OnPropertyChanged("NeedsSave");
            }
        }
#endif

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<String> ResourcTypes
        {
            get
            {
                return new List<String>(mapTypeResources.Keys);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> ListChildProjectPaths
        {
            get
            {
                if (listChildProjectPaths == null)
                    listChildProjectPaths = new List<Uri>();
                return listChildProjectPaths;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (listChildProjectPaths == value)
                    return;
                listChildProjectPaths = value;
                OnPropertyChanged("ListChildProjectPaths");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<StartupScript> ListStartupScripts
        {
            get
            {
                if (listStartupScriptsEx == null)
                    listStartupScriptsEx = new List<StartupScript>();
                return listStartupScriptsEx;
            }
            set
            {
                if (listStartupScriptsEx == value)
                    return;
                listStartupScriptsEx = value;
                OnPropertyChanged("ListStartupScripts");
                NeedsSave = true;
            }
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<StartupLogic> ListStartupLogics
        {
            get
            {
                if (listStartupLogicsEx == null)
                    listStartupLogicsEx = new List<StartupLogic>();
                return listStartupLogicsEx;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (listStartupLogicsEx == value)
                    return;
                listStartupLogicsEx = value;
                OnPropertyChanged("ListStartupLogics");
                NeedsSave = true;
            }
#endif
        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<AutoloadScreen> AutoloadScreenList
        {
            get
            {
                if (autoloadScreenListEx == null)
                    autoloadScreenListEx = new List<AutoloadScreen>();
                return autoloadScreenListEx;
            }
#if !WINDOWS_UWP
            set
            {
                if (autoloadScreenListEx == value)
                    return;
                autoloadScreenListEx = value;
                OnPropertyChanged("AutoloadScreenList");
                NeedsSave = true;
            }
#endif
        }
#endif
        public StartType StartType
        {
            get
            {
                return startType == StartType.GalleryPage ? StartType.TilePage : startType;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (startType == value)
                    return;
                startType = value;
                OnPropertyChanged("StartType");
                NeedsSave = true;
            }
#endif
        }

        public ThemeType ThemeType
        {
            get
            {
                return theme;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (theme == value)
                    return;
                theme = value;
                OnPropertyChanged("Theme");
                NeedsSave = true;
            }
#endif
        }

        /*
        public TouchType TouchType
        {
            get
            {
                return touchType;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (touchType == value)
                    return;
                touchType = value;
                OnPropertyChanged("TouchType");
                NeedsSave = true;
            }
#endif
        }
        */

        public String SpeechCulture
        {
            get
            {
                return speechCulture;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (speechCulture == value)
                    return;
                speechCulture = value;
                OnPropertyChanged("SpeechCulture");
                NeedsSave = true;
            }
#endif
        }

        public String CultureName
        {
            get
            {
                return cultureName;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (cultureName == value)
                    return;
                cultureName = value;
                OnPropertyChanged("CultureName");
                NeedsSave = true;
            }
#endif
        }

        public bool ForceStartupCultureName
        {
            get
            {
                return forceStartupCultureName;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (forceStartupCultureName == value)
                    return;
                forceStartupCultureName = value;
                OnPropertyChanged(nameof(ForceStartupCultureName));
                NeedsSave = true;
            }
#endif
        }

        public String ConverterName
        {
            get
            {
                return converterName;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (converterName == value)
                    return;
                converterName = value;
                OnPropertyChanged("ConverterName");
                NeedsSave = true;
            }
#endif
        }

        public List<String> DefaultSpeechCommands
        {
            get
            {
                return defaultSpeechCommands;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (defaultSpeechCommands == value)
                    return;
                defaultSpeechCommands = value;
                OnPropertyChanged("DefaultSpeechCommands");
                NeedsSave = true;
            }
#endif
        }

        public double Latitude
        {
            get
            {
                return latitude;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (latitude == value)
                    return;
                latitude = value;
                OnPropertyChanged("Latitude");
                NeedsSave = true;
            }
#endif
        }

        public double Longitude
        {
            get
            {
                return longitude;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (longitude == value)
                    return;
                longitude = value;
                OnPropertyChanged("Longitude");
                NeedsSave = true;
            }
#endif
        }

        public bool ChildVisible
        {
            get { return visible; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (visible == value)
                    return;
                visible = value;
                OnPropertyChanged("Visible");
                NeedsSave = true;
            }
#endif
        }

        public BingMapKind BingMapKind
        {
            get { return bingMapKind; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (bingMapKind == value)
                    return;
                bingMapKind = value;
                OnPropertyChanged("BingMapKind");
                NeedsSave = true;
            }
#endif
        }
        

        public DocumentManager.ComponentService.TileSize ChildTileSize
        {
            get { return tileSize; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (tileSize == value)
                    return;
                tileSize = value;
                OnPropertyChanged("TileSize");
                NeedsSave = true;
            }
#endif
        }

        public bool SpeechEnabled
        {
            get
            {
                return speechEnabled;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (speechEnabled == value)
                    return;
                speechEnabled = value;
                OnPropertyChanged("SpeechEnabled");
                NeedsSave = true;
            }
#endif
        }

        public double SpeechConfidenceLevel
        {
            get
            {
                return speechConfidenceLevel;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (speechConfidenceLevel == value)
                    return;
                speechConfidenceLevel = value;
                OnPropertyChanged("SpeechConfidenceLevel");
                NeedsSave = true;
            }
#endif
        }

        public Color IdentityColor
        {
            get
            {
                return identityColor;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (identityColor == value)
                    return;
                identityColor = value;
                OnPropertyChanged("IdentityColor");
                NeedsSave = true;
            }
#endif
        }

        public String Description
        {
            get
            {
                return description;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (description == value)
                    return;
                description = value;
                OnPropertyChanged("Description");
                NeedsSave = true;
            }
#endif
        }

        public bool HasGeoCoordinates
        {
            get
            {
                return !Double.IsNaN(Longitude) && !Double.IsNaN(Latitude) && Latitude >= -90 && Latitude <= 90 && Longitude >= -180 && Longitude <= 180;
            }
        }

        public int RemoveDisabledItemAfterSecs
        {
            get { return removeDisabledItemAfterSecs; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (removeDisabledItemAfterSecs == value)
                    return;
                removeDisabledItemAfterSecs = value;
                OnPropertyChanged("RemoveDisabledItemAfterSecs");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int MaxCleanCount
        {
            get { return maxCleanCount; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (maxCleanCount == value)
                    return;
                maxCleanCount = value;
                OnPropertyChanged("MaxCleanCount");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool UseAlwaysSecureConnections
        {
            get { return useAlwaysSecureConnections; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (useAlwaysSecureConnections == value)
                    return;
                useAlwaysSecureConnections = value;
                OnPropertyChanged("UseAlwaysSecureConnections");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int FastSamplingInterval
        {
            get
            {
                return fastSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (fastSamplingInterval == value)
                    return;
                fastSamplingInterval = value;
                OnPropertyChanged("FastSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int SlowSamplingInterval
        {
            get
            {
                return slowSamplingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (slowSamplingInterval == value)
                    return;
                slowSamplingInterval = value;
                OnPropertyChanged("SlowSamplingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }
#if !WINDOWS_UWP && !NET_STANDARD
        EventLogCommand eventLogCommand;
        public void AddLogEntity(string source, DateTime recordingTime, string message, System.Diagnostics.EventLogEntryType eventSeverity)
        {
            AddLogEntity(source, recordingTime, message, null, null, null, eventSeverity);
        }

        public void AddLogEntity(string source, DateTime recordingTime, string message, string details, string comment, string userName, System.Diagnostics.EventLogEntryType eventSeverity)
        {
            if (eventLogCommand != null)
                eventLogCommand.AddLogEntity(source ?? Title, recordingTime, message, details, comment, userName, eventSeverity);
        }
#endif
        public bool DisableWhenNotUsed
        {
            get { return disableWhenNotUsed; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (disableWhenNotUsed == value)
                    return;
                disableWhenNotUsed = value;
                OnPropertyChanged("DisableWhenNotUsed");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public int PublishingInterval
        {
            get
            {
                return publishingInterval;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (publishingInterval == value)
                    return;
                publishingInterval = value;
                OnPropertyChanged("PublishingInterval");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool ConnectItemsAtStartup
        {
            get { return connectItemsAtStartup; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (connectItemsAtStartup == value)
                    return;
                connectItemsAtStartup = value;
                OnPropertyChanged("ConnectItemsAtStartup");
                OnPropertyVisiblityChanged("ConnectItemsAtStartup");
                NeedsSave = true;

                UpdateSessionSettings();
            }
#endif
        }

        public bool AwaitConnectedItemsAtStartup
        {
            get { return awaitConnectedItemsAtStartup; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (awaitConnectedItemsAtStartup == value)
                    return;
                awaitConnectedItemsAtStartup = value;
                OnPropertyChanged("AwaitConnectedItemsBeforeStartup");
                NeedsSave = true;
            }
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, AppNameSettings> MapAppNameSettings
        {
            get
            {
                return mapAppNameSettings;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                mapAppNameSettings.Clear();
                foreach (var entry in value.Keys)
                {
                    if (value[entry].HasOverriddenString())
                        mapAppNameSettings.Add(value[entry].ToString(), value[entry]);
                    else
                        mapAppNameSettings.Add(entry, value[entry]);
                }

                OnPropertyChanged("MapAppNameSettings");
                NeedsSave = true;
            }
#endif
        }

        /*
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Dictionary<String, AppNameSettings> MapAppNameChildProjectSettings
        {
            get
            {
                return mapAppNameChildProjectSettings;
            }
            set
            {
                mapAppNameChildProjectSettings.Clear();
                foreach (var entry in value.Keys)
                {
                    if (value[entry].HasOverriddenString())
                        mapAppNameChildProjectSettings.Add(value[entry].ToString(), value[entry]);
                    else
                        mapAppNameChildProjectSettings.Add(entry, value[entry]);
                }

                OnPropertyChanged("MapAppNameChildProjectSettings");
                NeedsSave = true;
            }
        }
        */

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnableBackup
        {
            get { return enableBackup; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (enableBackup == value)
                    return;
                enableBackup = value;
                OnPropertyChanged("EnableBackup");
                NeedsSave = true;
            }
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public int MaxBackupCount
        {
            get { return maxBackupCount; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (maxBackupCount == value)
                    return;
                maxBackupCount = value;
                OnPropertyChanged("MaxBackupCount");
                NeedsSave = true;
            }
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool LayoutScreenOrder
        {
            get { return layoutScreenOrder; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (layoutScreenOrder == value)
                    return;
                layoutScreenOrder = value;
                OnPropertyChanged("LayoutScreenOrder");
                NeedsSave = true;
            }
#endif
        }

        
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool RestoreLastOpenScreenAndZoom
        {
            get { return restoreLastOpenScreenAndZoom; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (restoreLastOpenScreenAndZoom == value)
                    return;
                restoreLastOpenScreenAndZoom = value;
                OnPropertyChanged("RestoreLastOpenScreenAndZoom");
                NeedsSave = true;
            }
#endif
        }

        public Uri MainScreen
        {
            get { return mainScreen; }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (mainScreen == value)
                    return;
                mainScreen = value;
                OnPropertyChanged("MainScreen");
                NeedsSave = true;
            }
#endif
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool EnablePageChangeGesture
        {

            get => enablePageChangeGesture;

#if !WINDOWS_UWP && !NET_STANDARD
            set
            {

                if (enablePageChangeGesture != value)
                {
                    enablePageChangeGesture = value;
                    OnPropertyChanged(nameof(EnablePageChangeGesture));
                    NeedsSave = true;

                }

            }
#endif

        }

        public bool DisablePageTransitions
        {

            get => disablePageTransitions;

#if !WINDOWS_UWP && !NET_STANDARD
            set
            {

                if (disablePageTransitions != value)
                {
                    disablePageTransitions = value;
                    OnPropertyChanged(nameof(DisablePageTransitions));
                    NeedsSave = true;

                }

            }
#endif

        }

        #endregion

        #region IDocument Members

        public event EventHandler Disposing;
        virtual public void OnDisposing(Object sender)
        {
            EventHandler temp = Disposing;
            if (temp != null)
                temp(sender, EventArgs.Empty);

            OPCUAViewModel.OPCUAEntityReference.DisposingDocumentParent(this);
            if (ExpressionManager.ExpressionBucket.IsRunning(this))
                ExpressionManager.ExpressionBucket.GetInstance(this).Dispose();
        }

#if !NET_STANDARD
        UserControl activeView;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public UserControl ActiveView
        {
            get
            {
                return activeView;
            }
            internal set
            {
                activeView = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public UserControl View
        {
            get
            {
                return activeView;
            }
        }
#endif

        IDocument parent;
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IDocument Parent
        {
            get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IList<IDocument> Childs
        {
            get
            {
                var list = new List<IDocument>();
                foreach(var project in ListChildProjectPaths)
                {
                    var relative = project.GetPathString();
                    var match = String.Format("{0}/", Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    var abs = MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));

                    var doc = projectManagerComponent.GetDocument(abs) as IDocument;
                    if(doc is UFProjectDocument && doc != null)
                    {
                        if(doc.Parent == null)
                            (doc as UFProjectDocument).Parent = this;
                    }
                    if (doc != null)
                        list.Add(doc);
                }

                return list;
            }
        }

        public String ProjectType
        {
            get
            {
                return projectType;
            }
#if !WINDOWS_UWP && !NET_STANDARD
            set
            {
                if (projectType == value)
                    return;
                projectType = value;
                OnPropertyChanged("ProjectType");
                NeedsSave = true;
            }
#endif
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String Theme
        {
            get
            {
                //if (Parent != null)
                //    return Parent.Theme;
                return GetTheme().ToString();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri MakeAbosoluteUri(Uri relative)
        {
            return MakeAbosoluteUri(relative, checkIfRenamed: true);
        }

        internal IList<IDocument> GetAllChilds()
        {
            var list = new List<IDocument>();
            foreach (var project in ListChildProjectPaths)
            {
                var relative = project.GetPathString();
                Uri abs = project;
                if (!XpoHelpers.XpoHelper.IsDataSource(relative))
                {
                    var match = String.Format("{0}/", Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    abs = MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                }

                var doc = projectManagerComponent.GetDocument(abs) as IDocument;
                if (doc != null)
                    list.Add(doc);
            }

            return list;
        }

        public IDocument UpdateParentFromUri(Uri relativeResource)
        {
            foreach(var project in ListChildProjectPaths)
            {
                var controller = GetChildProjectData(project);
                var relative = project.GetPathString();
                var match = String.Format("{0}/", Title);
                if (relative.StartsWith(match))
                    relative = relative.Replace(match, "");
                else
                {
                    match = String.Format("{0}\\", Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                }
                var abs = MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));
                var doc = projectManagerComponent.GetDocument(abs) as UFProjectDocument;

                if (doc != null && projectManagerComponent != null && projectManagerComponent.UriRisolver != null)
                {
                    String type = projectManagerComponent.UriRisolver.GetUriType(relativeResource);
                    if (type != null && doc.mapTypeResources.ContainsKey(type))
                    {
                        if (relativeResource.GetPathString().StartsWith(doc.mapTypeResources[type].Path))
                            return doc;
                    }
                }
            }

            if (Parent != null)
                return Parent.UpdateParentFromUri(relativeResource);
            return this;
        }

        public Uri MakeRelativeUri(Uri absolute, IDocumentManager idoc)
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
                return new Uri(absolute.GetPathString(), UriKind.RelativeOrAbsolute);
#endif
            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle, bWatch: false))
            {
                return new Uri(folderWatcher.Path).MakeRelativeUri(absolute);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetSpecialFolder(SpecialFolders specialFolder)
        {
            //if (Parent != null)
            //    return Parent.GetSpecialFolder(specialFolder);

            string sourceDir = String.Empty;
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase == null)
                sourceDir = System.IO.Path.GetDirectoryName(ProjectPath);
#endif
            switch (specialFolder)
            {
#if !WINDOWS_UWP
                case SpecialFolders.Images: sourceDir = String.Format("{0}{2}{1}{2}", sourceDir, Properties.Settings.Default.ImageFolder, Path.DirectorySeparatorChar); break;
                case SpecialFolders.Documents: sourceDir = String.Format("{0}{2}{1}{2}", sourceDir, Properties.Settings.Default.DocumentFolder, Path.DirectorySeparatorChar); break;
                case SpecialFolders.RuntimeData: sourceDir = String.Format("{0}{2}{1}{2}", sourceDir, Properties.Settings.Default.RuntimeDataFolder, Path.DirectorySeparatorChar); break;
#else
                case SpecialFolders.Images: sourceDir = String.Format("{0}\\Images\\", sourceDir); break;
                case SpecialFolders.Documents: sourceDir = String.Format("{0}\\Documents\\", sourceDir); break;
                case SpecialFolders.RuntimeData: sourceDir = String.Format("{0}\\RuntimeData\\", sourceDir); break;
#endif
            }

            try
            {
#if !WINDOWS_UWP && !NET_STANDARD
                if (fileSystemProviderBase != null)
                {
                    fileSystemProviderBase.CreateFolder(null, sourceDir);
                }
                else
#endif
                {
                    Directory.CreateDirectory(sourceDir);
                }
            }
            catch
            {

            }

            return new Uri(sourceDir, UriKind.RelativeOrAbsolute);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri MakeRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            //if (Parent != null)
            //    return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            if (projectManagerComponent != null && projectManagerComponent.UriRisolver != null)
            {
                String type = projectManagerComponent.UriRisolver.GetUriType(absolute);
                if (!String.IsNullOrEmpty(type) && mapTypeResources.ContainsKey(type))
                {
                    var ret = new Uri(mapTypeResources[type].Path, UriKind.RelativeOrAbsolute);
                    if (ret.IsAbsoluteUri)
                        return ret.MakeRelativeUri(absolute);
                }
            }

            return absolute;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri MakeRelativeUri(Uri absolute, String path)
        {
            if (absolute == null)
                return null;

            //if (Parent != null)
            //    return Parent.MakeRelativeUri(absolute);
            if (!absolute.IsAbsoluteUri)
                return absolute;

            return new Uri(path).MakeRelativeUri(absolute);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FileSystemProviderBase fileSystemProviderBase
        {
            get
            {
                //if (Parent != null)
                //    return Parent.fileSystemProviderBase;
                return fileSystemProvider;
            }
        }
#endif
#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String rootBase
        {
            get
            {
                return ProjectFolder;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String rootBaseDB
        {
            get
            {
                return projectDataTag;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool Protected
        {
            get
            {
                return !temporaryDisableProtection && IsPasswordProtected();
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid Id
        {
            get
            {
                return projectId;
            }
        }

        public String Title
        {
            get
            { 
                if (Parent is UFProjectDocument)
                {
                    var docParent = Parent as UFProjectDocument;
                    var controller = docParent.GetChildProjectData(new Uri(ProjectPath, UriKind.RelativeOrAbsolute));
                    if (!String.IsNullOrWhiteSpace(controller.ProjectName))
                        return controller.ProjectName;
                }
#if !WINDOWS_UWP && !NET_STANDARD
                if (XpoHelpers.XpoHelper.IsDataSource(ProjectPath))
                    return XpoHelpers.XpoHelper.GetDataSourceTitle(ProjectPath);
#endif
                return Path.GetFileNameWithoutExtension(ProjectPath);
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public String FilePath
        {
            get
            {
                return ProjectPath;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsEmpty
        {
            get
            {
                return false;
            }
        }

#if !WINDOWS_UWP
        [Browsable(false)]
#endif
        public bool IsRoot
        {
            get
            {
                return true;
            }
        }

        public Object GetService(Type type)
        {
            if (Parent != null)
                return Parent.GetService(type);

            if (projectManagerComponent != null)
                return projectManagerComponent.GetComponentService(type);
            return null;
        }
#endregion

#region Validations
#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Error
        {
            get
            {
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Latitude")
            {
                if (!Double.IsNaN(Latitude) && (Latitude < -90 || Latitude > 90))
                    return Properties.Resources.LatitudeNoInValidRange;
            }
            else if (propertyName == "Longitude")
            {
                if (!Double.IsNaN(Latitude) && (Longitude < -180 || Longitude > 180))
                    return Properties.Resources.LatitudeNoInValidRange;
            }
            else if (propertyName == "EnableBackup")
            {
                if (fileSystemProviderBase != null && !(fileSystemProviderBase is DataSourceFileSystemProvider))
                {
                    if (EnableBackup)
                    {
                        //EnableBackup = false;
                        return Properties.Resources.BackupNotAvailableOnDB;
                    }
                }
            }

            return null;
        }

#region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "MaxBackupCount")
                {
                    return fileSystemProviderBase == null;
                }
                else if (propertyName == "ProjectFolder")
                {
                    return fileSystemProviderBase == null;
                }
                else if (propertyName == "AwaitConnectedItemsAtStartup")
                {
                    return ConnectItemsAtStartup;
                }
                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

#endregion
#endif
#endregion

        internal void Terminate()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (tts != null)
            {
                tts.Dispose();
                tts = null;
            }
            if (servers != null)
            {
                servers.Dispose();
                servers = null;
            }
            if (alarms != null)
            {
                alarms.Dispose();
                alarms = null;
            }
#endif
            RealTimeConnectionManagerViewModel.CleanDeadConnections(true);
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal void AddRenamedFolder(String scheme, Uri oldName, Uri newName)
        {
            if (mapTypeResources.ContainsKey(scheme))
            {
                if (mapRenamedFolders == null)
                    mapRenamedFolders = new Dictionary<String, String>();
                var relativeOldName = MakeRelativeUri(oldName, mapTypeResources[scheme].Path).GetPathString();
                var relativeNewName = MakeRelativeUri(newName, mapTypeResources[scheme].Path).GetPathString();
                var found = (from c in mapRenamedFolders where c.Value == relativeOldName select c).ToList();
                if (found.Count > 0)
                {
                    found.ForEach(key =>
                    {
                        mapRenamedFolders.Remove(key.Key);
                        if (key.Key != relativeNewName)
                            mapRenamedFolders.Add(key.Key, relativeNewName);


                    });
                }
                else
                {
                    if (mapRenamedFolders.ContainsKey(relativeOldName))
                        mapRenamedFolders.Remove(relativeOldName);
                    if (oldName != newName)
                        mapRenamedFolders.Add(relativeOldName, relativeNewName);
                }
                NeedsSave = true;
            }
        }
#endif
        Uri CheckRenamed(Uri relative)
        {
            var originalString = relative.GetPathString();
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
            {
                if (fileSystemProvider.Exists(new FileManagerFile(fileSystemProvider, originalString)))
                    return relative;
            }
            else
#endif
            {
                try
                {
                    if (File.Exists(originalString) || (!relative.IsAbsoluteUri && File.Exists(MakeAbosoluteUri(relative, checkIfRenamed: false).LocalPath)))
                        return relative;
                }
                catch { }
            }

            originalString = MakeRelativeUri(relative).GetPathString();
            if (mapRenamedResources != null && mapRenamedResources.ContainsKey(originalString))
            {
                originalString = mapRenamedResources[originalString];
                relative = new Uri(originalString, UriKind.RelativeOrAbsolute);
            }
            else if (mapRenamedFolders != null)
            {
                foreach (var folder in mapRenamedFolders.Keys)
                {
                    if (originalString.StartsWith(folder))
                    {
                        originalString = originalString.Replace(folder, mapRenamedFolders[folder]);
                        relative = new Uri(originalString, UriKind.RelativeOrAbsolute);
                    }
                }
            }
            if (!relative.IsAbsoluteUri)
                relative = MakeAbosoluteUri(relative, checkIfRenamed: false);

            return relative;
        }

        Uri MakeAbosoluteUri(Uri relative, bool checkIfRenamed)
        {
            if (relative == null)
                return null;
            Uri absolute = null;
#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
            {
                var path = relative.GetPathString().Replace('/', '\\');
                if (!path.StartsWith(projectDataTag))
                    path = String.Format("{0}\\{1}", projectDataTag, path);
                absolute = new Uri(path, UriKind.RelativeOrAbsolute);
                return checkIfRenamed ? CheckRenamed(absolute) : absolute;
            }
#endif
            //if (Parent != null)
            //    return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
            {
                return checkIfRenamed ? CheckRenamed(relative) : relative;
            }

            if (projectManagerComponent != null && projectManagerComponent.UriRisolver != null)
            {
                String type = projectManagerComponent.UriRisolver.GetUriType(relative);
                if (type != null && mapTypeResources.ContainsKey(type))
                {
                    absolute = new Uri(new Uri(mapTypeResources[type].Path), relative);
                    return checkIfRenamed ? CheckRenamed(absolute) : absolute;
                }
            }
            else if (checkIfRenamed)
                relative = CheckRenamed(relative);

            absolute = new Uri(new Uri(String.Format("{0}{1}", ProjectFolder, Path.DirectorySeparatorChar)), relative);
            return checkIfRenamed ? CheckRenamed(absolute) : absolute;
        }

        Uri GetControllerDataRelativeUri(Uri absolute)
        {
            if (absolute == null)
                return null;

            if (!absolute.IsAbsoluteUri)
                return absolute;

            if (projectManagerComponent != null && projectManagerComponent.UriRisolver != null)
            {
                String type = projectManagerComponent.UriRisolver.GetUriType(absolute);
                if (!String.IsNullOrEmpty(type) && mapTypeResources.ContainsKey(type))
                {
                    var ret = new Uri(mapTypeResources[type].Path, UriKind.RelativeOrAbsolute);
                    if (ret.IsAbsoluteUri)
                        return ret.MakeRelativeUri(absolute);
                }
                else
                {
                    try
                    {
                        var ret = new Uri(String.Format("{0}{1}", ProjectFolder, Path.DirectorySeparatorChar), UriKind.RelativeOrAbsolute);
                        if (ret.IsAbsoluteUri)
                            return ret.MakeRelativeUri(absolute);
                    }
                    catch
                    {

                    }
                }
            }

            return absolute;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        internal void ClearRenamedResourcesMap(List<string> typeLabelList)
        {
            if (mapRenamedResources == null)
                mapRenamedResources = new Dictionary<String, String>();
            if (typeLabelList == null)
                return;
            bool needToSave = false;
            typeLabelList.ForEach(t =>
            {
                var found = (from c in mapRenamedResources where c.Key.StartsWith($"{t}/") select c).ToList();
                if (found.Count > 0)
                {
                    found.ForEach(key =>
                    {
                        mapRenamedResources.Remove(key.Key);
                        needToSave = true;
                    });
                }
            });
            if (needToSave)
                SaveToFile();
        }

        internal void AddRenamedResource(Uri oldName, Uri newName)
        {
            if (mapRenamedResources == null)
                mapRenamedResources = new Dictionary<String, String>();
            var relativeOldName = MakeRelativeUri(oldName).GetPathString();
            var relativeNewName = MakeRelativeUri(newName).GetPathString();
            var found = (from c in mapRenamedResources where c.Value == relativeOldName select c).ToList();
            if (found.Count > 0)
            {
                found.ForEach(key =>
                {
                    mapRenamedResources.Remove(key.Key);
                    if (key.Key != relativeNewName)
                        mapRenamedResources.Add(key.Key, relativeNewName);
                });
            }
            else
            {
                if (mapRenamedResources.ContainsKey(relativeOldName))
                    mapRenamedResources.Remove(relativeOldName);
                if (oldName != newName)
                    mapRenamedResources.Add(relativeOldName, relativeNewName);
            }

            NeedsSave = true;
        }
#endif

#region Isolated Storage Persistence
        static IsolatedStorageFile GetStorage()
        {
            try
            {
                return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            }
            catch
            {
                try
                {
                    return IsolatedStorageFile.GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly, null, null);
                }
                catch (Exception ex)
                { }
            }

            return null;
        }

        String GetStorageFileName(String name)
        {
            var destFile = String.Format("{0}.{1}.{2}.dat",
                System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location), Title, name);
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                destFile = destFile.Replace(c, '_');
            return destFile;
        }

        void SaveStorageValue(String name, String value)
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(GetStorageFileName(name)))
                return;

            using (Stream stream = new IsolatedStorageFileStream(GetStorageFileName(name), FileMode.Create, isoStorage))
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    try
                    {
                        writer.WriteElementString(name, value);
                    }
                    catch (Exception ex)
                    {
                        writer.Close();
                    }
                }
            }
        }

        String LoadStorageValue(String name)
        {
            IsolatedStorageFile isoStorage = GetStorage();
            if (null == isoStorage || string.IsNullOrEmpty(GetStorageFileName(name)) ||
                isoStorage.GetFileNames(GetStorageFileName(name)).Length <= 0)
                return null;

            using (Stream stream = new IsolatedStorageFileStream(GetStorageFileName(name), FileMode.OpenOrCreate, isoStorage))
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Document,
                    CloseInput = true
                };

                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    try
                    {
                        return reader.ReadElementString(name);
                    }
                    catch (Exception ex)
                    {
                        reader.Close();
                    }
                }
            }

            return null;
        }
#endregion

#region IDisposable Members
        bool bDisposed;
        protected override void OnDispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;

            OnDisposing(this);

            base.OnDispose();

#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProvider != null && fileSystemProvider is IDisposable)
            {
                (fileSystemProvider as IDisposable).Dispose();
                fileSystemProvider = null;
            }
#endif
            foreach (var watcher in mapTypeResources.Values)
                watcher.Dispose();

#if !WINDOWS_UWP && !NET_STANDARD
            if (servers != null)
            {
                servers.Dispose();
                servers = null;
            }
            if (alarms != null)
            {
                alarms.Dispose();
                alarms = null;
            }
#endif
        }

#endregion

#region IScreenController

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetTitle()
        {
            return Title;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public StartType GetStartType()
        {
            return
#if !DEBUG && !WINDOWS_UWP && !NET_STANDARD
                !geolocal && StartType == DocumentManager.ComponentService.StartType.GeoPage ? 
                DocumentManager.ComponentService.StartType.TilePage : StartType;
#else
                StartType;
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TouchType GetTouchType()
        {
            // return TouchType;
            return TouchType.None;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ThemeType GetTheme()
        {
            return ThemeType;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsVisible()
        {
            return ChildVisible;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public BingMapKind GetBingMapKind()
        {
            return BingMapKind;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DocumentManager.ComponentService.TileSize TileSize()
        {
            return ChildTileSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetHasSpeechEnabled()
        {
            return speechEnabled;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetSpeechConfidenceLevel()
        {
            return speechConfidenceLevel;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetSpeechCulture()
        {
            return SpeechCulture;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetProjectCulture()
        {
            return CultureName;
        }       

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetProjectConverter()
        {
            return ConverterName;
        }

       [EditorBrowsable(EditorBrowsableState.Never)]
        public List<String> GetDefaultSpeechCommands()
        {
            if (DefaultSpeechCommands != null)
                return DefaultSpeechCommands.ToList();
            else
                return null;
        }

        const String clientEditor = "ClientEditor";
        internal IDocumentManager GetClientEditorDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(clientEditor))
                return null;

            return mapTypeDocumentManagers[clientEditor];
        }

        const String screenManager = "ScreenManager";
        internal IDocumentManager GetScreenDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(screenManager))
                return null;

            return mapTypeDocumentManagers[screenManager];
        }

        const String parameterManager = "ScreenParametersEditor";
        internal IDocumentManager GetParameterDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(parameterManager))
                return null;

            return mapTypeDocumentManagers[parameterManager];
        }

#if !WINDOWS_UWP && !NET_STANDARD
        const String scriptManager = "ScriptManager";
        internal IDocumentManager GetScriptDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(scriptManager))
                return null;

            return mapTypeDocumentManagers[scriptManager];
        }
#endif

#if !NET_STANDARD
        const String logicManager = "LogicManager";
        internal IDocumentManager GetLogicDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(logicManager))
                return null;

            return mapTypeDocumentManagers[logicManager];
        }

        const String recipeManager = "UFRecipeEditor";
        internal IDocumentManager GetRecipeDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(recipeManager))
                return null;

            return mapTypeDocumentManagers[recipeManager];
        }

        const String reportManager = "ReportManager";
        internal IDocumentManager GetReportDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(reportManager))
                return null;

            return mapTypeDocumentManagers[reportManager];
        }

        const String shortcutManager = "UFShortcutEditor";
        internal IDocumentManager GetShortcutDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(shortcutManager))
                return null;

            return mapTypeDocumentManagers[shortcutManager];
        }

        const String eventManager = "UFEventEditor";
        internal IDocumentManager GetEventDocumentManager()
        {
            if (!mapTypeDocumentManagers.ContainsKey(eventManager))
                return null;

            return mapTypeDocumentManagers[eventManager];
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<IScreenController> GetScreenControllers()
        {
            var ret = new List<IScreenController>();
            ListChildProjectPaths.ForEach(project =>
            {
                if (IsVisible(project))
                {
                    var relative = project.GetPathString();
                    var match = String.Format("{0}/", Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                    else
                    {
                        match = String.Format("{0}\\", Title);
                        if (relative.StartsWith(match))
                            relative = relative.Replace(match, "");
                    }
                    var abs = MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute));

                    var doc = projectManagerComponent.GetDocument(abs) as IScreenController;
                    if (doc != null)
                        ret.Add(doc);
                }
            });

            foreach(var folder in mapTypeResources[screenManager].ListFolders)
            {
                var uri = MakeRelativeUri(new Uri(folder.Path, UriKind.RelativeOrAbsolute), mapTypeResources[screenManager].Path);

                if (IsVisible(uri))
                {
                    ret.Add(folder);
                }
            }

            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<IScreenController> GetScreenControllers(IDocumentManager idoc)
        {
            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle))
            {
                var ret = new List<IScreenController>();
                ret.Add(folderWatcher);

                ListChildProjectPaths.ForEach(project =>
                {
                    if (IsVisible(project))
                    {
                        var doc = projectManagerComponent.GetDocument(project) as IScreenController;
                        if (doc != null)
                            ret.Add(doc);
                    }
                });
                /*
                foreach (var folder in folderWatcher.ListFolders)
                {
                    var uri = MakeRelativeUri(new Uri(folder.Path, UriKind.RelativeOrAbsolute), folderWatcher.Path);

                    if (IsVisible(uri))
                    {
                        ret.Add(folder);
                    }
                }
                */
                return ret;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static List<Uri> GetWholeResourceLists(ResourceFolderWatcher folderWatcher)
        {
            List<Uri> ret = new List<Uri>();
            if (folderWatcher.ListResources.Count > 0)
                ret.AddRange(folderWatcher.ListResources.ToList());
            if (folderWatcher.ListFolders.Count > 0)
            {
                foreach (var watcher in folderWatcher.ListFolders)
                    ret.AddRange(GetWholeResourceLists(watcher));
            }

            return ret;
        }

#if !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetWholeScreenLists()
        {
            if (!mapTypeResources.ContainsKey(screenManager))
                return new List<Uri>();
            return (from uri in GetWholeResourceLists(mapTypeResources[screenManager])// .AsParallel() 
                    where IsVisible(uri) && !IsInExclusedList(uri)
                    select uri).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetWholeReportLists()
        {
            if (!mapTypeResources.ContainsKey(reportManager))
                return new List<Uri>();
            return (from uri in GetWholeResourceLists(mapTypeResources[reportManager])// .AsParallel() 
                    where IsVisible(uri)
                    select uri).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetWholeRecipeLists()
        {
            if (!mapTypeResources.ContainsKey(recipeManager))
                return new List<Uri>();
            return (from uri in GetWholeResourceLists(mapTypeResources[recipeManager])// .AsParallel() 
                    where IsVisible(uri)
                    select uri).ToList();
        }
#endif

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetWholeDocumentLists(IDocumentManager idoc)
        {
            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle, bWatch: false))
            {
                return GetWholeResourceLists(folderWatcher);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetScreenLists()
        {
            return (from uri in mapTypeResources[screenManager].ListResources// .AsParallel() 
                    where IsVisible(uri) && !IsInExclusedList(uri)
                    select uri).ToList();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetScreenLists(IDocumentManager idoc)
        {
            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle, bWatch: false))
            {
                return (from uri in folderWatcher.ListResources// .AsParallel() 
                        where IsVisible(uri) && !IsInExclusedList(uri)
                        select uri).ToList();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetStartupScreen(ResourceFolderWatcher folderWatcher, String main = "Main", bool bReturnDefault = true, 
            IDocumentManager idoc = null)
        {
            if (main == "Main")
            {
                try
                {
                    Utility.CommandArgs commandArgs = Utility.CommandLine.Parse(Environment.GetCommandLineArgs());
                    if (commandArgs.ArgPairs.ContainsKey("startscreen"))
                        return MakeAbosoluteUri(new Uri(commandArgs.ArgPairs["startscreen"], UriKind.RelativeOrAbsolute));
                }
                catch (Exception ex)
                {
                    log.Error(Properties.Resources.ErrorReadingCommandLineStartScren, ex);
                }
            }

            if (idoc == null)
            {
                if (main == "Main" && MainScreen != null && !String.IsNullOrEmpty(MainScreen.GetPathString()))
                {
                    var absMainUri = new Uri(MakeAbosoluteUri(MainScreen).GetPathString(), UriKind.RelativeOrAbsolute);
                    main = MakeRelativeUri(absMainUri).GetPathString();
                }

                var listscreens = GetWholeResourceLists(folderWatcher);
                var ret = from uri in listscreens/*.AsParallel()*/
                          where System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()) == main ||
                          MakeRelativeUri(uri).GetPathString() == main
                          select uri;
                if (ret.Count() == 0)
                {
                    if (bReturnDefault && listscreens.Count > 0)
                        return MakeAbosoluteUri(listscreens[0]);
                    return null;
                    // throw new ArgumentException("Project does NOT contain any screen !");
                }
                return MakeAbosoluteUri(ret.FirstOrDefault<Uri>());
            }
            else
            {
                if (main == "Main" && MainScreen != null && !String.IsNullOrEmpty(MainScreen.GetPathString()))
                {
                    Uri absMainUri;
                    var absMain = MakeAbosoluteUri(MainScreen, idoc).GetPathString();
                    if (!Uri.TryCreate(absMain, UriKind.Absolute, out absMainUri))
                        absMainUri = new Uri(absMain, UriKind.RelativeOrAbsolute);
                    main = CheckRenamed(MakeRelativeUri(absMainUri, idoc)).GetPathString();
                }

                var listscreens = GetWholeResourceLists(folderWatcher);
                var ret = from uri in listscreens/*.AsParallel()*/
                          where System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()) == main ||
                          MakeRelativeUri(uri, idoc).GetPathString() == main ||
                          System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()) == System.IO.Path.GetFileNameWithoutExtension(main)
                          select uri;
                if (ret.Count() == 0)
                {
                    if (bReturnDefault && listscreens.Count > 0)
                        return MakeAbosoluteUri(listscreens[0]);
                    return null;
                    // throw new ArgumentException("Project does NOT contain any screen !");
                }
                return MakeAbosoluteUri(ret.FirstOrDefault<Uri>(), idoc);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetStartupScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager]);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetStartupScreen(IDocumentManager idoc, string main = "Main", bool bReturnDefault = true)
        {
            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle, bWatch: false))
            {
                return GetStartupScreen(folderWatcher, main:main, bReturnDefault:bReturnDefault);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri MakeAbosoluteUri(Uri relative, IDocumentManager idoc)
        {
            if (relative == null)
                return null;

#if !WINDOWS_UWP && !NET_STANDARD
            if (fileSystemProviderBase != null)
                return CheckRenamed(new Uri(relative.GetPathString(), UriKind.RelativeOrAbsolute));
#endif
            //if (Parent != null)
            //    return Parent.MakeAbosoluteUri(relative);
            if (relative.IsAbsoluteUri)
                return CheckRenamed(relative);

            var folderName = String.Format("{0}{2}{1}", ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
            using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                fileSystemProviderBase, 
#endif
                idoc.TypeTitle, bWatch: false))
            {
                return CheckRenamed(new Uri(new Uri(folderWatcher.Path), relative));
            }
        }

        public IDocument UpdateParentFromUri(Uri relativeResource, IDocumentManager idoc)
        {
            foreach (var project in ListChildProjectPaths)
            {
                var controller = GetChildProjectData(project);
                var relative = project.GetPathString();
                var match = String.Format("{0}/", Title);
                if (relative.StartsWith(match))
                    relative = relative.Replace(match, "");
                else
                {
                    match = String.Format("{0}\\", Title);
                    if (relative.StartsWith(match))
                        relative = relative.Replace(match, "");
                }
                var abs = MakeAbosoluteUri(new Uri(relative, UriKind.RelativeOrAbsolute), idoc);
                var doc = projectManagerComponent.GetDocument(abs) as UFProjectDocument;

                if (doc != null)
                {
                    var folderName = String.Format("{0}{2}{1}", doc.ProjectFolder, idoc.TypeLabel, Path.DirectorySeparatorChar);
                    using (var folderWatcher = new ResourceFolderWatcher(this, folderName, idoc.FileType, idoc.TypeScheme, true,
#if !WINDOWS_UWP && !NET_STANDARD
                    fileSystemProviderBase,
#endif
                    idoc.TypeTitle, bWatch: false))
                    {
                        if (relativeResource.GetPathString().StartsWith(folderWatcher.Path))
                            return doc;
                    }

                    var found = doc.UpdateParentFromUri(relativeResource, idoc);
                    if (found != doc)
                        return found;
                }
            }

            if (Parent != null)
                return Parent.UpdateParentFromUri(relativeResource);
            return this;
        }

#if !WINDOWS_UWP && !NET_STANDARD
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri ExportRelativeUriToTemporaryFile(Uri relative, String extension = null)
        {
            if (relative == null || fileSystemProviderBase == null)
                return null;

            System.Diagnostics.Debug.Assert(!relative.IsAbsoluteUri);

            try
            {
                var fileManagerFileSettings = new FileManagerFile(fileSystemProvider, relative.GetPathString());
                var data = fileSystemProviderBase.ReadFile(fileManagerFileSettings);
                var fileDest = Path.GetTempFileName();
                if (extension != null)
                    fileDest = System.IO.Path.ChangeExtension(fileDest, extension);
                System.IO.File.WriteAllBytes(fileDest, data);
                return new Uri(fileDest, UriKind.RelativeOrAbsolute);
            }
            catch
            {

            }

            return null;
        }
#endif
        static readonly String _Layout_Top = "_Layout_Top";
        static readonly String _Layout_Left = "_Layout_Left";
        static readonly String _Layout_Right = "_Layout_Right";
        static readonly String _Layout_Bottom = "_Layout_Bottom";
        static readonly String _AppBar_Top = "_AppBar_Top";
        static readonly String _AppBar_Bottom = "_AppBar_Bottom";
        static readonly String _AppBar_Left = "_AppBar_Left";
        static readonly String _AppBar_Right = "_AppBar_Right";
        static readonly String _Gadget = "_Gadget_";
        static readonly String _AutoLoad = "_AutoLoad_";

        static readonly String _Login_User = "_Login_User";
        static readonly String _Verify_Password = "_Verify_Password";
        
        static readonly String _Numeric_Pad = "_Numeric_Pad";
        static readonly String _Alphanumeric_Pad = "_Alphanumeric_Pad";
        static readonly String _Numeric_PasswordPad = "_Numeric_PasswordPad";
        static readonly String _Alphanumeric_PasswordPad = "_Alphanumeric_PasswordPad";

        static readonly List<String> excludeNameList = new List<String> { _Layout_Top, _Layout_Left, _Layout_Right, 
                                                                           _Layout_Bottom, _AppBar_Top, _AppBar_Bottom, _Login_User,
                                                                           _Numeric_Pad, _Alphanumeric_Pad, _Numeric_PasswordPad,
                                                                           _Alphanumeric_PasswordPad
                                                                        };

        bool IsInExclusedList(Uri uri)
        {
            var path = System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString());
            return excludeNameList.Contains(path) || path.StartsWith(_Gadget) || path.StartsWith(_AutoLoad);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetGadgetScreens()
        {
            var listscreens = GetWholeResourceLists(mapTypeResources[screenManager]);
            var ret = (from uri in listscreens/*.AsParallel()*/
                      where System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()).StartsWith(_Gadget)
                      select uri).ToList();
            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<Uri> GetAutoLoadScreens()
        {
#if !NET_STANDARD
            var listscreens = GetWholeResourceLists(mapTypeResources[screenManager]);
            var autoloadScreenList = (from c in AutoloadScreenList select c.Uri.GetPathString()).ToList();
            var ret = (from uri in listscreens/*.AsParallel()*/
                       where System.IO.Path.GetFileNameWithoutExtension(uri.GetPathString()).StartsWith(_AutoLoad)
                       || autoloadScreenList.Contains(MakeRelativeUri(uri).GetPathString())
                       select uri).ToList();

            return ret;
#else
            return new List<Uri>();
#endif
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetLoginScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Login_User, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetVerifyPasswordScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Verify_Password, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetNumericPadScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Numeric_Pad, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAlphanumericPadScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Alphanumeric_Pad, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetNumericPasswordPadScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Numeric_PasswordPad, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAlphanumericPasswordPadScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Alphanumeric_PasswordPad, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarTopScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _AppBar_Top, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarTopScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _AppBar_Top, false);
        }

        public Uri GetAppBarLeftScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _AppBar_Left, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarLeftScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _AppBar_Left, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarBottomScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _AppBar_Bottom, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarBottomScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _AppBar_Bottom, false);
        }

        public Uri GetAppBarRightScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _AppBar_Right, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetAppBarRightScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _AppBar_Right, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetTopScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Layout_Top, false); 
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetTopScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _Layout_Top, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetLeftScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Layout_Left, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetLeftScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _Layout_Left, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetRightScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Layout_Right, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetRightScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _Layout_Right, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetBottomScreen()
        {
            return GetStartupScreen(mapTypeResources[screenManager], _Layout_Bottom, false);
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetLayoutScreenOrder()
        {
            return LayoutScreenOrder;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Uri GetBottomScreen(IDocumentManager manager)
        {
            return GetStartupScreen(manager, _Layout_Bottom, false);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetHasGeoCoordinates()
        {
            return HasGeoCoordinates;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetLatitude()
        {
            return Latitude;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetLongitude()
        {
            return Longitude;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetMapMinZoomLevelVisibility()
        {
            return mapZoomLevelVisibility;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetMapMaxZoomLevelVisibility()
        {
            return mapMaxZoomLevelVisibility;
        }

        public String GetUsersVisibility()
        {
            return String.Empty;
        }

        public String GetRolesVisibility()
        {
            return String.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color GetIdentityColor()
        {
#if !WINDOWS_UWP && !NET_STANDARD
            if (identityColor == Colors.Transparent)
            {
                var random = new Random();
                identityColor = Color.FromRgb((byte)random.Next(255),
                                                (byte)random.Next(255),
                                                (byte)random.Next(255));
            }
#endif
            return IdentityColor;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetDescription()
        {
            return Description;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetIconSource()
        {
            return System.IO.Path.ChangeExtension(ProjectPath, "png");
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color GetIdentityColor(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.Color == Colors.Transparent ? GetIdentityColor() : data.Color;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsVisible(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.Visible;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TileSize TileSize(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.TileSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetMapMinZoomLevelVisibility(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.MapMinZoomLevelVisibility;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetMapMaxZoomLevelVisibility(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.MapMaxZoomLevelVisibility;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetUsersVisibility(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.UsersVisibility;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetRolesVisibility(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.RolesVisibility;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public String GetDescription(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.Description;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetHasGeoCoordinates(Uri uri)
        {
            var data = GetControllerData(uri);
            return !Double.IsNaN(data.Longitude) && !Double.IsNaN(data.Latitude) && data.Latitude >= -90 && data.Latitude <= 90 && data.Longitude >= -180 && data.Longitude <= 180;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetLatitude(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.Latitude;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetLongitude(Uri uri)
        {
            var data = GetControllerData(uri);
            return data.Longitude;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetLatitudeTag(Uri uri)
        {
            var data = GetControllerData(uri);
            if (data.LatitudeTag == null
#if !WINDOWS_UWP && !NET_STANDARD
                || !data.LatitudeTag.IsValid
#endif
                )
                return null;
            return data.LatitudeTag.ToXml();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetLongitudeTag(Uri uri)
        {
            var data = GetControllerData(uri);
            if (data.LongitudeTag == null
#if !WINDOWS_UWP && !NET_STANDARD
                || !data.LongitudeTag.IsValid
#endif
                )
                return null;
            return data.LongitudeTag.ToXml();
        }                
        #endregion

        #region Script Friendly Methods

#if !WINDOWS_UWP && !NET_STANDARD
        Screens screens;
        [Browsable(false)]
        public Screens Screens
        {
            get
            {
                lock (lockObject)
                {
                    if (screens == null)
                    {
                        var screenManager = GetScreenDocumentManager();
                        if (screenManager == null)
                            return null;

                        screens = new Screens(this, screenManager, GetParameterDocumentManager());
                    }
                }
                return screens;
            }
        }

        Scripts scripts;
        [Browsable(false)]
        public Scripts Scripts
        {
            get
            {
                lock (lockObject)
                {
                    if (scripts == null)
                    {
                        var scriptManager = projectManagerComponent.ScriptManager as IDocumentManager;
                        if (scriptManager == null)
                            return null;

                        scripts = new Scripts(this, scriptManager);
                    }
                }
                return scripts;
            }
        }

        Strings strings;
        [Browsable(false)]
        public Strings Strings
        {
            get
            {
                lock (lockObject)
                {
                    if (strings == null)
                    {
                        var stringManager = projectManagerComponent.StringEditor;
                        if (stringManager == null)
                            return null;

                        strings = new Strings(this, stringManager);
                    }
                }
                return strings;
            }
        }

        Servers servers;
        [Browsable(false)]
        public Servers Servers
        {
            get
            {
                lock (lockObject)
                {
                    if (servers == null)
                    {
                        servers = new Servers(this);
                    }
                }
                return servers;
            }
        }

        TTS tts;
        [Browsable(false)]
        public TTS TTS
        {
            get
            {
                lock (lockObject)
                {
                    if (tts == null)
                    {
                        tts = new TTS(this);
                    }
                }
                return tts;
            }
        }

        Alarms alarms;
        [Browsable(false)]
        public Alarms Alarms
        {
            get
            {
                lock (lockObject)
                {
                    if (alarms == null)
                    {
                        var ufuaEditor = projectManagerComponent.UFUAEditor;
                        var authenticationCredentialsProvider = projectManagerComponent.AuthenticationCredentialsProvider;
                        if (ufuaEditor == null)
                            return null;

                        alarms = new Alarms(this, ufuaEditor, authenticationCredentialsProvider);
                    }
                }
                return alarms;
            }
        }
#endif
#endregion

#region EnityReference

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            ImageSource CollapsedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Media.
#endif
            ImageSource ExpandedImageSource
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public
#if !WINDOWS_UWP && !NET_STANDARD
            System.Windows.Controls.
#endif
            ContextMenu contextMenu
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object Tooltip
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object ContainedObject
        {
            get { return null; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object EntityParent
        {
            get { return Parent; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TypeDefinitionString
        {
            get { return null; }
        }

#endregion
    }
}
