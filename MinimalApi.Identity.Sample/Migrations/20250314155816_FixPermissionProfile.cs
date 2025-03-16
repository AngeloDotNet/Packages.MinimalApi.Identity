using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalApi.Identity.Sample.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "Profile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "Profiles");
        }
    }
}
