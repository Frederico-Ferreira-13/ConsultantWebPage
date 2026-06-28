using System.Collections.Generic;

namespace ConsultantWebPage.Core.Models.DTOs;

 public class ImportedPropertyData
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public string Currency { get; set; } = "EUR";

    public string? Street { get; set; }
    public string? Number { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string Country { get; set; } = "Portugal";

    public string? PropertyTypeName { get; set; } // ex: "Apartamento", "Moradia"

    public string? ExternalReferenceId { get; set; } // ID/referência no site de origem
    public string SourceUrl { get; set; } = string.Empty;
    public string SourceName { get; set; } = string.Empty; // ex: "RE/MAX", "Century 21"

    public List<string> ImageUrls { get; set; } = new();

    /// <summary>
    /// Indica se a importação foi bem sucedida e os dados podem ser confiados.
    /// Quando false, Warnings explica o que correu mal ou ficou incompleto.
    /// </summary>
    public bool Success { get; set; } = true;
    public List<string> Warnings { get; set; } = new();
}