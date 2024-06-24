using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using Type = AssetManagement.Domain.Entities.Type;

namespace AssetManagement.UnitTest.Services.Users;

[TestFixture]
public class UserServiceUpdateAsyncTest
{

    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IGenericRepository<Assignment>> _assignmentRepositoryMock;
    private Mock<IGenericRepository<Type>> _typeRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private UserService _userService;
    private Mock<User> _userMock;
    private Mock<ResponseUserDto> _userDtoMock;
    private Mock<RequestUserEditDto> _updateFormMock;
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _assignmentRepositoryMock = new Mock<IGenericRepository<Assignment>>();
        _typeRepositoryMock = new Mock<IGenericRepository<Type>>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_userRepositoryMock.Object, _assignmentRepositoryMock.Object, _typeRepositoryMock.Object, _mapperMock.Object);
    }

    [SetUp]
    public void Setup()
    {
        _updateFormMock = new Mock<RequestUserEditDto>();
        _userMock = new Mock<User>();
        _userDtoMock = new Mock<ResponseUserDto>();
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnSuccessResponse_WhenUserIsUpdatedSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var typeMock = new Mock<Type>();
        var typeListMock = new List<Type> { typeMock.Object };
        var mockQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);

        _userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                           .Returns(new List<User> { _userMock.Object }.AsQueryable());
        _mapperMock.Setup(m => m.Map(It.IsAny<RequestUserEditDto>(), It.IsAny<User>()))
                                .Callback<RequestUserEditDto, User>((src, dest) =>
                                {
                                    dest.DateOfBirth = src.DateOfBirth;
                                    dest.Gender = src.Gender;
                                    dest.TypeId = typeMock.Object.Id;
                                    dest.JoinedDate = src.JoinedDate;
                                });
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(1);
        _mapperMock.Setup(r => r.Map<ResponseUserDto>(It.IsAny<User>())).Returns(_userDtoMock.Object);
        // Act
        var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status200OK);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserUpdateSuccess);
        result.Data.Should().BeEquivalentTo(_userDtoMock.Object, options => options.ExcludingMissingMembers());
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnBadRequestResponse_WhenTypeIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var typeListMock = new List<Type>();
        var mockQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);

        // Act
        var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserUpdateFail);
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnBadRequestResponse_WhenUserIsNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var typeMock = new Mock<Type>();
        var typeListMock = new List<Type> { typeMock.Object };
        var mockQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);

        _userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                           .Returns(Enumerable.Empty<User>().AsQueryable());

        // Act
        var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserNotFound);
    }

    [Test]
    public async Task UpdateAsync_ShouldReturnErrorResponse_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var typeMock = new Mock<Type>();
        var typeListMock = new List<Type> { typeMock.Object };
        var mockQueryable = typeListMock.AsQueryable().BuildMock();

        _typeRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Type, bool>>>())).Returns(mockQueryable);

        _userRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                           .Returns(new List<User> { _userMock.Object }.AsQueryable());
        _mapperMock.Setup(m => m.Map(It.IsAny<RequestUserEditDto>(), It.IsAny<User>()))
                                .Callback<RequestUserEditDto, User>((src, dest) =>
                                {
                                    dest.DateOfBirth = src.DateOfBirth;
                                    dest.Gender = src.Gender;
                                    dest.TypeId = typeMock.Object.Id;
                                    dest.JoinedDate = src.JoinedDate;
                                });
        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(0);
        _mapperMock.Setup(r => r.Map<ResponseUserDto>(It.IsAny<User>())).Returns(_userDtoMock.Object);

        // Act
        var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.Message.Should().Be(UserApiResponseMessageConstant.UserUpdateFail);
        result.Data.Should().BeEquivalentTo(_userDtoMock.Object, options => options.ExcludingMissingMembers());
    }

}

