using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services.Assets
{
    public class AssetServiceGetByIdAsyncTest
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
        public async Task GetByIdAsync_ShouldReturnAsset_WhenAssetsAreFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var assets = new List<Asset> {
                new Asset { Id = id, AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false }
            };
            var assetDto = new ResponseAssetDto { AssetName = "Laptop Dell", AssetCode = "LA000001" };

            var x = assets.AsQueryable().BuildMockDbSet().Object;

            _mockAssetRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                    .Returns(assets.AsQueryable().BuildMockDbSet().Object);
            _mockMapper.Setup(mapper => mapper.Map<ResponseAssetDto>(It.IsAny<Asset>()))
            .Returns(assetDto);

            // Act
            var result = await _assetService.GetByIdAysnc(id);

            // Assert
            Assert.AreEqual(assetDto, result.Data);
        }
        [Test]
        public async Task GetByIdAsync_ShouldReturnAsset_WhenAssetsAreNotFound()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var assets = new List<Asset> {
                new Asset { Id = id, AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.UtcNow, IsDeleted = false }
            };
            var assetDto = new ResponseAssetDto {};

            var x = assets.AsQueryable().BuildMockDbSet().Object;

            _mockAssetRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                    .Returns(assets.AsQueryable().BuildMockDbSet().Object);
            _mockMapper.Setup(mapper => mapper.Map<ResponseAssetDto>(assets))
            .Returns(assetDto);

            // Act
            var result = await _assetService.GetByIdAysnc(id);

            // Assert
            Assert.IsNull(result.Data);
        }
    }
}
