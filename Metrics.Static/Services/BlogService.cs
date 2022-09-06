using Metrics.Core.Model;
using System.Net.Http.Json;

namespace Metrics.Static.Services
{
    public class BlogService
    {
        private HttpClient Client { get; set; }

        public BlogService(HttpClient httpClient, IConfiguration config)
        {
            Client = httpClient;
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
            }
        }

        public async Task<IList<IList<ChartView>>> GetChart(int type, int day, int offSet, string username)
        {
            return await Client.GetFromJsonAsync<IList<IList<ChartView>>>(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={offSet}&username={username}"));
        }
    }
}
