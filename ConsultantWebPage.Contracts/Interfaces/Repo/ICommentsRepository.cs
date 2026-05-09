using System;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICommentsRepository : IGenericRepository<Comments>
{
    Task<List<Comments>> GetCommentsByRecipeIdAsync(int recipeId);
}
