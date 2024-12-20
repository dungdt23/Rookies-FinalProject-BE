using AssetManagement.Api;
using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Users;
[TestFixture]
public class UserControllerPutTest
{
    private Mock<IUserService> _userServiceMock;
    private Mock<IOptions<AppSetting>> _applicationSettingsMock;
    private UsersController _usersController;
    private Mock<RequestUserEditDto> _updateUserFormMock;
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
        _updateUserFormMock = new Mock<RequestUserEditDto>();
    }

    [Test]
    public async Task Put_ShouldReturnOk_WhenUserIsUpdated()
    {
        // Arrange
        var id = Guid.NewGuid();
        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = UserApiResponseMessageConstant.UserUpdateSuccess,
            Data = _userMock.Object
        };

        _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestUserEditDto>())).ReturnsAsync(response);

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
            StatusCode = StatusCodes.Status400BadRequest,
            Message = UserApiResponseMessageConstant.UserNotFound,
            Data = id
        };

        _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestUserEditDto>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Put(id, _updateUserFormMock.Object);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
		badRequestResult.Should().NotBeNull();
		badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		badRequestResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task Put_ShouldReturnInternalServerError_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = UserApiResponseMessageConstant.UserUpdateFail,
            Data = _userMock.Object
        };

        _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<RequestUserEditDto>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Put(id, _updateUserFormMock.Object);

        // Assert
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult.Should().NotBeNull();
        internalServerErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        internalServerErrorResult.Value.Should().BeEquivalentTo(response);
    }
}