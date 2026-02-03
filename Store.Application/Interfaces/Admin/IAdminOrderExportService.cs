using Store.Domain.Enums;
namespace Store.Application.Interfaces.Admin;

public interface IAdminOrderExportService
{
    Task<byte[]> ExportAsync(OrderStatus? status);
}
