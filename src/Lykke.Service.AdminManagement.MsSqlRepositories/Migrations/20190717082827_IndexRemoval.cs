using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class IndexRemoval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUser_admin_id",
                schema: "admin_users",
                table: "AdminUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_admin_id",
                schema: "admin_users",
                table: "AdminUser",
                column: "admin_id",
                unique: true);
        }
    }
}
