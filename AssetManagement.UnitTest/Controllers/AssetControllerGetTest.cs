using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace AssetManagement.UnitTests.Controllers
{
    [TestFixture]
    public class AssetsControllerTests
    {
        private Mock<IAssetService> _mockAssetService;
        private AssetsController _controller;
        private Mock<ILogger<AssetsController>> _mockLogger;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _mockAssetService = new Mock<IAssetService>();
            _mockLogger = new Mock<ILogger<AssetsController>>();
            _controller = new AssetsController(_mockAssetService.Object, _mockLogger.Object);
        }

        private void SetUpHttpContextWithClaim(string claimType)
        {
            var locationId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(claimType, locationId.ToString())
            }));

            var httpContext = new DefaultHttpContext { User = user };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
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

            SetUpHttpContextWithClaim("locationId");

            _mockAssetService.Setup(svc => svc.GetAllAsync(locationId, filter, index, size))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.Get(filter, index, size) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(pagedResponse, result.Value);
        }

        [Test]
        public async Task Post_ShouldReturnOkResult_WhenAssetIsAddedSuccessfully()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var requestAssetDto = new RequestAssetDto { AssetName = "New Asset" };
            var apiResponse = new ApiResponse
            {
                Data = new ResponseAssetDto(),
                Message = "Add asset successfully",
                StatusCode = StatusCodes.Status200OK
            };

            SetUpHttpContextWithClaim("locationId");

            _mockAssetService.Setup(svc => svc.AddAsync(It.Is<RequestAssetDto>(dto => dto.LocationId == locationId)))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Post(requestAssetDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(apiResponse, result.Value);
        }

        [Test]
        public async Task Delete_ShouldReturnOkResult_WhenAssetIsDeletedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var apiResponse = new ApiResponse
            {
                Data = "Asset deleted successfully",
                Message = "Delete asset successfully",
                StatusCode = StatusCodes.Status200OK
            };

            _mockAssetService.Setup(svc => svc.DeleteAsync(id))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Delete(id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(apiResponse, result.Value);
        }

        [Test]
        public async Task Put_ShouldReturnOkResult_WhenAssetIsUpdatedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var requestAssetDto = new RequestAssetDto { AssetName = "Updated Asset" };
            var apiResponse = new ApiResponse
            {
                Data = new ResponseAssetDto(),
                Message = "Update asset successfully",
                StatusCode = StatusCodes.Status200OK
            };

            _mockAssetService.Setup(svc => svc.UpdateAsync(id, requestAssetDto))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Put(id, requestAssetDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual(apiResponse, result.Value);
        }
    }
}
