using ExternalAuthentication.Model;
using Microsoft.AspNetCore.Http;

namespace WebNExTHMI.Services;

public interface IElaborateIncomingPostDataService
{
    ElaborateIncomingPostDataResult Execute(HttpRequest request);
}