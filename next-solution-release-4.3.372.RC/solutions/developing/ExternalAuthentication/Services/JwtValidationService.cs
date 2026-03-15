using ExternalAuthentication.Model;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;

namespace ExternalAuthentication.Services
{
    public interface IJwtValidationService
    {
        TokenValidationResult Execute(string token, ExternalIdPUserSettings externalIdPUserSettings);
    }
    public class JwtValidationService : IJwtValidationService
    {
        public TokenValidationResult Execute(string token, ExternalIdPUserSettings externalIdPUserSettings)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters();

                tokenValidationParameters.ValidAudience = externalIdPUserSettings.ClientId;
                tokenValidationParameters.ValidIssuer = externalIdPUserSettings.ValidIssuer;
                tokenValidationParameters.ValidateLifetime = true;
                tokenValidationParameters.ValidateIssuerSigningKey = false;
                tokenValidationParameters.SignatureValidator = OverrideStandardKeysValidator;

                var jsonWebTokenHandler = new JsonWebTokenHandler();
                return jsonWebTokenHandler.ValidateToken(token, tokenValidationParameters);
            }
            catch(Exception ex)
            {
                throw new Exception(Properties.Resources.JsonWebTokenHandlerError + ex.Message.ToString());
            }
        }

        public static JsonWebToken OverrideStandardKeysValidator(string token, TokenValidationParameters parameters)
        {
            var jwt = new JsonWebToken(token);
            return jwt;
        }
    }
}

