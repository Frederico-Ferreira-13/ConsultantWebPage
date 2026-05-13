using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ITestimonialsServices
{
    Task<Result<IEnumerable<Testimonials>>> GetApprovedTestimonialsAsync();
}