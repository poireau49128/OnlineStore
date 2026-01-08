using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Web.ViewModels.Order;

[Authorize]
public class OrderController : Controller
{
    private readonly CartService _cart;
    private readonly IOrderService _orderService;
    private readonly IDiscountService _discountService;

    public OrderController(
        CartService cart,
        IOrderService orderService,
        IDiscountService discountService)
    {
        _cart = cart;
        _orderService = orderService;
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = User.GetUserId();
        var cartItems = await _cart.GetAsync(userId);

        if (!cartItems.Any())
            return RedirectToAction("Index", "Product");

        var vm = new CheckoutViewModel
        {
            Items = new List<CheckoutItemViewModel>()
        };

        foreach (var i in cartItems)
        {
            var variant = i.ProductVariant;
            var product = variant.Product;
            var unitPrice = variant.GetPrice(product.BasePrice);

            var discountPercent = await _discountService.GetCategoryDiscountAsync(
                userId,
                product.CategoryId,
                DateTime.UtcNow
            );

            vm.Items.Add(new CheckoutItemViewModel
            {
                CartItemId = i.Id,
                ProductVariantId = variant.Id,
                WarehouseId = i.WarehouseId,
                ProductName = product.Name,
                Color = variant.Color,
                Size = variant.Size,
                Quantity = i.Quantity,
                UnitPrice = unitPrice.Amount,
                DiscountPercent = discountPercent
            });
        }

        return View(vm);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel vm)
    {
        var command = new CheckoutCommand
        {
            UserId = User.GetUserId(),
            Comment = vm.Comment,
            Items = vm.Items.Select(i => new CheckoutItemDto
            {
                ProductVariantId = i.ProductVariantId,
                WarehouseId = i.WarehouseId,
                Quantity = i.Quantity
            }).ToList()
        };

        try
        {
            var orderId = await _orderService.CheckoutAsync(command);
            return RedirectToAction("Success", new { id = orderId });
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["Error"] =
                "Один из товаров закончился. Корзина обновлена.";
            return RedirectToAction(nameof(Checkout));
        }
    }

    public IActionResult Success(int id) => View(id);
}
