using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Hold information about the Current page and the amount of items on each page
    /// </summary>
    public class PaginationRequestModel
    {
        /// <summary>
        /// The Current Page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The amount of items that the page holds
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>Whether to select only Active or Non-active users. Null for both.</summary>
        public bool? Active { get; set; }
    }
}
