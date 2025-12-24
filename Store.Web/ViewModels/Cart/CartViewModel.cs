namespace Store.Web.ViewModels.Cart;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    public string Currency => Items.FirstOrDefault()?.Currency ?? "BYN";
}
