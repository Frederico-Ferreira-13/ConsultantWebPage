using System;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface IGenericRepository<TEntity> where TEntity : class, IEntity
{
    Task CreateAddAssync(TEntity entity);
    Task<TEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TEntity>> GetAllAsync(); 
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
}
