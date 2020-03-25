using System.Threading.Tasks;
using Lykke.Service.AdminManagement.Domain.Enums;
using Lykke.Service.AdminManagement.Domain.Models;

namespace Lykke.Service.AdminManagement.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthResultModel> AuthAsync(string login, string password);
        Task<PasswordUpdateError> UpdatePasswordAsync(string login, string oldPassword, string newPassword);
    }
}
