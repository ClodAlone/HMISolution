using ExternalAuthentication.Model;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;


namespace ExternalAuthentication.Services
{
    public interface ISetLoginResultFromTokensService
    {
        ExternalIdPLoginResult Execute(string idToken, string accessToken);
    }

    public class SetLoginResultFromTokensService : ISetLoginResultFromTokensService
    {
        private readonly ICreateUserTokenFromJwtService _createUserTokenFromJwt;
        private readonly IGetRolesListService _getRolesListService;
        

        public SetLoginResultFromTokensService(
            ICreateUserTokenFromJwtService createUserTokenFromJwt,
            IGetRolesListService getRolesListService)
        {
            _createUserTokenFromJwt = createUserTokenFromJwt;
            _getRolesListService = getRolesListService;
        }

        public ExternalIdPLoginResult Execute(string idToken, string accessToken)
        {
            try
            {
                var name = string.Empty;
                var surname = string.Empty;
                var username = string.Empty;
                var roles = new List<string>();

                var jwtIdToken = new JsonWebToken(idToken);
                var jwtAccessToken = new JsonWebToken(accessToken);

                foreach (var claim in jwtIdToken.Claims)
                {
                    switch (claim.Type.ToLower())
                    {
                        case "email":
                            username = claim.Value;
                            break;
                        case "roles":
                            roles = _getRolesListService.Execute(claim.Value);
                            break;
                    }
                }

                foreach (var claim in jwtAccessToken.Claims)
                {
                    switch (claim.Type.ToLower())
                    {
                        case "given_name":
                            name = claim.Value;
                            break;
                        case "family_name":
                            surname = claim.Value;
                            break;
                    }
                }

                return ExternalIdPLoginResult.Create(
                    name,
                    surname,
                    username,
                    roles,
                    true,
                    string.Empty,
                    _createUserTokenFromJwt.Execute(idToken));
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while setting the login result: " + ex.Message.ToString());
            }
        }
    }
}
