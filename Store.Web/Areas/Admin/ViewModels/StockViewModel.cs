using System.ComponentModel. DataAnnotations;

namespace Store. Web.Areas.Admin.ViewModels;

public sealed class StockViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int VariantId { get; set; }
    public string VariantDescription { get; set; } = null!; // "Красный, XL"

    public List<StockItemViewModel> Stocks { get; set; } = new();
    public List<WarehouseSelectItem> Warehouses { get; set; } = new();

    public UpdateStockFormViewModel UpdateForm { get; set; } = new();
}

public sealed class StockItemViewModel
{
    public int StockId { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = null!;
    public int Quantity { get; set; }
}

public sealed class UpdateStockFormViewModel
{
    [Required(ErrorMessage = "Выберите склад")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите склад")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "Введите количество")]
    [Range(0, 999999, ErrorMessage = "Количество должно быть от 0 до 999999")]
    public int Quantity { get; set; }
}

public sealed class WarehouseSelectItem
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}