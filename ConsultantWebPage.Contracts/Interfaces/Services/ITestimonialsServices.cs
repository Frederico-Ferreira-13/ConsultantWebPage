using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ITestimonialsServices
{
    Task<Result> SubmitTestimonialAsync(Testimonials testimonial);
    Task<Result<IEnumerable<Testimonials>>> GetApprovedTestimonialsAsync(int consultantId);

    Task<Result<IEnumerable<Testimonials>>> GetAllTestimonialsForModerationAsync(int consultantId);
    Task<Result> DeleteTestimonialAsync(int testimonialId);Task<Result> UpdateVisibilityAsync(int testimonialId, bool isVisible);
}