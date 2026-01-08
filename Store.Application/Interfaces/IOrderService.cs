public interface IOrderService
{
    Task<int> CheckoutAsync(CheckoutCommand command);
}
