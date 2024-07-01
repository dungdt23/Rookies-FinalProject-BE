using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "ReturnRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Populate the LocationId column with values from the related Assignment's Asset
            migrationBuilder.Sql(@"
                UPDATE rr
                SET rr.LocationId = a.LocationId
                FROM ReturnRequests rr
                INNER JOIN Assignments ass ON rr.AssignmentId = ass.Id
                INNER JOIN Assets a ON ass.AssetId = a.Id
            ");

            // Alter the column to remove the default value and make it required
            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "ReturnRequests",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_LocationId",
                table: "ReturnRequests",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Locations_LocationId",
                table: "ReturnRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Locations_LocationId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_LocationId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ReturnRequests");
        }
    }
}