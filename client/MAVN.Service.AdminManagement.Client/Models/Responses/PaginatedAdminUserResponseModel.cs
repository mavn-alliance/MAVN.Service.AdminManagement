using System.Collections.Generic;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Returns Paginated information about Admin Users
    /// </summary>
    public class PaginatedAdminUserResponseModel
    {
        /// <summary>
        /// Current page in pagination result
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total count of records
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// List of Admin Users for the given page
        /// </summary>
        public IEnumerable<AdminUser> AdminUsers { get; set; }
    }
}
