using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>Gestiona la relación Integración &lt;-&gt; Conector (qué conector resuelve qué integración).</summary>
public class IntegracionConectorApiService : ApiServiceBase
{
    public IntegracionConectorApiService(HttpClient http) : base(http) { }

    public async Task<IntegracionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/IntegracionConectores/{id}");
        return await ReadOrThrowNullableAsync<IntegracionConectorDto>(response);
    }

    /// <summary>Conectores ya configurados para resolver una integración específica.</summary>
    public async Task<List<IntegracionConectorDto>> ObtenerPorIntegracionAsync(long integracionId)
    {
        var response = await Http.GetAsync($"api/v1/IntegracionConectores/integracion/{integracionId}");
        return await ReadOrThrowAsync<List<IntegracionConectorDto>>(response);
    }

    public async Task<List<IntegracionConectorDto>> ObtenerPorConectorAsync(long conectorId)
    {
        var response = await Http.GetAsync($"api/v1/IntegracionConectores/conector/{conectorId}");
        return await ReadOrThrowAsync<List<IntegracionConectorDto>>(response);
    }

    public async Task<IntegracionConectorDto> CrearAsync(CrearIntegracionConectorDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/IntegracionConectores", dto);
        return await ReadOrThrowAsync<IntegracionConectorDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarIntegracionConectorDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/IntegracionConectores/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/IntegracionConectores/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
