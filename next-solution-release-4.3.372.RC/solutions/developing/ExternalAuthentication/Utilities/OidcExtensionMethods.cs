using IdentityModel;
using IdentityModel.Client;


namespace ExternalAuthentication.Utilities
{
    public static class OidcExtensionMethods
    {
        public static string CreateGetAppPublicKeysUrl(
            RequestUrl requestUrl,
            string clientId)
        {
            var values = new Parameters
            {
                { "appid", clientId }
            };

            return requestUrl.Create(values.Merge(null));
        }

        public static string CreateSignOutUrl(
            string clientId,
            RequestUrl requestUrl,
            string idTokenHint = null,
            string postLogoutRedirectUri = null,
            string state = null,
            Parameters extra = null)
        {
            var values = new Parameters();

            values.AddOptional("client_id", clientId);
            values.AddOptional(OidcConstants.EndSessionRequest.IdTokenHint, idTokenHint);
            values.AddOptional(OidcConstants.EndSessionRequest.PostLogoutRedirectUri, postLogoutRedirectUri);
            values.AddOptional(OidcConstants.EndSessionRequest.State, state);

            return requestUrl.Create(values.Merge(extra));
        }
    }
}
