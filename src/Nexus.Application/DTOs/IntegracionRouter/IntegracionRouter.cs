namespace Nexus.Application.DTOs.IntegracionRouter
{
    public record EjecutarIntegracionRequest(
    string CodigoAplicacion,
    string CodigoIntegracion,
    int CodigoEmpresa,
    dynamic Payload,
    IDictionary<string, string>? queryParams = null
    );

	public record EjecutarIntegracionResponse
	{
		public bool Exitoso { get; init; }
		public int Codigo { get; init; }
		public string Mensaje { get; init; } = string.Empty;
		public object? Data { get; init; }
		public string? TransaccionId { get; init; }

		// Constructor base partiendo del estado booleano (Exitoso)
		public EjecutarIntegracionResponse(bool exitoso, string mensaje, object? data = null, string? transaccionId = null)
		{
			Exitoso = exitoso;
			Codigo = exitoso ? 0 : 1;
			Mensaje = mensaje;
			Data = data;
			TransaccionId = transaccionId;
		}

		// Constructor alternativo partiendo del entero (Codigo)
		public EjecutarIntegracionResponse(int codigo, string mensaje, object? data = null, string? transaccionId = null)
		{
			Codigo = codigo;
			Exitoso = codigo == 0;
			Mensaje = mensaje;
			Data = data;
			TransaccionId = transaccionId;
		}
	}
}
