using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinimalApi.Identity.Sample.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Default",
                table: "Permissions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Default",
                table: "Permissions");
        }
    }
}
