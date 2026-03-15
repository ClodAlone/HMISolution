using Opc.Ua;



namespace AuthenticationCredentialsProvider.Model
{
    public class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public UserIdentity UserIdentity { get; set; }

        public LoginDetails(
            string username,
            string password,
            string role,
            UserIdentity userIdentity)
        {
            Username = username;
            Password = password;
            Role = role;
            UserIdentity = userIdentity;
        }
    }
}
