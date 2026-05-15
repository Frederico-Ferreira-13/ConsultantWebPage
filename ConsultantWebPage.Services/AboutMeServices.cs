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
        // Aqui podias adicionar lógica de negócio (ex: verificar permissões)
        // Por agora, chamamos o repositório que o teu colega fez
        return await _unitOfWork.AboutMe.GetByUserIdAsync(userId);
    }    
    public async Task<Result<AboutMe>> SaveAboutMeAsync(int userId, AboutMe aboutMe)
    {
        // Lógica de negócio para salvar o perfil
        return await _unitOfWork.AboutMe.SaveAboutMeAsync(userId, aboutMe);
    }
    public async Task<Result<AboutMe>> CreateAboutMeAsync(AboutMe aboutMe)
    {
        // Lógica de negócio para criar o perfil
        return await _unitOfWork.AboutMe.CreateAboutMeAsync(aboutMe);
    }
    public async Task<Result<AboutMe>> UpdateAboutMeAsync(AboutMe aboutMe)
    {
        // Lógica de negócio para atualizar o perfil
        return await _unitOfWork.AboutMe.UpdateAboutMeAsync(aboutMe);
    }
    public async Task<Result<bool>> HasProfileAsync(int userId)
    {
        // Lógica de negócio para verificar se o usuário tem um perfil
        return await _unitOfWork.AboutMe.HasProfileAsync(userId);
    }
}