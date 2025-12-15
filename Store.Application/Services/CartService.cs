using Store.Domain.Entities;

public class CartService
{
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    public Task<List<CartItem>> GetAsync(string userId)
        => _repo.GetAsync(userId);

    public Task AddAsync(string userId, int productId, int warehouseId, int qty)
        => _repo.AddAsync(new CartItem(userId, productId, warehouseId, qty));

    public Task ClearAsync(string userId)
        => _repo.ClearAsync(userId);
}
