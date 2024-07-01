using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NoActionOnDeletingActiveReturnRequest2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_ActiveReturnRequestId",
                table: "Assignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId",
                principalTable: "ReturnRequests",
                principalColumn: "Id");
        }
    }
}
