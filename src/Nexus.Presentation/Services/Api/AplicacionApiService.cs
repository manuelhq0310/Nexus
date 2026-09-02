using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

public class AplicacionApiService : ApiServiceBase
{
    public AplicacionApiService(HttpClient http) : base(http) { }

    public async Task<List<AplicacionDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/Aplicaciones?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<AplicacionDto>>(response);
    }

    public async Task<AplicacionDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/Aplicaciones/{id}");
        return await ReadOrThrowNullableAsync<AplicacionDto>(response);
    }

    public async Task<AplicacionDto?> ObtenerPorCodigoAsync(string codigoApp)
    {
        var response = await Http.GetAsync($"api/v1/Aplicaciones/codigo/{Uri.EscapeDataString(codigoApp)}");
        return await ReadOrThrowNullableAsync<AplicacionDto>(response);
    }

    public async Task<AplicacionDto> CrearAsync(CrearAplicacionDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/Aplicaciones", dto);
        return await ReadOrThrowAsync<AplicacionDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarAplicacionDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/Aplicaciones/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/Aplicaciones/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
