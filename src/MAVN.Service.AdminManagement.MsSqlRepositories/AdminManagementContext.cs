using System;
using System.Data.Common;
using JetBrains.Annotations;
using MAVN.Common.MsSql;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.AdminManagement.MsSqlRepositories
{
    public class AdminManagementContext : MsSqlContext
    {
        private const string Schema = "admin_users";

        internal DbSet<AdminUserEntity> AdminUser { get; set; }
        internal DbSet<PermissionEntity> Permissions { get; set; }
        internal DbSet<EmailVerificationCodeEntity> EmailVerificationCodes { get; set; }

        [UsedImplicitly]
        public AdminManagementContext() : base(Schema)
        {
        }

        public AdminManagementContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public AdminManagementContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminUserEntity>()
                .HasIndex(c => c.EmailHash)
                .IsUnique();
            
            modelBuilder.Entity<AdminUserEntity>()
                .Property(x => x.IsDisabled)
                .HasDefaultValue(false);
            
            modelBuilder.Entity<AdminUserEntity>()
                .Property(b => b.UseCustomPermissions)
                .HasDefaultValue(false);

            modelBuilder.Entity<PermissionEntity>()
                .HasIndex(c => c.AdminUserId);
            
            modelBuilder
                .Entity<PermissionEntity>()
                .Property(e => e.Level)
                .HasConversion(
                    v => v.ToString(),
                    v => (PermissionLevel)Enum.Parse(typeof(PermissionLevel), v));

            modelBuilder.Entity<EmailVerificationCodeEntity>()
                .HasIndex(_ => _.VerificationCode)
                .IsUnique();
        }
    }
}
