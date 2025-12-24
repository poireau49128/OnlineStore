using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;
using X.PagedList;

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

        public async Task<IActionResult> Index(int page = 1, int? categoryId = null, string? searchTerm = null)
        {
            var query = _db.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images) // картинки вариаций
                .Include(p => p.Category)
                    .ThenInclude(c => c.ProductType)
                
                .AsQueryable();


            // Фильтр по категории
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Поиск по имени
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }

            // Подсчет общего количества
            var totalCount = await query.CountAsync();

            // Получение текущей страницы
            var items = await query
                .OrderBy(p => p.CategoryId)
                .ThenBy(p => p.Name)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Создаем PagedList для Razor
            var pagedList = new StaticPagedList<Product>(items, page, PageSize, totalCount);

            // Передаем фильтры и категории в View
            ViewBag.Categories = await _db.Categories
                .Include(c => c.ProductType)
                .ToListAsync();
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SearchTerm = searchTerm;

            return View(pagedList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Images)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Stocks)
                        .ThenInclude(s => s.Warehouse)
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
        [HttpPost]
        public async Task<IActionResult> AddToCart(int variantId, int warehouseId, int qty = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

            await _cart.AddAsync(userId, variantId, warehouseId, qty);

            return RedirectToAction("Index", "Cart");
        }


    }
}
