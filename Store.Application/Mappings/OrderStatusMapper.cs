using Store.Domain.Entities;

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
