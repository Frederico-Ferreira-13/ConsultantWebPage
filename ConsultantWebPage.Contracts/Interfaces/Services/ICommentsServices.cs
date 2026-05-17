using System;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICommentsServices
{
    Task<Result<Comments>> GetCommentsByIdAsync(int id);
    Task<Result<IEnumerable<Comments>>> GetCommentsByRealEstateIdAsync(int realEstateId);
    Task<Result<Comments>> CreateCommentAsync(Comments newComment);
    Task<Result> UpdateCommentAsync(int id, Comments updateComment);
    Task<Result> DeleteCommentAsync(int id);
}
