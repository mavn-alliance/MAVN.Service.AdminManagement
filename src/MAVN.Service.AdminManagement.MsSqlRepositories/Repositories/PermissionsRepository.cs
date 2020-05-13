using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MAVN.Common.MsSql;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Repositories
{
    public class PermissionsRepository : IPermissionsRepository
    {
        private readonly IDbContextFactory<AdminManagementContext> _contextFactory;

        public PermissionsRepository(
            IDbContextFactory<AdminManagementContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IReadOnlyList<Permission>> GetAsync(string adminId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Permissions.Where(x => x.AdminUserId == adminId).ToListAsync();

                return entities.Select(x =>
                        new Permission
                        {
                            Type = x.Type,
                            Level = x.Level
                        })
                    .ToImmutableList();
            }
        }

        public async Task<IReadOnlyList<Permission>> AddAndRemoveAsync(string adminId,
            IReadOnlyList<Permission> permissionsToAdd, IReadOnlyList<Permission> permissionsToRemove)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var entities = await context.Permissions.Where(x => x.AdminUserId == adminId).ToListAsync();

                context.Permissions.RemoveRange(
                    entities.Where(
                        x => permissionsToRemove.Any(y => x.Type == y.Type && x.Level == y.Level)));

                context.Permissions.AddRange(permissionsToAdd.Select(x => new
                    PermissionEntity
                    {
                        Id = Guid.NewGuid().ToString(),
                        AdminUserId = adminId,
                        Type = x.Type,
                        Level = x.Level
                    }));

                await context.SaveChangesAsync();

                return permissionsToRemove;
            }
        }
    }
}