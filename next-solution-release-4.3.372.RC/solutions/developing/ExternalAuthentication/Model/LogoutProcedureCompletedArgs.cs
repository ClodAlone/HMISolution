using System;


namespace ExternalAuthentication.Model
{
    public class LogoutProcedureCompletedArgs : EventArgs
    {
        public readonly ExternalIdPLogoutResult LogoutResult;

        public LogoutProcedureCompletedArgs(ExternalIdPLogoutResult logoutResult)
        {
            LogoutResult = logoutResult;
        }
    }
}
