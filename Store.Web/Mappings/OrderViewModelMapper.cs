using Store.Web.ViewModels.Order;

public static class OrderViewModelMapper
{
    public static OrderListViewModel ToVm(this OrderListDto dto)
        => new()
        {
            Id = dto.Id,
            CreatedAt = dto.CreatedAt,
            Status = OrderStatusMapper.ToUserText(dto.Status),
            Total = dto.Total
        };

    public static OrderDetailsViewModel ToVm(this OrderDetailsDto dto)
        => new()
        {
            Id = dto.Id,
            CreatedAt = dto.CreatedAt,
            Status = OrderStatusMapper.ToUserText(dto.Status),
            Items = dto.Items.Select(i => new OrderItemViewModel
            {
                ProductName = i.ProductName,
                Color = i.Color,
                Size = i.Size,
                WarehouseName = i.WarehouseName,
                Quantity = i.Quantity,
                Price = i.UnitPrice,
                Discount = i.Discount,
                Total = i.UnitPrice * i.Quantity
            }).ToList(),
            Comment = dto.Comment,
            Total = dto.Total            
        };
}
