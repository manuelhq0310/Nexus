using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.EmpresaConectores;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class EmpresaConectorService : IEmpresaConectorService
{
    private readonly IEmpresaConectorRepository _repository;
    private readonly IEmpresaRepository _empresaRepository;
    private readonly IConectorRepository _conectorRepository;

    public EmpresaConectorService(
        IEmpresaConectorRepository repository,
        IEmpresaRepository empresaRepository,
        IConectorRepository conectorRepository)
    {
        _repository = repository;
        _empresaRepository = empresaRepository;
        _conectorRepository = conectorRepository;
    }

    public async Task<IEnumerable<EmpresaConectorDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var lista = await _repository.GetAllAsync(soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<EmpresaConectorDto?> ObtenerPorEmpresaAsync(long empresaId)
    {
        var entity = await _repository.GetByEmpresaAsync(empresaId);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<EmpresaConectorDto?> ObtenerPorIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<EmpresaConectorDto> CrearAsync(AsignarEmpresaConectorDto dto)
    {
        if (await _empresaRepository.GetByIdAsync(dto.EmpresaId) is null)
        {
            throw new NotFoundException("La empresa indicada no existe.");
        }

        if (await _conectorRepository.GetByIdAsync(dto.ConectorId) is null)
        {
            throw new NotFoundException("El conector indicado no existe.");
        }

        if (await _repository.GetByEmpresaAsync(dto.EmpresaId) is not null)
        {
            throw new BadRequestException("Esta empresa ya tiene un conector asociado. Usa el endpoint de cambio de conector para reemplazarlo.");
        }

        var entity = new IntgEmpresaConector
        {
            EmpresaId = dto.EmpresaId,
            ConectorId = dto.ConectorId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var creado = await _repository.GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException("No fue posible recuperar la relación recién creada.");
        return ToDto(creado);
    }

    public async Task<bool> CambiarConectorAsync(long id, long nuevoConectorId)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        if (await _conectorRepository.GetByIdAsync(nuevoConectorId) is null)
        {
            throw new NotFoundException("El conector indicado no existe.");
        }

        entity.ConectorId = nuevoConectorId;
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

    private static EmpresaConectorDto ToDto(IntgEmpresaConector entity) => new()
    {
        Id = entity.Id,
        EmpresaId = entity.EmpresaId,
        ConectorId = entity.ConectorId,
        NombreConector = entity.Conector?.Nombre,
        TipoProtocolo = entity.Conector?.TipoProtocolo,
        UrlBase = entity.Conector?.UrlBase,
        Estado = entity.Estado
    };
}
