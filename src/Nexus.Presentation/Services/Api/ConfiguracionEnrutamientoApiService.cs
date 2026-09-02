using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>
/// Gestiona la matriz final de enrutamiento: qué Empresa usa qué combinación
/// Integración-Conector (endpoint /api/v1/ConfiguracionEnrutamiento del backend).
/// </summary>
public class ConfiguracionEnrutamientoApiService : ApiServiceBase
{
    public ConfiguracionEnrutamientoApiService(HttpClient http) : base(http) { }

    public async Task<List<EmpresaIntegracionConectorDto>> ObtenerPorEmpresaAsync(long empresaId)
    {
        var response = await Http.GetAsync($"api/v1/ConfiguracionEnrutamiento/empresa/{empresaId}");
        return await ReadOrThrowAsync<List<EmpresaIntegracionConectorDto>>(response);
    }

    public async Task<EmpresaIntegracionConectorDto?> ObtenerPorIdAsync(long id)
    {
        var response = await Http.GetAsync($"api/v1/ConfiguracionEnrutamiento/{id}");
        return await ReadOrThrowNullableAsync<EmpresaIntegracionConectorDto>(response);
    }

    public async Task<EmpresaIntegracionConectorDto> CrearAsync(CrearEmpresaIntegracionConectorDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/ConfiguracionEnrutamiento", dto);
        return await ReadOrThrowAsync<EmpresaIntegracionConectorDto>(response);
    }

    public async Task ActualizarAsync(long id, ActualizarEmpresaIntegracionConectorDto dto)
    {
        var response = await Http.PutAsJsonAsync($"api/v1/ConfiguracionEnrutamiento/{id}", dto);
        await EnsureSuccessAsync(response);
    }

    /// <summary>Resuelve en caliente la URL/cola final para una empresa + código de acción.</summary>
    public async Task<RutaEnrutamientoResueltaDto?> ResolverAsync(long empresaId, string codigoAccion)
    {
        var response = await Http.GetAsync(
            $"api/v1/ConfiguracionEnrutamiento/resolver?empresaId={empresaId}&codigoAccion={Uri.EscapeDataString(codigoAccion)}");
        return await ReadOrThrowNullableAsync<RutaEnrutamientoResueltaDto>(response);
    }
}
