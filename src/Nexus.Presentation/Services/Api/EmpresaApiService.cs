using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

public class EmpresaApiService : ApiServiceBase
{
    public EmpresaApiService(HttpClient http) : base(http) { }

    public async Task<List<EmpresaDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/Empresas?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<EmpresaDto>>(response);
    }

    public async Task<EmpresaDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/Empresas/{id}");
        return await ReadOrThrowNullableAsync<EmpresaDto>(response);
    }

    public async Task<EmpresaDto?> ObtenerPorIdentificacionAsync(string tipoIdentificacion, string numeroIdentificacion)
    {
        var response = await Http.GetAsync(
            $"api/v1/Empresas/buscar?tipoIdentificacion={Uri.EscapeDataString(tipoIdentificacion)}&numeroIdentificacion={Uri.EscapeDataString(numeroIdentificacion)}");
        return await ReadOrThrowNullableAsync<EmpresaDto>(response);
    }

    public async Task<EmpresaDto> CrearAsync(CrearEmpresaDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/Empresas", dto);
        return await ReadOrThrowAsync<EmpresaDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarEmpresaDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/Empresas/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/Empresas/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
