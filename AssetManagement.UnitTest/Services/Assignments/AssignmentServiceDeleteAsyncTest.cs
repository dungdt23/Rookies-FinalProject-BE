using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
	public class AssignmentServiceDeleteAsyncTest
	{
		private Mock<IAssignmentRepository> _assignmentRepositoryMock;
		private Mock<IAssetRepository> _assetRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private AssignmentService _assignmentService;
		private Mock<ResponseAssignmentDto> _assignmentDtoMock;
		private Mock<Assignment> _assignmentMock;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_assetRepositoryMock = new Mock<IAssetRepository>();
			_assignmentRepositoryMock = new Mock<IAssignmentRepository>();
			_mapperMock = new Mock<IMapper>();
			_assignmentService = new AssignmentService(_assignmentRepositoryMock.Object, _assetRepositoryMock.Object, _mapperMock.Object);
		}


		[SetUp]
		public void SetUp()
		{
			_assignmentMock = new Mock<Assignment>();
			_assignmentDtoMock = new Mock<ResponseAssignmentDto>();
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnNotFound_WhenAssignmentNotFoundWithId()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment>();
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.DeleteAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentNotFound);
			result.Data.Should().Be(id);
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnBadRequest_WhenAssignmentNotWaitingForAcceptance()
		{
			//Arrange
			var id = Guid.NewGuid();
			_assignmentMock.Object.State = TypeAssignmentState.Rejected;
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.DeleteAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentDeleteNotWaitingForAcceptance);
			result.Data.Should().Be(_assignmentMock.Object.State.ToString());
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnError_WhenDeleteAssignmentFail()
		{
			//Arrange
			var id = Guid.NewGuid();
			_assignmentMock.Object.State = TypeAssignmentState.WaitingForAcceptance;
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_assignmentRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(0);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.DeleteAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentDeleteFail);
			result.Data.Should().Be(_assignmentDtoMock.Object);
		}

		[Test]
		public async Task DeleteAsync_ShouldReturnOk_WhenDeleteAssignmentSuccess()
		{
			//Arrange
			var id = Guid.NewGuid();
			_assignmentMock.Object.State = TypeAssignmentState.WaitingForAcceptance;
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_assignmentRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(1);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.DeleteAsync(id);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentDeleteSuccess);
			result.Data.Should().Be(_assignmentDtoMock.Object);
		}
	}
}
