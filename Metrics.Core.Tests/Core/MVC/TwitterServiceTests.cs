using Autofac.Core;
using AutoFixture;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Core.Misc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Iterators;
using Tweetinvi.Parameters;
using Xunit;

namespace Metrics.Core.Tests.Core.MVC
{
    public class TwitterServiceTests
    {
        private readonly TwitterService service;
        private readonly Fixture fixture = new();
        private readonly Mock<IMongoService> _mongoService;
        private readonly Mock<ITwitterClient> TwitterClient;

        public TwitterServiceTests()
        {
            _mongoService = new Mock<IMongoService>();
            TwitterClient = new Mock<ITwitterClient>();
            //var inMemorySettings = new Dictionary<string, string> {
            //    {"TWConsumerKey", "TWConsumerKey"},
            //    {"TWConsumerSecret", "TWConsumerSecret"},
            //    {"TWAccessToken", "TWAccessToken"},
            //    {"TWAccessSecret", "TWAccessSecret"},
            //};

            //IConfiguration config = new ConfigurationBuilder()
            //    .AddInMemoryCollection(inMemorySettings)
            //    .Build();
            service = new TwitterService(TwitterClient.Object, _mongoService.Object);
        }

        [Fact]
        public async Task GetTwitterFollowers_ReturnsOK()
        {
            var log = new Mock<ILogger>();
            TwitterClient.Setup(x => x.Users.GetFollowerIdsIterator(It.IsAny<IGetFollowerIdsParameters>())).Returns(fixture.Create<ITwitterIterator<long>>());

            var response = await service.GetTwitterFollowers(log.Object, "test");

            Assert.IsType<OkObjectResult>(response);
        }
    }
}
