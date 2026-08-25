using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class CategoryServices : ICategoryServices
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Category>> GetCategoryByIdAsync(int categoryId)
    {
        if (categoryId <= 0)
        {
            return Error.Validation("O ID da categoria deve ser um inteiro positivo.");
        }

        var repositoryResult = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (repositoryResult.IsFailure || repositoryResult.Value == null) 
        {
            return Error.NotFound("CATEGORY_NOT_FOUND", $"Categoria com ID {categoryId} não encontrada.");
        }

        return Result<Category>.Success(repositoryResult.Value);
    }

    // NOVO: expõe o GetCategoryByNameAndAccount do repositório, que já existia
    // mas nunca tinha sido chamado por nenhum Service/Controller. Necessário
    // para o frontend conseguir associar a categoria de tipologia (T0-T5+)
    // correta a um imóvel a partir do nome, sem ter de conhecer o ID de antemão.
    public async Task<Result<Category>> GetCategoryByNameAsync(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return Error.Validation("O nome da categoria é obrigatório.");
        }

        var repositoryResult = await _unitOfWork.Categories.GetCategoryByNameAndAccount(categoryName, 0);
        if (repositoryResult.IsFailure || repositoryResult.Value == null)
        {
            return Error.NotFound("CATEGORY_NOT_FOUND", $"Categoria com o nome '{categoryName}' não encontrada.");
        }

        return Result<Category>.Success(repositoryResult.Value);
    }

    public async Task<Result<IEnumerable<Category>>> GetRootCategoriesAsync()
    {
        var rootCategoriesResult = await _unitOfWork.Categories.GetAllAsync();
        if (rootCategoriesResult.IsFailure || rootCategoriesResult.Value == null)
        {
            return Error.NotFound("ROOT_CATEGORIES_NOT_FOUND", "Não foram encontradas categorias principais.");
        }

        var rootCategories = rootCategoriesResult.Value.Where(c => c.ParentCategoryId == null);

        return Result<IEnumerable<Category>>.Success(rootCategories);
    }

    public async Task<Result<Category>> GetCategoryWithSubsAsync(int categoryId)
    {
        if (categoryId <= 0)
        {
            return Error.Validation("O ID da categoria deve ser um inteiro positivo.");
        }

        var repositoryResult = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (repositoryResult.IsFailure || repositoryResult.Value == null)
        {
            return Error.NotFound("CATEGORY_NOT_FOUND", $"Categoria com ID {categoryId} não encontrada.");
        }

        return Result<Category>.Success(repositoryResult.Value);
    }

    public async Task<Result<Category>> CreateCategoryAsync(Category newCategory)
    {
        if (string.IsNullOrWhiteSpace(newCategory.CategoryName))
        {
            return Error.Validation("O nome da categoria é obrigatório.");
        }

        if (newCategory.CategoryName.Length > 255)
        {
            return Error.Validation("O nome da categoria não pode exceder 255 caracteres.");
        }

        if (newCategory.CategoryTypeId <= 0)
        {
            return Error.Validation("Tipo de categoria inválido.");
        }

        var categoryTypeResult = await _unitOfWork.CategoryTypes.GetByIdAsync(newCategory.CategoryTypeId);
        if (categoryTypeResult.IsFailure || categoryTypeResult.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPE_NOT_FOUND", $"Tipo de categoria com ID {newCategory.CategoryTypeId} não encontrado.");
        }

        if (newCategory.ParentCategoryId.HasValue)
        {
            if (newCategory.ParentCategoryId.Value == 0)
            {
                return Error.Validation("Categoria pai inválida.");
            }

            var parentResult = await _unitOfWork.Categories.GetByIdAsync(newCategory.ParentCategoryId.Value);
            if (parentResult.IsFailure || parentResult.Value == null)
            {
                return Error.Validation($"Categoria pai com ID {newCategory.ParentCategoryId.Value} não existe.");
            }
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var repoResult = await _unitOfWork.Categories.AddAsync(newCategory);
            if (repoResult.IsFailure)
            {
                return Error.InternalServer("Não foi possível registar a categoria no repositório.");
            }

            await _unitOfWork.CommitAsync();

            return Result<Category>.Success(newCategory, "Categoria criada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.InternalServer($"Erro inesperado ao criar categoria: {ex.Message}");
        }
    }

    public async Task<Result<Category>> UpdateCategoryAsync(Category updateCategory)
    {
        var existingCategoryResult = await _unitOfWork.Categories.GetByIdAsync(updateCategory.CategoriesId);
        if (existingCategoryResult.IsFailure || existingCategoryResult.Value == null)
        {
            return Error.NotFound("CATEGORY_NOT_FOUND", "A categoria que tenta atualizar não existe.");
        }

        var existingCategory = existingCategoryResult.Value;

        existingCategory.UpdateCategory(
            updateCategory.CategoryName,
            updateCategory.CategoryTypeId,
            updateCategory.ParentCategoryId
        );
        
        var repoResult = await _unitOfWork.Categories.UpdateAsync(updateCategory);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Erro ao atualizar a categoria no repositório.");
        }

        await _unitOfWork.CommitAsync();
        return Result<Category>.Success(updateCategory, "Categoria atualizada com sucesso.");
    }

    public async Task<Result> DeleteCategoryAsync(int categoryId)
    {
        if (categoryId <= 0)
        {
            return Error.Validation("O ID da categoria é inválido.");
        }

        var existingCategoryResult = await _unitOfWork.Categories.GetByIdAsync(categoryId);
        if (existingCategoryResult.IsFailure || existingCategoryResult.Value == null)
        {
            return Error.NotFound("CATEGORY_NOT_FOUND", "A categoria que tenta eliminar não existe.");
        }

        var repoResult = await _unitOfWork.Categories.DeleteAsync(categoryId);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível eliminar a categoria.");
        }

        await _unitOfWork.CommitAsync();
        return Result.Success("Categoria eliminada com sucesso.");
    }

    public async Task<Result<IEnumerable<CategoryType>>> GetAllCategoryTypesAsync()
    {
        var result = await _unitOfWork.CategoryTypes.GetAllAsync();
        if (result.IsFailure || result.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPES_NOT_FOUND", "Nenhum tipo de categoria encontrado.");
        }
        return Result<IEnumerable<CategoryType>>.Success(result.Value);
    }

    public async Task<Result<CategoryType>> GetCategoryTypeByIdAsync(int id)
    {
        var result = await _unitOfWork.CategoryTypes.GetByIdAsync(id);
        if (result.IsFailure || result.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPE_NOT_FOUND", "Tipo de categoria não encontrado.");
        }
        return Result<CategoryType>.Success(result.Value);
    }

    public async Task<Result<CategoryType>> GetCategoryTypeByNameAsync(string name)
    {
        var result = await _unitOfWork.CategoryTypes.GetByNameAsync(name);
        if (result.IsFailure || result.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPE_NOT_FOUND", "Tipo de categoria não encontrado.");
        }
        return Result<CategoryType>.Success(result.Value);
    }

    public async Task<Result<CategoryType>> CreateCategoryTypeAsync(CategoryType categoryType)
    {
        var repoResult = await _unitOfWork.CategoryTypes.AddAsync(categoryType); // Ajustado de CreateAsync para AddAsync do repositório genérico
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível criar o tipo de categoria.");
        }

        await _unitOfWork.CommitAsync();
        return Result<CategoryType>.Success(categoryType, "Tipo de categoria criado com sucesso.");
    }

    public async Task<Result<CategoryType>> UpdateCategoryTypeAsync(CategoryType updateCategoryType)
    {
        var existingType = await _unitOfWork.CategoryTypes.GetByIdAsync(updateCategoryType.CategoryTypeId);
        if (existingType.IsFailure || existingType.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPE_NOT_FOUND", "O tipo de categoria não existe.");
        }

        var repoResult = await _unitOfWork.CategoryTypes.UpdateAsync(updateCategoryType);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Erro ao atualizar o tipo de categoria.");
        }

        await _unitOfWork.CommitAsync();
        return Result<CategoryType>.Success(updateCategoryType, "Tipo de categoria atualizado com sucesso.");
    }

    public async Task<Result> DeleteCategoryTypeAsync(int id)
    {
        var existingType = await _unitOfWork.CategoryTypes.GetByIdAsync(id);
        if (existingType.IsFailure || existingType.Value == null)
        {
            return Error.NotFound("CATEGORY_TYPE_NOT_FOUND", "O tipo de categoria não existe.");
        }

        var repoResult = await _unitOfWork.CategoryTypes.DeleteAsync(id);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível eliminar o tipo de categoria.");
        }

        await _unitOfWork.CommitAsync();
        return Result.Success("Tipo de categoria eliminado com sucesso.");
    }
}