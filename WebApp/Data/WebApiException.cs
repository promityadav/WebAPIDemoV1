using System.Text.Json;

namespace WebApp.Data
{
    public class WebApiException: Exception
    {
        public ErrorResponse? errorResponse { get; }
        public WebApiException(string errorJson)
        {
            errorResponse=JsonSerializer.Deserialize<ErrorResponse>(errorJson);
        }
    }
}
