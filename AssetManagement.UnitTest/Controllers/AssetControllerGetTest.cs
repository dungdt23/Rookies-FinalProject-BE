using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Domain.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers
{
    [TestFixture]
    public class AssetControllerGetTest
    {
        private Mock<IAssetService> _mockAssetService;
        private AssetsController _assetsControoler;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockAssetService = new Mock<IAssetService>();
            _assetsControoler = new AssetsController(_mockAssetService.Object);
        }
        [Test]
        public async Task Get_ShouldReturnOkResult_WhenAssetsAreFound()
        {
            // Arrange
            var filter = new AssetFilter();
            var pagedResponse = new PagedResponse<ResponseAssetDto>
            {
                Data = new List<ResponseAssetDto>
                {
                    new ResponseAssetDto
                    {
                        Id = Guid.NewGuid(),
                        AssetCode = "LA000001",
                        AssetName = "Laptop Thinkpad",
                        Category = "Laptop",
                        Location = "Ha Noi",
                        Specification = "Best laptop",
                        InstalledDate = DateTime.Now,
                        State = "Available"
                    }
                },
                TotalCount = 1,
                Message = "Get asset list successfully!"
            };

            _mockAssetService.Setup(service => service.GetAllAsync(filter, 1, 10))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _assetsControoler.Get(filter, 1, 10);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(pagedResponse, okResult.Value);
        }
        [Test]
        public async Task Get_ShouldReturnOkResult_WhenAssetListAreEmpty()
        {
            // Arrange
            var filter = new AssetFilter();
            var pagedResponse = new PagedResponse<ResponseAssetDto>
            {
                Data = new List<ResponseAssetDto>(),
                TotalCount = 0,
                Message = "List asset is empty"
            };

            _mockAssetService.Setup(service => service.GetAllAsync(filter, 1, 10))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _assetsControoler.Get(filter, 1, 10);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(pagedResponse, okResult.Value);
        }
        [Test]
        public async Task Get_ShouldReturnInternalServerError_WhenServiceReturnsError()
        {
            // Arrange
            var filter = new AssetFilter();
            var pagedResponse = new PagedResponse<ResponseAssetDto>
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Error"
            };

            _mockAssetService.Setup(service => service.GetAllAsync(filter, 1, 10))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _assetsControoler.Get(filter, 1, 10);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual(pagedResponse, objectResult.Value);
        }
    }
}
