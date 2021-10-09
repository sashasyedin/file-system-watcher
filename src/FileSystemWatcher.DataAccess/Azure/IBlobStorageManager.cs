using System.IO;
using System.Threading.Tasks;

namespace FileSystemWatcher.DataAccess.Azure
{
    public interface IBlobStorageManager
    {
        Task UploadFile(string blobContainerUri, string fileName, Stream stream);
    }
}