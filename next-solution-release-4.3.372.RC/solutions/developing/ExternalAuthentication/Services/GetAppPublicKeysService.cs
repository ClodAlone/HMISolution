using ExternalAuthentication.Model;
using ExternalAuthentication.Utilities;
using IdentityModel.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExternalAuthentication.Services
{
    public interface IGetAppPublicKeysService
    {
        Task<IdpPublicKeys> Execute(ExternalIdPUserSettings externalIdPUserSetting);
    }

    public class GetAppPublicKeysService : IGetAppPublicKeysService
    {
        private readonly HttpClient _httpClient;
        public GetAppPublicKeysService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IdpPublicKeys> Execute(ExternalIdPUserSettings externalIdPUserSetting)
        {
            try
            {
                var publicKeys = new IdpPublicKeys();
                var requestUrl = new RequestUrl(externalIdPUserSetting.AppKeysBaseEndpoint);
                var url = OidcExtensionMethods.CreateGetAppPublicKeysUrl(requestUrl, externalIdPUserSetting.ClientId);
                var response = await _httpClient.GetAsync(url);

                if (response != null && response.Content != null)
                {
                    var responseToString = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(responseToString) == false) 
                    {
                        var keysDeserialized = JsonSerializer.Deserialize<IdpPublicKeys>(responseToString);

                        if (keysDeserialized?.keys?.Count() > 0)
                        {
                            publicKeys = keysDeserialized;
                        }
                    }
                }

                return publicKeys;
            }
            catch(Exception ex)
            {
                throw(new Exception(Properties.Resources.GetAppPublicKeysServiceError + ex.Message.ToString()));
            }
        }
    }
}
