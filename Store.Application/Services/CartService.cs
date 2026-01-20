using Store.Domain.Entities;

public class CartService
{
    private readonly ICartRepository _repo;
    private readonly IProductStockRepository _stockRepo;

    public CartService(ICartRepository repo, IProductStockRepository stockRepo)
    {
        _repo = repo;
        _stockRepo = stockRepo;
    }

    public Task<List<CartItem>> GetAsync(string userId)
        => _repo.GetAsync(userId);

    public Task AddAsync(string userId, int productVariantId, int warehouseId, int qty)
        => _repo.AddAsync(new CartItem(userId, productVariantId, warehouseId, qty));

    public Task RemoveAsync(int id)
        => _repo.RemoveAsync(id);

    public Task ClearAsync(string userId)
        => _repo.ClearAsync(userId);

    public async Task UpdateQuantityAsync(int cartItemId, int quantity)
    {
        var item = await _repo.GetByIdAsync(cartItemId);
        if (item == null)
            return;

        var stock = item.ProductVariant.Stocks
            .FirstOrDefault(s => s.WarehouseId == item.WarehouseId);

        if (stock == null)
            throw new InvalidOperationException("Товар отсутствует на складе");

        if (quantity < 1)
            quantity = 1;

        if (quantity > stock.Quantity)
            quantity = stock.Quantity;

        item.SetQuantity(quantity);

        await _repo.SaveChangesAsync();
    }

    public async Task<int> AddWithStockCheckAsync(
                            string userId,
                            int productVariantId,
                            int warehouseId,
                            int qty)
    {
        var stock = await _stockRepo.GetAsync(productVariantId, warehouseId);

        if (stock == null)
            throw new InvalidOperationException("Товар отсутствует на этом складе");

        if (stock.Quantity < qty)
            throw new InvalidOperationException(
                $"На складе только {stock.Quantity} шт. товара");

        await _repo.AddAsync(
            new CartItem(userId, productVariantId, warehouseId, qty));

        var cartItems = await _repo.GetAsync(userId);
        return cartItems.Sum(x => x.Quantity);
    }


}
