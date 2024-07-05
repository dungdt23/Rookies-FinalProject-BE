using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
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
        public async Task AddAsync_ShouldReturnApiResponse_WhenAssetIsAddedSuccessfully()
        {
            // Arrange
            var assetDto = new RequestAssetDto { AssetName = "Laptop Dell", CategoryId = Guid.NewGuid() };
            var asset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var category = new Category { Id = assetDto.CategoryId, CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var typeListMock = new List<Category> { category }.AsQueryable().BuildMock();

            _mockCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(typeListMock);

            _mockMapper.Setup(mapper => mapper.Map<Asset>(assetDto))
                .Returns(asset);

            _mockAssetRepository.Setup(repo => repo.CreateAssetCode(category.Prefix, category.Id))
                .Returns("LA000001");

            _mockAssetRepository.Setup(repo => repo.AddAsync(asset))
                .ReturnsAsync(StatusConstant.Success);

            var returnAsset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };

            _mockAssetRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Asset, bool>>>()))
                                .Returns(new List<Asset> { asset }.AsQueryable().BuildMockDbSet().Object);

            var returnAssetDto = new ResponseAssetDto { AssetName = "Laptop Dell"};
            _mockMapper.Setup(mapper => mapper.Map<ResponseAssetDto>(asset))
            .Returns(returnAssetDto);

            // Act
            var result = await _assetService.AddAsync(assetDto);

            // Assert
            Assert.AreEqual("Add new asset successfully", result.Message);
            Assert.IsNotNull(returnAssetDto);
        }

        [Test]
        public async Task AddAsync_ShouldReturnApiResponse_WhenAssetIsAddedFailed()
        {
            // Arrange
            var assetDto = new RequestAssetDto { AssetName = "Laptop Dell", CategoryId = Guid.NewGuid() };
            var asset = new Asset { AssetName = "Laptop Dell", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var category = new Category { Id = assetDto.CategoryId, CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.UtcNow, IsDeleted = false };
            var typeListMock = new List<Category> { category }.AsQueryable().BuildMock();

            _mockCategoryRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(typeListMock);

            _mockMapper.Setup(mapper => mapper.Map<Asset>(assetDto))
                .Returns(asset);

            _mockAssetRepository.Setup(repo => repo.CreateAssetCode(category.Prefix, category.Id))
                .Returns("LA000001");

            _mockAssetRepository.Setup(repo => repo.AddAsync(asset))
                .ReturnsAsync(StatusConstant.Failed);

            // Act
            var result = await _assetService.AddAsync(assetDto);

            // Assert
            Assert.AreEqual("Add new asset failed", result.Message);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
        }
    }
}
