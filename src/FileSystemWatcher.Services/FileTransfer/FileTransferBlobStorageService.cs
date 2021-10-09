using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using FileSystemWatcher.DataAccess.Azure;
using FileSystemWatcher.Utils.Extensions;

namespace FileSystemWatcher.Services.FileTransfer
{
    public class FileTransferBlobStorageService : IFileTransferService
    {
        private readonly ILogger<FileTransferBlobStorageService> _logger;
        private readonly IBlobStorageManager _storageManager;

        public FileTransferBlobStorageService(ILogger<FileTransferBlobStorageService> logger, IBlobStorageManager storageManager)
        {
            _logger = logger;
            _storageManager = storageManager;
        }

        public async Task Transfer(IFileTransferParams fileTransferParams)
        {
            var blobStorageParams = fileTransferParams as FileTransferBlobStorageParams;
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(blobStorageParams.FilePath);
            using var memoryStream = new MemoryStream();

            _logger.LogInformation("Compressing the file");

            await ZipExtensions.CompressFile(memoryStream, blobStorageParams.FilePath);

            _logger.LogInformation("Transferring the file to Blob Storage");

            var archiveFileName = $"{blobStorageParams.FilePrefix}{fileNameWithoutExt}.zip";
            await _storageManager.UploadFile(blobStorageParams.BlobContainerUri, archiveFileName, memoryStream);
        }
    }
}