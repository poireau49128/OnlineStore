using Store.Application.DTOs.Admin;
namespace Store.Application.Interfaces.Admin;

public interface ICategoryCommandService
{
    Task<CreateCategoryResultDto> CreateAsync(CreateCategoryDto dto);
}
