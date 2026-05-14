using Microsoft.AspNetCore.Http;
using System;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface ICloudService
{
    Task<string> UploadImageAsync(IFormFile? imageFile);
}
