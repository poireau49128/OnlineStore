namespace Store.Application.DTOs.Admin;

public sealed class CreateCategoryDto
{
    public string Name { get; set; } = null!;
    public int? ProductTypeId { get; set; }
    public string? NewProductTypeName { get; set; }
}

public sealed class CreateCategoryResultDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ProductTypeName { get; set; } = null!;
}