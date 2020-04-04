namespace MAVN.Service.AdminManagement.Client.Models.Requests
{
    /// <summary>
    /// Request model for getting by email.
    /// </summary>
    public class GetByEmailRequestModel
    {
        /// <summary>Email</summary>
        public string Email { get; set; }
        
        /// <summary>Whether to select only Active or Non-active users. Null for both.</summary>
        public bool? Active { get; set; }
    }
}
