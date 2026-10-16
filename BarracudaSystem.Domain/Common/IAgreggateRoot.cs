namespace BarracudaSystem.Domain.Common;

/// Interfaz marcadora para indicar que una entidad es una Raíz de Agregado (Aggregate Root).
/// Solo las entidades que implementen esta interfaz tendrán su propio Repository.
public interface IAggregateRoot
{
}