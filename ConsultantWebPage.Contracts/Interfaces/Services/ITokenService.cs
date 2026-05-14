using System;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ITokenService
{
    TokenResponse GenerateToken(Users user);
    Task<Result<int>> GetUserIdFromContextAsync();
    Task InvalidateTokenAsync();
}
