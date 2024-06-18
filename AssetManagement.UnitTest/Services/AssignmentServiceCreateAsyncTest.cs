using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AssetManagement.UnitTest.Services
{
    [TestFixture]
	public class AssignmentServiceCreateAsyncTest
	{
		private  Mock<IGenericRepository<Assignment>> _assignmentRepositoryMock;
		private  Mock<IMapper> _mapperMock;
		private  AssignmentService _assignmentService;
		private Mock<RequestAssignmentDto> _requestDtoMock;
		private Mock<Assignment> _assignmentMock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_assignmentRepositoryMock = new Mock<IGenericRepository<Assignment>>();
			_mapperMock = new Mock<IMapper>();
			_assignmentService = new AssignmentService(_assignmentRepositoryMock.Object, _mapperMock.Object);
		}

		[SetUp]
		public void SetUp()
		{
			_requestDtoMock = new Mock<RequestAssignmentDto>();
			_assignmentMock = new Mock<Assignment>();
		}

		[Test]
		public async Task CreateAsync_ShouldReturnOk_WhenAssignmentIsCreatedSuccessfully()
		{
			// Arrange
			_mapperMock.Setup(m => m.Map<Assignment>(It.IsAny<RequestAssignmentDto>())).Returns(_assignmentMock.Object);
			_assignmentRepositoryMock.Setup(ar => ar.AddAsync(It.IsAny<Assignment>())).ReturnsAsync(1);

			// Act
			var result = await _assignmentService.CreateAsync(_requestDtoMock.Object);

			// Assert
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageContraint.AssignmentCreateSuccess);
			result.Data.Should().Be(_assignmentMock.Object);
		}

		[Test]
		public async Task CreateAsync_ShouldReturnStatus500InternalServerError_WhenCreationFails()
		{
			// Arrange
			_mapperMock.Setup(m => m.Map<Assignment>(It.IsAny<RequestAssignmentDto>())).Returns(_assignmentMock.Object);
			_assignmentRepositoryMock.Setup(ar => ar.AddAsync(It.IsAny<Assignment>())).ReturnsAsync(0);

			// Act
			var result = await _assignmentService.CreateAsync(_requestDtoMock.Object);

			// Assert
			result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			result.Message.Should().Be(AssignmentApiResponseMessageContraint.AssignmentCreateFail);
			result.Data.Should().Be(_assignmentMock.Object);
		}
	}
}
