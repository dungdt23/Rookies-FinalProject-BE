using AssetManagement.Api;
using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AssetManagement.UnitTest.Controller;

[TestFixture]
public class UserControllerPostTest
{
    private Mock<IUserService> _userServiceMock;
    private Mock<IOptions<AppSetting>> _applicationSettingsMock;
    private UsersController _usersController;
    private Mock<CreateUpdateUserForm> _createUserFormMock;
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
        _createUserFormMock = new Mock<CreateUpdateUserForm>();
        _userFilterMock = new Mock<UserFilter>();
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
    [Test]
    public async Task GetAll_ShouldReturnOk_WhenUsersAreRetrieved()
    {
        // Arrange
        var users = new List<User>();
        var userDtos = new List<ResponseUserDto> { new ResponseUserDto(), new ResponseUserDto() };

        var response = new PagedResponse<ResponseUserDto>
        {
            Data = userDtos,
            Message = "Get user list successfully!",
            TotalCount = 20
        };

        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<UserFilter>(), It.IsAny<int?>(), It.IsAny<int?>()))
                        .ReturnsAsync(response);

        _mapperMock.Setup(m => m.Map<IEnumerable<ResponseUserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _usersController.Get(_userFilterMock.Object, 1, 10);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task GetAll_ShouldReturnOk_WhenUserListIsEmpty()
    {
        // Arrange
        var users = new List<User>();
        var userDtos = new List<ResponseUserDto>();

        var response = new PagedResponse<ResponseUserDto>
        {
            Data = userDtos,
            Message = "List user is empty",
            TotalCount = 0
        };

        _userServiceMock.Setup(s => s.GetAllAsync(It.IsAny<UserFilter>(), It.IsAny<int?>(), It.IsAny<int?>()))
                        .ReturnsAsync(response);

        _mapperMock.Setup(m => m.Map<IEnumerable<ResponseUserDto>>(users)).Returns(userDtos);

        // Act
        var result = await _usersController.Get(_userFilterMock.Object, 1, 10);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(response);
    }
    [Test]
    public async Task DisableUser_ShouldReturnInternalServerError_WhenUserNotFoundOrInactive()
    {
        // Arrange
        var response = new ApiResponse
        {
            Message = "User not found or no long active!",
            StatusCode = StatusCodes.Status500InternalServerError
        };

        _userServiceMock.Setup(s => s.DisableUser(It.IsAny<Guid>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Delete(Guid.NewGuid());

        // Assert
        var errorResult = result as ObjectResult;
        errorResult.Should().NotBeNull();
        errorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        errorResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task DisableUser_ShouldReturnOk_WhenUserIsDisabledSuccessfully()
    {
        // Arrange
        var response = new ApiResponse
        {
            Message = "Disable user successfully!",
            StatusCode = StatusCodes.Status200OK
        };

        _userServiceMock.Setup(s => s.DisableUser(It.IsAny<Guid>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Delete(Guid.NewGuid());

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task DisableUser_ShouldReturnBadRequest_WhenUserHasValidAssignments()
    {
        // Arrange
        var response = new ApiResponse
        {
            Message = "Can't disable user because user still has valid assignments",
            StatusCode = StatusCodes.Status500InternalServerError
        };

        _userServiceMock.Setup(s => s.DisableUser(It.IsAny<Guid>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Delete(Guid.NewGuid());

        // Assert
        var okResult = result as ObjectResult;
        okResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
