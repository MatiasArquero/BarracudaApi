using BarracudaSystem.Domain.Common;
using BarracudaSystem.Domain.Exceptions;

namespace BarracudaSystem.Domain.Aggregates.InventoryAggregate;

public class Item : IAggregateRoot
{
    //Identificador
    public int Id { get; private set; }

    //Datos Basicos OBLIGATORIOS
    public string? Barcode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Marca { get; private set; } = string.Empty;
    public int Stock { get; private set; }

    //Clasificacion (Id para relacionarse)
    public int? CategoriaId { get; private set; }
    public Category Category{get; private set;} = null!;// Navegación de EF Core
    // =null! SIGNIFICA: Prometo que EFCORE rellenará el dato, no lances error null

    public int? ProveedorId { get; private set; }
    public Provider Provider { get; private set; } = null!; // Navegación de EF Core

    //Finanzas e Inventario
    public decimal PrecioCompra { get; private set; } = 0m;
    public decimal PrecioVenta { get; private set; } = 0m; //CON IVA
    public decimal PorcentajeMargen {get; private set;} = 0m;
    public const decimal MARGEN_POR_DEFECTO = 50.0m;

    public bool AplicaIva { get; private set; } = true;

    public decimal PrecioVentaSinIva => AplicaIva ? Math.Round(PrecioVenta / 1.19m, 2) : PrecioVenta;
    public decimal GananciaEnPesos => PrecioVenta - CostoConIva;  //DIFERENCIA DE: (PrecioCompra + IVA del proveedor) Y (Precio + IVA)
    public decimal CostoConIva => AplicaIva ? PrecioCompra * 1.19m : PrecioCompra;// PrecioCompra + IVA del proveedor
    
    

    //Auditoria y estado
    public bool EsFavorito { get; private set; } = false;
    public bool IsDeleted { get; private set; } = false; //Eliminado o activo   
    public bool IsActive { get; private set; } = true; //Eliminado o activo
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    //Opcionales
    public string? Peso { get; private set; }
    public string? Tamaño {get; private set;}
    public string? Color { get; private set; }
    public string? Modelo { get; private set; }

    private Item(){}

    public Item(
        string? barcode,
        string name,
        string marca,
        int stock,
        decimal precioCompra,

        int idCaregoria,
        int proveedorId,
        
        string? peso,
        string? tamaño,
        string? color,
        string? modelo,
        decimal porcentajeMargen = MARGEN_POR_DEFECTO,
        bool aplicaIva = true
    )
    {
     SetBarcode(barcode);   
     SetNombre(name);
     SetMarca(marca);
     SetStock(stock);
     SetCostoYMargen(precioCompra, porcentajeMargen, aplicaIva);

     SetPeso(peso);
     SetTamaño(tamaño);
     SetColor(color);
     SetModelo(modelo);
    }



    public Item(
        decimal precioVentaManual,
        decimal? margenPersonalizado = null,
        bool aplicaIva = true
    )
    {
     EstablecerPrecioVentaManual(precioVentaManual, aplicaIva, margenPersonalizado);

     
    }
    // =========================================================================
    // MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA (Ideales para el Doble Clic en la UI)
    // =========================================================================

    public void SetBarcode(string? barcode)
    {
        ValidarEliminado();
        string? barcodeLimpio = string.IsNullOrWhiteSpace(barcode) ? null : barcode.Trim();
        if (barcodeLimpio?.Length > 50)
            throw new DomainException("El Barcodeo no puede superar los 50 caracteres.");
        
        Barcode = barcodeLimpio;
    }
//-------------------------------------------------------------------------------------
    public void SetNombre(string name)
    {
        ValidarEliminado();
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("El Nombre no puede estár Vacío");
        string nombreLimpio = name.Trim();
        if (nombreLimpio.Length < 2 || nombreLimpio.Length > 30)
            throw new DomainException("El Nombre debe tener entre 2 y 30 caracteres.");
        Name = nombreLimpio;
    }
//-------------------------------------------------------------------------------------
    public void SetMarca (string marca)
    {
        ValidarEliminado();
        if (string.IsNullOrWhiteSpace(marca)){
            Marca = "Genérica";
            return;
        }
        string marcaLimpio = marca.Trim();
        if (marcaLimpio.Length < 2 || marcaLimpio.Length > 30) throw new DomainException("La Marca debe tener entre 2 y 30 caracteres.");
        Marca = marcaLimpio;
    }    
//-------------------------------------------------------------------------------------
    public void SetStock(int stock)
    {
        ValidarEliminado();
        if (stock < 0)
            throw new DomainException("El stock no puede ser Menor a 0");
        Stock = stock;
    }

//-------------------------------------------------------------------------------------
//MÉTODOS DE NEGOCIO Y EDICIÓN ATÓMICA --------------DATOS OPCIONALES------------------
//-------------------------------------------------------------------------------------

    public void SetPeso(string? peso)
    {
        ValidarEliminado();
        string? pesoLimpio = string.IsNullOrWhiteSpace(peso) ? null : peso.Trim();
        if (pesoLimpio?.Length > 3) throw new DomainException("El Peso deben ser solo 3 digitos");
        Peso = pesoLimpio != null ? $"{pesoLimpio} kg" : null;
    }//-------------------------------------------------------------------------------------
    public void SetTamaño(string? tamaño)
    {
        ValidarEliminado();
        string? tamañoLimpio = string.IsNullOrWhiteSpace(tamaño) ? null : tamaño.Trim();
        if (tamañoLimpio?.Length > 30) throw new DomainException("El Tamaño no puede superar los 30' caracteres.");
        
        Tamaño = tamañoLimpio;
    }//-------------------------------------------------------------------------------------
    public void SetColor(string? color)
    {
        ValidarEliminado();
        string? colorLimpio = string.IsNullOrWhiteSpace(color) ? null : color.Trim();
        if (colorLimpio?.Length > 30) throw new DomainException("El Color no puede superar los 30 caracteres.");
        
        Color = colorLimpio;
    }//-------------------------------------------------------------------------------------
    public void SetModelo(string? modelo)
    {
        ValidarEliminado();
        string? modeloLimpio = string.IsNullOrWhiteSpace(modelo) ? null : modelo.Trim();
        if (modeloLimpio?.Length > 30) throw new DomainException("El Nombre del Modelo no puede superar los 300 caracteres.");
        
        Modelo = modeloLimpio;
    }//-------------------------------------------------------------------------------------


//-------------------------------------------------------------------------------------
//DATOS DE PRECIO SENSIBLES------------------
//-------------------------------------------------------------------------------------
    public void SetCostoYMargen(decimal precioCompra,decimal porcentajeMargen = MARGEN_POR_DEFECTO, bool aplicaIva = true)
    {
        ValidarEliminado();
        if (precioCompra < 0) throw new DomainException("El Precio de Compra no puede ser negativo.");

        PrecioCompra = precioCompra;
        PorcentajeMargen = porcentajeMargen;
        AplicaIva = aplicaIva;

        //precioCompraCalculadoIva PUEDE contener el PrecioCompra CON O SIN IVA (depende si es True el aplicaIva)
        decimal precioCompraCalculadoIva = CostoConIva; //RECORDAR => AplicaIva ? PrecioCompra * 1.19m : PrecioCompra;

        //Se le agrega el Margen de Ganancia y se almacena en PrecioVenta ya redondeado
        decimal precioConMargen = precioCompraCalculadoIva * (1 + (porcentajeMargen / 100m));  
        PrecioVenta = Math.Round(precioConMargen, 0);  
        }
    
    

    public void EstablecerPrecioVentaManual(decimal nuevoPrecioVenta, bool aplicaIva = true,decimal? margenPersonalizado = null)
    {
        ValidarEliminado();
        if (nuevoPrecioVenta <= 0) throw new DomainException("El precio de venta no puede ser negativo.");

        //Si aplica iva es TRUE como siempre y el precio simplemente se actualizó se deja así mismo
        AplicaIva = aplicaIva;
        PrecioVenta = Math.Round(nuevoPrecioVenta, 0);

        // ¿CASO 1: El producto YA TENÍA un precio de compra guardado (mayor a 0 )?
        if (PrecioCompra > 0)
        {
            // Mantiene el PrecioCompra intacto y RECALCULA EL MARGEN HACIA ATRÁS
            decimal costoBase = CostoConIva;
            PorcentajeMargen = Math.Round(((PrecioVenta - costoBase) / costoBase) * 100m, 2);
        } 
        else// ¿CASO 2: El producto NO tenía precio de compra ($0)?
        {
            // TRUE = Define el margen personalizado del usuario
            // FALSE = Usa el 50% de margen por defecto
            PorcentajeMargen = margenPersonalizado ?? MARGEN_POR_DEFECTO;      

            // Despejamos el costo: PrecioVenta / (1 + Margen)
            //RECALCULAR EL PRECIO DE COMPRA HACIA ATRÁS
            decimal costoEstimadoConIva = PrecioVenta / (1 + (PorcentajeMargen / 100m));
            
            // ? = ( AplicaIva = True ) SÍ tiene IVA.debemos quitarle el impuesto y guardar el costo neto real.
            // : = (AplicaIva = False ) No tiene IVA.El costo ya es neto, se deja tal cual.
            PrecioCompra = AplicaIva
                ? Math.Round(costoEstimadoConIva / 1.19m, 2) 
                : Math.Round(costoEstimadoConIva, 2);
        }

        
    }

//-------------------------------------------------------------------------------------  
//-------------------------------------------------------------------------------------
    // =========================================================================
    // MÉTODOS AUXILIARES Y PRIVADOS
    // =========================================================================
    private void ValidarEliminado()
    {
        if(IsDeleted){throw new DomainException("No puedes modificar algo eliminado");}
    }
    
    private static void ValidarValoresPositivos(decimal costo, decimal margen)
    {
        
        if (margen < 0) throw new DomainException("El margen no puede ser negativo.");
    }


}


