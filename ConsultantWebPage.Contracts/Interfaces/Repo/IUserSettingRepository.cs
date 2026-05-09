using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUserSettingRepository : IGenericRepository<UserSettings>
{    
    Task<UserSettings?> GetByUserIdAsync(int userId);
}
