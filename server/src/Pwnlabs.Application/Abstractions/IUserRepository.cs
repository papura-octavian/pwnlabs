using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string normalizedUsername);
    Task AddAsync(User user);
}