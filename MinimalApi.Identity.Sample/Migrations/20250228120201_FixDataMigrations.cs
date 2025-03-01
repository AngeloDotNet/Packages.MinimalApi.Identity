using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MinimalApi.Identity.Sample.Migrations
{
    /// <inheritdoc />
    public partial class FixDataMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(

                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Profiles");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Default", "Name" },
                values: new object[,]
                {
                    { 22, true, "GetProfiles" },
                    { 23, true, "EditProfiles" },
                    { 24, true, "DeleteProfiles" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "Users");
        }
    }
}
