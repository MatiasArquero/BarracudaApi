using BarracudaSystem.Domain.Aggregates.SalesAggregate;

namespace BarracudaSystem.Domain.Repositories;

public interface ISaleRepository
{
    Task<Sale?> GetByIdAsync(int id);
    Task<List<Sale>> GetByDateRangeAsync(DateTime inicio, DateTime fin);
    Task AddAsync(Sale sale);
    void Update(Sale sale); // Para anular ventas o cambiar estados
}