// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Migrations
{
    [DbContext(typeof(AdminManagementContext))]
    [Migration("20190717082827_IndexRemoval")]
    partial class IndexRemoval
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("admin_users")
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MAVN.Service.AdminManagement.MsSqlRepositories.AdminUsers.Entities.AdminUserEntity", b =>
                {
                    b.Property<string>("AdminUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("admin_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email");

                    b.Property<string>("FirstName")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .HasColumnName("last_name");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnName("registered_at");

                    b.HasKey("AdminUserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("AdminUser");
                });
#pragma warning restore 612, 618
        }
    }
}
