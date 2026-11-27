using Microsoft.AspNetCore.Mvc.Rendering;

// Listado de vuelos con los filtros de la URL (?DestinoId=…&Desde=…) y las opciones del select de destino
public class VuelosIndexViewModel
{
    public int? DestinoId { get; set; }
    public DateTime? Desde { get; set; }
    public List<Vuelo> Vuelos { get; set; } = [];
    public List<SelectListItem> Destinos { get; set; } = [];
}
