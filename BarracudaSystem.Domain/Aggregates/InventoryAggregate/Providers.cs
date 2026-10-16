using System.Text.RegularExpressions;
using BarracudaSystem.Domain.Exceptions;
using BarracudaSystem.Domain.ValueObjects;
using BarracudaSystem.Domain.Common;

namespace BarracudaSystem.Domain.Aggregates.InventoryAggregate;

public partial class Provider : IAggregateRoot
{
    public int Id { get; private set; } //PERMIRIT NULL O STRING.EMPTY NO SIGNIFICA QUE SEA OPCIONAL

    //Datos Basicos
    public string NombreFantasia { get; private set; } = string.Empty; //Inicializa con ("") para evitar null y que sea obligatorio
    public string Rut { get; private set; } = string.Empty;
    public string RazonSocial { get; private set; } = string.Empty;
    public string GiroComercial { get; private set; } = string.Empty;

    //Datos de contacto OPCIONALES
    public string? ContactoTelefono { get; private set; }
    public string? Telefono { get; private set; }
    public string? SitioWeb { get; private set; }

    //Objetos de valor  ( null! significa que se inicializa en el constructor y no puede ser nulo)
    public Direccion Direccion { get; private set; } = null!;
    public TerminosCredito TerminosCredito { get; private set; } = null!;
    public DatosBancarios? DatosBancarios { get; private set; } //OPCIONAL

    //Datos de estado
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Provider() { }

    public Provider(
        string nombreFantasia,
        string rut,
        string razonSocial,
        string giroComercial,

        Direccion direccion,
        TerminosCredito terminosCredito,

        DatosBancarios? datosBancarios = null,
        string? contactoTelefono = null,
        string? telefono = null,
        string? sitioWeb = null
        )
    {
        SetNombreFantasia(nombreFantasia);
        SetRut(rut);
        SetRazonSocial(razonSocial);
        SetGiroComercial(giroComercial);

        SetContactoTelefono(contactoTelefono);
        SetTelefono(telefono);
        SetSitioWeb(sitioWeb);


        UpdateDireccion(direccion);
        UpdateTerminosCredito(terminosCredito);
        UpdateDatosBancarios(datosBancarios);

        IsActive = true;
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }


    // =========================================================================
    // MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA (Ideales para el Doble Clic en la UI)
    // =========================================================================
    public void SetNombreFantasia(string nombreFantasia)
    {
        ValidarEliminado();
        if (string.IsNullOrWhiteSpace(nombreFantasia))
            throw new DomainException("El Nombre del proveedor no puede estar vacío.");

        string nombreLimpio = nombreFantasia.Trim();
        if (nombreLimpio.Length < 2 || nombreLimpio.Length > 30)
            throw new DomainException("El Nombre debe tener entre 2 y 30 caracteres.");
        NombreFantasia = nombreLimpio;
    }
    //-------------------------------------------------------------------------------------
    public void SetRut(string rut)
    {
        ValidarEliminado();
        Rut = ValidarYFormatearRut(rut);
    }
    //-------------------------------------------------------------------------------------
    public void SetRazonSocial(string razonSocial)
    {
        ValidarEliminado();
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new DomainException("La Razón Social del proveedor no puede estar vacía.");

        string razonLimpia = razonSocial.Trim();
        if (razonLimpia.Length < 2 || razonLimpia.Length > 100)
            throw new DomainException("La Razón Social debe tener entre 2 y 100 caracteres.");
        RazonSocial = razonLimpia;
    }
    //-------------------------------------------------------------------------------------
    public void SetGiroComercial(string giroComercial)
    {
        ValidarEliminado();
        if (string.IsNullOrWhiteSpace(giroComercial))
            throw new DomainException("El Giro Comercial del proveedor no puede estar vacío.");

        string giroLimpio = giroComercial.Trim();
        if (giroLimpio.Length < 2 || giroLimpio.Length > 100)
            throw new DomainException("El Giro Comercial debe tener entre 2 y 100 caracteres.");

        GiroComercial = giroLimpio;
    }
    //-------------------------------------------------------------------------------------

    //-------------------------------------------------------------------------------------
    public void SetContactoTelefono(string? contactoTelefono)
    {
        ValidarEliminado();
        string? contactoLimpio = string.IsNullOrWhiteSpace(contactoTelefono) ? null : contactoTelefono.Trim();
        if (contactoLimpio?.Length > 100)
            throw new DomainException("El Nombre del Contacto no puede superar los 100 caracteres.");

        ContactoTelefono = contactoLimpio;
    }
    //-------------------------------------------------------------------------------------
    public void SetSitioWeb(string? sitioWeb)
    {
        ValidarEliminado();
        string? sitioWebLimpio = string.IsNullOrWhiteSpace(sitioWeb) ? null : sitioWeb.Trim();
        if (sitioWebLimpio?.Length > 100)
            throw new DomainException("El Sitio Web no puede superar los 100 caracteres.");

        SitioWeb = sitioWebLimpio;
    }
    //-------------------------------------------------------------------------------------
    public void SetTelefono(string? telefono)
    {
        ValidarEliminado();
        Telefono = ValidarYFormatearTelefono(telefono);
    }
    //-------------------------------------------------------------------------------------
    // =========================================================================
    // ACTUALIZACIÓN DE VALUE OBJECTS (Inmutables: se reemplazan completos)
    // =========================================================================
    public void UpdateDireccion(Direccion nuevaDireccion)
    {
        ValidarEliminado();
        Direccion = nuevaDireccion ?? throw new ArgumentNullException(
            nameof(nuevaDireccion), "La configuración de dirección es obligatoria.");
    }

    public void UpdateTerminosCredito(TerminosCredito nuevoTerminoCredito)
    {
        ValidarEliminado();
        TerminosCredito = nuevoTerminoCredito ?? throw new ArgumentNullException(
            nameof(nuevoTerminoCredito), "La configuración de crédito es obligatoria.");
    }

    public void UpdateDatosBancarios(DatosBancarios? nuevosDatosBancarios)
    {
        ValidarEliminado();
        DatosBancarios = nuevosDatosBancarios; // Puede ser nulo
    }

    // =========================================================================
    // ESTADOS Y SOFT DELETE (Publicos para usarlos en otras clases)
    // =========================================================================

    public void Activate()
    {
        ValidarEliminado();
        IsActive = true;
    }

    public void Deactivate()
    {
        ValidarEliminado();
        IsActive = false;
    }

    public void SoftDelete()
    {
        if (IsDeleted)
            throw new DomainException("El proveedor ya se encuentra eliminado.");

        IsDeleted = true;
        IsActive = false; // Al eliminar, también se desactiva
    }

    public void Restore()
    {
        if (!IsDeleted)
            throw new DomainException("El proveedor no está eliminado.");

        IsDeleted = false;
        IsActive = true; // Al restaurarlo desde la Papelera, vuelve a estar activo
    }

    // =========================================================================
    // MÉTODOS AUXILIARES Y PRIVADOS
    // =========================================================================
    private void ValidarEliminado()
    {
        if (IsDeleted) { throw new DomainException("No puedes modificar algo eliminado"); }
    }
    // VALIDAR RUT CON REGEX
    [GeneratedRegex(@"^\d{7,8}[\dK]$")]
    private static partial Regex RutRegex();

    private static string ValidarYFormatearRut(string rut)
    {
        if (string.IsNullOrWhiteSpace(rut))
            throw new DomainException("El RUT del proveedor no puede estar vacío.");
        string rutLimpio = rut.Replace(".", "").Replace("-", "").Trim().ToUpper();

        if (!RutRegex().IsMatch(rutLimpio))
            throw new DomainException("Formato de rut Incorrecto (8 a 9 caracteres)");

        return rutLimpio;
    }

    //VALIDAR TELEFONO CON REGEX
    [GeneratedRegex(@"^(9|2)[0-9]{8}$")]
    private static partial Regex TelefonoRegex();

    private static string? ValidarYFormatearTelefono(string? telefono)
    {
        //Validar que tenga 9 numeros y que no sea nulo
        if (string.IsNullOrWhiteSpace(telefono))
            return null;

        string telefonoLimpio = telefono.Replace("+", "").Replace("-", "").Replace(" ", "").Trim();

        if (!TelefonoRegex().IsMatch(telefonoLimpio))
            throw new DomainException("El teléfono debe tener 9 dígitos y comenzar con 9 (móvil) o 2 (fijo). Ej: 912345678");

        return telefonoLimpio;
    }
    //-----------------------------------------------------------------------------------------

}



