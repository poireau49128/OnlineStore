namespace Store.Web.ViewModels.Order;

public sealed class CheckoutItemViewModel
{
    public int CartItemId { get; init; }
    public int ProductVariantId { get; init; }
    public int WarehouseId { get; init; }

    public string ProductName { get; init; } = null!;
    public string Color { get; init; } = null!;
    public string Size { get; init; } = null!;

    public int Quantity { get; init; }

    // Цена без скидки
    public decimal UnitPrice { get; init; }

    // Сумма без скидки
    public decimal Total => UnitPrice * Quantity;

    public decimal DiscountPercent { get; init; }

    // Итоговая цена с учётом скидки
    public decimal TotalWithDiscount => Total * (1 - DiscountPercent / 100m);
}

public sealed class CheckoutViewModel
{
    public List<CheckoutItemViewModel> Items { get; init; } = new();

    // Сумма без скидки
    public decimal TotalAmount => Items.Sum(i => i.Total);

    // Сумма со скидкой
    public decimal TotalWithDiscount => Items.Sum(i => i.TotalWithDiscount);

    public string? Comment { get; init; }
}
