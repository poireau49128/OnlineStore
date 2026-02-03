using ClosedXML.Excel;
using Store.Application.Interfaces.Admin;
using Store.Domain.Enums;
using Store.Domain.ValueObjects;

public sealed class AdminOrderExportService : IAdminOrderExportService
{
    private readonly IOrderRepository _orderRepository;

    public AdminOrderExportService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<byte[]> ExportAsync(OrderStatus? status)
    {
        var data = await _orderRepository.GetAllWithEmailsAsync();
        //var orders = await _orderRepository.GetAllAsync();

        if (status.HasValue)
            data = data.Where(o => o.Order.Status == status.Value).ToList();

        using var workbook = new XLWorkbook();

        // ---------- Лист 1: Заказы ----------
        var ordersSheet = workbook.Worksheets.Add("Заказы");

        ordersSheet.Cell(1, 1).Value = "№";
        ordersSheet.Cell(1, 2).Value = "Дата";
        ordersSheet.Cell(1, 3).Value = "Пользователь";
        ordersSheet.Cell(1, 4).Value = "Статус";
        ordersSheet.Cell(1, 5).Value = "Сумма";

        ordersSheet.Range("A1:E1").Style.Font.Bold = true;

        var row = 2;
        foreach (var item in data)
        {
            var o = item.Order;
            ordersSheet.Cell(row, 1).Value = o.Id;
            ordersSheet.Cell(row, 2).Value = o.CreatedAt;
            ordersSheet.Cell(row, 3).Value = item.Email; 
            ordersSheet.Cell(row, 4).Value = o.Status.ToUserText();
            ordersSheet.Cell(row, 5).Value = o.TotalPrice.Amount;
            row++;
        }

        ordersSheet.Columns().AdjustToContents();

        // ---------- Лист 2: Позиции заказов ----------
        var itemsSheet = workbook.Worksheets.Add("Товары");

        itemsSheet.Cell(1, 1).Value = "№";
        itemsSheet.Cell(1, 2).Value = "Товар";
        itemsSheet.Cell(1, 3).Value = "Цвет";
        itemsSheet.Cell(1, 4).Value = "Размер";
        itemsSheet.Cell(1, 5).Value = "Склад";
        itemsSheet.Cell(1, 6).Value = "Кол-во";
        itemsSheet.Cell(1, 7).Value = "Цена";
        itemsSheet.Cell(1, 8).Value = "Скидка %";
        itemsSheet.Cell(1, 9).Value = "Итого";

        itemsSheet.Range("A1:I1").Style.Font.Bold = true;

        row = 2;
        foreach (var entry in data)
        {
            foreach (var item in entry.Order.Items)
            {
                itemsSheet.Cell(row, 1).Value = entry.Order.Id;
                itemsSheet.Cell(row, 2).Value = item.ProductVariant.Product.Name;
                itemsSheet.Cell(row, 3).Value = item.ProductVariant.Color;
                itemsSheet.Cell(row, 4).Value = item.ProductVariant.Size;
                itemsSheet.Cell(row, 5).Value = item.Warehouse?.Name ?? "N/A";
                itemsSheet.Cell(row, 6).Value = item.Quantity;
                itemsSheet.Cell(row, 7).Value = item.UnitPrice.Amount;
                itemsSheet.Cell(row, 8).Value = item.DiscountPercent;
                itemsSheet.Cell(row, 9).Value = item.TotalPrice.Amount;
                row++;
            }
        }

        itemsSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
