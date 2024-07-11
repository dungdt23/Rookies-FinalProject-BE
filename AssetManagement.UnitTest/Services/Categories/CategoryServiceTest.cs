using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.ICategoryServices;
using AssetManagement.Application.Services.CategoryServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;

namespace AssetManagement.UnitTest.Services.Categories
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IGenericRepository<Category>> _mockCategoryRepository;
        private Mock<IMapper> _mockMapper;
        private ICategoryService _categoryService;

        [SetUp]
        public void SetUp()
        {
            _mockCategoryRepository = new Mock<IGenericRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockMapper.Object);
        }

        [Test]
        public async Task AddAsync_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var requestCategoryDto = new RequestCategoryDto();
            var category = new Category();
            _mockMapper.Setup(m => m.Map<Category>(requestCategoryDto)).Returns(category);
            _mockCategoryRepository.Setup(r => r.AddAsync(category)).ReturnsAsync(StatusConstant.Success);

            // Act
            var result = await _categoryService.AddAsync(requestCategoryDto);

            // Assert
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
            Assert.AreEqual("Add new category successfully!", result.Message);
            Assert.AreEqual(category, result.Data);
        }

        [Test]
        public async Task AddAsync_Failure_ReturnsErrorResponse()
        {
            // Arrange
            var requestCategoryDto = new RequestCategoryDto();
            var category = new Category();
            _mockMapper.Setup(m => m.Map<Category>(requestCategoryDto)).Returns(category);
            _mockCategoryRepository.Setup(r => r.AddAsync(category)).ReturnsAsync(StatusConstant.Failed);

            // Act
            var result = await _categoryService.AddAsync(requestCategoryDto);

            // Assert
            Assert.AreEqual(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.AreEqual("Add new category failed!", result.Message);
        }

        [Test]
        public async Task IsUniqueAsync_PrefixIsDuplicated_ReturnsErrorResponse()
        {
            // Arrange
            var prefixName = "test";
            var category = new Category { Prefix = prefixName };
            _mockCategoryRepository.Setup(r => r.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .Returns(new List<Category> { category }.AsQueryable().BuildMock());

            // Act
            var result = await _categoryService.IsUniqueAsync(true, prefixName);

            // Assert
            Assert.AreEqual(false, result.Data);
            Assert.AreEqual("Category prefix is duplicated!", result.Message);
        }

        [Test]
        public async Task IsUniqueAsync_PrefixIsUnique_ReturnsSuccessResponse()
        {
            // Arrange
            var prefixName = "test";
            _mockCategoryRepository.Setup(r => r.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .Returns(new List<Category>().AsQueryable().BuildMock());

            // Act
            var result = await _categoryService.IsUniqueAsync(true, prefixName);

            // Assert
            Assert.AreEqual(true, result.Data);
            Assert.IsEmpty(result.Message);
        }

        [Test]
        public async Task IsUniqueAsync_NameIsDuplicated_ReturnsErrorResponse()
        {
            // Arrange
            var name = "test";
            var category = new Category { CategoryName = name };
            _mockCategoryRepository.Setup(r => r.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .Returns(new List<Category> { category }.AsQueryable().BuildMock());

            // Act
            var result = await _categoryService.IsUniqueAsync(false, name);

            // Assert
            Assert.AreEqual(false, result.Data);
            Assert.AreEqual("Category name is duplicated!", result.Message);
        }

        [Test]
        public async Task IsUniqueAsync_NameIsUnique_ReturnsSuccessResponse()
        {
            // Arrange
            var name = "test";
            _mockCategoryRepository.Setup(r => r.GetByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Category, bool>>>()))
                .Returns(new List<Category>().AsQueryable().BuildMock());

            // Act
            var result = await _categoryService.IsUniqueAsync(false, name);

            // Assert
            Assert.AreEqual(true, result.Data);
            Assert.IsEmpty(result.Message);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category> { new Category { CategoryName = "test" } };
            var categoryDtos = new List<ResponseCategoryDto> { new ResponseCategoryDto { CategoryName = "test" } };
            _mockCategoryRepository.Setup(r => r.GetAllAsync(null, null)).Returns(categories.AsQueryable().BuildMock());
            _mockMapper.Setup(m => m.Map<IEnumerable<ResponseCategoryDto>>(categories)).Returns(categoryDtos);

            // Act
            var result = await _categoryService.GetAllAsync(null, null);

            // Assert
            Assert.AreEqual(categoryDtos, result.Data);
        }
    }
}
