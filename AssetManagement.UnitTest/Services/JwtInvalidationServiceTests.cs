using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services;
using AssetManagement.Domain.Entities;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using Xunit;
using Assert = Xunit.Assert;

namespace AssetManagement.UnitTest.Services
{
    public class JwtInvalidationServiceTests
    {
        private readonly Mock<IGlobalSettingsRepository> _mockGlobalSettingsRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly JwtInvalidationService _jwtInvalidationService;

        public JwtInvalidationServiceTests()
        {
            _mockGlobalSettingsRepository = new Mock<IGlobalSettingsRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _jwtInvalidationService = new JwtInvalidationService(_mockGlobalSettingsRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task UpdateGlobalInvalidationTimeStampAsync_ShouldUpdateTimestamp()
        {
            // Arrange
            var expectedTimestamp = DateTime.Now;

            // Act
            await _jwtInvalidationService.UpdateGlobalInvalidationTimeStampAsync(expectedTimestamp);

            // Assert
            _mockGlobalSettingsRepository.Verify(x => x.UpdateGlobalInvalidationTimestampAsync(It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetGlobalInvalidationTimestampAsync_ShouldReturnTimestamp()
        {
            // Arrange
            var expectedTimestamp = DateTime.Now;
            var globalSetting = new GlobalSetting { GlobalInvalidationTimestamp = expectedTimestamp };
            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(globalSetting);

            // Act
            var result = await _jwtInvalidationService.GetGlobalInvalidationTimestampAsync();

            // Assert
            Assert.Equal(expectedTimestamp, result);
        }

        [Fact]
        public async Task GetUserInvalidationTimestampAsync_ShouldReturnTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedTimestamp = DateTime.Now;
            var user = new User { Id = userId, TokenInvalidationTimestamp = expectedTimestamp };
            var mock = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            // Act
            var result = await _jwtInvalidationService.GetUserInvalidationTimestampAsync(userId);

            // Assert
            Assert.Equal(expectedTimestamp, result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var globalTimestamp = DateTime.UtcNow.AddHours(-2);
            var userTimestamp = DateTime.UtcNow.AddHours(-1);
            var tokenIssuedAt = DateTime.UtcNow;
            var user = new User { Id = userId, TokenInvalidationTimestamp = userTimestamp };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(new GlobalSetting { GlobalInvalidationTimestamp = globalTimestamp });
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(tokenIssuedAt, userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenTokenIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var globalTimestamp = DateTime.UtcNow.AddHours(-2);
            var userTimestamp = DateTime.UtcNow.AddHours(-1);
            var tokenIssuedAt = DateTime.UtcNow.AddHours(-3);
            var user = new User { Id = userId, TokenInvalidationTimestamp = userTimestamp };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(new GlobalSetting { GlobalInvalidationTimestamp = globalTimestamp });
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(tokenIssuedAt, userId);

            // Assert
            Assert.False(result);
        }
    }
}