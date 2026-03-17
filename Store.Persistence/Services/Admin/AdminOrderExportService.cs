using ClosedXML.Excel;
using Store.Application.Interfaces.Admin;
using Store.Domain.Enums;

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

        if (status.HasValue)
            data = data.Where(o => o.Order.Status == status.Value).ToList();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Заказы");

        // 🔹 HEADER
        var headers = new[]
        {
            "№ заказа", "Дата", "Email", "Телефон", "Адрес", "Статус",
            "Товар", "Цвет", "Размер", "Склад",
            "Кол-во", "Цена", "Скидка %", "Итого"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Cell(1, i + 1).Value = headers[i];
        }

        var headerRange = sheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F3A8A");
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int row = 2;

        foreach (var entry in data)
        {
            var order = entry.Order;
            int startRow = row;

            foreach (var item in order.Items)
            {
                sheet.Cell(row, 1).Value = order.Id;
                sheet.Cell(row, 2).Value = order.CreatedAt;
                sheet.Cell(row, 3).Value = entry.Email;
                sheet.Cell(row, 4).Value = entry.PhoneNumber ?? "N/A";
                sheet.Cell(row, 5).Value = entry.Address ?? "N/A";
                sheet.Cell(row, 6).Value = order.Status.ToUserText();

                sheet.Cell(row, 7).Value = item.ProductVariant.Product.Name;
                sheet.Cell(row, 8).Value = item.ProductVariant.Color;
                sheet.Cell(row, 9).Value = item.ProductVariant.Size;
                sheet.Cell(row, 10).Value = item.Warehouse?.Name ?? "N/A";

                sheet.Cell(row, 11).Value = item.Quantity;
                sheet.Cell(row, 12).Value = item.UnitPrice.Amount;
                sheet.Cell(row, 13).Value = item.DiscountPercent;
                sheet.Cell(row, 14).Value = item.TotalPrice.Amount;

                row++;
            }

            int endRow = row - 1;

            // 🔥 ГРУППИРОВКА ЗАКАЗА (визуально)
            var orderRange = sheet.Range(startRow, 1, endRow, 14);

            orderRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            orderRange.Style.Border.OutsideBorderColor = XLColor.Black;

            // Лёгкий фон для чередования
            if (startRow % 2 == 0)
            {
                orderRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
            }
        }

        // 🔹 Форматы
        sheet.Column(2).Style.DateFormat.Format = "dd.MM.yyyy HH:mm";
        sheet.Column(12).Style.NumberFormat.Format = "#,##0.00";
        sheet.Column(14).Style.NumberFormat.Format = "#,##0.00";

        // 🔹 Автофильтр
        sheet.RangeUsed().SetAutoFilter();

        // 🔹 Freeze header
        sheet.SheetView.FreezeRows(1);

        // 🔹 Ширина колонок
        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}