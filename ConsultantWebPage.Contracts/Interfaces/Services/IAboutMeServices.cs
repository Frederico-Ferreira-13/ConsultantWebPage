using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IAboutMeServices
{
    Task<Result<AboutMe>> GetByUserIdAsync(int userId);
}