using Microsoft.AspNetCore.Authentication;
using Microsoft.VisualBasic;
using mie.era.mvc.Models;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Diagnostics;
using System.Net;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;

namespace mie.era.mvc.Helpers
{
    public static class ExtensionMethods
    {
        public static InternalPerson GetAuthUser(this HttpContext context)
        {
            if ((context.User != null) && (context.User.Identity.IsAuthenticated))
            {
                var principal = context.User as ClaimsPrincipal;
                Console.WriteLine("Claims count " + principal.Claims.Count());
                var userClaim = principal.Claims.SingleOrDefault(c => c.Type == "User");
                if (userClaim != null)
                {
                    Console.WriteLine("Claims NOT null");
                    return JsonConvert.DeserializeObject<InternalPerson>(userClaim.Value);
                }
                else
                {
                    Console.WriteLine("Claims null");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static string GetToken(this HttpContext context)
        {
            if ((context.User != null) && (context.User.Identity.IsAuthenticated))
            {
                var principal = context.User as ClaimsPrincipal;
                var userClaim = principal?.Claims.SingleOrDefault(c => c.Type == "Token");
                if (userClaim != null)
                {
                    return userClaim.Value;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return "";
            }
        }
    }
}
