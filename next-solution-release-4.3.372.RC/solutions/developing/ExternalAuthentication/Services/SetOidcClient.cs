using IdentityModel.Jwk;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;



namespace ExternalAuthentication.Services
{
    public interface ISetOidcClient
    {
        OidcClient Execute(
            string issuerName, 
            string tokenEndpoint, 
            string authorizeEndpoint,
            JsonWebKeySet keySet,
            string userInfoEndpoint,
            string endSessionEndpoint,
            string authority,
            string clientId,
            IBrowser webBrowser,
            string redirectUri,
            string scope,
            string secret);
    }
    public class SetOidcClient : ISetOidcClient
    {
        public OidcClient Execute(
            string issuerName, 
            string tokenEndpoint, 
            string authorizeEndpoint, 
            JsonWebKeySet keySet, 
            string userInfoEndpoint, 
            string endSessionEndpoint, 
            string authority, 
            string clientId, 
            IBrowser webBrowser, 
            string redirectUri, 
            string scope,
            string secret)
        {
            var providerInformation = new ProviderInformation()
            {
                IssuerName = issuerName,
                TokenEndpoint = tokenEndpoint,
                AuthorizeEndpoint = authorizeEndpoint,
                KeySet = keySet,
                UserInfoEndpoint = userInfoEndpoint,
                EndSessionEndpoint = endSessionEndpoint
            };

            var options = new OidcClientOptions
            {
                Authority = authority,
                ProviderInformation = providerInformation,
                ClientId = clientId,
                Browser = webBrowser,
                RedirectUri = redirectUri,
                Scope = scope,
                LoadProfile = true,
                ClientSecret = secret
            };

            return new OidcClient(options);
        }
    }
}
