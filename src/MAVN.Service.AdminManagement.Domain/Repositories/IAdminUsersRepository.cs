using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Repositories
{
    public interface IAdminUsersRepository
    {
        Task<IReadOnlyList<AdminUserEncrypted>> GetAllAsync();

        Task<(IReadOnlyList<AdminUserEncrypted> admins, int count)> GetPaginatedAsync(int skip, int take, bool? active);

        Task<AdminUserEncrypted> GetByEmailAsync(string emailHash, bool? active);

        Task<bool> TryCreateAsync(AdminUserEncrypted adminUserEncrypted);

        Task<bool> TryUpdateAsync(AdminUserEncrypted adminUserEncrypted);

        Task<AdminUserEncrypted> GetAsync(string adminUserId);
    }
}
