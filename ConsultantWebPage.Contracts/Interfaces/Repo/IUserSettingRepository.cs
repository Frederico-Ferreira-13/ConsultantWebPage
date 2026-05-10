using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUserSettingRepository : IGenericRepository<UserSettings>
{    
    Task<Result<UserSettings>> GetByUserIdAsync(int userId);
    Task<Result> InitializeSettingsAsync(int userId);
}
