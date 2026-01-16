public sealed class OrderQueryService : IOrderQueryService
{
    private readonly IOrderRepository _orderRepo;

    public OrderQueryService(IOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    public async Task<IReadOnlyList<OrderListItemDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _orderRepo.GetByUserAsync(userId);

        return orders.Select(o => new OrderListItemDto
        {
            Id = o.Id,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            Total = o.Items.Sum(i => i.UnitPrice.Amount * i.Quantity)
        }).ToList();
    }

    public async Task<OrderDetailsDto?> GetOrderDetailsAsync(int orderId, string userId)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);

        if (order == null || order.UserId != userId)
            return null;

        return new OrderDetailsDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.Items.Select(i => new OrderItemDto
            {
                ProductName = i.ProductVariant.Product.Name,
                Color = i.ProductVariant.Color,
                Size = i.ProductVariant.Size,
                WarehouseName = i.Warehouse.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                Discount = i.DiscountPercent,
                TotalPrice = i.UnitPrice.Amount * i.Quantity
            }).ToList(),
            Comment = order.Comment,
            Total = order.Items.Sum(i => i.UnitPrice.Amount * i.Quantity)
        };
    }
}
