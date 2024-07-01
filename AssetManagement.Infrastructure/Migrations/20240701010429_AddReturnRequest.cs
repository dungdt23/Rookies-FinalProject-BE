using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Assets_AssetId",
                table: "ReturnRequests");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "ReturnRequests",
                newName: "AssignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_ReturnRequests_AssetId",
                table: "ReturnRequests",
                newName: "IX_ReturnRequests_AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Assignments_AssignmentId",
                table: "ReturnRequests",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Assignments_AssignmentId",
                table: "ReturnRequests");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "ReturnRequests",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_ReturnRequests_AssignmentId",
                table: "ReturnRequests",
                newName: "IX_ReturnRequests_AssetId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Assets_AssetId",
                table: "ReturnRequests",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
