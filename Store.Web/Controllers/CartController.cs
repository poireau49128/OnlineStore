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
                    UnitPrice = price,
                };
            }).ToList()
        };

        return View(vm);
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Update(int id, int quantity)
    {
        try
        {
            if (id <= 0 || quantity < 1)
                return BadRequest(new { success = false, message = "Некорректные данные" });

            await _cart.UpdateQuantityAsync(id, quantity);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var cartItems = await _cart.GetAsync(userId);
            int cartCount = cartItems.Sum(x => x.Quantity);

            decimal totalAmount = 0;
            string currency = "BYN";

            if (cartItems.Any())
            {
                var item = cartItems.First();
                currency = item.ProductVariant.GetPrice(item.ProductVariant.Product.BasePrice).Currency;
                totalAmount = cartItems.Sum(x =>
                    x.Quantity * x.ProductVariant.GetPrice(x.ProductVariant.Product.BasePrice).Amount
                );
            }

            var updatedItem = cartItems.FirstOrDefault(x => x.Id == id);
            if (updatedItem == null)
                return NotFound(new { success = false, message = "Товар не найден" });

            var variant = updatedItem.ProductVariant;
            var product = variant.Product;
            var price = variant.GetPrice(product.BasePrice);
            var stock = variant.Stocks.FirstOrDefault(s => s.WarehouseId == updatedItem.WarehouseId);

            return Json(new
            {
                success = true,
                message = "✓ Количество обновлено",
                cartCount,
                totalAmount,
                currency,
                item = new
                {
                    id = updatedItem.Id,
                    quantity = updatedItem.Quantity,
                    unitPrice = price.Amount,
                    totalPrice = updatedItem.Quantity * price.Amount,
                    availableQuantity = stock?.Quantity ?? 0
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ошибка при обновлении:  " + ex.Message });
        }
    }

    [Authorize]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            if (id <= 0)
                return BadRequest(new { success = false, message = "Некорректный ID товара" });

            await _cart.RemoveAsync(id);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var cartItems = await _cart.GetAsync(userId);
            int cartCount = cartItems.Sum(x => x.Quantity);

            decimal totalAmount = 0;
            string currency = "BYN";

            if (cartItems.Any())
            {
                var item = cartItems.First();
                currency = item.ProductVariant.GetPrice(item.ProductVariant.Product.BasePrice).Currency;
                totalAmount = cartItems.Sum(x =>
                    x.Quantity * x.ProductVariant.GetPrice(x.ProductVariant.Product.BasePrice).Amount
                );
            }

            return Json(new
            {
                success = true,
                message = "✓ Товар удален из корзины",
                cartCount,
                totalAmount,
                currency
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = "Ошибка при удалении: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
        var cartItems = await _cart.GetAsync(userId);
        int count = cartItems.Sum(item => item.Quantity);
        return Json(new { count });
    }
}

// DTO для запросов
public class RemoveCartItemRequest
{
    public int Id { get; set; }
}

public class UpdateCartItemRequest
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}