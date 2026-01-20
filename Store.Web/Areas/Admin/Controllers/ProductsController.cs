using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.Interfaces;
using Store.Web.Areas.Admin.ViewModels;

[Area("Admin")]
[Authorize(Policy = "Admin")]
public class ProductsController : Controller
{
    private readonly IProductCatalogQueryService _catalog;

    public ProductsController(IProductCatalogQueryService catalog)
    {
        _catalog = catalog;
    }

    public async Task<IActionResult> Index(
        string? search,
        int? categoryId)
    {
        var products = await _catalog.GetCatalogAsync(categoryId, search);
        var model = products.Select(p => new ProductListItemViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = $"{p.ProductTypeName} / {p.CategoryName}",
            Price = p.Price.Amount,
            IsActive = true
        })
        .ToList();

        return View(model);
    }
    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        return View();
    }

    public IActionResult Variants(int id)
    {
        return View();
    }

}
