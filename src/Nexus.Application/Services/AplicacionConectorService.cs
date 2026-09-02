using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.AplicacionConectores;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class AplicacionConectorService : IAplicacionConectorService
{
    private readonly IAplicacionConectorRepository _repository;
    private readonly IAplicacionRepository _aplicacionRepository;
    private readonly IConectorRepository _conectorRepository;

    public AplicacionConectorService(
        IAplicacionConectorRepository repository,
        IAplicacionRepository aplicacionRepository,
        IConectorRepository conectorRepository)
    {
        _repository = repository;
        _aplicacionRepository = aplicacionRepository;
        _conectorRepository = conectorRepository;
    }

    public async Task<IEnumerable<AplicacionConectorDto>> ObtenerPorAplicacionAsync(long aplicacionId)
    {
        var lista = await _repository.GetByAplicacionAsync(aplicacionId);
        return lista.Select(ToDto);
    }

    public async Task<AplicacionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<AplicacionConectorDto> CrearAsync(CrearAplicacionConectorDto dto)
    {
        if (await _aplicacionRepository.GetByIdAsync(dto.AplicacionId) is null)
        {
            throw new NotFoundException("La aplicación indicada no existe.");
        }

        if (await _conectorRepository.GetByIdAsync(dto.ConectorId) is null)
        {
            throw new NotFoundException("El conector indicado no existe.");
        }

        if (await _repository.ExistsAsync(dto.AplicacionId, dto.ConectorId))
        {
            throw new BadRequestException("Este conector ya está habilitado para la aplicación.");
        }

        var entity = new IntgAplicacionConector
        {
            AplicacionId = dto.AplicacionId,
            ConectorId = dto.ConectorId,
            UrlBasePersonalizada = dto.UrlBasePersonalizada,
            UsuarioErp = dto.UsuarioErp,
            PasswordErp = dto.PasswordErp,
            ApiKey = dto.ApiKey,
            TokenBearer = dto.TokenBearer
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var creado = await _repository.GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException("No fue posible recuperar la relación recién creada.");
        return ToDto(creado);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarAplicacionConectorDto dto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.UrlBasePersonalizada = dto.UrlBasePersonalizada;
        entity.UsuarioErp = dto.UsuarioErp;
        entity.PasswordErp = dto.PasswordErp;
        entity.ApiKey = dto.ApiKey;
        entity.TokenBearer = dto.TokenBearer;
        entity.Estado = dto.Estado;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.Estado = activo;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static AplicacionConectorDto ToDto(IntgAplicacionConector entity) => new()
    {
        Id = entity.Id,
        AplicacionId = entity.AplicacionId,
        NombreAplicacion = entity.Aplicacion?.Nombre,
        CodigoApp = entity.Aplicacion?.CodigoApp,
        ConectorId = entity.ConectorId,
        NombreConector = entity.Conector?.Nombre,
        UsuarioErp = entity.UsuarioErp,
        Estado = entity.Estado
    };
}
