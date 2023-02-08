using Metrics.Core.MVC;
using Metrics.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json;

namespace Metrics.Core.Tests.Core.MVC
{
    public class GetAllBlogsTests
    {
        private IConfiguration? config;
        private Mock<IHttpClientFactory>? factory;

        private void SharedStuff(List<BlogPosts> posts)
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"DEVTOAPI", "anykey"},
                {"DEVTOURL", "https://anyurl/"},
            };

            config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            factory = new Mock<IHttpClientFactory>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var content = new StringContent(JsonConvert.SerializeObject(posts));
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);
            factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);
        }

        [Fact]
        public async Task GetAll_EmptyList_ReturnsOK()
        {
            var posts = new List<BlogPosts>
            {
                new BlogPosts()
                {
                    Published = false,
                    Description = "not published yet"
                }
            };
            SharedStuff(posts);

            var result = await GetAllBlogs.GetAll(config, 200, factory.Object);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ReturnsOK()
        {
            var posts = new List<BlogPosts>
            {
                new BlogPosts()
                {
                    Published = true,
                    Description = "list of one"
                }
            };
            SharedStuff(posts);

            var result = await GetAllBlogs.GetAll(config, 200, factory.Object);

            Assert.NotEmpty(result);
        }
    }
}
