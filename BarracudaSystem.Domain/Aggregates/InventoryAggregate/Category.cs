using BarracudaSystem.Domain.Common;
using BarracudaSystem.Domain.Exceptions;

namespace BarracudaSystem.Domain.Aggregates.InventoryAggregate;

public class Category : IAggregateRoot
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } //OPCIONAL
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    // Relación Jerárquica Auto-referenciada
    private readonly List<Item> _items = new();
    public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

    //ESTE ID padre ES EL MISMO QUE UNA CATEGORIA EXISTENTE ENTONCES ES SUBCATEGORIA 
    public int? ParentCategoryId { get; private set; } //FK del PADRE NULL SI ES CATEGORIA Y NO SUBCAT
    public Category? ParentCategory { get; private set; }

    private readonly List<Category> _subcategories = new();
    public IReadOnlyCollection<Category> Subcategories => _subcategories.AsReadOnly();

    //Constructor privado para EF Core
    private Category() { }

    public Category(string name, string? description = null)
    {

        //-------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------

        SetName(name);
        SetDescription(description);
        IsActive = true;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }
    //-------------------------------------------------------------------------------------
    //Metodos de Negocio -------------Comportamiento y reglas ------------

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El nombre de la Categoría es obligatorio.");

        string nombreLimpio = name.Trim();
        if (nombreLimpio.Length < 2 || nombreLimpio.Length > 30)
            throw new DomainException("El Nombre debe tener entre 2 y 30 caracteres.");

        Name = nombreLimpio;
    }

    public void SetDescription(string? description)
    {
        string? descripcionLimpia = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (descripcionLimpia?.Length > 100)
            throw new DomainException("La descripción no puede superar los 100 caracteres.");

        Description = descripcionLimpia;
    }

    public void AddSubcategory(Category subcategory)
    {
        ArgumentNullException.ThrowIfNull(subcategory);

        if (IsDeleted)
            throw new DomainException("No se puede agregar una subcategoría a una categoría eliminada.");

        if (subcategory == this)
            throw new DomainException("Una categoría no puede ser su propia subcategoría.");

        if (subcategory.IsDeleted)
            throw new DomainException("No se puede agregar una subcategoría eliminada.");
        //Si la lista de subcategorias no contiene EL objeto subcategoria
        //Se añade a la lista, Se le asigna el objeto y 
        if (!_subcategories.Contains(subcategory))
        {
            _subcategories.Add(subcategory);
            subcategory.ParentCategory = this;
            subcategory.ParentCategoryId = Id == 0 ? null : Id;
        }
    }

    //Validacion Activar la Categoria COn SoftDelete
    public void Activate()
    {
        if (IsDeleted == true)
        {
            throw new DomainException("No se puede activar una categoría eliminada.");
        }
        IsActive = true;
    }

    //Validacion Desactivar la categoria con SoftDelete
    public void Deactivate()
    {
        if (IsDeleted == true)
        {
            throw new DomainException("No se puede desactivar una categoría eliminada.");
        }
        IsActive = false;
    }

    //Validacion que la categoria no este eliminada para poder eliminarla
    public void SoftDelete()
    {
        IsDeleted = true;
        IsActive = false; //Al eliminar, también se desactiva
    }

}
