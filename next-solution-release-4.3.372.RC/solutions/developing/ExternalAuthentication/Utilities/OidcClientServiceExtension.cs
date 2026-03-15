using ExternalAuthentication.Model;
using ExternalAuthentication.Services;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using System;
using System.Threading.Tasks;

namespace ExternalAuthentication.Utilities
{
    public interface IOidcClientServiceExtension
    {
        Task<LoginResult> ExecuteLogin();
        Task<LogoutResult> ExecuteLogout();
    }
    public class OidcClientServiceExtension : IOidcClientServiceExtension
    {
        private OidcClient _oidcClient;
        private readonly ISetOidcClient _setOidcClient;

        public OidcClientServiceExtension(ISetOidcClient setOidcClient, IBrowser browser, ExternalIdPUserSettings externalIdPUserSettings)
        {
            _setOidcClient = setOidcClient;

            _oidcClient = _setOidcClient.Execute(
                            externalIdPUserSettings.TenantId,
                            externalIdPUserSettings.TokenEndpoint,
                            externalIdPUserSettings.AuthorizeEndpoint,
                            new IdentityModel.Jwk.JsonWebKeySet(),
                            externalIdPUserSettings.UserInfoEndpoint,
                            externalIdPUserSettings.EndInfoEndpoint,
                            externalIdPUserSettings.Authority,
                            externalIdPUserSettings.ClientId,
                            browser,
                            externalIdPUserSettings.RedirectUriNativeClient,
                            externalIdPUserSettings.Scope,
                            externalIdPUserSettings.ClientSecret);
        }

        public async Task<LoginResult> ExecuteLogin()
        {
            try
            {
                return await _oidcClient.LoginAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(Properties.Resources.OidcClientUnexpectedError + ex.Message);
            }   
        }
        
        public async Task<LogoutResult> ExecuteLogout()
        {
            try
            {
                return await _oidcClient.LogoutAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(Properties.Resources.OidcClientUnexpectedError + ex.Message);
            }
           
        }
    }
}
