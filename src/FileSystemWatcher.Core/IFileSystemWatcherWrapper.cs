using System.IO;

namespace FileSystemWatcher.Core
{
    public interface IFileSystemWatcherWrapper
    {
        event FileSystemEventHandler Created;
        bool EnableRaisingEvents { get; set; }
    }
}