using System. ComponentModel.DataAnnotations;

namespace Store.Application.DTOs. Admin;

public class UpdateStockRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductVariantId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int WarehouseId { get; set; }

    [Required]
    [Range(0, 999999, ErrorMessage = "Количество должно быть от 0 до 999999")]
    public int Quantity { get; set; }
}