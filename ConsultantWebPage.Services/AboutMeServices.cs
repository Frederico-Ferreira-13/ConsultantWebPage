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
}