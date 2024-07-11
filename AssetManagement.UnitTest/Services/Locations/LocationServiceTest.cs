using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.LocationServices;
using AutoMapper;
using MockQueryable.Moq;
using AssetManagement.Domain.Entities;
using Moq;

namespace AssetManagement.UnitTest.Services.Locations
{
    public class LocationServiceTest
    {
        [TestFixture]
        public class LocationServiceTests
        {
            private Mock<IGenericRepository<Location>> _mockLocationRepository;
            private Mock<IMapper> _mockMapper;
            private LocationService _locationService;

            [SetUp]
            public void SetUp()
            {
                _mockLocationRepository = new Mock<IGenericRepository<Location>>();
                _mockMapper = new Mock<IMapper>();
                _locationService = new LocationService(_mockLocationRepository.Object, _mockMapper.Object);
            }

            [Test]
            public async Task GetAllAsync_ReturnsAllLocations()
            {
                // Arrange
                var locations = new List<Location> { new Location { LocationName = "Test Location" } };
                var locationDtos = new List<ResponseLocationDto> { new ResponseLocationDto { LocationName = "Test Location" } };
                var mockQueryable = locations.AsQueryable().BuildMock();
                _mockLocationRepository.Setup(r => r.GetAllAsync(null, null)).Returns(mockQueryable);
                _mockMapper.Setup(m => m.Map<IEnumerable<ResponseLocationDto>>(locations)).Returns(locationDtos);

                // Act
                var result = await _locationService.GetAllAsync(null, null);

                // Assert
                Assert.AreEqual(locationDtos, result.Data);
            }
        }
    }
}
