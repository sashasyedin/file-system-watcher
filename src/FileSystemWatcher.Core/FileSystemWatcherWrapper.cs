using System.IO;

namespace FileSystemWatcher.Core
{
    public class FileSystemWatcherWrapper : IFileSystemWatcherWrapper
    {
        private readonly System.IO.FileSystemWatcher _watcher;

        public event FileSystemEventHandler Created;

        public FileSystemWatcherWrapper(System.IO.FileSystemWatcher watcher)
        {
            _watcher = watcher;
            _watcher.Created += (sender, args) => Created?.Invoke(sender, args);
        }

        public bool EnableRaisingEvents
        {
            get { return _watcher.EnableRaisingEvents; }
            set { _watcher.EnableRaisingEvents = value; }
        }
    }
}