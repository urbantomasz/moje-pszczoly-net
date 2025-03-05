using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AllowedEmailsRequirement : IAuthorizationRequirement { }

public class AllowedEmailsAuthorizationHandler : AuthorizationHandler<AllowedEmailsRequirement>
{
    private static readonly List<string> AllowedEmails = new() { "urbantomasz94@gmail.com", "mojepszczolymk@gmail.com" };

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowedEmailsRequirement requirement)
    {
        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (userEmail != null && AllowedEmails.Contains(userEmail))
        {
            context.Succeed(requirement); 
        }
        else
        {
            context.Fail(); 
        }

        return Task.CompletedTask;
    }
}
