using Microsoft.AspNetCore.Mvc;

[RequiereLogin]   // en la clase: vale para todas las acciones
public class PasajerosController(IPasajeroService servicio, IPasajeroRepository pasajeros, IReservaRepository reservas) : Controller
{
    // GET /Pasajeros?buscar=texto  →  el parámetro llega por model binding desde el query string
    public IActionResult Index(string? buscar)
    {
        ViewData["Buscar"] = buscar;
        return View(pasajeros.ObtenerTodos(buscar));
    }

    // GET /Pasajeros/Detalle/5
    public IActionResult Detalle(int id)
    {
        Pasajero? pasajero = pasajeros.ObtenerPorId(id);
        if (pasajero is null) return NotFound();
        return View(new PasajeroDetalleViewModel(pasajero, reservas.ObtenerPorPasajero(id)));
    }

    // GET /Pasajeros/Crear
    public IActionResult Crear() => View(new PasajeroCrearViewModel());

    // POST /Pasajeros/Crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(PasajeroCrearViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);
        try
        {
            int id = servicio.Crear(new Pasajero
            {
                Nombre = modelo.Nombre!.Trim(),
                Dni = modelo.Dni!,
                Email = modelo.Email?.Trim()
            });
            TempData["Mensaje"] = $"Pasajero {modelo.Nombre!.Trim()} registrado.";
            return RedirectToAction(nameof(Detalle), new { id });
        }
        catch (ReglaDeNegocioException ex)
        {
            ModelState.AddModelError(ex.Campo, ex.Message);
            return View(modelo);
        }
    }

    // POST /Pasajeros/Eliminar/5  →  solo Admin
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequiereRol(Roles.Admin)]
    public IActionResult Eliminar(int id)
    {
        try
        {
            Pasajero? pasajero = servicio.Eliminar(id);
            if (pasajero is null) return NotFound();
            TempData["Mensaje"] = $"Pasajero {pasajero.Nombre} eliminado.";
            return RedirectToAction(nameof(Index));
        }
        catch (ReglaDeNegocioException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Detalle), new { id });
        }
    }
}
