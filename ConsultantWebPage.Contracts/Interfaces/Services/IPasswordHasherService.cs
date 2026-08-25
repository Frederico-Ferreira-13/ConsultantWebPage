using System;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Contracts.Interfaces.Services;

public interface IPasswordHasherService
{
    string GenerateSalt();
    Result<HashResult> HashPassword(string password, string salt);
    bool VerifyPassword(string storedHash, string passwordToVerify);
}
