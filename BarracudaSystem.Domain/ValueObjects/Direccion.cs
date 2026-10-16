using BarracudaSystem.Domain.Exceptions;

namespace BarracudaSystem.Domain.ValueObjects;

public class Direccion
{
    public string Ciudad {get; private set; } = string.Empty;
    public string Comuna { get; private set; } = string.Empty;
    public string Calle { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    
    private Direccion(){}
    public Direccion(string ciudad,string comuna, string calle, string numero)
    {
        if (string.IsNullOrWhiteSpace(ciudad))
            throw new DomainException("La Ciudad no puede estar vacío");
        if (string.IsNullOrWhiteSpace(comuna))
            throw new DomainException("La Comuna no puede estar vacío");
        if (string.IsNullOrWhiteSpace(calle))
            throw new DomainException("La Calle no puede estar vacía");
        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("La Numeración no puede estar vacía");
        
        Ciudad = ciudad.Trim();
        Comuna = comuna.Trim();
        Calle = calle.Trim();
        Numero = numero.Trim();

    }
}