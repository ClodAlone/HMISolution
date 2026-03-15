using ExternalAuthentication.Model;
using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;



namespace ExternalAuthentication.Services
{
    public interface ISetOidcClientLoginResultService
    {
        ExternalIdPLoginResult Execute(LoginResult oidcClientLoginResult);
    }

    public class SetOidcClientLoginResultService : ISetOidcClientLoginResultService
    {
        private readonly ICreateUserTokenFromJwtService _createUserTokenFromJwt;
        private readonly IGetRolesListService _getRolesListService;

        public SetOidcClientLoginResultService(
            ICreateUserTokenFromJwtService createUserTokenFromJwt, 
            IGetRolesListService getRolesListService)
        {
            _createUserTokenFromJwt = createUserTokenFromJwt;
            _getRolesListService = getRolesListService;
        }

        public ExternalIdPLoginResult Execute(LoginResult oidcClientLoginResult)
        {
            try
            {
                var name = string.Empty;
                var surname = string.Empty;
                var username = string.Empty;
                var roles = new List<string>();

                foreach (var claim in oidcClientLoginResult.User.Claims)
                {
                    switch (claim.Type.ToLower())
                    {
                        case "given_name":
                            name = claim.Value;
                            break;
                        case "family_name":
                            surname = claim.Value;
                            break;
                        case "email":
                            username = claim.Value;
                            break;
                        case "roles":
                            roles = _getRolesListService.Execute(claim.Value);
                            break;
                    }
                }

                if (string.IsNullOrEmpty(username))
                {
                    return ExternalIdPLoginResult.Create(
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        new List<string>(),
                        false,
                        Properties.Resources.IdPUsernameNotSet);
                }

                return ExternalIdPLoginResult.Create(
                    name,
                    surname,
                    username,
                    roles,
                    true,
                    string.Empty,
                    _createUserTokenFromJwt.Execute(oidcClientLoginResult.IdentityToken));
            }
            catch (Exception ex)
            {
                throw new Exception("Unexpected error while setting the login result: " + ex.Message.ToString());
            }
            
        }
    }
}
