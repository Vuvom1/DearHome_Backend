using System;
using DearHome_Backend.Services.Interfaces;
using FirebaseAdmin.Auth;
using Firebase.Storage;

namespace DearHome_Backend.Services.Implementations;

public class FirebaseStorageService : IFirebaseStorageService
{
    private readonly string _bucket = "makemyhome-27df4.appspot.com";
    private readonly string _apiKey;

    public FirebaseStorageService(IConfiguration configuration)
    {
        _bucket = configuration["Firebase:StorageBucket"] ?? _bucket;
        _apiKey = configuration["Firebase:ApiKey"] ?? string.Empty;
    }

    public async Task<string> UploadImageAsync(IFormFile imageFile)
    {
        try
        {
            using var stream = imageFile.OpenReadStream();
            var fileName = Path.GetRandomFileName();

            // Create storage reference with proper authentication
            var storage = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => GetFirebaseToken(),
                    ThrowOnCancel = true
                });

            var task = storage
                .Child("Images")
                .Child(fileName)
                .PutAsync(stream);

            var downloadUrl = await task;
            return downloadUrl;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error uploading image to Firebase Storage: {ex.Message}", ex);
        }
    }

    public async Task<string> UpdateImageAsync(IFormFile imageFile, string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        // Delete the existing image
        var storage = new FirebaseStorage(
            _bucket,
            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = async () => await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(Guid.NewGuid().ToString()),
                ThrowOnCancel = true
            });

        await storage
            .Child("Images")
            .Child(fileName)
            .DeleteAsync();

        var stream = imageFile.OpenReadStream();
        var newTask = storage
            .Child("Images")
            .Child(fileName)
            .PutAsync(stream);

        var newDownloadUrl = await newTask;
        return newDownloadUrl;
    }

    public async Task DeleteImageAsync(string imageUrl)
    {
        var uri = new Uri(imageUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        var storage = new FirebaseStorage(
            _bucket,
            new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = async () => await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(Guid.NewGuid().ToString()),
                ThrowOnCancel = true
            });

        await storage
            .Child("Images")
            .Child(fileName)
            .DeleteAsync();
    }
    
    private async Task<string> GetFirebaseToken()
    {
        try
        {
            // Using a service account with proper permissions
            return await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync("firebase-storage-admin");
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Error generating Firebase token: {ex.Message}", ex);
        }
    }
}
