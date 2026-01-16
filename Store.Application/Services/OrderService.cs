using Store.Domain.Entities;
using Store.Domain.ValueObjects;
using Store.Application.Exceptions;

public sealed class OrderService : IOrderService
{
    private readonly CartService _cartService;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductStockRepository _stockRepository;
    private readonly IUnitOfWork _uow;
    private readonly IDiscountService _discountService;

    public OrderService(
        CartService cartService,
        IOrderRepository orderRepository,
        IProductStockRepository stockRepository,
        IUnitOfWork uow,
        IDiscountService discountService)
    {
        _cartService = cartService;
        _orderRepository = orderRepository;
        _stockRepository = stockRepository;
        _uow = uow;
        _discountService = discountService;
    }
    
    public async Task<int> CheckoutAsync(CheckoutCommand command)
    {
        await _uow.BeginAsync();

        try
        {
            var cartItems = await _cartService.GetAsync(command.UserId);

            if (! cartItems.Any())
                throw new InvalidOperationException("Cart is empty");

            var order = new Order(command.UserId, command.Comment);

            foreach (var cartItem in cartItems)
            {
                var stock = await _stockRepository.GetAsync(
                    cartItem.ProductVariantId,
                    cartItem.WarehouseId);

                if (! stock.CanFulfill(cartItem.Quantity))
                    throw new InsufficientStockException(
                        $"Недостаточно \"{cartItem.ProductVariant.Product.Name} {cartItem.ProductVariant.Color}\" на складе. Доступно: {stock.Quantity}"
                    );

                stock.Decrease(cartItem.Quantity);

                if (stock.Quantity == 0)
                    await _stockRepository.RemoveAsync(stock.Id);

                var variant = cartItem.ProductVariant;
                var product = variant.Product;

                var baseUnitPrice = variant.GetPrice(product.BasePrice);

                var discountPercent =
                    await _discountService.GetCategoryDiscountAsync(
                        command.UserId,
                        cartItem.ProductVariant.Product.CategoryId,
                        DateTime.UtcNow);

                var discountedAmount = baseUnitPrice.Amount * (1 - discountPercent / 100m);
                var finalUnitPrice = Money.From(discountedAmount, baseUnitPrice.Currency);

                order.AddItem(
                    cartItem.ProductVariantId,
                    cartItem.WarehouseId,
                    cartItem.Quantity,
                    finalUnitPrice,
                    discountPercent);
                order.RecalculateTotal();
            }

            await _orderRepository.AddAsync(order);
            await _uow.CommitAsync();

            await _cartService.ClearAsync(command.UserId);

            return order.Id;
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}
