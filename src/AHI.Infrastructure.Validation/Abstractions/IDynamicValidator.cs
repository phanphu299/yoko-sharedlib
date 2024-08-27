
using System.Threading;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Validation.Abstraction
{
    public interface IDynamicValidator
    {
        /// <summary>
        /// Do dynamic validation asynchronously.
        /// </summary>
        /// <exception cref="EntityValidationException">Throw if validation failed.</exception>
        /// <param name="requestObject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ValidateAsync(object requestObject, CancellationToken cancellationToken = default);
    }
}