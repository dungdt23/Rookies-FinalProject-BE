using AssetManagement.Api.Controllers;
using AssetManagement.Api;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManagement.Domain.Entities;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace AssetManagement.UnitTest.Controllers.Users
{
    public class UserControllerGetTest
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";
        private Mock<IUserService> _userServiceMock;
        private Mock<IOptions<AppSetting>> _applicationSettingsMock;
        private UsersController _usersController;
        private Mock<User> _userMock;
        private Mock<UserFilter> _userFilterMock;
        private Mock<IMapper> _mapperMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _userServiceMock = new Mock<IUserService>();
            _applicationSettingsMock = new Mock<IOptions<AppSetting>>();
            _usersController = new UsersController(_userServiceMock.Object, _applicationSettingsMock.Object);
            _mapperMock = new Mock<IMapper>();

        }

        [SetUp]
        public void SetUp()
        {
            _userMock = new Mock<User>();
            _userFilterMock = new Mock<UserFilter>();
        }
        [Test]
        public async Task GetAll_ShouldReturnOk_WhenUsersAreRetrieved()
        {
            // Arrange
            var filter = new UserFilter();

            _usersController.ControllerContext = new ControllerContext();
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
            _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<Guid>(), filter, 1, 10)).ReturnsAsync(new PagedResponse<ResponseUserDto>
            {
                Data = new List<ResponseUserDto> { new ResponseUserDto { } },
                Message = "User list retrieved successfully"
            });

            // Act
            var result = await _usersController.Get(filter, 1, 10) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [Test]
        public async Task GetAll_ShouldReturnOk_WhenUserListIsEmpty()
        {
            // Arrange
            var filter = new UserFilter();

            _usersController.ControllerContext = new ControllerContext();
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
            _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<Guid>(), filter, 1, 10)).ReturnsAsync(new PagedResponse<ResponseUserDto>
            {
                Data = new List<ResponseUserDto>(),
                Message = "List user is empty"
            });

            // Act
            var result = await _usersController.Get(filter, 1, 10) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }
        [Test]
        public async Task GetById_ShouldReturnOkResult_WhenUserIsFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var apiResponse = new ApiResponse { StatusCode = StatusCodes.Status200OK };

            _usersController.ControllerContext = new ControllerContext();
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
            _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

            _userServiceMock.Setup(us => us.GetById(userId)).ReturnsAsync(apiResponse);

            // Act
            var result = await _usersController.GetById(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

    }
}
