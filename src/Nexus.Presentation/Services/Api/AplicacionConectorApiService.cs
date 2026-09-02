using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>Gestiona qué Conectores puede usar una Aplicación, con sus credenciales de autenticación (usuario/password del ERP, API key, token).</summary>
public class AplicacionConectorApiService : ApiServiceBase
{
    public AplicacionConectorApiService(HttpClient http) : base(http) { }

    public async Task<List<AplicacionConectorDto>> ObtenerPorAplicacionAsync(long aplicacionId)
    {
        var response = await Http.GetAsync($"api/v1/AplicacionConectores/aplicacion/{aplicacionId}");
        return await ReadOrThrowAsync<List<AplicacionConectorDto>>(response);
    }

    public async Task<AplicacionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/AplicacionConectores/{id}");
        return await ReadOrThrowNullableAsync<AplicacionConectorDto>(response);
    }

    public async Task<AplicacionConectorDto> CrearAsync(CrearAplicacionConectorDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/AplicacionConectores", dto);
        return await ReadOrThrowAsync<AplicacionConectorDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarAplicacionConectorDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/AplicacionConectores/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        // A diferencia de los demás endpoints "estado", este espera un booleano crudo en el body (no { activo }).
        var response = await Http.PatchAsJsonAsync($"api/v1/AplicacionConectores/{id}/estado", activo);
        await EnsureSuccessAsync(response);
    }
}
