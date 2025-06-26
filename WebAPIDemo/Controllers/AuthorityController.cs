using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebAPIDemo.Authority;
using WebAPIDemo.Data;
using WebAPIDemo.Models;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext db;
        public AuthorityController(IConfiguration configuration, ApplicationDbContext db)
        {
            this.configuration = configuration;
            this.db = db;
        }
        [HttpPost("auth")]
        public IActionResult Authenticate([FromBody] AppCredential credential)
        {

            var Application = db.Applications.FirstOrDefault(u => u.ClientId == credential.ClientId && u.Secret == credential.Secret);
            if (Application == null)
            {
                ModelState.AddModelError("Unauthorized", "You are not authorised.");
                var problemDetails = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status401Unauthorized
                };
                return new UnauthorizedObjectResult(problemDetails);
            }
            else
            {
                
                var expierAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = Authenticator.CreateToken(credential.ClientId, expierAt, configuration["SecurityKey"] ?? String.Empty),
                    expires_at = expierAt
                });
            }

            //if (Authenticator.Authenticate(credential.ClientId, credential.Secret))
            //{
            //    var expierAt = DateTime.UtcNow.AddMinutes(1);
            //    return Ok(new
            //    {
            //        access_token = Authenticator.CreateToken(credential.ClientId, expierAt, configuration["SecurityKey"] ?? String.Empty),
            //        expires_at = expierAt
            //    });
            //}
            //else
            //{
            //    ModelState.AddModelError("Unauthorized", "You are not authorised.");
            //    var problemDetails = new ValidationProblemDetails(ModelState)
            //    {
            //        Status = StatusCodes.Status401Unauthorized

            //    };
            //    return new UnauthorizedObjectResult(problemDetails);
            //}
        }



    }
}
