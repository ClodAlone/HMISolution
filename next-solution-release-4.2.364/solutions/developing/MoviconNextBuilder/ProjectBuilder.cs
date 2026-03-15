using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UFInterfaces;
using UFInterfaces.CoreHostComponents;
using DocumentManager.ComponentService;
using Utilities;
#if !NET_STANDARD
using UFProjectManager.Document;
using UFUserEditor.ComponentService;
using StringManager.ComponentService;
using ScriptManager.ComponentService;
using TempVariablesManager;
using Toolbox.ComponentService;
using VFS;
#endif

namespace MoviconNextBuilder
{
    public class ProjectBuilder: IDisposable
    {
        #region Ctor
        public ProjectBuilder()
        { }
        private ProjectBuilder(Uri project)
        {
            _Project = project;
            projectDocument = UFProjectManager.UFProjectDocument.FromFile(_Project.OriginalString, UFProjectManagerComponent);
        }

        ~ProjectBuilder()
        { }
        #endregion Ctor

        #region data
        UriResolver.ComponentService.UriResolverComponent uriRisolver = new UriResolver.ComponentService.UriResolverComponent();
#if !NET_STANDARD
        ToolboxComponent toolboxcomponent = new ToolboxComponent();
        ScriptManagerComponent scriptManagerComponent = new ScriptManagerComponent();
        TempVariables tempVariableContainer;

        bool bNeedToDisposeTempVariables;
        //bool bNeedToDisposeGPIOVariables;
        bool bNeedToDisposeGPIOVariables;
        bool bNeedToDisposeSysVariables;
#endif
        #endregion data

        #region Methods
        /// <summary>
        /// load project referred by the argument
        /// </summary>
        /// <param name="project"></param>
        /// <returns>true if the project document has been loaded with success</returns>
        public bool Init(Uri project, string password = null)
        {
            CleanProject();
#if !NET_STANDARD
            SetPropertyHelper();
#endif
            _Project = project;

            componentHost.Components.Add(UFProjectManagerComponent);
            componentHost.Components.Add(uriRisolver);
            if (uriRisolver is IUFInterfaceBase)
                (uriRisolver as IUFInterfaceBase).Initialize();
            componentHost.Components.Add(UFUAEditorComponent);
#if !NET_STANDARD
            componentHost.Components.Add(toolboxcomponent);
            componentHost.Components.Add(scriptManagerComponent);

            tempVariableContainer = OPCUAViewModel.OPCUAEntityReference.GetDataSinkInterface("TemporaryVariables") as TempVariables;
            if (tempVariableContainer == null)
            {
                bNeedToDisposeTempVariables = true;
                tempVariableContainer = new TempVariables();
                tempVariableContainer.Initialize();
            }
#endif
            projectDocument = UFProjectManager.UFProjectDocument.FromFile(_Project.GetPathString()/*OriginalString*/, UFProjectManagerComponent);
            if(projectDocument.Protected && (password == null || !projectDocument.CheckPassword(password)))
            {
                projectDocument = null;
            }
            return (projectDocument != null);
        }

#if !NET_STANDARD
        private static void SetPropertyHelper()
        {
            var commonFolder = String.Format("{0}\\{1}\\{2}",
                             Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                             Properties.Settings.Default.CompanyName,
                             Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("CommonFolder", commonFolder);

            var userFolder = String.Format("{0}\\{1}\\{2}",
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            Properties.Settings.Default.CompanyName,
                            Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("UserFolder", userFolder);

            var projectFolder = String.Format("{0}\\{1}\\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                Properties.Settings.Default.CompanyName,
                Properties.Settings.Default.CommonApplicationFolder);
            ApplicationPropertiesHelper.SetProperty("ProjectFolder", projectFolder);
        }
#endif

        /// <summary>
        /// saves all the resources that have been modified in the current working session
        /// </summary>
        public void Save(
#if !NET_STANDARD            
            string pwd = null
#endif
            )
        {
            if(IODataServer != null)
                IODataServer.Save();
#if !NET_STANDARD
            if (ScreenContainer != null)
                ScreenContainer.Save();
            if (StringsResource != null)
                StringsResource.Save();
            if (ParametersContainer != null)
                ParametersContainer.Save();
            if (UsersManagerResource != null)
                UsersManagerResource.Save();
            if (AlarmDispatcher != null)
                AlarmDispatcher.Save();
            if (EventManager != null)
                EventManager.Save();
            if (MenuContainer != null)
                MenuContainer.Save();
            if (ShortcutContainer != null)
                ShortcutContainer.Save();
            if (RecipeContainer != null)
                RecipeContainer.Save();
            if (ReportContainer != null)
                ReportContainer.Save();

            if (projectDocument != null )
            {
                if (pwd != null)
                    projectDocument.AddPassword(pwd);

                if (projectDocument.NeedsSave)
                    projectDocument.SaveToFile();
            }
#endif
        }

#if !NET_STANDARD
        /// <summary>
        /// Clear the list of Startup scripts
        /// </summary>
        /// <returns>Return false if the project is not initialized.</returns>
        public bool ClearStartupScriptList()
        {
            if (projectDocument != null)
            {
                projectDocument.ClearStartupScripts();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Add a script to the list of Startup script of the project
        /// </summary>
        /// <param name="startupscript">System.Uri pointing to the script.</param>
        /// <param name="executeAsService">bool, indicate if the script has to be executed as a service (default is false)</param>
        /// <param name="exeMode">Execution mode, can be one of the folllowing: Normal, Synchro, Shared, Stop. (Default is Normal)</param>
        /// <returns></returns>
        public bool AddStartupScript(Uri startupscript, bool executeAsService = false, ExecutionMode exeMode = ExecutionMode.Normal)
        {
            var sm = projectDocument.GetService(typeof(IScriptManager)) as IDocumentManager;
            if (sm == null || !startupscript.GetPathString().EndsWith(sm.FileType))
                return false;

            if(projectDocument != null)
            {
                var reluri = projectDocument.MakeRelativeUri(startupscript);
                if(reluri != null)
                {
                    var lst = (from s in projectDocument.ListStartupScripts where s.Uri == reluri select s).ToList();
                    if (lst.Count > 0)
                        return false;
                    projectDocument.AddStartupScript(new StartupScript() { Uri = reluri, ExecuteAsService = executeAsService, ExecutionMode = exeMode });
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Return an Uri for a Script
        /// </summary>
        /// <param name="name"></param>
        /// <param name="subfolder"></param>
        /// <param name="bRelative"></param>
        /// <returns></returns>
        public Uri GetScriptUri(string name, string subfolder = null, bool bRelative = false, bool bCheckExistence = false)
        {
            var path = projectDocument.GetResourcePath("ScriptManager");

            if(subfolder != null)
            {
                subfolder.Trim('\\');
                name = string.Format("{0}\\{1}", subfolder, name);
            }
            Uri uri = ProjectBuilder.GetUriFromName(path, name, scriptManagerComponent, projectDocument, bCheckExistence);
            if (bRelative && uri != null)
                return projectDocument.MakeRelativeUri(uri);
            return uri;
        }


        /// <summary>
        /// Return a list with the Uri of the Startup scripts of the project
        /// </summary>
        /// <returns>Return a list of Uri or null, if the Startup script list is empty.</returns>
        public List<Uri> GetStartupScriptUriList()
        {
            if(projectDocument != null && projectDocument.ListStartupScripts != null && projectDocument.ListStartupScripts.Count > 0)
            {
                List<Uri> list = new List<Uri>();
                foreach(var s in projectDocument.ListStartupScripts)
                {
                    list.Add(s.Uri);
                }
                return list;
            }
            return null;
        }

        /// <summary>
        /// Return a list with the Startup scripts of the project
        /// </summary>
        /// <returns>Return a list of StartupScript or null, if the Startup script list is empty.</returns>
        public List<StartupScript> GetStartupScriptList()
        {
            if (projectDocument != null && projectDocument.ListStartupScripts != null && projectDocument.ListStartupScripts.Count > 0)
                return projectDocument.ListStartupScripts;
            return null;
        }

        /// <summary>
        /// Delete a script in the list of Startup script of the project
        /// </summary>
        /// <param name="startupscript">System.Uri pointing to the script.</param>
        /// <param name="executeAsService">bool, indicate if the script has to be executed as a service (default is false)</param>
        /// <param name="exeMode">Execution mode, can be one of the folllowing: Normal, Synchro, Shared, Stop. (Default is Normal)</param>
        /// <returns>Return true if the Startup Script has been deleted, false if the Startup script list is empty, the Startup script to delete was not found or uri is not correct.</returns>
        public bool DeleteStartupScript(Uri startupscript, bool executeAsService, ExecutionMode exeMode)
        {
            var sm = projectDocument.GetService(typeof(IScriptManager)) as IDocumentManager;
            if (sm == null || !startupscript.GetPathString().EndsWith(sm.FileType))
                return false;
            if (projectDocument != null && projectDocument.ListStartupScripts != null && projectDocument.ListStartupScripts.Count > 0)
            {
                var reluri = projectDocument.MakeRelativeUri(startupscript);
                if (reluri != null)
                {
                    List<StartupScript> list = new List<StartupScript>();
                    list.AddRange(projectDocument.ListStartupScripts);
                    var todel = (from s in list where s.Uri == startupscript && s.ExecuteAsService == executeAsService && s.ExecutionMode == exeMode select s).ToList();
                    if(todel.Count > 0)
                    {
                        foreach (var d in todel)
                        {
                            projectDocument.RemoveStartupScript(d);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Delete a script in the list of Startup script of the project
        /// </summary>
        /// <param name="startupscript">System.Uri pointing to the script.</param>
        /// <returns>Return true if the Startup Script has been deleted, false if the Startup script list is empty, the Startup script to delete was not found or uri is not correct.</returns>
        public bool DeleteStartupScript(Uri startupscript)
        {
            var sm = projectDocument.GetService(typeof(IScriptManager)) as IDocumentManager;
            if (sm == null || !startupscript.GetPathString().EndsWith(sm.FileType))
                return false;
            if (projectDocument != null && projectDocument.ListStartupScripts != null && projectDocument.ListStartupScripts.Count > 0)
            {
                var reluri = projectDocument.MakeRelativeUri(startupscript);
                if (reluri != null)
                {
                    var todel = (from s in projectDocument.ListStartupScripts where s.Uri == startupscript select s).ToList();
                    if (todel.Count > 0)
                    {
                        foreach (var d in todel)
                        {
                            projectDocument.RemoveStartupScript(d);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public void AddChildProject(Uri child)
        {
            if(projectDocument != null)
                projectDocument.AddChildProject(child);
        }

        public void RemoveChildProject(Uri child)
        {
            if (projectDocument != null)
                projectDocument.RemoveChildProject(child);
        }
        public List<Uri> GetChildProjectList()
        {
            if (projectDocument != null)
                return (from c in projectDocument.ListChildProjectPaths.AsParallel() select c).ToList();
            return null;
        }
        public CommandManager.CommandManager CreateCommand(string type)
        {
            return CommandManager.CommandManager.CreateFrom(type);
        }
        public List<string> GetCommandTypesList()
        {
            return (from c in CommandManager.CommandManager.LoadCommandTypes() orderby c select c).ToList();
        }
#endif

        void CleanProject()
        {
#if !NET_STANDARD
            if (_ReportContainer != null)
            {
                _ReportContainer.Dispose();
                _ReportContainer = null;
            }

            if (_RecipeContainer != null)
            {
                _RecipeContainer.Dispose();
                _RecipeContainer = null;
            }
                
            if(_ShortcutContainer != null)
            {
                _ShortcutContainer.Dispose();
                _ShortcutContainer = null;
            }
            
            if(_MenuContainer != null)
            {
                _MenuContainer.Dispose();
                _MenuContainer = null;
            }
            
            if(_ParametersContainer != null)
            {
                _ParametersContainer.Dispose();
                _ParametersContainer = null;
            }
            
            if(_StringsResource != null)
            {
                _StringsResource.Dispose();
                _StringsResource = null;
            }
            
            if(_UsersManager != null)
            {
                _UsersManager.Dispose();
                _UsersManager = null;
            }
            
            if(_ScreenContainer != null)
            {
                _ScreenContainer.Dispose();
                _ScreenContainer = null;
            }
            
            if(_EventManager != null)
            {
                _EventManager.Dispose();
                _EventManager = null;
            }

            if (_AlarmDispatcher != null)
            {
                _AlarmDispatcher.Dispose();
                _AlarmDispatcher = null;
            }
#endif                       
            if (_IODataServer != null)
            {
                _IODataServer.Dispose();
                _IODataServer = null;
            }
    }
        #endregion

        #region Static Methods
        internal static Uri GetUriFromName(Uri relative, string name, IDocumentManager manager, IDocument parent, bool bCheckExists = false)
        {
            int i = 0;
            Uri url;

            var path = String.Format("{0}{1}{2}", relative.OriginalString, name, manager.FileType);
#if !NET_STANDARD
            if (parent != null && parent.fileSystemProviderBase != null)
            {
                if (bCheckExists && !parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)))
                    return null;
                else if (!bCheckExists)
                {
                    do
                    {
                        path = String.Format("{0}{1}{2}", relative.OriginalString, name, manager.FileType);
                        name = String.Format("{0}{1}", name, ++i);
                    } while (parent.fileSystemProviderBase.Exists(new FileManagerFile(parent.fileSystemProviderBase, path)));
                }

                url = new Uri(path, UriKind.RelativeOrAbsolute);
                return url;
            }
#endif

            if (relative.IsAbsoluteUri && Path.HasExtension(relative.OriginalString))
            {
                var ret = Path.ChangeExtension(relative.OriginalString, manager.FileType);
                if (bCheckExists && !File.Exists(ret))
                    return null;
                var uri = new Uri(ret, UriKind.RelativeOrAbsolute);
                return uri;
            }

            if (bCheckExists && !File.Exists(path))
                return null;
            else if (!bCheckExists)
            {
                do
                {
                    path = String.Format("{0}{1}{2}", relative.OriginalString, name, manager.FileType);
                    name = String.Format("{0}{1}", name, ++i);
                } while (File.Exists(path));
            }

            if (path.StartsWith("\\"))
                url = new Uri(path);
            else
                url = new Uri(String.Format("{0}://{1}", relative.Scheme, path), UriKind.RelativeOrAbsolute);

            return url;
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if(uriRisolver != null)
            {
                uriRisolver.Dispose();
                uriRisolver = null;
            }

#if !NET_STANDARD
            if(toolboxcomponent != null)
            {
                toolboxcomponent.Dispose();
                toolboxcomponent = null;
            }

            if(scriptManagerComponent != null)
            {
                scriptManagerComponent.Dispose();
                scriptManagerComponent = null;
            }

            if (bNeedToDisposeTempVariables)
            {
                tempVariableContainer.Dispose();
                tempVariableContainer = null;
            }
#endif

            CleanProject();

            if(projectDocument != null)
            {
                projectDocument.Dispose();
                projectDocument = null;
            }
            GC.SuppressFinalize(this);//
        }
        #endregion IDisposable Members

        /*static*/ readonly ComponentHost componentHost = new ComponentHost();

        #region Properties
        static private UFProjectManager.UFProjectDocument projectDocument { get; set; }

        private Uri _Project;
        /// <summary>
        /// Uri contatining project location
        /// </summary>
        public Uri Project
        {
            get { return _Project; }
            set { _Project = value; }
        }

        private bool _Modified;
        /// <summary>
        /// true if the project has been modified in one or more part/resouces
        /// </summary>
        public bool Modified
        {
            get { return _Modified; }
            set { _Modified = value; }
        }

        
        static UFProjectManager.ComponentService.UFProjectManagerComponent ufProjectManagerComponent = new UFProjectManager.ComponentService.UFProjectManagerComponent();
        static public UFProjectManager.ComponentService.UFProjectManagerComponent UFProjectManagerComponent
        {
            get
            {
                return ufProjectManagerComponent;
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

#if !NET_STANDARD
        static MSEditor.ComponentService.SchedulerEditorManagerComponent schEditorComponent = new MSEditor.ComponentService.SchedulerEditorManagerComponent();
        static public MSEditor.ComponentService.SchedulerEditorManagerComponent SCHEditorComponent
        {
            get
            {
                return schEditorComponent;
            }
        }
        static ADEditor.ComponentService.ADEditorManagerComponent adEditorComponent = new ADEditor.ComponentService.ADEditorManagerComponent();
        static public ADEditor.ComponentService.ADEditorManagerComponent ADEditorComponent
        {
            get
            {
                return adEditorComponent;
            }
        }

        static UFEventEditor.ComponentService.EventEditorManagerComponent evEditorComponent = new UFEventEditor.ComponentService.EventEditorManagerComponent();
        static public UFEventEditor.ComponentService.EventEditorManagerComponent EVEditorComponent
        {
            get
            {
                return evEditorComponent;
            }
        }
        static StringEditorManagerComponent stringManagerComponent = new StringEditorManagerComponent();
        static public StringEditorManagerComponent StringManagerComponent
        {
            get { return stringManagerComponent; }
        }

        static UFUserEditorManagerComponent userManagerComponent = new UFUserEditorManagerComponent();
        static public UFUserEditorManagerComponent UserManagerComponent
        {
            get { return UserManagerComponent; }
        }
#endif
        private IODataServer _IODataServer = null;
        /// <summary>
        /// reference to the settings of the I/O DataServer of the project
        /// </summary>
        public IODataServer IODataServer
        {
            get
            {
                if(_IODataServer == null && projectDocument != null && ufuaEditorComponent != null)
                {
                    _IODataServer = new IODataServer(projectDocument, ufuaEditorComponent);
                }
                return _IODataServer;
            }
            private set { _IODataServer = value; }
        }

#if !NET_STANDARD
        private AlarmDispatcher _AlarmDispatcher = null;
        /// <summary>
        /// reference to the settings of the Alarm Dispatcher Server of the project
        /// </summary>
        public AlarmDispatcher AlarmDispatcher
        {
            get
            {
                if (_AlarmDispatcher == null && projectDocument != null && adEditorComponent != null)
                    _AlarmDispatcher = new AlarmDispatcher(projectDocument, adEditorComponent);
                return _AlarmDispatcher;
            }
            private set { _AlarmDispatcher = value; }
        }

        private EventsManager _EventManager =  null;
        public EventsManager EventManager
        {
            get
            {
                if (_EventManager == null && projectDocument != null && evEditorComponent != null)
                    _EventManager = new EventsManager(projectDocument, evEditorComponent);
                return _EventManager;
            }
            private set { _EventManager = value; }
        }

        private ScreenContainer _ScreenContainer = null;
        /// <summary>
        /// reference to the Screens containeed in the project
        /// </summary>
        public ScreenContainer ScreenContainer
        {
            get
            {
                if (_ScreenContainer == null && projectDocument != null)
                    _ScreenContainer = new ScreenContainer(projectDocument, UFProjectManagerComponent);
                return _ScreenContainer;
            }
            private set { _ScreenContainer = value; }
        }

        private UsersManager _UsersManager = null;
        public UsersManager UsersManagerResource
        {
            get
            {
                if (_UsersManager == null && projectDocument != null)
                    _UsersManager = new UsersManager(projectDocument, userManagerComponent);
                return _UsersManager;
            }
        }
        private TextResource _StringsResource = null;
        public TextResource StringsResource
        {
            get
            {
                if(_StringsResource == null && projectDocument != null)
                    _StringsResource = new TextResource(projectDocument, stringManagerComponent);
                return _StringsResource;
            }
            private set { _StringsResource = value; }
        }

        private Parameters _ParametersContainer = null;
        public Parameters ParametersContainer
        {
            get
            {
                if (_ParametersContainer == null && projectDocument != null)
                    _ParametersContainer = new Parameters(projectDocument, UFProjectManagerComponent);
                return _ParametersContainer;
            }
            private set { _ParametersContainer = value; }
        }

        private MenuContainer _MenuContainer = null;
        public MenuContainer MenuContainer
        {
            get
            {
                if (_MenuContainer == null && projectDocument != null)
                    _MenuContainer = new MenuContainer(projectDocument, UFProjectManagerComponent);
                return _MenuContainer;
            }
            private set { _MenuContainer = value; }
        }
        private ShortcutContainer _ShortcutContainer = null;
        public ShortcutContainer ShortcutContainer
        {
            get
            {
                if (_ShortcutContainer == null && projectDocument != null)
                    _ShortcutContainer = new ShortcutContainer(projectDocument, UFProjectManagerComponent);
                return _ShortcutContainer;
            }
            private set { _ShortcutContainer = value; }
        }

        private RecipeContainer _RecipeContainer = null;
        public RecipeContainer RecipeContainer
        {
            get
            {
                if (_RecipeContainer == null && projectDocument != null)
                    _RecipeContainer = new RecipeContainer(projectDocument, UFProjectManagerComponent);
                return _RecipeContainer;
            }
        }

        private ReportContainer _ReportContainer = null;
        public ReportContainer ReportContainer
        {
            get
            {
                if (_ReportContainer == null && projectDocument != null)
                    _ReportContainer = new ReportContainer(projectDocument, UFProjectManagerComponent);
                return _ReportContainer;
            }
        }

        private SchedulerServer _SchedulerServer = null;
        public SchedulerServer SchedulerServer
        {
            get
            {
                if(_SchedulerServer == null && projectDocument != null && schEditorComponent != null)
                    _SchedulerServer = new SchedulerServer(projectDocument, schEditorComponent);
                return _SchedulerServer;
            }
        }

#endif
        #endregion Properties
    }
}
