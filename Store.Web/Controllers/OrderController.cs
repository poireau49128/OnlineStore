using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;

[Authorize]
public class OrderController : Controller
{
    private readonly CartService _cart;
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(
        CartService cart,
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        _cart = cart;
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        var cart = await _cart.GetAsync(user!.Id);

        if (!cart.Any())
            return RedirectToAction("Index", "Product");

        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Create(string? comment)
    {
        var user = await _userManager.GetUserAsync(User);
        var cartItems = await _cart.GetAsync(user!.Id);

        if (!cartItems.Any())
            return RedirectToAction("Index", "Product");

        var order = new Order(user.Id, comment);

        foreach (var item in cartItems)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            var stock = await _db.ProductStocks.FirstAsync(s =>
                s.ProductVariantId == item.ProductId &&
                s.WarehouseId == item.WarehouseId);

            if (!stock.CanFulfill(item.Quantity))
                throw new InvalidOperationException("Not enough stock");

            stock.Decrease(item.Quantity);

            order.AddItem(
                item.ProductId,
                item.WarehouseId,
                item.Quantity,
                product!.BasePrice,
                null
            );
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await _cart.ClearAsync(user.Id);

        return RedirectToAction("Success");
    }

    public IActionResult Success() => View();
}
