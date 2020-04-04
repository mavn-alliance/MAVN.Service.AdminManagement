using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Repositories
{
    public interface IPermissionsRepository
    {
        Task<IReadOnlyList<Permission>> GetAsync(string adminId);
        Task<IReadOnlyList<Permission>> AddAndRemoveAsync(string adminId, IReadOnlyList<Permission> permissionsToAdd, IReadOnlyList<Permission> permissionsToRemove);
    }
}