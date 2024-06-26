using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using Azure.Core;
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
	public class AssignmentServiceRespondAsyncTest
	{
		private Mock<IAssignmentRepository> _assignmentRepositoryMock;
		private Mock<IAssetRepository> _assetRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private AssignmentService _assignmentService;
		private Mock<ResponseAssignmentDto> _assignmentDtoMock;
		private Mock<Assignment> _assignmentMock;
		private Mock<RequestAssignmentRespondDto> _requestDtoMock;

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
			_requestDtoMock = new Mock<RequestAssignmentRespondDto>();
		}

		[Test]
		public async Task RespondAsync_ShouldReturnNotFound_WhenAssignmentNotFoundWithId()
		{
			//Arrange
			var assignmentsMock = new List<Assignment>();
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.RespondAsync(_requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentNotFound);
			result.Data.Should().Be($"Assignment Id : {_requestDtoMock.Object.AssignmentId}");
		}

		[Test]
		public async Task RespondAsync_ShouldReturnBadRequest_WhenAssignmentNotWaitingForAcceptance()
		{
			//Arrange
			_assignmentMock.Object.State = TypeAssignmentState.Declined;
			_assignmentMock.Object.Asset = new Asset { State = TypeAssetState.Available, CreatedAt = DateTime.Now, IsDeleted = false };
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object};
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.RespondAsync(_requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentRespondNotWaitingForAcceptance);
			result.Data.Should().Be(_assignmentMock.Object.State.ToString());
		}

		[Test]
		public async Task RespondAsync_ShouldReturnBadRequest_WhenAssetIsNotAvailable()
		{
			//Arrange
			_assignmentMock.Object.State = TypeAssignmentState.WaitingForAcceptance;
			_assignmentMock.Object.Asset = new Asset { State = TypeAssetState.NotAvailable, CreatedAt = DateTime.Now, IsDeleted = false };
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.RespondAsync(_requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentRespondNotAvailable);
			result.Data.Should().Be(_assignmentMock.Object.Asset.State.ToString());
		}

		[Test]
		public async Task RespondAsync_ShouldReturnError_WhenRespondToAssignmentFail()
		{
			//Arrange
			_requestDtoMock.Object.IsAccept = false;
			_assignmentMock.Object.State = TypeAssignmentState.WaitingForAcceptance;
			_assignmentMock.Object.Asset = new Asset { State = TypeAssetState.Available, CreatedAt = DateTime.Now, IsDeleted = false };
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_assignmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).ReturnsAsync(0);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.RespondAsync(_requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentRespondFail);
			result.Data.Should().Be(_assignmentDtoMock.Object);
		}

		[Test]
		public async Task RespondAsync_ShouldReturnOk_WhenRespondToAssignmentSuccess()
		{
			//Arrange
			_assignmentMock.Object.State = TypeAssignmentState.WaitingForAcceptance;
			_assignmentMock.Object.Asset = new Asset { State = TypeAssetState.Available, CreatedAt = DateTime.Now, IsDeleted = false };
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_assignmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).ReturnsAsync(1);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.RespondAsync(_requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentRespondSuccess);
			result.Data.Should().Be(_assignmentDtoMock.Object);
		}

		
	}
}
