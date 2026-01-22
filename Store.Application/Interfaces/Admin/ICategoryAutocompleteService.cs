using Store.Application.DTOs;
using Store.Application.DTOs.Admin;

namespace Store.Application.Interfaces.Admin;

public interface ICategoryAutocompleteService
{
    Task<List<CategoryDto>> SearchCategoriesAsync(string searchTerm, int?  productTypeId = null);
    Task<List<ProductTypeDto>> GetAllProductTypesAsync();
}