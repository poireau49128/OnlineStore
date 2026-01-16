using Store.Domain.ValueObjects;

namespace Store.Web.ViewModels.Cart;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();

    public Money Total =>
        Items.Count == 0
            ? Money.Zero()
            : Items
                .Select(i => i.TotalPrice)
                .Aggregate((a, b) => a + b);
}
