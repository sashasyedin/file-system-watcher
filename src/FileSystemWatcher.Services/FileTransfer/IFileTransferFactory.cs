namespace FileSystemWatcher.Services.FileTransfer
{
    public interface IFileTransferFactory
    {
        IFileTransferService Create(FileTransferMode fileTransferMode);
    }
}