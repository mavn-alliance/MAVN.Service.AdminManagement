// <auto-generated />
using System;
using MAVN.Service.AdminManagement.MsSqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    [DbContext(typeof(AdminManagementContext))]
    [Migration("20191112073458_AdminState")]
    partial class AdminState
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("admin_users")
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MAVN.Service.AdminManagement.MsSqlRepositories.Entities.AdminUserEntity", b =>
                {
                    b.Property<string>("AdminUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("admin_id");

                    b.Property<string>("EmailHash")
                        .IsRequired()
                        .HasColumnName("email_hash")
                        .HasColumnType("char(64)");

                    b.Property<bool>("IsActive")
                        .HasColumnName("is_active");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnName("registered_at");

                    b.HasKey("AdminUserId");

                    b.HasIndex("EmailHash")
                        .IsUnique();

                    b.ToTable("AdminUser");
                });
#pragma warning restore 612, 618
        }
    }
}
