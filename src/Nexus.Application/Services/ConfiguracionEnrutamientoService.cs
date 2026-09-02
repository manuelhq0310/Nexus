using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.ConfiguracionEnrutamiento;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class ConfiguracionEnrutamientoService : IConfiguracionEnrutamientoService
{
    private readonly IEmpresaIntegracionConectorRepository _repository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IIntegracionConectorRepository _integracionConectorRepository;

    public ConfiguracionEnrutamientoService(
        IEmpresaIntegracionConectorRepository repository,
        IEmpresaRepository empresaRepository,
        IIntegracionConectorRepository integracionConectorRepository)
    {
        _repository = repository;
        _empresaRepository = empresaRepository;
        _integracionConectorRepository = integracionConectorRepository;
    }

    public async Task<EmpresaIntegracionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IEnumerable<EmpresaIntegracionConectorDto>> ObtenerPorEmpresaAsync(long empresaId)
    {
        var lista = await _repository.GetByEmpresaAsync(empresaId);
        return lista.Select(ToDto);
    }

    public async Task<EmpresaIntegracionConectorDto> CrearAsync(CrearEmpresaIntegracionConectorDto dto)
    {
        if (await _empresaRepository.GetByIdAsync(dto.EmpresaId) is null)
        {
            throw new NotFoundException("La empresa indicada no existe.");
        }

        if (await _integracionConectorRepository.GetByIdAsync(dto.IntegracionConectorId) is null)
        {
            throw new NotFoundException("La relación integración-conector indicada no existe.");
        }

        if (await _repository.ExistsAsync(dto.EmpresaId, dto.IntegracionConectorId))
        {
            throw new BadRequestException("Esta empresa ya tiene configurada esta combinación de integración y conector.");
        }

        var entity = new IntgEmpresaIntegracionConector
        {
            EmpresaId = dto.EmpresaId,
            IntegracionConectorId = dto.IntegracionConectorId,
            RequiereAutenticacion = dto.RequiereAutenticacion,
            ApiKey = dto.RequiereAutenticacion ? dto.ApiKey : null,
            TokenBearer = dto.RequiereAutenticacion ? dto.TokenBearer : null
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var creado = await _repository.GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException("No fue posible recuperar la relación recién creada.");
        return ToDto(creado);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarEmpresaIntegracionConectorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.RequiereAutenticacion = dto.RequiereAutenticacion;
        entity.ApiKey = dto.RequiereAutenticacion ? dto.ApiKey : null;
        entity.TokenBearer = dto.RequiereAutenticacion ? dto.TokenBearer : null;
        entity.Estado = dto.Activo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<RutaEnrutamientoResueltaDto?> ResolverAsync(long empresaId, string codigoAccion)
    {
        var entity = await _repository.GetActivaPorEmpresaYCodigoAccionAsync(empresaId, codigoAccion);
        if (entity is null) return null;

        var conector = entity.IntegracionConector.Conector;
        var urlCompleta = $"{conector.UrlBase.TrimEnd('/')}/{entity.IntegracionConector.RutaEndpoint.TrimStart('/')}";

        return new RutaEnrutamientoResueltaDto
        {
            EmpresaId = empresaId,
            CodigoAccion = codigoAccion,
            ProtocoloConector = conector.TipoProtocolo,
            UrlBaseConector = conector.UrlBase,
            RutaEndpoint = entity.IntegracionConector.RutaEndpoint,
            UrlCompleta = urlCompleta,
            ColaRabbitMQDestino = entity.IntegracionConector.ColaRabbitMQDestino
        };
    }

    private static EmpresaIntegracionConectorDto ToDto(IntgEmpresaIntegracionConector entity) => new()
    {
        Id = entity.Id,
        EmpresaId = entity.EmpresaId,
        IntegracionConectorId = entity.IntegracionConectorId,
        IntegracionId = entity.IntegracionConector.IntegracionId,
        NombreIntegracion = entity.IntegracionConector.Integracion?.Nombre,
        CodigoAccion = entity.IntegracionConector.Integracion?.CodigoAccion,
        ConectorId = entity.IntegracionConector.ConectorId,
        NombreConector = entity.IntegracionConector.Conector?.Nombre,
        UrlBaseConector = entity.IntegracionConector.Conector?.UrlBase,
        RutaEndpoint = entity.IntegracionConector.RutaEndpoint,
        ColaRabbitMQDestino = entity.IntegracionConector.ColaRabbitMQDestino
    };
}
