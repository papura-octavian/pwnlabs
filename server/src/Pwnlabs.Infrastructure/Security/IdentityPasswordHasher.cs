using Microsoft.AspNetCore.Identity;
using Pwnlabs.Application.Abstractions;
using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Infrastructure.Security;

public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = _hasher.VerifyHashedPassword(null!, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}