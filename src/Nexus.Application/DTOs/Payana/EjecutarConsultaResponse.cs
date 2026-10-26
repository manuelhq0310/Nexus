namespace Nexus.Application.DTOs.Payana
{
	public record EjecutarConsultaResponse(
		int Codigo,
		string Mensaje,
		object? Detalle
	);
}
