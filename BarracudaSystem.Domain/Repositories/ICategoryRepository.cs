using BarracudaSystem.Domain.Aggregates.InventoryAggregate;

namespace BarracudaSystem.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<List<Category>> GetAllAsync();
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Category category);
    void Update(Category category);
}