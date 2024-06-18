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
using Type = AssetManagement.Domain.Entities.Type;

namespace AssetManagement.UnitTest.Services;

[TestFixture]
public class UserServiceUpdateAsyncTest
{
	private Mock<IUserRepository> _userRepositoryMock;
	private Mock<IGenericRepository<Assignment>> _assignmentRepositoryMock;
	private Mock<IGenericRepository<Domain.Entities.Type>> _typeRepositoryMock;
	private Mock<IMapper> _mapperMock;
	private UserService _userService;
	private Mock<User> _userMock;
	private Mock<CreateUpdateUserForm> _updateFormMock;
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
		_updateFormMock = new Mock<CreateUpdateUserForm>();
		_userMock = new Mock<User>();
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
		_mapperMock.Setup(m => m.Map(It.IsAny<CreateUpdateUserForm>(), It.IsAny<User>()))
								.Callback<CreateUpdateUserForm, User>((src, dest) =>
								{
									dest.FirstName = src.FirstName;
									dest.LastName = src.LastName;
									dest.DateOfBirth = src.DateOfBirth;
									dest.Gender = src.Gender;
									dest.TypeId = typeMock.Object.Id;
									dest.JoinedDate = src.JoinedDate;
									dest.LocationId = src.LocationId;
								});
		_userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(1);

		// Act
		var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(StatusCodes.Status200OK);
		result.Message.Should().Be(UserApiResponseMessageContraint.UserUpdateSuccess);
		result.Data.Should().BeEquivalentTo(_userMock.Object, options => options.ExcludingMissingMembers());
	}

	[Test]
	public async Task UpdateAsync_ShouldReturnErrorResponse_WhenTypeIsNotFound()
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
		result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		result.Message.Should().Be(UserApiResponseMessageContraint.UserUpdateFail);
	}

	[Test]
	public async Task UpdateAsync_ShouldReturnNotFoundResponse_WhenUserIsNotFound()
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
		result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
		result.Message.Should().Be(UserApiResponseMessageContraint.UserNotFound);
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
		_mapperMock.Setup(m => m.Map(It.IsAny<CreateUpdateUserForm>(), It.IsAny<User>()))
								.Callback<CreateUpdateUserForm, User>((src, dest) =>
								{
									dest.FirstName = src.FirstName;
									dest.LastName = src.LastName;
									dest.DateOfBirth = src.DateOfBirth;
									dest.Gender = src.Gender;
									dest.TypeId = typeMock.Object.Id;
									dest.JoinedDate = src.JoinedDate;
									dest.LocationId = src.LocationId;
								});
		_userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(0);

		// Act
		var result = await _userService.UpdateAsync(id, _updateFormMock.Object);

		// Assert
		result.Should().NotBeNull();
		result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		result.Message.Should().Be(UserApiResponseMessageContraint.UserUpdateFail);
		result.Data.Should().BeEquivalentTo(_userMock.Object, options => options.ExcludingMissingMembers());
	}
}

