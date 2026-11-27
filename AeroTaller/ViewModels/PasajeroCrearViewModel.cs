using System.ComponentModel.DataAnnotations;

public class PasajeroCrearViewModel
{
    [Required(ErrorMessage = "Ingresá el nombre.")]
    [StringLength(100, ErrorMessage = "El nombre admite hasta {1} caracteres.")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "Ingresá el DNI.")]
    [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos, sin puntos.")]
    [Display(Name = "DNI")]
    public string? Dni { get; set; }

    // Opcional: un input vacío llega como null y [EmailAddress] no lo evalúa
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }
}
