using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSystemWatcher.Host.Options;

namespace FileSystemWatcher.Host
{
    public class AgentHealthCheck : IHealthCheck
    {
        private readonly FileWatcherOptions _fileWatcherOptions;

        public AgentHealthCheck(FileWatcherOptions fileWatcherOptions)
        {
            _fileWatcherOptions = fileWatcherOptions;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (!Directory.Exists(_fileWatcherOptions.DirectoryPath))
            {
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy));
            }

            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy));
        }
    }
}