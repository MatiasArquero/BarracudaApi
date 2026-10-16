namespace BarracudaSystem.Domain.ValueObjects;

public class DatosBancarios
{
    public string Banco { get; private set; } = string.Empty;
    public string TipoCuenta { get; private set; } = string.Empty;
    public string NumeroCuenta { get; private set; } = string.Empty;

    private DatosBancarios(){}

    public DatosBancarios(string banco, string tipoCuenta, string numeroCuenta)
    {
        //Reglas de Dominio:
        //3 DATOS OBLIGATORIOS

        if(string.IsNullOrWhiteSpace(banco))
            throw new ArgumentException("El nombre del banco no puede estar vacío.", nameof(banco));
            
        if(string.IsNullOrWhiteSpace(tipoCuenta))
            throw new ArgumentException("El tipo de cuenta no puede estar vacío.", nameof(tipoCuenta));
            
        if(string.IsNullOrWhiteSpace(numeroCuenta))
            throw new ArgumentException("El número de cuenta no puede estar vacío.", nameof(numeroCuenta));    

        Banco = banco.Trim();
        TipoCuenta = tipoCuenta.Trim();
        NumeroCuenta = numeroCuenta.Trim();

    }

}   