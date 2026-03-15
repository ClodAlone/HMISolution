



namespace ExternalAuthentication.Model
{
    public class ElaborateIncomingPostDataResult
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string ErrorMessage { get; set; }

        public ElaborateIncomingPostDataResult(string code, string state, string errorMessage)
        {
            Code = code;
            State = state;
            ErrorMessage = errorMessage;
        }

        public static ElaborateIncomingPostDataResult Create(string code, string state, string errorMessage)
        {
            return new ElaborateIncomingPostDataResult(code, state, errorMessage);
        }

    }
}
