using Microsoft.AspNetCore.Mvc;

public class HomeController(IVueloRepository vuelos, IReservaRepository reservas) : Controller
{
    // GET /  →  tablero: vuelos que todavía no salieron, con su ocupación
    public IActionResult Index()
    {
        var ocupacion = reservas.ContarConfirmadasPorVuelo();
        var tablero = vuelos.ObtenerTodos(desde: DateTime.Now)
            .Select(v => new VueloOcupacionViewModel(v, ocupacion.GetValueOrDefault(v.Id)))
            .ToList();
        return View(tablero);
    }

    // /Home/Error  →  la ejecuta UseExceptionHandler ante una excepción no controlada.
    // Sin [HttpGet]: se re-ejecuta con el método original (un POST que falló llega como POST).
    // El código 500 ya lo fijó el middleware, que además registró la excepción completa en el log.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel(HttpContext.TraceIdentifier));
}
