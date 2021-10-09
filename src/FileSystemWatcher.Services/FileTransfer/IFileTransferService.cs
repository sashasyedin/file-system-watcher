using System.Threading.Tasks;

namespace FileSystemWatcher.Services.FileTransfer
{
    public interface IFileTransferService
    {
        Task Transfer(IFileTransferParams fileTransferParams);
    }
}