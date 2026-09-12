namespace Nexus.Application.DTOs.IntegracionRouter
{
    public record EjecutarIntegracionRequest(
    string CodigoAplicacion,
    string CodigoIntegracion,
    string CodigoEmpresa,
    dynamic Payload
    );

    public record EjecutarIntegracionResponse(
        bool Exitoso,
        string Mensaje,
        object? Data,
        string? TransaccionId
    );
}
