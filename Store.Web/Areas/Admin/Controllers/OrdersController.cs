using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.Interfaces.Admin;
using Store.Domain.Enums;

[Area("Admin")]
[Authorize(Policy = "Admin")]
public class OrdersController : Controller
{
    private readonly IAdminOrderQueryService _queryService;
    private readonly IAdminOrderExportService _exportService;

    public OrdersController(IAdminOrderQueryService queryService,
        IAdminOrderExportService exportService)
    {
        _queryService = queryService;
        _exportService = exportService;
    }

    public async Task<IActionResult> Index(OrderStatus? status)
    {
        var orders = await _queryService.GetOrdersAsync(status);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _queryService.GetByIdAsync(id);

        if (order == null)
            return NotFound();

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
    {
        var order = await _queryService.GetByIdAsync(id);
        if (order == null)
            return NotFound();
        
        if (order.Status == status)
        {
            return Ok(new
            {
                success = true,
                status = order.Status.ToUserText(),
                statusEnum = order.Status.ToString(),
                message = "Статус не изменился",
                allowedNextStatuses = OrderStatusRules
                    .GetAllowedNext(order.Status)
                    .Select(s => new {
                        value = s.ToString(),
                        text = s.ToUserText()
                    })
            });
        }

        if (!OrderStatusRules.CanChange(order.Status, status))
        {
            return BadRequest(new
            {
                success = false,
                message = "Недопустимый переход статуса",
                allowedNextStatuses = OrderStatusRules.GetAllowedNext(order.Status)
                                                    .Select(s => new { text = s.ToUserText(), value = s.ToString() })
            });
        }

        

        await _queryService.UpdateStatusAsync(id, status);

        return Ok(new
        {
            success = true,
            status = status.ToUserText(),
            statusEnum = status.ToString(),
            message = "Статус заказа обновлён",
            allowedNextStatuses = OrderStatusRules
                .GetAllowedNext(status)
                .Select(s => new {
                    value = s.ToString(),
                    text = s.ToUserText()
                })
        });
    }

    [HttpGet]
    public async Task<IActionResult> Export(OrderStatus? status)
    {
        var file = await _exportService.ExportAsync(status);

        var fileName = $"orders_{status?.ToString() ?? "all"}_{DateTime.UtcNow:yyyyMMddHHmm}.xlsx";

        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }



}
