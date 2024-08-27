using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AHI.Infrastructure.Repository.Generic
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        Task CommitWithTimeoutRetryAsync(IConfiguration configuration, IBaseLoggerAdapter logger = null);
        Task CommitWithTimeoutRetryAsync(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null);
        Task CommitWithTimeoutRetryAsync(IConfiguration configuration, ILogger logger = null);
        Task CommitWithTimeoutRetryAsync(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null);
        Task RollbackAsync();
        Task BeginTransactionAsync();
    }
}
