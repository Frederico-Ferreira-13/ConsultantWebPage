using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IUsersServices
{
    Task<Result<Users>> GetUserProfileAsync(int userId);
}