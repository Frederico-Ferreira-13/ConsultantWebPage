using System;
using System.Text.RegularExpressions;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models.ValueObjects;

public class ContactInfo : ValueObject
{
    private const int MaxPhoneLength = 20;    
    
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    public string Email { get; }
    public string PhoneNumber { get; }

    // Construtor privado para o EF e para o Factory Method
    private ContactInfo()
    {
        Email = null!;
        PhoneNumber = null!;
    }

    private ContactInfo(string email, string phoneNumber)
    {
        Email = email.Trim().ToLowerInvariant();
        PhoneNumber = SanitizePhoneNumber(phoneNumber);
    }

    public static Result<ContactInfo> Create(string email, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email)) 
            return Error.Validation("Email é obrigatório.");

        if (string.IsNullOrWhiteSpace(phoneNumber)) 
            return Error.Validation("Telefone é obrigatório.");

        var sanitizedPhone = SanitizePhoneNumber(phoneNumber);
        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Validações
        if (!EmailRegex.IsMatch(normalizedEmail))
            return Error.Validation($"Formato de email inválido: {normalizedEmail}");

        if (sanitizedPhone.Length is < 9 or > MaxPhoneLength)
            return Error.Validation($"Número de telefone com comprimento inválido ({sanitizedPhone.Length}).");

        return new ContactInfo(normalizedEmail, sanitizedPhone);
    }

    private static string SanitizePhoneNumber(string phone)
        => Regex.Replace(phone, @"[\s\-\(\)\.]", "");

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Email;
        yield return PhoneNumber;
    }

    public override string ToString() => $"{Email} | {PhoneNumber}";
}
