using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Domain.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.UnitTest.Controllers.Assignments
{
    [TestFixture]
    public class AssignmentControllerGetTest
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";
        private AssignmentsController _controller;
        private Mock<IAssignmentService> _assignmentServiceMock;
        private Mock<ResponseAssignmentDto> _assignmentDtoMock;
        private Mock<List<ResponseAssignmentDto>> _assignmentDtosMock;
        private Mock<AssignmentFilter> _filterMock;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assignmentServiceMock = new Mock<IAssignmentService>();
            _controller = new AssignmentsController(_assignmentServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _assignmentDtoMock = new Mock<ResponseAssignmentDto>();
            _filterMock = new Mock<AssignmentFilter>();
            _assignmentDtosMock = new Mock<List<ResponseAssignmentDto>>();
        }

        [Test]
        public async Task Get_ShouldReturnNotFound_WhenFindNoRecords()
        {
            //Arrange
            var index = 1;
            var size = 10;
            var response = new PagedResponse<ResponseAssignmentDto>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AssignmentApiResponseMessageConstant.AssignmentGetNotFound
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            _assignmentServiceMock.Setup(a => a.GetAllAsync(It.IsAny<AssignmentFilter>(),
                                                            It.IsAny<Guid>(),
                                                            It.IsAny<UserType>(),
                                                            It.IsAny<Guid>(),
                                                            It.IsAny<int?>(),
                                                            It.IsAny<int?>())).ReturnsAsync(response);

            //Act
            var result = await _controller.Get(_filterMock.Object, index, size);

            //Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task Get_ShouldReturnUnauthorize_WhenCantDecodeToken()
        {
            //Arrange
            var index = 1;
            var size = 10;
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //Act
            var result = await _controller.Get(_filterMock.Object, index, size);

            //Assert
            var unauthorizedResult = result as UnauthorizedResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenRecordsFound()
        {
            //Arrange
            var index = 1;
            var size = 10;
            var response = new PagedResponse<ResponseAssignmentDto>
            {
                Data = _assignmentDtosMock.Object,
                TotalCount = 2,
                Message = AssignmentApiResponseMessageConstant.AssignmentGetSuccess
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            _assignmentServiceMock.Setup(a => a.GetAllAsync(It.IsAny<AssignmentFilter>(),
                                                            It.IsAny<Guid>(),
                                                            It.IsAny<UserType>(),
                                                            It.IsAny<Guid>(),
                                                            It.IsAny<int?>(),
                                                            It.IsAny<int?>())).ReturnsAsync(response);

            //Act
            var result = await _controller.Get(_filterMock.Object, index, size);

            //Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenRecordFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            var response = new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = AssignmentApiResponseMessageConstant.AssignmentGetSuccess,
                Data = _assignmentDtoMock.Object
            };

            _assignmentServiceMock.Setup(a => a.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(response);

            //Act
            var result = await _controller.Get(id);

            //Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Test]
        public async Task Get_ShouldReturnNotFound_WhenNoRecordFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            var response = new ApiResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
                Data = id
            };

            _assignmentServiceMock.Setup(a => a.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(response);

            //Act
            var result = await _controller.Get(id);

            //Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            notFoundResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
