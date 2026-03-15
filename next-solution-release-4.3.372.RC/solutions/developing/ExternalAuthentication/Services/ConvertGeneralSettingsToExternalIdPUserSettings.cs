using ExternalAuthentication.Model;
using System;
using System.Collections.Generic;

namespace ExternalAuthentication.Services
{
    public interface IConvertGeneralSettingsToExternalIdPUserSettings
    {
        ExternalIdPUserSettings Execute(Dictionary<String, String> settingsDictionary);
    }

    public class ConvertGeneralSettingsToExternalIdPUserSettings : IConvertGeneralSettingsToExternalIdPUserSettings
    {
        public ExternalIdPUserSettings Execute(Dictionary<String, String> settingsDictionary)
        {
            try
            {
                string accessTokenValidIssuer;
                string appKeysBaseEndpoint;
                string authority;
                string authorizeEndpoint;
                string clientId;
                string endInfoEndpoint;
                bool externalIdentityProviderActive;
                string externalIdentityProviderActiveString;
                string logoutEndpoint;
                string nonce;
                string redirectUriNativeClient;
                string responseType;
                string returnUrlAfterLogin;
                string returnUrlAfterLogout;
                string scope;
                string tenantId;
                string tokenEndpoint;
                string userInfoEndpoint;
                string validIssuer;
                string clientSecret;

                settingsDictionary.TryGetValue("AccessTokenValidIssuer", out accessTokenValidIssuer);
                settingsDictionary.TryGetValue("AppKeysBaseEndpoint", out appKeysBaseEndpoint);
                settingsDictionary.TryGetValue("Authority", out authority);
                settingsDictionary.TryGetValue("AuthorizeEndpoint", out authorizeEndpoint);
                settingsDictionary.TryGetValue("ClientId", out clientId);
                settingsDictionary.TryGetValue("EndInfoEndpoint", out endInfoEndpoint);
                settingsDictionary.TryGetValue("ExternalIdentityProviderActive", out externalIdentityProviderActiveString);
                settingsDictionary.TryGetValue("LogoutEndpoint", out logoutEndpoint);
                settingsDictionary.TryGetValue("Nonce", out nonce);
                settingsDictionary.TryGetValue("RedirectUriNativeClient", out redirectUriNativeClient);
                settingsDictionary.TryGetValue("ResponseType", out responseType);
                settingsDictionary.TryGetValue("ReturnUrlAfterLogin", out returnUrlAfterLogin);
                settingsDictionary.TryGetValue("ReturnUrlAfterLogout", out returnUrlAfterLogout);
                settingsDictionary.TryGetValue("Scope", out scope);
                settingsDictionary.TryGetValue("TenantId", out tenantId);
                settingsDictionary.TryGetValue("TokenEndpoint", out tokenEndpoint);
                settingsDictionary.TryGetValue("UserInfoEndpoint", out userInfoEndpoint);
                settingsDictionary.TryGetValue("ValidIssuer", out validIssuer);
                settingsDictionary.TryGetValue("ClientSecret", out clientSecret);

                externalIdentityProviderActive = bool.Parse(externalIdentityProviderActiveString);

                return ExternalIdPUserSettings.Create(
                    accessTokenValidIssuer,
                    appKeysBaseEndpoint,
                    authority,
                    authorizeEndpoint,
                    clientId,
                    endInfoEndpoint,
                    externalIdentityProviderActive,
                    logoutEndpoint,
                    nonce,
                    redirectUriNativeClient,
                    responseType,
                    returnUrlAfterLogin,
                    returnUrlAfterLogout,
                    scope,
                    tenantId,
                    tokenEndpoint,
                    userInfoEndpoint,
                    validIssuer,
                    clientSecret);
            }
            catch
            {
                throw new Exception(Properties.Resources.ConvertGeneralSettingsToExternalIdPUserSettingsError);
            }    
        }
    }
}
