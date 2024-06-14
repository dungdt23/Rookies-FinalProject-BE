using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Models;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AssetManagement.UnitTest.Services;

[TestFixture]
public class UserServiceCreateAsyncTest
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private UserService _userService;
    private Mock<User> _userMock;
    private Mock<CreateUpdateUserForm> _createFormMock;
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [SetUp]
    public void Setup(){
        _createFormMock = new Mock<CreateUpdateUserForm>();
        _userMock = new Mock<User>();
    }

    [Test]
    public async Task CreateAsync_ShouldReturnSuccessResponse_WhenUserIsCreated()
    {
        // Arrange
        var generatedUserName = "sonnvb";
        var generatedStaffCode = "SD0001";
        _userMock.Object.FirstName = "Nguyen Viet Bao";
        _userMock.Object.LastName = "Son";

        _mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateUpdateUserForm>())).Returns(_userMock.Object);
        _userRepositoryMock.Setup(r => r.GenerateStaffCode()).Returns(generatedStaffCode);
        _userRepositoryMock.Setup(r => r.GenerateUserName(It.IsAny<string>())).Returns(generatedUserName);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(1);

        // Act
        var result = await _userService.CreateAsync(_createFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiResponse>();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().Be("User created successfully");
        result.Data.Should().BeEquivalentTo(_userMock.Object);
    }

	[Test]
	public async Task CreateAsync_ShouldReturnErrorResponse_WhenUserCreationFails()
    {
		// Arrange
		var generatedUserName = "sonnvb";
		var generatedStaffCode = "SD0001";
		_userMock.Object.FirstName = "Nguyen Viet Bao";
		_userMock.Object.LastName = "Son";

		_mapperMock.Setup(m => m.Map<User>(It.IsAny<CreateUpdateUserForm>())).Returns(_userMock.Object);
		_userRepositoryMock.Setup(r => r.GenerateStaffCode()).Returns(generatedStaffCode);
		_userRepositoryMock.Setup(r => r.GenerateUserName(It.IsAny<string>())).Returns(generatedUserName);
		_userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(0);

		// Act
		var result = await _userService.CreateAsync(_createFormMock.Object);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApiResponse>();
		result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		result.Message.Should().Be("There something went wrong while creating user, please try again later");
		result.Data.Should().BeEquivalentTo(_userMock.Object);
	}
}
