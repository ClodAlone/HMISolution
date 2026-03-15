#if NET48
using ExternalAuthentication.Model;
using ExternalAuthentication.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ExternalAuthentication.Services
{
    public interface IExternalLogoutService
    {
        Task Execute(bool openBrowser);

        event EventHandler LogoutProcedureCompleted;
    }

    public class ExternalLogoutService : IExternalLogoutService
    {
        private readonly IOidcClientServiceExtension _oidcClientServiceExtension;

        public ExternalLogoutService(IOidcClientServiceExtension oidcClientServiceExtension)
        {
           _oidcClientServiceExtension = oidcClientServiceExtension;
        }
        public async Task Execute(bool openBrowser)
        {
            try
            {
                ExternalIdPLogoutResult logoutCompletedResult;

                if (openBrowser) 
                {
                    var oidcClientLogoutResult = await _oidcClientServiceExtension.ExecuteLogout();

                    if (oidcClientLogoutResult.IsError)
                    {
                        logoutCompletedResult =
                            ExternalIdPLogoutResult.Create(
                                false,
                                oidcClientLogoutResult.Error + " " + oidcClientLogoutResult.ErrorDescription);

                        var logoutProcedureCompletedArgs = new LogoutProcedureCompletedArgs(logoutCompletedResult);
                        OnLogoutProcedureCompleted(logoutProcedureCompletedArgs);
                    }
                    else
                    {
                        logoutCompletedResult =
                            ExternalIdPLogoutResult.Create(
                                true,
                                string.Empty);

                        var logoutProcedureCompletedArgs = new LogoutProcedureCompletedArgs(logoutCompletedResult);
                        OnLogoutProcedureCompleted(logoutProcedureCompletedArgs);
                    }
                }
                else
                {
                    logoutCompletedResult =
                            ExternalIdPLogoutResult.Create(
                                true,
                                string.Empty);

                    var logoutProcedureCompletedArgs = new LogoutProcedureCompletedArgs(logoutCompletedResult);
                    OnLogoutProcedureCompleted(logoutProcedureCompletedArgs);
                }
            }
            catch (Exception ex)
            {
                var logoutProcedureFail =
                        ExternalIdPLogoutResult.Create(
                        false,
                        Properties.Resources.UnexpectedLoginError + ex.Message);

                OnLogoutProcedureCompleted(new LogoutProcedureCompletedArgs(logoutProcedureFail));
                throw new Exception(Properties.Resources.UnexpectedLogoutError + ex.Message);
            }
        }

        #region Events
        public event EventHandler LogoutProcedureCompleted;

        protected virtual void OnLogoutProcedureCompleted(LogoutProcedureCompletedArgs e)
        {
            LogoutProcedureCompleted?.Invoke(this, e);
        }
        #endregion
    }
}
#endif