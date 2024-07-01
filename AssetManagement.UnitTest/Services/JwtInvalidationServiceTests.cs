using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services;
using AssetManagement.Domain.Entities;
using MockQueryable.Moq;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
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
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenUserIdClaimIsMissing()
        {
            // Arrange
            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("BlTimestamp", DateTime.UtcNow.ToString())
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("BlTimestamp", DateTime.UtcNow.ToString())
            });

            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(Enumerable.Empty<User>().AsQueryable().BuildMock());

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenBlTimestampClaimIsMissing()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString())
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenTokenIssuedAtIsInvalid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("BlTimestamp", "invalid_date")
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenTokenIsOlderThanGlobalInvalidationTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var globalTimestamp = DateTime.UtcNow.AddHours(-2);
            var tokenIssuedAt = DateTime.UtcNow.AddHours(-3);
            var user = new User { Id = userId, TokenInvalidationTimestamp = DateTime.UtcNow.AddHours(-1) };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(new GlobalSetting { GlobalInvalidationTimestamp = globalTimestamp });
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("BlTimestamp", tokenIssuedAt.ToString())
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnFalse_WhenTokenIsOlderThanUserInvalidationTimestamp()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var globalTimestamp = DateTime.UtcNow.AddHours(-3);
            var tokenIssuedAt = DateTime.UtcNow.AddHours(-2);
            var user = new User { Id = userId, TokenInvalidationTimestamp = DateTime.UtcNow.AddHours(-1) };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(new GlobalSetting { GlobalInvalidationTimestamp = globalTimestamp });
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("BlTimestamp", tokenIssuedAt.ToString())
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsTokenValidAsync_ShouldReturnTrue_WhenTokenIsValid()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var globalTimestamp = DateTime.UtcNow.AddHours(-3);
            var userTimestamp = DateTime.UtcNow.AddHours(-2);
            var tokenIssuedAt = DateTime.UtcNow;
            var user = new User { Id = userId, TokenInvalidationTimestamp = userTimestamp };
            var mock = new[] { user }.AsQueryable().BuildMock();

            _mockGlobalSettingsRepository.Setup(x => x.GetGlobalSettingAsync()).ReturnsAsync(new GlobalSetting { GlobalInvalidationTimestamp = globalTimestamp });
            _mockUserRepository.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<User, bool>>>())).Returns(mock);

            var jwtToken = new JwtSecurityToken(claims: new[]
            {
                new Claim("id", userId.ToString()),
                new Claim("BlTimestamp", tokenIssuedAt.ToString())
            });

            // Act
            var result = await _jwtInvalidationService.IsTokenValidAsync(jwtToken);

            // Assert
            Assert.True(result);
        }
    }
}
