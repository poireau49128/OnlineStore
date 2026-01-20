namespace Store.Application.DTOs
{
    public sealed class CategoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public int ProductTypeId { get; init; }
        public string ProductTypeName { get; init; } = null!;
    }
}
