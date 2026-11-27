using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

// Formulario de nueva reserva. Solo VueloId, PasajeroId y Asiento viajan en el POST;
// las listas de los select se recargan en el servidor cada vez que se muestra la vista.
public class ReservaCrearViewModel
{
    // int? + [Required]: con int a secas, un select sin elegir llegaría como 0 y pasaría la validación
    [Required(ErrorMessage = "Elegí un vuelo.")]
    [Display(Name = "Vuelo")]
    public int? VueloId { get; set; }

    [Required(ErrorMessage = "Elegí un pasajero.")]
    [Display(Name = "Pasajero")]
    public int? PasajeroId { get; set; }

    // El formato se valida también aquí para tener validación en el navegador; acepta minúsculas ('5b'),
    // que ReservaService normaliza. Fila existente y asiento libre necesitan la base: van en el servicio.
    [Required(ErrorMessage = "Indicá el asiento.")]
    [RegularExpression(@"^\s*[1-9][0-9]*[A-Fa-f]\s*$", ErrorMessage = "El asiento debe ser fila + letra de la A a la F (por ejemplo 12C).")]
    public string? Asiento { get; set; }

    public List<SelectListItem> Vuelos { get; set; } = [];
    public List<SelectListItem> Pasajeros { get; set; } = [];
}
