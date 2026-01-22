using System.ComponentModel.DataAnnotations;

namespace Store.Application. DTOs.Admin;

public class UpdateVariantRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int VariantId { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Цена должна быть от 0 до 999999.99")]
    public decimal? OverridePrice { get; set; }
}