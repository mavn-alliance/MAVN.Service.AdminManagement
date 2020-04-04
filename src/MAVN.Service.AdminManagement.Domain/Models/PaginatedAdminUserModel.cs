using System.Collections.Generic;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class PaginatedAdminUserModel
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IReadOnlyList<AdminUser> AdminUsers { get; set; }
    }
}
