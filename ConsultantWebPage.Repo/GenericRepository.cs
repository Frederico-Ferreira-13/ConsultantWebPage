using System;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ConsultantWebPage.Repo;

public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity>
    where TEntity : class, IEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    protected GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<Result<TEntity>> GetByIdAsync(int id)
    {        
        var entity = await _dbSet.FindAsync(id);

        if (entity == null)
            return Result<TEntity>.Failure(Error.NotFound($"Registo {typeof(TEntity).Name} com ID {id} não encontrado."));

        return Result<TEntity>.Success(entity);
    }

    public virtual async Task<Result<IEnumerable<TEntity>>> GetAllAsync()
    {
        var items = await _dbSet.AsNoTracking().ToListAsync();
        return Result<IEnumerable<TEntity>>.Success(items);
    }

    public virtual async Task<Result> AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public virtual async Task<Result> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public virtual async Task<Result> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null)
            return Result.Failure(Error.NotFound("Registo não encontrado para eliminação."));

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return Result.Success();
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }
}
