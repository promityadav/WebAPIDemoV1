using Newtonsoft.Json;

namespace WebApp.Data
{
    public class JwtToken
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }
        [JsonProperty("expire_at")]
        public DateTime ExpireAt  { get; set; }
    }
}
