namespace FileSystemWatcher.Services.FileTransfer
{
    public class FileTransferBlobStorageParams : IFileTransferParams
    {
        public string BlobContainerUri { get; set; }
        public string FilePath { get; set; }
        public string FilePrefix { get; set; }
    }
}