using ExternalAuthentication.Model;
using System.Threading.Tasks;

namespace WebNExTHMI.Services
{
    public interface IExchangeAuthCodeWithTokensService
    {
        public Task<IdPTokenResponse> Execute(
           string tokenEndpoint,
           string clientId,
           string redirectUri,
           string code,
           string codeVerifier,
           string clientSecret);
    }
}
