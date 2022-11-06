using AutoFixture;
using AutoFixture.AutoMoq;
using Metrics.Core.Model;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Metrics.Core.Tests.Core.MVC
{
    public class MastodonServiceTests
    {
        private readonly Mock<IMongoService> _mongoService;
        private readonly Fixture fixture;
        public MastodonServiceTests()
        {
            _mongoService = new Mock<IMongoService>();
            fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());
        }

        [Fact]
        public async Task GetMastodonFollowers_ReturnsOK()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"MastodonServer", "http://example.com"},
                {"MastodonUser", "user"},
                {"MastodonPass", "pass"},
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _mongoService.Setup(x => x.CreateAsync(It.IsAny<Metric>()));
            var service = new MastodonService(config, _mongoService.Object);

            await service.GetMastodonFollowers(fixture.Create<ILogger>(), "bob");
        }
    }
}
