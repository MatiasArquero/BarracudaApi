using BarracudaSystem.Application.DTOs;
using BarracudaSystem.Domain.Entities;

namespace BarracudaSystem.Application.Interfaces;

// LOS METODOS SE CREARAN DESPUES EN ItemService.cs

//Task = Operacion Asincronica
//<objeto> = Tipo de objeto de retorno
//IEnumerable<OBJETO> = Retornara Coleccion de objetos
////Si posee un ? puede retornar null o el valor dentro de <> 


//la interfaz sirve para de
public interface IItemService
{

    //FUNCIONES DE CREAR Y EDITAR

    //NUEVO ITEM
    // (CreateItemDto = Datos del formulario puros y completos)
    // (dto = )
    Task<ItemResponseDto> CreateAsync(CreateItemDto dto);

    //Actualiza la informacion de un item existente
    // (id = id del item a actualizar)
    //( CreateItemDto = Datos del formulario)
    //dto = Objeto creado a partir del DTO osea el final actualizado?)
    Task<ItemResponseDto?>UpdateAsync(int id, CreateItemDto dto);

    //obtener 1 item por su id (un solo item, id unico)
    Task<ItemResponseDto?> GetByIdAsync(int id);

    //obtener toda la lista de items activos
    Task<IEnumerable<ItemResponseDto>> GetAllActiveAsync();

    //Buscar por codigo de barras
    //Dependiendo del Count de la lista es como reacciona el frontend 
    // (0 = no encontrado, 1 = encontrado, >1 = multiples resultados (ventana modal))
    Task<IEnumerable<ItemResponseDto>> GetByBarcodeAsync(string barcode);

    //Buscar 1 Item por SKU (Es unico)
    Task<ItemResponseDto?>GetBySKUAsync(string sku);

    

    //Borrado logico
    Task<bool>SoftDeleteAsync(int id);







}