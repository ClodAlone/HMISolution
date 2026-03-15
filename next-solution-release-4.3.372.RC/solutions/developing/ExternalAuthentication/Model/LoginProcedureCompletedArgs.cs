using System;


namespace ExternalAuthentication.Model
{
    public class LoginProcedureCompletedArgs : EventArgs
    {

        public readonly ExternalIdPLoginResult LoginResult;

        public LoginProcedureCompletedArgs(ExternalIdPLoginResult loginResult)
        {
            LoginResult = loginResult;
        }
    }
}
