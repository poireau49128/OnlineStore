using System. ComponentModel.DataAnnotations;

namespace Store.Application.DTOs. Admin;

public class UpdateProductRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Название товара обязательно")]
    [StringLength(200, MinimumLength = 3, 
        ErrorMessage = "Название должно быть от 3 до 200 символов")]
    public string Name { get; set; } = null! ;

    [StringLength(2000, ErrorMessage = "Описание не должно превышать 2000 символов")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Цена обязательна")]
    [Range(0.01, 999999.99, ErrorMessage = "Цена должна быть от 0.01 до 999999.99")]
    public decimal BasePrice { get; set; }

    [StringLength(50, ErrorMessage = "SKU не должен превышать 50 символов")]
    public string?  Sku { get; set; }

    [Required(ErrorMessage = "Категория обязательна")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
    public int CategoryId { get; set; }
}