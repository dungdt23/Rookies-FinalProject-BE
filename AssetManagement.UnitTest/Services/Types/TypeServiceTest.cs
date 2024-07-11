using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.TypeServices;
using AssetManagement.Domain.Entities;
using AutoMapper;
using MockQueryable.Moq;
using Moq;

namespace AssetManagement.UnitTest.Services.Types
{
    [TestFixture]
    public class TypeServiceTests
    {
        private Mock<IGenericRepository<Domain.Entities.Type>> _mockTypeRepository;
        private Mock<IMapper> _mockMapper;
        private TypeService _typeService;

        [SetUp]
        public void SetUp()
        {
            _mockTypeRepository = new Mock<IGenericRepository<Domain.Entities.Type>>();
            _mockMapper = new Mock<IMapper>();
            _typeService = new TypeService(_mockTypeRepository.Object, _mockMapper.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllTypes()
        {
            // Arrange
            var types = new List<Domain.Entities.Type> { new Domain.Entities.Type { TypeName = "test" } };
            var typeDtos = new List<ResponseTypeDto> { new ResponseTypeDto { TypeName = "test" } };
            var mockQueryable = types.AsQueryable().BuildMock();
            _mockTypeRepository.Setup(r => r.GetAllAsync(null, null)).Returns(mockQueryable);
            _mockMapper.Setup(m => m.Map<IEnumerable<ResponseTypeDto>>(types)).Returns(typeDtos);

            // Act
            var result = await _typeService.GetAllAsync(null, null);

            // Assert
            Assert.AreEqual(typeDtos, result.Data);
        }
    }
}
