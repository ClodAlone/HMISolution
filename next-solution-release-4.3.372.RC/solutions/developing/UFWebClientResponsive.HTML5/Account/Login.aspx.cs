using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using UFWebClient.HTML5;

namespace UFWebClientResponsive_HTML5
{
    public partial class Login : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(UFWebClient.HTML5.Properties.Resources.WebClientHTML5Log);

        //static string cookieLoginFailed = "LoginFailed";
        static int nCounterLoginFailed = 0;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Global.HideUserRegister)
                RegisterHyperLink.Visible = false;
            var action = Request.QueryString["Action"];
            if (action == "Logout")
            {
                System.Web.Security.FormsAuthentication.SignOut();
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
        }

        protected void LoginUser_LoginError(object sender, EventArgs e)
        {
            var user = Membership.GetUser(LoginUser.UserName);
            if (user != null && user.IsLockedOut)
            {
                log.Info(String.Format(UFWebClient.HTML5.Properties.Resources.UserLockedFailedLogIn, LoginUser.UserName));
                LoginUser.FailureText = UFWebClient.HTML5.Properties.Resources.FailedToLoginUserLocked;
            }
            else
            {
                //if (Session[cookieLoginFailed] == null)
                //    Session[cookieLoginFailed] = 0;

                //var nCounterLoginFailed = (int)Session[cookieLoginFailed];
                var maxInvalidPasswordAttempts = Global.UFUserEditorComponent.GetMaxInvalidPasswordAttempts(Global.projectDocument);
                if (++nCounterLoginFailed >= maxInvalidPasswordAttempts)
                    log.Info(String.Format(UFWebClient.HTML5.Properties.Resources.UserFailedLogIn, LoginUser.UserName));
                //Session[cookieLoginFailed] = nCounterLoginFailed;
            }
        }

        protected void LoginUser_LoggingIn(object sender, LoginCancelEventArgs e)
        {
            if (!Global.UFUserEditorComponent.GetEnableUserManager(Global.projectDocument))
                return;

            var userLockType = Global.UFUserEditorComponent.GetUserLockMode(Global.projectDocument);
            if (userLockType == UFUserModel.UserLockType.AllUsers)
                return;

            var unlock = false;
            if (userLockType == UFUserModel.UserLockType.None)
                unlock = true;
            else if (userLockType == UFUserModel.UserLockType.OnlyEditableUsers)
            {
                var accessLevel = Global.UFUserEditorComponent.GetMaxRuntimeEditAccessLevel(Global.projectDocument);
                var userLevel = Global.UFUserEditorComponent.GetUserAccessLevel(Global.projectDocument, LoginUser.UserName);
                if (userLevel > accessLevel)
                    unlock = true;
            }

            if (unlock)
            {
                try
                {
                    var mbsu = Membership.GetUser(LoginUser.UserName);
                    if (mbsu != null)
                        mbsu.UnlockUser();
                }
                catch
                { }
            }
        }

        protected void LoginUser_LoggedIn(object sender, EventArgs e)
        {
            nCounterLoginFailed = 0;
        }
    }
}
