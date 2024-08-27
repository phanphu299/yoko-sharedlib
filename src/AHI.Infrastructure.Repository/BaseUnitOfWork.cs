using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using AHI.Infrastructure.Repository.Generic;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.SharedKernel.Helper;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Polly;
using Microsoft.Extensions.Logging;

namespace AHI.Infrastructure.Repository
{
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected Microsoft.EntityFrameworkCore.DbContext Context { get; private set; }
        private IDbContextTransaction _transaction;
        public BaseUnitOfWork(Microsoft.EntityFrameworkCore.DbContext context)
        {
            Context = context;
        }
        public async Task BeginTransactionAsync()
        {
            _transaction = await Context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            Context.SaveChanges();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }

        public async Task CommitWithTimeoutRetryAsync(IConfiguration configuration, IBaseLoggerAdapter logger = null)
        {
            var retryPolicy = PollyPolicyHelpers.GetDbTimeoutRetryAsyncPolicy(configuration, logger);
            var circuitBreakerPolicy = PollyPolicyHelpers.GetDbTimeoutCircuitBreakerAsyncPolicy(configuration);

            var strategy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
            await strategy.ExecuteAsync(async () => await CommitAsync());
        }

        public async Task CommitWithTimeoutRetryAsync(int maxRetries = 3, int delayInSeconds = 1, IBaseLoggerAdapter logger = null)
        {
            var retryPolicy = PollyPolicyHelpers.GetDbTimeoutRetryAsyncPolicy(maxRetries, delayInSeconds, logger);
            var circuitBreakerPolicy = PollyPolicyHelpers.GetDbTimeoutCircuitBreakerAsyncPolicy(maxRetries, delayInSeconds);

            var strategy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
            await strategy.ExecuteAsync(async () => await CommitAsync());
        }

        public async Task CommitWithTimeoutRetryAsync(IConfiguration configuration, ILogger logger = null)
        {
            var retryPolicy = PollyPolicyHelpers.GetDbTimeoutRetryAsyncPolicy(configuration, logger);
            var circuitBreakerPolicy = PollyPolicyHelpers.GetDbTimeoutCircuitBreakerAsyncPolicy(configuration);

            var strategy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
            await strategy.ExecuteAsync(async () => await CommitAsync());
        }

        public async Task CommitWithTimeoutRetryAsync(int maxRetries = 3, int delayInSeconds = 1, ILogger logger = null)
        {
            var retryPolicy = PollyPolicyHelpers.GetDbTimeoutRetryAsyncPolicy(maxRetries, delayInSeconds, logger);
            var circuitBreakerPolicy = PollyPolicyHelpers.GetDbTimeoutCircuitBreakerAsyncPolicy(maxRetries, delayInSeconds);

            var strategy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
            await strategy.ExecuteAsync(async () => await CommitAsync());
        }
    }
}
