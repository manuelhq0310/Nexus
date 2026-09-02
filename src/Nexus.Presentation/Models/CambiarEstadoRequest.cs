namespace Nexus.Presentation.Models;

/// <summary>
/// Cuerpo compartido para los endpoints PATCH .../estado de Empresas, Conectores,
/// Integraciones e IntegracionConectores (todos comparten la misma forma: { activo }).
/// </summary>
public class CambiarEstadoRequest
{
    public bool Activo { get; set; }
}
