using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Assets
{
    public class AssetControllerPostTest
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiZGRhZDhjNjgtYTI1YS00MTZhLTk4MDgtYzIyYWIyYWJkZTBmIiwic3ViIjoiMTIzNDU2Nzg5MCIsIm5hbWUiOiJKb2huIERvZSIsImlhdCI6MTUxNjIzOTAyMn0.zGkUGTif6i3P1iTc-rQ2hxuzyNELXN-9OGlXEcOj04c";
        private Mock<IAssetService> _mockAssetService;
        private AssetsController _assetsController;
        private Mock<ILogger<AssetsController>> _mockLogger;
        private Mock<RequestAssetDto> _requestAssetDto; 
        private Mock<ResponseAssetDto> _responseAssetDto; 
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockAssetService = new Mock<IAssetService>();
            _mockLogger = new Mock<ILogger<AssetsController>>();
            _assetsController = new AssetsController(_mockAssetService.Object, _mockLogger.Object);
        }
        [SetUp] 
        public void SetUp()
        {
            _requestAssetDto = new Mock<RequestAssetDto>();
            _responseAssetDto = new Mock<ResponseAssetDto>(); 
        }
        [Test]
        public async Task Post_ShouldReturnOkResult_WhenAssetIsAddedSuccessfully()
        {
            // Arrange
            var response = new ApiResponse
            {
                Data = _responseAssetDto.Object,
                Message = "Add new asset successfully"
            };
            _assetsController.ControllerContext = new ControllerContext();
            _assetsController.ControllerContext.HttpContext = new DefaultHttpContext();
            _assetsController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _mockAssetService.Setup(service => service.AddAsync(It.IsAny<RequestAssetDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Post(_requestAssetDto.Object);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(response, okResult.Value);
        }
        [Test]
        public async Task Post_ShouldReturnInternalServerError_WhenServiceReturnsError()
        {
            // Arrange
            var response = new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Add new asset failed"
            };
            _assetsController.ControllerContext = new ControllerContext();
            _assetsController.ControllerContext.HttpContext = new DefaultHttpContext();
            _assetsController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _mockAssetService.Setup(service => service.AddAsync(It.IsAny<RequestAssetDto>()))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Post(_requestAssetDto.Object);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual(response, objectResult.Value);
        }
    }
}
