using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Lykke.Service.AdminManagement.Domain.Enums;
using Lykke.Service.AdminManagement.Domain.Services;

namespace Lykke.Service.AdminManagement.DomainServices
{
    public class AutofillValuesService : IAutofillValuesService
    {
        private readonly Dictionary<SuggestedValueType, List<string>> _autoFIllValues = new Dictionary<SuggestedValueType, List<string>>
        {
            {
                SuggestedValueType.Company,
                new List<string>
                {
                    "Properties",
                    "Hospitality",
                    "Malls",
                    "Hotels & Resorts",
                    "Retail",
                    "Community Management",
                    "Technologies",
                    "Industries and Investments",
                    "Finance",
                    "Investment Holdings"
                }
            },
            {
                SuggestedValueType.Department,
                new List<string>
                {
                    "Technology",
                    "Finance",
                    "Sales",
                    "HR",
                    "Operations"
                }
            }
        };

        public Task<ReadOnlyDictionary<SuggestedValueType, List<string>>> GetAllAsync()
        {
            return Task.FromResult(new ReadOnlyDictionary<SuggestedValueType, List<string>>(_autoFIllValues));
        }
    }
}
