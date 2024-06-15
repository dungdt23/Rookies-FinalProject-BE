using AssetManagement.Api.Extensions;
using AssetManagement.Api.ValidateModel;
using AssetManagement.Application.IRepositories;
using AssetManagement.Application.IServices.IUserServices;
using AssetManagement.Application.Mappings;
using AssetManagement.Application.Services.UserServices;
using AssetManagement.Infrastructure.Migrations;
using AssetManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
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
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // Add services to the container.
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUserRepository, UserRepository>();


            builder.Services.AddScoped<IUserService, UserService>();


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

            // Configure the HTTP request pipeline.
            // if (app.Environment.IsDevelopment())
            // {
                app.UseSwagger();
                app.UseSwaggerUI();
            // }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();


            app.MapControllers();

            await app.SeedData();

            app.Run();
        }
    }
}
