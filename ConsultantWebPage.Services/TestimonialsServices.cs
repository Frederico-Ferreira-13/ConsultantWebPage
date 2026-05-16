using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class TestimonialsServices
{
    private readonly IUnitOfWork _unitOfWork;

    public TestimonialsServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> SubmitTestimonialAsync(Testimonials testimonial)
    {          


    }

    public async Task<Result<IEnumerable<Testimonials>>> GetApprovedTestimonialsAsync(int consultantId)
    {
        // Implementation here
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetAllTestimonialsForModerationAsync(int consultantId)
    {
        // Implementation here
    }

    public async Task<Result> DeleteTestimonialAsync(int testimonialId)
    {
        // Implementation here
    }

    public async Task<Result> UpdateVisibilityAsync(int testimonialId, bool isVisible)
    {
        // Implementation here
    }
}