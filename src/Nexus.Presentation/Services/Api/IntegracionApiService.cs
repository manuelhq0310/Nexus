using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

public class IntegracionApiService : ApiServiceBase
{
    public IntegracionApiService(HttpClient http) : base(http) { }

    public async Task<List<IntegracionDto>> ObtenerTodasAsync(bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/Integraciones?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<IntegracionDto>>(response);
    }

    public async Task<IntegracionDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/Integraciones/{id}");
        return await ReadOrThrowNullableAsync<IntegracionDto>(response);
    }

    public async Task<IntegracionDto?> ObtenerPorCodigoAccionAsync(string codigoAccion)
    {
        var response = await Http.GetAsync($"api/v1/Integraciones/codigo/{Uri.EscapeDataString(codigoAccion)}");
        return await ReadOrThrowNullableAsync<IntegracionDto>(response);
    }

    public async Task<IntegracionDto> CrearAsync(CrearIntegracionDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/Integraciones", dto);
        return await ReadOrThrowAsync<IntegracionDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarIntegracionDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/Integraciones/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/Integraciones/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
