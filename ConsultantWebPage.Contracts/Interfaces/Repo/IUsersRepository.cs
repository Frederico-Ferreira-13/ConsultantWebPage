using System;
using ConsultantWebPage.Core.Models;
using System.Linq.Expressions;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUsersRepository : IGenericRepository<Users>
{
    
    Task<Users?> GetByEmailAsync(string email);
    Task<Users?> GetByUserNameAsync(string userName);

    Task<IEnumerable<Users>> FindAsync(Expression<Func<Users, bool>> predicate);
}
