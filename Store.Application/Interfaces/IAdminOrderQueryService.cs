using Store.Domain.Enums;

public interface IAdminOrderQueryService
{
    Task<List<AdminOrderListDto>> GetOrdersAsync(OrderStatus? status);
    Task<AdminOrderDetailsDto?> GetByIdAsync(int orderId);
    Task UpdateStatusAsync(int id, OrderStatus status);
}
