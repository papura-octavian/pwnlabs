using Pwnlabs.Domain.Entities;

namespace Pwnlabs.Application.Abstractions;

public interface ILabRepository
{
    void Add(LabInstance lab);
    Task<LabInstance?> GetByIdAsync(Guid id);
    Task<List<LabInstance>> GetActiveByUserAsync(Guid userId);
    Task<int> CountActiveByUserAsync(Guid userId);
    Task SaveChangesAsync();
}