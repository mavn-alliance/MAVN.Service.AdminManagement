using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IAdminUserService
    {
        Task<IReadOnlyList<AdminUser>> GetAllAsync();

        Task<PaginatedAdminUserModel> GetPaginatedAsync(int currentPage, int pageSize, bool? active);

        Task<AdminUserResult> GetByEmailAsync(string email, bool? active);

        Task<AdminUserResult> UpdateDataAsync(
            string adminUserId,
            string company,
            string department,
            string firstName,
            string lastName,
            string jobTitle,
            string phoneNumber,
            bool isActive);

        Task<AdminPasswordResetResult> ResetPasswordAsync(string adminUserId, string password);
        
        Task<AdminUserResult> UpdatePermissionsAsync(
            string adminUserId,
            List<Permission> permissions);

        Task<RegistrationResultModel> RegisterAsync(RegistrationRequestDto model);

        Task<AdminUserResult> GetByIdAsync(string adminId);

        Task<List<Permission>> GetPermissionsAsync(string adminId);
    }
}
