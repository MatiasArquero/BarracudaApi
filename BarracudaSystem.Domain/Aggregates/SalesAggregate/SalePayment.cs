using BarracudaSystem.Domain.Enums;
using BarracudaSystem.Domain.Exceptions;

namespace BarracudaSystem.Domain.Aggregates.SalesAggregate;

public class SalePayment
{
    public int Id{get;private set;}
    public int SaleId {get;private set;}
    public Sale Sale {get;private set;} = null!;

    public PaymentMethod TipoPago {get;private set;}
    public decimal Monto {get;private set;}
    public decimal MontoEntregado{get;private set;}
    public  decimal Vuelto => TipoPago == PaymentMethod.Efectivo ? Math.Max(0m,MontoEntregado - Monto) : 0m;
    
    private SalePayment(){}

    public SalePayment(
        PaymentMethod tipoPago,
        decimal monto,
        decimal montoEntregado = 0m
    ){
        TipoPago = tipoPago;
        SetMontos(monto, montoEntregado);
    }

    // =========================================================================
    // MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA
    // =========================================================================

    public void SetMontos(decimal monto, decimal montoEntregado)
    {
        if (monto <= 0)
            throw new DomainException("El monto a pagar debe ser mayor a cero.");

        Monto = Math.Round(monto, 0);

        if (TipoPago == PaymentMethod.Efectivo)
        {
            // Si entregan menos de lo que deben pagar, se asume que entregó exactamente el monto
            decimal entregadoLimpio = montoEntregado < monto ? monto : montoEntregado;
            MontoEntregado = Math.Round(entregadoLimpio, 0);
        }
        else
        {
            // En pagos digitales o con tarjeta, el monto entregado es idéntico al monto a cobrar
            MontoEntregado = Monto;
        }
    }
}