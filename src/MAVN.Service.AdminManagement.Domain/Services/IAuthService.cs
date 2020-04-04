using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IAuthService
    {
        Task<AuthResultModel> AuthAsync(string login, string password);
        Task<PasswordUpdateError> UpdatePasswordAsync(string login, string oldPassword, string newPassword);
    }
}
