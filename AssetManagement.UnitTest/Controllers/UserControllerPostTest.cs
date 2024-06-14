using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controller;

[TestFixture]
public class UserControllerPostTest
{
	private Mock<IUserService> _userServiceMock;
	private UsersController _usersController;
	private Mock<CreateUpdateUserForm> _createUserFormMock;
	private Mock<User> _userMock;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_userServiceMock = new Mock<IUserService>();
		_usersController = new UsersController(_userServiceMock.Object);
	}

	[SetUp]
	public void SetUp()
	{
		_userMock = new Mock<User>();
		_createUserFormMock = new Mock<CreateUpdateUserForm>();
	}

	[Test]
	public async Task Post_ShouldReturnOk_WhenUserIsCreated()
	{
		// Arrange
		var response = new ApiResponse
		{
			StatusCode = StatusCodes.Status200OK,
			Message = UserApiResponseMessageContraint.UserCreateSuccess,
			Data = _userMock.Object
		};

		_userServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateUserForm>())).ReturnsAsync(response);

		// Act
		var result = await _usersController.Post(_createUserFormMock.Object);

		// Assert
		var okResult = result as OkObjectResult;
		okResult.Should().NotBeNull();
		okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		okResult.Value.Should().BeEquivalentTo(response);
	}

	[Test]
	public async Task Post_ShouldReturnInternalServerError_WhenUserCreationFails()
	{
		// Arrange
		var response = new ApiResponse
		{
			StatusCode = StatusCodes.Status500InternalServerError,
			Message = UserApiResponseMessageContraint.UserCreateFail,
			Data = _userMock.Object
		};

		_userServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateUpdateUserForm>())).ReturnsAsync(response);

		// Act
		var result = await _usersController.Post(_createUserFormMock.Object);

		// Assert
		var internalServerErrorResult = result as ObjectResult;
		internalServerErrorResult.Should().NotBeNull();
		internalServerErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		internalServerErrorResult.Value.Should().BeEquivalentTo(response);
	}
}
