using Metrics.Core.Model;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Metrics.Core.Tests.Core.MVC
{
    public class MastodonServiceTests
    {
        [Fact]
        public async Task GetMastodonFollowers_ReturnsIActionResult()
        {
            // Arrange
            var configuration = new Mock<IConfiguration>();
            var mongoOptions = new Mock<IOptions<MyMongoDatabaseSettings>>();
            mongoOptions.Setup(x => x.Value).Returns(new MyMongoDatabaseSettings() { ConnectionString = "test" });
            var mongoService = new MongoService(mongoOptions.Object);
            var mastodonService = new MastodonService(configuration.Object, mongoService);
            var logger = new Mock<ILogger>();
            var username = "testuser";

            // Act
            var result = await mastodonService.GetMastodonFollowers(logger.Object, username);

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
