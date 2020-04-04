using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    public partial class IsActiveDefaultValueFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));

            migrationBuilder.Sql("UPDATE [admin_users].[AdminUser] SET [admin_users].[AdminUser].[is_active] = 'true'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                schema: "admin_users",
                table: "AdminUser",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
