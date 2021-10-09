using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FileSystemWatcher.Host.Options;

namespace FileSystemWatcher.Host
{
    public class TcpHealthProbe : BackgroundService
    {
        private const int PeriodSeconds = 5;

        private readonly ILogger<TcpHealthProbe> _logger;
        private readonly HealthCheckService _healthCheckService;
        private readonly TcpListener _listener;

        public TcpHealthProbe(
            ILogger<TcpHealthProbe> logger,
            WorkerOptions workerOptions,
            HealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            _logger = logger;
            _listener = new TcpListener(IPAddress.Any, workerOptions.HealthProbeTcpPort);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Started health check service");

            await Task.Yield();

            _listener.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateHeartbeatAsync(stoppingToken);
                Thread.Sleep(TimeSpan.FromSeconds(PeriodSeconds));
            }

            _listener.Stop();
        }

        private async Task UpdateHeartbeatAsync(CancellationToken token)
        {
            try
            {
                var result = await _healthCheckService.CheckHealthAsync(token);
                var isHealthy = result.Status == HealthStatus.Healthy;

                if (!isHealthy)
                {
                    _listener.Stop();
                    _logger.LogInformation("Service is unhealthy, listener stopped");
                    return;
                }

                _listener.Start();

                while (_listener.Server.IsBound && _listener.Pending())
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    client.Close();
                    _logger.LogInformation("Successfully processed health check request");
                }

                _logger.LogDebug("Heartbeat check executed");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while checking heartbeat");
            }
        }
    }
}