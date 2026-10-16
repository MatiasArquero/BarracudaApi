using BarracudaSystem.Domain.Aggregates.InventoryAggregate;

namespace BarracudaSystem.Domain.Repositories;

public interface IProviderRepository
{
    Task<Provider?> GetByIdAsync(int id);
    Task<List<Provider>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Provider provider);
    void Update(Provider provider);
}