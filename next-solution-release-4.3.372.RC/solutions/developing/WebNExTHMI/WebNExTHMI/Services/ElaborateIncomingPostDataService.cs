using System;
using ExternalAuthentication.Model;
using Microsoft.AspNetCore.Http;


namespace WebNExTHMI.Services;

public class ElaborateIncomingPostDataService : IElaborateIncomingPostDataService
{
    public ElaborateIncomingPostDataResult Execute(HttpRequest request)
    {
        try
        {
            var code = string.Empty;
            var incomingResponseState = string.Empty;
            var errorMessage = ExternalAuthentication.Properties.Resources.ElaborateIncomingPostDataError;

            if (request != null && request.Method == "POST")
            {
                request.ReadFormAsync();
                var form = request.Form;

                if (form != null &&
                    string.IsNullOrEmpty(form["code"]) == false &&
                    string.IsNullOrEmpty(form["state"]) == false)
                {
                    code = form["code"];
                    incomingResponseState = form["state"];
                    errorMessage = string.Empty;
                }
            }

            return ElaborateIncomingPostDataResult.Create(code, incomingResponseState, errorMessage);
        }
        catch (Exception ex)
        {
            throw new Exception(ExternalAuthentication.Properties.Resources.ElaborateIncomingPostDataError + ex.Message);
        }
    }
}