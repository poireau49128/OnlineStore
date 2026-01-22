using Microsoft.AspNetCore. Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Application.DTOs;
using Store.Application.DTOs.Admin;
using Store. Application.Interfaces;
using Store.Application.Interfaces.Admin;
using Store.Persistence;
using Store.Web.Areas.Admin.ViewModels;

namespace Store.Web.Areas.Admin. Controllers;

[Area("Admin")]
[Authorize(Policy = "Admin")]
public class ProductsController :  Controller
{
    private readonly IProductCatalogQueryService _catalogQuery;
    private readonly IProductAdminService _adminService;
    private readonly ICategoryAutocompleteService _categoryService;
    private readonly AppDbContext _db;

    public ProductsController(
        IProductCatalogQueryService catalogQuery,
        IProductAdminService adminService,
        ICategoryAutocompleteService categoryService,
        AppDbContext db)
    {
        _catalogQuery = catalogQuery;
        _adminService = adminService;
        _categoryService = categoryService;
        _db = db;
    }

    // ============ LIST ============

    [HttpGet]
    public async Task<IActionResult> Index(string?  search, int? categoryId)
    {
        var products = await _catalogQuery.GetCatalogAsync(categoryId, search, includeAdminData: true);
        
        var model = products.Select(p => new ProductListItemViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Category = $"{p.ProductTypeName} / {p.CategoryName}",
            Sku = "N/A", // TODO: Добавить SKU в ProductCatalogItemDto
            Price = p.Price. Amount,
            VariantsCount = p. VariantsCount ??  0,
            TotalStock = p.TotalStock ??  0,
            IsActive = p.TotalStock > 0
        }).ToList();

        return View(model);
    }

    // ============ CREATE ============

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.SearchCategoriesAsync("");
        
        var model = new CreateProductViewModel
        {
            Categories = categories.Select(c => new SelectListItem
            {
                Id = c.Id,
                Name = c.Name,
                ProductTypeId = c.ProductTypeId,
                ProductTypeName = c.ProductTypeName
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Categories = (await _categoryService.SearchCategoriesAsync(""))
                .Select(c => new SelectListItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c.ProductTypeName
                }).ToList();

            return View(model);
        }

        try
        {
            // Конвертируем IFormFile в ProductImageFile
            var imageFiles = new List<ProductImageFile>();
            if (model.FirstVariantImages?. Any() == true)
            {
                foreach (var file in model. FirstVariantImages)
                {
                    var imageFile = new ProductImageFile
                    {
                        FileName = file.FileName,
                        Content = file.OpenReadStream(),
                        Size = file.Length
                    };
                    imageFiles.Add(imageFile);
                }
            }

            var request = new CreateProductRequest
            {
                Name = model.Name,
                Description = model.Description,
                BasePrice = model.BasePrice,
                Sku = model. Sku,
                CategoryId = model.CategoryId,
                FirstVariantColor = model.FirstVariantColor,
                FirstVariantSize = model.FirstVariantSize,
                FirstVariantPrice = model.FirstVariantPrice,
                FirstVariantImages = imageFiles
            };

            var productId = await _adminService.CreateProductWithVariantAsync(request);

            TempData["Success"] = $"Товар '{model.Name}' успешно создан";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Ошибка при создании товара: {ex.Message}");
            model.Categories = (await _categoryService.SearchCategoriesAsync(""))
                .Select(c => new SelectListItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c.ProductTypeName
                }).ToList();

            return View(model);
        }
    }

    // ============ EDIT ============

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _adminService.GetForEditAsync(id);
        if (product == null)
            return NotFound();

        var categories = await _categoryService.SearchCategoriesAsync("");

        var model = new EditProductViewModel
        {
            ProductId = product.Id,
            Name = product.Name,
            Description = product.Description,
            BasePrice = product. BasePrice. Amount,
            Sku = product.Sku,
            CategoryId = product.CategoryId,
            Categories = categories.Select(c => new SelectListItem
            {
                Id = c.Id,
                Name = c.Name,
                ProductTypeId = c.ProductTypeId,
                ProductTypeName = c.ProductTypeName
            }).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditProductViewModel model)
    {
        if (id != model.ProductId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            model.Categories = (await _categoryService.SearchCategoriesAsync(""))
                .Select(c => new SelectListItem
                {
                    Id = c.Id,
                    Name = c. Name,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c. ProductTypeName
                }).ToList();

            return View(model);
        }

        try
        {
            var request = new UpdateProductRequest
            {
                ProductId = model.ProductId,
                Name = model.Name,
                Description = model.Description,
                BasePrice = model.BasePrice,
                Sku = model.Sku,
                CategoryId = model.CategoryId
            };

            await _adminService.UpdateProductAsync(request);

            TempData["Success"] = $"Товар '{model.Name}' успешно обновлён";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState. AddModelError("", $"Ошибка при обновлении:  {ex.Message}");
            model.Categories = (await _categoryService.SearchCategoriesAsync(""))
                .Select(c => new SelectListItem
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductTypeId = c.ProductTypeId,
                    ProductTypeName = c.ProductTypeName
                }).ToList();

            return View(model);
        }
    }

    // ============ VARIANTS ============

    [HttpGet]
    public async Task<IActionResult> Variants(int id)
    {
        var product = await _adminService.GetForEditAsync(id);
        if (product == null)
            return NotFound();

        var model = new VariantsViewModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            BasePrice = product. BasePrice.Amount,
            Variants = product. Variants.Select(v => new VariantItemViewModel
            {
                Id = v.Id,
                Color = v.Color,
                Size = v.Size,
                OverridePrice = v.OverridePrice?. Amount,
                ActualPrice = v.ActualPrice. Amount,
                ImagePaths = v.ImagePaths,
                TotalStock = v. Stocks.Sum(s => s. Quantity)
            }).ToList(),
            CreateForm = new()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVariant(int productId, CreateVariantFormViewModel form)
    {
        if (! ModelState.IsValid)
            return RedirectToAction(nameof(Variants), new { id = productId });

        try
        {
            var imageFiles = new List<ProductImageFile>();
            if (form.Images?. Any() == true)
            {
                foreach (var file in form. Images)
                {
                    var imageFile = new ProductImageFile
                    {
                        FileName = file.FileName,
                        Content = file.OpenReadStream(),
                        Size = file.Length
                    };
                    imageFiles.Add(imageFile);
                }
            }

            var request = new CreateVariantRequest
            {
                ProductId = productId,
                Color = form.Color,
                Size = form.Size,
                OverridePrice = form. OverridePrice,
                Images = imageFiles
            };

            await _adminService.CreateVariantAsync(request);

            TempData["Success"] = $"Вариант '{form.Color}' успешно добавлен";
            return RedirectToAction(nameof(Variants), new { id = productId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Ошибка при добавлении варианта: {ex.Message}";
            return RedirectToAction(nameof(Variants), new { id = productId });
        }
    }

    // ============ STOCK ============

    [HttpGet]
    public async Task<IActionResult> Stock(int variantId)
    {
        var variant = await _db.ProductVariants
            .Include(v => v.Product)
            .Include(v => v. Stocks)
                .ThenInclude(s => s. Warehouse)
            .FirstOrDefaultAsync(v => v. Id == variantId);

        if (variant == null)
            return NotFound();

        var warehouses = await _db.Warehouses
            .OrderBy(w => w.Name)
            .ToListAsync();

        var model = new StockViewModel
        {
            ProductId = variant.ProductId,
            ProductName = variant.Product.Name,
            VariantId = variant.Id,
            VariantDescription = $"{variant.Color}" + (string.IsNullOrEmpty(variant.Size) ? "" : $", {variant.Size}"),
            Stocks = variant.Stocks.Select(s => new StockItemViewModel
            {
                StockId = s.Id,
                WarehouseId = s.WarehouseId,
                WarehouseName = s.Warehouse.Name,
                Quantity = s.Quantity
            }).ToList(),
            Warehouses = warehouses. Select(w => new WarehouseSelectItem
            {
                Id = w.Id,
                Name = w.Name
            }).ToList(),
            UpdateForm = new()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStock(int variantId, UpdateStockFormViewModel form)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Stock), new { variantId });

        try
        {
            var request = new UpdateStockRequest
            {
                ProductVariantId = variantId,
                WarehouseId = form.WarehouseId,
                Quantity = form.Quantity
            };

            await _adminService.UpdateStockAsync(request);

            TempData["Success"] = "Остаток успешно обновлён";
            return RedirectToAction(nameof(Stock), new { variantId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Ошибка при обновлении остатка:  {ex.Message}";
            return RedirectToAction(nameof(Stock), new { variantId });
        }
    }

    // ============ AUTOCOMPLETE API ============

    [HttpGet]
    public async Task<IActionResult> SearchCategories(string term, int?  typeId)
    {
        var categories = await _categoryService.SearchCategoriesAsync(term, typeId);
        return Json(categories. Select(c => new
        {
            c.Id,
            c.Name,
            c.ProductTypeId,
            c.ProductTypeName
        }));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductTypes()
    {
        var types = await _categoryService.GetAllProductTypesAsync();
        return Json(types);
    }
}