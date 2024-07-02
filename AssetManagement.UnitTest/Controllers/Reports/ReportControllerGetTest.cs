using AssetManagement.Api.Controllers;
using AssetManagement.Application.ApiResponses;
using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IServices.IReportServices;
using ClosedXML.Excel;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Data;
using System.Security.Claims;

namespace AssetManagement.UnitTest.Controllers.Reports
{
    [TestFixture]
    public class ReportsControllerTests
    {
        private readonly string _authorizeHeaderMock = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2NhdGlvbklkIjoiOGIwYTY0MjMtNjkxMy00ZDQ5LWJhMmYtOTQ2ZjZkOTMwOWYxIiwiaWQiOiI4YjBhNjQyMy02OTEzLTRkNDktYmEyZi05NDZmNmQ5MzA5ZjEiLCJyb2xlIjoiQWRtaW4iLCJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.z-W0nZeRlzWxzXa8XjXNNsr1h1jOI9jZsWN-q_5NrLU";
        private Mock<IReportService> _reportServiceMock;
        private ReportsController _controller;

        [SetUp]
        public void SetUp()
        {
            _reportServiceMock = new Mock<IReportService>();
            _controller = new ReportsController(_reportServiceMock.Object);
        }

        [Test]
        public async Task Get_ShouldReturnFileResult_WhenCalled()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            var locationId = Guid.NewGuid();
            var workbook = new XLWorkbook();
            var dataTable = new DataTable();
            var worksheet = workbook.AddWorksheet(dataTable, "Asset Management");
            var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            _reportServiceMock.Setup(x => x.ExportAssetManagementFile(It.IsAny<Guid>())).ReturnsAsync(workbook);

            // Act
            var result = await _controller.Get() as FileContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
            Assert.IsTrue(result.FileDownloadName.StartsWith("AssetManagement_Report_"));
        }
        [Test]
        public async Task Get_ShouldReturnOk_WhenReportDataFound()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.HttpContext.Request.Headers.Add("Authorization", _authorizeHeaderMock);
            var filter = new ReportFilter();
            var reportData = new PagedResponse<ResponseReportDto>
            {
                Data = new List<ResponseReportDto> {
                    new ResponseReportDto
                    {
                        CategoryName = "PC",
                        Total = 100,
                        Assigned = 10,
                        Available = 10,
                        NotAvailable = 10,
                        WaitingForRecycling = 10,
                        Recycled = 60
                    }
                },
                TotalCount = 1,
            };
            _reportServiceMock.Setup(x => x.GetReportData(It.IsAny<Guid>(), filter, 1, 10)).ReturnsAsync(reportData);

            // Act
            var result = await _controller.Get(filter,1,10);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        }
    }
}
