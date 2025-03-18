using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalApi.Identity.Sample.Migrations
{
    /// <inheritdoc />
    public partial class FixClaimsType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "Value",
                value: "Claim");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "Value",
                value: "ClaimRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                column: "Value",
                value: "ClaimWrite");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "Value",
                value: "Permesso");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "Value",
                value: "PermessoRead");

            migrationBuilder.UpdateData(
                table: "ClaimTypes",
                keyColumn: "Id",
                keyValue: 9,
                column: "Value",
                value: "PermessoWrite");
        }
    }
}
