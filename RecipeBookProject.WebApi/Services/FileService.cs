using Microsoft.AspNetCore.Http;

namespace RecipeBookProject.WebApi.Services
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile file, string folderName = "images");
        Task<bool> DeleteImageAsync(string fileName, string folderName = "images");
        bool TestImageAccess(string fileName, string folderName = "images");
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadPath;
        private readonly IConfiguration _configuration;

        public FileService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            
            // WebRootPath zaten wwwroot klasörünü içeriyor, sadece uploads ekle
            _uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
            
            // Debug için path'leri logla
            Console.WriteLine($"WebRootPath: {_environment.WebRootPath}");
            Console.WriteLine($"ContentRootPath: {_environment.ContentRootPath}");
            Console.WriteLine($"FileService initialized with upload path: {_uploadPath}");
            
            // wwwroot klasörünün varlığını kontrol et
            if (string.IsNullOrEmpty(_environment.WebRootPath))
            {
                Console.WriteLine("WARNING: WebRootPath is null or empty!");
            }
            else if (!Directory.Exists(_environment.WebRootPath))
            {
                Console.WriteLine($"WARNING: WebRootPath directory does not exist: {_environment.WebRootPath}");
            }
            else
            {
                Console.WriteLine($"WebRootPath directory exists: {_environment.WebRootPath}");
                var wwwrootContents = Directory.GetDirectories(_environment.WebRootPath);
                Console.WriteLine($"WebRoot contents: {string.Join(", ", wwwrootContents)}");
            }
            
            // Uploads klasörünü oluştur
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
                Console.WriteLine($"Created upload folder: {_uploadPath}");
            }
            
            // Images alt klasörünü de oluştur
            var imagesPath = Path.Combine(_uploadPath, "images");
            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
                Console.WriteLine($"Created images folder: {imagesPath}");
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folderName = "images")
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Dosya bulunamadı.");

            // Dosya uzantısını kontrol et
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            
            // Dosya adı kontrolü
            if (string.IsNullOrEmpty(file.FileName))
            {
                Console.WriteLine($"ERROR: FileService - File name is null or empty");
                throw new ArgumentException("Dosya adı geçersiz.");
            }
            
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            Console.WriteLine($"DEBUG: FileService - Original filename: {file.FileName}");
            Console.WriteLine($"DEBUG: FileService - Extracted extension: '{fileExtension}'");
            
            // Uzantı kontrolü
            if (string.IsNullOrEmpty(fileExtension))
            {
                Console.WriteLine($"ERROR: FileService - No file extension found in: {file.FileName}");
                throw new ArgumentException("Dosya uzantısı bulunamadı.");
            }
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                Console.WriteLine($"ERROR: FileService - Invalid file extension: {fileExtension}");
                throw new ArgumentException($"Sadece şu dosya türleri kabul edilir: {string.Join(", ", allowedExtensions)}");
            }

            // Dosya boyutunu kontrol et (5MB)
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Dosya boyutu 5MB'dan büyük olamaz.");

            // Klasör yoksa oluştur
            var uploadFolder = Path.Combine(_uploadPath, folderName);
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
                Console.WriteLine($"Created upload folder: {uploadFolder}");
            }

            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            Console.WriteLine($"DEBUG: FileService - Original filename: {file.FileName}");
            Console.WriteLine($"DEBUG: FileService - File extension: {fileExtension}");
            Console.WriteLine($"DEBUG: FileService - Generated filename: {fileName}");
            Console.WriteLine($"DEBUG: FileService - Full file path: {filePath}");
            Console.WriteLine($"Saving file to: {filePath}");
            Console.WriteLine($"File size: {file.Length} bytes");

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Dosya kaydedildi mi kontrol et
            if (File.Exists(filePath))
            {
                Console.WriteLine($"File saved successfully: {filePath}");
                Console.WriteLine($"File size on disk: {new FileInfo(filePath).Length} bytes");
            }
            else
            {
                Console.WriteLine($"ERROR: File was not saved to: {filePath}");
            }

            // Base URL'i al
            var baseUrl = _configuration["BaseUrl"] ?? "https://localhost:7129";
            
            // Absolute URL oluştur
            var imageUrl = $"{baseUrl}/uploads/{folderName}/{fileName}";
            Console.WriteLine($"DEBUG: FileService - Base URL: {baseUrl}");
            Console.WriteLine($"DEBUG: FileService - Folder name: {folderName}");
            Console.WriteLine($"DEBUG: FileService - File name: {fileName}");
            Console.WriteLine($"DEBUG: FileService - Final image URL: {imageUrl}");
            Console.WriteLine($"Base URL: {baseUrl}");
            Console.WriteLine($"Returning absolute image URL: {imageUrl}");
            
            // Dosyanın web'den erişilebilir olup olmadığını kontrol et
            var webPath = Path.Combine(_environment.WebRootPath, "uploads", folderName, fileName);
            Console.WriteLine($"Web path for verification: {webPath}");
            Console.WriteLine($"File exists in web path: {File.Exists(webPath)}");
            
            return imageUrl;
        }

        public Task<bool> DeleteImageAsync(string fileName, string folderName = "images")
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                    return Task.FromResult(false);

                var filePath = Path.Combine(_uploadPath, folderName, Path.GetFileName(fileName));
                
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
                
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public bool TestImageAccess(string fileName, string folderName = "images")
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, folderName, fileName);
                var exists = File.Exists(filePath);
                
                Console.WriteLine($"DEBUG: TestImageAccess - fileName: {fileName}");
                Console.WriteLine($"DEBUG: TestImageAccess - filePath: {filePath}");
                Console.WriteLine($"DEBUG: TestImageAccess - exists: {exists}");
                
                if (exists)
                {
                    var fileInfo = new FileInfo(filePath);
                    Console.WriteLine($"DEBUG: TestImageAccess - file size: {fileInfo.Length} bytes");
                }
                
                return exists;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: TestImageAccess - Exception: {ex.Message}");
                return false;
            }
        }
    }
}

