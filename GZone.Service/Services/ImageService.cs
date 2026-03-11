using GZone.Repository.Base;
using GZone.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Text.RegularExpressions;

namespace GZone.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly long _imageMaxSizeByte;
        private readonly int _compressionQuality;
        private readonly string[] _allowedImageExtensions;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly string _privateStoragePath;

        public ImageService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IContentTypeProvider contentTypeProvider)
        {
            _unitOfWork = unitOfWork;
            //Convert from MB -> Mb -> b
            _imageMaxSizeByte = (long.TryParse(Environment.GetEnvironmentVariable("Image_Max_Size_MB"), out var MB)
                ? MB : 2) * 1024 * 1024;
            _compressionQuality = int.TryParse(Environment.GetEnvironmentVariable("Image_Compressiom_Quality_Percent"), out var percent) ? percent : 75;

            var rawExtensions = Environment.GetEnvironmentVariable("Image_Type") ?? "jpg,png";
            // Xử lý chuỗi này để đảm bảo mọi phần tử đều có dấu "."
            _allowedImageExtensions = Regex.Split(rawExtensions, ",")
                .Select(ext => ext.Trim().ToLowerInvariant()) // 1. Dọn dẹp: cắt khoảng trắng, chuyển chữ thường
                .Where(ext => !string.IsNullOrWhiteSpace(ext)) // 2. Loại bỏ các chuỗi rỗng (nếu ENV là "png,,jpg")
                .Select(ext => ext.StartsWith(".") ? ext : "." + ext) // 3. Thêm dấu "." nếu chưa có
                .ToArray();
            _contentTypeProvider = contentTypeProvider;

            // Get folder publish
            var contentRoot = webHostEnvironment.ContentRootPath;// Example "C:\inetpub\wwwroot\YourApiApp"

            var storageDirectory = Path.Combine(contentRoot, "..", "GZone_PrivateStorage");// Example "C:\inetpub\wwwroot\GZone_PrivateStorage"

            _privateStoragePath = Path.GetFullPath(storageDirectory);// Example "C:\inetpub\wwwroot\GZone_PrivateStorage"
            // Make sure folder exist
            if (!Directory.Exists(_privateStoragePath))
                Directory.CreateDirectory(_privateStoragePath);
        }

        //====================================================================================
        public async Task<string> SaveImageAsync(IFormFile imageFile, string fileName, string category)
        {
            // --- Validate input ---
            if (imageFile == null || imageFile.Length == 0)
                throw new Exception("Image null");

            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(category))
                throw new Exception("Name or Category null");
            // ----------------------

            var relativePath = Path.Combine("images", category.ToLower() + "s");
            var uploadPath = Path.Combine(_privateStoragePath, relativePath);

            // --- Validate file type ---
            if (_allowedImageExtensions.Length == 0 || _allowedImageExtensions.Any(arr => string.IsNullOrWhiteSpace(arr)))
                throw new Exception("Image types allow is null");

            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(fileExtension) || !_allowedImageExtensions.Contains(fileExtension))
                throw new Exception("Image type not allow");
            // --------------------------

            // Handle physic location
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            fileName = Path.GetFileNameWithoutExtension(fileName);
            var safeFileName = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(safeFileName) || safeFileName != fileName)
                throw new Exception("File name contain invalid character");

            string finalFileName;
            string physicalFilePath;

            // Handle Size
            if (imageFile.Length <= _imageMaxSizeByte)// Valid Size -> Override if exist
            {
                finalFileName = $"{fileName}{fileExtension}";
                physicalFilePath = Path.Combine(uploadPath, finalFileName);

                using (var fileStream = new FileStream(physicalFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            else// File size larger than allow
            {
                // Use .jpg for make sure compress size
                finalFileName = $"{fileName}.jpg";
                physicalFilePath = Path.Combine(uploadPath, finalFileName);

                // use ImageSharp for compress
                using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
                {
                    // (Tùy chọn) có thể giới hạn kích thước tối đa ở đây nếu muốn
                    // image.Mutate(x => x.Resize(new ResizeOptions
                    // {
                    //     Mode = ResizeMode.Max,
                    //     Size = new Size(1920, 1080) // Ví dụ: giới hạn tối đa 1920x1080
                    // }));

                    var encoder = new JpegEncoder
                    {
                        Quality = _compressionQuality
                    };

                    using (var fileStream = new FileStream(physicalFilePath, FileMode.Create))
                    {
                        await image.SaveAsJpegAsync(fileStream, encoder);
                    }
                }
            }
            // if return path error: $"{relativePath}/{finalFileName}";
            return Path.Combine(relativePath, finalFileName);
        }

        public (Stream FileStream, string ContentType) GetImageFile(string relativePath)
        {
            // relativePath VD: /images/avatars/file.png
            var physicalFilePath = Path.Combine(_privateStoragePath, relativePath);

            // Find file
            if (!System.IO.File.Exists(physicalFilePath))
                throw new ArgumentException("Tên file không hợp lệ.");

            // Get ContentType (MIME type)
            if (!_contentTypeProvider.TryGetContentType(physicalFilePath, out var contentType))
            {
                // Default binary stream type
                contentType = "application/octet-stream";
            }

            // Create file stream
            Stream fileStream = new FileStream(physicalFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return (fileStream, contentType);
        }

        public bool DeleteImage(string relativePath)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Đường dẫn file không được để trống.");

            // Tạo đường dẫn vật lý tương tự như hàm GetImageFile
            var physicalFilePath = Path.Combine(_privateStoragePath, relativePath);

            // Kiểm tra xem file có tồn tại không trước khi thực hiện lệnh xóa
            if (System.IO.File.Exists(physicalFilePath))
            {
                System.IO.File.Delete(physicalFilePath);
                return true; // Trả về true nếu xóa thành công
            }

            return false; // Trả về false nếu file không tồn tại
        }
    }
}
