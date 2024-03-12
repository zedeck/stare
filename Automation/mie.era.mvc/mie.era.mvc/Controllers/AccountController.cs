using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using NuGet.Protocol.Plugins;
using System.Net.Http;
using System.Security.Claims;
using mie.era.mvc.Models;
using System.Net;
using System.Text.Json;
using mie.era.mvc.Helpers;
using System.Security.Principal;

namespace mie.era.mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private MyLogger _logger;

        public AccountController(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
            _logger = new MyLogger();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }

        private async Task<AuthenticateResponse> Authenticate()
        {

            _logger.Log("In authenticate");
            //var credentialCache = new CredentialCache();
            //credentialCache.Add(new Uri(_config.GetConnectionString("AuthAPIEndpoint").ToString()), "Negotiate", CredentialCache.DefaultNetworkCredentials);
            //HttpClientHandler handler = handler = new HttpClientHandler();
            //handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //handler.Credentials = credentialCache;// CredentialCache.DefaultNetworkCredentials;
            //handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

            var user = HttpContext.User;
            if (user.Identity.IsAuthenticated && user.Identity is WindowsIdentity windowsIdentity)
            {
                // Create a HttpClientHandler and set its credentials
                var handler = new HttpClientHandler
                {
                    Credentials = new NetworkCredential(windowsIdentity.Name, "", windowsIdentity.AuthenticationType)
                };



                var client = new HttpClient(handler);
                var request = new HttpRequestMessage(HttpMethod.Post, _config.GetConnectionString("AuthAPIEndpoint").ToString());
                _logger.Log("In authenticate 2");

                var response = client.SendAsync(request).Result;
                _logger.Log("In authenticate 3 " + response.Content.ReadAsStringAsync().Result);

                if (response.IsSuccessStatusCode)
                {
                    _logger.Log("In authenticate 4");
                    var resultString = response.Content.ReadAsStringAsync().Result;
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var authenticateResponse = JsonSerializer.Deserialize<AuthenticateResponse>(resultString, options);


                    var claims = new List<Claim>
                    {
                        //new Claim(ClaimTypes.Name, authenticateResponse.Person.Name),
                        new Claim("User", JsonSerializer.Serialize(authenticateResponse.Person)),
                        new Claim("Token", authenticateResponse.Token)
                    };

                    var authProperties = new AuthenticationProperties
                    {
                    };

                    var claimsIdentity = new ClaimsIdentity(
                                claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                                CookieAuthenticationDefaults.AuthenticationScheme,
                                new ClaimsPrincipal(claimsIdentity),
                                authProperties);

                    return authenticateResponse;
                }
                else return null;
            } else
            {
                return null;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {

            AuthenticateResponse response = await Authenticate();

            if (response != null)
            {
                return Redirect("~/Home/Index");
            } else
                return Redirect("~/Account/Forbidden");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Logout()
        {

            if ((HttpContext.User != null) && (HttpContext.User.Identity.IsAuthenticated))
            {
                await HttpContext.SignOutAsync();
            }
            return Redirect("~/Account/Forbidden");
        }
    }
}
