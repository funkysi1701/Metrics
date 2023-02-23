using BlazorApplicationInsights;
using Metrics.Core.Errors;
using Metrics.Model;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Metrics.Core.Service
{
    public class ChartService
    {
        private HttpClient Client { get; set; }
        private IApplicationInsights AppInsights { get; set; }

        public ChartService(HttpClient httpClient, IConfiguration config, IApplicationInsights AppInsights)
        {
            Client = httpClient;
            this.AppInsights = AppInsights;
            if (!string.IsNullOrEmpty(config.GetValue<string>("BaseURL")))
            {
                Client.BaseAddress = new Uri(config.GetValue<string>("BaseURL"));
            }
        }

        public async Task<IList<IList<ChartViewWithType>>> Get(int type, int day, int offSet, string username)
        {
            try
            {
                var response = await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetChart?type={type}&day={day}&offset={offSet}&username={username}"));
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpStatusCodeException(response.StatusCode, $"Reason: {response.ReasonPhrase}, Message: {content}");

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

        public async Task<string> Delete(string Id)
        {
            try
            {
                var response = await Client.DeleteAsync(new Uri($"{Client.BaseAddress}api/Delete?Id={Id}"));
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpStatusCodeException(response.StatusCode, $"Reason: {response.ReasonPhrase}, Message: {content}");
                return "ok";
            }
            catch (Exception ex)
            {
                var er = new Error
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await AppInsights.TrackException(er);
                return "error";
            }
        }

        public async Task<IList<Metric>> GetData(int type, string username, DateTime date)
        {
            try
            {
                var response = await Client.GetAsync(new Uri($"{Client.BaseAddress}api/GetData?type={type}&username={username}&date={date:yyyy-MM-dd HH:mm:ss}"));
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    throw new HttpStatusCodeException(response.StatusCode, $"Reason: {response.ReasonPhrase}, Message: {content}");

                return JsonSerializer.Deserialize<IList<Metric>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
