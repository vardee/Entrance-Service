using adv_Backend_Entrance.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers
{
    public class TokenHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        public TokenHelper(IConfiguration configuration, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _secretKey = configuration.GetValue<string>("Jwt:Secret");
            _issuer = configuration.GetValue<string>("Jwt:Issuer");
            _audience = configuration.GetValue<string>("Jwt:Audience");
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetTokenFromHeader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                string authorizationHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    return authorizationHeader.Substring("Bearer ".Length);
                }
            }
            return null;
        }
        public string GetTokenFromSession()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                return httpContext.Session.GetString("AccessToken");
            }
            return null;
        }
        public string? GetUserIdFromToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                if (claimsPrincipal is null || !(validatedToken is JwtSecurityToken jwtSecurityToken))
                    return null;

                var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

                return emailClaim?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
