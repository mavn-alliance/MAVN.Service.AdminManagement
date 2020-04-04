using System;

namespace MAVN.Service.AdminManagement.Contract.Events
{
    /// <summary>
    /// Represents a Customer registration event
    /// </summary>
    public class CustomerRegistrationEvent
    {
        /// <summary>
        /// Represents CustomerId
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Represents timeStamp of registration
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }
}
