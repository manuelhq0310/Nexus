using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>Gestiona la relación directa Empresa &lt;-&gt; Conector (qué conector usa cada empresa).</summary>
public class EmpresaConectorApiService : ApiServiceBase
{
    public EmpresaConectorApiService(HttpClient http) : base(http) { }

    /// <summary>
    /// Devuelve el conector asociado a la empresa, o null si no tiene ninguno.
    /// Nota: el backend modela esto como una relación 1 a 1 (una empresa tiene a lo sumo un conector activo a la vez),
    /// no como una lista.
    /// </summary>
    public async Task<EmpresaConectorDto?> ObtenerPorEmpresaAsync(long empresaId)
    {
        var response = await Http.GetAsync($"api/v1/EmpresaConectores/empresa/{empresaId}");
        return await ReadOrThrowNullableAsync<EmpresaConectorDto>(response);
    }

    public async Task<EmpresaConectorDto> CrearAsync(AsignarEmpresaConectorDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/v1/EmpresaConectores", dto);
        return await ReadOrThrowAsync<EmpresaConectorDto>(response);
    }

    /// <summary>Cambia el conector asignado a una relación Empresa-Conector existente.</summary>
    public async Task CambiarConectorAsync(long id, long nuevoConectorId)
    {
        var response = await Http.PutAsync($"api/v1/EmpresaConectores/{id}/conector/{nuevoConectorId}", null);
        await EnsureSuccessAsync(response);
    }

    public async Task CambiarEstadoAsync(long id, bool activo)
    {
        var response = await Http.PatchAsJsonAsync($"api/v1/EmpresaConectores/{id}/estado", new CambiarEstadoRequest { Activo = activo });
        await EnsureSuccessAsync(response);
    }
}
