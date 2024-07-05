using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Constants;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Bogus;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AssetManagement.Api.Extensions;

public static class ApplicationExtension
{
	private static readonly int UserToGenerate = 200;
	private static readonly int AssetToGenerate = 300;
	private static readonly int CategoryToGenerate = 30;
	private static readonly int MaxAssignmentHistory = 10;

	public static async Task SeedDataAsync(this IApplicationBuilder app)
	{
		await app.SeedAllExceptionReturnRequestAsync();
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetService<AssetManagementDBContext>();

			if (!dbContext!.ReturnRequests.Any())
			{
				await dbContext!.SeedReturnRequestsAsync();

				await dbContext!.SeedAssignmentHistoriesAsync("Hà Nội");
				await dbContext!.SeedAssignmentHistoriesAsync("Đà Nẵng");
				await dbContext!.SeedAssignmentHistoriesAsync("Hồ Chí Minh");
			}


			var jwtInvalidationService = scope.ServiceProvider.GetService<IJwtInvalidationService>();
			await jwtInvalidationService!.UpdateGlobalInvalidationTimeStampAsync(null);
		}
	}

	public static async Task<IApplicationBuilder> SeedAllExceptionReturnRequestAsync(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetService<AssetManagementDBContext>();
			var userService = scope.ServiceProvider.GetService<IUserService>();
			dbContext.Database.EnsureCreated();

			using var transaction = dbContext.Database.BeginTransaction();
			try
			{


				if (!dbContext.Types.Any())
				{
					dbContext.Add(new Domain.Entities.Type { TypeName = "Staff", Description = "Staff of system", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Domain.Entities.Type { TypeName = "Admin", Description = "Administration of system", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.SaveChanges();
				}

				if (!dbContext.Locations.Any())
				{
					dbContext.Add(new Location { LocationName = "Hà Nội", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Location { LocationName = "Đà Nẵng", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Location { LocationName = "Hồ Chí Minh", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.SaveChanges();
				}

				if (!dbContext.Users.Any())
				{
					List<string> firstNames = new List<string> { "An", "Bảo", "Cường", "Minh Duy", "Minh Ánh", "Lan", "Mai", "Ngọc" };
					List<string> lastNames = new List<string> { "Nguyễn", "Trần", "Lê", "Phan", "Hoàng", "Huỳnh", "Phan", "Vũ" };
					List<Domain.Entities.Type> types = dbContext.Types.ToList();
					List<Guid> locationIds = dbContext.Locations.Select(t => t.Id).ToList();
					Random random = new Random();

					DateTime startDate = new DateTime(1990, 1, 1);
					int dateOfBirthRange = (DateTime.Today.AddYears(-18) - startDate).Days;

					//pre-user admin for easier login
					var adminUser = new RequestUserCreateDto
					{
						FirstName = "Minh Ánh",
						LastName = "Nguyễn",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2003, 9, 19),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Male
					};
					await userService.CreateAsync(adminUser);
					var adminUser1 = new RequestUserCreateDto
					{
						FirstName = "Tien Dung",
						LastName = "Dao",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Đà Nẵng")).Id,
						DateOfBirth = new DateTime(2002, 7, 20),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Male
					};
					await userService.CreateAsync(adminUser1);
					var adminUser2 = new RequestUserCreateDto
					{
						FirstName = "Son",
						LastName = "Nguyen Viet Bao",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hồ Chí Minh")).Id,
						DateOfBirth = new DateTime(2002, 4, 9),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Male
					};
					await userService.CreateAsync(adminUser2);

					//account for mentor
					var adminUser3 = new RequestUserCreateDto
					{
						FirstName = "Hong",
						LastName = "Pham",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2000, 1, 1),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(adminUser3);
					var adminUser4 = new RequestUserCreateDto
					{
						FirstName = "Hung",
						LastName = "Bui",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2000, 1, 1),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Male
					};
					await userService.CreateAsync(adminUser4);
					var adminUser5 = new RequestUserCreateDto
					{
						FirstName = "Tuan",
						LastName = "Tran",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2000, 1, 1),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Male
					};
					await userService.CreateAsync(adminUser5);
					var adminUser6 = new RequestUserCreateDto
					{
						FirstName = "Thuy",
						LastName = "Nghiem",
						Type = "Admin",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2000, 1, 1),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(adminUser6);





					//pre-user sstaff
					var staffUser = new RequestUserCreateDto
					{
						FirstName = "Trang",
						LastName = "Mac",
						Type = "Staff",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(1999, 01, 01),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(staffUser);

					var staffUser2 = new RequestUserCreateDto
					{
						FirstName = "Huong",
						LastName = "Ho",
						Type = "Staff",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Đà Nẵng")).Id,
						DateOfBirth = new DateTime(2003, 01, 01),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(staffUser2);


					var staffUser3 = new RequestUserCreateDto
					{
						FirstName = "Quynh",
						LastName = "Pham",
						Type = "Staff",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hồ Chí Minh")).Id,
						DateOfBirth = new DateTime(2003, 01, 01),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(staffUser3);

					var staffUser4 = new RequestUserCreateDto
					{
						FirstName = "Phung",
						LastName = "Linh",
						Type = "Staff",
						LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
						DateOfBirth = new DateTime(2003, 01, 01),
						JoinedDate = new DateTime(2024, 4, 22),
						Gender = TypeGender.Female
					};
					await userService.CreateAsync(staffUser4);

					for (int i = 0; i < UserToGenerate; i++)
					{
						string firstName = firstNames[random.Next(firstNames.Count)];
						string lastName = lastNames[random.Next(lastNames.Count)];
						Domain.Entities.Type type = types[random.Next(types.Count)];
						Guid locationId = locationIds[random.Next(locationIds.Count)];
						DateTime dateOfBirth = startDate.AddDays(random.Next(dateOfBirthRange));
						DateTime joinedDate = dateOfBirth.AddYears(18).AddDays(random.Next(1, 60));
						if (joinedDate.DayOfWeek == DayOfWeek.Saturday || joinedDate.DayOfWeek == DayOfWeek.Sunday)
						{
							joinedDate.AddDays(2);
						}


						RequestUserCreateDto form = new RequestUserCreateDto
						{
							FirstName = firstName,
							LastName = lastName,
							Type = type.TypeName,
							LocationId = locationId,
							DateOfBirth = dateOfBirth,
							JoinedDate = joinedDate,
							Gender = (TypeGender)random.Next(0, 2)
						};

						await userService.CreateAsync(form);

					}

				}



				if (!dbContext.Categories.Any())
				{
					const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
					var random = new Random();
					var prefixs = new List<string>();
					var categoriesFaker = new Faker<Category>()
						.RuleFor(a => a.CategoryName, f => f.Commerce.ProductName())
						.RuleFor(a => a.Prefix, (f, a) =>
						{
							var prefix = $"{a.CategoryName.Substring(0, 4).ToUpper()}";
							while (prefixs.Contains(prefix))
							{
								prefix = "";
								for(int i = 0; i<4; i++)
								{
									prefix += chars[random.Next(chars.Length)];
								}
							}
							prefixs.Add(prefix);
							return prefix;
						})
						.Generate(CategoryToGenerate);

					dbContext.AddRange(categoriesFaker);
					dbContext.Add(new Category { CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Category { CategoryName = "Monitor", Prefix = "MO", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Category { CategoryName = "PC", Prefix = "PC", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.Add(new Category { CategoryName = "Keyboard", Prefix = "KE", CreatedAt = DateTime.UtcNow, IsDeleted = false });
					dbContext.SaveChanges();
				}
				var assetCodes = new Dictionary<Guid, int>();
				if (!dbContext.Assets.Any())
				{
					var assets = GenerateAsset(assetCodes, 200, dbContext);
				}

				if (!dbContext.Assignments.Any())
				{
					var random = new Random();
					var assigner = dbContext.Users.Where(a => a.StaffCode.Equals("SD0001")).AsNoTracking().FirstOrDefault();
					var typeStaff = dbContext.Types.Where(a => a.TypeName == "Staff").AsNoTracking().FirstOrDefault();
					var assignee = dbContext.Users.Where(a => a.TypeId == typeStaff.Id && a.LocationId == assigner.LocationId).AsNoTracking().ToList();
					var assets = GenerateAsset(assetCodes, 100, dbContext, "Hà Nội");
					dbContext.ChangeTracker.Clear();

					for (int i = 0; i < assets.Count; i++)
					{
						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assets[i].Id)
										.RuleFor(a => a.AssignerId, f => assigner.Id)
										.RuleFor(a => a.AssigneeId, f => f.PickRandom(assignee).Id)
										.RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);
					}
					dbContext.SaveChanges();

					//generate assignment for QC account in HN
					var assignerHN = dbContext.Users
						.Include(a => a.Location)
						.Where(a => a.StaffCode.Equals("SD0001")).AsNoTracking().FirstOrDefault();
					var assigneesHN = dbContext.Users.Where(a => a.UserName.Equals("trangm") || a.UserName.Equals("linhp")).AsNoTracking().ToList();
					var assetsHN = GenerateAsset(assetCodes, 150, dbContext, "Hà Nội");
					dbContext.ChangeTracker.Clear();
					for (int i = 0; i < 100; i++)
					{

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsHN[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerHN.Id)
										.RuleFor(a => a.AssigneeId, f => f.PickRandom(assigneesHN).Id)
										.RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);

					}
					//generate assignment with accepted to mock for return request
					for (int i = 100; i < 150; i++)
					{

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsHN[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerHN.Id)
										.RuleFor(a => a.AssigneeId, f => f.PickRandom(assigneesHN).Id)
										.RuleFor(a => a.State, f => TypeAssignmentState.Accepted)
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);
					}
					dbContext.SaveChanges();
					//generate assignment for QC account in DaNang
					var assignerDN = dbContext.Users
						.Include(a => a.Location)
						.Where(a => a.UserName.Equals("dungdt")).AsNoTracking().FirstOrDefault();
					var assigneeDN = dbContext.Users.Where(a => a.UserName.Equals("huongh")).AsNoTracking().FirstOrDefault();
					var assetsDN = GenerateAsset(assetCodes, 150, dbContext, "Đà Nẵng");
					dbContext.ChangeTracker.Clear();
					for (int i = 0; i < 100; i++)
					{

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsDN[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerDN.Id)
										.RuleFor(a => a.AssigneeId, f => assigneeDN.Id)
										.RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);

					}
					//generate assignment with accepted to mock for return request
					for (int i = 100; i < 150; i++)
					{

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsDN[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerDN.Id)
										.RuleFor(a => a.AssigneeId, f => assigneeDN.Id)
										.RuleFor(a => a.State, f => TypeAssignmentState.Accepted)
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);
					}
					dbContext.SaveChanges();
					//generate assignment for QC account in HCM
					var assignerHCM = dbContext.Users
						.Include(a => a.Location)
						.Where(a => a.UserName.Equals("sonnvb")).AsNoTracking().FirstOrDefault();
					var assigneeHCM = dbContext.Users.Where(a => a.UserName.Equals("quynhp")).AsNoTracking().FirstOrDefault();
					var assetsHCM = GenerateAsset(assetCodes, 150, dbContext, "Hồ Chí Minh");
					dbContext.ChangeTracker.Clear();
					for (int i = 0; i < 100; i++)
					{
						int numberOfAssignments = new Random().Next(3, 5);

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsHCM[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerHCM.Id)
										.RuleFor(a => a.AssigneeId, f => assigneeHCM.Id)
										.RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);
					}
					//generate assignment with accepted to mock for return request
					for (int i = 100; i < 150; i++)
					{

						var assignmentFaker = new Faker<Assignment>()
										.RuleFor(a => a.AssetId, f => assetsHCM[i].Id)
										.RuleFor(a => a.AssignerId, f => assignerHCM.Id)
										.RuleFor(a => a.AssigneeId, f => assigneeHCM.Id)
										.RuleFor(a => a.State, f => TypeAssignmentState.Accepted)
										.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
										.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
						var assignment = assignmentFaker.Generate(1).FirstOrDefault();
						dbContext.Assignments.Add(assignment);
					}
					dbContext.SaveChanges();

					dbContext.ChangeTracker.Clear();

					var assignmentsAssetId = await dbContext.Assignments
						.Where(a => a.State == TypeAssignmentState.WaitingForAcceptance || a.State == TypeAssignmentState.Accepted)
						.Select(a => a.AssetId)
						.ToListAsync();
					var assetToUpdates = await dbContext.Assets.Where(a => assignmentsAssetId.Contains(a.Id)).ToListAsync();
					foreach (var asset in assetToUpdates)
					{
						asset.State = TypeAssetState.Assigned;
					}
					dbContext.UpdateRange(assetToUpdates);
					dbContext.SaveChanges();

					var declinedAssettIds = await dbContext.Assignments
						.Where(a => a.State == TypeAssignmentState.Declined)
						.Select(a => a.AssetId)
						.ToListAsync();
					var declinedAssetToUpdates = await dbContext.Assets.Where(a => declinedAssettIds.Contains(a.Id)).ToListAsync();
					foreach (var asset in declinedAssetToUpdates)
					{
						asset.State = TypeAssetState.Available;
					}
					dbContext.UpdateRange(declinedAssetToUpdates);
					dbContext.SaveChanges();



				}
				transaction.Commit();
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				throw ex;
			}
			return app;
		}
	}

	public static List<Asset> GenerateAsset(Dictionary<Guid, int> assetCodes, int amount, AssetManagementDBContext dbContext, string? locationName = null)
	{
		Random random = new Random();
		var categories = dbContext.Categories.ToList();
		List<Location> locations;
		if (locationName.IsNullOrEmpty())
		{
			locations = dbContext.Locations.ToList();

		}
		else
		{
			locations = dbContext.Locations.Where(x => x.LocationName == locationName).ToList();
		}
		var assetFaker = new Faker<Asset>()
			.RuleFor(a => a.CategoryId, f => f.PickRandom(categories).Id)
			.RuleFor(a => a.LocationId, f => f.PickRandom(locations).Id)
			.RuleFor(a => a.Specification, f => f.Commerce.ProductDescription())
			.RuleFor(a => a.InstalledDate, f => f.Date.Past(2))
			.RuleFor(a => a.State, f => (TypeAssetState)f.PickRandom(new int[4] { 1, 2, 4, 5 }))
			.RuleFor(a => a.AssetName, f => f.Commerce.ProductName())
			.RuleFor(a => a.AssetCode, (f, a) =>
			{
				if (!assetCodes.ContainsKey(a.CategoryId))
				{
					assetCodes[a.CategoryId] = 1;
				}
				var categoryPrefix = categories.First(c => c.Id == a.CategoryId).Prefix;
				return $"{categoryPrefix}{assetCodes[a.CategoryId]++.ToString("D6")}";
			})
			.RuleFor(a => a.CreatedAt, f => DateTime.UtcNow)
			.RuleFor(a => a.IsDeleted, f => false)
			.Generate(amount);

		dbContext.AddRange(assetFaker);
		dbContext.SaveChanges();
		return assetFaker;
	}

	public static async Task SeedAssignmentHistoriesAsync(this AssetManagementDBContext dBContext, string locationName)
	{
		var assets = dBContext.Assets.Where(a => a.IsDeleted == false && a.Location.LocationName == locationName)
			.Include(a => a.Location)
			.AsNoTracking()
			.ToList();
		var assigner = dBContext.Users.Where(a => a.IsDeleted == false && a.Type.TypeName == "Admin" && a.Location.LocationName == locationName)
			.Include(a => a.Type)
			.Include(a => a.Location)
			.AsNoTracking()
			 .ToList();
		var assignee = dBContext.Users.Where(a => a.IsDeleted == false && a.Type.TypeName == "Staff" && a.Location.LocationName == locationName)
			.Include(a => a.Type)
			.Include(a => a.Location)
			.AsNoTracking()
			.ToList();

		var random = new Random();
		foreach (var asset in assets)
		{
			var assignmentFaker = new Faker<Assignment>()
									   .RuleFor(a => a.AssetId, f => asset.Id)
									   .RuleFor(a => a.AssignerId, f => f.PickRandom(assigner).Id)
									   .RuleFor(a => a.AssigneeId, f => f.PickRandom(assignee).Id)
									   .RuleFor(a => a.State, f => TypeAssignmentState.Accepted)
									   .RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
									   .RuleFor(a => a.Note, f => f.Lorem.Sentence(5))
									   .RuleFor(a => a.IsDeleted, f => true)
									   .RuleFor(a => a.DeletedAt, f => f.Date.Past(1))
									   .Generate(random.Next(0, MaxAssignmentHistory + 1));

			foreach (var assignment in assignmentFaker)
			{
				var returnRequestFaker = new Faker<ReturnRequest>()
					.RuleFor(r => r.AssignmentId, f => assignment.Id)
					.RuleFor(r => r.RequestorId, f => f.PickRandom(assigner).Id)
					.RuleFor(r => r.RequestedDate, f => assignment.AssignedDate.AddDays(random.Next(0, 100)))
					.RuleFor(r => r.LocationId, f => asset.LocationId)
					.RuleFor(r => r.CreatedAt, (f, a) => a.RequestedDate)
					.RuleFor(r => r.State, f => TypeRequestState.Completed)
					.RuleFor(r => r.ResponderId, f => f.PickRandom(assigner).Id)
					.RuleFor(r => r.ReturnedDate, (f, a) => a.RequestedDate.AddDays(random.Next(0, 50)))
					.Generate(1);
				await dBContext.AddRangeAsync(returnRequestFaker);

			}
			await dBContext.AddRangeAsync(assignmentFaker);
			//		RequestorId = new Random().Next(2) == 0 ? assignment.AssigneeId : admin.Id,
			//		AssignmentId = assignment.Id,
			//		RequestedDate = past,
			//		State = randomState,
			//		LocationId = location.Id,
			//		CreatedAt = past,
		}
		await dBContext.SaveChangesAsync();
	}

	public static async Task SeedReturnRequestsAsync(this AssetManagementDBContext dbContext)
	{


		var locations = await dbContext.Locations.ToListAsync();
		var assignments = new List<Assignment>();
		var returnRequests = new List<ReturnRequest>();
		foreach (var location in locations)
		{
			var acceptedAssignments = await dbContext.Assignments
			   .Where(a => a.State == TypeAssignmentState.Accepted && a.Asset.LocationId == location.Id)
			   .Include(a => a.Asset)
			   .ToListAsync();

			var random = new Random();
			var requestStates = Enum.GetValues(typeof(TypeRequestState)).Cast<TypeRequestState>().ToArray();
			var admin = await dbContext.Users
				.Where(u => u.Type.TypeName == TypeNameConstants.TypeAdmin && u.Location.Id == location.Id)
				.FirstOrDefaultAsync();

			foreach (var assignment in acceptedAssignments)
			{
				var randomState = requestStates[random.Next(requestStates.Length)];

				var past = new Faker().Date.Past(1);

				var newReturnRequest = new ReturnRequest
				{
					Id = Guid.NewGuid(),
					RequestorId = new Random().Next(2) == 0 ? assignment.AssigneeId : admin.Id,
					AssignmentId = assignment.Id,
					RequestedDate = past,
					State = randomState,
					LocationId = location.Id,
					CreatedAt = past,
				};

				assignment.ActiveReturnRequestId = newReturnRequest.Id;

				// Depending on the state, set appropriate properties
				if (randomState == TypeRequestState.Completed)
				{
					newReturnRequest.ReturnedDate = DateTime.UtcNow;
					newReturnRequest.ResponderId = admin.Id;
					newReturnRequest.UpdatedAt = DateTime.UtcNow;

					assignment.IsDeleted = true;
					assignment.DeletedAt = DateTime.UtcNow;
					assignment.UpdatedAt = DateTime.UtcNow;

					assignment.Asset.State = TypeAssetState.Available;
					assignment.Asset.UpdatedAt = DateTime.UtcNow;

				}
				else if (randomState == TypeRequestState.Rejected)
				{
					newReturnRequest.ReturnedDate = DateTime.UtcNow;
					newReturnRequest.ResponderId = admin.Id;
					newReturnRequest.UpdatedAt = DateTime.UtcNow;

					assignment.ActiveReturnRequestId = null;
					assignment.UpdatedAt = DateTime.UtcNow;
				}

				assignments.Add(assignment);
				returnRequests.Add(newReturnRequest);
			}
		}
		await dbContext.Database.BeginTransactionAsync();
		try
		{
			await dbContext.ReturnRequests.AddRangeAsync(returnRequests);
			dbContext.Assignments.UpdateRange(assignments);


			await dbContext.SaveChangesAsync();
			await dbContext.Database.CommitTransactionAsync();
		}
		catch (Exception)
		{
			await dbContext.Database.RollbackTransactionAsync();
			throw;
		}
	}

	public static async Task<IApplicationBuilder> DeleteAllDataAsync(this IApplicationBuilder app)
	{
		using (var serviceScope = app.ApplicationServices.CreateScope())
		{
			var dbContext = serviceScope.ServiceProvider.GetRequiredService<AssetManagementDBContext>();

			// Ensure the database is created
			await dbContext.Database.EnsureCreatedAsync();

			// Delete seed data as needed
			if (dbContext.Users.Any())
			{
				dbContext.Users.RemoveRange(dbContext.Users);
			}

			if (dbContext.Assets.Any())
			{
				dbContext.Assets.RemoveRange(dbContext.Assets);
			}

			if (dbContext.Categories.Any())
			{
				dbContext.Categories.RemoveRange(dbContext.Categories);
			}

			if (dbContext.Assignments.Any())
			{
				dbContext.Assignments.RemoveRange(dbContext.Assignments);
			}

			if (dbContext.ReturnRequests.Any())
			{
				dbContext.ReturnRequests.RemoveRange(dbContext.ReturnRequests);
			}
			// Save changes to the database
			await dbContext.SaveChangesAsync();
			Console.WriteLine("Successfully delete the database records");
		}

		return app;
	}
}
