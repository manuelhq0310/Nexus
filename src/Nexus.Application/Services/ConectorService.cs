using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.Conectores;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class ConectorService : IConectorService
{
    private readonly IConectorRepository _repository;
    public ConectorService(IConectorRepository repository) => _repository = repository;

    public async Task<IEnumerable<ConectorDto>> ObtenerTodosAsync(bool soloActivos = true)
    {
        var conectores = await _repository.GetAllAsync(soloActivos);
        return conectores.Select(ToDto);
    }

    public async Task<ConectorDto?> ObtenerPorIdAsync(long id)
    {
        var conector = await _repository.GetByIdAsync(id);
        return conector is null ? null : ToDto(conector);
    }

    public async Task<ConectorDto> CrearAsync(CrearConectorDto dto)
    {
        if (await _repository.ExistsByNombreAsync(dto.Nombre))
        {
            throw new BadRequestException("Ya existe un conector registrado con ese nombre.");
        }

        var conector = new IntgConector
        {
            Nombre = dto.Nombre,
            TipoProtocolo = dto.TipoProtocolo,
            UrlBase = dto.UrlBase
        };

        await _repository.AddAsync(conector);
        await _repository.SaveChangesAsync();

        return ToDto(conector);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarConectorDto dto)
    {
        var conector = await _repository.GetByIdAsync(id);
        if (conector is null) return false;

        if (await _repository.ExistsByNombreAsync(dto.Nombre, excludeId: id))
        {
            throw new BadRequestException("Ya existe otro conector registrado con ese nombre.");
        }

        conector.Nombre = dto.Nombre;
        conector.TipoProtocolo = dto.TipoProtocolo;
        conector.UrlBase = dto.UrlBase;
        conector.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var conector = await _repository.GetByIdAsync(id);
        if (conector is null) return false;

        conector.Estado = activo;
        conector.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static ConectorDto ToDto(IntgConector conector) => new()
    {
        Id = conector.Id,
        Nombre = conector.Nombre,
        TipoProtocolo = conector.TipoProtocolo,
        UrlBase = conector.UrlBase,
        Estado = conector.Estado
    };
}
