using Metrics.Model.Enum;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Metrics.IIS.Services
{
    public class SaveDataService
    {
        private readonly IConfiguration Configuration;
        private readonly TelemetryClient telemetry;

        public SaveDataService(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            this.telemetry = telemetry;
        }

        public async Task<IActionResult> SaveData(decimal value, MetricType type, string username)
        {
            var client = CreateClient();
            var typeParameter = (int)type;
            using var httpResponse = await client.GetAsync($"{client.BaseAddress}api/SaveData?type={typeParameter}&username={username}&value={value}");
            string result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                telemetry.TrackEvent($"Error {result} for SaveData {typeParameter}");
                return new BadRequestObjectResult($"Error {result} for SaveData {typeParameter}");
            }

            var ob = JsonConvert.DeserializeObject<OkObjectResult>(result);
            var obvalue = (double)ob.Value;

            return new OkObjectResult(obvalue);
        }

        public HttpClient CreateClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("FunctionAPI")),
            };
        }
    }
}
