using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FileSystemWatcher.Host.Options;
using FileSystemWatcher.DataAccess.Azure;
using FileSystemWatcher.Services.FileTransfer;

namespace FileSystemWatcher.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSystemd()
                .UseWindowsService()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(GetBasePath());
                    config.AddJsonFile("appsettings.json", false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    var blobStorageOptions = configuration.GetSection(BlobStorageOptions.BlobStorage).Get<BlobStorageOptions>();
                    var fileWatcherOptions = configuration.GetSection(FileWatcherOptions.FileWatcher).Get<FileWatcherOptions>();
                    var workerOptions = configuration.GetSection(WorkerOptions.Worker).Get<WorkerOptions>();
                    services.AddSingleton(blobStorageOptions);
                    services.AddSingleton(fileWatcherOptions);
                    services.AddSingleton(workerOptions);

                    services.AddTransient<IBlobStorageManager, BlobStorageManager>();
                    services.AddTransient<IFileTransferFactory, FileTransferFactory>();
                    services.AddTransient<FileTransferBlobStorageService>();
                    services.AddSingleton<Func<FileTransferBlobStorageService>>(sp => sp.GetService<FileTransferBlobStorageService>);

                    services.AddHostedService<Worker>();
                    services.AddApplicationInsightsTelemetryWorkerService();

                    services.AddHealthChecks().AddCheck<AgentHealthCheck>("agent_hc");
                    services.AddHostedService<TcpHealthProbe>();
                });

        private static string GetBasePath()
        {
            using var processModule = Process.GetCurrentProcess().MainModule;
            return Path.GetDirectoryName(processModule?.FileName);
        }
    }
}