using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.Integraciones;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class IntegracionService : IIntegracionService
{
    private readonly IIntegracionRepository _repository;
    public IntegracionService(IIntegracionRepository repository) => _repository = repository;

    public async Task<IEnumerable<IntegracionDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var integraciones = await _repository.GetAllAsync(soloActivas);
        return integraciones.Select(ToDto);
    }

    public async Task<IntegracionDto?> ObtenerPorIdAsync(long id)
    {
        var integracion = await _repository.GetByIdAsync(id);
        return integracion is null ? null : ToDto(integracion);
    }

    public async Task<IntegracionDto?> ObtenerPorCodigoAccionAsync(string codigoAccion)
    {
        var integracion = await _repository.GetByCodigoAccionAsync(codigoAccion);
        return integracion is null ? null : ToDto(integracion);
    }

    public async Task<IntegracionDto> CrearAsync(CrearIntegracionDto dto)
    {
        var codigo = dto.CodigoAccion.Trim().ToUpperInvariant();

        if (await _repository.ExistsByCodigoAccionAsync(codigo))
        {
            throw new BadRequestException("Ya existe una integración registrada con ese código de acción.");
        }

        var integracion = new IntgIntegracion
        {
            CodigoAccion = codigo,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        await _repository.AddAsync(integracion);
        await _repository.SaveChangesAsync();

        return ToDto(integracion);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarIntegracionDto dto)
    {
        var integracion = await _repository.GetByIdAsync(id);
        if (integracion is null) return false;

        // El código de acción se trata como clave de negocio inmutable tras la creación;
        // se ignora cualquier cambio que llegue en el DTO para ese campo.
        integracion.Nombre = dto.Nombre;
        integracion.Descripcion = dto.Descripcion;
        integracion.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var integracion = await _repository.GetByIdAsync(id);
        if (integracion is null) return false;

        integracion.Estado = activo;
        integracion.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static IntegracionDto ToDto(IntgIntegracion integracion) => new()
    {
        Id = integracion.Id,
        CodigoAccion = integracion.CodigoAccion,
        Nombre = integracion.Nombre,
        Descripcion = integracion.Descripcion,
        Estado = integracion.Estado
    };
}
