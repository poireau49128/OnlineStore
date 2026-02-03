using Store.Application.DTOs;

public sealed class ProductTypeWithCategoriesDto
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public List<CategoryDto> Categories { get; init; } = new();
}
