using Nexus.Application.DTOs.AplicacionIntegraciones;

namespace Nexus.Application.Interfaces.Services;

public interface IAplicacionIntegracionService
{
    Task<IEnumerable<AplicacionIntegracionDto>> ObtenerTodasAsync(bool soloActivas = true);
    Task<IEnumerable<AplicacionIntegracionDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true);
    Task<IEnumerable<AplicacionIntegracionDto>> ObtenerPorIntegracionAsync(long integracionId, bool soloActivas = true);
    Task<AplicacionIntegracionDto?> ObtenerPorClaveCompuestaAsync(long aplicacionId, long integracionId);

    /// <summary>Crea la relación. El endpoint POST no devuelve cuerpo (201 sin contenido), ya que la
    /// relación no tiene un identificador propio expuesto en el contrato de la API.</summary>
    Task CrearAsync(CrearAplicacionIntegracionDto dto);

    Task<bool> CambiarEstadoAsync(long aplicacionId, long integracionId, bool activo);
}
