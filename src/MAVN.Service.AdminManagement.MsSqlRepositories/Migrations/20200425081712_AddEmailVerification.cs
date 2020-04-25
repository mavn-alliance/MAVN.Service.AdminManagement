using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class AddEmailVerification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_email_verification_codes_code",
                schema: "admin_users",
                table: "email_verification_codes",
                column: "code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_verification_codes",
                schema: "admin_users");
        }
    }
}
