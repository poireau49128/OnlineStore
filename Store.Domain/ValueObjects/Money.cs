namespace Store.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BYN";

    public Money() { }

    public Money(decimal amount, string currency = "BYN")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Zero(string currency = "BYN") => new Money(0, currency);

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator *(Money a, int quantity)
        => new Money(a.Amount * quantity, a.Currency);

    public bool Equals(Money other) =>
        Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) =>
        obj is Money m && Equals(m);

    public override int GetHashCode() => HashCode.Combine(Amount, Currency);

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
