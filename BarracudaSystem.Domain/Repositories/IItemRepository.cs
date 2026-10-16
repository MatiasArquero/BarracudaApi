using BarracudaSystem.Domain.Aggregates.InventoryAggregate;

namespace BarracudaSystem.Domain.Repositories;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(int id);
    Task<Item?> GetByBarcodeAsync(string barcode);
    Task<List<Item>> GetByBarcodeMultipleAsync(string barcode); // En caso de códigos compartidos
    Task<List<Item>> SearchByNameAsync(string name);
    Task<List<Item>> GetAllActiveAsync();
    Task<bool> ExistsBarcodeAsync(string barcode);
    Task AddAsync(Item item);
    void Update(Item item);
    void Delete(Item item);
}