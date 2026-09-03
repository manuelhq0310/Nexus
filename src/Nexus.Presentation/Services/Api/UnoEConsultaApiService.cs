using System.Net.Http.Json;
using Nexus.Presentation.Models;

namespace Nexus.Presentation.Services.Api;

/// <summary>
/// Gestiona las plantillas de consulta configurables del Conector UnoE
/// (endpoints "/api/unoe/consultas/configuraciones").
///
/// Nota de diseño: la documentación de estos endpoints no especifica el cuerpo de
/// respuesta de POST/PUT/PATCH (solo "200 OK" sin esquema). Para no depender de un
/// contrato no confirmado, después de crear/actualizar se vuelve a consultar el
/// registro por su código — así el dato mostrado en pantalla siempre es el que
/// realmente quedó guardado en el servidor.
/// </summary>
public class UnoEConsultaApiService : ApiServiceBase
{
    public UnoEConsultaApiService(HttpClient http) : base(http) { }

    public async Task<List<UnoEConsultaConfigDto>> ObtenerTodasAsync()
    {
        var response = await Http.GetAsync("api/unoe/consultas/configuraciones");
        return await ReadOrThrowAsync<List<UnoEConsultaConfigDto>>(response);
    }

    public async Task<UnoEConsultaConfigDto?> ObtenerPorCodigoAsync(string codigoConsulta)
    {
        var response = await Http.GetAsync($"api/unoe/consultas/configuraciones/{Uri.EscapeDataString(codigoConsulta)}");
        return await ReadOrThrowNullableAsync<UnoEConsultaConfigDto>(response);
    }

    public async Task<UnoEConsultaConfigDto> CrearAsync(UnoEConsultaConfigDto dto)
    {
        var response = await Http.PostAsJsonAsync("api/unoe/consultas/configuraciones", ToWritePayload(dto));
        await EnsureSuccessAsync(response);

        // El cuerpo de la respuesta no está garantizado; se relee el recurso por su código.
        return await ObtenerPorCodigoAsync(dto.CodigoConsulta)
            ?? throw new ApiException("La consulta se creó, pero no fue posible recuperarla inmediatamente después.", 200);
    }

    public async Task<UnoEConsultaConfigDto> ActualizarAsync(UnoEConsultaConfigDto dto)
    {
        var response = await Http.PutAsJsonAsync("api/unoe/consultas/configuraciones", ToWritePayload(dto));
        await EnsureSuccessAsync(response);

        return await ObtenerPorCodigoAsync(dto.CodigoConsulta)
            ?? throw new ApiException("La consulta se actualizó, pero no fue posible recuperarla inmediatamente después.", 200);
    }

    public async Task CambiarEstadoAsync(string codigoConsulta, bool estado)
    {
        var response = await Http.PatchAsync(
            $"api/unoe/consultas/configuraciones/{Uri.EscapeDataString(codigoConsulta)}/cambiar-estado?estado={estado.ToString().ToLowerInvariant()}",
            content: null);
        await EnsureSuccessAsync(response);
    }

    /// <summary>
    /// Construye el payload que realmente se envía al servidor, excluyendo los campos que
    /// gestiona el propio backend (Id, CreatedAt, UpdatedAt). Enviar "createdAt": null provoca
    /// un 400: del lado del servidor esa propiedad es un DateTime NO anulable, y json null no
    /// se puede convertir a DateTime durante el model binding.
    /// </summary>
    private static UnoEConsultaConfigWritePayload ToWritePayload(UnoEConsultaConfigDto dto) => new()
    {
        CodigoConsulta = dto.CodigoConsulta,
        Descripcion = dto.Descripcion,
        NombreConexion = dto.NombreConexion,
        IdProveedor = dto.IdProveedor,
        PlantillaParametrosXml = dto.PlantillaParametrosXml,
        Estado = dto.Estado
    };

    private class UnoEConsultaConfigWritePayload
    {
        public string CodigoConsulta { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? NombreConexion { get; set; }
        public string? IdProveedor { get; set; }
        public string? PlantillaParametrosXml { get; set; }
        public bool Estado { get; set; }
    }
}
