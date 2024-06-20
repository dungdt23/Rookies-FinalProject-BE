using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
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
	public class AssignmentServiceUpdateAsync
	{
		private Mock<IAssignmentRepository> _assignmentRepositoryMock;
		private Mock<IAssetRepository> _assetRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private AssignmentService _assignmentService;
		private Mock<ResponseAssignmentDto> _assignmentDtoMock;
		private Mock<Assignment> _assignmentMock;
		private Mock<RequestAssignmentDto> _requestDtoMock;

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
			_requestDtoMock = new Mock<RequestAssignmentDto>();
		}

		[Test]
		public async Task UpdateAsync_ShouldReturnNotFound_WhenAssignmentNotFoundWithId()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment>();
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.UpdateAsync(id, _requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentNotFound);
			result.Data.Should().Be(id);
		}

		[Test]
		public async Task UpdateAsync_ShouldReturnError_WhenAssignmentUpdateFail()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_mapperMock.Setup(m => m.Map(It.IsAny<RequestAssignmentDto>(), It.IsAny<Assignment>()))
				.Callback<RequestAssignmentDto, Assignment>((src, dest) =>
				{
					dest.AssetId = src.AssetId;
					dest.AssignerId = src.AssignerId;
					dest.AssigneeId = src.AssigneeId;
					dest.AssignedDate = src.AssignedDate;
					dest.Note = src.Note;
				});
			_assignmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).ReturnsAsync(0);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.UpdateAsync(id, _requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentUpdateFail);
			result.Data.Should().BeEquivalentTo(_assignmentDtoMock.Object);
		}

		[Test]
		public async Task UpdateAsync_ShouldReturnOk_WhenAssignmentUpdateSuccess()
		{
			//Arrange
			var id = Guid.NewGuid();
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object };
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetByCondition(It.IsAny<Expression<Func<Assignment, bool>>>())).Returns(assignmentsQueryMock);
			_mapperMock.Setup(m => m.Map(It.IsAny<RequestAssignmentDto>(), It.IsAny<Assignment>()))
				.Callback<RequestAssignmentDto, Assignment>((src, dest) =>
				{
					dest.AssetId = src.AssetId;
					dest.AssignerId = src.AssignerId;
					dest.AssigneeId = src.AssigneeId;
					dest.AssignedDate = src.AssignedDate;
					dest.Note = src.Note;
				});
			_assignmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Assignment>())).ReturnsAsync(1);
			_mapperMock.Setup(m => m.Map<ResponseAssignmentDto>(It.IsAny<Assignment>())).Returns(_assignmentDtoMock.Object);

			//Act
			var result = await _assignmentService.UpdateAsync(id, _requestDtoMock.Object);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(ApiResponse));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentUpdateSuccess);
			result.Data.Should().BeEquivalentTo(_assignmentDtoMock.Object);
		}

	}
}
