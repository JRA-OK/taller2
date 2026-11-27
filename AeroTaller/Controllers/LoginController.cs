using Microsoft.AspNetCore.Mvc;

public class LoginController(IAutenticacionService autenticacion) : Controller
{
    // GET /Login?returnUrl=/Reservas
    public IActionResult Index(string? returnUrl) => View(new LoginViewModel { ReturnUrl = returnUrl });

    // POST /Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(LoginViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        if (autenticacion.IniciarSesion(modelo.NombreUsuario!, modelo.Clave!) is null)
        {
            // Mensaje genérico (clave "" = error del modelo, va al resumen): no revelar si el usuario existe
            ModelState.AddModelError(string.Empty, "Usuario o clave incorrectos.");
            return View(modelo);
        }

        // Solo URLs locales: evita el "open redirect" (/Login?returnUrl=https://sitio-malicioso)
        if (Url.IsLocalUrl(modelo.ReturnUrl)) return LocalRedirect(modelo.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    // POST /Login/Salir  →  POST + antiforgery: un link o una imagen de otro sitio no pueden cerrar la sesión
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Salir()
    {
        autenticacion.CerrarSesion();
        return RedirectToAction("Index", "Home");
    }
}
