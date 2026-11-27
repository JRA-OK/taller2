using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Ingresá el usuario.")]
    [Display(Name = "Usuario")]
    public string? NombreUsuario { get; set; }

    [Required(ErrorMessage = "Ingresá la clave.")]
    [DataType(DataType.Password)]   // el tag helper genera type="password" y nunca devuelve el valor a la vista
    public string? Clave { get; set; }

    // Página a la que volver después del login (la que pidió RequiereLogin); viaja en un campo oculto
    public string? ReturnUrl { get; set; }
}
