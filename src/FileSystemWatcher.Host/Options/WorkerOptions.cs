namespace FileSystemWatcher.Host.Options
{
    public class WorkerOptions
    {
        public const string Worker = "Worker";

        public int HealthProbeTcpPort { get; set; }
        public int RepeatIntervalSeconds { get; set; }
    }
}