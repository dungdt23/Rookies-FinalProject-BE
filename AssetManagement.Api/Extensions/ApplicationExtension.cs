using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Bogus;

namespace AssetManagement.Api.Extensions;

public static class ApplicationExtension
{
	public static async Task<IApplicationBuilder> SeedData(this IApplicationBuilder app)
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
				dbContext.Add(new Location { LocationName = "Ha Noi", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Location { LocationName = "Da Nang", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.Add(new Location { LocationName = "Ho Chi Minh", CreatedAt = DateTime.Now, IsDeleted = false });
				dbContext.SaveChanges();
			}

			if (!dbContext.Users.Any())
			{
				List<string> firstNames = new List<string> { "Nguyen", "Tran", "Le", "Phan", "Hoang", "Huynh", "Phan", "Vu" };
				List<string> lastNames = new List<string> { "An", "Bao", "Cuong", "Duy", "Anh", "Lan", "Mai", "Ngoc" };
				List<Domain.Entities.Type> types = dbContext.Types.ToList();
				List<Guid> locationIds = dbContext.Locations.Select(t => t.Id).ToList();
				Random random = new Random();

				DateTime startDate = new DateTime(1990, 1, 1);
				int dateOfBirthRange = (DateTime.Today.AddYears(-18) - startDate).Days;


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
				var categories = dbContext.Categories.ToList();
				var locations = dbContext.Locations.ToList();
				var assetCodes = new Dictionary<Guid, int>();
				var assetFaker = new Faker<Asset>()
					.RuleFor(a => a.CategoryId, f => f.PickRandom(categories).Id)
					.RuleFor(a => a.LocationId, f => f.PickRandom(locations).Id)
					.RuleFor(a => a.Specification, f => f.Commerce.ProductDescription())
					.RuleFor(a => a.InstalledDate, f => f.Date.Past(2))
					.RuleFor(a => a.State, f => TypeAssetState.Available)
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
					.Generate(20);

				dbContext.AddRange(assetFaker);
				dbContext.SaveChanges();
			}

			return app;
		}
	}

}
