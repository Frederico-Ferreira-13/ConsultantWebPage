using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Core.Models.ValueObjects;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Contracts.Interfaces.Repo;

namespace ConsultantWebPage.Services;

public class RealEstateServices : IRealEstateServices
{
    private readonly IUnitOfWork _unitOfWork;

    public RealEstateServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<RealEstate>> GetRealEstateByIdAsync(int id, int currentUserId)
    {
        if (id <= 0)
            return Result<RealEstate>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstates.GetByIdAsync(id);
        if (!result.IsSuccessful)
            return Result<RealEstate>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {id} não encontrado."));

        var property = result.Value!;

        if (!property.GetIsActive() && property.ConsultantId != currentUserId)
            return Result<RealEstate>.Failure(
                Error.Forbidden(ErrorCodes.AuthForbidden, "Não tem permissão para ver este imóvel."));

        return Result<RealEstate>.Success(property);
    }

    public async Task<Result<IEnumerable<RealEstate>>> GetAllActivePropertiesAsync()
    {
        var result = await _unitOfWork.RealEstates.GetAllAsync();
        if (!result.IsSuccessful)
            return Result<IEnumerable<RealEstate>>.Failure(result.Error);

        var active = result.Value!.Where(p => p.GetIsActive());
        return Result<IEnumerable<RealEstate>>.Success(active);
    }

    public async Task<Result<RealEstate>> GetPropertyDetailsAsync(int id)
    {
        if (id <= 0)
            return Result<RealEstate>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstates.GetByIdAsync(id);
        if (!result.IsSuccessful)
            return Result<RealEstate>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {id} não encontrado."));

        return Result<RealEstate>.Success(result.Value!);
    }

    public async Task<Result<RealEstate>> CreatePropertyAsync(RealEstate property, int consultantId)
    {
        if (property == null)
            return Result<RealEstate>.Failure(Error.Validation("O imóvel não pode ser nulo."));

        if (consultantId <= 0)
            return Result<RealEstate>.Failure(
                Error.Validation("ID do consultor inválido.",
                    new Dictionary<string, string[]> { { nameof(consultantId), new[] { "Deve ser maior que zero" } } }));

        var consultantExists = await _unitOfWork.Users.ExistsAsync(consultantId);
        if (!consultantExists)
            return Result<RealEstate>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Consultor com ID {consultantId} não encontrado."));

        var typeExists = await _unitOfWork.RealEstatesTypes.ExistsAsync(property.RealEstateTypeId);
        if (!typeExists)
        {
            return Result<RealEstate>.Failure(
                Error.Validation("Tipo de imóvel inválido.", 
                new Dictionary<string, string[]> { { nameof(property.RealEstateTypeId), new[] { "O tipo de imóvel selecionado não existe." } } }));
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addResult = await _unitOfWork.RealEstates.AddAsync(property);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<RealEstate>.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao criar o imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result<RealEstate>.Success(property);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<RealEstate>.Failure(
                Error.InternalServer($"Ocorreu um erro ao criar o imóvel: {ex.Message}"));
        }
    }

    public async Task<Result> UpdatePropertyStatusAsync(int id, bool isActive)
    {
        if (id <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstates.GetByIdAsync(id);
        if (!result.IsSuccessful)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {id} não encontrado."));

        var property = result.Value!;
        var newStatus = isActive ? "Available" : "Inactive";

        if (property.Status == newStatus)
            return Result.Success("Estado já está atualizado.");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            property.UpdateRealEstate(
                property.RealEstateTypeId, property.ConsultantId, property.CityId,
                property.Title, property.Description, property.ThumbnailUrl,
                property.ExternalSourceUrl, property.ExternalReferenceId,
                property.Price, property.Address, newStatus, property.IsFeatured
            );

            var updateResult = await _unitOfWork.RealEstates.UpdateAsync(property);
            if (!updateResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(updateResult.Message ?? "Erro ao atualizar o estado do imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Estado do imóvel atualizado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao atualizar o estado: {ex.Message}"));
        }
    }

    public async Task<Result> RequestInformationAsync(int propertyId, string clientName, string clientEmail, string message)
    {
        if (propertyId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(propertyId), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(clientName))
            return Result.Failure(
                Error.Validation("O nome do cliente é obrigatório.",
                    new Dictionary<string, string[]> { { nameof(clientName), new[] { "Campo obrigatório" } } }));

        if (string.IsNullOrWhiteSpace(clientEmail) || !clientEmail.Contains("@"))
            return Result.Failure(
                Error.Validation("Email inválido.",
                    new Dictionary<string, string[]> { { nameof(clientEmail), new[] { "Formato de email inválido" } } }));

        if (string.IsNullOrWhiteSpace(message))
            return Result.Failure(
                Error.Validation("A mensagem é obrigatória.",
                    new Dictionary<string, string[]> { { nameof(message), new[] { "Campo obrigatório" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(propertyId);
        if (!propertyExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {propertyId} não encontrado."));


        var hasRecent = await _unitOfWork.Leads.HasRecentLeadFromEmailAsync(clientEmail, propertyId);
        if (hasRecent)
            return Result.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists,
                    "Já existe um pedido de informação recente deste email para este imóvel."));

        var contactResult = ContactInfo.Create(clientEmail, "900000000");
        if (contactResult.IsFailure)
        {
            return Result.Failure(contactResult.Error);
        }

        var leadResult = Leads.Create(propertyId, null, clientName, contactResult.Value!, message);
        if (leadResult.IsFailure)
        {
            return Result.Failure(leadResult.Error);
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var lead = leadResult.Value;

            var addResult = await _unitOfWork.Leads.AddAsync(lead!);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao registar o pedido de informação."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Pedido de informação registado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao registar o pedido: {ex.Message}"));
        }
    }

    public async Task<Result> ScheduleVisitAsync(int propertyId, int clientId, DateTime preferredDate)
    {
        if (propertyId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(propertyId), new[] { "Deve ser maior que zero" } } }));

        if (clientId <= 0)
            return Result.Failure(
                Error.Validation("ID do cliente inválido.",
                    new Dictionary<string, string[]> { { nameof(clientId), new[] { "Deve ser maior que zero" } } }));

        if (preferredDate <= DateTime.UtcNow)
            return Result.Failure(
                Error.Validation("A data da visita deve ser no futuro.",
                    new Dictionary<string, string[]> { { nameof(preferredDate), new[] { "Deve ser uma data futura" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(propertyId);
        if (!propertyExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {propertyId} não encontrado."));

        var userResult = await _unitOfWork.Users.GetByIdAsync(clientId);
        if (userResult.IsFailure || userResult.Value == null)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Utilizador com ID {clientId} não encontrado."));

        var user = userResult.Value;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // CORRIGIDO: antes usava reflection (GetProperty("Email")/("PhoneNumber")/("Name"))
            // que nunca encontrava nada porque estes dados vivem no Value Object Contact
            // e a propriedade chama-se FullName, não Name. Resultado: caía sempre no
            // fallback "sem@email.com". Agora lê diretamente da entidade.
            var contactResult = ContactInfo.Create(user.Contact.Email, user.Contact.PhoneNumber);
            if (contactResult.IsFailure)
            {
                _unitOfWork.Rollback();
                return Result.Failure(contactResult.Error);
            }

            string message = $"Pedido de visita agendado para a data: {preferredDate.ToLocalTime():dd/MM/yyyy HH:mm}";

            var leadResult = Leads.Create(propertyId, null, user.FullName, contactResult.Value!, message);
            if (leadResult.IsFailure)
            {
                _unitOfWork.Rollback();
                return Result.Failure(leadResult.Error);
            }

            var lead = leadResult.Value;

            var addResult = await _unitOfWork.Leads.AddAsync(lead!);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao agendar a visita."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Visita agendada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao agendar a visita: {ex.Message}"));
        }
    }

    // -------------------------------------------------------------------------
    // COMENTÁRIOS
    // -------------------------------------------------------------------------

    public async Task<Result> AddCommentAsync(int propertyId, int userId, string commentText)
    {
        if (propertyId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(propertyId), new[] { "Deve ser maior que zero" } } }));

        if (userId <= 0)
            return Result.Failure(
                Error.Validation("ID do utilizador inválido.",
                    new Dictionary<string, string[]> { { nameof(userId), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(commentText))
            return Result.Failure(
                Error.Validation("O comentário não pode estar vazio.",
                    new Dictionary<string, string[]> { { nameof(commentText), new[] { "Campo obrigatório" } } }));

        if (commentText.Length > 1000)
            return Result.Failure(
                Error.Validation("O comentário não pode exceder 1000 caracteres.",
                    new Dictionary<string, string[]> { { nameof(commentText), new[] { "Máximo 1000 caracteres" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(propertyId);
        if (!propertyExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {propertyId} não encontrado."));

        var userExists = await _unitOfWork.Users.ExistsAsync(userId);
        if (!userExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Utilizador com ID {userId} não encontrado."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var comment = new Comments(propertyId, userId, commentText);

            var addResult = await _unitOfWork.Comments.AddAsync(comment);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao adicionar o comentário."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Comentário adicionado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao adicionar o comentário: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<Comments>>> GetCommentsAsync(int propertyId)
    {
        if (propertyId <= 0)
            return Result<IEnumerable<Comments>>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(propertyId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.Comments.GetCommentsByRealEstateIdAsync(propertyId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<Comments>>.Failure(result.Error);

        return Result<IEnumerable<Comments>>.Success(result.Value!);
    }

    // -------------------------------------------------------------------------
    // PESQUISA
    // -------------------------------------------------------------------------

    public async Task<Result<IEnumerable<RealEstate>>> SearchPropertiesAsync(string city, decimal? maxPrice, int? rooms)
    {
        // IMPLEMENTADO: rooms deixou de ser ignorado — é agora aplicado no
        // repositório via associação à categoria de tipologia correspondente
        // (T0, T1, T2...). Ver RealEstateRepository.SearchAsync.
        var (items, _) = await _unitOfWork.RealEstates.SearchAsync(
            searchTerm: city,
            categoryId: null,
            minPrice: null,
            maxPrice: maxPrice,
            rooms: rooms,
            page: 1,
            pageSize: 100
        );

        return Result<IEnumerable<RealEstate>>.Success(items);
    }

    // -------------------------------------------------------------------------
    // CATEGORIAS
    // -------------------------------------------------------------------------

    public async Task<Result> AssignCategoryToPropertyAsync(int realEstateId, int categoryId)
    {
        if (realEstateId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        if (categoryId <= 0)
            return Result.Failure(
                Error.Validation("ID da categoria inválido.",
                    new Dictionary<string, string[]> { { nameof(categoryId), new[] { "Deve ser maior que zero" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(realEstateId);
        if (!propertyExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {realEstateId} não encontrado."));

        var alreadyAssigned = await _unitOfWork.RealEstatesCategories.IsRealEstateInCategoryAsync(realEstateId, categoryId);
        if (alreadyAssigned)
            return Result.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists, "Esta categoria já está associada a este imóvel."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var realEstateCategory = new RealEstateCategories(realEstateId, categoryId);
            var addResult = await _unitOfWork.RealEstatesCategories.AddAsync(realEstateCategory);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao associar categoria ao imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Categoria associada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao associar a categoria: {ex.Message}"));
        }
    }

    public async Task<Result> RemoveCategoryFromPropertyAsync(int realEstateId, int categoryId)
    {
        if (realEstateId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        if (categoryId <= 0)
            return Result.Failure(
                Error.Validation("ID da categoria inválido.",
                    new Dictionary<string, string[]> { { nameof(categoryId), new[] { "Deve ser maior que zero" } } }));

        var isAssigned = await _unitOfWork.RealEstatesCategories.IsRealEstateInCategoryAsync(realEstateId, categoryId);
        if (!isAssigned)
            return Result.Success("Categoria já não está associada a este imóvel (idempotente).");

        var categoryResult = await _unitOfWork.RealEstatesCategories.GetByCategoryIdAsync(categoryId);
        if (!categoryResult.IsSuccessful)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, "Associação não encontrada."));

        var association = categoryResult.Value!.FirstOrDefault(c => c.RealEstateId == realEstateId);
        if (association == null)
            return Result.Success("Associação não encontrada (idempotente).");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.RealEstatesCategories.DeleteAsync(association.GetId());
            if (!deleteResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(deleteResult.Message ?? "Erro ao remover categoria do imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Categoria removida com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao remover a categoria: {ex.Message}"));
        }
    }

    public async Task<Result> SyncPropertyCategoriesAsync(int realEstateId, IEnumerable<int> categoryIds)
    {
        if (realEstateId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var propertyExists = await _unitOfWork.RealEstates.ExistsAsync(realEstateId);
        if (!propertyExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imóvel com ID {realEstateId} não encontrado."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Remove todas as categorias existentes
            var removeResult = await _unitOfWork.RealEstatesCategories.RemoveAllFromRealEstateAsync(realEstateId);
            if (!removeResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(removeResult.Message ?? "Erro ao remover categorias existentes."));
            }

            // Adiciona as novas categorias
            foreach (var categoryId in categoryIds.Distinct())
            {
                var realEstateCategory = new RealEstateCategories(realEstateId, categoryId);
                var addResult = await _unitOfWork.RealEstatesCategories.AddAsync(realEstateCategory);
                if (!addResult.IsSuccessful)
                {
                    _unitOfWork.Rollback();
                    return Result.Failure(
                        Error.InternalServer(addResult.Message ?? $"Erro ao adicionar categoria {categoryId}."));
                }
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Categorias sincronizadas com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao sincronizar as categorias: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<Category>>> GetCategoriesByPropertyIdAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<IEnumerable<Category>>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstatesCategories.GetByRealEstateIdAsync(realEstateId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<Category>>.Failure(result.Error);

        var categories = result.Value!
            .Where(rc => rc.Category != null)
            .Select(rc => rc.Category!);

        return Result<IEnumerable<Category>>.Success(categories);
    }

    public async Task<Result<IEnumerable<RealEstate>>> GetPropertiesByCategoryIdAsync(int categoryId)
    {
        if (categoryId <= 0)
            return Result<IEnumerable<RealEstate>>.Failure(
                Error.Validation("ID da categoria inválido.",
                    new Dictionary<string, string[]> { { nameof(categoryId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstatesCategories.GetByCategoryIdAsync(categoryId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<RealEstate>>.Failure(result.Error);

        var properties = result.Value!
            .Where(rc => rc.RealEstate != null)
            .Select(rc => rc.RealEstate!);

        return Result<IEnumerable<RealEstate>>.Success(properties);
    }

    public async Task<bool> IsPropertyInCategoryAsync(int realEstateId, int categoryId)
    {
        if (realEstateId <= 0 || categoryId <= 0) return false;
        return await _unitOfWork.RealEstatesCategories.IsRealEstateInCategoryAsync(realEstateId, categoryId);
    }

    // -------------------------------------------------------------------------
    // MEDIA
    // -------------------------------------------------------------------------

    public async Task<Result<RealEstateMedia>> AddMediaAsync(RealEstateMedia media)
    {
        if (media == null)
            return Result<RealEstateMedia>.Failure(Error.Validation("O ficheiro de media não pode ser nulo."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addResult = await _unitOfWork.RealEstatesMedia.AddAsync(media);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<RealEstateMedia>.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao adicionar media."));
            }

            await _unitOfWork.CommitAsync();
            return Result<RealEstateMedia>.Success(media);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<RealEstateMedia>.Failure(
                Error.InternalServer($"Ocorreu um erro ao adicionar media: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<RealEstateMedia>>> AddMultipleMediaAsync(IEnumerable<RealEstateMedia> mediaList)
    {
        if (mediaList == null || !mediaList.Any())
            return Result<IEnumerable<RealEstateMedia>>.Failure(
                Error.Validation("A lista de media não pode estar vazia."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var added = new List<RealEstateMedia>();

            foreach (var media in mediaList)
            {
                var addResult = await _unitOfWork.RealEstatesMedia.AddAsync(media);
                if (!addResult.IsSuccessful)
                {
                    _unitOfWork.Rollback();
                    return Result<IEnumerable<RealEstateMedia>>.Failure(
                        Error.InternalServer(addResult.Message ?? "Erro ao adicionar um dos ficheiros de media."));
                }
                added.Add(media);
            }

            await _unitOfWork.CommitAsync();
            return Result<IEnumerable<RealEstateMedia>>.Success(added);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<IEnumerable<RealEstateMedia>>.Failure(
                Error.InternalServer($"Ocorreu um erro ao adicionar media: {ex.Message}"));
        }
    }

    public async Task<Result<IEnumerable<RealEstateMedia>>> GetMediaByPropertyIdAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<IEnumerable<RealEstateMedia>>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstatesMedia.GetByRealEstateIdAsync(realEstateId);
        if (!result.IsSuccessful)
            return Result<IEnumerable<RealEstateMedia>>.Failure(result.Error);

        return Result<IEnumerable<RealEstateMedia>>.Success(result.Value!);
    }

    public async Task<Result<RealEstateMedia>> GetMainImageAsync(int realEstateId)
    {
        if (realEstateId <= 0)
            return Result<RealEstateMedia>.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstatesMedia.GetMainImageByRealEstateIdAsync(realEstateId);
        if (!result.IsSuccessful)
            return Result<RealEstateMedia>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Imagem principal não encontrada para o imóvel {realEstateId}."));

        return Result<RealEstateMedia>.Success(result.Value!);
    }

    public async Task<Result> SetMainImageAsync(int mediaId, int realEstateId)
    {
        if (mediaId <= 0)
            return Result.Failure(
                Error.Validation("ID do media inválido.",
                    new Dictionary<string, string[]> { { nameof(mediaId), new[] { "Deve ser maior que zero" } } }));

        if (realEstateId <= 0)
            return Result.Failure(
                Error.Validation("ID do imóvel inválido.",
                    new Dictionary<string, string[]> { { nameof(realEstateId), new[] { "Deve ser maior que zero" } } }));

        var mediaExists = await _unitOfWork.RealEstatesMedia.ExistsAsync(mediaId);
        if (!mediaExists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Media com ID {mediaId} não encontrado."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var setResult = await _unitOfWork.RealEstatesMedia.SetMainImageAsync(realEstateId, mediaId);
            if (!setResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(setResult.Message ?? "Erro ao definir imagem principal."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Imagem principal definida com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao definir a imagem principal: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteMediaAsync(int mediaId)
    {
        if (mediaId <= 0)
            return Result.Failure(
                Error.Validation("ID do media inválido.",
                    new Dictionary<string, string[]> { { nameof(mediaId), new[] { "Deve ser maior que zero" } } }));

        var exists = await _unitOfWork.RealEstatesMedia.ExistsAsync(mediaId);
        if (!exists)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Media com ID {mediaId} não encontrado."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.RealEstatesMedia.DeleteAsync(mediaId);
            if (!deleteResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(deleteResult.Message ?? "Erro ao eliminar o ficheiro de media."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Media eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao eliminar o media: {ex.Message}"));
        }
    }

    public async Task<Result> UpdateMediaOrderAsync(List<int> mediaIdsInOrder)
    {
        if (mediaIdsInOrder == null || mediaIdsInOrder.Count == 0)
            return Result.Failure(Error.Validation("A lista de IDs de media não pode estar vazia."));

        // Assumimos que todos os IDs pertencem ao mesmo imóvel —
        // obtemos o realEstateId a partir do primeiro media
        var firstMedia = await _unitOfWork.RealEstatesMedia.GetByIdAsync(mediaIdsInOrder[0]);
        if (!firstMedia.IsSuccessful)
            return Result.Failure(
                Error.NotFound(ErrorCodes.NotFound, "Media não encontrado."));

        var realEstateId = firstMedia.Value!.RealEstateId;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var updateResult = await _unitOfWork.RealEstatesMedia.UpdateMediaOrderAsync(realEstateId, mediaIdsInOrder);
            if (!updateResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(updateResult.Message ?? "Erro ao atualizar a ordem dos media."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Ordem dos media atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao atualizar a ordem: {ex.Message}"));
        }
    }

    // -------------------------------------------------------------------------
    // TIPOS DE IMÓVEL
    // -------------------------------------------------------------------------

    public async Task<Result<IEnumerable<RealEstateType>>> GetAllRealEstateTypesAsync()
    {
        var result = await _unitOfWork.RealEstatesTypes.GetAllAsync();
        if (!result.IsSuccessful)
            return Result<IEnumerable<RealEstateType>>.Failure(result.Error);

        return Result<IEnumerable<RealEstateType>>.Success(result.Value!);
    }

    public async Task<Result<RealEstateType>> GetRealEstateTypeByIdAsync(int id)
    {
        if (id <= 0)
            return Result<RealEstateType>.Failure(
                Error.Validation("ID do tipo inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var result = await _unitOfWork.RealEstatesTypes.GetByIdAsync(id);
        if (!result.IsSuccessful)
            return Result<RealEstateType>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Tipo de imóvel com ID {id} não encontrado."));

        return Result<RealEstateType>.Success(result.Value!);
    }

    public async Task<Result<RealEstateType>> CreateRealEstateTypeAsync(RealEstateType realEstateType)
    {
        if (realEstateType == null)
            return Result<RealEstateType>.Failure(Error.Validation("O tipo de imóvel não pode ser nulo."));

        if (string.IsNullOrWhiteSpace(realEstateType.TypeName))
            return Result<RealEstateType>.Failure(
                Error.Validation("O nome do tipo é obrigatório.",
                    new Dictionary<string, string[]> { { nameof(realEstateType.TypeName), new[] { "Campo obrigatório" } } }));

        var existing = await _unitOfWork.RealEstatesTypes.GetByNameAsync(realEstateType.TypeName);
        if (existing.IsSuccessful)
            return Result<RealEstateType>.Failure(
                Error.Conflict(ErrorCodes.AlreadyExists, $"O tipo '{realEstateType.TypeName}' já existe."));

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var addResult = await _unitOfWork.RealEstatesTypes.AddAsync(realEstateType);
            if (!addResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<RealEstateType>.Failure(
                    Error.InternalServer(addResult.Message ?? "Erro ao criar o tipo de imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result<RealEstateType>.Success(realEstateType);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<RealEstateType>.Failure(
                Error.InternalServer($"Ocorreu um erro ao criar o tipo: {ex.Message}"));
        }
    }

    public async Task<Result<RealEstateType>> UpdateRealEstateTypeAsync(RealEstateType updateRealEstateType)
    {
        if (updateRealEstateType == null)
            return Result<RealEstateType>.Failure(Error.Validation("Os dados do tipo não podem ser nulos."));

        if (updateRealEstateType.RealEstateTypeId <= 0)
            return Result<RealEstateType>.Failure(
                Error.Validation("ID do tipo inválido.",
                    new Dictionary<string, string[]> { { nameof(updateRealEstateType.RealEstateTypeId), new[] { "Deve ser maior que zero" } } }));

        if (string.IsNullOrWhiteSpace(updateRealEstateType.TypeName))
            return Result<RealEstateType>.Failure(
                Error.Validation("O nome do tipo é obrigatório.",
                    new Dictionary<string, string[]> { { nameof(updateRealEstateType.TypeName), new[] { "Campo obrigatório" } } }));

        var existingResult = await _unitOfWork.RealEstatesTypes.GetByIdAsync(updateRealEstateType.RealEstateTypeId);
        if (!existingResult.IsSuccessful)
            return Result<RealEstateType>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Tipo de imóvel com ID {updateRealEstateType.RealEstateTypeId} não encontrado."));

        // Verificar nome duplicado (se mudou)
        if (!existingResult.Value!.TypeName.Equals(updateRealEstateType.TypeName, StringComparison.OrdinalIgnoreCase))
        {
            var duplicate = await _unitOfWork.RealEstatesTypes.GetByNameAsync(updateRealEstateType.TypeName);
            if (duplicate.IsSuccessful)
                return Result<RealEstateType>.Failure(
                    Error.Conflict(ErrorCodes.AlreadyExists, $"O tipo '{updateRealEstateType.TypeName}' já existe."));
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var updateResult = await _unitOfWork.RealEstatesTypes.UpdateAsync(updateRealEstateType);
            if (!updateResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result<RealEstateType>.Failure(
                    Error.InternalServer(updateResult.Message ?? "Erro ao atualizar o tipo de imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result<RealEstateType>.Success(updateRealEstateType);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result<RealEstateType>.Failure(
                Error.InternalServer($"Ocorreu um erro ao atualizar o tipo: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteRealEstateTypeAsync(int id)
    {
        if (id <= 0)
            return Result.Failure(
                Error.Validation("ID do tipo inválido.",
                    new Dictionary<string, string[]> { { nameof(id), new[] { "Deve ser maior que zero" } } }));

        var exists = await _unitOfWork.RealEstatesTypes.ExistsAsync(id);
        if (!exists)
            return Result.Success($"Tipo de imóvel com ID {id} não encontrado (idempotente).");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var deleteResult = await _unitOfWork.RealEstatesTypes.DeleteAsync(id);
            if (!deleteResult.IsSuccessful)
            {
                _unitOfWork.Rollback();
                return Result.Failure(
                    Error.InternalServer(deleteResult.Message ?? "Erro ao eliminar o tipo de imóvel."));
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Tipo de imóvel eliminado com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Result.Failure(
                Error.InternalServer($"Ocorreu um erro ao eliminar o tipo: {ex.Message}"));
        }
    }

    public async Task<bool> TypeNameExistsAsync(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return false;
        var result = await _unitOfWork.RealEstatesTypes.GetByNameAsync(typeName);
        return result.IsSuccessful;
    }
}