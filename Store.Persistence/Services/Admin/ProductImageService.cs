using Microsoft.EntityFrameworkCore;
using Store.Application. DTOs;
using Store.Application. Interfaces. Admin;
using Store.Application.Utilities;
using Store. Domain.Entities;
using System. Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;

namespace Store.Persistence.Services.Admin;

public sealed class ProductImageService :  IProductImageService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _webHostEnv;

    private const string OriginalFolder = "img/products/variant";
    private const string ThumbFolder = "img/products/thumb";
    private const int ThumbSize = 400;
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public ProductImageService(AppDbContext db, IWebHostEnvironment webHostEnv)
    {
        _db = db;
        _webHostEnv = webHostEnv;
    }

    public async Task<List<string>> UploadVariantImagesAsync(
        int productVariantId,
        List<ProductImageFile> files,
        int startingSortOrder = 0)
    {
        if (!files.Any())
            return new();

        var variant = await _db.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == productVariantId);

        if (variant == null)
            throw new InvalidOperationException("Вариант не найден");

        var uploadedPaths = new List<string>();
        var sortOrder = startingSortOrder;

        EnsureFoldersExist();

        foreach (var file in files)
        {
            ValidateFile(file);

            var baseName = SanitizeFileName(
                Path.GetFileNameWithoutExtension(file.FileName)
            );

            var fileName = baseName + ".webp";

            var originalFullPath = Path.Combine(
                _webHostEnv.WebRootPath,
                OriginalFolder,
                fileName
            );
            if (File.Exists(originalFullPath))
            {
                fileName = $"{baseName}-{productVariantId}.webp";
                originalFullPath = Path.Combine(
                    _webHostEnv.WebRootPath,
                    OriginalFolder,
                    fileName
                );
            }
            var thumbFullPath = Path.Combine(
                _webHostEnv.WebRootPath,
                ThumbFolder,
                fileName
            );

            using var image = await Image.LoadAsync(file.Content);
            await image.SaveAsync(
                originalFullPath,
                new WebpEncoder
                {
                    Quality = 90,
                    Method = WebpEncodingMethod.BestQuality
                }
            );

            var relativeOriginalPath = $"/{OriginalFolder}/{fileName}";

            await CreateThumbnailAsync(originalFullPath, thumbFullPath);

            variant.AddImage(relativeOriginalPath, sortOrder);
            uploadedPaths.Add(relativeOriginalPath);

            sortOrder++;
        }

        await _db.SaveChangesAsync();
        return uploadedPaths;
    }

    public async Task DeleteImageAsync(int imageId)
    {
        var image = await _db.ProductImages. FirstOrDefaultAsync(i => i.Id == imageId);
        if (image == null)
            throw new InvalidOperationException("Изображение не найдено");

        var originalPath = Path.Combine(_webHostEnv.WebRootPath, image.RelativePath.TrimStart('/'));

        var thumbPath = originalPath
            .Replace("/variant/", "/thumb/")
            .Replace("\\variant\\", "\\thumb\\");

        if (File.Exists(originalPath))
            File.Delete(originalPath);

        if (File.Exists(thumbPath))
            File.Delete(thumbPath);

        _db.ProductImages.Remove(image);
        await _db.SaveChangesAsync();
    }

    public async Task ReorderImagesAsync(List<(int ImageId, int NewSortOrder)> orders)
    {
        var imageIds = orders.Select(o => o.ImageId).ToList();
        var images = await _db.ProductImages
            .Where(i => imageIds.Contains(i.Id))
            .ToListAsync();

        foreach (var (imageId, newSortOrder) in orders)
        {
            var image = images.FirstOrDefault(i => i.Id == imageId);
            image?.SetSortOrder(newSortOrder);
        }

        await _db.SaveChangesAsync();
    }

    private void ValidateFile(ProductImageFile file)
    {
        if (file.Size > MaxFileSize)
            throw new InvalidOperationException(
                $"Файл '{file.FileName}' слишком большой (максимум 5 МБ)");

        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"Файл '{file.FileName}' имеет недопустимый формат.  Допускаются: {string.Join(", ", AllowedExtensions)}");
    }

    private string SanitizeFileName(string fileName)
    {
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        var transliterated = TransliterationHelper.Transliterate(nameWithoutExtension);

        var sanitized = Regex.Replace(transliterated, @"[^a-z0-9]+", "-", RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"-+", "-");
        sanitized = sanitized.Trim('-');

        var extension = Path.GetExtension(fileName);
        return $"{sanitized}{extension}";
    }

    private void EnsureFoldersExist()
    {
        var imagesFolderPath = Path. Combine(_webHostEnv.WebRootPath, OriginalFolder);
        var thumbFolderPath = Path.Combine(_webHostEnv.WebRootPath, ThumbFolder);

        if (!Directory.Exists(imagesFolderPath))
            Directory.CreateDirectory(imagesFolderPath);

        if (!Directory.Exists(thumbFolderPath))
            Directory.CreateDirectory(thumbFolderPath);
    }

    private async Task CreateThumbnailAsync(
        string originalFullPath,
        string thumbFullPath)
    {
        using var image = await Image.LoadAsync(originalFullPath);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(ThumbSize, ThumbSize),
            Mode = ResizeMode.Max,
            Position = AnchorPositionMode.Center
        }));

        await image.SaveAsync(
            thumbFullPath,
            new WebpEncoder { Quality = 80 }
        );
    }

}