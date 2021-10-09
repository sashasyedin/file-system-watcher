using System;

namespace FileSystemWatcher.Host.Options
{
    public class FileWatcherOptions
    {
        public const string FileWatcher = "FileWatcher";

        public string DirectoryPath { get; set; }
        public string Filter { get; set; }
        public TimeSpan RetryInterval { get; set; }
        public int MaxAttemptCount { get; set; }
    }
}