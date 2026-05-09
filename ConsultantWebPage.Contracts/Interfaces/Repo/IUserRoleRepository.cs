using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUsersRoleRepository : IGenericRepository<UsersRole>
{
    Task<IEnumerable<UsersRole>> GetUsersByRoleIdAsync(int roleId);

    Task<UsersRole?> GetByNameAsync(string roleName);
}
