using BarracudaSystem.Domain.Exceptions;
using BarracudaSystem.Domain.Enums;
using BarracudaSystem.Domain.Common;
using BarracudaSystem.Domain.Aggregates.InventoryAggregate;


namespace BarracudaSystem.Domain.Aggregates.SalesAggregate;
//REPRESENTA LA BOLETA, GUARDA FECHA, TOTALES, DESCUENTOS, SI ESTÁ ANULADA
//CONTIENE LA LISTA DE PRODUCTOS ( SaleDetail que representa 1 producto )
//CONTIENE LA LISTA DE PAGOS ( SalePayment.cs  que representa 1 pago realizado en esta Boleta)
public class Sale : IAggregateRoot
{
    // 1. Identificador y Auditoría
    public int Id { get; private set; }
    public DateTime FechaVenta { get; private set; } = DateTime.UtcNow;

    // 2. Montos de la Venta
    public decimal TotalBruto { get; private set; } = 0m;
    public decimal MontoDescuento { get; private set; } = 0m;
    public decimal TotalNeto => Math.Max(0m, TotalBruto - MontoDescuento);

    // 3. Estado y Anulación
    public bool EstaAnulada { get; private set; } = false;
    public string? MotivoAnulacion { get; private set; }
    public DateTime? FechaAnulacion { get; private set; }

    // 4. Relaciones (Colecciones)
    private readonly List<SaleDetail> _details = new();
    public IReadOnlyCollection<SaleDetail> Details => _details.AsReadOnly();

    private readonly List<SalePayment> _payments = new();
    public IReadOnlyCollection<SalePayment> Payments => _payments.AsReadOnly();

    // Constructor privado para EF Core
    private Sale() { }

    // Constructor de Dominio
    public Sale(decimal montoDescuento = 0m)
    {
        SetMontoDescuento(montoDescuento);
    }

    // =========================================================================
    // MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA
    // =========================================================================

    public void SetMontoDescuento(decimal montoDescuento)
    {
        ValidarAnulada();
        if (montoDescuento < 0)
            throw new DomainException("El descuento no puede ser un monto negativo.");

        MontoDescuento = Math.Round(montoDescuento, 0);
    }

    //AGREGAR 1 PRUDUCTO AL SALE DETAIL (LA LISTA SE LLAMA _detail<Items>)
    //Si existe el Item dentro de la lista de SaleDetails se agrega a la cantidad del SaleDetail
    //Si no existe se agrega el Item, como cada Item es un SaleDetails se crea un nuevo objeto
    public void AgregarProducto(Item item, int cantidad)
    {
        ValidarAnulada();
        if (item == null) throw new DomainException("El producto no puede ser nulo.");
        if (cantidad <= 0) throw new DomainException("La cantidad agregada debe ser mayor a 0.");

        //Recorre la lista, retorna null o la lista de d.items (null lo agrega, existe se suma)
        var detalleExistente = _details.FirstOrDefault(d => d.ItemId == item.Id);
        if (detalleExistente != null)
        {
            detalleExistente.ActualizarCantidad(detalleExistente.Cantidad + cantidad);
        }
        else
        {
            _details.Add(new SaleDetail(item.Id, item.Name, item.PrecioVenta, cantidad));
        }

        RecalcularTotalBruto();
    }

//LISTA DE PAGOS ( SalePayment.cs  que representa 1 pago realizado en esta Boleta)
    public void RegistrarPago(PaymentMethod tipoPago, decimal monto, decimal montoEntregado = 0m)
    {
        ValidarAnulada();
        if (monto <= 0)
            throw new DomainException("El monto a pagar debe ser mayor a 0.");

        _payments.Add(new SalePayment(tipoPago, monto, montoEntregado));
    }

    public void AnularVenta(string motivo)
    {
        ValidarAnulada();
        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("Debe proporcionar un motivo para anular la venta.");

        EstaAnulada = true;
        MotivoAnulacion = motivo.Trim();
        FechaAnulacion = DateTime.UtcNow;
    }

    // =========================================================================
    // MÉTODOS AUXILIARES PRIVADOS
    // =========================================================================

    private void RecalcularTotalBruto()
    {
        TotalBruto = _details.Sum(d => d.Subtotal);
    }

    private void ValidarAnulada()
    {
        if (EstaAnulada)
            throw new DomainException("No se pueden realizar modificaciones en una venta anulada.");
    }
}