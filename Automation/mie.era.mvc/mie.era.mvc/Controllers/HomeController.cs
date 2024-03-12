using Microsoft.AspNetCore.Mvc;
using mie.era.mvc.Interfaces;
using Microsoft.AspNetCore.Authorization;
using mie.era.mvc.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using mie.era.mvc.Models;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using System.Security;


namespace mie.era.mvc.Controllers
{
    //Add below if you want to restrict domain access
    //[Authorize(Policy = "RequireIntranetAccess")]
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IHomeService _homeService;
        private readonly IConfiguration _config;
        private MyLogger _logger;

        public HomeController(IConfiguration config, IHomeService homeService) 
        {
            _config = config;
            _homeService = homeService;
            _logger = new MyLogger();    

        }

        //private async Task<AuthenticateResponse> Authenticate()
        //{

        //    _logger.Log("In authenticate");

        //    var user = HttpContext.User;
        //    if (user.Identity.IsAuthenticated && user.Identity is WindowsIdentity windowsIdentity)
        //    {
        //        SecureString emptyPassword = new SecureString();
        //        // Create a HttpClientHandler and set its credentials
        //        var handler = new HttpClientHandler
        //        {
        //            Credentials = CredentialCache.DefaultNetworkCredentials
        //        };

        //        var client = new HttpClient(handler);
        //        var request = new HttpRequestMessage(HttpMethod.Post, _config.GetConnectionString("AuthAPIEndpoint").ToString() + windowsIdentity.Name);
        //        _logger.Log("In authenticate 2");

        //        var response = client.SendAsync(request).Result;
        //        _logger.Log("In authenticate 3 " + response.Content.ReadAsStringAsync().Result);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            _logger.Log("In authenticate 4");
        //            var resultString = response.Content.ReadAsStringAsync().Result;
        //            var options = new JsonSerializerOptions
        //            {
        //                PropertyNameCaseInsensitive = true
        //            };
        //            var authenticateResponse = JsonSerializer.Deserialize<AuthenticateResponse>(resultString, options);

        //            var principal = HttpContext.User as ClaimsPrincipal;
        //            var identity = (ClaimsIdentity)principal.Identity;
        //            identity.AddClaim(new Claim("User", JsonSerializer.Serialize(authenticateResponse.Person)));
        //            identity.AddClaim(new Claim("Token", authenticateResponse.Token));

        //            return authenticateResponse;
        //        }
        //        else return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           try
            {
                var viewModel = await _homeService.GetDashboardData(HttpContext);
                return View(viewModel);

            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}
