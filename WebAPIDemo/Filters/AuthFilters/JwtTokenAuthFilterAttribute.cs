using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

            if (!await Authenticator.verifyTokenAsync(tokenString,securityKey))
            {
                context.Result=new UnauthorizedResult();
            }
        }
    }
}
