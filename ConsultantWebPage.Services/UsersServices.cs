using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.Extensions.Configuration;

namespace ConsultantWebPage.Services;

public class UsersServices : IUsersServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IAuthenticationService _authenticationService;
    private readonly HashSet<string> _adminEmails;

    public UsersServices(
        IUnitOfWork unitOfWork,
        IPasswordHasherService passwordHasher,
        IAuthenticationService authenticationService,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));

        var adminEmailsSection = configuration.GetSection("AdminEmails");
        _adminEmails = new HashSet<string>(
            adminEmailsSection.GetChildren()
                .Select(child => child.Value?.Trim() ?? "")
                .Where(v => !string.IsNullOrWhiteSpace(v)),
            StringComparer.OrdinalIgnoreCase
        );
    }   

    public async Task<Result<Users>> RegisterUserAsync(string userName, string name, string email, string password)
    {
        // Validações de entrada
        if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3 || userName.Length > 50)
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Nome de utilizador inválido (3-50 caracteres).",
                    new Dictionary<string, string[]> { { nameof(userName), new[] { "Mínimo 3, máximo 50 caracteres" } } }
                )
            );
        }

        if (string.IsNullOrWhiteSpace(name) || name.Length < 2 || name.Length > 100)
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Nome inválido (2-100 caracteres).",
                    new Dictionary<string, string[]> { { nameof(name), new[] { "Mínimo 2, máximo 100 caracteres" } } }
                )
            );
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || email.Length > 100)
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Email inválido.",
                    new Dictionary<string, string[]> { { nameof(email), new[] { "Formato de email inválido" } } }
                )
            );
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Password inválida (mínimo 8 caracteres).",
                    new Dictionary<string, string[]> { { nameof(password), new[] { "Mínimo 8 caracteres" } } }
                )
            );
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        // Verificar duplicados
        var existingUserName = await _unitOfWork.Users.GetByUserNameAsync(userName);
        if (existingUserName.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.Conflict(
                    ErrorCodes.AlreadyExists,
                    "Este nome de utilizador já está em uso.",
                    new Dictionary<string, string[]> { { nameof(userName), new[] { "Escolha outro nome de utilizador" } } }
                )
            );
        }

        var existingEmail = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
        if (existingEmail.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists, "Este email já está registado.")
            );
        }

        // Criar ContactInfo (Value Object)
        var contactInfoResult = ContactInfo.Create(normalizedEmail, "000000000");
        if (!contactInfoResult.IsSuccessful)
            return Result<Users>.Failure(contactInfoResult.Error);

        // Hash da password
        var salt = _passwordHasher.GenerateSalt();
        var hashResult = _passwordHasher.HashPassword(password, salt);
        if (!hashResult.IsSuccessful)
            return Result<Users>.Failure(hashResult.Error);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            bool isAdminEmail = _adminEmails.Contains(normalizedEmail);

            // Criar utilizador via factory method
            var userResult = Users.Create(
                name: name,
                userName: userName,
                contact: contactInfoResult.Value,
                usersRoleId: isAdminEmail ? 1 : 2,
                isApproved: isAdminEmail
            );

            if (!userResult.IsSuccessful)
                return Result<Users>.Failure(userResult.Error);

            var user = userResult.Value;
            user.SetPassword(hashResult.Value.Hash, salt);

            var addResult = await _unitOfWork.Users.AddAsync(user);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<Users>.Failure(
                    Error.InternalServer
                    ($"Erro ao adicionar utilizador: {addResult.Message}"));
            }

            await _unitOfWork.CommitAsync();

            // Criar settings padrão numa transação separada (o utilizador já tem ID após commit)
            await CreateDefaultSettingsAsync(user.UserId);

            return Result<Users>.Success(user);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();

            if (ex.Message.Contains("UNIQUE") || ex.Message.Contains("duplicate key"))
            {
                return Result<Users>.Failure(
                    Error.Conflict(ErrorCodes.AlreadyExists, "Este email ou nome de utilizador já existe.")
                );
            }

            return Result<Users>.Failure(
                Error.InternalServer($"Erro ao registar utilizador: {ex.Message}")
            );
        }
    }

    // -------------------------------------------------------------------------
    // AUTENTICAÇÃO
    // -------------------------------------------------------------------------

    public async Task<Result<Users>> AuthenticateUserAsync(string identifier, string password)
    {
        var authResult = await _authenticationService.AuthenticateAsync(identifier, password);
        if (!authResult.IsSuccessful)
            return Result<Users>.Failure(authResult.Error);

        var userResult = await _unitOfWork.Users.GetByIdAsync(authResult.Value!.UserId);
        if (!userResult.IsSuccessful || !userResult.Value.IsActive)
        {
            return Result<Users>.Failure(
                Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Conta inativa ou não encontrada.")
            );
        }

        return Result<Users>.Success(userResult.Value);
    }    

    public async Task<Result<Users>> GetUserByIdAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "ID do utilizador inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }
                )
            );
        }

        var result = await _unitOfWork.Users.GetByIdAsync(userId);
        if (!result.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Utilizador com ID {userId} não encontrado.")
            );
        }

        return Result<Users>.Success(result.Value);
    }

    public async Task<Result<Users>> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Email inválido.",
                    new Dictionary<string, string[]> { { nameof(email), new[] { "Campo obrigatório" } } }
                )
            );
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();
        var result = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
        if (!result.IsSuccessful)
        {
            return Result<Users>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Utilizador com email '{email}' não encontrado.")
            );
        }

        return Result<Users>.Success(result.Value);
    }

    public async Task<Result<Users>> GetUserByUsernameOrEmailAsync(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result<Users>.Failure(
                Error.Validation(
                    "Identificador inválido.",
                    new Dictionary<string, string[]> { { nameof(identifier), new[] { "Campo obrigatório" } } }
                )
            );
        }

        var normalized = identifier.Trim().ToLowerInvariant();

        var byEmail = await _unitOfWork.Users.GetByEmailAsync(normalized);
        if (byEmail.IsSuccessful)
            return Result<Users>.Success(byEmail.Value);

        var byUserName = await _unitOfWork.Users.GetByUserNameAsync(normalized);
        if (byUserName.IsSuccessful)
            return Result<Users>.Success(byUserName.Value);

        return Result<Users>.Failure(
            Error.NotFound(ErrorCodes.NotFound, $"Utilizador com identificador '{identifier}' não encontrado.")
        );
    }

    public async Task<Result<Users>> GetCurrentUserAsync()
    {
        var userResult = await _authenticationService.GetPersistedUserAsync();
        if (!userResult.IsSuccessful)
            return Result<Users>.Failure(userResult.Error);

        if (userResult.Value == null)
        {
            return Result<Users>.Failure(
                Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Utilizador autenticado não encontrado ou expirado.")
            );
        }

        return Result<Users>.Success(userResult.Value);
    }

    public async Task<Result<int>> GetCurrentUserIdAsync()
    {
        return await _authenticationService.GetCurrentUserIdAsync();
    }    

    public async Task<bool> UserExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var normalized = email.Trim().ToLowerInvariant();
        var result = await _unitOfWork.Users.GetByEmailAsync(normalized);
        return result.IsSuccessful;
    }

    public async Task<bool> IsUserNameUniqueAsync(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName)) return false;
        var result = await _unitOfWork.Users.GetByUserNameAsync(userName);
        return !result.IsSuccessful;
    }

    public async Task<bool> UserExistsAsync(int userId)
    {
        if (userId <= 0) return false;
        return await _unitOfWork.Users.ExistsAsync(userId);
    }  

    public async Task<Result> UpdateUserProfileAsync(Users userToUpdate)
    {
        var existingResult = await _unitOfWork.Users.GetByIdAsync(userToUpdate.UserId);
        if (!existingResult.IsSuccessful)
        {
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Utilizador com ID {userToUpdate.UserId} não encontrado.")
            );
        }

        var existingUser = existingResult.Value;

        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful || currentUserIdResult.Value != userToUpdate.UserId)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas o próprio utilizador pode atualizar o seu perfil.")
            );
        }

        // Verificar username duplicado
        if (!string.IsNullOrWhiteSpace(userToUpdate.UserName) && userToUpdate.UserName != existingUser.UserName)
        {
            var existingUserName = await _unitOfWork.Users.GetByUserNameAsync(userToUpdate.UserName);
            if (existingUserName.IsSuccessful)
            {
                return Result.Failure(
                    Error.Conflict(
                        ErrorCodes.AlreadyExists,
                        "Este nome de utilizador já está em uso.",
                        new Dictionary<string, string[]> { { nameof(userToUpdate.UserName), new[] { "Escolha outro" } } }
                    )
                );
            }
        }

        // Verificar email duplicado
        if (userToUpdate.Contact != null &&
            !string.IsNullOrWhiteSpace(userToUpdate.Contact.Email) &&
            userToUpdate.Contact.Email != existingUser.Contact?.Email)
        {
            var normalizedEmail = userToUpdate.Contact.Email.Trim().ToLowerInvariant();
            var existingEmail = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
            if (existingEmail.IsSuccessful)
            {
                return Result.Failure(
                    Error.Conflict(
                        ErrorCodes.AlreadyExists,
                        "Este email já está registado.",
                        new Dictionary<string, string[]> { { "Email", new[] { "Use outro email" } } }
                    )
                );
            }
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (!string.IsNullOrWhiteSpace(userToUpdate.ProfilePicture))
            {
                existingUser.UpdateProfilePicture(userToUpdate.ProfilePicture);
            }

            if (!string.IsNullOrWhiteSpace(userToUpdate.UserName))
            {
                existingUser.UpdateUserName(userToUpdate.UserName);
            }

            if (userToUpdate.Contact != null)
            {
                var updateContactResult = existingUser.UpdateContact(userToUpdate.Contact);
                if (!updateContactResult.IsSuccessful)
                {
                    _unitOfWork.Rollback();
                    return Result.Failure(Error.InternalServer($"Erro ao atualizar contacto: {updateContactResult.Message}"));
                }
            }

            await _unitOfWork.Users.UpdateAsync(existingUser);
            await _unitOfWork.CommitAsync();

            return Result.Success("Perfil atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao atualizar perfil: {ex.Message}"));
        }
    }

    

    public async Task<Result> ChangeUserPasswordAsync(int userId, string oldPassword, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            return Result.Failure(
                Error.Validation(
                    "A password antiga e a nova password não podem ser vazias.",
                    new Dictionary<string, string[]> { { "Password", new[] { "Passwords não podem ser nulas." } } }
                )
            );
        }

        if (newPassword.Length < 8)
        {
            return Result.Failure(
                Error.Validation(
                    "A nova password deve ter pelo menos 8 caracteres.",
                    new Dictionary<string, string[]> { { nameof(newPassword), new[] { "Mínimo 8 caracteres" } } }
                )
            );
        }

        var userResult = await _unitOfWork.Users.GetByIdAsync(userId);
        if (!userResult.IsSuccessful || !userResult.Value.IsActive)
        {
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, "Utilizador não encontrado ou inativo.")
            );
        }

        var user = userResult.Value;

        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful || currentUserIdResult.Value != userId)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas o próprio utilizador pode alterar a sua password.")
            );
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, oldPassword, user.Salt))
        {
            return Result.Failure(Error.Validation("Password antiga incorreta."));
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var newSalt = _passwordHasher.GenerateSalt();
            var hashResult = _passwordHasher.HashPassword(newPassword, newSalt);
            if (!hashResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(hashResult.Error);
            }

            user.SetPassword(hashResult.Value.Hash, newSalt);
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result.Success("Password alterada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao alterar password: {ex.Message}"));
        }
    }    

    public async Task<Result> DeactivateUserAsync(int userId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful || currentUserIdResult.Value != userId)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas o próprio utilizador pode desativar a sua conta.")
            );
        }

        var userResult = await _unitOfWork.Users.GetByIdAsync(userId);
        if (!userResult.IsSuccessful)
            return Result.Success("Utilizador não encontrado.");

        var user = userResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            user.Deactivate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result.Success("Utilizador desativado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao desativar utilizador: {ex.Message}"));
        }
    }

    public async Task<Result> ActivateUserAsync(int userId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Result.Failure(Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Não autenticado."));

        var currentUserResult = await _unitOfWork.Users.GetByIdAsync(currentUserIdResult.Value);
        if (!currentUserResult.IsSuccessful || currentUserResult.Value.UsersRoleId != 1)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas administradores podem ativar contas.")
            );
        }

        var userResult = await _unitOfWork.Users.GetByIdAsync(userId);
        if (!userResult.IsSuccessful)
            return Result.Success("Utilizador não encontrado (idempotente).");

        var user = userResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            user.Activate();
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result.Success($"Utilizador {userId} ativado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao ativar utilizador: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful)
            return Result.Failure(Error.Unauthorized(ErrorCodes.AuthUnauthorized, "Não autenticado."));

        var currentUserResult = await _unitOfWork.Users.GetByIdAsync(currentUserIdResult.Value);
        if (!currentUserResult.IsSuccessful ||
            (currentUserIdResult.Value != userId && currentUserResult.Value.UsersRoleId != 1))
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas o próprio utilizador ou administradores podem eliminar contas.")
            );
        }

        var userResult = await _unitOfWork.Users.GetByIdAsync(userId);
        if (!userResult.IsSuccessful)
            return Result.Success("Utilizador não encontrado ou já eliminado.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.Users.DeleteAsync(userId);
            await _unitOfWork.CommitAsync();

            return Result.Success("Utilizador eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao eliminar utilizador: {ex.Message}"));
        }
    }

    // -------------------------------------------------------------------------
    // SETTINGS
    // -------------------------------------------------------------------------

    public async Task<Result<UserSettings>> GetSettingsByUserIdAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<UserSettings>.Failure(
                Error.Validation(
                    "ID do utilizador inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }
                )
            );
        }

        var result = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if (!result.IsSuccessful)
        {
            return Result<UserSettings>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Configurações para o utilizador com ID {userId} não foram encontradas.")
            );
        }

        return Result<UserSettings>.Success(result.Value);
    }

    public async Task<Result> UpdateUserSettingsAsync(UserSettings settings)
    {
        if (settings == null || settings.UserId <= 0)
        {
            return Result.Failure(
                Error.Validation("O ID do utilizador é obrigatório para atualizar as configurações.")
            );
        }

        var currentUserIdResult = await GetCurrentUserIdAsync();
        if (!currentUserIdResult.IsSuccessful || currentUserIdResult.Value != settings.UserId)
        {
            return Result.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Apenas o próprio utilizador pode atualizar as suas configurações.")
            );
        }

        var existingResult = await _unitOfWork.UserSettings.GetByUserIdAsync(settings.UserId);
        if (!existingResult.IsSuccessful)
        {
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Configurações para o utilizador com ID {settings.UserId} não encontradas.")
            );
        }

        var existingSettings = existingResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            existingSettings.UpdateSettings(settings.Theme, settings.Language, settings.ReceiveNotifications);
            await _unitOfWork.UserSettings.UpdateAsync(existingSettings);
            await _unitOfWork.CommitAsync();

            return Result.Success("Configurações atualizadas com sucesso.");
        }
        catch (ArgumentException ex)
        {
            _unitOfWork.Rollback();
            string fieldName = ex.ParamName ?? "Geral";
            return Result.Failure(
                Error.Validation(
                    "Dados de entrada inválidos para atualizar as configurações.",
                    new Dictionary<string, string[]> { { fieldName, new[] { ex.Message } } }
                )
            );
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao atualizar configurações: {ex.Message}"));
        }
    }

    public async Task<Result<UserSettings>> CreateDefaultSettingsAsync(int userId)
    {
        if (userId <= 0)
        {
            return Result<UserSettings>.Failure(
                Error.Validation(
                    "ID do utilizador inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }
                )
            );
        }

        var userExists = await _unitOfWork.Users.ExistsAsync(userId);
        if (!userExists)
        {
            return Result<UserSettings>.Failure(
                Error.BusinessRuleViolation(ErrorCodes.BizInvalidOperation, $"Utilizador com ID {userId} não existe.")
            );
        }

        var existing = await _unitOfWork.UserSettings.GetByUserIdAsync(userId);
        if (existing.IsSuccessful)
        {
            return Result<UserSettings>.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists, $"Configurações para o utilizador {userId} já existem.")
            );
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var defaultSettings = new UserSettings(
                userId: userId,
                theme: "Light",
                language: "pt-PT",
                receiveNotifications: true
            );

            await _unitOfWork.UserSettings.AddAsync(defaultSettings);
            await _unitOfWork.CommitAsync();

            return Result<UserSettings>.Success(defaultSettings);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<UserSettings>.Failure(
                Error.InternalServer($"Erro ao criar configurações padrão: {ex.Message}")
            );
        }
    }

    // -------------------------------------------------------------------------
    // ROLES
    // -------------------------------------------------------------------------

    public async Task<Result<UsersRole>> CreateUsersRoleAsync(UsersRole dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RoleName) || dto.RoleName.Length > 50)
        {
            return Result<UsersRole>.Failure(
                Error.Validation(
                    "Nome do nível de acesso inválido (obrigatório, máximo 50 caracteres).",
                    new Dictionary<string, string[]> { { nameof(dto.RoleName), new[] { "Campo inválido" } } }
                )
            );
        }

        var existingRole = await _unitOfWork.UsersRole.GetByNameAsync(dto.RoleName);
        if (existingRole.IsSuccessful)
        {
            return Result<UsersRole>.Failure(
                Error.Validation(
                    $"O Nível de Acesso '{dto.RoleName}' já existe.",
                    new Dictionary<string, string[]> { { nameof(dto.RoleName), new[] { "Nome já em uso." } } }
                )
            );
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var newRole = new UsersRole(dto.RoleName);
            await _unitOfWork.UsersRole.AddAsync(newRole);
            await _unitOfWork.CommitAsync();

            return Result<UsersRole>.Success(newRole);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<UsersRole>.Failure(
                Error.InternalServer($"Erro ao criar nível de acesso: {ex.Message}")
            );
        }
    }

    public async Task<Result<UsersRole>> GetUsersRoleByIdAsync(int id)
    {
        if (id <= 0)
            return Result<UsersRole>.Failure(Error.Validation("ID do nível de acesso inválido."));

        var result = await _unitOfWork.UsersRole.GetByIdAsync(id);
        if (!result.IsSuccessful)
        {
            return Result<UsersRole>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Nível de Acesso com ID {id} não encontrado.")
            );
        }

        return Result<UsersRole>.Success(result.Value);
    }

    public async Task<Result<UsersRole>> GetUsersRoleByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<UsersRole>.Failure(Error.Validation("Nome do nível de acesso obrigatório."));

        var result = await _unitOfWork.UsersRole.GetByNameAsync(name);
        if (!result.IsSuccessful)
        {
            return Result<UsersRole>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Nível de Acesso com nome '{name}' não encontrado.")
            );
        }

        return Result<UsersRole>.Success(result.Value);
    }

    public async Task<Result<IEnumerable<UsersRole>>> GetAllUsersRolesAsync()
    {
        var result = await _unitOfWork.UsersRole.GetAllAsync();
        if (!result.IsSuccessful)
            return Result<IEnumerable<UsersRole>>.Failure(result.Error);

        return Result<IEnumerable<UsersRole>>.Success(result.Value);
    }

    public async Task<Result> UpdateUsersRoleAsync(UsersRole updateRole)
    {
        var existingResult = await _unitOfWork.UsersRole.GetByIdAsync(updateRole.UsersRoleId);
        if (!existingResult.IsSuccessful)
        {
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Nível de Acesso com ID {updateRole.UsersRoleId} não encontrado.")
            );
        }

        if (string.IsNullOrWhiteSpace(updateRole.RoleName) || updateRole.RoleName.Length > 50)
        {
            return Result.Failure(
                Error.Validation("Nome do nível de acesso inválido (obrigatório, máximo 50 caracteres).")
            );
        }

        var existingRole = existingResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            if (!existingRole.RoleName.Equals(updateRole.RoleName, StringComparison.Ordinal))
            {
                var duplicateResult = await _unitOfWork.UsersRole.GetByNameAsync(updateRole.RoleName);
                if (duplicateResult.IsSuccessful)
                {
                    _unitOfWork.Rollback();
                    return Result.Failure(
                        Error.Conflict(ErrorCodes.AlreadyExists, $"O nome '{updateRole.RoleName}' já está em uso.")
                    );
                }

                existingRole.UpdateName(updateRole.RoleName);
            }

            await _unitOfWork.UsersRole.UpdateAsync(existingRole);
            await _unitOfWork.CommitAsync();

            return Result.Success("Nível de Acesso atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao atualizar nível de acesso: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteUsersRoleAsync(int id)
    {
        var existingResult = await _unitOfWork.UsersRole.GetByIdAsync(id);
        if (!existingResult.IsSuccessful)
            return Result.Success($"Nível de Acesso com ID {id} não encontrado (idempotente).");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _unitOfWork.UsersRole.DeleteAsync(id);
            await _unitOfWork.CommitAsync();

            return Result.Success("Nível de Acesso eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(Error.InternalServer($"Erro ao eliminar nível de acesso: {ex.Message}"));
        }
    }

    // -------------------------------------------------------------------------
    // APROVAÇÃO / PENDENTES
    // -------------------------------------------------------------------------

    public async Task<Result<IEnumerable<Users>>> GetPendingUsersAsync()
    {
        try
        {
            var result = await _unitOfWork.Users.FindAsync(u => !u.IsApproved && u.IsActive);
            if (!result.IsSuccessful)
                return Result<IEnumerable<Users>>.Failure(result.Error);

            return Result<IEnumerable<Users>>.Success(result.Value);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<Users>>.Failure(Error.InternalServer(ex.Message));
        }
    }

    public async Task<Result<bool>> ApproveUserAsync(int userId)
    {
        try
        {
            var userResult = await _unitOfWork.Users.GetByIdAsync(userId);
            if (!userResult.IsSuccessful)
            {
                return Result<bool>.Failure(
                    Error.NotFound(ErrorCodes.NotFound, "Utilizador não encontrado.")
                );
            }

            var user = userResult.Value;
            user.Approve();

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(Error.InternalServer(ex.Message));
        }
    }
}