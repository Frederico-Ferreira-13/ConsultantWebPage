using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using Microsoft.EntityFrameworkCore;

namespace ConsultantWebPage.Repo;

public class TestimonialsRepository : GenericRepository<Testimonials>, ITestimonialsRepository
{
    public TestimonialsRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetPublishedAsync()
    {
        var publishedTestimonials = await _context.Testimonials
            .Where(t => t.IsVisible)
            .ToListAsync();

        return Result<IEnumerable<Testimonials>>.Success(publishedTestimonials);
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetFeaturedAsync(int count)
    {
        var featuredTestimonials = await _context.Testimonials
            .Where(t => t.IsVisible)
            .Take(count)
            .ToListAsync();

        return Result<IEnumerable<Testimonials>>.Success(featuredTestimonials);
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetPendingApprovalAsync()
    {
        var pendingApprovalTestimonials = await _context.Testimonials
            .Where(t => !t.IsVisible)
            .ToListAsync();

        return Result<IEnumerable<Testimonials>>.Success(pendingApprovalTestimonials);
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetByRealEstateIdAsync(int realEstateId)
    {
        var testimonials = await _context.Testimonials
            .Where(t => t.TestimonialId == realEstateId)
            .ToListAsync();

        return Result<IEnumerable<Testimonials>>.Success(testimonials);
    }
}
