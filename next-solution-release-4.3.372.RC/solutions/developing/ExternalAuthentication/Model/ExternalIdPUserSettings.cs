using System;
using System.Runtime.Serialization;

namespace ExternalAuthentication.Model
{
    [DataContract]
    public class ExternalIdPUserSettings
    {
        [DataMember]
        public string AccessTokenValidIssuer { get; set; }
        [DataMember]
        public string AppKeysBaseEndpoint { get; set; }
        [DataMember]
        public string Authority { get; set; }
        [DataMember]
        public string AuthorizeEndpoint { get; set; }
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string EndInfoEndpoint { get; set; }
        [DataMember]
        public bool ExternalIdentityProviderActive { get; set; }
        [DataMember]
        public string LogoutEndpoint { get; set; }
        [DataMember]
        public string Nonce { get; set; }
        [DataMember]
        public string RedirectUriNativeClient { get; set; }
        [DataMember]
        public string ResponseType { get; set; }
        [DataMember]
        public string ReturnUrlAfterLogin { get; set; }
        [DataMember]
        public string ReturnUrlAfterLogout { get; set; }
        [DataMember]
        public string Scope { get; set; }
        [DataMember]
        public string TenantId { get; set; }
        [DataMember]
        public string TokenEndpoint { get; set; }
        [DataMember]
        public string UserInfoEndpoint { get; set; }
        [DataMember]
        public string ValidIssuer { get; set; }
        [DataMember]
        public string ClientSecret { get; set; }


        public ExternalIdPUserSettings() {}

        public ExternalIdPUserSettings(
            string accessTokenValidIssuer, 
            string appKeysBaseEndpoint,
            string authority,
            string authorizeEndpoint,
            string clientId,
            string endInfoEndpoint,
            bool externalIdentityProviderActive,
            string logoutEndpoint,
            string nonce,
            string redirectUriNativeClient,
            string responseType,
            string returnUrlAfterLogin,
            string returnUrlAfterLogout,
            string scope,
            string tenantId,
            string tokenEndpoint,
            string userInfoEndpoint,
            string validIssuer,
            string clientSecret)
        {
            AccessTokenValidIssuer = accessTokenValidIssuer;
            AppKeysBaseEndpoint = appKeysBaseEndpoint;
            Authority = authority;
            AuthorizeEndpoint = authorizeEndpoint;
            ClientId = clientId;
            EndInfoEndpoint = endInfoEndpoint;
            ExternalIdentityProviderActive = externalIdentityProviderActive;
            LogoutEndpoint = logoutEndpoint;
            Nonce = nonce;
            RedirectUriNativeClient = redirectUriNativeClient;
            ResponseType = responseType;
            ReturnUrlAfterLogin = returnUrlAfterLogin;
            ReturnUrlAfterLogout = returnUrlAfterLogout;
            Scope = scope;
            TenantId = tenantId;
            TokenEndpoint = tokenEndpoint;
            UserInfoEndpoint = userInfoEndpoint;
            ValidIssuer = validIssuer;
            ClientSecret = clientSecret;
        }

        public static ExternalIdPUserSettings Create(
            string accessTokenValidIssuer,
            string appKeysBaseEndpoint,
            string authority,
            string authorizeEndpoint,
            string clientId,
            string endInfoEndpoint,
            bool externalIdentityProviderActive,
            string logoutEndpoint,
            string nonce,
            string redirectUriNativeClient,
            string responseType,
            string returnUrlAfterLogin,
            string returnUrlAfterLogout,
            string scope,
            string tenantId,
            string tokenEndpoint,
            string userInfoEndpoint,
            string validIssuer,
            string clientSecret)
        {
            return new ExternalIdPUserSettings(
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
    }
}
