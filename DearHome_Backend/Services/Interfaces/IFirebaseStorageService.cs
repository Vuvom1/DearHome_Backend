using System;

namespace DearHome_Backend.Services.Interfaces;

public interface IFirebaseStorageService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UpdateImageAsync(IFormFile file, string imageUrl);
    Task DeleteImageAsync(string imageUrl);
}
