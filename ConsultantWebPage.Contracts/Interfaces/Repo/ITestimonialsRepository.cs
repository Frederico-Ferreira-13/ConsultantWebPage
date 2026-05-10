using System.Collections.Generic;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Repo;

public interface ITestimonialsRepository : IGenericRepository<Testimonials>
{
    Task<Result<IEnumerable<Testimonials>>> GetPublishedAsync();

    Task<Result<IEnumerable<Testimonials>>> GetFeaturedAsync(int count);

    Task<Result<IEnumerable<Testimonials>>> GetPendingApprovalAsync();

    Task<Result<IEnumerable<Testimonials>>> GetByRealEstateIdAsync(int realEstateId);
}