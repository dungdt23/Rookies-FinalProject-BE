using AssetManagement.Api.Controllers;
using AssetManagement.Api;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using AssetManagement.Application.Dtos.RequestDtos;

namespace AssetManagement.UnitTest.Controllers.Users
{
    [TestFixture]
    public class UserControllerLoginTest
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<IOptions<AppSetting>> _applicationSettingsMock;
        private UsersController _usersController;
        private Mock<RequestLoginDto> _loginFormMock;
        private Mock<User> _userMock;
        private Mock<UserFilter> _userFilterMock;
        private Mock<IMapper> _mapperMock;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _userServiceMock = new Mock<IUserService>();
            _applicationSettingsMock = new Mock<IOptions<AppSetting>>();
            _applicationSettingsMock.Setup(ap => ap.Value).Returns(new AppSetting { Secret = "secret" });
            _usersController = new UsersController(_userServiceMock.Object, _applicationSettingsMock.Object);
            _mapperMock = new Mock<IMapper>();

        }

        [SetUp]
        public void SetUp()
        {
            _userMock = new Mock<User>();
            _loginFormMock = new Mock<RequestLoginDto>();
            _userFilterMock = new Mock<UserFilter>();
        }

        [Test]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = UserApiResponseMessageConstant.UserLoginSuccess,
                Data = new { tokenType = "Bearer", token = "mockToken", isFirstTimeLogin = false }
            };

            _userServiceMock.Setup(us => us.LoginAsync(It.IsAny<RequestLoginDto>(), It.IsAny<byte[]>()))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _usersController.Login(_loginFormMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenLoginFailsDueToInvalidCredentials()
        {
            // Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername,
                Data = UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername
            };

            _userServiceMock.Setup(us => us.LoginAsync(It.IsAny<RequestLoginDto>(), It.IsAny<byte[]>()))
                            .ReturnsAsync(apiResponse);

            // Act
            var result = await _usersController.Login(_loginFormMock.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequestResult.Value.Should().BeEquivalentTo(apiResponse);
        }
    }
}
