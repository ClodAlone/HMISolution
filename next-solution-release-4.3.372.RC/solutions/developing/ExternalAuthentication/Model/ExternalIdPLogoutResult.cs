


namespace ExternalAuthentication.Model
{
    public class ExternalIdPLogoutResult
    {
        public bool IsLogoutSuccessful { get; set; }
        public string ErrorMessage { get; set; }

        public ExternalIdPLogoutResult(bool isLogoutSuccessful, string errorMessage)
        {
            IsLogoutSuccessful = isLogoutSuccessful;
            ErrorMessage = errorMessage;
        }

        public static ExternalIdPLogoutResult Create(bool isLogoutSuccessful, string errorMessage)
        {
            return new ExternalIdPLogoutResult(isLogoutSuccessful, errorMessage);
        }
    }
}

