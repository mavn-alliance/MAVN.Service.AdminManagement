using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class Permissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.AddColumn<bool>(
                name: "is_disabled",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "use_custom_permissions",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "admin_users",
                columns: table => new
                {
                    id = table.Column<string>(nullable: false),
                    admin_id = table.Column<string>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    level = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_permissions_admin_id",
                schema: "admin_users",
                table: "permissions",
                column: "admin_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permissions",
                schema: "admin_users");

            migrationBuilder.DropColumn(
                name: "is_disabled",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.DropColumn(
                name: "use_custom_permissions",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                defaultValue: true);
        }
    }
}
