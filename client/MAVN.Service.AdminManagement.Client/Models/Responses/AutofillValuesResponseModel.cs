using System.Collections.Generic;

namespace MAVN.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents a list of autofill values to be presented to the admin.
    /// </summary>
    public class AutofillValuesResponseModel
    {
        /// <summary>
        /// A list of autofill values.
        /// </summary>
        public List<SuggestedValueMapping> Values { set; get; }
    }
}