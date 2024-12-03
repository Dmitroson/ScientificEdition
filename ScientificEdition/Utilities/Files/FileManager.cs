using Syncfusion.Pdf;

namespace ScientificEdition.Utilities.Files
{
    public class FileManager
    {
        private const string RootDirectory = "data";

        private readonly IWebHostEnvironment environment;

        public FileManager(IWebHostEnvironment environment)
            => this.environment = environment;

        public async Task<string> SaveFileAsync(IFormFile sourceFile, string fileName, string[] destinationDirectory)
        {
            var fullDirectoryPath = GetFullPath(destinationDirectory);

            var directory = new DirectoryInfo(fullDirectoryPath);
            if (!directory.Exists)
                directory.Create();

            var fullFilePath = Path.Combine(fullDirectoryPath, $"{fileName}{Path.GetExtension(sourceFile.FileName)}");
            using (var stream = new FileStream(fullFilePath, FileMode.Create))
                await sourceFile.CopyToAsync(stream);

            return fullFilePath;
        }

        public string SavePdfFile(PdfDocument pdfFile, string fileName, string[] destinationDirectory)
        {
            var fullDirectoryPath = GetFullPath(destinationDirectory);

            var directory = new DirectoryInfo(fullDirectoryPath);
            if (!directory.Exists)
                directory.Create();

            var fullFilePath = Path.Combine(fullDirectoryPath, $"{fileName}.pdf");
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                pdfFile.Save(fileStream);

            return fullFilePath;
        }

        public string GetFullPath(params string[] pathSegments)
        {
            var directoryPath = BuildRelatedPath(RootDirectory, pathSegments);
            var fullDirectoryPath = Path.Combine(environment.WebRootPath, directoryPath);
            return fullDirectoryPath;
        }

        private static string BuildRelatedPath(string rootFolderName, string[] pathSegments)
        {
            var path = rootFolderName;

            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var segment in pathSegments)
            {
                var validSegment = segment;
                foreach (var invalidChar in invalidChars)
                    validSegment = validSegment.Replace(invalidChar, '-');

                path = Path.Combine(path, validSegment);
            }

            return path;
        }

        public void DeleteDirectory(string[] pathSegments)
        {
            var path = GetFullPath(pathSegments);
            DeleteDirectory(path);
        }

        public void DeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch (Exception) { }
        }

        public void DeleteFiles(string directoryPath, string searchPattern)
        {
            var files = Directory.GetFiles(directoryPath, searchPattern);
            foreach (var filePath in files)
                DeleteFile(filePath);
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File path: {filePath}.");

            File.Delete(filePath);
        }
    }
}
