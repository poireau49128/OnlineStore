public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderListItemDto>> GetUserOrdersAsync(string userId);
    Task<OrderDetailsDto?> GetOrderDetailsAsync(int orderId, string userId);
}

