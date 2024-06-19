using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.UnitTest.Services.Assignments
{
	[TestFixture]
	public class AssignmentServiceGetByIdAsyncTest
	{
		private Mock<IAssignmentRepository> _assignmentRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private AssignmentService _assignmentService;
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
			_assignmentMock = new Mock<Assignment>();
			_assignmentDtoMock = new Mock<ResponseAssignmentDto>();
		}

		[Test]
		public async Task GetByIdAsync_ReturnNotFound_WhenNoAssignmentFoundWithId()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment>();
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.GetByIdAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentNotFound);
			result.Data.Should().BeEquivalentTo(id);
		}

		[Test]
		public async Task GetByIdAsync_ReturnOk_WhenAssignmentFoundWithId()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object};
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.GetByIdAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentGetSuccess);
			result.Data.Should().BeEquivalentTo(_assignmentDtoMock.Object);
		}
	}
}
