using System;
using System.Windows;
using UFInterfaces.AuthenticationCredentialsProvider;
using UIMsgBoxAlertService.ComponentService;
using ViewModelLib;
using Utilities.WPF;
using System.Windows.Input;
using Utilities;
using DocumentManager.ComponentService;
using StringManager.ComponentService;
using UFRecipeExecutionContext;
using System.Collections.Generic;
using OPCUAViewModel;
using System.Linq;
using Opc.Ua;
using System.ComponentModel;
using UFRecipeExecuter.OPCUA;
using TranslationHelpers;

namespace RecipeAuditTrace
{
    public class RecipeAuditTraceViewModel : NotifyingBase, IDataErrorInfo
    {
        #region Declarations
        readonly RecipeUAViewModel recipeUAViewModel;
        readonly IDocument rootParent;
        
        internal readonly static string stringPlaceolder = "RecipeAuditTraceComment";

        string userName;
        string password;
        #endregion

        #region Constructors
        public RecipeAuditTraceViewModel(RecipeUAViewModel recipeUAViewModel)
        {
            this.recipeUAViewModel = recipeUAViewModel;

            rootParent = recipeUAViewModel.RecipeDocument;
            while (rootParent.Parent != null)
                rootParent = rootParent.Parent;
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs> Execute;
        void OnExecute()
        {
            var t = Execute;
            if (t != null)
                t(this, EventArgs.Empty);
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

        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "AuditComment")
            {
                if (String.IsNullOrWhiteSpace(AuditComment))
                {
                    return TranslationHelper.TranlslateText($"_{stringPlaceolder}_CommentEmptyError", currentStringIds, Properties.Resources.AuditCommentEmptyError);
                }
            }

            return null;
        }

        #region Methods
        public bool AskUserComment()
        {
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

            CurrentStringIds = StringManager?.GetListStringForCulture(recipeUAViewModel.RecipeDocument, stringManager.GetActiveCulture(recipeUAViewModel.RecipeDocument));
            
            var title = TranslationHelper.TranlslateText($"_{stringPlaceolder}_Title", currentStringIds, Properties.Resources.AuditTraceCommentTitle);
            auditTraceComment.txtRecipeName.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecipeName", currentStringIds, Properties.Resources.AuditTraceComment_RecipeName);
            auditTraceComment.txtRecipeIndex.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecipeIndex", currentStringIds, Properties.Resources.AuditTraceComment_RecipeIndex);
            auditTraceComment.txtRecipeCommand.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_RecipeCommand", currentStringIds, Properties.Resources.AuditTraceComment_RecipeCommand);
            auditTraceComment.txtUserComment.Text = TranslationHelper.TranlslateText($"_{stringPlaceolder}_UserComment", currentStringIds, Properties.Resources.AuditTraceComment_UserComment);

            var captions = new Dictionary<GeneralDialogButtons, String>()
                            {
                                { GeneralDialogButtons.OkButton, TranslationHelper.TranlslateText("_UserDialog_OkLabel", currentStringIds, WPFUtilities.Properties.Resources.LabelOk) },
                                { GeneralDialogButtons.CancelButton, TranslationHelper.TranlslateText("_UserDialog_CancelLabel", currentStringIds, WPFUtilities.Properties.Resources.LabelCancel) }
                            };
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
                    var wnd = auditTraceComment.FindParent<Window>();
                    e.Cancel = !CheckAudit(wnd);
                }
            };

            var bRet = auditDialog.ShowDialog() == true;
            if (!bRet)
                return false;
            return true;
        }

        bool CheckAudit(Window wnd)
        {
            try
            {
                userName = password = null;
                if (AuthenticationCredentialsProvider != null)
                {
                    AuthenticationCredentialsProvider.UserOnline += AuthenticationCredentialsProvider_UserOnline;
                    AuthenticationCredentialsProvider.RefreshCurrentUser(rootParent.Title);
                    AuthenticationCredentialsProvider.UserOnline -= AuthenticationCredentialsProvider_UserOnline;
                }

                int counter = 0;
                bool isPasswordRenewed = false;
                while (true)
                {
                    try
                    {
                        if (!recipeUAViewModel.IsPasswordRequired || isPasswordRenewed)
                        {
                            using (new WaitCursor())
                            {
                                OnExecute();
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is ServiceResultException))
                        {
                            var error = TranslationHelper.TranlslateText($"_{stringPlaceolder}_OperationFailed", currentStringIds, Properties.Resources.AuditTrace_RecipeOperationFailed, ex.Message);
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
                                var error = TranslationHelper.TranlslateText($"_{stringPlaceolder}_OperationFailed", currentStringIds, Properties.Resources.AuditTrace_RecipeOperationFailed, exception.Message);
                                if (UIInterface != null)
                                    UIInterface.ShowError(error);
                                return false;
                            }
                        }

                        isPasswordRenewed = false;
                    }

                    if (AuthenticationCredentialsProvider == null)
                    {
                        if (UIInterface != null)
                            UIInterface.ShowError(TranslationHelper.TranlslateText($"_{stringPlaceolder}_LoginRequired", currentStringIds, Properties.Resources.AuditLoginRequired));
                        return false;
                    }

                    if (++counter > 3)
                        System.Threading.Thread.Sleep((counter - 3) * 2000);

                    var credentials = AuthenticationCredentialsProvider.ShowLoginWindow(rootParent, requestedRoleOrUser: null, requestedLevel: recipeUAViewModel.AccessLevelRequired, owner: wnd);
                    if (credentials == null)
                        return false;
                    isPasswordRenewed = AuthenticationCredentialsProvider.Validate(rootParent.Title, credentials.UserName, credentials.Password);
                    if (isPasswordRenewed)
                    {
                        try
                        {
                            if (String.IsNullOrEmpty(credentials.UserName))
                                RealTimeConnectionManagerViewModel.SetUserIdentity(recipeUAViewModel.SessionName, null, new StringCollection());
                            else
                                RealTimeConnectionManagerViewModel.SetUserIdentity(recipeUAViewModel.SessionName, new UserIdentity(credentials.UserName, credentials.Password), new StringCollection());
                        }
                        catch (Exception ex)
                        {
                            isPasswordRenewed = false;
                        }
                    }
                }
            }
            finally
            {
                if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
                    RealTimeConnectionManagerViewModel.SetUserIdentity(recipeUAViewModel.SessionName, new UserIdentity(userName, password), new StringCollection());
                else
                    RealTimeConnectionManagerViewModel.SetUserIdentity(recipeUAViewModel.SessionName, null, new StringCollection());
            }
        }

        void AuthenticationCredentialsProvider_UserOnline(object sender, UFInterfaces.AuthenticationCredentialsProvider.LoginInfoEventArgs e)
        {
            userName = e.User;
            password = e.Password;
        }
        #endregion

        #region Properties
        public String RecipeName
        {
            get
            {
                return recipeUAViewModel.RecipeDocument.RecipeEntity.RecipeName;
            }
        }

        String recipeIndex;
        public String RecipeIndex
        {
            get
            {
                return recipeIndex;
            }
            set
            {
                if (recipeIndex == value)
                    return;
                recipeIndex = value;
                OnPropertyChanged("RecipeIndex");
            }
        }

        RecipeCommandType recipeCommand;
        public RecipeCommandType RecipeCommand
        {
            get
            {
                return recipeCommand;
            }
            set
            {
                if (recipeCommand == value)
                    return;
                recipeCommand = value;
                OnPropertyChanged("RecipeCommand");
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
                if (auditComment == value)
                    return;
                auditComment = value;
                OnPropertyChanged("AuditComment");
            }
        }

        IDictionary<String, String> currentStringIds;
        public IDictionary<String, String> CurrentStringIds
        {
            get
            {
                return currentStringIds;
            }
            set
            {
                if (currentStringIds == value)
                    return;
                currentStringIds = value;
                OnPropertyChanged("CurrentStringList");
            }
        }

        public UIElement Control { get; set; }


        IAuthenticationCredentialsProvider authenticationCredentialsProvider;
        IAuthenticationCredentialsProvider AuthenticationCredentialsProvider
        {
            get
            {
                if (authenticationCredentialsProvider == null)
                    authenticationCredentialsProvider = rootParent.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                return authenticationCredentialsProvider;
            }
        }

        IUIMsgBoxAlertService uiInterface;
        public IUIMsgBoxAlertService UIInterface
        {
            get
            {
                if (uiInterface == null)
                    uiInterface = rootParent.GetService(typeof(IUIMsgBoxAlertService)) as IUIMsgBoxAlertService;
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
