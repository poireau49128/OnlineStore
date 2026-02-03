namespace Store. Application.Interfaces.Admin;
public interface IProductTypeAdminQueryService
{
    Task<List<ProductTypeWithCategoriesDto>> GetAllWithCategoriesAsync();
}
