using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Models;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;

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
                List<Guid> typeIds = dbContext.Types.Select(t => t.Id).ToList();
                List<Guid> locationIds = dbContext.Locations.Select(t => t.Id).ToList();
                Random random = new Random();

                DateTime startDate = new DateTime(1990, 1, 1);
                int dateOfBirthRange = (DateTime.Today.AddYears(-18) - startDate).Days;


                for (int i = 0; i < 300; i++)
                {
                    string firstName = firstNames[random.Next(firstNames.Count)];
                    string lastName = lastNames[random.Next(lastNames.Count)];
                    Guid typeId = typeIds[random.Next(typeIds.Count)];
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
                        TypeId = typeId,
                        LocationId = locationId,
                        DateOfBirth = dateOfBirth,
                        JoinedDate = joinedDate,
                        Gender = (TypeGender) random.Next(0,2)
                    };

                    await userService.CreateAsync(form);

                }

            }

            return app;
        }
    }

}
