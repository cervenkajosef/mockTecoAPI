using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace mockTecoAPI
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
         IOptionsMonitor<AuthenticationSchemeOptions> options,
         ILoggerFactory logger,
         UrlEncoder encoder,
         ISystemClock clock)
         : base(options, logger, encoder, clock)
        {
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Session token should be present and in GUID format");
            }

            string authHeader = Request.Headers["Authorization"];

            if (authHeader == "Basic YWRtaW46YWRtaW4=")
            {
                var identity = new ClaimsIdentity("Basic");
                identity.AddClaim(new Claim(ClaimTypes.Name, "admin"));
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), "Basic");
                return AuthenticateResult.Success(ticket);
            }
            Request.Headers.Add("Server", "TecoApi/1.0.1 (F2x CP2007I v2.3.058 N8 0105)");
            Request.ContentType = "application/json";
            Request.Headers.Add("Cache-Control", "no-cache");

            return AuthenticateResult.Fail("Unauthorized");
        }

    }

}
