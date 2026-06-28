using System.Threading.Tasks;
using ConsultantWebPage.Core.Models.DTOs;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

 public interface IPropertyImportProvider
{
    /// <summary>
    /// Nome do provider, usado para o identificar nos logs e na UI
    /// (ex: "RE/MAX", "Century 21", "Mock/Demo").
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Indica se este provider sabe processar o URL fornecido
    /// (normalmente verifica o domínio do URL).
    /// </summary>
    bool CanHandle(string url);

    /// <summary>
    /// Importa e normaliza os dados do imóvel a partir do URL.
    /// Nunca deve lançar exceção por dados malformados — em vez disso,
    /// devolve ImportedPropertyData com Success = false e Warnings preenchido.
    /// </summary>
    Task<ImportedPropertyData> ImportFromUrlAsync(string url);
}