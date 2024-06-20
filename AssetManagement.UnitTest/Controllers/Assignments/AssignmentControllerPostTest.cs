using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.UnitTest.Controllers.Assignments
{
    [TestFixture]
    public class AssignmentControllerPostTest
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiZGRhZDhjNjgtYTI1YS00MTZhLTk4MDgtYzIyYWIyYWJkZTBmIiwic3ViIjoiMTIzNDU2Nzg5MCIsIm5hbWUiOiJKb2huIERvZSIsImlhdCI6MTUxNjIzOTAyMn0.zGkUGTif6i3P1iTc-rQ2hxuzyNELXN-9OGlXEcOj04c";
        private AssignmentsController _controller;
        private Mock<IAssignmentService> _assignmentServiceMock;
        private Mock<RequestAssignmentDto> _requestDtoMock;
        private Mock<Assignment> _assignmentMock;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assignmentServiceMock = new Mock<IAssignmentService>();
            _controller = new AssignmentsController(_assignmentServiceMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _requestDtoMock = new Mock<RequestAssignmentDto>();
            _assignmentMock = new Mock<Assignment>();
        }

        [Test]
        public async Task Post_ShouldReturnOk_WhenAssignmentIsCreatedSuccessfully()
        {
            // Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = AssignmentApiResponseMessageConstant.AssignmentCreateSuccess,
                Data = _assignmentMock.Object
            };
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            _assignmentServiceMock.Setup(s => s.CreateAsync(It.IsAny<RequestAssignmentDto>()))
                                  .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Post(_requestDtoMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(apiResponse);
        }

        [Test]
        public async Task Post_ShouldReturnUnauthorized_WhenCantDecodeUserId()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Post(_requestDtoMock.Object);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(UnauthorizedResult));
        }

        [Test]
        public async Task Post_ShouldReturnError_WhenAssignmentFailedToCreate()
        {
            // Arrange
            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = AssignmentApiResponseMessageConstant.AssignmentCreateFail,
                Data = _assignmentMock.Object
            };

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            _assignmentServiceMock.Setup(s => s.CreateAsync(It.IsAny<RequestAssignmentDto>()))
                                  .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.Post(_requestDtoMock.Object);

            // Assert
            var okResult = result as ObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            okResult.Value.Should().BeEquivalentTo(apiResponse);
        }

    }
}
