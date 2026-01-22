using Microsoft.EntityFrameworkCore;
using Store.Application. DTOs;
using Store.Application. Interfaces. Admin;
using Store.Application.Utilities;
using Store. Domain.Entities;
using System. Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace Store.Persistence.Services.Admin;

public sealed class ProductImageService :  IProductImageService
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _webHostEnv;

    private const string ImagesFolder = "img/products";
    private const string ThumbFolder = "img/products/thumb";
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
        if (! files.Any())
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

            var sanitizedFileName = SanitizeFileName(file.FileName);
            var fileName = sanitizedFileName;
            var relativePath = $"/{ImagesFolder}/{fileName}";
            var fullPath = Path. Combine(_webHostEnv.WebRootPath, ImagesFolder, fileName);

            // Сохранение оригинального изображения
            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.Content.CopyToAsync(stream);
            }

            // TODO: Создание миниатюры (требует ImageSharp или подобной библиотеки)

            // Добавление в БД
            variant.AddImage(relativePath, sortOrder);
            uploadedPaths.Add(relativePath);
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

        // Удаление файла с диска
        var originalPath = Path.Combine(_webHostEnv.WebRootPath, image.RelativePath. TrimStart('/'));
        if (File.Exists(originalPath))
            File.Delete(originalPath);

        // TODO: Удаление миниатюры

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
            if (image != null)
            {
                // TODO: Добавить SetSortOrder метод в ProductImage и реализовать через reflection если нужно
            }
        }

        await _db.SaveChangesAsync();
    }

    // ============ Private Methods ============

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
        var imagesFolderPath = Path. Combine(_webHostEnv.WebRootPath, ImagesFolder);
        var thumbFolderPath = Path.Combine(_webHostEnv.WebRootPath, ThumbFolder);

        if (!Directory.Exists(imagesFolderPath))
            Directory.CreateDirectory(imagesFolderPath);

        if (!Directory.Exists(thumbFolderPath))
            Directory.CreateDirectory(thumbFolderPath);
    }
}