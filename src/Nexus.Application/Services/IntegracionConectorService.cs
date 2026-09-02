using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.IntegracionConectores;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class IntegracionConectorService : IIntegracionConectorService
{
    private readonly IIntegracionConectorRepository _repository;
    private readonly IIntegracionRepository _integracionRepository;
    private readonly IConectorRepository _conectorRepository;

    public IntegracionConectorService(
        IIntegracionConectorRepository repository,
        IIntegracionRepository integracionRepository,
        IConectorRepository conectorRepository)
    {
        _repository = repository;
        _integracionRepository = integracionRepository;
        _conectorRepository = conectorRepository;
    }

    public async Task<IntegracionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IEnumerable<IntegracionConectorDto>> ObtenerPorIntegracionAsync(long integracionId)
    {
        var lista = await _repository.GetByIntegracionAsync(integracionId);
        return lista.Select(ToDto);
    }

    public async Task<IEnumerable<IntegracionConectorDto>> ObtenerPorConectorAsync(long conectorId)
    {
        var lista = await _repository.GetByConectorAsync(conectorId);
        return lista.Select(ToDto);
    }

    public async Task<IntegracionConectorDto> CrearAsync(CrearIntegracionConectorDto dto)
    {
        if (await _integracionRepository.GetByIdAsync(dto.IntegracionId) is null)
        {
            throw new NotFoundException("La integración indicada no existe.");
        }

        if (await _conectorRepository.GetByIdAsync(dto.ConectorId) is null)
        {
            throw new NotFoundException("El conector indicado no existe.");
        }

        if (await _repository.ExistsAsync(dto.IntegracionId, dto.ConectorId))
        {
            throw new BadRequestException("Ese conector ya está habilitado para esta integración.");
        }

        var entity = new IntgIntegracionConector
        {
            IntegracionId = dto.IntegracionId,
            ConectorId = dto.ConectorId,
            RutaEndpoint = dto.RutaEndpoint,
            ColaRabbitMQDestino = dto.ColaRabbitMQDestino
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var creado = await _repository.GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException("No fue posible recuperar la relación recién creada.");
        return ToDto(creado);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarIntegracionConectorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.RutaEndpoint = dto.RutaEndpoint;
        entity.ColaRabbitMQDestino = dto.ColaRabbitMQDestino;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.Estado = activo;
        entity.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static IntegracionConectorDto ToDto(IntgIntegracionConector entity) => new()
    {
        Id = entity.Id,
        IntegracionId = entity.IntegracionId,
        NombreIntegracion = entity.Integracion?.Nombre,
        CodigoAccion = entity.Integracion?.CodigoAccion,
        ConectorId = entity.ConectorId,
        NombreConector = entity.Conector?.Nombre,
        RutaEndpoint = entity.RutaEndpoint,
        ColaRabbitMQDestino = entity.ColaRabbitMQDestino,
        Estado = entity.Estado
    };
}
