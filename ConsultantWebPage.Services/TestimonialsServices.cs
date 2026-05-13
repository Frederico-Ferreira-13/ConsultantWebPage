using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class TestimonialsServices : ITestimonialsServices
{
    private readonly IUnitOfWork _unitOfWork;

    public TestimonialsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetApprovedTestimonialsAsync()
    {
        // Exemplo de lógica: filtrar apenas os que estão aprovados
        var all = await _unitOfWork.Testimonials.GetAllAsync();
        if (!all.IsSuccess) return all;

        return Result<IEnumerable<Testimonials>>.Success(all.Value.Where(t => t.IsApproved));
    }
}