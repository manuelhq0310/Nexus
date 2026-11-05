namespace Nexus.Application.DTOs.IntegracionRouter
{
	public record EjecutarConsultaResponse(
		int Codigo,
		string Mensaje,
		object? Detalle
	);
}
