using AssetManagement.Application.Dtos.ReturnRequest;
using AssetManagement.Application.Exceptions.Assignment;
using AssetManagement.Application.Exceptions.Common;
using AssetManagement.Application.Exceptions.ReturnRequest;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;
using Xunit;
using Assert = Xunit.Assert;

namespace AssetManagement.UnitTest.Services
{
    public class ReturnRequestServiceTests
    {
        private readonly Mock<IReturnRequestRepository> _mockReturnRequestRepository;
        private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly IMapper _mapper;
        private readonly ReturnRequestService _service;

        public ReturnRequestServiceTests()
        {
            _mockReturnRequestRepository = new Mock<IReturnRequestRepository>();
            _mockAssignmentRepository = new Mock<IAssignmentRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReturnRequest, ReturnRequestViewModel>();
                cfg.CreateMap<ReturnRequest, ReturnRequestGetAllViewModel>();
            });
            _mapper = config.CreateMapper();

            _service = new ReturnRequestService(
                _mockReturnRequestRepository.Object,
                _mockAssignmentRepository.Object,
                _mapper,
                _mockUserRepository.Object,
                _mockTransactionRepository.Object);
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldThrowNotFoundException_WhenAssignmentNotFound()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();

            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(Enumerable.Empty<Assignment>().AsQueryable().BuildMock());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateReturnRequestAsync(request, userId));
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldThrowWrongLocationException_WhenLocationMismatched()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = request.AssignmentId,
                Asset = new Asset { LocationId = Guid.NewGuid() }
            };
            var user = new User { Id = userId, LocationId = Guid.NewGuid() };

            var mockAssignments = new[] { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(mockAssignments);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<WrongLocationException>(() => _service.CreateReturnRequestAsync(request, userId));
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldThrowUnauthorizedAssignmentAccessException_WhenStaffTriesToCreateForOtherAssignment()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = request.AssignmentId,
                AssigneeId = Guid.NewGuid(), // Different from userId
                Asset = new Asset { LocationId = Guid.NewGuid() }
            };
            var user = new User { Id = userId, LocationId = assignment.Asset.LocationId, Type = new Domain.Entities.Type { TypeName = TypeNameConstants.TypeStaff } };

            var mockAssignments = new[] { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(mockAssignments);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAssignmentAccessException>(() => _service.CreateReturnRequestAsync(request, userId));
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldThrowAssignmentNotAcceptedException_WhenAssignmentNotAccepted()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = request.AssignmentId,
                State = TypeAssignmentState.WaitingForAcceptance, // Not accepted
                AssigneeId = userId,
                Asset = new Asset { LocationId = Guid.NewGuid() }
            };
            var user = new User { Id = userId, LocationId = assignment.Asset.LocationId, Type = new Domain.Entities.Type { TypeName = TypeNameConstants.TypeStaff } };

            var mockAssignments = new[] { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(mockAssignments);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<AssignmentNotAcceptedException>(() => _service.CreateReturnRequestAsync(request, userId));
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldThrowActiveReturnRequestAlreadyExistsException_WhenActiveReturnRequestExists()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = request.AssignmentId,
                State = TypeAssignmentState.Accepted,
                AssigneeId = userId,
                ActiveReturnRequestId = Guid.NewGuid(), // Active return request exists
                Asset = new Asset { LocationId = Guid.NewGuid() }
            };
            var user = new User { Id = userId, LocationId = assignment.Asset.LocationId, Type = new Domain.Entities.Type { TypeName = TypeNameConstants.TypeStaff } };

            var mockAssignments = new[] { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(mockAssignments);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<ActiveReturnRequestAlreadyExistsException>(() => _service.CreateReturnRequestAsync(request, userId));
        }

        [Fact]
        public async Task CreateReturnRequestAsync_ShouldCreateReturnRequestSuccessfully()
        {
            // Arrange
            var request = new CreateReturnRequestRequest { AssignmentId = Guid.NewGuid() };
            var userId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = request.AssignmentId,
                State = TypeAssignmentState.Accepted,
                AssigneeId = userId,
                Asset = new Asset { LocationId = Guid.NewGuid() }
            };
            var user = new User { Id = userId, LocationId = assignment.Asset.LocationId, Type = new Domain.Entities.Type { TypeName = TypeNameConstants.TypeStaff } };

            var mockAssignments = new[] { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>()))
                .Returns(mockAssignments);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            _mockReturnRequestRepository.Setup(repo => repo.AddAsync(It.IsAny<ReturnRequest>())).Returns(Task.FromResult(0));
            _mockAssignmentRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Assignment>())).Returns(Task.FromResult(0));
            _mockTransactionRepository.Setup(repo => repo.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateReturnRequestAsync(request, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.AssignmentId, result.AssignmentId);
            _mockReturnRequestRepository.Verify(repo => repo.AddAsync(It.IsAny<ReturnRequest>()), Times.Once);
            _mockAssignmentRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Assignment>()), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteReturnRequestAsync_ShouldThrowNotFoundException_WhenReturnRequestNotFound()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var mockReturnRequests = Enumerable.Empty<ReturnRequest>().AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.CompleteReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task CompleteReturnRequestAsync_ShouldThrowWrongLocationException_WhenLocationMismatched()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.WaitingForReturning,
                Assignment = new Assignment { Asset = new Asset() }
            };
            var user = new User { Id = requesterId, LocationId = Guid.NewGuid() };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<WrongLocationException>(() => _service.CompleteReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task CompleteReturnRequestAsync_ShouldThrowReturnRequestNotWaitingException_WhenReturnRequestNotWaiting()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.Completed,
                Assignment = new Assignment { Asset = new Asset() }
            };
            var user = new User { Id = requesterId, LocationId = returnRequest.LocationId };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<ReturnRequestNotWaitingException>(() => _service.CompleteReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task CompleteReturnRequestAsync_ShouldCompleteReturnRequestSuccessfully()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.WaitingForReturning,
                Assignment = new Assignment
                {
                    Asset = new Asset(),
                    IsDeleted = false
                }
            };
            var user = new User { Id = requesterId, LocationId = returnRequest.LocationId };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            _mockReturnRequestRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ReturnRequest>()))
                .Returns(Task.FromResult(0));
            _mockTransactionRepository.Setup(repo => repo.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.CompleteReturnRequestAsync(returnRequestId, requesterId);

            // Assert
            Assert.Equal(TypeRequestState.Completed, returnRequest.State);
            Assert.NotNull(returnRequest.ReturnedDate);
            Assert.Equal(user.Id, returnRequest.ResponderId);
            Assert.Equal(TypeAssetState.Available, returnRequest.Assignment.Asset.State);
            Assert.True(returnRequest.Assignment.IsDeleted);
            _mockReturnRequestRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ReturnRequest>()), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task RejectReturnRequestAsync_ShouldThrowNotFoundException_WhenReturnRequestNotFound()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();

            var mockReturnRequests = Enumerable.Empty<ReturnRequest>().AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.RejectReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task RejectReturnRequestAsync_ShouldThrowWrongLocationException_WhenLocationMismatched()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.WaitingForReturning,
                Assignment = new Assignment { Asset = new Asset() }
            };
            var user = new User { Id = requesterId, LocationId = Guid.NewGuid() };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<WrongLocationException>(() => _service.RejectReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task RejectReturnRequestAsync_ShouldThrowReturnRequestNotWaitingException_WhenReturnRequestNotWaiting()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.Completed,
                Assignment = new Assignment { Asset = new Asset() }
            };
            var user = new User { Id = requesterId, LocationId = returnRequest.LocationId };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            // Act & Assert
            await Assert.ThrowsAsync<ReturnRequestNotWaitingException>(() => _service.RejectReturnRequestAsync(returnRequestId, requesterId));
        }

        [Fact]
        public async Task RejectReturnRequestAsync_ShouldRejectReturnRequestSuccessfully()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                LocationId = Guid.NewGuid(),
                State = TypeRequestState.WaitingForReturning,
                Assignment = new Assignment { Asset = new Asset() }
            };
            var user = new User { Id = requesterId, LocationId = returnRequest.LocationId };

            var mockReturnRequests = new[] { returnRequest }.AsQueryable().BuildMock();
            _mockReturnRequestRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<ReturnRequest, bool>>>()))
                .Returns(mockReturnRequests);

            var mockUsers = new[] { user }.AsQueryable().BuildMock();
            _mockUserRepository.Setup(repo => repo.GetByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(mockUsers);

            _mockReturnRequestRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ReturnRequest>()))
                .Returns(Task.FromResult(0));
            _mockTransactionRepository.Setup(repo => repo.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockTransactionRepository.Setup(repo => repo.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.RejectReturnRequestAsync(returnRequestId, requesterId);

            // Assert
            Assert.Equal(TypeRequestState.Rejected, returnRequest.State);
            Assert.NotNull(returnRequest.ReturnedDate);
            Assert.Equal(user.Id, returnRequest.ResponderId);
            Assert.Null(returnRequest.Assignment.ActiveReturnRequestId);
            _mockReturnRequestRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ReturnRequest>()), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.CommitTransactionAsync(), Times.Once);
        }
    }
}