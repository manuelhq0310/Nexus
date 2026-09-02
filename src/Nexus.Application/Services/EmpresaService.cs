using Nexus.Application.Common.Exceptions;
using Nexus.Application.DTOs.Empresas;
using Nexus.Application.Interfaces.Repositories;
using Nexus.Application.Interfaces.Services;
using Nexus.Domain.Entities.Integraciones;

namespace Nexus.Application.Services;

public class EmpresaService : IEmpresaService
{
    private readonly IEmpresaRepository _repository;
    public EmpresaService(IEmpresaRepository repository) => _repository = repository;

    public async Task<IEnumerable<EmpresaDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var empresas = await _repository.GetAllAsync(soloActivas);
        return empresas.Select(ToDto);
    }

    public async Task<EmpresaDto?> ObtenerPorIdAsync(long id)
    {
        var empresa = await _repository.GetByIdAsync(id);
        return empresa is null ? null : ToDto(empresa);
    }

    public async Task<EmpresaDto?> ObtenerPorIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion)
    {
        var empresa = await _repository.GetByIdentificacionAsync(tipoIdentificacion, numeroIdentificacion);
        return empresa is null ? null : ToDto(empresa);
    }

    public async Task<EmpresaDto> CrearAsync(CrearEmpresaDto dto)
    {
        if (await _repository.ExistsByIdentificacionAsync(dto.TipoIdentificacion, dto.NumeroIdentificacion))
        {
            throw new BadRequestException("Ya existe una empresa registrada con ese tipo y número de identificación.");
        }

        var empresa = new IntgEmpresa
        {
            TipoIdentificacion = dto.TipoIdentificacion,
            NumeroIdentificacion = dto.NumeroIdentificacion,
            NombreRazonSocial = dto.NombreRazonSocial
        };

        await _repository.AddAsync(empresa);
        await _repository.SaveChangesAsync();

        return ToDto(empresa);
    }

    public async Task<bool> ActualizarAsync(long id, ActualizarEmpresaDto dto)
    {
        var empresa = await _repository.GetByIdAsync(id);
        if (empresa is null) return false;

        if (await _repository.ExistsByIdentificacionAsync(dto.TipoIdentificacion, dto.NumeroIdentificacion, excludeId: id))
        {
            throw new BadRequestException("Ya existe otra empresa registrada con ese tipo y número de identificación.");
        }

        empresa.TipoIdentificacion = dto.TipoIdentificacion;
        empresa.NumeroIdentificacion = dto.NumeroIdentificacion;
        empresa.NombreRazonSocial = dto.NombreRazonSocial;
        empresa.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(long id, bool activo)
    {
        var empresa = await _repository.GetByIdAsync(id);
        if (empresa is null) return false;

        empresa.Estado = activo;
        empresa.UpdatedAt = DateTime.UtcNow;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static EmpresaDto ToDto(IntgEmpresa empresa) => new()
    {
        Id = empresa.Id,
        TipoIdentificacion = empresa.TipoIdentificacion,
        NumeroIdentificacion = empresa.NumeroIdentificacion,
        NombreRazonSocial = empresa.NombreRazonSocial,
        Estado = empresa.Estado
    };
}
