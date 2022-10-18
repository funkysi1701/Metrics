using Metrics.Core.Model;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Metrics.Core.Tests.Core.MVC
{
    public class MongoDataServiceTests
    {
        private MongoDataService service;
        private Mock<IMongoService> _mongoService;
        public MongoDataServiceTests()
        {
            _mongoService = new Mock<IMongoService>();
            service = new MongoDataService(_mongoService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOK()
        {
            _mongoService.Setup(x => x.GetAsync()).ReturnsAsync(new List<Metric>());
            
            var response = await service.GetAll();

            _mongoService.Verify(x => x.GetAsync());
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Get_ReturnsOK()
        {
            _mongoService.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<Metric>());

            var response = await service.Get(0, "username");

            _mongoService.Verify(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Delete_ReturnsOK()
        {
            var dt = DateTime.Now;
            var response = new List<Metric>
            {
                new Metric()
            };
            response[0].Date = dt;
            _mongoService.Setup(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(response);
            _mongoService.Setup(x => x.RemoveAsync(It.IsAny<string>()));

            await service.Delete(0, dt, "username");

            _mongoService.Verify(x => x.GetAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()));
            _mongoService.Verify(x => x.RemoveAsync(It.IsAny<string>()));
        }

        [Fact]
        public async Task SaveData_ReturnsOK()
        {
            var dt = DateTime.Now;
            _mongoService.Setup(x => x.CreateAsync(It.IsAny<Metric>()));

            var response = await service.SaveData(0, 0, dt, "username");

            _mongoService.Verify(x => x.CreateAsync(It.IsAny<Metric>()));

            Assert.IsType<OkObjectResult>(response);
        }
    }
}
