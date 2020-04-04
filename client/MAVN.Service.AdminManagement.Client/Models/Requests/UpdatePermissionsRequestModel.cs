using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MAVN.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdatePermissionsRequestModel
    {
        /// <summary>Admin Id</summary>
        [Required]
        public string AdminUserId { get; set; }
        
        /// <summary>Back Office Permissions</summary>
        [Required]
        public IReadOnlyList<AdminPermission> Permissions { get; set; }
    }
}