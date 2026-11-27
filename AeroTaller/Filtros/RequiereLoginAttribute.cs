using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// [RequiereLogin] en un controlador o una acción: sin sesión, redirige a /Login?returnUrl=<página pedida>.
// Es un filtro de autorización: corre antes del model binding y de la acción, así que la acción ni se ejecuta.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiereLoginAttribute : Attribute, IAuthorizationFilter
{
    public virtual void OnAuthorization(AuthorizationFilterContext context)
    {
        // Los atributos no reciben dependencias por constructor: el servicio se pide al contenedor del request
        var autenticacion = context.HttpContext.RequestServices.GetRequiredService<IAutenticacionService>();
        if (!autenticacion.EstaAutenticado)
            context.Result = RedirigirAlLogin(context);
    }

    // Asignar context.Result "cortocircuita" el pipeline: MVC devuelve ese resultado en lugar de ejecutar la acción
    protected static IActionResult RedirigirAlLogin(AuthorizationFilterContext context)
    {
        var pedido = context.HttpContext.Request;
        string returnUrl = pedido.PathBase + pedido.Path + pedido.QueryString;
        return new RedirectToActionResult("Index", "Login", new { returnUrl });
    }
}
