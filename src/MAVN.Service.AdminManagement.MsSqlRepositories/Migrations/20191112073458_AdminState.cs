using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class AdminState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser");
        }
    }
}
