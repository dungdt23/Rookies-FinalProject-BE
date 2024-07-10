using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Azure.Core;
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
	public class AssignmentControllerRespondTest
	{
		private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";
		private AssignmentsController _controller;
		private Mock<IAssignmentService> _assignmentServiceMock;
		private Mock<RequestAssignmentRespondDto> _requestDtoMock;
		private Mock<ResponseAssignmentDto> _assignmentDtoMock;
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_assignmentServiceMock = new Mock<IAssignmentService>();
			_controller = new AssignmentsController(_assignmentServiceMock.Object);
		}

		[SetUp]
		public void SetUp()
		{
			_requestDtoMock = new Mock<RequestAssignmentRespondDto>();
			_assignmentDtoMock = new Mock<ResponseAssignmentDto>();
		}

		[Test]
		public async Task Respond_ShouldReturnNotFound_WhenNoAssignmentFoundWithId()
		{
			//Arrange
			var response = new ApiResponse
			{
				StatusCode = StatusCodes.Status404NotFound,
				Message = AssignmentApiResponseMessageConstant.AssignmentNotFound,
				Data = $"Request Id : {_requestDtoMock.Object.AssignmentId}"
			};
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
			_controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

			_assignmentServiceMock.Setup(s => s.RespondAsync(_requestDtoMock.Object,It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(response);

			//Act
			var result = await _controller.Respond(_requestDtoMock.Object);

			//Assert
			var notFoundResult = result as NotFoundObjectResult;
			notFoundResult.Should().NotBeNull();
			notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);
			notFoundResult.Value.Should().BeEquivalentTo(response);
		}

		[Test]
		public async Task Respond_ShouldReturnError_WhenRespondAssignmentFail()
		{
			//Arrange
			var response = new ApiResponse
			{
				StatusCode = StatusCodes.Status500InternalServerError,
				Message = AssignmentApiResponseMessageConstant.AssignmentRespondFail,
				Data = _assignmentDtoMock.Object
			};
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
			_controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

			_assignmentServiceMock.Setup(s => s.RespondAsync(_requestDtoMock.Object, It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(response);

			//Act
			var result = await _controller.Respond(_requestDtoMock.Object);

			//Assert
			var errorResult = result as ObjectResult;
			errorResult.Should().NotBeNull();
			errorResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
			errorResult.Value.Should().BeEquivalentTo(response);
		}

		[Test]
		public async Task Respond_ShouldReturnBadRequest_WhenRespondAssignmentNotWaitingForAcceptance()
		{
			//Arrange
			var response = new ApiResponse
			{
				StatusCode = StatusCodes.Status400BadRequest,
				Message = AssignmentApiResponseMessageConstant.AssignmentRespondNotWaitingForAcceptance,
				Data = TypeAssignmentState.Declined
			};
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
			_controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

			_assignmentServiceMock.Setup(s => s.RespondAsync(_requestDtoMock.Object, It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(response);

			//Act
			var result = await _controller.Respond(_requestDtoMock.Object);

			//Assert
			var badRequestResult = result as BadRequestObjectResult;
			badRequestResult.Should().NotBeNull();
			badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
			badRequestResult.Value.Should().BeEquivalentTo(response);
		}

		[Test]
		public async Task Respond_ShouldReturnOk_WhenRespondAssignmentSuccess()
		{
			//Arrange
			var response = new ApiResponse
			{
				StatusCode = StatusCodes.Status200OK,
				Message = AssignmentApiResponseMessageConstant.AssignmentRespondSuccess,
				Data = _assignmentDtoMock.Object
			};
			_controller.ControllerContext = new ControllerContext();
			_controller.ControllerContext.HttpContext = new DefaultHttpContext();
			_controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);

			_assignmentServiceMock.Setup(s => s.RespondAsync(_requestDtoMock.Object, It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(response);

			//Act
			var result = await _controller.Respond(_requestDtoMock.Object);

			//Assert
			var okResult = result as OkObjectResult;
			okResult.Should().NotBeNull();
			okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
			okResult.Value.Should().BeEquivalentTo(response);
		}
	}
}
