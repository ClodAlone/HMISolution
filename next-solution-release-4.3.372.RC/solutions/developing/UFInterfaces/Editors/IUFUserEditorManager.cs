using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WINDOWS_UWP && !NET_STANDARD
using System.Windows.Controls;
#endif
using DocumentManager.ComponentService;
using UFInterfaces;

namespace UFUserEditor.ComponentService
{
    public class VerifyPasswordEventArgs : EventArgs
    {
        public String User;
        public String OldPassword;
        public String Password;
        public String VerifyPassword;
        public bool bForce;
        public bool requestingVerifyPassword;
        public bool dialogResult;
    }

    public interface IUFUserEditorManager : IUFInterfaceBase
    {
        IEnumerable<String> GetListRoleNames(IDocument parent, bool bRefresh = false, bool bRuntime = true);
        IEnumerable<String> GetListUserNames(IDocument parent, bool bRefresh = false);
        IEnumerable<object> GetRoles(IDocument parent);
        int GetRoleAccessMask(IDocument parent, String role);
        int GetRoleAccessLevel(IDocument parent, String role);
        string GetRoleCultureName(IDocument parent, String role);
        string GetRoleConverterName(IDocument parent, String role);
        IList<String> GetSMTPSettings(IDocument parent);
        Dictionary<String, String> GetExternalAuthenticationSettings(IDocument parent);
        Dictionary<String, String> GetUsersEmail(IDocument parent, String NodeId);

        String GetConnectionStringFromFile(String file);

        String GetSharedApplicationName(IDocument parent);

        String GetUserRole(IDocument parent, String user);
        int GetUserAccessLevel(IDocument parent, String user);
        int GetUserAccessMask(IDocument parent, String user);
        String GetUserElectronicSignature(IDocument parent, String user);
        bool VerifyPasswordExpired(IDocument parent, String user, bool bForce = false);
        int VerifyUser(IDocument parent, String user, String password);
        bool UpdateUser(IDocument parent, String user, String oldpassword, String newpassword);

        int GetAutoLogoutSeconds(IDocument parent, String user);
        int GetRoleAutoLogoutSeconds(IDocument parent, String role);
        int GetMinRequiredPasswordLength(IDocument parent);
        int GetMaxInvalidPasswordAttempts(IDocument parent, bool bRefresh = false);

        bool GetEnableUserManager(IDocument parent);
        bool GetLoginControlVisible(IDocument parent);
        String GetDesktopSystemRole(IDocument parent);
        String GetUserCultureName(IDocument parent, String user);
        String GetUserConverterName(IDocument parent, String user);

        bool SetUserRole(IDocument parent, String user, String newRole);

        string GetExternalAuthenticationUserRole(IDocument parent, String externalRole);

#if !WINDOWS_UWP && !NET_STANDARD
        UserControl GetUserEditControl(IDocument parent, bool bRefresh = false);
        UserControl GetRuntimeUserEditControl(IDocument parent);
        bool SaveAndRelease(IDocument parent);
#endif
#if !NET_STANDARD
        void EnsureCredentialProvider(IDocument parent);
#endif

#if !WINDOWS_UWP && !NET_STANDARD
        event EventHandler<UserSettingsChangedArg> UserSettingsChanged;
        Type GetAccessRoleEditorType();

        event EventHandler<VerifyPasswordEventArgs> PromptVerifyPassword;
#endif
    }
}
