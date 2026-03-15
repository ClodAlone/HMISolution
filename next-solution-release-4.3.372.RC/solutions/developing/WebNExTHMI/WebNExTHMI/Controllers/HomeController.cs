using ExternalAuthentication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using WebNExTHMI.Hubs;
using WebNExTHMI.Services;

namespace WebNExTHMI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<CoreHub> _hubContext;

        public HomeController(
            IHttpContextAccessor httpContextAccessor,
            IHubContext<CoreHub> hubContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ExternalLogin()
        {
            var elaborateIncomingDataService = new ElaborateIncomingPostDataService();
            var request = _httpContextAccessor?.HttpContext?.Request;
            var incomingData = elaborateIncomingDataService.Execute(request);
            
            if (incomingData != null && string.IsNullOrEmpty(incomingData.ErrorMessage)) 
            {
                var guidLength = new Guid().ToString().Length;
                var clientId = incomingData.State.Substring(guidLength);

                _hubContext.Clients.Client(clientId)
                    .SendAsync("externalIdpUserLoggedIn", incomingData.Code, incomingData.State);
                
                return View();
            }
            else
            {
                ViewData["Message"] = incomingData.ErrorMessage;

                return View("LoginProblem");
            }
        }

        public IActionResult ExternalLogout()
        {    
            return View("ExternalLogout");
        }
    }
}
