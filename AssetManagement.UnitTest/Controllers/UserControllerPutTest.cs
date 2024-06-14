using AssetManagement.Api.Controllers;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;
using Moq;

namespace AssetManagement.UnitTest.Controllers;
[TestFixture]
public class UserControllerPutTest
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
}
