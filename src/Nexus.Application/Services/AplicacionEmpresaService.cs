using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.AplicacionEmpresas;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class AplicacionEmpresaService : IAplicacionEmpresaService
{
    private readonly IAplicacionEmpresaRepository _repository;
    private readonly IAplicacionRepository _aplicacionRepository;
    private readonly IEmpresaRepository _empresaRepository;

    public AplicacionEmpresaService(
        IAplicacionEmpresaRepository repository,
        IAplicacionRepository aplicacionRepository,
        IEmpresaRepository empresaRepository)
    {
        _repository = repository;
        _aplicacionRepository = aplicacionRepository;
        _empresaRepository = empresaRepository;
    }

    public async Task<IEnumerable<AplicacionEmpresaDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var lista = await _repository.GetAllAsync(soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<IEnumerable<AplicacionEmpresaDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true)
    {
        var lista = await _repository.GetByAplicacionAsync(aplicacionId, soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<IEnumerable<AplicacionEmpresaDto>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivas = true)
    {
        var lista = await _repository.GetByEmpresaAsync(empresaId, soloActivas);
        return lista.Select(ToDto);
    }

    public async Task<AplicacionEmpresaDto?> ObtenerPorIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<AplicacionEmpresaDto> CrearAsync(AsignarEmpresaAAplicacionDto dto)
    {
        if (await _aplicacionRepository.GetByIdAsync(dto.AplicacionId) is null)
        {
            throw new NotFoundException("La aplicación indicada no existe.");
        }

        if (await _empresaRepository.GetByIdAsync(dto.EmpresaId) is null)
        {
            throw new NotFoundException("La empresa indicada no existe.");
        }

        if (await _repository.ExistsAsync(dto.AplicacionId, dto.EmpresaId))
        {
            throw new BadRequestException("Esta empresa ya está asociada a la aplicación.");
        }

        var entity = new IntgAplicacionEmpresa
        {
            AplicacionId = dto.AplicacionId,
            EmpresaId = dto.EmpresaId
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var creado = await _repository.GetByIdAsync(entity.Id)
            ?? throw new InvalidOperationException("No fue posible recuperar la relación recién creada.");
        return ToDto(creado);
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is null) return false;

        entity.Estado = activo;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static AplicacionEmpresaDto ToDto(IntgAplicacionEmpresa entity) => new()
    {
        Id = entity.Id,
        AplicacionId = entity.AplicacionId,
        NombreAplicacion = entity.Aplicacion?.Nombre,
        CodigoAplicacion = entity.Aplicacion?.CodigoApp,
        EmpresaId = entity.EmpresaId,
        NombreEmpresa = entity.Empresa?.NombreRazonSocial,
        Estado = entity.Estado
    };
}
