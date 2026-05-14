using System;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IAuthenticationService
{
    Task<Result<int>> GetCurrentUserIdAsync();
    Task<Result<Users>> AuthenticateAsync(string identifier, string password);
    Task LogoutAsync();
    Task<Result<Users>> GetPersistedUserAsync();
}
