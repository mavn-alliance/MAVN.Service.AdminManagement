using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IPermissionsCache
    {
        Task<(bool, List<Permission>)> TryGetAsync(string adminId);
        Task SetAsync(string adminId, List<Permission> permissions);
    }
}