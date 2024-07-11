using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.IServices.ITypeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Types
{
    public class TypeControllerTest
    {
        [TestFixture]
        public class TypesControllerTests
        {
            private TypesController _controller;
            private Mock<ITypeService> _mockTypeService;

            [SetUp]
            public void SetUp()
            {
                _mockTypeService = new Mock<ITypeService>();
                _controller = new TypesController(_mockTypeService.Object);
            }

            [Test]
            public async Task Get_ServiceReturnsInternalServerError_ReturnsInternalServerError()
            {
                // Arrange
                var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError };
                _mockTypeService.Setup(x => x.GetAllAsync(null, null)).ReturnsAsync(serviceResult);

                // Act
                var result = await _controller.Get();

                // Assert
                Assert.IsInstanceOf<ObjectResult>(result);
                var objectResult = result as ObjectResult;
                Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            }

            [Test]
            public async Task Get_ServiceReturnsOk_ReturnsOk()
            {
                // Arrange
                var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status200OK };
                _mockTypeService.Setup(x => x.GetAllAsync(null, null)).ReturnsAsync(serviceResult);

                // Act
                var result = await _controller.Get();

                // Assert
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.AreEqual(serviceResult, okResult.Value);
            }
        }
    }
}
