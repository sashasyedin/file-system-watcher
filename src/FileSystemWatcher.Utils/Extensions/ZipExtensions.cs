using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace FileSystemWatcher.Utils.Extensions
{
    public static class ZipExtensions
    {
        public static async Task CompressFile(this Stream stream, string filePath)
        {
            var fileName = Path.GetFileName(filePath);

            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry(fileName);
                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(filePath);
                await fileStream.CopyToAsync(entryStream);
            }

            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}