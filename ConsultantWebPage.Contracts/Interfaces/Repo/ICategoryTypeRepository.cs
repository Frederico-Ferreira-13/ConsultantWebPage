using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICategoryTypeRepository : IGenericRepository<CategoryType>
{
    Task<CategoryType?> GetByNameAsync(string TypeName);
}
