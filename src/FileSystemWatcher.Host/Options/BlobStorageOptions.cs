namespace FileSystemWatcher.Host.Options
{
    public class BlobStorageOptions
    {
        public const string BlobStorage = "BlobStorage";

        public string FilePrefix { get; set; }
        public string Uri { get; set; }
    }
}