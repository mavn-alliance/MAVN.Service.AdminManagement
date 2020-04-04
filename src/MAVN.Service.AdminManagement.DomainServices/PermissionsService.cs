using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.Domain.Services;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;

        public PermissionsService(IPermissionsRepository permissionsRepository)
        {
            _permissionsRepository = permissionsRepository;
        }

        public async Task<IReadOnlyList<Permission>> CreateOrUpdatePermissionsAsync(string adminId, IReadOnlyList<Permission> permissions)
        {
            var currentPermissions = await _permissionsRepository.GetAsync(adminId);

            var permissionsToRemove =
                currentPermissions
                    .Where(x => !permissions.Any(y => y.Type == x.Type && y.Level == x.Level))
                    .ToImmutableList();
            
            var permissionsToAdd =
                permissions
                    .Where(x => currentPermissions.All(y => y.Type != x.Type || y.Level != x.Level))
                    .ToImmutableList();

            await _permissionsRepository.AddAndRemoveAsync(adminId, permissionsToAdd, permissionsToRemove);

            return permissions;
        }

        public Task<IReadOnlyList<Permission>> GetPermissionsAsync(string adminId)
        {
            return _permissionsRepository.GetAsync(adminId);
        }
    }
}