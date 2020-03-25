using JetBrains.Annotations;
using Lykke.Service.AdminManagement.Client.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.AdminManagement.Client.Models
{
    /// <summary>
    /// Represents result of password change attempt.
    /// </summary>
    [PublicAPI]
    public class ChangePasswordResponseModel
    {
        /// <summary>
        /// Holds any Error that might have happened during the execution
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ChangePasswordErrorCodes Error { get; set; }
    }
}