using System;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Models.DTOs;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services.Import;

 public class MockImportProvider : IPropertyImportProvider
{
    public string ProviderName => "Mock/Demo";

    // O Mock "aceita" qualquer URL — é sempre o último recurso (ver PropertyImportService).
    public bool CanHandle(string url) => false;

    private static readonly string[] SampleTitles =
    {
        "Apartamento T3 com vista mar",
        "Moradia V4 com jardim e piscina",
        "Apartamento T2 renovado no centro",
        "Penthouse T4 com terraço panorâmico",
        "Moradia T3 geminada com garagem",
    };

    private static readonly string[] SampleCities =
    {
        "Lisboa", "Porto", "Cascais", "Setúbal", "Sintra", "Almada",
    };

    private static readonly string[] SamplePropertyTypes =
    {
        "Apartamento", "Moradia", "Penthouse",
    };

    public Task<ImportedPropertyData> ImportFromUrlAsync(string url)
    {
        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return Task.FromResult(new ImportedPropertyData
            {
                Success = false,
                SourceUrl = url ?? string.Empty,
                SourceName = ProviderName,
                Warnings = { "URL inválido ou em falta." }
            });
        }

        // Seed determinístico a partir do URL: mesmo URL → mesmos dados sempre,
        // o que ajuda a testar e a demonstrar de forma consistente.
        var seed = url.GetHashCode();
        var random = new Random(seed);

        var title = SampleTitles[Math.Abs(seed) % SampleTitles.Length];
        var city = SampleCities[Math.Abs(seed / 7) % SampleCities.Length];
        var propertyType = SamplePropertyTypes[Math.Abs(seed / 13) % SamplePropertyTypes.Length];
        var price = random.Next(120, 850) * 1000m;
        var reference = $"DEMO-{Math.Abs(seed) % 100000:D5}";

        var data = new ImportedPropertyData
        {
            Success = true,
            SourceUrl = url,
            SourceName = ProviderName,
            ExternalReferenceId = reference,
            Title = title,
            Description = $"Imóvel importado automaticamente a partir de {uri.Host} (dados de demonstração). " +
                            "Reveja e edite todos os campos antes de publicar.",
            Price = price,
            Currency = "EUR",
            City = city,
            Country = "Portugal",
            PropertyTypeName = propertyType,
            ImageUrls =
            {
                "https://images.unsplash.com/photo-1568605114967-8130f3a36994?auto=format&fit=crop&w=1200&q=80",
                "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?auto=format&fit=crop&w=1200&q=80",
            },
            Warnings =
            {
                "Dados de demonstração — este URL ainda não tem uma integração real associada.",
                "Confirme e edite todos os campos antes de publicar este imóvel."
            }
        };

        return Task.FromResult(data);
    }
}