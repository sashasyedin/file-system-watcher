using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using FileSystemWatcher.Host.Options;

namespace FileSystemWatcher.Host
{
    public abstract class WorkerBase : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly WorkerOptions _workerOptions;

        public WorkerBase(ILogger logger, WorkerOptions workerOptions)
        {
            _logger = logger;
            _workerOptions = workerOptions;

            WorkerName = GetType().Name;
        }

        public string WorkerName { get; }

        public abstract Task<bool> DoWork();

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service startup");

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(10, stoppingToken).ConfigureAwait(false);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        _logger.LogInformation("Calling DoWorkAsync");

                        var done = await DoWork().ConfigureAwait(false);
                        if (done) break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Unhandled exception occurred in the {worker}. Sending an alert. Worker will retry after the normal interveral.",
                            WorkerName);
                    }

                    await Task.Delay(_workerOptions.RepeatIntervalSeconds * 1000, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation(
                    "Execution ended. Cancelation token cancelled = {IsCancellationRequested}",
                    stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning(ex, "Execution Cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandeled exception. Execution Stopping");
            }
        }
    }
}