using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class UserRoleRepository : GenericRepository<UsersRole>, IUsersRoleRepository
{
    public UserRoleRepository(ApplicationDbContext context) : base(context)
    {        
    }

    public async Task<Result<IEnumerable<UsersRole>>> GetUsersByRoleIdAsync(int roleId)
    {
        var usersInRole = await _context.UsersRoles
            .Where(ur => ur.UsersRoleId == roleId)
            .ToListAsync();
        return Result<IEnumerable<UsersRole>>.Success(usersInRole);
    }

    public async Task<Result<UsersRole>> GetByNameAsync(string roleName)
    {
        var role = await _context.UsersRoles
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
        if (role == null)
            return Result<UsersRole>.Failure(Error.NotFound("Função não encontrada."));
        return Result<UsersRole>.Success(role);
    }

    public async Task<bool> IsRoleInUseAsync(int roleId)
    {
        var isInUse = await _context.UsersRoles
            .AnyAsync(ur => ur.UsersRoleId == roleId);
        return isInUse;
    }
}
