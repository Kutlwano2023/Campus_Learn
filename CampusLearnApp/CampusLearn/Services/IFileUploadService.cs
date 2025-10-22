using Microsoft.AspNetCore.Http;

namespace CampusLearn.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string uploadFolder);
        Task<bool> DeleteFileAsync(string filePath);
        string GetContentType(string fileName);
        bool IsValidFileType(IFormFile file);
        bool IsValidFileSize(IFormFile file);
    }
}