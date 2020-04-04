using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IPermissionsService
    {
        Task<IReadOnlyList<Permission>> CreateOrUpdatePermissionsAsync(string adminId,
            IReadOnlyList<Permission> permissions);

        Task<IReadOnlyList<Permission>> GetPermissionsAsync(string adminId);
    }
}