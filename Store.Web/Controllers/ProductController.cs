using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Persistence;

public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private const int PageSize = 9;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var totalProducts = await _db.Products.CountAsync();
        var totalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

        var products = await _db.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _db.Products
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            .Include(p => p.Category)
                .ThenInclude(c => c.ProductType)
            // .Include(p => p.StockEntries)
            //     .ThenInclude(s => s.Warehouse)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound();
        return View(product);
    }
}
