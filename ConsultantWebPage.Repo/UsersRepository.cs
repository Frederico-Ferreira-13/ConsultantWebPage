using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class UsersRepository : GenericRepository<Users>, IUsersRepository
{
    public UsersRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<Users>> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Contact.Email == email);

        if (user == null)
            return Result<Users>.Failure(Error.NotFound("Utilizador não encontrado com este email."));

        return Result<Users>.Success(user);
    }

    public async Task<Result<Users>> GetByUserNameAsync(string userName)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName);
        
        if (user == null)
            return Result<Users>.Failure(Error.NotFound("Utilizador não encontrado."));

        return Result<Users>.Success(user);
    }
    public async Task<Result<IEnumerable<Users>>> FindAsync(Expression<Func<Users, bool>> predicate)
    {
        var users = await _context.Users
            .Where(predicate)
            .ToListAsync();

        return Result<IEnumerable<Users>>.Success(users);
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Contact.Email == email);
    }
}
