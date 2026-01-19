using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.Commands;
using Store.Application.Interfaces;
using Store.Web.Areas.Admin.ViewModels;

namespace Store.Web.Controllers.Admin;

[Area("Admin")]
[Authorize(Policy = "Admin")]
public sealed class UsersController : Controller
{
    private readonly IUserQueryService _userQueryService;
    private readonly IUserDetailsQueryService _userDetailsQueryService;
    private readonly IUserRoleService _userRoleService;
    private readonly IOrderQueryService _orderQueryService;

    public UsersController(
        IUserQueryService userQueryService,
        IUserDetailsQueryService userDetailsQueryService,
        IUserRoleService userRoleService,
        IOrderQueryService orderQueryService)
    {
        _userQueryService = userQueryService;
        _userDetailsQueryService = userDetailsQueryService;
        _userRoleService = userRoleService;
        _orderQueryService = orderQueryService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userQueryService.GetAllAsync();
        return View(users);
    }

    public async Task<IActionResult> Details(string id)
{
    var user = await _userDetailsQueryService.GetByIdAsync(id);
    if (user == null)
        return NotFound();

    var orders = await _orderQueryService.GetUserOrdersAsync(id);

    var vm = new UserDetailsViewModel
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        IsAdmin = user.Roles.Contains("Admin"),
        Orders = orders.Select(o => new UserOrderDto
            {
                Id = o.Id,
                Status = o.Status.ToUserText(),
                CreatedAt = o.CreatedAt
            }).ToList()
    };

    return View(vm);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAdminRole(string userId, bool isAdmin)
    {
        var command = new ChangeUserAdminRoleCommand(userId, isAdmin, User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _userRoleService.SetAdminRoleAsync(command);
            TempData["Success"] = "Роль пользователя обновлена";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = command.UserId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        try
        {
            var newPassword = "123456";
            await _userRoleService.ResetPasswordAsync(userId, newPassword);
            TempData["Success"] = $"Пароль сброшен. Новый пароль: {newPassword}";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = userId });
    }

}
