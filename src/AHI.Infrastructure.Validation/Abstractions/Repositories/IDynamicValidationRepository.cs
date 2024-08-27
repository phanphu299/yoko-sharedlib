using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Validation.Model;

namespace AHI.Infrastructure.Validation.Repository.Abstraction
{
    public interface IDynamicValidationRepository
    {
        #region Methods

        Task<IEnumerable<PropertyValidationRule>> GetValidationRulesAsync(IEnumerable<string> validationKeys);

        #endregion
    }
}
