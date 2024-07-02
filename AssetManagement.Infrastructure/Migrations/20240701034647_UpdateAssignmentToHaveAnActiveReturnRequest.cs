using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignmentToHaveAnActiveReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestedDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveReturnRequestId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments",
                column: "ActiveReturnRequestId",
                principalTable: "ReturnRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_ReturnRequests_ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "RequestedDate",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ActiveReturnRequestId",
                table: "Assignments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReturnedDate",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
