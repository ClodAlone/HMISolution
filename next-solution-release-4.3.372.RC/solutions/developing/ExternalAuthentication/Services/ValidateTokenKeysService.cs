using ExternalAuthentication.Model;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExternalAuthentication.Services
{
    public interface IValidateTokenKeysService 
    { 
        Task<bool> Execute(string token, ExternalIdPUserSettings externalIdPUserSettings);
    }
    public class ValidateTokenKeysService : IValidateTokenKeysService
    {
        private readonly IGetAppPublicKeysService _getAppPublicKeysService;

        public ValidateTokenKeysService(IGetAppPublicKeysService getAppPublicKeysService)
        {
            _getAppPublicKeysService = getAppPublicKeysService;
        }

        public async Task<bool> Execute(string token, ExternalIdPUserSettings externalIdPUserSettings)
        {
            try
            {
                var validationResult = false;
                var jwt = new JsonWebToken(token);
                var publicKeys = 
                    await _getAppPublicKeysService.Execute(externalIdPUserSettings);

                if (publicKeys != null && publicKeys.keys != null && publicKeys.keys.Length > 0)
                {
                    var keyFound = publicKeys.keys.Where(publicKey => publicKey.kid == jwt.Kid).FirstOrDefault();

                    if (keyFound != null)
                    {
                        validationResult = true;
                    }
                }

                return validationResult;
            }
            catch(Exception ex)
            {
                throw new Exception(Properties.Resources.ValidateTokenKeysServiceError + ex.Message.ToString()); 
            }   
        }
    }
}
