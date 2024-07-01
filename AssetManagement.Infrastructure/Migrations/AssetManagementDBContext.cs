using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Migrations
{
    public class AssetManagementDBContext : DbContext
    {
        public AssetManagementDBContext()
        {

        }
        public AssetManagementDBContext(DbContextOptions<AssetManagementDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(e => e.Type)
                .WithMany(d => d.Users)
                .HasForeignKey(x => x.TypeId);

                entity.HasOne(e => e.Location)
                .WithMany(d => d.Users)
                .HasForeignKey(x => x.LocationId);

            });

            modelBuilder.Entity<ReturnRequest>(entity =>
            {
                entity.HasOne(e => e.Requestor)
                .WithMany(d => d.RequestedReturnRequests)
                .HasForeignKey(x => x.RequestorId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(e => e.Responder)
                .WithMany(d => d.RespondedReturnRequests)
                .HasForeignKey(x => x.ResponderId)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Assignment)
                .WithMany(d => d.ReturnRequests)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasOne(e => e.Category)
                .WithMany(d => d.Assets)
                .HasForeignKey(x => x.CategoryId);

                entity.HasOne(e => e.Location)
                .WithMany(d => d.Assets)
                .HasForeignKey(x => x.LocationId);

            });

            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasOne(e => e.Assigner)
                .WithMany(d => d.AssignedAssignments)
                .HasForeignKey(x => x.AssignerId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(e => e.Assignee)
                .WithMany(d => d.ReceivedAssignments)
                .HasForeignKey(x => x.AssigneeId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(e => e.Asset)
                .WithMany(d => d.Assignments)
                .HasForeignKey(x => x.AssetId);

                entity.HasOne(e => e.ActiveReturnRequest)
                .WithMany()
                .HasForeignKey(x => x.ActiveReturnRequestId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<GlobalSetting>().HasData(
                new GlobalSetting { Id = 1, GlobalInvalidationTimestamp = DateTime.MinValue }
            );
        }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ReturnRequest> ReturnRequests { get; set; }
        public DbSet<Domain.Entities.Type> Types { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GlobalSetting> GlobalSettings { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var builder = new ConfigurationBuilder()
        //          .SetBasePath(Directory.GetCurrentDirectory())
        //          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //        IConfigurationRoot configuration = builder.Build();
        //        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        //    }
        //}

    }
}
