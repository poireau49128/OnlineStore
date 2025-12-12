namespace Store.Domain.ValueObjects;

public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; } = "BYN";

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty.");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money From(decimal amount, string currency = "BYN")
        => new Money(amount, currency);

    public static Money Zero(string currency = "BYN")
        => new Money(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Different currencies");

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Quantity must be >= 0");
        
        return new Money(Amount * quantity, Currency);
    }

    // Операторы
    public static Money operator +(Money a, Money b) => a.Add(b);
    public static Money operator *(Money a, int q) => a.Multiply(q);

    // Equality
    public bool Equals(Money? other)
        => other != null &&
           Amount == other.Amount &&
           Currency == other.Currency;

    public override bool Equals(object? obj)
        => obj is Money m && Equals(m);

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);

    public override string ToString()
        => $"{Amount:0.00} {Currency}";
}
