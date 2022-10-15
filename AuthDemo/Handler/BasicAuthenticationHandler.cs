using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace AuthDemo.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                    return AuthenticateResult.Fail("Auth missing in header");

                var authHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                string tp = AuthenticationHeaderValue.Parse(Request.Headers["Timepass"]).ToString();

                string[]? credentials = null;

                if (authHeaderValue.Parameter != null)
                {
                    var bytes = Convert.FromBase64String(authHeaderValue.Parameter);

                    credentials = Encoding.UTF8.GetString(bytes).Split(":");
                }

                var user = credentials[0];
                var password = credentials[1];

                if (credentials != null && (user != "divij" || password != "kardile"))
                    return AuthenticateResult.Fail("Invalid!");
                else
                {
                    var claim = new[] 
                    { 
                        new Claim(ClaimTypes.Name, user),           new Claim(ClaimTypes.Role, tp) 
                    };

                    var identity = new ClaimsIdentity(claim, Scheme.Name);

                    var principal = new ClaimsPrincipal(identity);

                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Error!!!!!");
            }   
        }
    }
}
