using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common;
using Lykke.Common.MsSql;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Repositories
{
    public class AdminUsersRepository : IAdminUsersRepository
    {
        private readonly IDbContextFactory<AdminManagementContext> _contextFactory;
        private readonly IMapper _mapper;

        public AdminUsersRepository(
            IDbContextFactory<AdminManagementContext> contextFactory,
            IMapper mapper)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<AdminUserEncrypted>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.AdminUser.AsNoTracking().ToListAsync();

                return _mapper.Map<List<AdminUserEncrypted>>(entities);
            }
        }

        public async Task<(IReadOnlyList<AdminUserEncrypted> admins, int count)> GetPaginatedAsync(int skip, int take, bool? active)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.AdminUser.AsNoTracking()
                    .Where(x => !active.HasValue || x.IsDisabled == !active.Value)
                    .OrderByDescending(x => x.RegisteredAt)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                var count = context.AdminUser.AsNoTracking().Count(x => !active.HasValue || x.IsDisabled == !active.Value);
                var admins = _mapper.Map<List<AdminUserEncrypted>>(entities);

                return (admins, count);
            }
        }

        public async Task<AdminUserEncrypted> GetByEmailAsync(string email, bool? active)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var emailHash = GetHash(email);
                
                var entity = await context
                    .AdminUser.AsNoTracking().FirstOrDefaultAsync(c => c.EmailHash == emailHash && (!active.HasValue || c.IsDisabled == !active.Value));

                if (entity == null)
                {
                    emailHash = GetHash(email.ToLower());
                    
                    entity = await context
                        .AdminUser.AsNoTracking().FirstOrDefaultAsync(c => c.EmailHash == emailHash && (!active.HasValue || c.IsDisabled == !active.Value));
                }

                return _mapper.Map<AdminUserEncrypted>(entity);
            }
        }

        public async Task<bool> TryCreateAsync(AdminUserEncrypted adminUserEncrypted)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.AdminUser
                    .FirstOrDefaultAsync(a => a.EmailHash == adminUserEncrypted.EmailHash);

                if (entity != null)
                    return false;

                entity = _mapper.Map<AdminUserEntity>(adminUserEncrypted);

                context.AdminUser.Add(entity);

                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task<bool> TryUpdateAsync(AdminUserEncrypted adminUserEncrypted)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.AdminUser
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AdminUserId == adminUserEncrypted.AdminUserId);

                if (entity == null)
                    return false;

                entity = _mapper.Map<AdminUserEntity>(adminUserEncrypted);

                context.AdminUser.Update(entity);

                await context.SaveChangesAsync();

                return true;
            }
        }

        public async Task<AdminUserEncrypted> GetAsync(string adminUserId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entity = await context.AdminUser.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AdminUserId == adminUserId);

                return _mapper.Map<AdminUserEncrypted>(entity);
            }
        }

        private static string GetHash(string value)
        {
            return new Sha256HashingUtil().Sha256HashEncoding1252(value);
        }
    }
}
