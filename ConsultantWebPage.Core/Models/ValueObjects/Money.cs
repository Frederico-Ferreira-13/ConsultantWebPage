using System;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money() 
    { 
        Amount = 0; 
        Currency = "EUR"; 
    }

    public Money(decimal amount, string currency)
    {        
        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static Result<Money> Create(decimal amount, string currency = "EUR")
    {
        if (amount < 0) 
            return Error.Validation("A quantia não pode ser negativa.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            return Error.Validation("Moeda inválida (use ISO de 3 letras, ex: EUR).");

        return new Money(amount, currency);
    }

    public static Money Zero(string currency = "EUR") => new Money(0, currency);

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return Error.Validation("Operações entre moedas diferentes não são permitidas.");

        return new Money(Amount + other.Amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return Error.Validation("Operações entre moedas diferentes não são permitidas.");

        if (Amount - other.Amount < 0)
            return Error.Validation("O resultado de uma operação monetária não pode ser negativo.");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {       
        return new Money(Math.Max(0, Amount * factor), Currency);
    }

    public Result<Money> Divide(decimal divisor)
    {
        if (divisor == 0)
            return Error.Validation("Não é possível dividir um valor monetário por zero.");

        var roundedAmount = Math.Round(Amount / divisor, 2, MidpointRounding.AwayFromZero);
        return new Money(roundedAmount, Currency);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:N2} {Currency}";

    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);
    public static Money operator *(Money money, int quantity) => money.Multiply((decimal)quantity);
}
    
