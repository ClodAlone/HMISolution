using System;
using System.Collections.Generic;
using System.Linq;



namespace ExternalAuthentication.Services
{
    public interface IGetRolesListService
    {
        List<string> Execute(string roles);
    }

    public class GetRolesListService : IGetRolesListService
    {
        public List<string> Execute(string roles)
        {
            var rolesResult = new List<string>();

            if (string.IsNullOrEmpty(roles) == false)
            {
                if (roles.Contains(','))
                {
                    var rolesToList = roles.Split(',').ToList();

                    rolesToList.ForEach(role =>
                    {
                        var trimmedRole = role.Trim();

                        if(string.IsNullOrEmpty(trimmedRole) == false)
                        {
                            rolesResult.Add(trimmedRole);
                        }
                    });
                }
                else
                {
                    rolesResult.Add(roles);
                }  
            }

            return rolesResult;
        }
    }
}
