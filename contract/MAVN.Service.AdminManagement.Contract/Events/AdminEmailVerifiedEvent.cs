using System;

namespace MAVN.Service.AdminManagement.Contract.Events
{
    /// <summary>
    /// Event which is raised when admin uses his email verification code to verify his email
    /// </summary>
    public class AdminEmailVerifiedEvent
    {
        /// <summary>
        /// Represents Admin Id 
        /// </summary>
        public string AdminId { get; set; }

        /// <summary>
        /// Represents Timestamp of Email verification
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
