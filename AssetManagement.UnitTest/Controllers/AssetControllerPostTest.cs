using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers
{
    public class AssetControllerPostTest
    {
        private Mock<IAssetService> _mockAssetService;
        private AssetsController _assetsController;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockAssetService = new Mock<IAssetService>();
            _assetsController = new AssetsController(_mockAssetService.Object);
        }
        [Test]
        public async Task Post_ShouldReturnOkResult_WhenAssetIsAddedSuccessfully()
        {
            // Arrange
            var requestDto = new RequestAssetDto();
            var response = new ApiResponse
            {
                Data = requestDto,
                Message = "Add new asset successfully"
            };

            _mockAssetService.Setup(service => service.AddAsync(requestDto))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Post(requestDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(response, okResult.Value);
        }
        [Test]
        public async Task Post_ShouldReturnInternalServerError_WhenServiceReturnsError()
        {
            // Arrange
            var requestDto = new RequestAssetDto();
            var response = new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Add new asset failed"
            };

            _mockAssetService.Setup(service => service.AddAsync(requestDto))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Post(requestDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual(response, objectResult.Value);
        }
    }
}
