
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using AHI.Infrastructure.Validation.CustomAttribute;
using AHI.Infrastructure.Validation.Model;

namespace AHI.Infrastructure.Validation.Abstraction
{
    public interface ITypeValidator
    {
        #region Methods

        Task<ValidationFailure[]> ValidateAsync(PropertyValidationContext context,
            CancellationToken cancellationToken = default);

        #endregion
    }
}