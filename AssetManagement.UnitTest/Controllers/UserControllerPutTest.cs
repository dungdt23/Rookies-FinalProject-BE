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

namespace AssetManagement.UnitTest.Controllers;
[TestFixture]
public class UserControllerPutTest
{
    private Mock<IUserService> _userServiceMock;
    private UsersController _usersController;
    private Mock<CreateUpdateUserForm> _updateUserFormMock;
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
        _updateUserFormMock = new Mock<CreateUpdateUserForm>();
    }

    [Test]
	public async Task Put_ShouldReturnOk_WhenUserIsUpdated()
    {
		// Arrange
		var id = Guid.NewGuid();
		var response = new ApiResponse
		{
			StatusCode = StatusCodes.Status200OK,
			Message = UserApiResponseMessageContraint.UserUpdateSuccess,
			Data = _userMock.Object
		};

		_userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CreateUpdateUserForm>())).ReturnsAsync(response);

		// Act
		var result = await _usersController.Put(id, _updateUserFormMock.Object);

		// Assert
		var okResult = result as OkObjectResult;
		okResult.Should().NotBeNull();
		okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
		okResult.Value.Should().BeEquivalentTo(response);
	}

	[Test]
	public async Task Put_ShouldReturnNotFound_WhenUserIsNotFound()
	{
		// Arrange
		var id = Guid.NewGuid();
		var response = new ApiResponse
		{
			StatusCode = StatusCodes.Status404NotFound,
			Message = UserApiResponseMessageContraint.UserNotFound,
			Data = id
		};

		_userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CreateUpdateUserForm>())).ReturnsAsync(response);

		// Act
		var result = await _usersController.Put(id, _updateUserFormMock.Object);

		// Assert
		var notFoundResult = result as ObjectResult;
		notFoundResult.Should().NotBeNull();
		notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		notFoundResult.Value.Should().BeEquivalentTo(response);
	}

	[Test]
	public async Task Put_ShouldReturnInternalServerError_WhenUpdateFails()
	{
		// Arrange
		var id = Guid.NewGuid();

		var response = new ApiResponse
		{
			StatusCode = StatusCodes.Status500InternalServerError,
			Message = UserApiResponseMessageContraint.UserUpdateFail,
			Data = _userMock.Object
		};

		_userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CreateUpdateUserForm>())).ReturnsAsync(response);

		// Act
		var result = await _usersController.Put(id, _updateUserFormMock.Object);

		// Assert
		var internalServerErrorResult = result as ObjectResult;
		internalServerErrorResult.Should().NotBeNull();
		internalServerErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		internalServerErrorResult.Value.Should().BeEquivalentTo(response);
	}
}
