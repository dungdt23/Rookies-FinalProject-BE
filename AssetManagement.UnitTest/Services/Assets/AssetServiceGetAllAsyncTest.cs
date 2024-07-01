using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Moq;

namespace AssetManagement.UnitTest.Services.Assets
{
    [TestFixture]
    public class AssetServiceGetAllAsyncTest
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
        public async Task GetAllAsync_ShouldReturnPagedResponse_WhenAssetsAreFound()
        {
            // Arrange
            var filter = new AssetFilter();
            var assets = new List<Asset> {
                new Asset { AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.Now, IsDeleted = false }
            };
            Guid locationId = Guid.NewGuid();
            var assetDtos = new List<ResponseAssetDto> { new ResponseAssetDto { AssetName = "Laptop Dell", AssetCode = "LA000001" } };

            _mockAssetRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<Asset, object>>(), locationId, filter, 1, 10))
                .ReturnsAsync(assets);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ResponseAssetDto>>(assets))
                .Returns(assetDtos);

            _mockAssetRepository.Setup(repo => repo.GetTotalCountAsync(locationId, filter))
                .ReturnsAsync(1);

            // Act
            var result = await _assetService.GetAllAsync(locationId, filter, 1, 10);

            // Assert
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual("Get asset list successfully!", result.Message);
            Assert.AreEqual(assetDtos, result.Data);
        }
        [Test]
        public async Task GetAllAsync_ShouldReturnPagedResponse_WhenAssetListAreEmpty()
        {
            // Arrange
            var filter = new AssetFilter();
            var assets = new List<Asset> { };
            var assetDtos = new List<ResponseAssetDto> { };
            Guid locationId = Guid.NewGuid();
            _mockAssetRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<Asset, object>>(), locationId, filter, 1, 10))
                .ReturnsAsync(assets);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ResponseAssetDto>>(assets))
                .Returns(assetDtos);

            _mockAssetRepository.Setup(repo => repo.GetTotalCountAsync(locationId, filter))
                .ReturnsAsync(0);

            // Act
            var result = await _assetService.GetAllAsync(locationId, filter, 1, 10);

            // Assert
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual("List asset is empty", result.Message);
            Assert.AreEqual(assetDtos, result.Data);
        }
        [Test]
        public async Task GetAllAsync_ShouldReturnPagedResponse_WhenSortByAssetName()
        {
            // Arrange
            var filter = new AssetFilter();
            filter.sort = AssetSort.AssetName;
            var assets = new List<Asset> { };
            var assetDtos = new List<ResponseAssetDto> { };
            Guid locationId = Guid.NewGuid();
            _mockAssetRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<Asset, object>>(), locationId, filter, 1, 10))
                .ReturnsAsync(assets);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ResponseAssetDto>>(assets))
                .Returns(assetDtos);

            _mockAssetRepository.Setup(repo => repo.GetTotalCountAsync(locationId, filter))
                .ReturnsAsync(0);

            // Act
            var result = await _assetService.GetAllAsync(locationId, filter, 1, 10);

            // Assert
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual("List asset is empty", result.Message);
            Assert.AreEqual(assetDtos, result.Data);
        }
        [Test]
        public async Task GetAllAsync_ShouldReturnPagedResponse_WhenSortByCategoryName()
        {
            // Arrange
            var filter = new AssetFilter();
            filter.sort = AssetSort.CategoryName;
            var assets = new List<Asset> { };
            var assetDtos = new List<ResponseAssetDto> { };
            Guid locationId = Guid.NewGuid();
            _mockAssetRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<Asset, object>>(), locationId, filter, 1, 10))
                .ReturnsAsync(assets);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ResponseAssetDto>>(assets))
                .Returns(assetDtos);

            _mockAssetRepository.Setup(repo => repo.GetTotalCountAsync(locationId, filter))
                .ReturnsAsync(0);

            // Act
            var result = await _assetService.GetAllAsync(locationId, filter, 1, 10);

            // Assert
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual("List asset is empty", result.Message);
            Assert.AreEqual(assetDtos, result.Data);
        }
        [Test]
        public async Task GetAllAsync_ShouldReturnPagedResponse_WhenSortByState()
        {
            // Arrange
            var filter = new AssetFilter();
            filter.sort = AssetSort.State;
            var assets = new List<Asset> { };
            var assetDtos = new List<ResponseAssetDto> { };
            Guid locationId = Guid.NewGuid();
            _mockAssetRepository.Setup(repo => repo.GetAllAsync(It.IsAny<Func<Asset, object>>(), locationId, filter, 1, 10))
                .ReturnsAsync(assets);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ResponseAssetDto>>(assets))
                .Returns(assetDtos);

            _mockAssetRepository.Setup(repo => repo.GetTotalCountAsync(locationId, filter))
                .ReturnsAsync(0);

            // Act
            var result = await _assetService.GetAllAsync(locationId, filter, 1, 10);

            // Assert
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual("List asset is empty", result.Message);
            Assert.AreEqual(assetDtos, result.Data);
        }
    }
}
