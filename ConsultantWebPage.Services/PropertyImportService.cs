using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Models.DTOs;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class PropertyImportService : IPropertyImportService
{
    private readonly IEnumerable<IPropertyImportProvider> _providers;

    public PropertyImportService(IEnumerable<IPropertyImportProvider> providers)
    {
        _providers = providers;
    }

    public async Task<ImportedPropertyData> ImportFromUrlAsync(string url)
    {
        var provider = _providers.FirstOrDefault(p => p.CanHandle(url));

        if (provider == null)
        {
            // Nenhum provider especializado reconheceu o domínio.
            // Cai no Mock, para a funcionalidade nunca falhar "a seco".
            provider = _providers.FirstOrDefault(p => p.ProviderName == "Mock/Demo");
        }

        if (provider == null)
        {
            return new ImportedPropertyData
            {
                Success = false,
                SourceUrl = url,
                Warnings = { "Nenhum provider de importação disponível para este URL." }
            };
        }

        return await provider.ImportFromUrlAsync(url);
    }
}