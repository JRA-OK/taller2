using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

// IValidatableObject: reglas que comparan dos campos del mismo formulario (origen ≠ destino).
// Validate solo se ejecuta si los atributos de todas las propiedades pasaron.
public class VueloCrearViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Ingresá el número de vuelo.")]
    [RegularExpression("^AT[0-9]{4}$", ErrorMessage = "El número debe ser AT seguido de 4 dígitos (AT1234).")]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [Required(ErrorMessage = "Elegí el origen.")]
    [Display(Name = "Origen")]
    public int? OrigenId { get; set; }

    [Required(ErrorMessage = "Elegí el destino.")]
    [Display(Name = "Destino")]
    public int? DestinoId { get; set; }

    [Required(ErrorMessage = "Ingresá la fecha y hora de salida.")]
    [FechaFutura(ErrorMessage = "La salida debe ser una fecha futura.")]
    public DateTime? Salida { get; set; }

    [Required(ErrorMessage = "Ingresá la duración.")]
    [Range(10, 1440, ErrorMessage = "La duración debe estar entre {1} y {2} minutos.")]
    [Display(Name = "Duración (minutos)")]
    public int? DuracionMinutos { get; set; }

    [Required(ErrorMessage = "Ingresá la cantidad de filas.")]
    [Range(1, 60, ErrorMessage = "El avión debe tener entre {1} y {2} filas.")]
    public int? Filas { get; set; }

    [Required(ErrorMessage = "Ingresá el precio.")]
    [Range(typeof(decimal), "1", "100000000", ErrorMessage = "El precio debe ser mayor que cero.")]
    public decimal? Precio { get; set; }

    public List<SelectListItem> Aeropuertos { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (OrigenId == DestinoId)
            yield return new ValidationResult("El destino debe ser distinto del origen.", [nameof(DestinoId)]);
    }
}
