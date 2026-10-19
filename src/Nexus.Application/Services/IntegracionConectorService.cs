using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.IntegracionConectores;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;
using Nexus.Domain.Enums;

namespace Nexus.Application.Services;

public class IntegracionConectorService : IIntegracionConectorService
{
    private readonly IIntegracionConectorRepository _repository;
    private readonly IIntegracionRepository _integracionRepository;
    private readonly IConectorRepository _conectorRepository;
    private readonly IUnoEConsultaConfigRepository _unoEConsultaRepository;

    public IntegracionConectorService(
        IIntegracionConectorRepository repository,
        IIntegracionRepository integracionRepository,
        IConectorRepository conectorRepository,
        IUnoEConsultaConfigRepository unoEConsultaRepository)
    {
        _repository = repository;
        _integracionRepository = integracionRepository;
        _conectorRepository = conectorRepository;
        _unoEConsultaRepository = unoEConsultaRepository;
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
        var integracion = await _integracionRepository.GetByIdAsync(dto.IntegracionId)
            ?? throw new NotFoundException("La integración indicada no existe.");

        if (await _conectorRepository.GetByIdAsync(dto.ConectorId) is null)
        {
            throw new NotFoundException("El conector indicado no existe.");
        }

        if (await _repository.ExistsAsync(dto.IntegracionId, dto.ConectorId))
        {
            throw new BadRequestException("Ese conector ya está habilitado para esta integración.");
        }

        var (ruta, cola, esGenerica, unoEId) = await ValidarYNormalizarAsync(
            integracion, dto.RutaEndpoint, dto.ColaRabbitMQDestino, dto.EsConsultaGenerica, dto.UnoEConsultaConfigId);

        var entity = new IntgIntegracionConector
        {
            IntegracionId = dto.IntegracionId,
            ConectorId = dto.ConectorId,
            RutaEndpoint = ruta,
            ColaRabbitMQDestino = cola,
            EsConsultaGenerica = esGenerica,
            UnoEConsultaConfigId = unoEId
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

        var integracion = entity.Integracion
            ?? await _integracionRepository.GetByIdAsync(entity.IntegracionId)
            ?? throw new NotFoundException("La integración asociada no existe.");

        var (ruta, cola, esGenerica, unoEId) = await ValidarYNormalizarAsync(
            integracion, dto.RutaEndpoint, dto.ColaRabbitMQDestino, dto.EsConsultaGenerica, dto.UnoEConsultaConfigId);

        entity.RutaEndpoint = ruta;
        entity.ColaRabbitMQDestino = cola;
        entity.EsConsultaGenerica = esGenerica;
        entity.UnoEConsultaConfigId = unoEId;
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

    /// <summary>
    /// Reglas según la integración padre:
    ///  - EsAsincrono = true  -> la cola de RabbitMQ es obligatoria.
    ///  - EsAsincrono = false -> la ruta del endpoint es obligatoria.
    ///  - EsConsultaGenerica solo aplica a integraciones de Tipo = Consulta y exige
    ///    asociar una consulta UnoE existente y activa.
    /// Devuelve los valores normalizados a persistir.
    /// Nota: la columna RutaEndpoint sigue siendo NOT NULL en la BD, por lo que una ruta
    /// ausente se guarda como cadena vacía (ver README / pendiente de migración).
    /// </summary>
    private async Task<(string Ruta, string? Cola, bool EsGenerica, long? UnoEId)> ValidarYNormalizarAsync(
        IntgIntegracion integracion, string? ruta, string? cola, bool esConsultaGenerica, long? unoEConsultaConfigId)
    {
        ruta = string.IsNullOrWhiteSpace(ruta) ? null : ruta.Trim();
        cola = string.IsNullOrWhiteSpace(cola) ? null : cola.Trim();

        if (integracion.EsAsincrono && cola is null)
        {
            throw new BadRequestException("La integración es asíncrona: la cola de RabbitMQ destino es obligatoria.");
        }

        if (!integracion.EsAsincrono && ruta is null)
        {
            throw new BadRequestException("La integración es síncrona: la ruta del endpoint es obligatoria.");
        }

        if (esConsultaGenerica && integracion.Tipo != TipoIntegracion.Consulta)
        {
            throw new BadRequestException("Solo las integraciones de tipo Consulta pueden ejecutarse como consulta genérica.");
        }

        if (!esConsultaGenerica)
        {
            // Sin consulta genérica no tiene sentido conservar una consulta UnoE asociada.
            return (ruta ?? string.Empty, cola, false, null);
        }

        if (unoEConsultaConfigId is null)
        {
            throw new BadRequestException("Para una consulta genérica debes asociar una consulta UnoE.");
        }

        var consulta = await _unoEConsultaRepository.GetByIdAsync(unoEConsultaConfigId.Value)
            ?? throw new NotFoundException("La consulta UnoE indicada no existe.");

        if (!consulta.Estado)
        {
            throw new BadRequestException("La consulta UnoE indicada está inactiva.");
        }

        return (ruta ?? string.Empty, cola, true, unoEConsultaConfigId);
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
        EsConsultaGenerica = entity.EsConsultaGenerica,
        UnoEConsultaConfigId = entity.UnoEConsultaConfigId,
        Estado = entity.Estado
    };
}
