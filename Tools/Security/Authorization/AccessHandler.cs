using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Security.Claims;
using Newtonsoft.Json;

namespace HeroServer.Security
{
    public class AccessRequirement : IAuthorizationRequirement
    {
    }

    public class AccessHandler : AuthorizationHandler<AccessRequirement>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirement requirement)
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
}
