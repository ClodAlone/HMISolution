



namespace ExternalAuthentication.Model
{
    public class IdpResponseValidatonOnWebHMI
    {
        public bool ValidResponse { get; set; }
        public string ErrorMessage { get; set; }

        public IdpResponseValidatonOnWebHMI(bool validResponse, string errorMessage)
        {
            ValidResponse = validResponse;
            ErrorMessage = errorMessage;
        }

        public static IdpResponseValidatonOnWebHMI Create(bool validResponse, string errorMessage)
        {
            return new IdpResponseValidatonOnWebHMI(validResponse, errorMessage);
        }
    }
}

