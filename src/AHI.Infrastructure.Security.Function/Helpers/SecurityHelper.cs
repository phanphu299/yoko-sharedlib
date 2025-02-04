using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AHI.Infrastructure.Security.Helper
{
    // Ref: https://blog.wille-zone.de/post/secure-azure-functions-with-jwt-token/
    public static class SecurityHelper
    {
        private static readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

        static SecurityHelper()
        {
            var issuer = Environment.GetEnvironmentVariable("Authentication__Authority").Trim('/');
            var documentRetriever = new HttpDocumentRetriever();
            documentRetriever.RequireHttps = issuer.StartsWith("https://");
            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}/.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public static async Task<ClaimsPrincipal> ValidateTokenAsync(AuthenticationHeaderValue value)
        {
            if (value?.Scheme != "Bearer")
            {
                return null;
            }

            var config = await _configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameter = new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                ValidIssuer = "idp",
                ValidateAudience = false,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys
            };

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(value.Parameter, validationParameter, out var token);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    _configurationManager.RequestRefresh();
                    tries++;
                }
                catch (System.Exception)
                {
                    break;
                }
            }
            return result;
        }

        public static async Task<bool> AuthenticateRequestAsync(HttpRequestMessage req, IConfiguration configuration)
        {
            var query = req.RequestUri.ParseQueryString();
            var code = query["code"];
            var configAuthorizationCode = configuration["AuthorizationCode"];
            bool isAuthenticated = false;
            if (configAuthorizationCode != null && string.Equals(code, configuration["AuthorizationCode"], StringComparison.InvariantCultureIgnoreCase))
            {
                isAuthenticated = true;
            }
            else
            {
                // fallback to the header
                ClaimsPrincipal principal = await ValidateTokenAsync(req.Headers.Authorization);
                if (principal != null)
                {
                    isAuthenticated = true;
                }
            }
            return isAuthenticated;
        }
    }
}