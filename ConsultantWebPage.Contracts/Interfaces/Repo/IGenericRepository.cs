using System;
using System.Collections.Generic;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IGenericRepository<TEntity> where TEntity : class, IEntity
{    
    Task<Result<TEntity>> GetByIdAsync(int id);
    Task<Result<IEnumerable<TEntity>>> GetAllAsync();
    Task<Result> AddAsync(TEntity entity);
    Task<Result> UpdateAsync(TEntity entity);
    Task<Result> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
