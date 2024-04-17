using adv_Backend_Entrance.Common.Data;
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

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenBlackListRequirment requirement)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<RedisDBContext>();
                var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

                var authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

                if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                {
                    var token = authorizationHeader.Substring("Bearer ".Length);

                    var blacklisted = await db.IsBlackToken(token);

                    if (blacklisted)
                    {
                        context.Fail();
                        httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        context.Succeed(requirement);
                    }
                }
                else
                {
                    context.Fail();
                    httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
        }

    }
}