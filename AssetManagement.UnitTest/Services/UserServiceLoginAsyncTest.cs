using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Models;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using System.Text;

namespace AssetManagement.UnitTest.Services;
[TestFixture]
public class UserServiceLoginAsyncTest
{
	private readonly string _secret = "This is secret of nashtech assetmanagement project to create jwt token";
	private Mock<IUserRepository> _userRepositoryMock;
	private Mock<IGenericRepository<Assignment>> _assignmentRepositoryMock;
	private Mock<IGenericRepository<Domain.Entities.Type>> _typeRepositoryMock;
	private Mock<IMapper> _mapperMock;
	private UserService _userService;
	private Mock<User> _userMock;
	private Mock<LoginForm> _loginFormMock;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_userRepositoryMock = new Mock<IUserRepository>();
		_assignmentRepositoryMock = new Mock<IGenericRepository<Assignment>>();
		_typeRepositoryMock = new Mock<IGenericRepository<Domain.Entities.Type>>();
		_mapperMock = new Mock<IMapper>();
		_userService = new UserService(_userRepositoryMock.Object, _assignmentRepositoryMock.Object, _typeRepositoryMock.Object, _mapperMock.Object);
	}

	[SetUp]
	public void Setup()
	{
		_loginFormMock = new Mock<LoginForm>();
		_userMock = new Mock<User>();
	}

	[Test]
	public async Task LoginAsync_ShouldReturnSuccessResponse_WhenCredentialsAreValid()
	{
		// Arrange
		var username = "sonnvb";
		var validPassword = "sonnvb@01012002";
		var login = new LoginForm
		{
			UserName = username,
			Password = validPassword
		};

		var key = Encoding.UTF8.GetBytes(_secret);
		_userMock.Object.UserName = username;
		_userMock.Object.DateOfBirth = new DateTime(2002, 01, 01);

		var mockList = new List<User> { _userMock.Object };
		var mockQueryable = mockList.AsQueryable().BuildMock();

		_userService.EncryptPassword(_userMock.Object, validPassword);


		_userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
						   .Returns(mockQueryable);
		// Act
		var result = await _userService.LoginAsync(login, key);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(StatusCodes.Status200OK);
		result.Message.Should().Be(UserApiResponseMessageConstant.UserLoginSuccess);
		var data = result.Data as ResponseLoginDto;
		data.TokenType.Should().Be("Bearer");
		data.Token.Should().NotBeNullOrEmpty();
		data.IsPasswordChanged.Should().Be(true);
	}

	[Test]
	public async Task LoginAsync_ShouldReturnBadRequest_WhenPasswordIsInvalid()
	{
		// Arrange
		var username = "sonnvb";
		var validPassword = "sonnvb@01012002";
		var invalidPassword = "invalid";
		var login = new LoginForm
		{
			UserName = username,
			Password = invalidPassword
		};

		var key = Encoding.UTF8.GetBytes(_secret);
		//Create userName and dateOfbirth to check if password is system generated
		_userMock.Object.UserName = username;
		_userMock.Object.DateOfBirth = new DateTime(2002, 01, 01);

		var mockList = new List<User> { _userMock.Object };
		var mockQueryable = mockList.AsQueryable().BuildMock();

		_userService.EncryptPassword(_userMock.Object, validPassword);


		_userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
						   .Returns(mockQueryable);
		// Act
		var result = await _userService.LoginAsync(login, key);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		result.Message.Should().Be(UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername);
		result.Data.Should().Be(UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername);
	}

	[Test]
	public async Task LoginAsync_ShouldReturnBadRequest_WhenUsernameNotExist()
	{
		// Arrange
		var username = "sonnvb";
		var validPassword = "sonnvb@01012002";
		var invalidPassword = "invalid";
		var login = new LoginForm
		{
			UserName = username,
			Password = invalidPassword
		};

		var key = Encoding.UTF8.GetBytes(_secret);
		//Create userName and dateOfbirth to check if password is system generated
		_userMock.Object.UserName = username;
		_userMock.Object.DateOfBirth = new DateTime(2002, 01, 01);

		//Make GetByCondition of repository find no user with the following username
		var mockList = new List<User>();
		var mockQueryable = mockList.AsQueryable().BuildMock();

		_userService.EncryptPassword(_userMock.Object, validPassword);


		_userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
						   .Returns(mockQueryable);
		// Act
		var result = await _userService.LoginAsync(login, key);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
		result.Message.Should().Be(UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername);
		result.Data.Should().Be(UserApiResponseMessageConstant.UserLoginWrongPasswordOrUsername);
	}
}
