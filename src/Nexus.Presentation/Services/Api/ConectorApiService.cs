using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

public class ConectorApiService : ApiServiceBase
{
    public ConectorApiService(HttpClient http) : base(http) { }

    public async Task<List<ConectorDto>> ObtenerTodosAsync(bool soloActivos = true)
    {
        var response = await Http.GetAsync($"api/v1/Conectores?soloActivos={soloActivos}");
        return await ReadOrThrowAsync<List<ConectorDto>>(response);
    }

    public async Task<ConectorDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/Conectores/{id}");
        return await ReadOrThrowNullableAsync<ConectorDto>(response);
    }

    public async Task<ConectorDto> CrearAsync(CrearConectorDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/Conectores", dto);
        return await ReadOrThrowAsync<ConectorDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarConectorDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/Conectores/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/Conectores/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
