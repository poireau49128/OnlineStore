using Store.Domain.Entities;
using Store.Domain.Enums;
public sealed class AdminOrderQueryService : IAdminOrderQueryService
{ 
	private readonly IOrderRepository _orderRepository; 
	public AdminOrderQueryService(IOrderRepository orderRepository) 
	{
		_orderRepository = orderRepository;
	}
	public async Task<List<AdminOrderListDto>> GetOrdersAsync(OrderStatus? status) 
	{
		var orders = await _orderRepository.GetAllAsync();
		if (status.HasValue)
			orders = orders.Where(o => o.Status == status.Value).ToList();
		return orders.Select(o => new AdminOrderListDto 
			{ Id = o.Id, CreatedAt = o.CreatedAt, Status = o.Status, TotalPrice = o.TotalPrice 
			}).ToList(); 
	}
	public async Task<AdminOrderDetailsDto?> GetByIdAsync(int orderId) 
	{
		var order = await _orderRepository.GetByIdAsync(orderId);
		if (order == null) 
			return null;

		var dto = new AdminOrderDetailsDto
        {
            Id = order.Id,
            UserEmail = order.UserId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            TotalPrice = order.TotalPrice,
            Items = order.Items.Select(i => new AdminOrderItemDto 
                {
                    ProductName = i.ProductVariant.Product.Name, Quantity = i.Quantity, Price = i.UnitPrice 
                }).ToList()
        };

        dto.AllowedNextStatuses =
            OrderStatusRules.GetAllowedNext(order.Status);

        return dto;
	}
	public async Task UpdateStatusAsync(int orderId, OrderStatus status) 
	{
		var order = await _orderRepository.GetByIdAsync(orderId);
		if (order == null) 
			throw new InvalidOperationException("Order not found");
		order.ChangeStatus(status);
		await _orderRepository.SaveChangesAsync(); 
	}
}