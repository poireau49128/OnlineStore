namespace Store.Application.Utilities;

public static class CurrencyHelper
{
    public const string DefaultCurrency = "BYN";
    
    public static readonly List<string> SupportedCurrencies = new()
    {
        "BYN",
        "USD",
        "EUR",
        "RUB"
    };
}