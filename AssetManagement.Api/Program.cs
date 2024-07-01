using AssetManagement.Api.Extensions;
using AssetManagement.Api.Middlewares;
using AssetManagement.Api.ValidateModel;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices;
using AssetManagement.Application.IServices.IAssetServices;
using AssetManagement.Application.IServices.IAssignmentServices;
using AssetManagement.Application.IServices.ICategoryServices;
using AssetManagement.Application.IServices.ILocationServices;
using AssetManagement.Application.IServices.IReportServices;
using AssetManagement.Application.IServices.ITypeServices;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Mappings;
using AssetManagement.Application.Services;
using AssetManagement.Application.Services.AssetServices;
using AssetManagement.Application.Services.AssignmentServices;
using AssetManagement.Application.Services.CategoryServices;
using AssetManagement.Application.Services.LocationServices;
using AssetManagement.Application.Services.ReportServices;
using AssetManagement.Application.Services.TypeServices;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Infrastructure.Migrations;
using AssetManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;


namespace AssetManagement.Api
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;
            builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("ApplicationSettings"));
            builder.Services.AddDbContext<AssetManagementDBContext>(
                options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }

            );
            builder.Services.AddDateOnlyTimeOnlyStringConverters();

            // The following line enables Application Insights telemetry collection.
            builder.Services.AddApplicationInsightsTelemetry();

            // Add services to the container.
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAssetRepository, AssetRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<IReturnRequestRepository, ReturnRequestRepository>();
            builder.Services.AddScoped<IGlobalSettingsRepository, GlobalSettinsgRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();


            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<ITypeService, TypeService>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<IReturnRequestService, ReturnRequestService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IJwtInvalidationService, JwtInvalidationService>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });


            //Add ValidationModelAsService
            builder.Services.AddScoped<ValidateModelFilter>();

            builder.Services.AddControllers(options =>
            {
                //Add custom validation error
                options.Filters.Add<ValidateModelFilter>();
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.UseDateOnlyTimeOnlyStringConverters();
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            // Mapping profile between dtos and entities
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            ).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["ApplicationSettings:Secret"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ApiExceptionHandlingMiddleware>();
            }

            // if (app.Environment.IsDevelopment())
            // {
            app.UseSwagger();
            app.UseSwaggerUI();
            // }

            app.MigrationDatabase();

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ValidateUserMiddleware>();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                await app.DeleteAllDataAsync();
                await app.SeedDataAsync();
            }

            app.Run();
        }
    }
}
