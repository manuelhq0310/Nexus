using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>Gestiona qué Integraciones puede ejecutar una Aplicación.</summary>
public class AplicacionIntegracionApiService : ApiServiceBase
{
    public AplicacionIntegracionApiService(HttpClient http) : base(http) { }

    public async Task<List<AplicacionIntegracionDto>> ObtenerPorAplicacionAsync(long aplicacionId, bool soloActivas = true)
    {
        var response = await Http.GetAsync($"api/v1/AplicacionIntegraciones/aplicacion/{aplicacionId}?soloActivas={soloActivas}");
        return await ReadOrThrowAsync<List<AplicacionIntegracionDto>>(response);
    }

    public async Task<AplicacionIntegracionDto> CrearAsync(CrearAplicacionIntegracionDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/AplicacionIntegraciones", dto);
        return await ReadOrThrowAsync<AplicacionIntegracionDto>(response);
    }

    public async Task CambiarEstadoAsync(long aplicacionId, long integracionId, bool activo)
    {
        var response = await Http.PatchAsJsonAsync(
            $"api/v1/AplicacionIntegraciones/{aplicacionId}/{integracionId}/estado",
            new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
