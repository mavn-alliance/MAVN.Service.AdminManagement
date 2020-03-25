using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admin_users");

            migrationBuilder.CreateTable(
                name: "AdminUser",
                schema: "admin_users",
                columns: table => new
                {
                    admin_id = table.Column<string>(nullable: false),
                    email = table.Column<string>(nullable: false),
                    first_name = table.Column<string>(nullable: true),
                    last_name = table.Column<string>(nullable: true),
                    registered_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUser", x => x.admin_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_admin_id",
                schema: "admin_users",
                table: "AdminUser",
                column: "admin_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_email",
                schema: "admin_users",
                table: "AdminUser",
                column: "email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUser",
                schema: "admin_users");
        }
    }
}
