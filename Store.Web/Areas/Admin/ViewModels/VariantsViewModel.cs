using System.ComponentModel.DataAnnotations;
using Store.Application.DTOs. Admin;

namespace Store.Web.Areas.Admin.ViewModels;

public sealed class VariantsViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal BasePrice { get; set; }

    public List<VariantItemViewModel> Variants { get; set; } = new();

    // Форма для добавления нового варианта
    public CreateVariantFormViewModel CreateForm { get; set; } = new();
}

public sealed class VariantItemViewModel
{
    public int Id { get; set; }
    public string Color { get; set; } = null!;
    public string?  Size { get; set; }
    public decimal?  OverridePrice { get; set; }
    public decimal ActualPrice { get; set; }
    public List<VariantImageViewModel> Images { get; set; } = new();
    public int TotalStock { get; set; }
    public bool isActive { get; set; }
}

public sealed class VariantImageViewModel
{
    public int Id { get; set; }
    public string RelativePath { get; set; } = null!;
}


public sealed class CreateVariantFormViewModel
{
    [Required(ErrorMessage = "Цвет обязателен")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Цвет должен быть от 2 до 50 символов")]
    public string Color { get; set; } = null!;

    [StringLength(50, ErrorMessage = "Размер не должен превышать 50 символов")]
    public string? Size { get; set; }

    [Range(0, 999999.99, ErrorMessage = "Цена должна быть от 0 до 999999.99")]
    public decimal? OverridePrice { get; set; }

    public List<IFormFile>? Images { get; set; }
    // if using to update
    public List<int>? ImagesToDelete { get; set; } 

    public bool IsActive { get; set; }
}