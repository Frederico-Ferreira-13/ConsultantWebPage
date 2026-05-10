using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICategoryTypeRepository : IGenericRepository<CategoryType>
{
    Task<Result<CategoryType>> GetByNameAsync(string TypeName);
}
