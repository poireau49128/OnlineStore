using Store.Domain.Entities;
using Store.Domain.Enums;

public static class OrderStatusMapper
{
    public static string ToUserText(this OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Ожидает подтверждения",
        OrderStatus.Confirmed => "Подтверждён",
        OrderStatus.Paid => "Оплачен",
        OrderStatus.Shipped => "Отправлен",
        OrderStatus.Completed => "Завершён",
        OrderStatus.Cancelled => "Отменён",
        _ => "Неизвестно"
    };
}

public static class OrderStatusRules
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> _allowedTransitions =
        new()
        {
            [OrderStatus.Pending] = new[]
            {
                OrderStatus.Confirmed,
                OrderStatus.Cancelled
            },

            [OrderStatus.Confirmed] = new[]
            {
                OrderStatus.Paid,
                OrderStatus.Cancelled
            },

            [OrderStatus.Paid] = new[]
            {
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            },

            [OrderStatus.Shipped] = new[]
            {
                OrderStatus.Completed
            },

            [OrderStatus.Completed] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
        };

    public static bool CanChange(OrderStatus from, OrderStatus to)
        => _allowedTransitions.TryGetValue(from, out var allowed)
           && allowed.Contains(to);

    public static IReadOnlyList<OrderStatus> GetAllowedNext(OrderStatus from)
        => _allowedTransitions.TryGetValue(from, out var allowed)
           ? allowed
           : Array.Empty<OrderStatus>();
}
