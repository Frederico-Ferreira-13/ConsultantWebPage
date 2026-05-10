using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ICommentsRepository : IGenericRepository<Comments>
{
    Task<Result<IEnumerable<Comments>>> GetCommentsByRealEstateIdAsync(int realEstateId);

    Task<Result<IEnumerable<Comments>>> GetCommentsByPostIdAsync(int postId);

    Task<Result<IEnumerable<Comments>>> GetPendingCommentsAsync();
}
