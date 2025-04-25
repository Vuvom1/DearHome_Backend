using System;
using DearHome_Backend.Services.Interfaces;
using FirebaseAdmin.Auth;
using Firebase.Storage;

namespace DearHome_Backend.Services.Implementations;

public class FirebaseStorageService : IFirebaseStorageService
{
    private static readonly string _bucket = "makemyhome-27df4.appspot.com";

    public FirebaseStorageService()
    {
        
    }
    public async Task<string> UploadImageAsync(IFormFile imageFile)
    {
        var stream = imageFile.OpenReadStream();
            var fileName = Path.GetRandomFileName();

            var uid = Guid.NewGuid().ToString();
            var customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(uid);

            var storage = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(customToken),
                    ThrowOnCancel = true
                });

            var task = storage
                .Child("Images")
                .Child(fileName)
                .PutAsync(stream);

            var downloadUrl = await task;
            return downloadUrl;
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
}
