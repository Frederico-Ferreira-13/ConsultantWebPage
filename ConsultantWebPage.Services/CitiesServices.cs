using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;
using ConsultantWebPage.Contracts.Interfaces.Repo;
using ConsultantWebPage.Contracts.Interfaces.Services;

namespace ConsultantWebPage.Services;

public class CitiesServices : ICitiesServices
{
    private readonly IUnitOfWork _unitOfWork;

    public CitiesServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Cities>> GetCityByIdAsync(int id)
    {
        var citiesResult = await _unitOfWork.Cities.GetByIdAsync(id);
        if (!citiesResult.IsSuccessful)
        {
            return Error.NotFound(
                ErrorCodes.NotFound, 
                $"Cidade com ID {id} não encontrada.");
        }
        
        return Result<Cities>.Success(citiesResult.Value!);
    }

    public async Task<Result<Cities>> GetCityByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("Nome da Cidade é obrigatório.");
        }        

        var citiesResult = await _unitOfWork.Cities.GetByNameAsync(name);
        if (citiesResult.IsFailure || citiesResult.Value == null)
        {
            return Result<Cities>.Failure(
                Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Cidade com nome '{name}' não encontrada."
                )
            );
        }
        return Result<Cities>.Success(citiesResult.Value);
    }

    public async Task<Result<IEnumerable<Cities>>> GetByDistrictAsync(string district)
    {
        if (string.IsNullOrWhiteSpace(district))
        {
            return Error.Validation("O nome do distrito é obrigatório.");
        }

        var citiesResult = await _unitOfWork.Cities.GetAllAsync();
        if (citiesResult.IsFailure || citiesResult.Value == null)
        {
            return Result<IEnumerable<Cities>>.Failure(citiesResult.Error);
        }

        var destrictedCities = citiesResult.Value.Where(c => c.DistrictName.Equals(district, StringComparison.OrdinalIgnoreCase));
        if (!destrictedCities.Any())        {
            return Result<IEnumerable<Cities>>.Failure(
                Error.NotFound(
                    ErrorCodes.NotFound,
                    $"Nenhuma cidade encontrada para o distrito '{district}'."
                )
            );
        }

        return Result<IEnumerable<Cities>>.Success(destrictedCities);
    }

    public async Task<Result<IEnumerable<Cities>>> GetAllCitiesAsync()
    {
        var citiesResult = await _unitOfWork.Cities.GetAllAsync();
        if (citiesResult.IsFailure || citiesResult.Value == null)
        {
            return Result<IEnumerable<Cities>>.Failure(citiesResult.Error);
        }
        return Result<IEnumerable<Cities>>.Success(citiesResult.Value);
    }

    public async Task<Result<IEnumerable<string>>> GetAllDistrictsAsync()
    {
        var citiesResult = await _unitOfWork.Cities.GetAllAsync();
        if (citiesResult.IsFailure || citiesResult.Value == null)
        {
            return Result<IEnumerable<string>>.Failure(citiesResult.Error);
        }

        var districts = citiesResult.Value.Select(c => c.DistrictName).Distinct();
        return Result<IEnumerable<string>>.Success(districts);
    }

    public async Task<Result<Cities>> CreateCityAsync(string name, string district)
    {
        var cityCreationResult = Cities.Create(name, district);
        if (cityCreationResult.IsFailure || cityCreationResult.Value == null)
        {
            return Result<Cities>.Failure(cityCreationResult.Error);
        }

        var existingCity = await _unitOfWork.Cities.GetByNameAsync(name);
        if (existingCity.IsSuccessful && existingCity.Value != null)
        {
            return Error.Conflict("CITY_ALREADY_EXISTS", $"Já existe uma cidade com o nome '{name}'.");
        }

        var cityToSave = cityCreationResult.Value;

        var repoResult = await _unitOfWork.Cities.AddAsync(cityToSave);
        if (repoResult.IsFailure)
        {
            return Error.InternalServer("Não foi possível registar a cidade no repositório.");
        }

        await _unitOfWork.CommitAsync();
        return Result<Cities>.Success(cityToSave, "Cidade criada com sucesso.");
    }

    public async Task<Result> UpdateCityAsync(int id, string name, string district)
    {
        var existingCityResult = await _unitOfWork.Cities.GetByIdAsync(id);
        if (existingCityResult.IsFailure || existingCityResult.Value == null)
        {
            return Result<Cities>.Failure(
                Error.NotFound(ErrorCodes.NotFound, $"Cidade com ID {id} não encontrada.")
            );
        }

        var cityToUpdate = existingCityResult.Value;

        if (!string.Equals(cityToUpdate.CityName, name, StringComparison.OrdinalIgnoreCase))
        {
            var duplicateCheck = await _unitOfWork.Cities.GetByNameAsync(name);
            if (duplicateCheck.IsSuccessful && duplicateCheck.Value != null)
            {
                return Error.Conflict("CITY_NAME_ALREADY_IN_USE", $"O nome '{name}' já está em uso por outra cidade.");
            }
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var updateResult = cityToUpdate.Update(name, district);
            if (updateResult.IsFailure)
            {
                return Result.Failure(updateResult.Error);
            }

            var repoResult = await _unitOfWork.Cities.UpdateAsync(cityToUpdate);
            if (repoResult.IsFailure)
            {
                return Error.InternalServer("Erro ao atualizar a cidade no repositório.");
            }

            await _unitOfWork.CommitAsync();
            return Result.Success("Cidade atualizada com sucesso.");
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return Error.InternalServer($"Erro inesperado ao atualizar Cidade: {ex.Message}");
        }
    }
}