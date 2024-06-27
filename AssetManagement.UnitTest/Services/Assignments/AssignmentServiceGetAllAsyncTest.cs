using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
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
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.UnitTest.Services.Assignments
{
	[TestFixture]
	public class AssignmentServiceGetAllAsyncTest
	{
		private Mock<IAssignmentRepository> _assignmentRepositoryMock;
		private Mock<IAssetRepository> _assetRepositoryMock;
		private Mock<IMapper> _mapperMock;
		private AssignmentService _assignmentService;
		private Mock<ResponseAssignmentDto> _assignmentDtoMock;
		private Mock<Assignment> _assignmentMock;
		private Mock<AssignmentFilter> _filterMock;

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
			_filterMock = new Mock<AssignmentFilter>();
		}

		[Test]
		public async Task GetAllAsync_ReturnOk_WhenFoundNoAssignmentRecord()
		{
			//Arrange
			var index = 1;
			var size = 10;
			var assignmentsMock = new List<Assignment>();
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();

			_assignmentRepositoryMock.Setup(r => r.GetAll(It.IsAny<bool>(),
															It.IsAny<Func<Assignment, object>>(), 
															It.IsAny<AssignmentFilter>(), 
															It.IsAny<Guid>(), 
															It.IsAny<UserType>(),
															It.IsAny<Guid>())).Returns(assignmentsQueryMock);

			//Act
			var result = await _assignmentService.GetAllAsync(It.IsAny<bool>(),_filterMock.Object,It.IsAny<Guid>(),It.IsAny<UserType>(),It.IsAny<Guid>(), index, size);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(PagedResponse<ResponseAssignmentDto>));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentGetNotFound);
			result.TotalCount.Should().Be(0);
		}

		[Test]
		public async Task GetAllAsync_ReturnOk_WhenFoundAssignmentRecords()
		{
			//Arrange
			var index = 1;
			var size = 10;
			var assignmentsMock = new List<Assignment> { _assignmentMock.Object};
			var assignmentsQueryMock = assignmentsMock.AsQueryable().BuildMock();
			var assignmentDtosMock = new List<ResponseAssignmentDto> { _assignmentDtoMock.Object};

			_assignmentRepositoryMock.Setup(r => r.GetAll(It.IsAny<bool>(),
															It.IsAny<Func<Assignment, object>>(),
															It.IsAny<AssignmentFilter>(),
															It.IsAny<Guid>(),
															It.IsAny<UserType>(),
															It.IsAny<Guid>())).Returns(assignmentsQueryMock); _mapperMock.Setup(m => m.Map<List<ResponseAssignmentDto>>(It.IsAny<List<Assignment>>())).Returns(assignmentDtosMock);
			//Act
			var result = await _assignmentService.GetAllAsync(It.IsAny<bool>(), _filterMock.Object, It.IsAny<Guid>(), It.IsAny<UserType>(), It.IsAny<Guid>(), index, size);

			//Assert
			result.Should().NotBeNull();
			result.Should().BeOfType(typeof(PagedResponse<ResponseAssignmentDto>));
			result.StatusCode.Should().Be(StatusCodes.Status200OK);
			result.Message.Should().Be(AssignmentApiResponseMessageConstant.AssignmentGetSuccess);
			result.Data.Should().BeEquivalentTo(assignmentDtosMock);
			result.TotalCount.Should().Be(assignmentDtosMock.Count);
		}
	}
}
