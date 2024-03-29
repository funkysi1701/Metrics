﻿using ImpSoft.OctopusEnergy.Api;
using Metrics.Core.Service;
using Metrics.Model.Enum;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metrics.Core.MVC
{
    public class PowerService
    {
        private readonly MongoDataService Chart;
        private IConfiguration Configuration { get; set; }

        private readonly string Key;
        private readonly IOctopusEnergyClient Client;

        public PowerService(IConfiguration configuration, IOctopusEnergyClient client, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new MongoDataService(mongoService);
            Client = client;
            Key = Configuration.GetValue<string>("OctopusKey");
        }

        public async Task GetGas()
        {
            DateTimeOffset From = new(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));

            var consumption = await Client.GetGasConsumptionAsync(Key, Configuration.GetValue<string>("OctopusGasMPAN"), Configuration.GetValue<string>("OctopusGasSerial"), From, To, Interval.HalfHour);
            await CheckConsumption(MetricType.Gas, consumption);
        }

        public async Task GetElec()
        {
            DateTimeOffset From = new(DateTime.UtcNow.AddDays(-30).AddHours(-1).AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));
            DateTimeOffset To = new(DateTime.UtcNow.AddMinutes(-1 * DateTime.UtcNow.AddMinutes(-30).Minute), TimeSpan.FromHours(0));

            var consumption = await Client.GetElectricityConsumptionAsync(Key, Configuration.GetValue<string>("OctopusElecMPAN"), Configuration.GetValue<string>("OctopusElecSerial"), From, To, Interval.HalfHour);
            await CheckConsumption(MetricType.Electricity, consumption);
        }

        public async Task CheckConsumption(MetricType type, IEnumerable<Consumption> consumption)
        {
            var exist = await Chart.Get(type, Configuration.GetValue<string>("Username1"));
            foreach (var item in consumption)
            {
                if (exist.Exists(x => x.Date.Value == item.Start.UtcDateTime.Date))
                {
                    await Chart.Delete(type, item.Start.UtcDateTime, Configuration.GetValue<string>("Username1"));
                }
                await Chart.SaveData(item.Quantity, type, item.Start.UtcDateTime, Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701");
            }
        }
    }
}
