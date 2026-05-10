using System.Threading.Tasks;
using System.Collections.Generic;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IAboutMeRepository : IGenericRepository<AboutMe>
{
    Task<Result<AboutMe>> GetByUserIdAsync(int userId);
    Task<Result<IEnumerable<AboutMe>>> GetAllByUserIdAsync(int userId);
    Task<Result> DeactivateAllForUserAsync(int userId);
}

