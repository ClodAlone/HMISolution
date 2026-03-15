using ExternalAuthentication.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebNExTHMI.Services
{
    public class ExchangeAuthCodeWithTokensService : IExchangeAuthCodeWithTokensService
    {
        private readonly HttpClient _httpClient;
        public ExchangeAuthCodeWithTokensService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IdPTokenResponse> Execute(
            string tokenEndpoint, 
            string clientId, 
            string redirectUri,
            string code, 
            string codeVerifier,
            string clientSecret)
        {
            try
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", clientId },
                    { "redirect_uri", redirectUri },
                    { "code", code },
                    { "code_verifier", codeVerifier },
                    { "scope", "openid" },
                    { "client_secret", clientSecret }
                };

                var postContent = new FormUrlEncodedContent(values);
                var idpResponse = await _httpClient.PostAsync(tokenEndpoint, postContent);
                var idpResponseToString = await idpResponse.Content.ReadAsStringAsync();
                var deserializedResponse = JsonSerializer.Deserialize<IdPTokenResponse>(idpResponseToString);

                return deserializedResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
