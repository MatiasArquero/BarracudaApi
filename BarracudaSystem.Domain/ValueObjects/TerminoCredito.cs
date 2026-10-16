namespace BarracudaSystem.Domain.ValueObjects;

public class TerminosCredito
{
    public bool EntregaCredito { get; private set; }
    public int DiasDeCredito { get; private set; }
    public decimal MontoCredito { get; private set; }

    private TerminosCredito(){}

    public TerminosCredito(bool entregaCredito, int diasCredito, decimal montoCredito)
    {
        //Si el Bool no está activado el credito
        if (!entregaCredito)
        {
            if (diasCredito != 0 || montoCredito != 0)
            {
                throw new InvalidOperationException(
                    "Regla de Dominio Violada: Si el proveedor no tiene crédito, los días y el cupo de crédito deben ser cero."
                );
            }
            EntregaCredito = false;
            DiasDeCredito = 0;
            MontoCredito = 0;
            }
            //SI el Bool está activado
            else
        {
            if (diasCredito <= 0)
                throw new ArgumentOutOfRangeException(nameof(diasCredito),"Si tiene crédito, los días de crédito deben ser mayores a cero.");
            if (montoCredito <= 0)
                throw new ArgumentOutOfRangeException(nameof(montoCredito),"Si tiene crédito, el monto de crédito deben ser mayor a cero.");

            EntregaCredito = true;
            DiasDeCredito = diasCredito;
            MontoCredito = montoCredito;
        }
    }

    //Para usarla en la futura Entidad FacturaDeCompra
    public DateTime CalcularFechaVencimiento(DateTime fechaEmisionFactura)
    {
        if (!EntregaCredito)return fechaEmisionFactura;//Pago Inmediato
        return fechaEmisionFactura.AddDays(DiasDeCredito);
    }
}