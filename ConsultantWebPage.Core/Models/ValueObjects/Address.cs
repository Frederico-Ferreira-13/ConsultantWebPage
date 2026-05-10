using System;
using System.Text.RegularExpressions;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models.ValueObjects;

public class Address : ValueObject
{
    private const int MaxStreetLength = 100;
    private const int MaxCityLength = 50;
    private const int MaxCountryLength = 50;
    private const int MaxComplementLength = 50;
    private const int MaxNumberLength = 10;

    private static readonly Regex PortugalPostalCodeRegex = new Regex(@"^\d{4}-\d{3}$", RegexOptions.Compiled);

    public string Street { get; }
    public string Number { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }
    public string? Complement { get; }

    private Address()
    {
        Street = null!;
        Number = null!;
        City = null!;
        PostalCode = null!;
        Country = null!;
    }

    private Address(string street, string number, string city, string postalCode, string country, string? complement = null)
    {
        Street = street.Trim();
        Number = number.Trim();
        City = city.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
        Complement = string.IsNullOrWhiteSpace(complement) ? null : complement.Trim();
    }

    public static Result<Address> Create(string street, string number, string city, string postalCode, string country, string? complement = null)
    {
        // Validações de obrigatoriedade e tamanho
        if (string.IsNullOrWhiteSpace(street) || street.Length > MaxStreetLength)
            return Error.Validation($"Rua inválida (obrigatória e máx {MaxStreetLength} chars).");

        if (string.IsNullOrWhiteSpace(number) || number.Length > MaxNumberLength)
            return Error.Validation($"Número inválido (obrigatório e máx {MaxNumberLength} chars).");

        if (string.IsNullOrWhiteSpace(city) || city.Length > MaxCityLength)
            return Error.Validation($"Cidade inválida (obrigatória e máx {MaxCityLength} chars).");

        if (string.IsNullOrWhiteSpace(country) || country.Length > MaxCountryLength)
            return Error.Validation($"País inválido (obrigatório e máx {MaxCountryLength} chars).");

        if (string.IsNullOrWhiteSpace(postalCode))
            return Error.Validation("Código postal é obrigatório.");
        
        // Validação específica para Portugal
        if (country.Equals("Portugal", StringComparison.OrdinalIgnoreCase) &&
            !PortugalPostalCodeRegex.IsMatch(postalCode.Trim()))
        {
            return Error.Validation("Formato de código postal inválido para Portugal (XXXX-XXX).");
        }

        // Retorna Sucesso com a nova instância
        return new Address(street, number, city, postalCode, country, complement);
    }    

    public Address WithComplement(string? newComplement) =>
        new Address(Street, Number, City, PostalCode, Country, newComplement);

    public Address ChangeNumberAndComplement(string newNumber, string? newComplement) =>
        new Address(Street, newNumber, City, PostalCode, Country, newComplement);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street.ToLower();
        yield return Number.ToLower();
        yield return City.ToLower();
        yield return PostalCode.ToLower();
        yield return Country.ToLower();
        yield return Complement?.ToLower();
    }

    public override string ToString()
    {
        var complementPart = string.IsNullOrWhiteSpace(Complement) ? "" : $" ({Complement})";
        return $"{Street}, {Number}{complementPart}, {PostalCode} {City}, {Country}";
    }
}
