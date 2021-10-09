using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using FileSystemWatcher.Host.Options;
using FileSystemWatcher.Core;
using FileSystemWatcher.Services.FileTransfer;

namespace FileSystemWatcher.Host
{
    public class Worker : WorkerBase
    {
        private readonly ILogger<Worker> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly BlobStorageOptions _blobStorageOptions;
        private readonly FileWatcherOptions _fileWatcherOptions;
        private readonly IFileTransferFactory _fileTransferFactory;
        private readonly IFileSystemWatcherWrapper _fileSystemWatcherWrapper;

        public Worker(
            ILogger<Worker> logger,
            TelemetryClient telemetryClient,
            BlobStorageOptions blobStorageOptions,
            FileWatcherOptions fileWatcherOptions,
            WorkerOptions workerOptions,
            IFileTransferFactory fileTransferFactory)
            : base(logger, workerOptions)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _blobStorageOptions = blobStorageOptions;
            _fileWatcherOptions = fileWatcherOptions;
            _fileTransferFactory = fileTransferFactory;

            _fileSystemWatcherWrapper = new FileSystemWatcherWrapper(
                new System.IO.FileSystemWatcher
                {
                    Path = fileWatcherOptions.DirectoryPath,
                    Filter = fileWatcherOptions.Filter,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size | NotifyFilters.Attributes,
                    IncludeSubdirectories = false
                });
        }

        public override Task<bool> DoWork()
        {
            _fileSystemWatcherWrapper.Created += FileWatcherCreatedHandler;
            _fileSystemWatcherWrapper.EnableRaisingEvents = true;

            return Task.FromResult(true);
        }

        private async void FileWatcherCreatedHandler(object sender, FileSystemEventArgs e)
        {
            var retryInterval = _fileWatcherOptions.RetryInterval;
            var attemptCount = _fileWatcherOptions.MaxAttemptCount;

            using (_telemetryClient.StartOperation<RequestTelemetry>("FileTransferTriggered"))
            {
                while (true)
                {
                    try
                    {
                        var fileTransferService = _fileTransferFactory.Create(FileTransferMode.BlobStorage);

                        var fileTransferParams = new FileTransferBlobStorageParams
                        {
                            FilePath = e.FullPath,
                            FilePrefix = _blobStorageOptions.FilePrefix,
                            BlobContainerUri = _blobStorageOptions.Uri
                        };
                        
                        await fileTransferService.Transfer(fileTransferParams);
                        File.Delete(e.FullPath);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Something went wrong");

                        if (--attemptCount == 0) throw;
                        await Task.Delay(retryInterval);
                    }
                }

                _telemetryClient.TrackEvent("Event completed");
            }
        }
    }
}