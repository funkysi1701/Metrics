using BlazorApplicationInsights;
using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Metrics.Core.Tests.Core.Service
{
    public class ChartServiceTests
    {
        [Fact]
        public async Task Get_RetunsData()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var res = new List<IList<ChartViewWithType>>();
            var bpp = JsonConvert.SerializeObject(res);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(bpp) });
            var http = new HttpClient(mockHttpMessageHandler.Object);
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseURL", "http://example.com"},
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var insights = new Mock<IApplicationInsights>();
            var chart = new ChartService(http, config, insights.Object);

            var response = await chart.Get(0, 0, 0, "funkysi1701");

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_RetunsError()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var res = new List<IList<ChartViewWithType>>();
            var bpp = JsonConvert.SerializeObject(res);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(bpp) });
            var http = new HttpClient(mockHttpMessageHandler.Object);
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseURL", "http://example.com"},
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var insights = new Mock<IApplicationInsights>();
            var chart = new ChartService(http, config, insights.Object);
            insights.Setup(x => x.TrackException(It.IsAny<Error>(), null, null, null));
            var response = await chart.Get(0, 0, 0, "funkysi1701");
            insights.Verify(x => x.TrackException(It.IsAny<Error>(), null, null, null));
            Assert.Null(response);
        }

        [Fact]
        public async Task GetData_RetunsData()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var res = new List<IList<ChartViewWithType>>();
            var bpp = JsonConvert.SerializeObject(res);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(bpp) });
            var http = new HttpClient(mockHttpMessageHandler.Object);
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseURL", "http://example.com"},
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var insights = new Mock<IApplicationInsights>();
            var chart = new ChartService(http, config, insights.Object);

            var response = await chart.GetData(0, "funkysi1701", DateTime.UtcNow);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task GetData_RetunsError()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var res = new List<IList<ChartViewWithType>>();
            var bpp = JsonConvert.SerializeObject(res);
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(bpp) });
            var http = new HttpClient(mockHttpMessageHandler.Object);
            var inMemorySettings = new Dictionary<string, string> {
                {"BaseURL", "http://example.com"},
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            var insights = new Mock<IApplicationInsights>();
            var chart = new ChartService(http, config, insights.Object);
            insights.Setup(x => x.TrackException(It.IsAny<Error>(), null, null, null));
            var response = await chart.GetData(0, "funkysi1701", DateTime.UtcNow);
            insights.Verify(x => x.TrackException(It.IsAny<Error>(), null, null, null));
            Assert.Null(response);
        }
    }
}
