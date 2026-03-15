using Opc.Ua;
using System;
using System.Text;


namespace ExternalAuthentication.Services
{
    public interface ICreateUserTokenFromJwtService
    {
        UserIdentity Execute(string idToken);
    }
    public class CreateUserTokenFromJwtService : ICreateUserTokenFromJwtService
    {
        public UserIdentity Execute(string idToken)
        {
            try
            {
                var jwtSecurityToken = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(idToken);
                IssuedIdentityToken issuedToken = new IssuedIdentityToken();

                issuedToken.IssuedTokenType = IssuedTokenType.JWT;
                issuedToken.DecryptedTokenData =
                    new UTF8Encoding(false).GetBytes((jwtSecurityToken).RawData);

                var userIdentity = new UserIdentity(issuedToken);

                return userIdentity;
            }
            catch(Exception ex)
            {
                throw new Exception("Unexpected error while setting the user identity: " + ex.Message.ToString());
            }
        }
    }
}
