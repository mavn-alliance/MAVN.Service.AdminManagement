using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class SensitiveData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUser_email",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.RenameColumn(
                name: "email",
                schema: "admin_users",
                table: "AdminUser",
                newName: "email_hash");
            
            migrationBuilder.AlterColumn<string>(
                name: "email_hash",
                schema: "admin_users",
                table: "AdminUser",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string));
            
            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_email_hash",
                schema: "admin_users",
                table: "AdminUser",
                column: "email_hash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdminUser_email_hash",
                schema: "admin_users",
                table: "AdminUser");

            migrationBuilder.RenameColumn(
                name: "email_hash",
                schema: "admin_users",
                table: "AdminUser",
                newName: "email");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "admin_users",
                table: "AdminUser",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "admin_users",
                table: "AdminUser",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "admin_users",
                table: "AdminUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminUser_email",
                schema: "admin_users",
                table: "AdminUser",
                column: "email",
                unique: true);
        }
    }
}
