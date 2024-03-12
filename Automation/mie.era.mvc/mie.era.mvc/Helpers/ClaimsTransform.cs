using Microsoft.AspNetCore.Authentication;
using mie.era.mvc.Models;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace mie.era.mvc.Helpers
{
    public class CustomClaimsTransformer : IClaimsTransformation
    {
        private readonly IConfiguration _config;

        public CustomClaimsTransformer(IConfiguration config)
        {
            _config = config;
        }

        private AuthenticateResponse LoadToken(string username)
        {
            var handler = new HttpClientHandler
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };

            var client = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Post, _config.GetConnectionString("AuthAPIEndpoint").ToString() + username);

            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var resultString = response.Content.ReadAsStringAsync().Result;
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var authenticateResponse = System.Text.Json.JsonSerializer.Deserialize<AuthenticateResponse>(resultString, options);

                return authenticateResponse;
            }
            return null;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            // Clone current identity
            var clone = principal.Clone();
            var newIdentity = (ClaimsIdentity)clone.Identity;

            // Support AD and local accounts
            var nameId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier ||
                                                              c.Type == ClaimTypes.Name);
            if (nameId == null)
            {
                return principal;
            }

            // Get user from database
            var userResponse = LoadToken(nameId.Value);
            if (userResponse == null)
            {
                return principal;
            }

            var claim = new Claim("User", JsonSerializer.Serialize(userResponse.Person));
            newIdentity.AddClaim(claim);
            claim = new Claim("Token", userResponse.Token);
            newIdentity.AddClaim(claim);
            return clone;
        }
    }
}
