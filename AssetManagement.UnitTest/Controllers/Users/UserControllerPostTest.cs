using AssetManagement.Api;
using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Users;

[TestFixture]
public class UserControllerPostTest
{
    private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwic3ViIjoiMTIzNDU2Nzg5MCIsIm5hbWUiOiJKb2huIERvZSIsImlhdCI6MTUxNjIzOTAyMn0.fO7XginXh1Zbjl4D8AYXxMliC_VeozuKBsl3EjmAiPg";
    private Mock<IUserService> _userServiceMock;
    private Mock<IOptions<AppSetting>> _applicationSettingsMock;
    private UsersController _usersController;
    private Mock<RequestUserCreateDto> _createUserFormMock;
    private Mock<ResponseUserDto> _userDtoMock;
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
        _userDtoMock = new Mock<ResponseUserDto>();
        _createUserFormMock = new Mock<RequestUserCreateDto>();
        _userFilterMock = new Mock<UserFilter>();
    }

    [Test]
    public async Task Post_ShouldReturnOk_WhenUserIsCreated()
    {
        // Arrange
        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = UserApiResponseMessageConstant.UserCreateSuccess,
            Data = _userDtoMock.Object
        };
        _usersController.ControllerContext = new ControllerContext();
        _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
        _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<RequestUserCreateDto>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Post(_createUserFormMock.Object);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Test]
    public async Task Post_ShouldReturnUnAuthorized_WhenCantDecodeLocationIdFromToken()
    {
        // Arrange
        _usersController.ControllerContext = new ControllerContext();
        _usersController.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await _usersController.Post(_createUserFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(UnauthorizedResult));
    }

    [Test]
    public async Task Post_ShouldReturnInternalServerError_WhenUserCreationFails()
    {
        // Arrange
        var response = new ApiResponse
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = UserApiResponseMessageConstant.UserCreateFail,
            Data = _userDtoMock.Object
        };

        _usersController.ControllerContext = new ControllerContext();
        _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
        _usersController.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<RequestUserCreateDto>())).ReturnsAsync(response);

        // Act
        var result = await _usersController.Post(_createUserFormMock.Object);

        // Assert
        var internalServerErrorResult = result as ObjectResult;
        internalServerErrorResult.Should().NotBeNull();
        internalServerErrorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        internalServerErrorResult.Value.Should().BeEquivalentTo(response);
    }

}