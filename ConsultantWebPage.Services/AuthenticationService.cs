using System;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthenticationService(IUnitOfWork unitOfWork, IPasswordHasherService passwordHasher, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    public async Task<Result<int>> GetCurrentUserIdAsync()
    {
        return await _tokenService.GetUserIdFromContextAsync();
    }

    public async Task<Result<Users>> AuthenticateAsync(string identifier, string password)
    {
        var userResult = await _unitOfWork.Users.GetByEmailAsync(identifier);
        if (!userResult.IsSuccessful)
        {
            userResult = await _unitOfWork.Users.GetByUserNameAsync(identifier);
        }

        if (!userResult.IsSuccessful || !userResult.Value.IsActive)
        {
            return Result<Users>.Failure(
                Error.Unauthorized(
                    ErrorCodes.AuthFailed,
                    "Utilizador não encontrado ou conta inativa."));
        }

        var user = userResult.Value;

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, password, user.Salt))
        {
            return Result<Users>.Failure(
                Error.Unauthorized(
                    ErrorCodes.AuthFailed,
                    "Credenciais inválidas."));
        }

        return Result<Users>.Success(user);
    }

    public async Task LogoutAsync()
    {
        await _tokenService.InvalidateTokenAsync();
    }

    public async Task<Result<Users>> GetPersistedUserAsync()
    {
        var userIdResult = await GetCurrentUserIdAsync();

        if (!userIdResult.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.Unauthorized(
                userIdResult.ErrorCode ?? ErrorCodes.AuthUnauthorized,
                userIdResult.Message ?? "Não autenticado ou token inválido.")
            );
        }

        int userId = userIdResult.Value;

        if (userId <= 0)
        {
            return Result<Users>.Failure(
                Error.Unauthorized(
                ErrorCodes.AuthUnauthorized,
                "Não autenticado ou token inválido.")
            );
        }

        var userResult = await _unitOfWork.Users.GetByIdAsync(userId);

        if (!userResult.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.NotFound(
                    ErrorCodes.NotFound,
                    "O utilizador associado ao token não existe.")
            );
        }

        var user = userResult.Value;

        if (!user.IsActive)
        {
            return Result<Users>.Failure(
                Error.Forbidden(
                    ErrorCodes.AuthForbidden,
                    "A sua conta encontra-se inativa ou bloqueada.")
            );
        }

        return Result<Users>.Success(user);
    }
}
