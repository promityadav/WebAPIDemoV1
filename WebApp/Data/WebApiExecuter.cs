using Newtonsoft.Json;
using System.Text.Json;

namespace WebApp.Data
{
    public class WebApiExecuter : IWebApiExecuter
    {
        private const string ApiName = "ShirtsApi";
        private const string AuthApiName = "AuthorityApi";

        private readonly IHttpClientFactory httpClientFactory;

        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WebApiExecuter(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpClientFactory = httpClientFactory;
            Configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<T?> InvokeGet<T>(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(ApiName);
            await AddJwtToheader(httpClient);
            var request = new HttpRequestMessage(HttpMethod.Get, relativeUrl);
            var response = await httpClient.SendAsync(request);
            await HandlePotentialError(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task<T?> InvokePost<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(ApiName);
            await AddJwtToheader(httpClient);

            var response = await httpClient.PostAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);
            return await response.Content.ReadFromJsonAsync<T>();
        }
        public async Task InvokePut<T>(string relativeUrl, T obj)
        {
            var httpClient = httpClientFactory.CreateClient(ApiName);
            await AddJwtToheader(httpClient);

            var response = await httpClient.PutAsJsonAsync(relativeUrl, obj);
            await HandlePotentialError(response);

        }
        public async Task InvokeDelete(string relativeUrl)
        {
            var httpClient = httpClientFactory.CreateClient(ApiName);
            await AddJwtToheader(httpClient);

            var response = await httpClient.DeleteAsync(relativeUrl);

            await HandlePotentialError(response);


        }
        private async Task HandlePotentialError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                throw new WebApiException(errorJson);
            }
        }
        private async Task AddJwtToheader(HttpClient httpClient)
        {
            JwtToken? token = null;
            string? strToken = httpContextAccessor.HttpContext?.Session.GetString("access_token");
            if(!string.IsNullOrWhiteSpace(strToken))
            {
                token= JsonConvert.DeserializeObject<JwtToken>(strToken);
            }
            if (token == null || token.ExpireAt<=DateTime.UtcNow)
            {
                var ClientId = Configuration.GetValue<string>("ClientId");
                var Secret = Configuration.GetValue<string>("Secret");

                //auth
                var AuthClient = httpClientFactory.CreateClient(AuthApiName);
                var response = await AuthClient.PostAsJsonAsync("auth", new AppCredential
                {
                    ClientId = ClientId,
                    Secret = Secret
                });
                response.EnsureSuccessStatusCode();

                // get token
                strToken = await response.Content.ReadAsStringAsync();
                token = JsonConvert.DeserializeObject<JwtToken>(strToken);
                httpContextAccessor.HttpContext?.Session.SetString("access_token", strToken);
            }
            
            // pass the JWT to endpoints
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token?.AccessToken);
        }
    }
}
