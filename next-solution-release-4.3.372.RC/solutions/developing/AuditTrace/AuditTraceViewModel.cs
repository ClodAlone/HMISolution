using DocumentManager.ComponentService;
using Opc.Ua;
using OPCUAViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using UFInterfaces;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUAEditor.ComponentService;
using UIMsgBoxAlertService.ComponentService;
using StringManager.ComponentService;
using Utilities;
using Utilities.WPF;
using ViewModelLib;
using TranslationHelpers;

namespace AuditTrace
{
    public class AuditTraceViewModel : ViewModelBase
    {
        #region Declarations
        readonly MonitoredItemViewModel monitoredItemViewModel;
        readonly IEntityReference entityReference;
        readonly IDocument document;
        readonly IDocument rootParent;
        readonly string sessionName;

        readonly static string stringPlaceolder = "TagAuditTraceComment";
        IDictionary<String, String> currentStringIds;

        readonly static List<String> auditBrowserNames = new List<String>()
        {
            UFUAServerInfo.BrowserNames.IsAuditTraceEnabled,
            UFUAServerInfo.BrowserNames.LastCommentOnAudit,
            UFUAServerInfo.BrowserNames.LastUserNameOnAudit
        };

        AuditWriteCommand writeAuditCommand = null;
        Dictionary<string, NodeIdViewModel> auditProperties;
        CancellationTokenSource cts;

        string userName;
        string password;

        bool bExecuted;
        #endregion

        #region Constructors
        public AuditTraceViewModel(MonitoredItemViewModel monitoredItemViewModel, IEntityReference entityReference, IDocument document, string sessionName)
        {
            this.monitoredItemViewModel = monitoredItemViewModel;
            this.entityReference = entityReference;
            this.document = document;
            this.sessionName = sessionName;

            rootParent = document;
            while (rootParent.Parent != null)
                rootParent = rootParent.Parent;

            PromoteIdleExecution(100, bStatic: true);
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs> AuditPropertiesFetched;
        void OnAuditPropertiesFetched()
        {
            var t = AuditPropertiesFetched;
            if (t != null)
                t(this, EventArgs.Empty);
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

        public override string this[string propertyName]
        {
            get
            {
                return PerformValidation(propertyName);
            }
        }

        protected override String PerformValidation(String propertyName)
        {
            if (propertyName == "AuditComment")
            {
                if (String.IsNullOrWhiteSpace(AuditComment))
                {
                    return TranslationHelper.TranlslateText($"_{stringPlaceolder}_CommentEmptyError", currentStringIds, Properties.Resources.AuditCommentEmptyError);
                }
            }

            return base.PerformValidation(propertyName); ;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }
        #endregion

        #region Methods
        void FetchAuditProperties()
        {
            if (auditProperties != null || monitoredItemViewModel.NodeIdModel == null)
                return;

            if (cts == null)
                cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            var result = new Dictionary<string, NodeIdViewModel>();
            var props = monitoredItemViewModel.NodeIdModel.NodeProperties.ToList();
            var properties = (from property in props/*.AsParallel().WithCancellation(token)*/
                              where auditBrowserNames.Contains(property.BrowseName.Name)
                              select property).ToList();
            properties.ForEach((property) =>
            {
                if (token.IsCancellationRequested)
                    return;

                if (!result.ContainsKey(property.BrowseName.Name))
                    result.Add(property.BrowseName.Name, property);
            });

            auditProperties = result;

            OnAuditPropertiesFetched();
            OnPropertyChanged("IsAuditTraceEnabled");
            OnPropertyChanged("LastCommentOnAudit");
            OnPropertyChanged("LastUserNameOnAudit");
        }

        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs e)
        {
            userName = e.User;
            password = e.Password;
        }

        public bool SetValue(string newValue)
        {
            var dataValue = monitoredItemViewModel.DataValue;
            if (!IsAuditTraceEnabled || (dataValue != null && !Opc.Ua.StatusCode.IsGood(dataValue.StatusCode) &&
                dataValue.StatusCode != Opc.Ua.StatusCodes.UncertainLastUsableValue))
                return false;

            if (Converter != null)
                newValue = Converter.ConvertBack(newValue, typeof(string), ConverterParameter, System.Globalization.CultureInfo.InvariantCulture) as string;

            AuditValue = newValue;
            AuditComment = null;

            var auditTraceComment = new AuditTraceComment()
            {
                DataContext = this
            };

            Window owner = null;
            if (Control != null)
                owner = Control.FindParent<Window>();
            if (owner == null)
            {
                var ie = Keyboard.FocusedElement as DependencyObject;
                if (ie != null)
                    owner = Window.GetWindow(ie);
            }

            currentStringIds = StringManager?.GetListStringForCulture(document, stringManager.GetActiveCulture(document));

            var title = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Title", currentStringIds, Properties.Resources.AuditTraceCommentTitle);
            auditTraceComment.txtDisplayName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_DisplayName", currentStringIds, Properties.Resources.AuditTraceComment_DisplayName);
            auditTraceComment.txtActualValue.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_ActualValue", currentStringIds, Properties.Resources.AuditTraceComment_ActualValue);
            auditTraceComment.txtAudiValue.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_AuditValue", currentStringIds, Properties.Resources.AuditTraceComment_AuditValue);
            auditTraceComment.txtUserComment.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_UserComment", currentStringIds, Properties.Resources.AuditTraceComment_AuditComment);
            
            var captions = GeneralDialogContent.GetDefaultButtonCaptions(currentStringIds);
            var auditDialog = new GeneralDialogContent(auditTraceComment, WPFUtilities.Properties.Settings.Default.DialogFontSize, WPFUtilities.Properties.Settings.Default.ButtonsWidth, WPFUtilities.Properties.Settings.Default.ButtonsHeight,
                GeneralDialogButtons.OkCancelButtons, captions)
            {
                Title = title,
                Owner = owner,
            };

            auditDialog.Closing += (s, e) =>
            {
                if (auditDialog.DialogResult == true)
                {
                    e.Cancel = !CheckAudit();
                }
            };

            try
            {
                PrepareExecution();
                var bRet = auditDialog.ShowDialog() == true;
                if (!bRet)
                    return false;
            }
            finally
            {
                TerminateExecution();
            }

            return true;
        }

        bool CheckAudit()
        {
            if (!IsReady)
            {
                if (UIInterface != null)
                    UIInterface.ShowError(TranslationHelper.TranlslateText($"_{stringPlaceolder}_CallMethodNotReday", currentStringIds, Properties.Resources.AuditCallMethodNotReday));
            }
            else
            {
                userName = password = null;
                if (AuthenticationCredentialsProvider != null)
                {
                    AuthenticationCredentialsProvider.UserOnline += AuthenticationCredentialsProvider_UserOnline;
                    AuthenticationCredentialsProvider.RefreshCurrentUser(rootParent.Title);
                    AuthenticationCredentialsProvider.UserOnline -= AuthenticationCredentialsProvider_UserOnline;
                }

                string userRole = null;
                int userLevel = 0;
                int counter = 0;

                var parameters = new AuditInputParameters()
                {
                    TagNodeId = monitoredItemViewModel.monitoredItem.ResolvedNodeId,
                    AuditValue = AuditValue,
                    AuditComment = AuditComment,
                    TokenId = Guid.NewGuid()
                };

                while (true)
                {
                    parameters.UserName = userName;
                    parameters.Password = password;

                    try
                    {
                        var outputs = writeAuditCommand.Execute(parameters);
                        if (outputs == null)
                            return true;

                        counter = 0;
                        if (AuthenticationCredentialsProvider == null)
                        {
                            if (UIInterface != null)
                                UIInterface.ShowError(TranslationHelper.TranlslateText($"_{stringPlaceolder}_LoginRequired", currentStringIds, Properties.Resources.AuditLoginRequired));
                            return false;
                        }

                        userRole = outputs.requiredUserRole;
                        userLevel = outputs.requiredUserLevel;
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is ServiceResultException))
                        {
                            var error = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CallMethodFailed", currentStringIds, Properties.Resources.AuditCallMethodFailed, ex.Message);
                            if (UIInterface != null)
                                UIInterface.ShowError(error);
                            return false;
                        }
                        else
                        {
                            var exception = ex as ServiceResultException;
                            if (exception.StatusCode != StatusCodes.BadIdentityTokenInvalid &&
                                exception.StatusCode != StatusCodes.BadIdentityTokenRejected &&
                                exception.StatusCode != StatusCodes.BadUserAccessDenied)
                            {
                                var error = TranslationHelper.TranlslateText($"_{stringPlaceolder}_CallMethodFailed", currentStringIds, Properties.Resources.AuditCallMethodFailed, exception.Message);
                                if (UIInterface != null)
                                    UIInterface.ShowError(error);
                                return false;
                            }
                        }
                    }

                    if (++counter > 3)
                        System.Threading.Thread.Sleep((counter - 3) * 2000);

                    var credentials = AuthenticationCredentialsProvider.ShowLoginWindow(document, requestedRoleOrUser: userRole, requestedLevel: userLevel);
                    if (credentials == null)
                        return false;

                    userName = credentials.UserName;
                    password = credentials.Password;
                }
            }

            return false;
        }

        void PrepareExecution()
        {
            if (bExecuted)
                return;
            bExecuted = true;

            if (UAEditorManager != null)
            {
                var opcString = UAEditorManager.GetNodeIdEntityReference(document,
                    UFUAServerInfo.BrowserNames.WriteAuditValue,
                    UFUAServerInfo.Guids.RootTagsGuid.ToString());
                if (!String.IsNullOrEmpty(opcString))
                {
                    var writeAuditValue = opcString.FromXml<OPCUAEntityReference>();
                    writeAuditCommand = new AuditWriteCommand();
                    writeAuditCommand.Init(entityReference, document, sessionName);
                }
            }
        }

        void TerminateExecution()
        {
            if (!bExecuted)
                return;
            bExecuted = false;

            if (writeAuditCommand != null)
                writeAuditCommand.Terminate();
        }
        #endregion

        #region Properties
        public string DisplayName
        {
            get
            {
                return monitoredItemViewModel.DisplayName;
            }
        }

        public string CurrentValue
        {
            get
            {
                return monitoredItemViewModel.InvariantCultureValue;
            }
        }

        string auditValue;
        public string AuditValue
        {
            get
            {
                return auditValue;
            }
            set
            {
                auditValue = value;
                OnPropertyChanged("AuditValue");
            }
        }

        string auditComment;
        public string AuditComment
        {
            get
            {
                return auditComment;
            }
            set
            {
                auditComment = value;
                OnPropertyChanged("AuditComment");
            }
        }

        UIElement control;
        public UIElement Control
        {
            get
            {
                return control;
            }
            set
            {
                if (control == value)
                    return;
                control = value;
                OnPropertyChanged("Control");
            }
        }

        public IEntityReference Entity
        {
            get
            {
                return entityReference;
            }
        }

        IValueConverter converter;
        public IValueConverter Converter
        {
            get
            {
                return converter;
            }
            set
            {
                if (converter == value)
                    return;
                converter = value;
                OnPropertyChanged("Converter");
            }
        }

        object converterParameter;
        public object ConverterParameter
        {
            get
            {
                return converterParameter;
            }
            set
            {
                if (converterParameter == value)
                    return;
                converterParameter = value;
                OnPropertyChanged("ConverterParameter");
            }
        }

        public bool IsAuditTraceEnabled
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(UFUAServerInfo.BrowserNames.IsAuditTraceEnabled, out n) &&
                        n.DataValue != null && n.DataValue.Value is Boolean)
                        return Convert.ToBoolean(n.DataValue.Value);
                }

                return false;
            }
        }

        public string LastCommentOnAudit
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(UFUAServerInfo.BrowserNames.LastCommentOnAudit, out n) &&
                        n.DataValue != null && n.DataValue.Value is String)
                        return Convert.ToString(n.DataValue.Value);
                }

                return null;
            }
        }

        public string LastUserNameOnAudit
        {
            get
            {
                if (auditProperties != null && auditProperties.Count > 0)
                {
                    NodeIdViewModel n;
                    if (auditProperties.TryGetValue(UFUAServerInfo.BrowserNames.LastUserNameOnAudit, out n) &&
                        n.DataValue != null && n.DataValue.Value is String)
                        return Convert.ToString(n.DataValue.Value);
                }

                return null;
            }
        }

        public bool IsReady
        {
            get
            {
                return writeAuditCommand != null && writeAuditCommand.CanExecute();
            }
        }

        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
        IAuthenticationCredentialsProvider AuthenticationCredentialsProvider
        {
            get
            {
                if (authenticationCredentialsProvider == null)
                    authenticationCredentialsProvider = document.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                return authenticationCredentialsProvider;
            }
        }
        
        IUFUAEditorManager uaEditorManager;
        IUFUAEditorManager UAEditorManager
        {
            get
            {
                if (uaEditorManager == null)
                    uaEditorManager = document.GetService(typeof(IUFUAEditorManager)) as IUFUAEditorManager;
                return uaEditorManager;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = document.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
                return uiInterface;
            }
        }

        IStringEditorManager stringManager;
        public IStringEditorManager StringManager
        {
            get
            {
                if (stringManager == null)
                    stringManager = rootParent.GetService(typeof(IStringEditorManager)) as IStringEditorManager;
                return stringManager;
            }
        }
        #endregion
    }
}
