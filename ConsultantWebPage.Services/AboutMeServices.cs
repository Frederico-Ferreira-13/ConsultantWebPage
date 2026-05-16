using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class AboutMeServices : IAboutMeServices
{
    private readonly IUnitOfWork _unitOfWork;

    public AboutMeServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AboutMe>> GetByUserIdAsync(int userId)
    {
        if(userId <= 0)
        {
            return Error.Validation("O ID do utilizador deve ser um inteiro positivo.");
        }

        var repositoryResult = await _unitOfWork.AboutMe.GetByUserIdAsync(userId);
        if (repositoryResult.IsFailure || repositoryResult.Value == null)
        {
            return Error.NotFound("PROFILE_NOT_FOUND", "Perfil 'About Me' não encontrado para o ID do utilizador fornecido.");
        }

        return Result<AboutMe>.Success(repositoryResult.Value);
    }    
    public async Task<Result<AboutMe>> SaveAboutMeAsync(int userId, AboutMe aboutMe)
    {
        var hasProfileResult = await _unitOfWork.AboutMe.GetAllByUserIdAsync(userId);

        if (hasProfileResult.IsSuccessful && hasProfileResult.Value != null && hasProfileResult.Value.Any())
        {
            return await UpdateAboutMeAsync(aboutMe);
        }
        else
        {
            return await CreateAboutMeAsync(aboutMe);
        }
    }
    public async Task<Result<AboutMe>> CreateAboutMeAsync(AboutMe aboutMe)
    {
        var existingProfileResult = await _unitOfWork.AboutMe.GetAllByUserIdAsync(aboutMe.UserId);
        if (existingProfileResult.IsSuccessful && existingProfileResult.Value != null && existingProfileResult.Value.Any())
        {
            return Error.Conflict("PROFILE_ALREADY_EXISTS", "Este utilizador já possui um perfil 'Sobre Mim' configurado.");
        }

        var userExists = await _unitOfWork.Users.GetByIdAsync(aboutMe.UserId); // Assumindo que o teu repositório de Users tem GetByIdAsync
        if (userExists == null)
        {
            return Error.NotFound("USER_NOT_FOUND", "Não é possível criar um perfil para um utilizador inexistente.");
        }

        var repoResult = await _unitOfWork.AboutMe.AddAsync(aboutMe);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível registar o perfil no repositório.");
        }

        await _unitOfWork.CommitAsync();
        return Result<AboutMe>.Success(aboutMe, "Perfil criado com sucesso.");
    }

    public async Task<Result<AboutMe>> UpdateAboutMeAsync(AboutMe aboutMe)
    {
        var repositoryResult = await _unitOfWork.AboutMe.GetByUserIdAsync(aboutMe.UserId);        
        if (repositoryResult.IsFailure || repositoryResult.Value == null)
        {
            return Error.NotFound("PROFILE_NOT_FOUND", "Não foi encontrado nenhum perfil para atualizar.");
        }

        var existingProfile = repositoryResult.Value;
        
        existingProfile.UpdateAboutMe(
            aboutMe.Bio,
            aboutMe.ProfileImageUrl,
            aboutMe.YearsOfExperience,
            aboutMe.SocialUrl_Linkedin,
            aboutMe.SocialUrl_Instagram,
            aboutMe.SocialUrl_X,
            aboutMe.SocialUrl_Facebook
        );

        var repoResult = await _unitOfWork.AboutMe.UpdateAsync(existingProfile);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível atualizar o perfil no repositório.");
        }

        await _unitOfWork.CommitAsync();
        return Result<AboutMe>.Success(existingProfile, "Perfil atualizado com sucesso.");
    }

    public async Task<Result<bool>> HasProfileAsync(int userId)
    {
       if (userId <= 0)
            return Error.Validation("O ID do utilizador é inválido.");

        var hasProfileResult = await _unitOfWork.AboutMe.GetAllByUserIdAsync(userId);
        
        bool hasProfile = hasProfileResult.IsSuccessful && hasProfileResult.Value != null && hasProfileResult.Value.Any();
        return Result<bool>.Success(hasProfile);
    }
}