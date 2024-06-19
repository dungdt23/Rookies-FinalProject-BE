using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services.Assignments
{
    [TestFixture]

    public class AssignmentServiceCreateAsyncTest
    {
        private Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private AssignmentService _assignmentService;
        private Mock<RequestAssignmentDto> _requestDtoMock;
        private Mock<ResponseAssignmentDto> _assignmentDtoMock;
        private Mock<Assignment> _assignmentMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _assignmentService = new AssignmentService(_assignmentRepositoryMock.Object, _mapperMock.Object);
        }


        [SetUp]
        public void SetUp()
        {
            _requestDtoMock = new Mock<RequestAssignmentDto>();
            _assignmentMock = new Mock<Assignment>();
            _assignmentDtoMock = new Mock<ResponseAssignmentDto>();
        }

        [Test]
        public async Task CreateAsync_ShouldReturnOk_WhenAssignmentIsCreatedSuccessfully()
        {
            // Arrange
            _mapperMock.Setup(m => m.Map<Assignment>(It.IsAny<RequestAssignmentDto>())).Returns(_assignmentMock.Object);
            _assignmentRepositoryMock.Setup(ar => ar.AddAsync(It.IsAny<Assignment>())).ReturnsAsync(1);

            var assignmentListMock = new List<Assignment> { _assignmentMock.Object };
            var assignmentQueryMock = assignmentListMock.AsQueryable().BuildMock();

            _assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentQueryMock);
            _mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

            // Act
            var result = await _assignmentService.CreateAsync(_requestDtoMock.Object);


            // Assert
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentCreateSuccess);
            result.Data.Should().Be(_assignmentDtoMock.Object);
        }


        [Test]
        public async Task CreateAsync_ShouldReturnStatus500InternalServerError_WhenCreationFails()
        {
            // Arrange
            _mapperMock.Setup(m => m.Map<Assignment>(It.IsAny<RequestAssignmentDto>())).Returns(_assignmentMock.Object);
            _assignmentRepositoryMock.Setup(ar => ar.AddAsync(It.IsAny<Assignment>())).ReturnsAsync(0);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			// Act
			var result = await _assignmentService.CreateAsync(_requestDtoMock.Object);


            // Assert
            result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentCreateFail);
            result.Data.Should().Be(_assignmentDtoMock.Object);
        }
    }

}
