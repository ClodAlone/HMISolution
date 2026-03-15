using DocumentManager.ComponentService;
using EditSettingsHelper.ComponentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UFInterfaces.AuthenticationCredentialsProvider;
using UFUserEditor.ComponentService;
using DocumentManager.ComponentService.Helpers;

namespace EditSettingsHelper
{
    public class Helper: IDisposable
    {
        bool bDisposed;
        IAuthenticationCredentialsProvider authenticationProvider;
        IUFUserEditorManager userEditor;
        IDocument Document;
        ISettingsHelper Control;

        public string Username
        {
            get;
            private set;
        } = String.Empty;

        public bool UserManagerEnabled
        {
            get
            {
                if (Document != null)
                {
                    if (userEditor == null)
                        userEditor = Document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
                    if (userEditor != null)
                        return userEditor.GetEnableUserManager(Document);
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        public Helper(IDocument document, ISettingsHelper control)
        {
            Control = control;
            Document = document;

            if (Document != null)
            {
                authenticationProvider = Document.GetService(typeof(IAuthenticationCredentialsProvider)) as IAuthenticationCredentialsProvider;
                if (authenticationProvider != null)
                    authenticationProvider.UserOnline += AuthenticationProvider_UserOnline;
            }
        }
        private void AuthenticationProvider_UserOnline(object sender, LoginInfoEventArgs e)
        {
            if (e.IsRefreshing && !bisRefreshing) return; 
            Username = e.User;
            _currentUserAccessLevel = 0;
            _currentUserReadAccessMask = 0;
           if (userEditor == null)
                userEditor = Document.GetService(typeof(IUFUserEditorManager)) as IUFUserEditorManager;
            if (userEditor != null)
            {
                if (!String.IsNullOrEmpty(e.User))
                {
                    _currentUserAccessLevel = userEditor.GetUserAccessLevel(Document, e.User);
                    _currentUserReadAccessMask = userEditor.GetUserAccessMask(Document, e.User);
                }
            }
            if (Control != null)
            {
                Control.UpdateWriteAccessCommands();
                Control.ReloadRuntimeSettings();
            }
        }
        int _currentUserAccessLevel;
        int _currentUserReadAccessMask;
        public int CurrentUserAccessLevel { get { return _currentUserAccessLevel; }}
        public int CurrentUserReadAccessMask { get { return _currentUserReadAccessMask; } }
        public void Dispose()
        {
            if (bDisposed)
                return;
            bDisposed = true;
            if (authenticationProvider != null)
                authenticationProvider.UserOnline -= AuthenticationProvider_UserOnline;
        }
        public bool HasAccessLevel()
        {
            if (Control == null)
                return false;
            return !(UserManagerEnabled &&  (CurrentUserAccessLevel < Control.EditingWriteAccessLevel ||
                           Control.EditingWriteAccessMask != 0 && (CurrentUserReadAccessMask & Control.EditingWriteAccessMask) == 0));
        }
        public bool ValidateAccessLevel()
        {
            if (!HasAccessLevel())
            {
                if (authenticationProvider != null && Control != null && Document != null)
                    authenticationProvider.ValidateUsingCredentialsProvider(Document, DocumentHelper.GetRootParent(Document, traverse: true).Title, null, Control.EditingWriteAccessLevel);
                return true;
            }
            return false;
        }
        bool bisRefreshing;
        public void RefreshCurrentUser()
        {
            bisRefreshing = true;
            try
            {
                if (Document != null && authenticationProvider != null)
                    Username = authenticationProvider.RefreshCurrentUser(DocumentHelper.GetRootParent(Document, traverse: true).Title);
            }
            finally
            {
                bisRefreshing = false;
            }
        }
    }
}
