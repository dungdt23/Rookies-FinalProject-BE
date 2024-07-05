using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services.Assets
{
    [TestFixture]

    public class AssetServiceUpdateAsyncTest
    {
        private Mock<IAssetRepository> _mockAssetRepository;
        private Mock<IGenericRepository<Category>> _mockCategoryRepository;
        private Mock<IMapper> _mockMapper;
        private AssetService _assetService;
        [SetUp]
        public void Setup()
        {
            _mockAssetRepository = new Mock<IAssetRepository>();
            _mockCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _assetService = new AssetService(_mockAssetRepository.Object, _mockCategoryRepository.Object, _mockMapper.Object);
        }
        [Test]
        public async Task UpdateAsync_ShouldReturnApiResponse_WhenAssetIsUpdatedSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var requestDto = new RequestAssetDto();


            var asset = new Asset { Id = id, AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false };

            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();

            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            var updateAsset = new Asset { Id = id, AssetName = "Laptop Lenovo", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false };

            _mockMapper.Setup(mapper => mapper.Map<Asset>(requestDto))
                .Returns(updateAsset);

            _mockAssetRepository.Setup(repo => repo.UpdateAsync(updateAsset))
                .ReturnsAsync(StatusConstant.Success);

            _mockAssetRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                    .Returns(new List<Asset> { asset }.AsQueryable().BuildMockDbSet().Object);

            var returnAssetDto = new ResponseAssetDto { AssetName = "Laptop Dell", AssetCode = "LA000001" };
            _mockMapper.Setup(mapper => mapper.Map<ResponseAssetDto>(asset))
            .Returns(returnAssetDto);

            // Act
            var result = await _assetService.UpdateAsync(id, requestDto);

            // Assert
            Assert.AreEqual("Update asset successfully", result.Message);
            Assert.IsNotNull(returnAssetDto);
        }
        [Test]
        public async Task UpdateAsync_ShouldReturnApiResponse_WhenAssetIsUpdatedFailed()
        {
            // Arrange
            var id = Guid.NewGuid();
            var requestDto = new RequestAssetDto();
            var asset = new Asset { Id = id, AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            var updateAsset = new Asset { Id = id, AssetName = "Laptop Lenovo", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false };

            _mockMapper.Setup(mapper => mapper.Map<Asset>(requestDto))
                .Returns(updateAsset);

            _mockAssetRepository.Setup(repo => repo.UpdateAsync(updateAsset))
                .ReturnsAsync(StatusConstant.Failed);

            // Act
            var result = await _assetService.UpdateAsync(id, requestDto);

            // Assert
            Assert.AreEqual("Update asset failed", result.Message);
        }
        [Test]
        public async Task UpdateAsync_ShouldReturnApiResponse_WhenAssetIsNotExisted()
        {
            // Arrange
            var id = Guid.NewGuid();
            var requestDto = new RequestAssetDto();
            var typeListMock = new List<Asset> { };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            // Act
            var result = await _assetService.UpdateAsync(id, requestDto);

            // Assert
            Assert.AreEqual("Asset doesn't exist", result.Message);
        }
        [Test]
        public async Task UpdateAsync_ShouldReturnApiResponse_WhenAssetIsAssigned()
        {
            // Arrange
            var id = Guid.NewGuid();
            var requestDto = new RequestAssetDto();
            var asset = new Asset { Id = id, AssetName = "Laptop Dell", State = TypeAssetState.Assigned, AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var typeListMock = new List<Asset> { asset };
            var mockQueryable = typeListMock.AsQueryable().BuildMock();
            _mockAssetRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                .Returns(mockQueryable);

            // Act
            var result = await _assetService.UpdateAsync(id, requestDto);

            // Assert
            Assert.AreEqual("Can't update asset because it is assigned", result.Message);
        }
    }
}
