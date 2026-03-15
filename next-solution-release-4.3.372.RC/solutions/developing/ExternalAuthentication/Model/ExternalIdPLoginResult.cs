using System.Collections.Generic;

namespace ExternalAuthentication.Model
{
    public class ExternalIdPLoginResult
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLoginSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public Opc.Ua.UserIdentity UserIdentity { get; set; }
       

        public ExternalIdPLoginResult(
            string name, 
            string surname, 
            string username,
            List<string> roles, 
            bool isLoginSuccessful, 
            string errorMessage,
            Opc.Ua.UserIdentity userIdentity = null)
        {
            Name = name;
            Surname = surname;
            Username = username;
            Roles = roles;
            IsLoginSuccessful = isLoginSuccessful;
            ErrorMessage = errorMessage;
            UserIdentity = userIdentity;
        }

        public static ExternalIdPLoginResult Create(
            string name, 
            string surname, 
            string username,
            List<string> roles, 
            bool isLoginSuccessful, 
            string errorMessage,
            Opc.Ua.UserIdentity userIdentity = null)
        {
            return new ExternalIdPLoginResult(name, surname, username, roles, isLoginSuccessful, errorMessage, userIdentity);
        }
    }
}
