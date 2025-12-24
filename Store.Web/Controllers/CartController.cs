using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Web.ViewModels.Cart;

[Authorize]
public class CartController : Controller
{
    private readonly CartService _cart;

    public CartController(CartService cart)
    {
        _cart = cart;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var cartItems = await _cart.GetAsync(userId);

        var vm = new CartViewModel
        {
            Items = cartItems.Select(item =>
            {
                var variant = item.ProductVariant;
                var product = variant.Product;
                var price = variant.GetPrice(product.BasePrice);

                var stock = variant.Stocks
                    .FirstOrDefault(s => s.WarehouseId == item.WarehouseId);

                return new CartItemViewModel
                {
                    Id = item.Id,
                    ProductVariantId = variant.Id,
                    ProductName = product.Name,
                    Color = variant.Color,
                    Size = variant.Size,

                    Quantity = item.Quantity,

                    WarehouseName = item.Warehouse.Name,
                    AvailableQuantity = stock?.Quantity ?? 0,

                    UnitPrice = price.Amount,
                    Currency = price.Currency,
                };
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        await _cart.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, int quantity)
    {
        await _cart.UpdateQuantityAsync(id, quantity);
        return RedirectToAction(nameof(Index));
    }


}
