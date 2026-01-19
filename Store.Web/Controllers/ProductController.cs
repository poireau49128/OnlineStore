using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft. EntityFrameworkCore;
using Store.Domain.Entities;
using Store. Persistence;
using X.PagedList;
using Microsoft.AspNetCore.Http.Extensions;

namespace Store.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly CartService _cart;
        private const int PageSize = 9;

        public ProductController(AppDbContext db, CartService cart)
        {
            _db = db;
            _cart = cart;
        }

        public async Task<IActionResult> Index(int page = 1, int?  categoryId = null, string?  searchTerm = null)
        {
            var query = _db.Products
                . Include(p => p. Variants)
                    .ThenInclude(v => v. Images)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ProductType)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query. Where(p => p.Name. Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.CategoryId)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var pagedList = new StaticPagedList<Product>(items, page, PageSize, totalCount);

            ViewBag.Categories = await _db.Categories
                .Include(c => c.ProductType)
                .ToListAsync();
            ViewBag. SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;

            return View(pagedList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db. Products
                .Include(p => p. Variants)
                    .ThenInclude(v => v. Images)
                .Include(p => p. Variants)
                    .ThenInclude(v => v. Stocks)
                        .ThenInclude(s => s. Warehouse)
                .Include(p => p.Category)
                    .ThenInclude(c => c.ProductType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetStock(int id)
        {
            var stocks = await _db.ProductStocks
                .Where(s => s.ProductVariantId == id)
                .Include(s => s.Warehouse)
                .Select(s => new 
                { 
                    warehouse = new { s.Warehouse.Id, s.Warehouse.Name }, 
                    s.Quantity 
                })
                .ToListAsync();

            return Json(stocks);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest model)
        {
            try
            {
                if (model == null || model.variantId <= 0 || model.warehouseId <= 0 || model.qty < 1)
                {
                    return BadRequest(new { success = false, message = "Некорректные данные товара" });
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

                var stock = await _db.ProductStocks
                    .FirstOrDefaultAsync(s => s.ProductVariantId == model.variantId && s. WarehouseId == model.warehouseId);

                if (stock == null)
                {
                    return BadRequest(new { success = false, message = "Товар отсутствует на этом складе" });
                }

                if (stock.Quantity < model. qty)
                {
                    return BadRequest(new { success = false, message = $"На складе только {stock.Quantity} шт.  товара" });
                }

                await _cart.AddAsync(userId, model.variantId, model.warehouseId, model. qty);

                var cartItems = await _cart.GetAsync(userId);
                int cartCount = cartItems. Sum(x => x. Quantity);

                return Json(new 
                { 
                    success = true, 
                    message = "✓ Товар добавлен в корзину", 
                    cartCount 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Ошибка сервера:  " + ex.Message });
            }
        }
    }

    public class AddToCartRequest
    {
        public int variantId { get; set; }
        public int warehouseId { get; set; }
        public int qty { get; set; }
    }
}