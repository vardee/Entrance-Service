using adv_Backend_Entrance.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace adv_Backend_Entrance.Common.Helpers.TokenRequirment
{
    public class TokenInBlackListHandler : AuthorizationHandler<TokenBlackListRequirment>
    {
        private readonly IServiceProvider _serviceProvider;

        public TokenInBlackListHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenBlackListRequirment requirement)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IAuthDbContext>();

                string authorizationHeader = _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length);

                    var blackToken = db.BlackListTokens.FirstOrDefault(b => b.BadToken == token);

                    if (blackToken != null)
                    {
                        context.Fail();
                        _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    context.Fail();
                    _serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }

            return Task.CompletedTask;
        }
    }
}
