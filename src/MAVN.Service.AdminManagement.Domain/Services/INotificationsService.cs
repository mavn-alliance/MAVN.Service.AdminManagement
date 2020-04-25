using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models.Emails;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface INotificationsService
    {
        Task NotifyAdminCreatedAsync(AdminCreatedEmailDto model);
        Task NotifyAdminPasswordResetAsync(string adminUserId, string email, string login, string password, string name);
    }
}
