using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPIDemo.Authority
{
    public static class Authenticator
    {
        public static bool Authenticate(string clientid, string secret)
        {
            var app =AppRepository.GetApplicationByClientId(clientid);
            if (app == null) return false;
                
            return (app.ClientId==clientid && app.Secret==secret);
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
                {"Read",(app?.Scopes??string.Empty).Contains("read")?"true":"false" },
                {"Write",(app?.Scopes??string.Empty).Contains("write")?"true":"false" },
            };
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

        public static async Task<bool> verifyTokenAsync(string tokenString, string securityKey)
        {
            if (string.IsNullOrWhiteSpace(tokenString) || string.IsNullOrWhiteSpace(securityKey))
            {
                return false;
            }
            var keyBytes = System.Text.Encoding.UTF8.GetBytes(securityKey);
            var tokenHandler=new JsonWebTokenHandler();
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
                return result.IsValid;
            }
            catch (SecurityTokenMalformedException)
            {
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
