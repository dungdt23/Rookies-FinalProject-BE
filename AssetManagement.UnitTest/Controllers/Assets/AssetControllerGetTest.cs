using AssetManagement.Api.Controllers;
using AssetManagement.Api.Hubs;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Assets
{
    [TestFixture]
    public class AssetsControllerTests
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiZGRhZDhjNjgtYTI1YS00MTZhLTk4MDgtYzIyYWIyYWJkZTBmIiwic3ViIjoiMTIzNDU2Nzg5MCIsIm5hbWUiOiJKb2huIERvZSIsImlhdCI6MTUxNjIzOTAyMn0.zGkUGTif6i3P1iTc-rQ2hxuzyNELXN-9OGlXEcOj04c";
        private Mock<IAssetService> _mockAssetService;
        private AssetsController _controller;
        private Mock<ILogger<AssetsController>> _mockLogger;
        private Mock<RequestAssetDto> _requestAssetDto;
        private Mock<ResponseAssetDto> _responseAssetDto;
        private Mock<AssetFilter> _assetFilter;
        private Mock<IHubContext<SignalRHub>> _mockHubContext;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockAssetService = new Mock<IAssetService>();
            _mockLogger = new Mock<ILogger<AssetsController>>();
            _mockHubContext = new Mock<IHubContext<SignalRHub>>();
            _controller = new AssetsController(_mockAssetService.Object, _mockLogger.Object, _mockHubContext.Object);
        }
        [SetUp]
        public void SetUp()
        {
            _requestAssetDto = new Mock<RequestAssetDto>();
            _responseAssetDto = new Mock<ResponseAssetDto>();
            _assetFilter = new Mock<AssetFilter>(); 
        }

        [Test]
        public async Task Get_ShouldReturnOkResult_WhenAssetsAreFound()
        {
            // Arrange
            var filter = new AssetFilter();
            var index = 1;
            var size = 10;
            var locationId = Guid.NewGuid();
            var pagedResponse = new PagedResponse<ResponseAssetDto>
            {
                Data = new List<ResponseAssetDto> { new ResponseAssetDto() },
                TotalCount = 1,
                Message = "Get asset list successfully",
                StatusCode = StatusCodes.Status200OK
            };
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _mockAssetService.Setup(svc => svc.GetAllAsync(It.IsAny<Guid>(), It.IsAny<AssetFilter>(), index, size))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.Get(filter, index, size) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(pagedResponse, result.Value);
        }
        [Test]
        public async Task Get_ShouldReturnOkResult_WhenAssetListAreEmpty()
        {
            // Arrange
            var filter = new AssetFilter();
            var index = 1;
            var size = 10;
            var locationId = Guid.NewGuid();
            var pagedResponse = new PagedResponse<ResponseAssetDto>
            {
                Data = new List<ResponseAssetDto> { },
                TotalCount = 0,
                Message = "List asset is empty",
                StatusCode = StatusCodes.Status200OK
            };
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _mockAssetService.Setup(svc => svc.GetAllAsync(It.IsAny<Guid>(), It.IsAny<AssetFilter>(), index, size))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.Get(filter, index, size) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(pagedResponse, result.Value);
        }
        [Test]
        public async Task Get_ShouldReturnInternalErrorResult_WhenAssetListAreEmpty()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var response = new ApiResponse();
            response.Data = _responseAssetDto.Object;
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _mockAssetService.Setup(svc => svc.GetByIdAysnc(It.IsAny<Guid>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(It.IsAny<Guid>()) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(response, result.Value);
        }
    }
}
