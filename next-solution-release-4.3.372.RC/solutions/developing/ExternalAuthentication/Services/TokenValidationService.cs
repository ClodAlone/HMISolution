using ExternalAuthentication.Model;
using System;
using System.Threading.Tasks;


namespace ExternalAuthentication.Services
{
    public interface ITokenValidationService
    {
        Task<bool> Execute(string token, ExternalIdPUserSettings externalIdPUserSettings);
    }

    public class TokenValidationService : ITokenValidationService
    {
        private readonly IValidateTokenKeysService _validateTokenKeysService;
        private readonly IJwtValidationService _jswValidateTokenService;

        public TokenValidationService(
            IValidateTokenKeysService validateTokenKeysService,
            IJwtValidationService jswValidateTokenService)
        {
            _validateTokenKeysService = validateTokenKeysService;
            _jswValidateTokenService = jswValidateTokenService;
        }

        public async Task<bool> Execute(string token, ExternalIdPUserSettings externalIdPUserSettings)
        {
            try
            {
                var jwtTokenValidationResult = _jswValidateTokenService.Execute(token, externalIdPUserSettings);

                var keysValidationResult = await _validateTokenKeysService.Execute(token, externalIdPUserSettings);

                if (jwtTokenValidationResult.IsValid == false || keysValidationResult == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                throw new Exception(Properties.Resources.TokenValidationError + ex.Message);
            }   
        }
    }
}
