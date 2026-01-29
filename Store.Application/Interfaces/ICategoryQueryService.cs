
using Store.Application.DTOs;

namespace Store.Application.Interfaces;

public interface ICategoryQueryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int categoryId);
    Task<List<CategoryFilterGroupDto>> GetCategoryFilterAsync();
}
