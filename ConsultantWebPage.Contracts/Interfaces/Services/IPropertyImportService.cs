using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Models.DTOs;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPropertyImportService
{
    Task<ImportedPropertyData> ImportFromUrlAsync(string url);
}