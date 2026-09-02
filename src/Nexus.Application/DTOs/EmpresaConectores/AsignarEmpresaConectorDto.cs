using System.ComponentModel.DataAnnotations;

namespace Nexus.Application.DTOs.EmpresaConectores;

public class AsignarEmpresaConectorDto
{
    [Required(ErrorMessage = "La empresa es obligatoria.")]
    public long EmpresaId { get; set; }

    [Required(ErrorMessage = "El conector es obligatorio.")]
    public long ConectorId { get; set; }
}
