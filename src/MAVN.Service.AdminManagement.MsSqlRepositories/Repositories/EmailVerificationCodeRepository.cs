using System;
using System.Threading.Tasks;
using MAVN.Persistence.PostgreSQL.Legacy;
using MAVN.Service.AdminManagement.Domain.Models.Verification;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.MsSqlRepositories.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Repositories
{
    public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
    {
        private readonly PostgreSQLContextFactory<AdminManagementContext> _contextFactory;
        private readonly TimeSpan _verificationLinkExpirePeriod;

        public EmailVerificationCodeRepository(
            PostgreSQLContextFactory<AdminManagementContext> contextFactory,
            TimeSpan verificationLinkExpirePeriod)
        {
            _contextFactory = contextFactory;
            _verificationLinkExpirePeriod = verificationLinkExpirePeriod;
        }

        public async Task<IVerificationCode> CreateOrUpdateAsync(string adminId, string verificationCode)
        {
            var entity =
                EmailVerificationCodeEntity.Create(adminId, verificationCode, _verificationLinkExpirePeriod);

            using (var context = _contextFactory.CreateDataContext())
            {
                await context.EmailVerificationCodes.AddAsync(entity);

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    if (e.InnerException is PostgresException sqlException &&
                        sqlException.SqlState == PostgreSqlErrorCodes.UniqueViolation.ToString())
                    {
                        context.EmailVerificationCodes.Update(entity);

                        await context.SaveChangesAsync();
                    }
                    else throw;
                }
            }

            return entity;
        }

        public async Task<IVerificationCode> GetByValueAsync(string value)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.EmailVerificationCodes.AsNoTracking().SingleOrDefaultAsync(x => x.VerificationCode == value);

                return entity;
            }
        }

        public async Task SetAsVerifiedAsync(string value)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.EmailVerificationCodes.SingleOrDefaultAsync(x => x.VerificationCode == value);

                if (entity == null)
                    throw new InvalidOperationException($"Verification code {value} doesn't exist");

                entity.IsVerified = true;

                context.EmailVerificationCodes.Update(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
