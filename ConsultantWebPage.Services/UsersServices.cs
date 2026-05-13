using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class UsersServices : IUsersServices
{
    private readonly IUnitOfWork _unitOfWork;

    public UsersServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Users>> GetUserProfileAsync(int userId)
    {
        return await _unitOfWork.Users.GetByIdAsync(userId);
    }
}