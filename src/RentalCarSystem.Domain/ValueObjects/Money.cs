namespace RentalCarSystem.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Money(decimal amount, string currency = "TWD")
    {
        if (amount < 0)
            throw new ArgumentException("金額不能為負數");
            
        Amount = amount;
        Currency = currency;
    }
    
    public static Money Zero => new(0);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new ArgumentException("貨幣類型不符");
            
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Multiply(decimal multiplier)
    {
        return new Money(Amount * multiplier, Currency);
    }
    
    public override string ToString()
    {
        return $"{Amount:C}";
    }
    
    public static implicit operator decimal(Money money) => money.Amount;
    public static implicit operator Money(decimal amount) => new(amount);
}