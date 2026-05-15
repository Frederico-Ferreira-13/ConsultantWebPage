using System;
using ConsultantWebPage.Contracts.Interfaces.Services;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models;

namespace ConsultantWebPage.Services;

public class RealEstateMediaService : IRealEstateMediaServices
{
    public async Task<Result<RealEstateMedia>> AddMediaAsync(RealEstateMedia media)
    {
        // Implementation here
    }

    public async Task<Result<IEnumerable<RealEstateMedia>>> AddMultipleMediaAsync(IEnumerable<RealEstateMedia> mediaList)
    {
        // Implementation here
    }

    public async Task<Result<IEnumerable<RealEstateMedia>>> GetMediaByPropertyIdAsync(int realEstateId)
    {
        // Implementation here
    }

    public async Task<Result<RealEstateMedia>> GetMainImageAsync(int realEstateId)
    {
        // Implementation here
    }

    public async Task<Result> SetMainImageAsync(int mediaId, int realEstateId)
    {
        // Implementation here
    }

    public async Task<Result> DeleteMediaAsync(int mediaId)
    {
        // Implementation here
    }
    public async Task<Result> UpdateMediaOrderAsync(List<int> mediaIdsInOrder)
    {
        // Implementation here
    }
}
