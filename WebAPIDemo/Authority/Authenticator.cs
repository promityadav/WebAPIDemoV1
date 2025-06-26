using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using System.Security.Claims;
using WebAPIDemo.Data;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPIDemo.Authority
{
    public static class Authenticator
    {
       
        public static bool Authenticate(string clientid, string secret)
        {
            var app = AppRepository.GetApplicationByClientId(clientid);
            if (app == null) return false;

            return (app.ClientId == clientid && app.Secret == secret);
        }
        public static string CreateToken(string clientId, DateTime expireAt, string strSecretKey)
        {
            //Algorith
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(strSecretKey)), SecurityAlgorithms.HmacSha256Signature);
            //PayLoads(claims)
            var app = AppRepository.GetApplicationByClientId(clientId);
            var claimsDictionary = new Dictionary<string, object>
            {
                {"AppName",app?.ApplicationName??string.Empty },
                // {"Read",(app?.Scopes??string.Empty).Contains("read")?"true":"false" },
                //{"Write",(app?.Scopes??string.Empty).Contains("write")?"true":"false" },
            };
            var scopes = app?.Scopes?.Split(',') ?? Array.Empty<string>();
            if (scopes.Length > 0)
            {
                foreach (var scope in scopes)
                {
                    claimsDictionary.Add(scope.Trim().ToLower(), "true");

                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expireAt,
                NotBefore = DateTime.UtcNow,

            };
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public static async Task<IEnumerable<Claim>?> verifyTokenAsync(string tokenString, string securityKey)
        {
            if (string.IsNullOrWhiteSpace(tokenString) || string.IsNullOrWhiteSpace(securityKey))
            {
                return null;
            }
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(securityKey);
            var tokenHandler = new JsonWebTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                var result = await tokenHandler.ValidateTokenAsync(tokenString, validationParameters);
                if (result.SecurityToken != null)
                {
                    var tokenObject = tokenHandler.ReadJsonWebToken(tokenString);
                    return tokenObject.Claims ?? Enumerable.Empty<Claim>();
                }
                else {
                    return null;
                }
            }
            catch (SecurityTokenMalformedException)
            {
                return null;
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
