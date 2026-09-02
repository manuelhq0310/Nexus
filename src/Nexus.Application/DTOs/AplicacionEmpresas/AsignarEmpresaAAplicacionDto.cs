using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.AplicacionEmpresas;

public class AsignarEmpresaAAplicacionDto
{
    [Required(ErrorMessage = "La aplicación es obligatoria.")]
    public long AplicacionId { get; set; }

    [Required(ErrorMessage = "La empresa es obligatoria.")]
    public long EmpresaId { get; set; }
}
