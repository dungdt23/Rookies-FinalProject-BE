using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Moq;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services
{
    [TestFixture]
    public class AssetServiceAddAsyncTest
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
        public async Task AddAsync_ShouldReturnApiResonse_WhenAssetIsAddedSuccessfully()
        {
            // Arrange
            var assetDto = new RequestAssetDto { AssetName = "Laptop Dell" };
            var asset = new Asset { AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.Now, IsDeleted = false };
            var category = new Category { CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.Now, IsDeleted = false };
            _mockCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                 .Returns(new List<Category> { category }.AsQueryable());
            _mockMapper.Setup(mapper => mapper.Map<RequestAssetDto>(asset))
                .Returns(assetDto);

            _mockAssetRepository.Setup(repo => repo.AddAsync(asset))
                .ReturnsAsync(StatusConstant.Success);

            // Act
            var result = await _assetService.AddAsync(assetDto);

            // Assert
            Assert.AreEqual("Get asset list successfully!", result.Message);
            Assert.AreEqual(assetDto, result.Data);
        }
        [Test]
        public async Task AddAsync_ShouldReturnApiResonse_WhenAssetIsAddedFailed()
        {
            // Arrange
            var assetDto = new RequestAssetDto { AssetName = "Laptop Dell" };
            var asset = new Asset { AssetName = "Laptop Dell", AssetCode = "LA000001", CreatedAt = DateTime.Now, IsDeleted = false };
            var category = new Category { CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.Now, IsDeleted = false };
            _mockCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                 .Returns(new List<Category> { category }.AsQueryable());
            _mockMapper.Setup(mapper => mapper.Map<RequestAssetDto>(asset))
                .Returns(assetDto);

            _mockAssetRepository.Setup(repo => repo.AddAsync(asset))
                .ReturnsAsync(StatusConstant.Success);

            // Act
            var result = await _assetService.AddAsync(assetDto);

            // Assert
            Assert.AreEqual("Get asset list successfully!", result.Message);
            Assert.AreEqual(assetDto, result.Data);
        }
    }
}
