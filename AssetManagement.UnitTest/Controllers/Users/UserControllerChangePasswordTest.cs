using AssetManagement.Api.Controllers;
using AssetManagement.Api;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
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
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AssetManagement.Application.ApiResponses;

namespace AssetManagement.UnitTest.Controllers.Users
{
	[TestFixture]
	public class UserControllerChangePasswordTest
	{
		private readonly string _validAuthorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjhiMGE2NDIzLTY5MTMtNGQ0OS1iYTJmLTk0NmY2ZDkzMDlmMSIsInN1YiI6IjEyMzQ1Njc4OTAiLCJuYW1lIjoiSm9obiBEb2UiLCJpYXQiOjE1MTYyMzkwMjJ9.qRC8HFPaQ3UXbi4_Y4CVcest3rDKgbukT4HRLYkz7u8";
		private readonly string _invalidAuthorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwic3ViIjoiMTIzNDU2Nzg5MCIsIm5hbWUiOiJKb2huIERvZSIsImlhdCI6MTUxNjIzOTAyMn0.fO7XginXh1Zbjl4D8AYXxMliC_VeozuKBsl3EjmAiPg";
		private Mock<IUserService> _userServiceMock;
		private Mock<IOptions<AppSetting>> _applicationSettingsMock;
		private UsersController _usersController;
		private Mock<RequestChangePasswordDto> _requestMock;
		private Mock<ApiResponse> _responseMock;

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
			_requestMock = new Mock<RequestChangePasswordDto>();
			_responseMock = new Mock<ApiResponse>();
		}

		[Test]
		public async Task ChangePassword_ShouldReturnUnAuthorized_WhenTokenInvalid()
		{
			//Arrange
			_usersController.ControllerContext = new ControllerContext();
			_usersController.ControllerContext.HttpContext = new DefaultHttpContext();
			_usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _invalidAuthorizeHeaderMock);
			//Act
			var result = await _usersController.ChangePassword(_requestMock.Object);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<UnauthorizedResult>();
		}

		[Test]
		public async Task ChangePassword_ShouldReturnBadRequest_WhenRequestIsInvalid()
		{
			//Arrange
			_responseMock.Object.StatusCode = StatusCodes.Status400BadRequest;

			_usersController.ControllerContext = new ControllerContext();
			_usersController.ControllerContext.HttpContext = new DefaultHttpContext();
			_usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _validAuthorizeHeaderMock);
			_userServiceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(_responseMock.Object);
			//Act
			var result = await _usersController.ChangePassword(_requestMock.Object);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<BadRequestObjectResult>();
		}

		[Test]
		public async Task ChangePassword_ShouldReturnError_WhenChangePasswordFail()
		{
			//Arrange
			_responseMock.Object.StatusCode = StatusCodes.Status500InternalServerError;

			_usersController.ControllerContext = new ControllerContext();
			_usersController.ControllerContext.HttpContext = new DefaultHttpContext();
			_usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _validAuthorizeHeaderMock);
			_userServiceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(_responseMock.Object);
			//Act
			var result = await _usersController.ChangePassword(_requestMock.Object);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<ObjectResult>();
		}

		[Test]
		public async Task ChangePassword_ShouldReturnOk_WhenChangePasswordSuccess()
		{
			//Arrange
			_responseMock.Object.StatusCode = StatusCodes.Status200OK;

			_usersController.ControllerContext = new ControllerContext();
			_usersController.ControllerContext.HttpContext = new DefaultHttpContext();
			_usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _validAuthorizeHeaderMock);
			_userServiceMock.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<bool>())).ReturnsAsync(_responseMock.Object);
			//Act
			var result = await _usersController.ChangePassword(_requestMock.Object);
			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType<OkObjectResult>();
		}
	}
}
