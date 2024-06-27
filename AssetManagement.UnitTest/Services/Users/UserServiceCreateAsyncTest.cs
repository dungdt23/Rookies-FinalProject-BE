using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using Type = AssetManagement.Domain.Entities.Type;

namespace AssetManagement.UnitTest.Services.Users;

[TestFixture]
public class UserServiceCreateAsyncTest
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IGenericRepository<Assignment>> _assignmentRepositoryMock;
    private Mock<IGenericRepository<Type>> _typeRepositoryMock;
    private Mock<IJwtInvalidationService> _jwtInvalidationServiceMock;
    private Mock<IMapper> _mapperMock;
    private UserService _userService;
    private Mock<User> _userMock;
    private Mock<ResponseUserDto> _userDtoMock;
    private Mock<RequestUserCreateDto> _createFormMock;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _assignmentRepositoryMock = new Mock<IGenericRepository<Assignment>>();
        _typeRepositoryMock = new Mock<IGenericRepository<Type>>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepositoryMock.Object, _assignmentRepositoryMock.Object, _typeRepositoryMock.Object, _mapperMock.Object, _jwtInvalidationServiceMock.Object);
    }

    [SetUp]
    public void Setup()
    {
        _createFormMock = new Mock<RequestUserCreateDto>();
        _userMock = new Mock<User>();
        _userDtoMock = new Mock<ResponseUserDto>();

    }


    [Test]
    public async Task CreateAsync_ShouldReturnSuccessResponse_WhenUserIsCreated()
    {
        // Arrange
        var generatedUserName = "sonnvb";
        var generatedStaffCode = "SD0001";
        _userMock.Object.FirstName = "Nguyen Viet Bao";
        _userMock.Object.LastName = "Son";

        var typeMock = new Mock<Type>();
        var typeListMock = new List<Type> { typeMock.Object };
        var mockTypeQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockTypeQueryable);
        _mapperMock.Setup(m => m.Map<User>(It.IsAny<RequestUserCreateDto>())).Returns(_userMock.Object);
        _userRepositoryMock.Setup(r => r.GenerateStaffCode()).Returns(generatedStaffCode);
        _userRepositoryMock.Setup(r => r.GenerateUserName(It.IsAny<string>())).Returns(generatedUserName);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(1);

        var userListMock = new List<User>() { _userMock.Object };
        var mockUserQueryable = userListMock.AsQueryable().BuildMock();

        _userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mockUserQueryable);
        _mapperMock.Setup(m => m.Map<ResponseUserDto>(It.IsAny<User>())).Returns(_userDtoMock.Object);

        // Act
        var result = await _userService.CreateAsync(_createFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiResponse>();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserCreateSuccess);
        result.Data.Should().BeEquivalentTo(_userDtoMock.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldReturnNotFoundResponse_WhenTypeDoesNotExist()
    {
        // Arrange
        var invalidTypeName = "invalid";
        _createFormMock.Object.Type = invalidTypeName;
        //Create empty list to return type == null
        var typeListMock = new List<Type>();
        var mockQueryable = typeListMock.AsQueryable().AsNoTracking().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);

        // Act
        var result = await _userService.CreateAsync(_createFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiResponse>();
        result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Message.Should().Be(UserApiResponseMessageConstant.TypeNotFound);
        result.Data.Should().BeEquivalentTo(_createFormMock.Object.Type);
    }
    [Test]
    public async Task CreateAsync_ShouldReturnErrorResponse_WhenUserCreationFails()
    {
        // Arrange
        var generatedUserName = "sonnvb";
        var generatedStaffCode = "SD0001";
        _userMock.Object.FirstName = "Nguyen Viet Bao";
        _userMock.Object.LastName = "Son";

        var typeMock = new Mock<Type>();
        var typeListMock = new List<Type> { typeMock.Object };
        var mockQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);
        _mapperMock.Setup(m => m.Map<User>(It.IsAny<RequestUserCreateDto>())).Returns(_userMock.Object);
        _userRepositoryMock.Setup(r => r.GenerateStaffCode()).Returns(generatedStaffCode);
        _userRepositoryMock.Setup(r => r.GenerateUserName(It.IsAny<string>())).Returns(generatedUserName);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(0);
        _mapperMock.Setup(m => m.Map<ResponseUserDto>(It.IsAny<User>())).Returns(_userDtoMock.Object);

        // Act
        var result = await _userService.CreateAsync(_createFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApiResponse>();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserCreateFail);
        result.Data.Should().BeEquivalentTo(_userDtoMock.Object);
    }

}