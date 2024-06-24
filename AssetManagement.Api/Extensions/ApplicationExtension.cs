﻿using AssetManagement.Application.Dtos.RequestDtos;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Infrastructure.Migrations;
using Bogus;
using Microsoft.EntityFrameworkCore;
using System;

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

                //pre-user admin for easier login
                var adminUser = new RequestUserCreateDto
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
                var adminUser1 = new RequestUserCreateDto
                {
                    FirstName = "Dao",
                    LastName = "Tien Dung",
                    Type = "Admin",
                    LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Đà Nẵng")).Id,
                    DateOfBirth = new DateTime(2002, 7, 20),
                    JoinedDate = new DateTime(2024, 4, 22),
                    Gender = TypeGender.Male
                };
                await userService.CreateAsync(adminUser1);
                var adminUser2 = new RequestUserCreateDto
                {
                    FirstName = "Nguyen",
                    LastName = "Viet Bao Son",
                    Type = "Admin",
                    LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hồ Chí Minh")).Id,
                    DateOfBirth = new DateTime(2002, 4, 9),
                    JoinedDate = new DateTime(2024, 4, 22),
                    Gender = TypeGender.Male
                };
                await userService.CreateAsync(adminUser2);




                //pre-user sstaff
                var staffUser = new RequestUserCreateDto
                {
                    FirstName = "Mac",
                    LastName = "Trang",
                    Type = "Staff",
                    LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Hà Nội")).Id,
                    DateOfBirth = new DateTime(1999, 01, 01),
                    JoinedDate = new DateTime(2024, 4, 22),
                    Gender = TypeGender.Female
                };
                await userService.CreateAsync(staffUser);

                var staffUser2 = new RequestUserCreateDto
                {
                    FirstName = "Ho",
                    LastName = "Huong",
                    Type = "Staff",
                    LocationId = (dbContext.Locations.FirstOrDefault(l => l.LocationName == "Đà Nẵng")).Id,
                    DateOfBirth = new DateTime(2003, 01, 01),
                    JoinedDate = new DateTime(2024, 4, 22),
                    Gender = TypeGender.Female
                };
                await userService.CreateAsync(staffUser2);


                var staffUser3 = new RequestUserCreateDto
                {
                    FirstName = "Pham",
                    LastName = "Quynh",
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
                var random = new Random();
                var assigner = dbContext.Users.Where(a => a.StaffCode.Equals("SD0001")).FirstOrDefault();
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
                //generate assignment for QC account in HN
                var assignerHN = dbContext.Users
                    .Include(a => a.Location)
                    .Where(a => a.StaffCode.Equals("SD0001")).FirstOrDefault();
                var assigneesHN = dbContext.Users.Where(a => a.UserName.Equals("trangm") || a.UserName.Equals("linhp")).ToList();
                var assetsHN = dbContext.Assets.Where(a => a.State == TypeAssetState.Available && a.LocationId == assignerHN.LocationId).ToList();
                for (int i = 0; i < 5; i++)
                {
                    var assignmentFaker = new Faker<Assignment>()
                                    .RuleFor(a => a.AssetId, f => assetsHN[i].Id)
                                    .RuleFor(a => a.AssignerId, f => assignerHN.Id)
                                    .RuleFor(a => a.AssigneeId, f => assigneesHN[random.Next(0, 2)].Id)
                                    .RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
                                    .RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
                                    .RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
                    var assignment = assignmentFaker.Generate(1).FirstOrDefault();
                    dbContext.Assignments.Add(assignment);
                    assetsHN[i].State = TypeAssetState.NotAvailable;
                    dbContext.Assets.Update(assetsHN[i]);
                }

                //generate assignment for QC account in DaNang
                var assignerDN = dbContext.Users
                    .Include(a => a.Location)
                    .Where(a => a.UserName.Equals("dungdt")).FirstOrDefault();
                var assigneeDN = dbContext.Users.Where(a => a.UserName.Equals("huongh")).FirstOrDefault();
                var assetsDN = dbContext.Assets.Where(a => a.State == TypeAssetState.Available && a.LocationId == assignerDN.LocationId).ToList();
                for (int i = 0; i < 5; i++)
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
                    assetsDN[i].State = TypeAssetState.NotAvailable;
                    dbContext.Assets.Update(assetsDN[i]);
                }
                //generate assignment for QC account in HCM
                var assignerHCM = dbContext.Users
                    .Include(a => a.Location)
                    .Where(a => a.UserName.Equals("sonnvb")).FirstOrDefault();
                var assigneeHCM = dbContext.Users.Where(a => a.UserName.Equals("quynhp")).FirstOrDefault();
                var assetsHCM = dbContext.Assets.Where(a => a.State == TypeAssetState.Available && a.LocationId == assignerHCM.LocationId).ToList();
                for (int i = 0; i < assetsHCM.Count; i++)
                {
                    var assignmentFaker = new Faker<Assignment>()
                                    .RuleFor(a => a.AssetId, f => assetsHCM[i].Id)
                                    .RuleFor(a => a.AssignerId, f => assignerHCM.Id)
                                    .RuleFor(a => a.AssigneeId, f => assigneeHCM.Id)
                                    .RuleFor(a => a.State, f => f.PickRandom<TypeAssignmentState>())
                                    .RuleFor(a => a.AssignedDate, f => f.Date.Past(1))
                                    .RuleFor(a => a.Note, f => f.Lorem.Sentence(5));
                    var assignment = assignmentFaker.Generate(1).FirstOrDefault();
                    dbContext.Assignments.Add(assignment);
                    assetsHCM[i].State = TypeAssetState.NotAvailable;
                    dbContext.Assets.Update(assetsHCM[i]);
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
