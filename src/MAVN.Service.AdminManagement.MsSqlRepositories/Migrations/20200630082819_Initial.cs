using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
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
                    email_hash = table.Column<string>(type: "char(64)", nullable: false),
                    registered_at = table.Column<DateTime>(nullable: false),
                    is_disabled = table.Column<bool>(nullable: false, defaultValue: false),
                    use_custom_permissions = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUser", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "email_verification_codes",
                schema: "admin_users",
                columns: table => new
                {
                    admin_id = table.Column<string>(nullable: false),
                    code = table.Column<string>(nullable: false),
                    is_verified = table.Column<bool>(nullable: false),
                    expire_date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_verification_codes", x => x.admin_id);
                });

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
                name: "IX_AdminUser_email_hash",
                schema: "admin_users",
                table: "AdminUser",
                column: "email_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_codes_code",
                schema: "admin_users",
                table: "email_verification_codes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_permissions_admin_id",
                schema: "admin_users",
                table: "permissions",
                column: "admin_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUser",
                schema: "admin_users");

            migrationBuilder.DropTable(
                name: "email_verification_codes",
                schema: "admin_users");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "admin_users");
        }
    }
}
