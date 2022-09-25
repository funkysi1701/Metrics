using BlazorApplicationInsights;
using Metrics.Core.Model;
using System.Text.Json;

namespace Metrics.Static.Services
{
    public class BlogService
    {
        private HttpClient Client { get; set; }
        private IApplicationInsights AppInsights { get; set; }

        public BlogService(HttpClient httpClient, IConfiguration config, IApplicationInsights AppInsights)
        {
            Client = httpClient;
            this.AppInsights = AppInsights;
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
            }
        }

        public async Task<IList<IList<ChartViewWithType>>> GetChart(int type, int day, int offSet, string username)
        {
            try
            {
                var response = await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={offSet}&username={username}"));
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new ApplicationException($"Reason: {response.ReasonPhrase}, Message: {content}");

                return JsonSerializer.Deserialize<IList<IList<ChartViewWithType>>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                var er = new Error
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await AppInsights.TrackException(er);
                return null;
            }
        }
    }
}
