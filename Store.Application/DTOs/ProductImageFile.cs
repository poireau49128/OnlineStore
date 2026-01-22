namespace Store.Application.DTOs;

public sealed class ProductImageFile
{
    public string FileName { get; set; } = null!;
    public Stream Content { get; set; } = null!;
    public long Size { get; set; }
    
    public ProductImageFile() { }

    public ProductImageFile(string fileName, Stream content, long size)
    {
        FileName = fileName;
        Content = content;
        Size = size;
    }
}