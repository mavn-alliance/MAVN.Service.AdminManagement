using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MAVN.Service.AdminManagement.Domain.Enums;

namespace MAVN.Service.AdminManagement.Domain.Services
{
    public interface IAutofillValuesService
    {
        Task<ReadOnlyDictionary<SuggestedValueType, List<string>>> GetAllAsync();
    }
}
