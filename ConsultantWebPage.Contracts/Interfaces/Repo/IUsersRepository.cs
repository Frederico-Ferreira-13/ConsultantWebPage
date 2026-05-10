using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IUsersRepository : IGenericRepository<Users>
{    
    Task<Result<Users>> GetByEmailAsync(string email);
    Task<Result<Users>> GetByUserNameAsync(string userName);
    Task<Result<IEnumerable<Users>>> FindAsync(Expression<Func<Users, bool>> predicate);    
    Task<bool> EmailExistsAsync(string email);
}
