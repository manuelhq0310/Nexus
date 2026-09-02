using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.AplicacionIntegraciones;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class AplicacionIntegracionService : IAplicacionIntegracionService
{
    private readonly IAplicacionIntegracionRepository _repository;
    private readonly IAplicacionRepository _aplicacionRepository;
    private readonly IIntegracionRepository _integracionRepository;

    public AplicacionIntegracionService(
        IAplicacionIntegracionRepository repository,
        IAplicacionRepository aplicacionRepository,
        IIntegracionRepository integracionRepository)
    {
        _repository = repository;
        _aplicacionRepository = aplicacionRepository;
        _integracionRepository = integracionRepository;
    }

    public async Task<IEnumerable<AplicacionIntegracionDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var lista = await _repository.GetAllAsync(soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<IEnumerable<AplicacionIntegracionDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true)
    {
        var lista = await _repository.GetByAplicacionAsync(aplicacionId, soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<IEnumerable<AplicacionIntegracionDto>> ObtenerPorIntegracionAsync(long integracionId, bool soloActivas = true)
    {
        var lista = await _repository.GetByIntegracionAsync(integracionId, soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<AplicacionIntegracionDto?> ObtenerPorClaveCompuestaAsync(long aplicacionId, long integracionId)
    {
        var entity = await _repository.GetByCompositeKeyAsync(aplicacionId, integracionId);
        return entity is null ? null : ToDto(entity);
    }

    public async Task CrearAsync(CrearAplicacionIntegracionDto dto)
    {
        if (await _aplicacionRepository.GetByIdAsync(dto.AplicacionId) is null)
        {
            throw new NotFoundException("La aplicación indicada no existe.");
        }

        if (await _integracionRepository.GetByIdAsync(dto.IntegracionId) is null)
        {
            throw new NotFoundException("La integración indicada no existe.");
        }

        if (await _repository.GetByCompositeKeyAsync(dto.AplicacionId, dto.IntegracionId) is not null)
        {
            throw new BadRequestException("Esta integración ya está asociada a la aplicación.");
        }

        var entity = new IntgAplicacionIntegracion
        {
            AplicacionId = dto.AplicacionId,
            IntegracionId = dto.IntegracionId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> CambiarEstadoAsync(long aplicacionId, long integracionId, bool activo)
    {
        var entity = await _repository.GetByCompositeKeyAsync(aplicacionId, integracionId);
        if (entity is null) return false;

        entity.Estado = activo;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static AplicacionIntegracionDto ToDto(IntgAplicacionIntegracion entity) => new()
    {
        AplicacionId = entity.AplicacionId,
        NombreAplicacion = entity.Aplicacion?.Nombre,
        IntegracionId = entity.IntegracionId,
        NombreIntegracion = entity.Integracion?.Nombre,
        DescripcionIntegracion = entity.Integracion?.Descripcion,
        Estado = entity.Estado
    };
}
