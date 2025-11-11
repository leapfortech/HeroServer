using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HeroServer.Security
{
    public class ApiKeyRequirement : IAuthorizationRequirement
    {
    }

    public class ApiKeyHandler : AuthorizationHandler<ApiKeyRequirement>
    {
        public const String API_KEY_HEADER_NAME = "X-API-KEY";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            SucceedRequirementIfApiKeyPresentAndValid(context, requirement);
            return Task.CompletedTask;
        }

        private static void SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                String apiKey = authorizationFilterContext.HttpContext.Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
                if (apiKey != null && apiKey == "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA")
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
