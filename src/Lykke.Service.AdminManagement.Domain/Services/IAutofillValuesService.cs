using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Lykke.Service.AdminManagement.Domain.Enums;

namespace Lykke.Service.AdminManagement.Domain.Services
{
    public interface IAutofillValuesService
    {
        Task<ReadOnlyDictionary<SuggestedValueType, List<string>>> GetAllAsync();
    }
}
