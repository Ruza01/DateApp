using System;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;
    public PhotoService(IConfiguration config)  //u konstruktoru pravimo acc
    {
        var cloudName = config["CloudinarySettings:CloudName"];
        var apiKey = config["CloudinarySettings:ApiKey"];
        var apiSecret = config["CloudinarySettings:ApiSecret"];

        var acc = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(acc);
    }
    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();     //kreiramo prazan rezultat koji ce biti popunjen podacima o slici

        if (file.Length > 0)    //ako je tacno, imamo sliku
        {
            using var stream = file.OpenReadStream();   //stream za citanje fajla
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                    .Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "datigApp-net8"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams); //saljemo sliku na Cloudinary

        }

        return uploadResult;    //vracamo Result koji sadrzi ULR slike i public ID

    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
}
