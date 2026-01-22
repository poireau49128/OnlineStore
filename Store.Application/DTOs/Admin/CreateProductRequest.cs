using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Admin;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Название товара обязательно")]
    [StringLength(200, MinimumLength = 3, 
        ErrorMessage = "Название должно быть от 3 до 200 символов")]
    public string Name { get; set; } = null!;

    [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
    public string?  Description { get; set; }

    [Required(ErrorMessage = "Цена обязательна")]
    [Range(0.01, 999999.99, ErrorMessage = "Цена должна быть от 0.01 до 999999.99")]
    public decimal BasePrice { get; set; }

    [StringLength(50, ErrorMessage = "SKU не должен превышать 50 символов")]
    public string? Sku { get; set; }

    [Required(ErrorMessage = "Категория обязательна")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Цвет первого варианта обязателен")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Цвет должен быть от 2 до 50 символов")]
    public string FirstVariantColor { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Размер не должен превышать 50 символов")]
    public string? FirstVariantSize { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Цена варианта должна быть от 0 до 999999.99")]
    public decimal? FirstVariantPrice { get; set; }
    public List<ProductImageFile>? FirstVariantImages { get; set; }
}