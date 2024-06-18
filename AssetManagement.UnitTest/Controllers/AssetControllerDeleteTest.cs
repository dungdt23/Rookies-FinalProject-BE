using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.IServices.IAssetServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers
{
    [TestFixture]
    public class AssetControllerDeleteTest
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
        public async Task Delete_ShouldReturnOkResult_WhenAssetIsDeletedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var response = new ApiResponse
            {
                Message = "Delete asset successfully"
            };

            _mockAssetService.Setup(service => service.DeleteAsync(id))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Delete(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(response, okResult.Value);
        }
        [Test]
        public async Task Delete_ShouldReturnInternalServerError_WhenServiceReturnsError()
        {
            // Arrange
            var id = Guid.NewGuid();
            var response = new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Delete asset failed"
            };

            _mockAssetService.Setup(service => service.DeleteAsync(id))
                .ReturnsAsync(response);

            // Act
            var result = await _assetsController.Delete(id);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual(response, objectResult.Value);
        }
    }
}
