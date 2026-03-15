#if NET48
using ExternalAuthentication.Model;
using ExternalAuthentication.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;



namespace ExternalAuthentication.Services
{
    public interface IExternalLoginService
    {
        Task Execute(ExternalIdPUserSettings externalIdPUserSettings);

        event EventHandler LoginProcedureCompleted;
    }
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IOidcClientServiceExtension _oidcClientServiceExtension;
        private readonly ITokenValidationService _tokenValidationService;
        private readonly ISetOidcClientLoginResultService _setLoginResultService;

        public ExternalLoginService(
            IOidcClientServiceExtension oidcClientServiceExtension,
            ITokenValidationService tokenValidationService, 
            ISetOidcClientLoginResultService setLoginResultService)
        {
            _oidcClientServiceExtension = oidcClientServiceExtension;
            _tokenValidationService = tokenValidationService;
            _setLoginResultService = setLoginResultService;
        }

        public async Task Execute(ExternalIdPUserSettings externalIdPUserSettings)
        {
            try
            {
                ExternalIdPLoginResult loginCompletedResult;
                var oidcClientLoginResult = await _oidcClientServiceExtension.ExecuteLogin();

                if (oidcClientLoginResult.IsError == true)
                {
                    loginCompletedResult =
                        ExternalIdPLoginResult.Create(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        new List<string>(),
                        false,
                        oidcClientLoginResult.Error + " " + oidcClientLoginResult.ErrorDescription);

                    OnLoginProcedureCompleted(new LoginProcedureCompletedArgs(loginCompletedResult));
                    return;
                }
      
                var idTokenValidationResult = 
                    await _tokenValidationService.Execute(oidcClientLoginResult.IdentityToken, externalIdPUserSettings);

                if (idTokenValidationResult == false)
                {
                    loginCompletedResult =
                        ExternalIdPLoginResult.Create(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        new List<string>(),
                        false,
                        Properties.Resources.TokenValidationFailed);

                    OnLoginProcedureCompleted(new LoginProcedureCompletedArgs(loginCompletedResult));
                    return;
                }

                if (oidcClientLoginResult.IsError == false && idTokenValidationResult == true)
                {
                    loginCompletedResult = _setLoginResultService.Execute(oidcClientLoginResult);
                    
                    OnLoginProcedureCompleted(new LoginProcedureCompletedArgs(loginCompletedResult));
                    return;
                }
            }
            catch (Exception ex)
            {
                var loginProcedureFail = 
                        ExternalIdPLoginResult.Create(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        new List<string>(),
                        false,
                        Properties.Resources.UnexpectedLoginError + ex.Message);

                OnLoginProcedureCompleted(new LoginProcedureCompletedArgs(loginProcedureFail));
                throw new Exception(Properties.Resources.UnexpectedLoginError  + ex.Message);
            }
        }

        #region Events
        public event EventHandler LoginProcedureCompleted;

        protected virtual void OnLoginProcedureCompleted(LoginProcedureCompletedArgs e)
        {
            LoginProcedureCompleted?.Invoke(this, e);
        }
        #endregion
    }
}

#endif