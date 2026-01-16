public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderListDto>> GetUserOrdersAsync(string userId);
    Task<OrderDetailsDto?> GetOrderDetailsAsync(int orderId, string userId);
}

