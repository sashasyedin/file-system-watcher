using System;

namespace FileSystemWatcher.Services.FileTransfer
{
    public class FileTransferFactory : IFileTransferFactory
    {
        private readonly Func<FileTransferBlobStorageService> _blobStorageService;

        public FileTransferFactory(Func<FileTransferBlobStorageService> blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public IFileTransferService Create(FileTransferMode fileTransferMode)
        {
            return fileTransferMode switch
            {
                FileTransferMode.BlobStorage => _blobStorageService(),
                _ => throw new InvalidOperationException(),
            };
        }
    }
}