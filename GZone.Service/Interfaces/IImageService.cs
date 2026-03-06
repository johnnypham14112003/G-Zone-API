using Microsoft.AspNetCore.Http;

namespace GZone.Service.Interfaces
{
    public interface IImageService
    {
        public Task<string> SaveImageAsync(IFormFile imageFile, string fileName, string category);
        public (Stream FileStream, string ContentType) GetImageFile(string relativePath);
        public bool DeleteImage(string relativePath);
    }
}
