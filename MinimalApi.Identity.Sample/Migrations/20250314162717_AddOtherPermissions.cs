using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MinimalApi.Identity.Sample.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "Profilo");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Value",
                value: "ProfiloRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Value",
                value: "ProfiloWrite");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Value",
                value: "Ruolo");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Value",
                value: "RuoloRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Value",
                value: "RuoloWrite");

            migrationBuilder.InsertData(
                table: "ClaimTypes",
                columns: new[] { "Id", "Type", "Value" },
                values: new object[,]
                {
                    { 7, "Permission", "Permesso" },
                    { 8, "Permission", "PermessoRead" },
                    { 9, "Permission", "PermessoWrite" },
                    { 10, "Permission", "Modulo" },
                    { 11, "Permission", "ModuloRead" },
                    { 12, "Permission", "ModuloWrite" },
                    { 13, "Permission", "Licenza" },
                    { 14, "Permission", "LicenzaRead" },
                    { 15, "Permission", "LicenzaWrite" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Value",
                value: "Profile");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Value",
                value: "ProfileRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Value",
                value: "ProfileWrite");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "Value",
                value: "Role");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "Value",
                value: "RoleRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "Value",
                value: "RoleWrite");
        }
    }
}
