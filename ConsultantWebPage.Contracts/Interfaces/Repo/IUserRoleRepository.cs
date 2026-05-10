using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUsersRoleRepository : IGenericRepository<UsersRole>
{
    Task<Result<IEnumerable<UsersRole>>> GetUsersByRoleIdAsync(int roleId);

    Task<Result<UsersRole>> GetByNameAsync(string roleName);

    Task<bool> IsRoleInUseAsync(int roleId);
}
