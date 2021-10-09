using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileSystemWatcher.DataAccess.Azure
{
    public class BlobStorageManager : IBlobStorageManager
    {
        private readonly ILogger<BlobStorageManager> _logger;

        public BlobStorageManager(ILogger<BlobStorageManager> logger)
        {
            _logger = logger;
        }

        public async Task UploadFile(string blobContainerUri, string fileName, Stream stream)
        {
            try
            {
                var uri = new Uri(blobContainerUri);
                var container = new BlobContainerClient(uri);
                var blob = container.GetBlobClient(fileName);
                await blob.UploadAsync(stream, overwrite: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload to Blob Storage failed");
                throw;
            }
        }
    }
}