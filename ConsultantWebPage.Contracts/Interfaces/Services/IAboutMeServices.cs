using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IAboutMeServices
{
    Task<Result<AboutMe>> GetByUserIdAsync(int userId);
    Task<Result<AboutMe>> SaveAboutMeAsync(int userId, AboutMe aboutMe);
    Task<Result<AboutMe>> CreateAboutMeAsync(AboutMe aboutMe);
    Task<Result<AboutMe>> UpdateAboutMeAsync(AboutMe aboutMe);
    Task<Result<bool>> HasProfileAsync(int userId);
}