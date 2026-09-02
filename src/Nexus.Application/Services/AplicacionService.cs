using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.Aplicaciones;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class AplicacionService : IAplicacionService
{
    private readonly IAplicacionRepository _repository;
    public AplicacionService(IAplicacionRepository repository) => _repository = repository;

    public async Task<IEnumerable<AplicacionDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var aplicaciones = await _repository.GetAllAsync(soloActivas);
        return aplicaciones.Select(ToDto);
    }

    public async Task<AplicacionDto?> ObtenerPorIdAsync(long id)
    {
        var aplicacion = await _repository.GetByIdAsync(id);
        return aplicacion is null ? null : ToDto(aplicacion);
    }

    public async Task<AplicacionDto?> ObtenerPorCodigoAsync(string codigoApp)
    {
        var aplicacion = await _repository.GetByCodigoAsync(codigoApp);
        return aplicacion is null ? null : ToDto(aplicacion);
    }

    public async Task<AplicacionDto> CrearAsync(CrearAplicacionDto dto)
    {
        if (await _repository.ExistsByCodigoAsync(dto.CodigoApp))
        {
            throw new BadRequestException("Ya existe una aplicación registrada con ese código.");
        }

        var aplicacion = new IntgAplicacion
        {
            Nombre = dto.Nombre,
            CodigoApp = dto.CodigoApp,
            Descripcion = dto.Descripcion ?? string.Empty
        };

        await _repository.AddAsync(aplicacion);
        await _repository.SaveChangesAsync();

        return ToDto(aplicacion);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarAplicacionDto dto)
    {
        var aplicacion = await _repository.GetByIdAsync(id);
        if (aplicacion is null) return false;

        if (await _repository.ExistsByCodigoAsync(dto.CodigoApp, excludeId: id))
        {
            throw new BadRequestException("Ya existe otra aplicación registrada con ese código.");
        }

        aplicacion.Nombre = dto.Nombre;
        aplicacion.CodigoApp = dto.CodigoApp;
        aplicacion.Descripcion = dto.Descripcion ?? string.Empty;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var aplicacion = await _repository.GetByIdAsync(id);
        if (aplicacion is null) return false;

        aplicacion.Estado = activo;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static AplicacionDto ToDto(IntgAplicacion aplicacion) => new()
    {
        Id = aplicacion.Id,
        Nombre = aplicacion.Nombre,
        CodigoApp = aplicacion.CodigoApp,
        Descripcion = aplicacion.Descripcion,
        Estado = aplicacion.Estado
    };
}
