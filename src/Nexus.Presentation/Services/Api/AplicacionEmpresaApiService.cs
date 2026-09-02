using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>Gestiona qué Empresas utilizan una Aplicación.</summary>
public class AplicacionEmpresaApiService : ApiServiceBase
{
    public AplicacionEmpresaApiService(HttpClient http) : base(http) { }

    public async Task<List<AplicacionEmpresaDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/AplicacionEmpresas/aplicacion/{aplicacionId}?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<AplicacionEmpresaDto>>(response);
    }

    public async Task<List<AplicacionEmpresaDto>> ObtenerPorEmpresaAsync(long empresaId, bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/AplicacionEmpresas/empresa/{empresaId}?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<AplicacionEmpresaDto>>(response);
    }

    public async Task<AplicacionEmpresaDto> CrearAsync(AsignarEmpresaAAplicacionDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/AplicacionEmpresas", dto);
        return await ReadOrThrowAsync<AplicacionEmpresaDto>(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/AplicacionEmpresas/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
