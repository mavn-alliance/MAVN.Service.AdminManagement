using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AdminManagement.Domain.Enums;
using Lykke.Service.AdminManagement.Domain.Models;

namespace Lykke.Service.AdminManagement.Domain.Services
{
    public interface IPermissionsService
    {
        Task<IReadOnlyList<Permission>> CreateOrUpdatePermissionsAsync(string adminId,
            IReadOnlyList<Permission> permissions);

        Task<IReadOnlyList<Permission>> GetPermissionsAsync(string adminId);
    }
}