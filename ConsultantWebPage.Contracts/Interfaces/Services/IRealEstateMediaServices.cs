using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IRealEstateMediaServices
{
    Task<Result<RealEstateMedia>> AddMediaAsync(RealEstateMedia media);
    Task<Result<IEnumerable<RealEstateMedia>>> AddMultipleMediaAsync(IEnumerable<RealEstateMedia> mediaList);
    Task<Result<IEnumerable<RealEstateMedia>>> GetMediaByPropertyIdAsync(int realEstateId);
    Task<Result<RealEstateMedia>> GetMainImageAsync(int realEstateId);
    Task<Result> SetMainImageAsync(int mediaId, int realEstateId);
    Task<Result> DeleteMediaAsync(int mediaId);
    Task<Result> UpdateMediaOrderAsync(List<int> mediaIdsInOrder);
}
