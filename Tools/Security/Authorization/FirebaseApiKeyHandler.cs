using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace HeroServer.Security
{
    public class FirebaseApiKeyRequirement : IAuthorizationRequirement
    {
    }

    public class AccessFirebaseHandler : AuthorizationHandler<FirebaseApiKeyRequirement>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirebaseApiKeyRequirement requirement)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            ClaimsPrincipal user = context.User;

            if (user != null && user.Claims != null)
            {
                Claim firebaseClaim = user.Claims.FirstOrDefault(c => c.Type == "firebase");
                FirebaseUserInfo firebaseUserInfo = null;

                if (firebaseClaim != null && firebaseClaim.Value != null)
                {
                    firebaseUserInfo = JsonConvert.DeserializeObject<FirebaseUserInfo>(firebaseClaim.Value);

                    if (firebaseUserInfo != null)
                        Debug.WriteLine(firebaseUserInfo.SignInProvider);

                    // Do some custom checks: call context.Succeed() if user is OK
                    context.Succeed(requirement);

                    //context.Fail();
                }
            }
        }
    }

    public class AccessApiKeyHandler : AuthorizationHandler<FirebaseApiKeyRequirement>
    {
        public const String API_KEY_HEADER_NAME = "X-API-KEY";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FirebaseApiKeyRequirement requirement)
        {
            SucceedRequirementIfApiKeyPresentAndValid(context, requirement);
            return Task.CompletedTask;
        }

        private static void SucceedRequirementIfApiKeyPresentAndValid(AuthorizationHandlerContext context, FirebaseApiKeyRequirement requirement)
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
