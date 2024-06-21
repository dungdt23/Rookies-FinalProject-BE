using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Bogus;

namespace AssetManagement.Api.Extensions;

public static class ApplicationExtension
{
	public static async Task<IApplicationBuilder> SeedDataAsync(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var dbContext = scope.ServiceProvider.GetService<AssetManagementDBContext>();
			var userService = scope.ServiceProvider.GetService<IUserService>();
			dbContext.Database.EnsureCreated();
			if (!dbContext.Types.Any())
			{
				dbContext.Add(new Domain.Entities.Type { TypeName = "Staff", Description = "Staff of system", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Domain.Entities.Type { TypeName = "Admin", Description = "Administration of system", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.SaveChanges();
			}

			if (!dbContext.Locations.Any())
			{
				dbContext.Add(new Location { LocationName = "Hà Nội", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Location { LocationName = "Đà Nẵng", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Location { LocationName = "Hồ Chí Minh", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.SaveChanges();
			}

			if (!dbContext.Users.Any())
			{
				List<string> firstNames = new List<string> { "Nguyễn", "Trần", "Lê", "Phan", "Hoàng", "Huỳnh", "Phan", "Vũ" };
				List<string> lastNames = new List<string> { "An", "Bảo", "Cường", "Minh Duy", "Minh Ánh", "Lan", "Mai", "Ngọc" };
				List<Domain.Entities.Type> types = dbContext.Types.ToList();
				List<Guid> locationIds = dbContext.Locations.Select(t => t.Id).ToList();
				Random random = new Random();

				DateTime startDate = new DateTime(1990, 1, 1);
				int dateOfBirthRange = (DateTime.Today.AddYears(-18) - startDate).Days;

				//pre-user for easier login
				var adminUser = new CreateUpdateUserForm
				{
					FirstName = "Nguyễn",
					LastName = "Minh Ánh",
					Type = "Admin",
					LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
					DateOfBirth = new DateTime(2003, 9, 19),
					JoinedDate = new DateTime(2024, 4, 22),
					Gender = TypeGender.Male
				};
				await userService.CreateAsync(adminUser);


				for (int i = 0; i < 300; i++)
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


					CreateUpdateUserForm form = new CreateUpdateUserForm
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
				dbContext.Add(new Category { CategoryName = "Laptop", Prefix = "LA", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Category { CategoryName = "Monitor", Prefix = "MO", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Category { CategoryName = "PC", Prefix = "PC", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Category { CategoryName = "Keyboard", Prefix = "KE", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.SaveChanges();
			}

			if (!dbContext.Assets.Any())
			{
				Random random = new Random();
				var categories = dbContext.Categories.ToList();
				var locations = dbContext.Locations.ToList();
				var assetCodes = new Dictionary<Guid, int>();
				var assetFaker = new Faker<Asset>()
					.RuleFor(a => a.CategoryId, f => f.PickRandom(categories).Id)
					.RuleFor(a => a.LocationId, f => f.PickRandom(locations).Id)
					.RuleFor(a => a.Specification, f => f.Commerce.ProductDescription())
					.RuleFor(a => a.InstalledDate, f => f.Date.Past(2))
					.RuleFor(a => a.State, f => (TypeAssetState)random.Next(1, 6))
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
					.RuleFor(a => a.CreatedAt, f => DateTime.Now)
					.RuleFor(a => a.IsDeleted, f => false)
					.Generate(450);

				dbContext.AddRange(assetFaker);
				dbContext.SaveChanges();
			}

			if (!dbContext.Assignments.Any())
			{
				var assigner = dbContext.Users.Where(a => a.FirstName == "Nguyễn" && a.LastName == "Minh Ánh").FirstOrDefault();
				var typeStaff = dbContext.Types.Where(a => a.TypeName == "Staff").FirstOrDefault();
				var assignee = dbContext.Users.Where(a => a.TypeId == typeStaff.Id).ToList();
				var assets = dbContext.Assets.Where(a => a.State == TypeAssetState.Available).ToList();

				for (int i = 0; i < assets.Count; i++)
				{
					var assignmentFaker = new Faker<Assignment>()
									.RuleFor(a => a.AssetId, f => assets[i].Id)
									.RuleFor(a => a.AssignerId, f => assigner.Id)
									.RuleFor(a => a.AssigneeId, f => assignee[i].Id)
									.RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
									.RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
									.RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
					var assignment = assignmentFaker.Generate(1).FirstOrDefault();
					dbContext.Assignments.Add(assignment);
					assets[i].State = TypeAssetState.NotAvailable;
					dbContext.Assets.Update(assets[i]);
				}
				dbContext.SaveChanges();
			}

			return app;
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

			// Save changes to the database
			await dbContext.SaveChangesAsync();
			Console.WriteLine("Successfully delete the database records");
		}

		return app;
	}
}
