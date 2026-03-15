using DocumentManager.ComponentService;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UFInterfaces.AuditTrace;
using UFRecipeEditor.ComponentService;
using UFRecipeSettings.Documents;
using Utilities;
using ViewModelLib;

namespace UFRecipeExecuter.OPCUA
{
    public class RecipeUAViewModel : ViewModelBase, IAuditTrace
    {
        #region Declarations
        readonly IDocument parent;
        readonly Uri recipeUri;

        readonly static List<String> auditBrowserNames = new List<String>()
        {
            RecipeUAServerInfo.BrowserNames.IsAuditTraceEnabled,
            RecipeUAServerInfo.BrowserNames.IsCommentRequiredOnAudit,
            RecipeUAServerInfo.BrowserNames.IsPasswordRequiredOnAudit,
            RecipeUAServerInfo.BrowserNames.MinAccessLevelRequiredOnAudit
        };

        Dictionary<string, NodeIdViewModel> auditProperties;
        SessionViewModel recipeSessionViewModel;
        String lastUser;

        UFRecipeDocument recipeDocument;
        OPCUAEntityReference serverReference;
        string sessionString;

        bool bDisposeRecipeDocument;
        bool bExecuted;
        #endregion

        #region Constructors
        public RecipeUAViewModel(IDocument parent, Uri recipeUri)
        {
            this.parent = parent;
            this.recipeUri = recipeUri;
        }

        public RecipeUAViewModel(UFRecipeDocument document)
        {
            this.recipeUri = new Uri(document.FullPath, UriKind.RelativeOrAbsolute);
            this.recipeDocument = document;
            this.parent = document.Parent;
        }
        #endregion

        #region IAuditTrace
        public event EventHandler<EventArgs> AuditPropertiesFetched;
        void OnAuditPropertiesFetched()
        {
            var t = AuditPropertiesFetched;
            if (t != null)
                t(this, EventArgs.Empty);
        }

        public bool IsAuditPropertiesFetched
        {
            get
            {
                lock (lockObject)
                {
                    return auditProperties != null;
                }
            }
        }
        #endregion

        #region Methods
        public void PrepareExecution(String sessionName)
        {
            if (bExecuted || RecipeManager == null)
                return;
            bExecuted = true;
            sessionString = sessionName;

            if (recipeDocument == null)
            {
                Uri uri = parent.MakeAbosoluteUri(recipeUri);
                recipeDocument = UFRecipeDocument.FromFile(uri.GetPathString(), parent);
                bDisposeRecipeDocument = true;
            }
            if (recipeDocument != null)
            {
                var stringReference = RecipeManager.GetRecipeUAServerEntityReference(parent, RecipeUAConnector.GetRelativePath(recipeUri, parent), RecipeUAConnector.GetNodeId(recipeUri, parent));
                if (stringReference != null)
                {
                    serverReference = stringReference.FromXml<OPCUAEntityReference>();
                    if (serverReference != null)
                    {
                        serverReference.PropertyChanged += RecipeEntityReference_PropertyChanged;
                        serverReference.Resolve(sessionName);
                        serverReference.SetInUse(recipeDocument.RecipeEntity, true);
                    }
                }
            }
        }

        public void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            if (serverReference != null)
            {
                serverReference.PropertyChanged -= RecipeEntityReference_PropertyChanged;
                serverReference.SetInUse(recipeDocument.RecipeEntity, false);
                serverReference = null;
            }

            if (recipeSessionViewModel != null)
            {
                recipeSessionViewModel.UserIdentityChanged -= RecipeUAViewModel_UserIdentityChanged;
                recipeSessionViewModel = null;
            }

            if (bDisposeRecipeDocument && recipeDocument != null)
            {
                recipeDocument.Dispose();
                recipeDocument = null;
            }
        }

        void RecipeEntityReference_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NodeIdViewModel")
            {
                if (serverReference.NodeIdViewModel != null)
                {
                    if (recipeSessionViewModel != null)
                        recipeSessionViewModel.UserIdentityChanged -= RecipeUAViewModel_UserIdentityChanged;
                    recipeSessionViewModel = serverReference.NodeIdViewModel.sessionViewModel;
                    if (recipeSessionViewModel != null)
                        recipeSessionViewModel.UserIdentityChanged += RecipeUAViewModel_UserIdentityChanged;
                    PromoteIdleExecution(100, bStatic: true);
                }
            }
        }

        void RecipeUAViewModel_UserIdentityChanged(object sender, OPCUAViewModel.UserIdentityChangedEventArgs e)
        {
            var userName = e.UserIdentity?.DisplayName;
            if (lastUser == userName)
                return;
            lastUser = userName;

            OnPropertyChanged("UserIdentity");
        }

        void FetchAuditProperties()
        {
            if (auditProperties != null || serverReference == null || serverReference.NodeIdViewModel == null)
                return;

            var result = new Dictionary<string, NodeIdViewModel>();
            var props = serverReference.NodeIdViewModel.NodeProperties.ToList();
            var properties = (from property in props/*.AsParallel().WithCancellation(token)*/
                              where auditBrowserNames.Contains(property.BrowseName.Name)
                              select property).ToList();
            properties.ForEach((property) =>
            {
                if (!result.ContainsKey(property.BrowseName.Name))
                    result.Add(property.BrowseName.Name, property);
            });

            lock (lockObject)
            {
                auditProperties = result;
            }

            OnAuditPropertiesFetched();
            OnPropertyChanged("IsAuditTraceEnabled");
            OnPropertyChanged("IsCommentRequired");
            OnPropertyChanged("IsPasswordRequired");
            OnPropertyChanged("AccessLevelRequired");
        }
        #endregion

        #region Overrides
        protected override void IdleExecution()
        {
            try
            {
                LastMessage = String.Empty;
                FetchAuditProperties();
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                LastMessage = message;
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            TerminateExecution();
        }
        #endregion

        #region Properties
        IRecipeEditorManager recipeManager;
        internal IRecipeEditorManager RecipeManager
        {
            get
            {
                if (recipeManager == null)
                {
                    recipeManager = parent.GetService(typeof(IRecipeEditorManager)) as IRecipeEditorManager;
                }

                return recipeManager;
            }
        }

        public UFRecipeDocument RecipeDocument
        {
            get
            {
                return recipeDocument;
            }
        }

        public String SessionName
        {
            get
            {
                return sessionString;
            }
        }

        public NodeIdViewModel NodeIdViewModel
        {
            get
            {
                return serverReference?.NodeIdViewModel;
            }
        }

        public bool IsConnected
        {
            get
            {
                return NodeIdViewModel != null && 
                    NodeIdViewModel.sessionViewModel != null && 
                    NodeIdViewModel.sessionViewModel.Connected;
            }
        }

        public bool IsAuditTraceEnabled
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(RecipeUAServerInfo.BrowserNames.IsAuditTraceEnabled, out n) &&
                        n.DataValue != null && n.DataValue.Value is Boolean)
                        return Convert.ToBoolean(n.DataValue.Value);
                }

                return false;
            }
        }

        public bool IsCommentRequired
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(RecipeUAServerInfo.BrowserNames.IsCommentRequiredOnAudit, out n) &&
                        n.DataValue != null && n.DataValue.Value is Boolean)
                        return Convert.ToBoolean(n.DataValue.Value);
                }

                return false;
            }
        }

        public bool IsPasswordRequired
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(RecipeUAServerInfo.BrowserNames.IsPasswordRequiredOnAudit, out n) &&
                        n.DataValue != null && n.DataValue.Value is Boolean)
                        return Convert.ToBoolean(n.DataValue.Value);
                }

                return false;
            }
        }

        public int AccessLevelRequired
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(RecipeUAServerInfo.BrowserNames.MinAccessLevelRequiredOnAudit, out n) &&
                        n.DataValue != null && n.DataValue.Value is Int32)
                        return Convert.ToInt32(n.DataValue.Value);
                }

                return 0;
            }
        }
        #endregion
    }
}
