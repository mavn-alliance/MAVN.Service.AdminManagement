using System.Collections.Generic;
using Lykke.Service.AdminManagement.Client.Models.Enums;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents a mapping for a suggested type
    /// </summary>
    public class SuggestedValueMapping
    {
        /// <summary>
        /// Type of information
        /// </summary>
        public SuggestedValueType Type { set; get; }
        
        /// <summary>
        /// Possible values
        /// </summary>
        public List<string> Values { set; get; }
    }
}