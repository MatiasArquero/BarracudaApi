using BarracudaSystem.Domain.Exceptions;

namespace BarracudaSystem.Domain.Aggregates.SalesAggregate;

public class SaleDetail
{
    // 1. Identificador y Relación
    public int Id { get; private set; }
    public int SaleId { get; private set; }
    public Sale Sale { get; private set; } = null!;

    // 2. Datos del Producto
    public int ItemId { get; private set; }
    public string NombreProductoHistorico { get; private set; } = string.Empty;
    public decimal PrecioUnitarioHistorico { get; private set; }
    public int Cantidad { get; private set; }
    public decimal Subtotal => Math.Round(PrecioUnitarioHistorico * Cantidad, 0);

    // Constructor privado para EF Core
    private SaleDetail() { }

    // Constructor de Dominio
    public SaleDetail(int itemId, string nombreProducto, decimal precioUnitario, int cantidad)
    {
        if (itemId <= 0) throw new DomainException("El ID del producto no es válido.");
        
        ItemId = itemId;
        SetNombreHistorico(nombreProducto);
        SetPrecioUnitario(precioUnitario);
        ActualizarCantidad(cantidad);
    }

    // =========================================================================
    // MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA
    // =========================================================================

    public void ActualizarCantidad(int nuevaCantidad)
    {
        if (nuevaCantidad <= 0)
            throw new DomainException("La cantidad del producto debe ser mayor a 0.");

        Cantidad = nuevaCantidad;
    }

    private void SetNombreHistorico(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del producto no puede estar vacío.");

        NombreProductoHistorico = nombre.Trim();
    }

    private void SetPrecioUnitario(decimal precio)
    {
        if (precio < 0)
            throw new DomainException("El precio unitario no puede ser negativo.");

        PrecioUnitarioHistorico = Math.Round(precio, 0);
    }
}