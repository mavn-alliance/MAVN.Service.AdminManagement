using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AdminManagement.Domain.Models;

namespace Lykke.Service.AdminManagement.Domain.Services
{
    public interface IPermissionsCache
    {
        Task<(bool, List<Permission>)> TryGetAsync(string adminId);
        Task SetAsync(string adminId, List<Permission> permissions);
    }
}