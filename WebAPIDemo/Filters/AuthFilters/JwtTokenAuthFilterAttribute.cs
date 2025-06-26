using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using WebAPIDemo.Attributes;
using WebAPIDemo.Authority;

namespace WebAPIDemo.Filters.AuthFilters
{
    public class JwtTokenAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                context.Result=new UnauthorizedResult();
                return;
            }
           string tokenString = authorizationHeader.ToString();
            if (tokenString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                tokenString=tokenString.Substring("Beare ".Length).Trim();  
            }
            else
            {
                context.Result=new UnauthorizedResult();
                return;
            }
            var configuration =context.HttpContext.RequestServices.GetService<IConfiguration>();
            var securityKey = configuration?["SecurityKey"] ?? string.Empty;

            //if (!await Authenticator.verifyTokenAsync(tokenString,securityKey))
            //{
            //    context.Result=new UnauthorizedResult();
            //}
            var claims=await Authenticator.verifyTokenAsync(tokenString, securityKey);
            if (claims == null)
            {
                context.Result=new UnauthorizedResult(); //401
            }
            else
            {
               var requiredClaims=context.ActionDescriptor.EndpointMetadata
                    .OfType<RequiredClaimAttribute>().ToList();
                if(requiredClaims!=null && !requiredClaims.All(rc=> claims.
                Any(c=> c.Type.Equals(rc.ClaimType,StringComparison.OrdinalIgnoreCase)&&
                c.Value.Equals(rc.ClaimValue, StringComparison.OrdinalIgnoreCase))))
                {
                    context.Result = new StatusCodeResult(403); //403
                }
                    
            }
        }
    }
}
