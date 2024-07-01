using AssetManagement.Api;
using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Users
{
    public class UserControllerDeleteTest
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";
        private Mock<IUserService> _userServiceMock;
        private Mock<IOptions<AppSetting>> _applicationSettingsMock;
        private UsersController _usersController;
        private Mock<User> _userMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _userServiceMock = new Mock<IUserService>();
            _applicationSettingsMock = new Mock<IOptions<AppSetting>>();
            _usersController = new UsersController(_userServiceMock.Object, _applicationSettingsMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _userMock = new Mock<User>();
        }

        [Test]
        public async Task Delete_UserDeleteSuccess_ReturnsOk()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _usersController.ControllerContext = new ControllerContext();
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
            _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _userServiceMock.Setup(s => s.DisableUser(userId)).ReturnsAsync(new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "User disabled successfully"
            });

            // Act
            var result = await _usersController.Delete(userId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task Delete_UserDeleteConflict_ReturnsConflict()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _usersController.ControllerContext = new ControllerContext();
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
            _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _userServiceMock.Setup(s => s.DisableUser(userId)).ReturnsAsync(new ApiResponse
            {
                StatusCode = StatusCodes.Status409Conflict,
                Message = "User has valid assignments"
            });

            // Act
            var result = await _usersController.Delete(userId) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);
        }
    }
}
