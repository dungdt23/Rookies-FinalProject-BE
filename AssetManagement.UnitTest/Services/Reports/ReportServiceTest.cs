using AssetManagement.Application.Dtos.ResponseDtos;
using AssetManagement.Application.Filters;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.Services.ReportServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AutoMapper;
using MockQueryable.Moq;
using Moq;
using System.Data;
using System.Linq.Expressions;

namespace AssetManagement.UnitTest.Services.Reports
{
    [TestFixture]
    public class ReportServiceTests
    {
        private Mock<IGenericRepository<Category>> _categoryRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private ReportService _reportService;

        [SetUp]
        public void SetUp()
        {
            _categoryRepositoryMock = new Mock<IGenericRepository<Category>>();
            _mapperMock = new Mock<IMapper>();
            _reportService = new ReportService(_categoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task ExportAssetManagementFile_ShouldReturnWorkbook_WhenCalled()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Category");
            dataTable.Columns.Add("Total");
            dataTable.Columns.Add("Assigned");
            dataTable.Columns.Add("Available");
            dataTable.Columns.Add("Not available");
            dataTable.Columns.Add("Waiting for recycling");
            dataTable.Columns.Add("Recycled");

            var dataRow = dataTable.NewRow();
            dataRow["Category"] = "Test Category";
            dataRow["Total"] = 10;
            dataRow["Assigned"] = 5;
            dataRow["Available"] = 3;
            dataRow["Not available"] = 1;
            dataRow["Waiting for recycling"] = 1;
            dataRow["Recycled"] = 0;
            dataTable.Rows.Add(dataRow);

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(new List<Category>
                {
                    new Category { CategoryName = "Test Category", Assets = new List<Asset> { new Asset { LocationId = locationId } } }
                }.AsQueryable().BuildMock());

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(new List<ResponseReportDto> { new ResponseReportDto { CategoryName = "Test Category", Total = 10, Assigned = 5, Available = 3, NotAvailable = 1, WaitingForRecycling = 1, Recycled = 0 } });

            // Act
            var result = await _reportService.ExportAssetManagementFile(locationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Asset Management", result.Worksheets.First().Name);
        }
        [Test]
        public async Task GetReportData_ShouldReturData_WhenSortByName()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var dataTable = new DataTable();
            dataTable.Columns.Add("Category");
            dataTable.Columns.Add("Total");
            dataTable.Columns.Add("Assigned");
            dataTable.Columns.Add("Available");
            dataTable.Columns.Add("Not available");
            dataTable.Columns.Add("Waiting for recycling");
            dataTable.Columns.Add("Recycled");

            var dataRow = dataTable.NewRow();
            dataRow["Category"] = "Test Category";
            dataRow["Total"] = 10;
            dataRow["Assigned"] = 5;
            dataRow["Available"] = 3;
            dataRow["Not available"] = 1;
            dataRow["Waiting for recycling"] = 1;
            dataRow["Recycled"] = 0;
            dataTable.Rows.Add(dataRow);

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(new List<Category>
                {
                    new Category { CategoryName = "Test Category", Assets = new List<Asset> { new Asset { LocationId = locationId } } }
                }.AsQueryable().BuildMock());

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(new List<ResponseReportDto> { new ResponseReportDto { CategoryName = "Test Category", Total = 10, Assigned = 5, Available = 3, NotAvailable = 1, WaitingForRecycling = 1, Recycled = 0 } });

            // Act
            var result = await _reportService.ExportAssetManagementFile(locationId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Asset Management", result.Worksheets.First().Name);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenDataExists()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, null, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByTotal()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.Total}, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByAssigned()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.Assigned }, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByAvailable()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.Available }, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByNotAvailable()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.NotAvailable }, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByWaitingForRecycling()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.WaitingForRecycling }, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnPagedResponse_WhenSortByNotRecycled()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "Keyboard",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false },
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "Keyboard", Total = 2, Assigned = 1, Available = 1 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            // Act
            var result = await _reportService.GetReportData(locationId, new ReportFilter { ascending = true, sortby = TypeReportSort.Recycled }, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Keyboard", result.Data.First().CategoryName);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetSuccess, result.Message);
        }
        [Test]
        public async Task GetReportData_ShouldReturnEmptyResponse_WhenNoDataExists()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>();

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            // Act
            var result = await _reportService.GetReportData(locationId, null, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(ReportApiResponseMessageConstant.ReportGetNotFound, result.Message);
        }

        [Test]
        public async Task GetReportData_ShouldSortDataBasedOnFilter()
        {
            // Arrange
            var locationId = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category
                {
                    CategoryName = "CategoryA",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Assigned, IsDeleted = false }
                    }
                },
                new Category
                {
                    CategoryName = "CategoryB",
                    Assets = new List<Asset>
                    {
                        new Asset { LocationId = locationId, State = TypeAssetState.Available, IsDeleted = false }
                    }
                }
            }.AsQueryable().BuildMock();

            var responseReportDtos = new List<ResponseReportDto>
            {
                new ResponseReportDto { CategoryName = "CategoryB", Total = 1, Assigned = 0, Available = 1 },
                new ResponseReportDto { CategoryName = "CategoryA", Total = 1, Assigned = 1, Available = 0 }
            };

            _categoryRepositoryMock.Setup(x => x.GetByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                .Returns(categories);

            _mapperMock.Setup(x => x.Map<IEnumerable<ResponseReportDto>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(responseReportDtos);

            var filter = new ReportFilter { sortby = TypeReportSort.CategoryName, ascending = false };

            // Act
            var result = await _reportService.GetReportData(locationId, filter, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Data.Count());
            Assert.AreEqual("CategoryB", result.Data.First().CategoryName);
            Assert.AreEqual("CategoryA", result.Data.Last().CategoryName);
        }
    }
}
