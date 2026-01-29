using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs.Admin;
using Store.Application.Interfaces.Admin;
using Store.Domain.Entities;

[Area("Admin")]
[ApiController]
[Route("Admin/Categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryCommandService _command;

    public CategoriesController(ICategoryCommandService command)
    {
        _command = command;
    }

    [HttpPost("CreateAjax")]
    public async Task<IActionResult> CreateAjax([FromBody] CreateCategoryDto dto)
    {
        try
        {
            var result = await _command.CreateAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}
