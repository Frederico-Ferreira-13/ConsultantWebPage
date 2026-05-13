using System;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class UserSettingRepository : GenericRepository<UserSettings>, IUserSettingRepository
{
    public UserSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<UserSettings>> GetByUserIdAsync(int userId)
    {
        var userSettings = await _context.UserSettings
            .FirstOrDefaultAsync(us => us.UserId == userId);
        if (userSettings == null)
            return Result<UserSettings>.Failure(Error.NotFound("Configurações do usuário não encontradas."));
        return Result<UserSettings>.Success(userSettings);
    }

    public async Task<Result> InitializeSettingsAsync(int userId)
    {
        var existingSettings = await _context.UserSettings
            .AnyAsync(us => us.UserId == userId);
        
        if (existingSettings)
        {
            return Result.Success(); // Já inicializado, não fazemos nada
        }

        var defaultSettings = new UserSettings(userId, "Light", "pt-PT", false);
        await _context.UserSettings.AddAsync(defaultSettings);         

        return Result.Success();
    }
}
