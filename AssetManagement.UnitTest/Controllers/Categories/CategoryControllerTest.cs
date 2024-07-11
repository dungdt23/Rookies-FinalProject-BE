using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.ICategoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Categories
{
    [TestFixture]
    public class CategoriesControllerTests
    {
        private CategoriesController _controller;
        private Mock<ICategoryService> _mockCategoryService;

        [SetUp]
        public void SetUp()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoriesController(_mockCategoryService.Object);
        }

        [Test]
        public async Task Get_ServiceReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError };
            _mockCategoryService.Setup(x => x.GetAllAsync(null, null)).ReturnsAsync(serviceResult);

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
            _mockCategoryService.Setup(x => x.GetAllAsync(null, null)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(serviceResult, okResult.Value);
        }

        [Test]
        public async Task Post_ServiceReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var requestCategoryDto = new RequestCategoryDto();
            var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError };
            _mockCategoryService.Setup(x => x.AddAsync(requestCategoryDto)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.Post(requestCategoryDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Test]
        public async Task Post_ServiceReturnsOk_ReturnsOk()
        {
            // Arrange
            var requestCategoryDto = new RequestCategoryDto();
            var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status200OK };
            _mockCategoryService.Setup(x => x.AddAsync(requestCategoryDto)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.Post(requestCategoryDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(serviceResult, okResult.Value);
        }

        [Test]
        public async Task CheckUnique_ServiceReturnsInternalServerError_ReturnsInternalServerError()
        {
            // Arrange
            var prefixName = new PrefixNameFilter { isPrefix = true, value = "test" };
            var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status500InternalServerError };
            _mockCategoryService.Setup(x => x.IsUniqueAsync(prefixName.isPrefix, prefixName.value)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CheckUnique(prefixName);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }

        [Test]
        public async Task CheckUnique_ServiceReturnsOk_ReturnsOk()
        {
            // Arrange
            var prefixName = new PrefixNameFilter { isPrefix = true, value = "test" };
            var serviceResult = new ApiResponse { StatusCode = StatusCodes.Status200OK };
            _mockCategoryService.Setup(x => x.IsUniqueAsync(prefixName.isPrefix, prefixName.value)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.CheckUnique(prefixName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(serviceResult, okResult.Value);
        }
    }
}
