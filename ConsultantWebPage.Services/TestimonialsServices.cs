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
        if (testimonial == null)
        {
            return Error.Validation("O testemunho não pode ser nulo.");
        }
        var consultantExists = await _unitOfWork.Users.ExistsAsync(testimonial.ConsultantId);
        if (!consultantExists)
        {
            return Error.NotFound(ErrorCodes.NotFound, $"Consultor com ID {testimonial.ConsultantId} não encontrado.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addResult = await _unitOfWork.Testimonials.AddAsync(testimonial);
            if (addResult.IsFailure)
            {
                _unitOfWork.Rollback();
                return addResult;
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Testemunho submetido com sucesso e aguarda moderação.");
        }
        catch (ArgumentException ex)
        {
            _unitOfWork.Rollback();
            return Error.Validation(ex.Message);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.InternalServer($"Erro ao submeter o testemunho: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetApprovedTestimonialsAsync(int consultantId)
    {
        if (consultantId <= 0)
        {
            return Error.Validation("ID do consultor inválido.");
        }

        var result = await _unitOfWork.Testimonials.GetPublishedAsync();
        if (result.IsFailure || result.Value == null)
        {
            return Result<IEnumerable<Testimonials>>.Failure(result.Error);
        }

        var filtered = System.Linq.Enumerable.Where(result.Value, t => t.ConsultantId == consultantId);

        return Result<IEnumerable<Testimonials>>.Success(filtered);
    }

    public async Task<Result<IEnumerable<Testimonials>>> GetAllTestimonialsForModerationAsync(int consultantId)
    {
        if (consultantId <= 0)
        {
            return Error.Validation("ID do consultor inválido.");
        }

        var result = await _unitOfWork.Testimonials.GetPendingApprovalAsync();
        if (result.IsFailure || result.Value == null)
        {
            return Result<IEnumerable<Testimonials>>.Failure(result.Error);
        }

        var filtered = System.Linq.Enumerable.Where(result.Value, t => t.ConsultantId == consultantId);

        return Result<IEnumerable<Testimonials>>.Success(filtered);
    }

    public async Task<Result> DeleteTestimonialAsync(int testimonialId)
    {
        if (testimonialId <= 0)
        {
            return Error.Validation("ID do testemunho inválido.");
        }

        var exists = await _unitOfWork.Testimonials.ExistsAsync(testimonialId);
        if (!exists)
        {
            return Error.NotFound(ErrorCodes.NotFound, $"O testemunho com ID {testimonialId} não existe.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.Testimonials.DeleteAsync(testimonialId);
            if (deleteResult.IsFailure)
            {
                _unitOfWork.Rollback();
                return deleteResult;
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Testemunho removido com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.InternalServer($"Erro ao remover o testemunho: {ex.Message}");
        }
    }

    public async Task<Result> UpdateVisibilityAsync(int testimonialId, bool isVisible)
    {
        if (testimonialId <= 0)
        {
            return Error.Validation("ID do testemunho inválido.");
        }

        var testimonialResult = await _unitOfWork.Testimonials.GetByIdAsync(testimonialId);
        if (testimonialResult.IsFailure || testimonialResult.Value == null)
        {
            return Error.NotFound(ErrorCodes.NotFound, $"O testemunho com ID {testimonialId} não foi encontrado.");
        }

        var testimonial = testimonialResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            
            testimonial.UpdateVisibility(isVisible);

            var updateResult = await _unitOfWork.Testimonials.UpdateAsync(testimonial);
            if (updateResult.IsFailure)
            {
                _unitOfWork.Rollback();
                return updateResult;
            }

            await _unitOfWork.CommitAsync();
            return Result.Success($"Visibilidade do testemunho atualizada para: {(isVisible ? "Visível" : "Oculto")}.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.InternalServer($"Erro ao atualizar a visibilidade do testemunho: {ex.Message}");
        }
    }
}