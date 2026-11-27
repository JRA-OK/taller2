using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class VuelosController(IVueloService servicio, IVueloRepository vuelos, IAeropuertoRepository aeropuertos, IReservaRepository reservas) : Controller
{
    // GET /Vuelos?DestinoId=5&Desde=2026-12-01  →  ambos filtros son opcionales
    public IActionResult Index(int? destinoId, DateTime? desde)
    {
        var modelo = new VuelosIndexViewModel
        {
            DestinoId = destinoId,
            Desde = desde,
            Vuelos = vuelos.ObtenerTodos(destinoId, desde),
            Destinos = OpcionesAeropuertos()
        };
        return View(modelo);
    }

    // GET /Vuelos/Detalle/5  →  404 si el vuelo no existe
    public IActionResult Detalle(int id)
    {
        Vuelo? vuelo = vuelos.ObtenerPorId(id);
        if (vuelo is null) return NotFound();
        return View(new VueloDetalleViewModel(vuelo, reservas.ObtenerAsientosOcupados(id).ToHashSet()));
    }

    // Index y Detalle son públicos (sin atributo); crear vuelos, solo Admin
    // GET /Vuelos/Crear
    [RequiereRol(Roles.Admin)]
    public IActionResult Crear() => View(new VueloCrearViewModel { Aeropuertos = OpcionesAeropuertos() });

    // POST /Vuelos/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol(Roles.Admin)]   // también en el POST: proteger solo el GET no impide enviar el formulario a mano
    public IActionResult Crear(VueloCrearViewModel modelo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                int id = servicio.Crear(new Vuelo
                {
                    Numero = modelo.Numero!,
                    Origen = new Aeropuerto { Id = modelo.OrigenId!.Value },
                    Destino = new Aeropuerto { Id = modelo.DestinoId!.Value },
                    Salida = modelo.Salida!.Value,
                    DuracionMinutos = modelo.DuracionMinutos!.Value,
                    Filas = modelo.Filas!.Value,
                    Precio = modelo.Precio!.Value
                });
                TempData["Mensaje"] = $"Vuelo {modelo.Numero} creado.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            catch (ReglaDeNegocioException ex)
            {
                ModelState.AddModelError(ex.Campo, ex.Message);
            }
        }
        modelo.Aeropuertos = OpcionesAeropuertos();
        return View(modelo);
    }

    private List<SelectListItem> OpcionesAeropuertos() => aeropuertos.ObtenerTodos()
        .Select(a => new SelectListItem($"{a.Ciudad} ({a.Codigo})", a.Id.ToString()))
        .ToList();
}
