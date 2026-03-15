


namespace ExternalAuthentication.Services
{
    public interface IValidateResponseStateService
    {
        bool Execute(string originalState, string responseState);
    }

    public class ValidateResponseStateService : IValidateResponseStateService
    {
        public bool Execute(string originalState, string responseState)
        {   
            if (responseState == originalState) 
            { 
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
