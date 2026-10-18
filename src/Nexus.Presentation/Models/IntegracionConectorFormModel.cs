namespace Nexus.Presentation.Models;

/// <summary>
/// Modelo de formulario compartido para habilitar/editar un conector en una integración.
/// Las reglas dependen de la integración padre:
///  - EsAsincrono = true  -> ColaRabbitMQDestino obligatoria (RutaEndpoint opcional).
///  - EsAsincrono = false -> RutaEndpoint obligatoria (ColaRabbitMQDestino opcional).
///  - Tipo = Consulta     -> se puede marcar EsConsultaGenerica; en ese caso se debe
///                           asociar una consulta UnoE (UnoEConsultaConfig).
/// </summary>
public class IntegracionConectorFormModel
{
    public long? ConectorId { get; set; }
    public string RutaEndpoint { get; set; } = string.Empty;
    public string ColaRabbitMQDestino { get; set; } = string.Empty;
    public bool EsConsultaGenerica { get; set; }
    public long? UnoEConsultaConfigId { get; set; }

    public static IntegracionConectorFormModel Desde(IntegracionConectorDto dto) => new()
    {
        ConectorId = dto.ConectorId,
        RutaEndpoint = dto.RutaEndpoint ?? string.Empty,
        ColaRabbitMQDestino = dto.ColaRabbitMQDestino ?? string.Empty,
        EsConsultaGenerica = dto.EsConsultaGenerica,
        UnoEConsultaConfigId = dto.UnoEConsultaConfigId
    };

    /// <summary>Devuelve un mensaje de error o null si el formulario es válido.</summary>
    public string? Validar(TipoIntegracion? tipo, bool esAsincrono, bool requiereConector)
    {
        if (requiereConector && ConectorId is null)
            return "Selecciona un conector.";

        if (esAsincrono && string.IsNullOrWhiteSpace(ColaRabbitMQDestino))
            return "La integración es asíncrona: indica la cola de RabbitMQ destino.";

        if (!esAsincrono && string.IsNullOrWhiteSpace(RutaEndpoint))
            return "La integración es síncrona: indica la ruta del endpoint.";

        if (EsConsultaGenerica && tipo == TipoIntegracion.Consulta && UnoEConsultaConfigId is null)
            return "Para una consulta genérica debes asociar una consulta UnoE.";

        return null;
    }

    // Solo aplica a integraciones de tipo Consulta; si no, se envía en false/null.
    private bool EsGenerica(TipoIntegracion? tipo) => tipo == TipoIntegracion.Consulta && EsConsultaGenerica;
    private static string? Nulo(string v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

    public CrearIntegracionConectorDto ACrear(long integracionId, TipoIntegracion? tipo) => new()
    {
        IntegracionId = integracionId,
        ConectorId = ConectorId!.Value,
        RutaEndpoint = Nulo(RutaEndpoint),
        ColaRabbitMQDestino = Nulo(ColaRabbitMQDestino),
        EsConsultaGenerica = EsGenerica(tipo),
        UnoEConsultaConfigId = EsGenerica(tipo) ? UnoEConsultaConfigId : null
    };

    public ActualizarIntegracionConectorDto AActualizar(TipoIntegracion? tipo) => new()
    {
        RutaEndpoint = Nulo(RutaEndpoint),
        ColaRabbitMQDestino = Nulo(ColaRabbitMQDestino),
        EsConsultaGenerica = EsGenerica(tipo),
        UnoEConsultaConfigId = EsGenerica(tipo) ? UnoEConsultaConfigId : null
    };

    public void Limpiar()
    {
        ConectorId = null;
        RutaEndpoint = string.Empty;
        ColaRabbitMQDestino = string.Empty;
        EsConsultaGenerica = false;
        UnoEConsultaConfigId = null;
    }
}
