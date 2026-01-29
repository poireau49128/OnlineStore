using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft. EntityFrameworkCore;
using Store.Domain.Entities;
using Store. Persistence;
using X.PagedList;
using Microsoft.AspNetCore.Http.Extensions;
using Store.Application.Interfaces;
using Store.Application.DTOs;
using Store.Web.ViewModels.Product;

namespace Store.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductCatalogQueryService _catalog;
        private readonly IProductStockQueryService _stockQuery;
        private readonly ICategoryQueryService _categories;
        private readonly CartService _cart;
        private const int PageSize = 9;

        public ProductController(IProductCatalogQueryService catalog,IProductStockQueryService stockQuery, CartService cart, ICategoryQueryService categories)
        {
            _catalog = catalog;
            _stockQuery = stockQuery;
            _cart = cart;
            _categories = categories;
        }

        public async Task<IActionResult> Index(int page = 1, int?  categoryId = null, string?  searchTerm = null)
        {
            var total = await _catalog.GetCatalogCountAsync(categoryId, searchTerm);
            var items = await _catalog.GetCatalogAsync(
                categoryId,
                searchTerm,
                skip: (page - 1) * PageSize,
                take: PageSize);

            var pagedList = new StaticPagedList<ProductCatalogItemDto>(
                items,
                page,
                PageSize,
                total);

            var model = new ProductCatalogViewModel
            {
                Products = pagedList,
                CategoryGroups = await _categories.GetCategoryFilterAsync(),
                SelectedCategoryId = categoryId,
                SearchTerm = searchTerm
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _catalog.GetDetailsAsync(id);

            if (product == null) return NotFound();

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                CategoryName = product.CategoryName,
                ProductTypeName = product.ProductTypeName,
                Sku = product.Sku,
                BasePrice = product.BasePrice,
                Variants = product.Variants,
                CategoryId = product.CategoryId
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetStock(int id)
        {
            var stocks = await _stockQuery.GetByVariantAsync(id);
            return Json(stocks.Select(s => new {
                        warehouse = s.Warehouse,
                        quantity = s.Quantity
                    }));
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest model)
        {
            if (model == null ||
                model.variantId <= 0 ||
                model.warehouseId <= 0 ||
                model.qty < 1)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Некорректные данные товара"
                });
            }

            try
            {
                var userId = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

                var cartCount = await _cart.AddWithStockCheckAsync(
                    userId,
                    model.variantId,
                    model.warehouseId,
                    model.qty);

                return Json(new
                {
                    success = true,
                    message = "✓ Товар добавлен в корзину",
                    cartCount
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
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