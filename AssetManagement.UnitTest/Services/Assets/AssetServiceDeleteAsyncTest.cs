using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services.Assets
{
    [TestFixture]
    public class AssetServiceDeleteAsyncTest
    {
        private Mock<IAssetRepository> _mockAssetRepository;
        private Mock<IGenericRepository<Category>> _mockCategoryRepository;
        private Mock<IMapper> _mockMapper;
        private AssetService _assetService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _mockAssetRepository = new Mock<IAssetRepository>();
            _mockCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _assetService = new AssetService(_mockAssetRepository.Object, _mockCategoryRepository.Object, _mockMapper.Object);
        }
        [Test]
        public async Task DeleteAsync_ShouldReturnApiResponse_WhenAssetIsDeletedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var asset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var assignments = new List<Assignment>();
            asset.Assignments = assignments;
            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            _mockAssetRepository.Setup(repo => repo.DeleteAsync(id))
                .ReturnsAsync(StatusConstant.Success);

            // Act
            var result = await _assetService.DeleteAsync(id);

            // Assert
            Assert.AreEqual("Delete asset successfully", result.Message);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnApiResponse_WhenAssetHasHistoricalAssignment()
        {
            // Arrange
            var id = Guid.NewGuid();
            var asset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var assignments = new List<Assignment>();
            assignments.Add(new Assignment { AssigneeId = Guid.NewGuid(), AssignerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, IsDeleted = false });
            asset.Assignments = assignments;
            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            // Act
            var result = await _assetService.DeleteAsync(id);

            // Assert
            Assert.AreEqual("Can not be deleted! Asset belong to an historical assignment", result.Message);
            Assert.AreEqual(StatusCodes.Status409Conflict, result.StatusCode);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnApiResponse_WhenAssetIsDeletedFailed()
        {
            // Arrange
            var id = Guid.NewGuid();
            var asset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var assignments = new List<Assignment>();
            asset.Assignments = assignments;
            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            _mockAssetRepository.Setup(repo => repo.DeleteAsync(id))
                .ReturnsAsync(StatusConstant.Failed);


            // Act
            var result = await _assetService.DeleteAsync(id);


            // Assert
            Assert.AreEqual("Delete asset failed", result.Message);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
        [Test]
        public async Task DeleteAsync_ShouldReturnApiResponse_WhenAssetIsNotExisted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var typeListMock = new List<Asset> { };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);
            _mockAssetRepository.Setup(repo => repo.DeleteAsync(id))
                .ReturnsAsync(StatusConstant.Failed);


            // Act
            var result = await _assetService.DeleteAsync(id);


            // Assert
            Assert.AreEqual("Asset doesn't exist", result.Message);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}
