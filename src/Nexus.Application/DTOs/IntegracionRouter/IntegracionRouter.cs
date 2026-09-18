namespace Nexus.Application.DTOs.IntegracionRouter
{
    public record EjecutarIntegracionRequest(
    string CodigoAplicacion,
    string CodigoIntegracion,
    int CodigoEmpresa,
    dynamic Payload
    );

    public record EjecutarIntegracionResponse(
        bool Exitoso,
        string Mensaje,
        object? Data,
        string? TransaccionId
    );
}
