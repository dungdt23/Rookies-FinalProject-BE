using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NoActionOnDeletingActiveReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId",
                principalTable: "ReturnRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId",
                principalTable: "ReturnRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
