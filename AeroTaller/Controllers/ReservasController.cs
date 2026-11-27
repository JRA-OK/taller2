using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[RequiereLogin]   // en la clase: vale para todas las acciones
public class ReservasController(IReservaService servicio, IReservaRepository reservas, IVueloRepository vuelos, IPasajeroRepository pasajeros) : Controller
{
    // GET /Reservas
    public IActionResult Index() => View(reservas.ObtenerTodas());

    // GET /Reservas/Crear?vueloId=2&asiento=4C  →  desde el mapa de asientos llega todo precargado
    public IActionResult Crear(int? vueloId, string? asiento)
    {
        var modelo = new ReservaCrearViewModel { VueloId = vueloId, Asiento = asiento };
        CargarListas(modelo);
        return View(modelo);
    }

    // POST /Reservas/Crear  →  éxito: Redirect (PRG); error: la misma vista con el error junto a su campo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(ReservaCrearViewModel modelo)
    {
        if (ModelState.IsValid)
        {
            try
            {
                Reserva reserva = servicio.Crear(modelo.VueloId!.Value, modelo.PasajeroId!.Value, modelo.Asiento!);
                TempData["Mensaje"] = $"Reserva confirmada: {reserva.Pasajero.Nombre}, vuelo {reserva.Vuelo.Numero}, asiento {reserva.Asiento}.";
                return RedirectToAction(nameof(Index));
            }
            catch (ReglaDeNegocioException ex)
            {
                ModelState.AddModelError(ex.Campo, ex.Message);
            }
        }
        CargarListas(modelo);
        return View(modelo);
    }

    // POST /Reservas/Cancelar/5  →  POST y no GET: cambia datos (un link o un buscador no deben poder cancelar)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id)
    {
        try
        {
            Reserva? reserva = servicio.Cancelar(id);
            if (reserva is null) return NotFound();
            TempData["Mensaje"] = $"Reserva cancelada: {reserva.Pasajero.Nombre}, vuelo {reserva.Vuelo.Numero}, asiento {reserva.Asiento} (queda libre).";
        }
        catch (ReglaDeNegocioException ex)
        {
            TempData["Error"] = ex.Message;   // no hay formulario donde mostrarlo: se informa en el listado
        }
        return RedirectToAction(nameof(Index));
    }

    // Las opciones de los select no viajan en el POST: se recargan cada vez que se muestra el formulario
    private void CargarListas(ReservaCrearViewModel modelo)
    {
        modelo.Vuelos = vuelos.ObtenerTodos(desde: DateTime.Now)
            .Select(v => new SelectListItem($"{v.Numero} · {v.Origen.Codigo} → {v.Destino.Codigo} · {v.Salida:dd/MM HH:mm}", v.Id.ToString()))
            .ToList();
        modelo.Pasajeros = pasajeros.ObtenerTodos()
            .Select(p => new SelectListItem($"{p.Nombre} (DNI {p.Dni})", p.Id.ToString()))
            .ToList();
    }
}
