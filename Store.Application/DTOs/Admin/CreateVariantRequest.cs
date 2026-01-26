using System.ComponentModel.DataAnnotations;

namespace Store.Application.DTOs.Admin;

public class CreateVariantRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Цвет обязателен")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Цвет должен быть от 2 до 50 символов")]
    public string Color { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Размер не должен превышать 50 символов")]
    public string? Size { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Цена должна быть от 0 до 999999.99")]
    public decimal?  OverridePrice { get; set; } // null = base product price

    public List<ProductImageFile>? Images { get; set; }
    public bool isActive { get; set; }
}